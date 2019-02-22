﻿using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomerApi.Data
{
    public class CustomerRepository : IRepository<Customer>
    {
        private readonly CustomerApiContext db;

        public CustomerRepository(CustomerApiContext context)
        {
            db = context;
        }

        Customer IRepository<Customer>.Add(Customer entity)
        {
            var newCustomer = db.Customers.Add(entity).Entity;
            db.SaveChanges();
            return newCustomer;
        }

        void IRepository<Customer>.Edit(Customer entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        Customer IRepository<Customer>.Get(int id)
        {
            return db.Customers.FirstOrDefault(c => c.customerId == id);
        }

        IEnumerable<Customer> IRepository<Customer>.GetAll()
        {
            return db.Customers.ToList();
        }

        void IRepository<Customer>.Remove(int id)
        {
            var customer = db.Customers.FirstOrDefault(c => c.customerId == id);
            db.Customers.Remove(customer);
            db.SaveChanges();
        }
    }
}
}
