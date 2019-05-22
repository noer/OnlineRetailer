using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace OrderApi.Models
{
    public class OrderInputType : InputObjectGraphType
    {
        public OrderInputType()
        {
            Name = "OrderInput";
            Field<NonNullGraphType<IntGraphType>>("CustomerId");
            Field<NonNullGraphType<ListGraphType<OrderLineInputType>>>("OrderLines");
        }
    }
}
