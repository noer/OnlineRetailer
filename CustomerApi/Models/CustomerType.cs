using GraphQL.Types;

namespace CustomerApi.Models
{
    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType()
        {
            Field(x => x.customerId);
            Field(x => x.name);
            Field(x => x.email);
            Field(x => x.phone);
            Field(x => x.billingAddress);
            Field(x => x.shippingAddress);
            Field(x => x.creditStanding);
        }
    }
}
