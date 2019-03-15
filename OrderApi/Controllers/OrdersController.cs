using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using OrderApi.Models;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Controllers
{
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly IRepository<Order> repository;
        IServiceGateway<ProductDTO> productServiceGateway;
        IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repos, IServiceGateway<ProductDTO> gateway, IMessagePublisher publisher)
        {
            repository = repos;
            productServiceGateway = gateway;
            messagePublisher = publisher;
        }

        // GET: api/orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET api/orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST api/orders
        [HttpPost]
        public IActionResult Post([FromBody]OrderDTO orderRequest)
        {
            if (orderRequest == null)
            {
                return BadRequest();
            }
            
            // Create Order
            var order = new Order()
            {
                CustomerId = orderRequest.CustomerId,
                Status = Order.OrderStatus.completed
            };
            
            // Check that products are in stock
            foreach (var orderLineRequest in orderRequest.OrderLines)
            {
                var stockProduct = productServiceGateway.Get(orderLineRequest.ProductId);
                if (orderLineRequest.Quantity > stockProduct.ItemsAvailable)
                {
                    return BadRequest("Order not created. Product " + stockProduct.Name + " is not in stock");
                }
                else
                {
                    var orderLine = new OrderLine()
                    {
                        ProductId = orderLineRequest.ProductId,
                        Quantity = orderLineRequest.Quantity,
                        UnitPrice = stockProduct.Price
                    };
                    order.OrderLines.Add(orderLine);
                }
            }
            
            messagePublisher.PublishOrderStatusChangedMessage(orderRequest, OrderDTO.OrderStatus.completed.ToString());

            var newOrder = repository.Add(order);
            return CreatedAtRoute("GetOrder", new { id = newOrder.orderId }, newOrder);
        }

        [HttpPut]
        [Route("{id}/ship")]
        public IActionResult ship(int id)
        {
            var order = repository.Get(id);
            if (order.CustomerId == null)
            {
                return BadRequest("No customer for this Order. Unable to ship");
            }

            var orderDTO = convertToOrderDto(order);
            
            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, OrderDTO.OrderStatus.completed.ToString());

            order.Status = Order.OrderStatus.shipped;
            repository.Edit(order);

            return Ok();
        }

        [HttpPut]
        [Route("{id}/cancel")]
        public IActionResult cancel(int id)
        {
            var order = repository.Get(id);
            if (order.Status != Order.OrderStatus.completed)
            {
                return BadRequest("Order already shipped");
            }

            var orderDTO = convertToOrderDto(order);
            
            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, OrderDTO.OrderStatus.cancelled.ToString());

            order.Status = Order.OrderStatus.cancelled;
            repository.Edit(order);

            return Ok();
        }

        [HttpPut]
        [Route("{id}/pay")]
        public IActionResult pay(int id)
        {
            var order = repository.Get(id);
            if (order.Status == Order.OrderStatus.paid)
                return BadRequest("Order already paid");
            if (order.Status != Order.OrderStatus.shipped)
                return BadRequest("Order needs to be shipped first");

            var orderDTO = convertToOrderDto(order);
            
            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, OrderDTO.OrderStatus.paid.ToString());

            order.Status = Order.OrderStatus.paid;
            repository.Edit(order);

            return Ok();
        }

        private OrderDTO convertToOrderDto(Order order)
        {
            var orderDTO = new OrderDTO()
            {
                CustomerId = order.CustomerId ?? -1,
                Status = OrderDTO.OrderStatus.shipped,
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
