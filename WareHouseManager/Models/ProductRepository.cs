using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System;
using WareHouseManager.Models;
using Microsoft.AspNetCore.Http;

namespace WareHouseManager.Repositories
{
    public class ProductRepository
    {
        private readonly string _apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/Product";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                  
                    var result = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        products = result;
                }
            }
            
            return products;
        }

        public async Task<bool> AddProductAsync(Product product)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(product);
                
                Console.WriteLine($"Adding product: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine($"Content: {content}");
                var response = await client.PostAsync(_apiUrl, content);
                 Console.WriteLine(response.StatusCode);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine($"Updating product: {json}");
                var response = await client.PutAsync($"{_apiUrl}/{product.ProductId}", content);
                Console.WriteLine($"Update response status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync($"{_apiUrl}/{productId}");
                Console.WriteLine($"Delete response status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
        }
    }
}
