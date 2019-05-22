using GraphQL.Types;

namespace ProductApi.Models
{
    public class ProductInputType : InputObjectGraphType
    {
        public ProductInputType()
        {
            Name = "ProductInput";
            Field<IntGraphType>("productId");
            Field<NonNullGraphType<StringGraphType>>("Name");
            Field<NonNullGraphType<StringGraphType>>("Price");
            Field<NonNullGraphType<StringGraphType>>("ItemsInStock");
            Field<StringGraphType>("ItemsReserved");
        }
    }
}
