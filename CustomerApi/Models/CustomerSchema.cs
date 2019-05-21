using CustomerApi.Models;
using GraphQL;

namespace CustomerApi.Models
{
    public class CustomerSchema : GraphQL.Types.Schema
    {
        public CustomerSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CustomerQuery>();
            Mutation = resolver.Resolve<CustomerMutation>();
        }
    }
}
