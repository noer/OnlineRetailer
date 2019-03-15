using System;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;

namespace ProductApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        readonly string connectionString;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            //string connectionStr = "host=hare.rmq.cloudamqp.com;virtualHost=npaprqop;username=npaprqop;password=TnP46q2gwIcrbfebFLHTk1PGI8j3-vbA";

            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.Subscribe<OrderStatusChangedMessage>("productApi", 
                    ReserveItems, x => x.WithTopic(OrderDTO.OrderStatus.completed.ToString()));

                bus.Subscribe<OrderStatusChangedMessage>("productApi",
                    ShipItems, x => x.WithTopic(OrderDTO.OrderStatus.shipped.ToString()));

                bus.Subscribe<OrderStatusChangedMessage>("productApi",
                    UnreserveItems, x => x.WithTopic(OrderDTO.OrderStatus.cancelled.ToString()));

                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void ReserveItems(OrderStatusChangedMessage message)
        {
            // A service scope is created to get an instance of the product repository.
            // When the service scope is disposed, the product repository instance will
            // also be disposed.
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<Product>>();

                foreach (var orderLine in message.Order.OrderLines)
                {
                    var product = productRepos.Get(orderLine.ProductId);
                    product.ItemsReserved += orderLine.Quantity;
                    productRepos.Edit(product);
                }
            }
        }

        private void ShipItems(OrderStatusChangedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<Product>>();

                foreach (var orderLine in message.Order.OrderLines)
                {
                    var product = productRepos.Get(orderLine.ProductId);
                    product.ItemsReserved =- orderLine.Quantity;
                    product.ItemsInStock =- orderLine.Quantity;
                    productRepos.Edit(product);
                }
            }
        }

        private void UnreserveItems(OrderStatusChangedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<Product>>();

                foreach (var orderLine in message.Order.OrderLines)
                {
                    var product = productRepos.Get(orderLine.ProductId);
                    product.ItemsReserved =- orderLine.Quantity;
                    productRepos.Edit(product);
                }
            }
        }

    }
}
