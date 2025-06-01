using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using WareHouseManager.Models;
using Microsoft.AspNetCore.Http;

namespace WareHouseManager.Repositories
{
    public class CustomerRepository
    {
        private readonly string _apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/Customer";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            var customers = new List<Customer>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                  
                    var result = JsonSerializer.Deserialize<List<Customer>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        customers = result;
                }
            }
            return customers;
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(customer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(customer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_apiUrl}/{customer.Id}", content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync($"{_apiUrl}/{customerId}");
                return response.IsSuccessStatusCode;
            }
        }
    }
}
