using System.Reflection;

namespace Gems.Data.Extensions.DynamicProxy
{
    internal class UnitOfWorkDelegateBuilderCacheKey
    {
        private readonly int hashCode;
        public UnitOfWorkDelegateBuilderCacheKey(MethodInfo method, Type targetType)
        {
            this.Method = method;
            this.TargetType = targetType;
            this.hashCode = HashCode.Combine(Method, TargetType);
        }

        public MethodInfo Method { get; }

        public Type TargetType { get; }

        public override bool Equals(object obj)
        {
            return obj is UnitOfWorkDelegateBuilderCacheKey key &&
                   EqualityComparer<MethodInfo>.Default.Equals(Method, key.Method) &&
                   EqualityComparer<Type>.Default.Equals(TargetType, key.TargetType);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
