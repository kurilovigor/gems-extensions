using System.Reflection;

namespace Gems.Extensions.DynamicProxy
{
    public interface IDelegateBuilder
    {
        Delegate Build(MethodInfo methodInfo, Type type);
    }
}
