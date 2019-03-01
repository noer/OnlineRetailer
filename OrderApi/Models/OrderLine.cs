using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class OrderLine
    {
        [Key]
        public int LineId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}