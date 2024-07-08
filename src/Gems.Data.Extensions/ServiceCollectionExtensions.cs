using Gems.Data.Extensions.DynamicProxy;
using Gems.Data.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gems.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository<TInterface>(this IServiceCollection services, string name = default)
            where TInterface : class
        {
            var ifType = typeof(TInterface);
            var unitOfWorkName = (string.IsNullOrEmpty(name) ? name : ifType.GetCustomAttribute<UnitOfWorkAttribute>()?.Name) ?? 
                throw new ArgumentException("Invalid UnitOfWork name");
            return services.AddScoped(sp => sp.GetRequiredService<IUnitOfWorkProvider>().Repository<TInterface>(unitOfWorkName));
        }
    }
}
