namespace Gems.Data.Extensions.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SqlMethodAttribute : Attribute
    {
        public SqlMethodAttribute(SqlMethod method)
        {
            this.Method = method;
            this.Name = string.Empty;
        }

        public SqlMethodAttribute(SqlMethod method, string name)
        {
            this.Method = method;
            this.Name = name;
        }

        public SqlMethod Method { get; }
        
        public string Name { get; }
    }
}
