using Gems.Data.Extensions.DynamicProxy;
using Gems.Data.UnitOfWork;
using Gems.Extensions.DynamicProxy;
using System.Reflection;

namespace Gems.Data
{
    public static class UnitOfWorkProviderExtensions
    {
        public static string DefaultName { get; set; } = "default";

        public static TInterface Repository<TInterface>(
            this IUnitOfWorkProvider unitOfWorkProvider,
            string name = default,
            CancellationToken cancellationToken = default)
            where TInterface : class
        {
            var ifType = typeof(TInterface);
            var unitOfWorkName = !string.IsNullOrEmpty(name) ? name : ifType.GetCustomAttribute<UnitOfWorkAttribute>()?.Name ?? DefaultName;
            var result = DynamicProxyFactory.Create<TInterface, UnitOfWorkTarget, UnitOfWorkDelegateBuilder>();
            var target = result as UnitOfWorkTarget;
            target.UnitOfWorkProvider = unitOfWorkProvider;
            target.Name = unitOfWorkName;
            target.CancellationToken = cancellationToken;
            return result;
        }

        public static TInterface Repository<TInterface>(
            this IUnitOfWorkProvider unitOfWorkProvider,
            CancellationToken cancellationToken)
        where TInterface : class
        {
            return Repository<TInterface>(unitOfWorkProvider, default(string), cancellationToken);
        }
    }
}
