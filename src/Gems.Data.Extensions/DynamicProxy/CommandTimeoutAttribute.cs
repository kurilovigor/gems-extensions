namespace Gems.Data.Extensions.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class CommandTimeoutAttribute: Attribute
    { 
        public CommandTimeoutAttribute() 
        {
        }
    }
}
