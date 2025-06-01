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
    public class SalesManagerRepository
    {
        private readonly string _apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/SalesManagers";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SalesManagerRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<SalesManager>> GetSalesManagersAsync()
        {
            var managers = new List<SalesManager>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<SalesManager>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        managers = result;
                }
            }
            return managers;
        }

        public async Task<bool> AddSalesManagerAsync(SalesManager manager)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(manager);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> UpdateSalesManagerAsync(SalesManager manager)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(manager);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{_apiUrl}/{manager.ManagerId}", content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DeleteSalesManagerAsync(int managerId)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.DeleteAsync($"{_apiUrl}/{managerId}");
                return response.IsSuccessStatusCode;
            }
        }
    }
}
