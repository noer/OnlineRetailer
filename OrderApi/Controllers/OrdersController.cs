using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using OrderApi.Models;
using SharedModels;

namespace OrderApi.Controllers
{
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private readonly IRepository<Order> orderRepo;
        private readonly IServiceGateway<ProductDTO> productServiceGateway;
        private readonly IServiceGateway<CustomerDTO> customerServiceGateway;
        private readonly IMessagePublisher messagePublisher;

        public OrdersController(
            IRepository<Order> repository, IServiceGateway<ProductDTO> productGateway,
            IServiceGateway<CustomerDTO> customerGateway, IMessagePublisher publisher)
        {
            orderRepo = repository;
            productServiceGateway = productGateway;
            customerServiceGateway = customerGateway;
            messagePublisher = publisher;
        }

        // GET: api/orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return orderRepo.GetAll();
        }

        // GET api/orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = orderRepo.Get(id);
            if (item == null)
                return NotFound();

            return new ObjectResult(item);
        }

        // POST api/orders
        [HttpPost]
        public IActionResult Post([FromBody]OrderDTO orderRequest)
        {
            if (orderRequest == null)
                return BadRequest();
            
            var customer = customerServiceGateway.Get(orderRequest.CustomerId);
            if (customer == null)
                return BadRequest("Customer does not exist. Please create customer first");

            if (customer.creditStanding == false)
                return BadRequest("Customer credit standing is not acceptable");
                
            var order = new Order()
            {
                CustomerId = orderRequest.CustomerId,
                Status = Order.OrderStatus.completed,
                OrderLines = new List<OrderLine>()
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
                        Order = order,
                        ProductId = orderLineRequest.ProductId,
                        Quantity = orderLineRequest.Quantity,
                        UnitPrice = stockProduct.Price
                    };
                    order.OrderLines.Add(orderLine);
                }
            }

            var newOrder = orderRepo.Add(order);

            messagePublisher.PublishOrderStatusChangedMessage(orderRequest, order.Status.ToString());

            return CreatedAtRoute("GetOrder", new { id = newOrder.OrderId }, newOrder);
        }

        [HttpPut]
        [Route("{id}/ship")]
        public IActionResult ship(int id)
        {
            var order = orderRepo.Get(id);
            if (order == null)
                return NotFound();

            if (order.CustomerId == null)
                return BadRequest("No customer for this Order. Unable to ship");

            var orderDTO = convertToOrderDto(order);
            
            order.Status = Order.OrderStatus.shipped;
            orderRepo.Edit(order);

            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

            return Ok("Order shipped");
        }

        [HttpPut]
        [Route("{id}/cancel")]
        public IActionResult cancel(int id)
        {
            var order = orderRepo.Get(id);
            if (order == null)
                return NotFound();

            if (order.Status != Order.OrderStatus.completed)
                return BadRequest("Order already shipped");

            var orderDTO = convertToOrderDto(order);
            
            order.Status = Order.OrderStatus.cancelled;
            orderRepo.Edit(order);

            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

            return Ok("Order cancelled");
        }

        [HttpPut]
        [Route("{id}/pay")]
        public IActionResult pay(int id)
        {
            var order = orderRepo.Get(id);
            if (order == null)
                return NotFound();

            if (order.Status == Order.OrderStatus.paid)
                return BadRequest("Order already paid");

            if (order.Status != Order.OrderStatus.shipped)
                return BadRequest("Order needs to be shipped first");

            var orderDTO = convertToOrderDto(order);
            
            order.Status = Order.OrderStatus.paid;
            orderRepo.Edit(order);

            messagePublisher.PublishOrderStatusChangedMessage(orderDTO, order.Status.ToString());

            return Ok("Order paid");
        }

        private OrderDTO convertToOrderDto(Order order)
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
