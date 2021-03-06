﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;

namespace ProductApi.Controllers
{
    [Route("api/Products")]
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> repository;

        public ProductsController(IRepository<Product> repos)
        {
            repository = repos;
        }

        // GET: api/products
        [HttpGet]
        public IActionResult Get()
        {
            var listProduct = repository.GetAll();
            List<ProductDTO> listProductDTO = new List<ProductDTO>();
            foreach (var prod in listProduct)
            {
                listProductDTO.Add(convertProduct(prod));
            }
            return Ok(listProductDTO);
        }

        // GET api/products/5
        [HttpGet("{id}", Name="GetProduct")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(convertProduct(item));
        }

        // POST api/products
        [HttpPost]
        public IActionResult Post([FromBody]ProductDTO product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var newProduct = repository.Add(convertProductDTO(product));

            return Ok(newProduct);
        }

        // PUT api/products/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ProductDTO product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }

            var modifiedProduct = repository.Get(id);

            if (modifiedProduct == null)
            {
                return NotFound();
            }

            modifiedProduct.Name = product.Name;
            modifiedProduct.Price = product.Price;
            modifiedProduct.ItemsInStock = modifiedProduct.ItemsReserved + product.ItemsAvailable;

            repository.Edit(modifiedProduct);
            return Ok(modifiedProduct);
        }

        // DELETE api/products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (repository.Get(id) == null)
            {
                return NotFound();
            }

            repository.Remove(id);
            return Ok();
        }

        private ProductDTO convertProduct(Product product)
        {
            ProductDTO productDTO = new ProductDTO();
            productDTO.Id = product.productId;
            productDTO.Name = product.Name;
            productDTO.Price = product.Price;
            productDTO.ItemsAvailable = (product.ItemsInStock - product.ItemsReserved);

            return productDTO;
        }

        private Product convertProductDTO(ProductDTO productDTO)
        {
            Product product = new Product();

            product.productId = productDTO.Id;
            product.Name = productDTO.Name;
            product.ItemsInStock = productDTO.ItemsAvailable;
            product.Price = productDTO.Price;

            return product;
        }
    }
}
