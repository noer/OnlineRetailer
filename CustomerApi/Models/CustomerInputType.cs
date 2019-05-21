using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace CustomerApi.Models
{
    public class CustomerInputType : InputObjectGraphType
    {
        public CustomerInputType()
        {
            Name = "CustomerInput";
            Field<IntGraphType>("customerId");
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("email");
            Field<StringGraphType>("phone");
            Field<NonNullGraphType<StringGraphType>>("billingAddress");
            Field<StringGraphType>("shippingAddress");
            Field<NonNullGraphType<BooleanGraphType>>("creditStanding");
        }
    }
}
