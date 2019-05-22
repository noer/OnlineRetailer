using GraphQL.Types;

namespace OrderApi.Models
{
    public class OrderLineType : ObjectGraphType<OrderLine>
    {
        public OrderLineType()
        {
            Field(x => x.LineId);
            Field(x => x.ProductId);
            Field(x => x.Quantity);
            Field(x => x.UnitPrice);
            Field(x => x.OrderId);
        }
    }
}
