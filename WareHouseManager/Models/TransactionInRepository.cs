using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using WareHouseManager.Models;
using Microsoft.AspNetCore.Http;

namespace WareHouseManager.Repositories
{
    public class TransactionInRepository
    {
        private readonly string _apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/TransactionIn";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionInRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<TransactionIn>> GetTransactionInsAsync()
        {
            var transactions = new List<TransactionIn>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<TransactionIn>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        transactions = result;
                }
            }
            return transactions;
        }

        public async Task<bool> AddTransactionInAsync(TransactionIn transaction)
        {
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(transaction);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine($"Adding transaction: {json}");
                Console.WriteLine($"Content: {content}");
                var response = await client.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }
        }
    }
}
