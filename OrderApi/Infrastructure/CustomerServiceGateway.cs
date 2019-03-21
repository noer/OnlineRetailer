using System;
using RestSharp;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class CustomerServiceGateway : IServiceGateway<CustomerDTO>
    {
        private readonly Uri customerServiceBaseUrl;

        public CustomerServiceGateway(Uri baseUrl)
        {
            customerServiceBaseUrl = baseUrl;
        }

        public CustomerDTO Get(int id)
        {
            RestClient c = new RestClient();
            c.BaseUrl = customerServiceBaseUrl;

            var request = new RestRequest(id.ToString(), Method.GET);
            var response = c.Execute<CustomerDTO>(request);
            var orderedProduct = response.Data;
            return orderedProduct;
        }
    }
}
