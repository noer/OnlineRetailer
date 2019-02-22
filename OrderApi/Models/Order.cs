using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class Order
    {
        [Key]
        public int orderId { get; set; }
        [Timestamp]
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string status { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }
}
