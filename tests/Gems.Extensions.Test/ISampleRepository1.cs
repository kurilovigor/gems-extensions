using Gems.Data.Extensions.DynamicProxy;

namespace Gems.Extensions.Test
{
    [UnitOfWork("DefaultUnitOfWork", Naming = NamingConvention.SnakeCase, Schema = "samples")]
    public interface ISampleRepository1
    {
        Task<List<int>> GetItemsAsync();

        Task<int> ItemCountAsync();

        Task<int> ItemCountAsync(CancellationToken cancellationToken);

        Task DoWorkAsync(
            string arg0, 
            int arg1, 
            [TimeMetric]SampleMetric metric,
            [CommandTimeout] int commandTimeout,
            CancellationToken cancellationToken);

        [SqlMethod(SqlMethod.Function)]
        Task DoWork2Async([SqlParameter("p_arg0")]string arg0, [SqlParameter("p_arg1")] int arg1, CancellationToken cancellationToken);

        Task<object> GetObjectAsync(string arg0, int arg1, CancellationToken cancellationToken);

        [SqlMethod(SqlMethod.Procedure)]
        Task<int> ProcCountAsync(string argOne, int argTwo, CancellationToken cancellationToken);

        [SqlMethod(SqlMethod.Procedure)]
        Task<Dictionary<string, object>> GetSettingsAsync(string arg0, int arg1, CancellationToken cancellationToken);

        [SqlMethod(SqlMethod.Procedure, "samples.proc_ints")]
        Task<IEnumerable<int>> ProcIntEnumerableAsync(string arg0, int arg1, CancellationToken cancellationToken);
    }
}
