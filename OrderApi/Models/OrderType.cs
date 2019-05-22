using GraphQL.Types;
using OrderApi.Data;

namespace OrderApi.Models
{
    public class OrderType : ObjectGraphType<Order>
    {
        public OrderType(IRepository<Order> repository)
        {
            Field(x => x.OrderId);
            Field(x => x.CustomerId, type: typeof(IntGraphType));
            Field(x => x.Date);
            Field(x => x.Status, type: typeof(StringGraphType));
            Field<ListGraphType<OrderLineType>>("OrderLines",
                resolve: context => repository.Get(context.Source.OrderId).OrderLines);
        }
    }
}
