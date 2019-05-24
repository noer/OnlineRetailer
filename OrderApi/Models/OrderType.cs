using GraphQL.Types;

namespace OrderApi.Models
{
    public class OrderType : ObjectGraphType<Order>
    {
        public OrderType()
        {
            Field(x => x.OrderId);
            Field(x => x.CustomerId, type: typeof(IntGraphType));
            Field(x => x.Date);
            Field(x => x.Status, type: typeof(StringGraphType));
            Field<ListGraphType<OrderLineType>>("OrderLines",
                resolve: context => context.Source.OrderLines);
        }
    }
}
