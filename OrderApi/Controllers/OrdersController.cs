using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;

namespace OrderApi.Controllers
{
    [Route("api/Orders")]
    public class OrdersController : Controller
    {
        private string PRODUCTS_API = "https://productapi/api/products/";

        private readonly IRepository<Order> repository;

        public OrdersController(IRepository<Order> repos)
        {
            repository = repos;
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
            var order = new Order() { status = "new", Date = new DateTime() };
            
            RestClient c = new RestClient();
            c.BaseUrl = new Uri(PRODUCTS_API);

            // Add OrderLines
            foreach (var orderLineRequest in orderRequest.OrderLines)
            {
                var request = new RestRequest(orderLineRequest.ProductId.ToString(), Method.GET);
                var response = c.Execute<Product>(request);
                var orderedProduct = response.Data;

                if (orderLineRequest.Quantity <= orderedProduct.ItemsInStock)
                {
                    // reduce the number of items in stock for the ordered product,
                    // and create a new order.
                    orderedProduct.ItemsReserved += orderLineRequest.Quantity;
                    var updateRequest = new RestRequest(orderedProduct.productId.ToString(), Method.PUT);
                    updateRequest.AddJsonBody(orderedProduct);
                    var updateResponse = c.Execute(updateRequest);

                    if (updateResponse.IsSuccessful)
                    {
                        var orderLine = new OrderLine()
                        {
                            ProductId = orderLineRequest.ProductId,
                            Description = orderedProduct.Name,
                            Quantity = orderLineRequest.Quantity,
                            UnitPrice = orderedProduct.Price
                        };
                        order.OrderLines.Add(orderLine);
                    }
                }
            }

            var newOrder = repository.Add(order);
            return CreatedAtRoute("GetOrder", new { id = newOrder.orderId }, newOrder);

            // If the order could not be created, "return no content".
            return NoContent();
        }

    }
}
