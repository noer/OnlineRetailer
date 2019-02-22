using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class Customer
    {
        [Key]
        public int customerId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string email { get; set; }
        public string phone { get; set; }
        [Required]
        public string billingAddress { get; set; }
        public string shippingAddress { get; set; }
        [Required]
        public bool creditStanding { get; set; }

    }
}
