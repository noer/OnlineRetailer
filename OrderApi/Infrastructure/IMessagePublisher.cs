using System.Collections.Generic;
using OrderApi.Models;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderStatusChangedMessage(int productId, int quantity, string topic);
        void ReserveProducts(ICollection<OrderLineDTO> orderLines);
    }
}
