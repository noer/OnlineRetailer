using System;
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
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
