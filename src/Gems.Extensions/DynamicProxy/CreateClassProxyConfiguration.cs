using Castle.DynamicProxy;

namespace Gems.Extensions.DynamicProxy
{
    internal class CreateClassProxyConfiguration
    { 
        public Type TargetType { get; set; }
        
        public Type[] Interfaces { get; set; }
        
        public ProxyGenerationOptions Options { get; set; }

        public IInterceptor[] Interceptors { get; set; }
    }
}
