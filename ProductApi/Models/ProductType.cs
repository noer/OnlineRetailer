using GraphQL.Types;

namespace ProductApi.Models
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            Field(x => x.productId);
            Field(x => x.Name);
            Field(x => x.Price);
            Field(x => x.ItemsInStock);
            Field(x => x.ItemsReserved);
        }
    }
}
