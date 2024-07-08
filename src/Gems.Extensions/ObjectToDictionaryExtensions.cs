using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Gems
{
    public static class ObjectToDictionaryExtensions
    {
        private static ConcurrentDictionary<ObjectToDictionaryKey, Action<object, Dictionary<string, object>>> cache = new ConcurrentDictionary<ObjectToDictionaryKey, Action<object, Dictionary<string, object>>>();

        public static Dictionary<string, object> ToDictionary(this object obj, NamingConvention naming = NamingConvention.None)
        {
            var result = new Dictionary<string, object>();
            if (obj != null)
            {
                GetSetter(obj.GetType(), naming)(obj, result);
            }

            return result;
        }

        public static Dictionary<string, object> ToDictionary<T>(this T obj, NamingConvention naming = NamingConvention.None)
        {
            var result = new Dictionary<string, object>();
            if (obj != null)
            {
                GetSetter(typeof(T), naming)(obj, result);
            }

            return result;
        }

        private static Action<object, Dictionary<string, object>> GetSetter(Type type, NamingConvention naming)
        {
            return cache.GetOrAdd(new ObjectToDictionaryKey(type, naming), key =>
            {
                var properties = key.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                var dictType = typeof(Dictionary<,>).MakeGenericType(typeof(string), typeof(object));
                var addMethodInfo = dictType.GetMethod(
                    "Add",
                    new Type[]
                    {
                            typeof(string),
                            typeof(object),
                    });
                var expressionDictParameter = Expression.Parameter(dictType);
                var expressionObjParameter = Expression.Parameter(typeof(object));
                var expressionObjCast = Expression.TypeAs(expressionObjParameter, key.Type);
                var body = new List<Expression>();
                body.AddRange(
                    properties.Select(prop =>
                        Expression.Call(
                            expressionDictParameter,
                            addMethodInfo!,
                            Expression.Constant(key.Naming.Convert(prop.Name)),
                            Expression.TypeAs(Expression.Property(expressionObjCast, prop), typeof(object)))));
                var expressionDelegateBody = Expression.Block(body);
                var expressionDelegate = Expression.Lambda<Action<object, Dictionary<string, object>>>(
                    expressionDelegateBody,
                    expressionObjParameter,
                    expressionDictParameter);
                return expressionDelegate.Compile();
            });
        }
    }
}
