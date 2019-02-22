using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerApi.Models
{
    public class OrderStatus
    {
        public static string COMPLETED = "COMPLETED";
        public static string CANCELED = "CANCELED";
        public static string SHIPPED = "SHIPPED";
        public static string PAID = "PAID";
    }
}
