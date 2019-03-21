using System;
using System.Collections.Generic;
using EasyNetQ;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishOrderStatusChangedMessage(OrderDTO order, string topic)
        {
            var message = new OrderStatusChangedMessage
            { Order = order};

            bus.Publish(message, topic);
        }
    }
}
