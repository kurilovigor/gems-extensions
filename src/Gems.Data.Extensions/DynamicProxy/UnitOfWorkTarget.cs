using Gems.Data.UnitOfWork;

namespace Gems.Data.Extensions.DynamicProxy
{
    public class UnitOfWorkTarget
    {
        public IUnitOfWorkProvider UnitOfWorkProvider { get; set; }
        
        public string Name { get; set; }

        public CancellationToken CancellationToken { get; set; } = default(CancellationToken);
    }
}
