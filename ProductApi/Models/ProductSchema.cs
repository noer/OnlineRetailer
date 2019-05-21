using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;

namespace ProductApi.Models
{
    public class ProductSchema : GraphQL.Types.Schema
    {
        public ProductSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<ProductQuery>();
            Mutation = resolver.Resolve<ProductMutation>();
        }
    }
}
