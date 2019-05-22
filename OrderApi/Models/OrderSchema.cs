using OrderApi.Models;
using GraphQL;

namespace OrderApi.Models
{
    public class OrderSchema : GraphQL.Types.Schema
    {
        public OrderSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<OrderQuery>();
            Mutation = resolver.Resolve<OrderMutation>();
        }
    }
}
