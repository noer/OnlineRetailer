using System.Collections.Generic;
using System.Linq;
using System;
using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Customers
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { name = "Billy bob", email = "BillyBob@mail.com", billingAddress = "Billystreet 3, Bobtown", creditStanding = true  }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
