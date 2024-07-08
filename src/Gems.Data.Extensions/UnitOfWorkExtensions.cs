using Gems.Data.UnitOfWork;

namespace Gems.Data
{
    public static class UnitOfWorkExtensions
    {
        public static Task<T> CallScalarFunctionAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallScalarFunctionAsync<T>(functionName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> CallScalarFunctionAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallScalarFunctionAsync<T>(functionName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task CallStoredProcedureAsync(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureAsync(storeProcedureName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task CallStoredProcedureAsync(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureAsync(storeProcedureName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureAsync<T>(storeProcedureName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<IEnumerable<T>> CallStoredProcedureAsync<T>(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureAsync<T>(storeProcedureName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureFirstOrDefaultAsync<T>(storeProcedureName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> CallStoredProcedureFirstOrDefaultAsync<T>(this IUnitOfWork unitOfWork, string storeProcedureName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallStoredProcedureFirstOrDefaultAsync<T>(storeProcedureName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<List<T>> CallTableFunctionAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallTableFunctionAsync<T>(functionName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<List<T>> CallTableFunctionAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallTableFunctionAsync<T>(functionName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> CallTableFunctionFirstAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.CallTableFunctionFirstAsync<T>(functionName, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> CallTableFunctionFirstAsync<T>(this IUnitOfWork unitOfWork, string functionName, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.CallTableFunctionFirstAsync<T>(functionName, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<List<T>> QueryAsync<T>(this IUnitOfWork unitOfWork, string commandText, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.QueryAsync<T>(commandText, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<List<T>> QueryAsync<T>(this IUnitOfWork unitOfWork, string commandText, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.QueryAsync<T>(commandText, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IUnitOfWork unitOfWork, string commandText, object parameters, Enum timeMetricType = null)
        {
            return unitOfWork.QueryFirstOrDefaultAsync<T>(commandText, parameters.ToDictionary(), timeMetricType);
        }

        public static Task<T> QueryFirstOrDefaultAsync<T>(this IUnitOfWork unitOfWork, string commandText, object parameters, int commandTimeout, Enum timeMetricType = null)
        {
            return unitOfWork.QueryFirstOrDefaultAsync<T>(commandText, commandTimeout, parameters.ToDictionary(), timeMetricType);
        }
    }
}
