
namespace SharedModels
{
    public class CustomerDTO
    {
        public int customerId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string billingAddress { get; set; }
        public string shippingAddress { get; set; }
        public bool creditStanding { get; set; }

    }
}
