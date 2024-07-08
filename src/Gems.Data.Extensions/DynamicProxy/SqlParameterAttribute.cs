namespace Gems.Data.Extensions.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class SqlParameterAttribute : Attribute
    {
        public SqlParameterAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
