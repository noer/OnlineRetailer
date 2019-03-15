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
        IServiceGateway<Product> productServiceGateway;
        IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repos, IServiceGateway<Product> gateway, IMessagePublisher publisher)
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
            var order = new Order() { status = "new" };
            
            // Check that products are in stock
            foreach (var orderLineRequest in orderRequest.OrderLines)
            {
                var orderedProduct = productServiceGateway.Get(orderLineRequest.ProductId);
                return Ok(orderLineRequest.ProductId + " " + orderLineRequest.Quantity);
                if (orderLineRequest.Quantity > orderedProduct.ItemsInStock)
                {
                    return BadRequest("Product " + orderedProduct.Name + " is not in stock");
                    /* 
                    // reduce the number of items in stock for the ordered product,
                    // and create a new order.
                    orderedProduct.ItemsReserved += orderLineRequest.Quantity;
                    
                    var updateRequest = new RestRequest(orderedProduct.productId.ToString(), Method.PUT);
                    updateRequest.AddJsonBody(orderedProduct);
                    var updateResponse = c.Execute(updateRequest);
                    
                    if (updateResponse.IsSuccessful)
                    {*/
                        var orderLine = new OrderLine()
                        {
                            ProductId = orderLineRequest.ProductId,
                            Description = orderedProduct.Name,
                            Quantity = orderLineRequest.Quantity,
                            UnitPrice = orderedProduct.Price
                        };
                        order.OrderLines.Add(orderLine);
                    /*}
                    */
                }
            }
            
            messagePublisher.ReserveProducts(orderRequest.OrderLines);

            var newOrder = repository.Add(order);
            return CreatedAtRoute("GetOrder", new { id = newOrder.orderId }, newOrder);

            // If the order could not be created, "return no content".
            return NoContent();
        }

    }
}
