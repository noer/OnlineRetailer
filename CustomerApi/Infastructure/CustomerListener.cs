using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomerApi.Data;
using CustomerApi.Models;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using SharedModels;

namespace CustomerApi.Infrastructure
{
    public class CustomerListener
    {
        IServiceProvider provider;
        readonly string connectionString;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public CustomerListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }


        public void Start()
        {
            //string connectionStr = "host=hare.rmq.cloudamqp.com;virtualHost=npaprqop;username=npaprqop;password=TnP46q2gwIcrbfebFLHTk1PGI8j3-vbA";

            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.Subscribe<OrderStatusChangedMessage>("customerApi",
                    UpdateCreditStanding, x => x.WithTopic(OrderDTO.OrderStatus.completed.ToString()));

                bus.Subscribe<OrderStatusChangedMessage>("customerApi",
                    UpdateCreditStanding, x => x.WithTopic(OrderDTO.OrderStatus.shipped.ToString()));


                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void UpdateCreditStanding(OrderStatusChangedMessage message)
        {
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customerRepos = services.GetService<IRepository<Customer>>();

                var customer = customerRepos.Get(message.Order.CustomerId);
                if (message.Order.Status == OrderDTO.OrderStatus.shipped)
                {
                    customer.creditStanding = false;
                } else if (message.Order.Status == OrderDTO.OrderStatus.paid) 
                {
                    customer.creditStanding = true;
                }
                customerRepos.Edit(customer);
            }
        }


    }
}
