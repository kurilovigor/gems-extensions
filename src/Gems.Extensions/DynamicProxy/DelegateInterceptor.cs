using Castle.DynamicProxy;

namespace Gems.Extensions.DynamicProxy
{
    internal class DelegateInterceptor : IInterceptor
    {
        private readonly Delegate func;

        public DelegateInterceptor(Delegate func)
        {
            this.func = func;
        }

        public void Intercept(IInvocation invocation)
        {
            var args = new object[invocation.Arguments.Length + 1];
            args[0] = invocation.Proxy;
            invocation.Arguments.CopyTo(args, 1);
            invocation.ReturnValue = this.func.DynamicInvoke(args);
        }
    }
}
