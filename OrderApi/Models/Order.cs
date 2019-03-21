using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        [Timestamp]
        public DateTime? Date { get; set; }
        [Required]
        public ICollection<OrderLine> OrderLines { get; set; }
        public OrderStatus Status { get; set; }

        public enum OrderStatus
        {
            cancelled,
            completed,
            shipped,
            paid
        }
    }
}
