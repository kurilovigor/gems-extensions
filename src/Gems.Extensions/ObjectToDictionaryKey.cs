
namespace Gems
{
    internal class ObjectToDictionaryKey
    {
        private readonly int hashCode;

        public ObjectToDictionaryKey(Type type, NamingConvention naming)
        {
            this.Type = type;
            this.Naming = naming;
            this.hashCode = HashCode.Combine(Type, Naming);
        }

        public Type Type { get; }

        public NamingConvention Naming { get; }

        public override bool Equals(object obj)
        {
            return obj is ObjectToDictionaryKey key &&
                   EqualityComparer<Type>.Default.Equals(this.Type, key.Type) &&
                   this.Naming == key.Naming;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }
}
