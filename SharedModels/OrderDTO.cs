using System.Collections.Generic;

namespace SharedModels
{
    public class OrderDTO
    {
        public int CustomerId { get; set; }
        public ICollection<OrderLineDTO> OrderLines { get; set; }
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
