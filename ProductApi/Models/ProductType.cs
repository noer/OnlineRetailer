using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using ProductApi.Models;

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
