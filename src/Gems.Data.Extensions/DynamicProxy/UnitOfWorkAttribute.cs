namespace Gems.Data.Extensions.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class UnitOfWorkAttribute : Attribute
    {
        public UnitOfWorkAttribute(string name)
        {
            this.Name = name;
        }

        public string Name{ get; }

        public string Schema { get; set; }

        public string MethodPrefix { get; set; }

        public string MethodSuffix { get; set; }

        public string ParameterPrefix { get; set; }

        public string ParameterSuffix { get; set; }

        public NamingConvention Naming { get; set; } = NamingConvention.None;
    }
}
