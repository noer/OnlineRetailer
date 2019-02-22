namespace OrderApi.Models
{
    public class OrderLine
    {
        public int LineId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public float UnitPrice { get; set; }
    }
}