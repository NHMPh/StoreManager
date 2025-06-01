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
    public class SupplierRepository
    {
        private readonly string _apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/Suppliers";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SupplierRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            var suppliers = new List<Supplier>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response JSON: {json}");
                    var result = JsonSerializer.Deserialize<List<Supplier>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        suppliers = result;
                }
            }
            return suppliers;
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
               
                var response = await client.PutAsync($"{_apiUrl}/{supplier.Id}", content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync($"{_apiUrl}/{supplierId}");
                return response.IsSuccessStatusCode;
            }
        }
    }
}
