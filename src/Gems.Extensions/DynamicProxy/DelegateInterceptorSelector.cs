using Castle.DynamicProxy;
using System.Reflection;

namespace Gems.Extensions.DynamicProxy
{
    internal class DelegateInterceptorSelector : IInterceptorSelector
    {
        private readonly Dictionary<MethodInfo, IInterceptor> interceptorMap = new Dictionary<MethodInfo, IInterceptor>();

        public DelegateInterceptorSelector(Dictionary<MethodInfo, IInterceptor> interceptorMap)
        {
            this.interceptorMap = interceptorMap;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptorMap.TryGetValue(method, out var interceptor) ? new IInterceptor[] { interceptor } : Array.Empty<IInterceptor>();
        }
    }
}
