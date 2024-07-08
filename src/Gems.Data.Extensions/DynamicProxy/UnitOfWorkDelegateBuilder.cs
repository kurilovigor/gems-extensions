using Gems.Extensions.DynamicProxy;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Humanizer;
using System.Collections.Concurrent;

namespace Gems.Data.Extensions.DynamicProxy
{
    internal class UnitOfWorkDelegateBuilder : IDelegateBuilder
    {
        private readonly ConcurrentDictionary<UnitOfWorkDelegateBuilderCacheKey, Delegate> cache = new ConcurrentDictionary<UnitOfWorkDelegateBuilderCacheKey, Delegate>();

        public Delegate Build(MethodInfo methodInfo, Type type)
        {
            return this.cache.GetOrAdd(
                new UnitOfWorkDelegateBuilderCacheKey(methodInfo, type), 
                x => this.InternalBuild(x.Method, x.TargetType));
        }

        private Delegate InternalBuild(MethodInfo methodInfo, Type type)
        {
            var returnTypeInformation = GetReturnTypeInformation(methodInfo);

            // Command timeout.
            ParameterExpression expressionCommandTimeout = null;

            // Sql method.
            var sqlMethodAttribute = methodInfo.GetCustomAttribute<SqlMethodAttribute>();
            var sqlMethod = sqlMethodAttribute?.Method ?? 
                (returnTypeInformation.Cardinality == ReturnTypeCardinality.Void ? SqlMethod.Procedure : SqlMethod.Function);

            // Sql method name.
            var sqlMethodName = string.IsNullOrEmpty(sqlMethodAttribute?.Name) ? 
                this.GetSqlMethodName(methodInfo) : 
                sqlMethodAttribute.Name;

            // Time metric.
            Expression expressionTimeMetric = null;

            var body = new List<Expression>();
            var parameterExpressions = new List<ParameterExpression>();
            var dictType = typeof(Dictionary<string, object>);
            var addMethodInfo = dictType.GetMethod(
                "Add", 
                BindingFlags.Public | BindingFlags.Instance, 
                null, 
                new[] 
                { 
                    typeof(string), 
                    typeof(object) 
                }, 
                null);
            Expression expressionParameterCancellationToken = null;
            var expressionTargetParameter = Expression.Parameter(type);
            parameterExpressions.Add(expressionTargetParameter);
            var requestMemberBindings = new List<MemberBinding>();
            var expressionRequest = Expression.Variable(dictType, "request");

            body.Add(Expression.Assign(expressionRequest, Expression.New(dictType)));

            foreach (var parameter in methodInfo.GetParameters())
            {
                var expressionParameter = Expression.Parameter(parameter.ParameterType, parameter.Name);
                parameterExpressions.Add(expressionParameter);
                if (parameter.ParameterType == typeof(CancellationToken) && expressionParameterCancellationToken == null)
                {
                    expressionParameterCancellationToken = expressionParameter;
                }
                else if (parameter.GetCustomAttribute<TimeMetricAttribute>() != null &&  expressionTimeMetric == null)
                {
                    if (parameter.ParameterType.BaseType != typeof(Enum))
                    {
                        throw new ArgumentException("Time metric should be Enum type");
                    }

                    expressionTimeMetric = Expression.TypeAs(expressionParameter, typeof(Enum));
                }
                else if (parameter.GetCustomAttribute<CommandTimeoutAttribute>() != null && expressionCommandTimeout == null)
                {
                    if (parameter.ParameterType != typeof(int))
                    {
                        throw new ArgumentException("Command timeout should be int");
                    }

                    expressionCommandTimeout = expressionParameter;
                }
                else
                {
                    var paramName = this.GetSqlParameterName(methodInfo, parameter);
                    body.Add(Expression.Call(
                        expressionRequest,
                        addMethodInfo,
                        Expression.Constant(paramName),
                        Expression.Convert(expressionParameter, typeof(object))));
                }
            }

            var expressionUnitOfWorkProvider = Expression.Call(
                Expression.Property(expressionTargetParameter, "UnitOfWorkProvider"), 
                "GetUnitOfWork",
                null,
                Expression.Property(expressionTargetParameter, "Name"),
                expressionParameterCancellationToken ?? Expression.Property(expressionTargetParameter, "CancellationToken"));

            switch (sqlMethod)
            {
                case SqlMethod.Function:
                    switch (returnTypeInformation.Cardinality)
                    {
                        case ReturnTypeCardinality.Void:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallScalarFunctionAsync",
                                typeof(object),
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.Scalar:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallScalarFunctionAsync",
                                returnTypeInformation.ResultType,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.Record:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallTableFunctionFirstAsync",
                                returnTypeInformation.ResultType,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.List:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallTableFunctionAsync",
                                returnTypeInformation.ElementType,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.Enumerable:
                            var enumType = typeof(IEnumerable<>).MakeGenericType(returnTypeInformation.ElementType);
                            body.Add(Expression.TypeAs(
                                this.CallMethod(
                                    expressionUnitOfWorkProvider,
                                    "CallTableFunctionAsync",
                                    returnTypeInformation.ElementType,
                                    sqlMethodName,
                                    expressionRequest,
                                    expressionCommandTimeout,
                                    expressionTimeMetric), 
                                enumType));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case SqlMethod.Procedure:
                    switch (returnTypeInformation.Cardinality)
                    {
                        case ReturnTypeCardinality.Void:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallStoredProcedureAsync",
                                null,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.Scalar:
                        case ReturnTypeCardinality.Record:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallStoredProcedureFirstOrDefaultAsync",
                                returnTypeInformation.ResultType,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        case ReturnTypeCardinality.List:
                            throw new ArgumentException($"Method {methodInfo} has unsupported return type. Use Task<IEnumerable<{returnTypeInformation.ElementType.Name}>> instead");
                        case ReturnTypeCardinality.Enumerable:
                            body.Add(this.CallMethod(
                                expressionUnitOfWorkProvider,
                                "CallStoredProcedureAsync",
                                returnTypeInformation.ElementType,
                                sqlMethodName,
                                expressionRequest,
                                expressionCommandTimeout,
                                expressionTimeMetric));
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            var expressionBody = Expression.Block(
                methodInfo.ReturnType,
                new ParameterExpression[] { expressionRequest },
                body);
            var lambda = Expression.Lambda(expressionBody, parameterExpressions);
            return lambda.Compile();
        }

        private ReturnTypeInformation GetReturnTypeInformation(MethodInfo methodInfo)
        {
            if (!(methodInfo.ReturnType == typeof(Task) ||
                methodInfo.ReturnType?.BaseType == typeof(Task)))
            {
                throw new ArgumentException("Async method expected");
            }
            
            var genericArgumentType = methodInfo.ReturnType.GenericTypeArguments.FirstOrDefault();

            if (genericArgumentType == null)
            {
                return new ReturnTypeInformation 
                {
                    ElementType = null,
                    ResultType = null,
                    Cardinality = ReturnTypeCardinality.Void,
                };
            }
            else 
            {
                var result = new ReturnTypeInformation
                {
                    ResultType = genericArgumentType,
                    ElementType = null,
                };

                if (result.ResultType.IsGenericType && result.ResultType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    result.Cardinality = ReturnTypeCardinality.List;
                    result.ElementType = result.ResultType.GetGenericArguments()[0];
                }
                else if (result.ResultType.IsGenericType && result.ResultType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    result.Cardinality = ReturnTypeCardinality.Enumerable;
                    result.ElementType = result.ResultType.GetGenericArguments()[0];
                }
                else if (result.ResultType.IsValueType)
                {
                    result.Cardinality = ReturnTypeCardinality.Scalar;
                }
                else
                {
                    result.Cardinality = ReturnTypeCardinality.Record;
                }

                return result;
            }
        }

        private MethodCallExpression CallMethod(
            Expression unitOfWorkInstance,
            string methodName,
            Type typeArg,
            string sqlMethodName,
            ParameterExpression request,
            ParameterExpression commandTimeout,
            Expression timeMetric)
        {
            var arguments = new List<Expression>();
            arguments.Add(Expression.Constant(sqlMethodName));
            
            if (commandTimeout != null)
            {
                arguments.Add(commandTimeout);
            }

            arguments.Add(request);
            if(timeMetric == null)
            {
                arguments.Add(Expression.Constant(null, typeof(Enum)));
            }
            else 
            {
                arguments.Add(timeMetric);
            }

            return Expression.Call(
                unitOfWorkInstance,
                methodName,
                typeArg == null ? null : new Type[] { typeArg },
                arguments.ToArray());
        }

        private string GetSqlMethodName(MethodInfo methodInfo)
        {
            var unitOfWorkAttribute = methodInfo.DeclaringType.GetCustomAttribute<UnitOfWorkAttribute>();
            var schema = unitOfWorkAttribute?.Schema;
            var naming = unitOfWorkAttribute?.Naming ?? NamingConvention.None;
            var sqlMethodName = new StringBuilder();

            if (!string.IsNullOrEmpty(schema))
            {
                sqlMethodName.Append(schema);
                sqlMethodName.Append('.');
            }
            var methodName = methodInfo.Name;
            if (methodName.EndsWith("Async", StringComparison.OrdinalIgnoreCase))
            {
                methodName = methodName.Substring(0, methodName.Length - 5);
            }

            var prefix = unitOfWorkAttribute?.MethodPrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                sqlMethodName.Append(prefix);
            }

            switch (naming)
            {
                case NamingConvention.None:
                    sqlMethodName.Append(methodName);
                    break;
                case NamingConvention.CamelCase:
                    sqlMethodName.Append(methodName.Camelize());
                    break;
                case NamingConvention.SnakeCase:
                    sqlMethodName.Append(methodName.Underscore());
                    break;
                case NamingConvention.PascalCase:
                    sqlMethodName.Append(methodName.Pascalize());
                    break;
                case NamingConvention.KebabCase:
                    sqlMethodName.Append(methodName.Dasherize());
                    break;
            }

            var suffix = unitOfWorkAttribute?.MethodSuffix;
            if (!string.IsNullOrEmpty(suffix))
            {
                sqlMethodName.Append(suffix);
            }

            return sqlMethodName.ToString();
        }

        private string GetSqlParameterName(MethodInfo methodInfo, ParameterInfo parameterInfo)
        {
            var paramAttribute = parameterInfo.GetCustomAttribute<SqlParameterAttribute>();
            if (!string.IsNullOrEmpty(paramAttribute?.Name))
            { 
                return paramAttribute?.Name;
            }

            var unitOfWorkAttribute = methodInfo.DeclaringType.GetCustomAttribute<UnitOfWorkAttribute>();
            var naming = unitOfWorkAttribute?.Naming ?? NamingConvention.None;
            var sqlParameterName = new StringBuilder();

            var parameterName = parameterInfo.Name;
            var prefix = unitOfWorkAttribute?.ParameterPrefix;
            if (!string.IsNullOrEmpty(prefix))
            {
                sqlParameterName.Append(prefix);
            }

            sqlParameterName.Append(naming.Convert(parameterName));

            var suffix = unitOfWorkAttribute?.ParameterSuffix;
            if (!string.IsNullOrEmpty(suffix))
            {
                sqlParameterName.Append(suffix);
            }

            return sqlParameterName.ToString();
        }
    }
}
