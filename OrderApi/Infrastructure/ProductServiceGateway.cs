using System;
using RestSharp;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class ProductServiceGateway : IServiceGateway<ProductDTO>
    {
        private readonly Uri productServiceBaseUrl;

        public ProductServiceGateway(Uri baseUrl)
        {
            productServiceBaseUrl = baseUrl;
        }

        public ProductDTO Get(int id)
        {
            RestClient c = new RestClient();
            c.BaseUrl = productServiceBaseUrl;

            var request = new RestRequest(id.ToString(), Method.GET);
            var response = c.Execute<ProductDTO>(request);
            var orderedProduct = response.Data;
            return orderedProduct;
        }
    }
}
