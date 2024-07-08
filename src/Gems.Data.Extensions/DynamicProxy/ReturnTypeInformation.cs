namespace Gems.Data.Extensions.DynamicProxy
{
    internal class ReturnTypeInformation
    {
        public ReturnTypeCardinality Cardinality {  get; set; }

        public Type ElementType { get; set; }
        
        public Type ResultType { get; set; }
    }
}
