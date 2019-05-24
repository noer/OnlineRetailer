using System.Collections.Generic;
using GraphQL;
using OrderApi.Data;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderApi.Infrastructure;
using SharedModels;

namespace OrderApi.Models
{
    public class OrderQuery : ObjectGraphType
    {
        public OrderQuery(IRepository<Order> repository)
        {
            Field<ListGraphType<OrderType>>(
                "orders",
                resolve: context => repository.GetAll());

            Field<OrderType>(
                "order",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "orderId" }),
                resolve: context => repository.Get(context.GetArgument<int>("orderId")));
        }
    }
    
    public class OrderMutation : ObjectGraphType
    {
        public OrderMutation(IRepository<Order> repository, IServiceGateway<ProductDTO> productGateway,
            IServiceGateway<CustomerDTO> customerGateway, IMessagePublisher messagePublisher,
            IMailService mail)
        {
            Name = "OrderMutation";

            Field<OrderType>(
                "addOrder",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<OrderInputType>> {Name = "order"}),
                resolve: context =>
                {
                    var orderRequest = context.GetArgument<Order>("order");
                    if (orderRequest == null)
                        throw new ExecutionError("Bad Request");
            
                    var customer = customerGateway.Get(orderRequest.CustomerId.GetValueOrDefault(-1));
                    if (customer == null)
                        throw new ExecutionError("Customer does not exist. Please create customer first");

                    if (customer.creditStanding == false)
                    {
                        mail.sendMessage(customer.email, "Bad credit standing",
                            "You are in bad credit standing");
                        throw new ExecutionError("Customer credit standing is not acceptable");
                    }
                        
                    var order = new Order()
                    {
                        CustomerId = orderRequest.CustomerId,
                        Status = Order.OrderStatus.completed,
                        OrderLines = new List<OrderLine>()
                    };

                    // Check that products are in stock
                    foreach (var orderLineRequest in orderRequest.OrderLines)
                    {
                        var stockProduct = productGateway.Get(orderLineRequest.ProductId);
                        if (orderLineRequest.Quantity > stockProduct.ItemsAvailable)
                        {
                            mail.sendMessage(customer.email, "Products not in stock",
                                "Some of the ordered products are not in stock");
                            throw new ExecutionError("Order not created. Product " + stockProduct.Name + " is not in stock");
                        }
                        else
                        {
                            var orderLine = new OrderLine()
                            {
                                Order = order,
                                ProductId = orderLineRequest.ProductId,
                                Quantity = orderLineRequest.Quantity,
                                UnitPrice = stockProduct.Price
                            };
                            order.OrderLines.Add(orderLine);
                        }
                    }

                    var newOrder = repository.Add(order);

                    messagePublisher.PublishOrderStatusChangedMessage(convertToOrderDto(orderRequest), order.Status.ToString());

                    mail.sendMessage(customer.email, "Order is confirmed",
                        "Your order has been successfully placed. The order number is: " + newOrder.OrderId);

        //            return CreatedAtRoute("GetOrder", new { id = newOrder.OrderId }, newOrder);

                    return newOrder;
                });

            Field<OrderType>(
                "shipOrder",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> {Name = "orderId"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("orderId");
                    var order = repository.Get(id);
                    if (order == null)
                        throw new ExecutionError("Order not found");

                    if (order.CustomerId == null)
                        throw new ExecutionError("No customer for this Order. Unable to ship");

                    var orderDTO = convertToOrderDto(order);

                    var updatedOrder = repository.UpdateStatus(id, Order.OrderStatus.shipped);

                    messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

                    return updatedOrder;
                });

            Field<OrderType>(
                "cancelOrder",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> {Name = "orderId"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("orderId");
                    var order = repository.Get(id);
                    if (order == null)
                        throw new ExecutionError("Order not found");

                    if (order.Status != Order.OrderStatus.completed)
                        throw new ExecutionError("Order already shipped");

                    var orderDTO = convertToOrderDto(order);

                    var updatedOrder = repository.UpdateStatus(id, Order.OrderStatus.cancelled);

                    messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

                    return updatedOrder;
                });

            Field<OrderType>(
                "payOrder",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> {Name = "orderId"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("orderId");
                    var order = repository.Get(id);
                    if (order == null)
                        throw new ExecutionError("Order not found");

                    if (order.Status == Order.OrderStatus.paid)
                        throw new ExecutionError("Order already paid");

                    if (order.Status != Order.OrderStatus.shipped)
                        throw new ExecutionError("Order needs to be shipped first");

                    var orderDTO = convertToOrderDto(order);
            
                    var updatedOrder = repository.UpdateStatus(id, Order.OrderStatus.paid);

                    messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

                    return updatedOrder;
                });
            
        }

        private static OrderDTO convertToOrderDto(Order order)
        {
            var orderDTO = new OrderDTO()
            {
                CustomerId = order.CustomerId ?? -1,
                Status = (OrderDTO.OrderStatus)order.Status,
                OrderLines = new List<OrderLineDTO>()
            };
            foreach (var line in order.OrderLines)
            {
                orderDTO.OrderLines.Add(new OrderLineDTO()
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity
                });
            }

            return orderDTO;
        }

    }
}