using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;

namespace OrderApi.Models
{
    public class OrderLineInputType : InputObjectGraphType
    {
        public OrderLineInputType()
        {
            Name = "OrderLineInput";
            Field<NonNullGraphType<IntGraphType>>("ProductId");
            Field<NonNullGraphType<IntGraphType>>("Quantity");
        }
    }
}