using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class Product
    {
        [Key]
        public int productId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int ItemsInStock { get; set; }
        public int ItemsReserved { get; set; }
    }
}
