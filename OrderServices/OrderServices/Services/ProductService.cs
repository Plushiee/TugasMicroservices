using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using OrderServices.Models;
using OrderServices.DTO.Product;

namespace OrderServices.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://localhost:7001");
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var response = await _httpClient.GetAsync("/catalogservices/api/product");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<IEnumerable<Product>>(results);
                return products;
            }
            else
            {
                throw new ArgumentException($"Cannot get products - httpstatus : {response.StatusCode}");
            }
        }

        public async Task<Product> GetByProductId(int productId)
        {
            var response = await _httpClient.GetAsync($"/catalogservices/api/product/id/{productId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Product>();
                if (result == null)
                {
                    throw new ArgumentException($"Product with id {productId} not found");
                }
                else
                {
                    return result;
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ArgumentException($"Product with id {productId} not found");
            }
            else
            {
                throw new ArgumentException($"Cannot get products - httpstatus : {response.StatusCode}");
            }
        }

        public async Task UpdateStockAfterOrder(ProductUpdateQuantityDTO productUpdateQuantityDTO)
        {
            var response = await _httpClient.PutAsync("/catalogservices/api/product/updatequantity", JsonContent.Create(productUpdateQuantityDTO));
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                throw new ArgumentException($"Cannot update stock - httpstatus : {response.StatusCode}");
            }
        }
    }
}
