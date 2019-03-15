using System.Collections.Generic;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderStatusChangedMessage(OrderDTO order, string topic);
    }
}
