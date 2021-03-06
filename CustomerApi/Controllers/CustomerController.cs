﻿using System.Collections.Generic;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/customer")]
    public class CustomerController : Controller
    {

        private readonly IRepository<Customer> repository;

        public CustomerController(IRepository<Customer> repo)
        {
            repository = repo;
        }
        // GET: api/customer
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return repository.GetAll();
        }

        // GET: api/customer/5
        [HttpGet("{id}", Name ="GetCustomer")]
        public ActionResult Get(int id)
        {
            var customer = repository.Get(id);

            if (customer == null)
            {
                return NotFound();
            }

            return new ObjectResult(customer);
        }

        // POST: api/customer
        [HttpPost]
        public ActionResult Create([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest();
            }
            customer.creditStanding = true;
            var newCustomer = repository.Add(customer);
            return CreatedAtRoute("GetCustomer", new { id = newCustomer.customerId }, newCustomer);
        }

        // GET: api/customer/5
        [HttpPut("{id}")]
        public ActionResult Edit(int id, [FromBody] Customer customer)
        {
            if (customer == null || customer.customerId != id)
            {
                return BadRequest();
            }

            var originalCustomer = repository.Get(id);
            if (originalCustomer == null)
            {
                return NotFound();
            }

            originalCustomer.shippingAddress = customer.shippingAddress;
            originalCustomer.billingAddress = customer.billingAddress;
            originalCustomer.creditStanding = customer.creditStanding;
            originalCustomer.email = customer.email;
            originalCustomer.phone = customer.phone;
            originalCustomer.name = customer.name;


            repository.Edit(originalCustomer);
            return Ok(originalCustomer);
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (repository.Get(id) == null)
            {
                return NotFound();
            }

            repository.Remove(id);
            return Ok();
        }
    }
}