using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models
{
    public class OrderDTO
    {
        public int customerId { get; set; }
        public ICollection<OrderLineDTO> OrderLines { get; set; }
    }
}
