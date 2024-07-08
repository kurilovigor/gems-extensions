using Castle.DynamicProxy;
using System.Collections.Concurrent;
using System.Reflection;

namespace Gems.Extensions.DynamicProxy
{
    public static class DynamicProxyFactory
    {
        private static ProxyGenerator generator = new ProxyGenerator();
        private static ConcurrentDictionary<DynamicProxyFactoryCacheKey, CreateClassProxyConfiguration> cache = new ConcurrentDictionary<DynamicProxyFactoryCacheKey, CreateClassProxyConfiguration>();

        public static TInterface Create<TInterface, TImplementation, TDelegateBuilder>()
            where TImplementation : class, new()
            where TInterface : class
            where TDelegateBuilder : IDelegateBuilder, new()
        {
            var key = new DynamicProxyFactoryCacheKey(
                typeof(TInterface),
                typeof(TImplementation),
                typeof(TDelegateBuilder));

            var configuration = cache.GetOrAdd(
                key, 
                x => CreateCreateClassProxyConfiguration(
                    x.TypeInterface, 
                    x.TypeImplementation, 
                    x.TypeDelegateBuilder));
            var obj = generator.CreateClassProxy(
                configuration.TargetType,
                configuration.Interfaces,
                configuration.Options,
                configuration.Interceptors);
            return obj as TInterface;
        }

        private static CreateClassProxyConfiguration CreateCreateClassProxyConfiguration(
            Type typeInterface, 
            Type typeImplementation, 
            Type typeDelegateBuilder)
        {
            var delegateBuilder = (IDelegateBuilder)Activator.CreateInstance(typeDelegateBuilder);
            var interceptorMap = CreateInterceptorMap(
                typeInterface,
                typeImplementation,
                delegateBuilder);
            var options = new ProxyGenerationOptions();
            options.Selector = new DelegateInterceptorSelector(interceptorMap);
            return new CreateClassProxyConfiguration
            {
                TargetType = typeImplementation,
                Interfaces = new Type[] { typeInterface },
                Options = options,
                Interceptors = interceptorMap.Values.ToArray(),
            };
        }

        private static Dictionary<MethodInfo, IInterceptor> CreateInterceptorMap(
            Type typeInterface, 
            Type typeImplementation,
            IDelegateBuilder delegateBuilder)
        {
            return typeInterface
                .GetMethods()
                .Where(x => x.IsPublic && x.IsVirtual && x.IsAbstract)
                .ToDictionary(x => x, x => (IInterceptor)new DelegateInterceptor(delegateBuilder.Build(x, typeImplementation)));
        }
    }
}
