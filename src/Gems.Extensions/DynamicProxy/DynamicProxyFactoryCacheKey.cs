
namespace Gems.Extensions.DynamicProxy
{
    internal class DynamicProxyFactoryCacheKey
    {
        private readonly int hashCode;

        public DynamicProxyFactoryCacheKey(Type typeInterface, Type typeImplementation, Type typeDelegateBuilder)
        {
            this.TypeInterface = typeInterface;
            this.TypeImplementation = typeImplementation;
            this.TypeDelegateBuilder = typeDelegateBuilder;
            this.hashCode = HashCode.Combine(typeInterface, typeImplementation, typeDelegateBuilder);
        }

        public Type TypeInterface { get; }

        public Type TypeImplementation { get; }

        public Type TypeDelegateBuilder {  get; }

        public override bool Equals(object obj)
        {
            return obj is DynamicProxyFactoryCacheKey key &&
                   EqualityComparer<Type>.Default.Equals(TypeInterface, key.TypeInterface) &&
                   EqualityComparer<Type>.Default.Equals(TypeImplementation, key.TypeImplementation) &&
                   EqualityComparer<Type>.Default.Equals(TypeDelegateBuilder, key.TypeDelegateBuilder);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
