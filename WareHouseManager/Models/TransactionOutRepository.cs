using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WareHouseManager.Models;

namespace WareHouseManager.Repositories
{
    public class TransactionOutRepository
    {
        // Remove _httpClient field, use local HttpClient in methods
        private readonly string _apiBaseUrl = "https://modest-gould.103-28-36-75.plesk.page/api/TransactionOut";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionOutRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
        }

        public async Task<List<TransactionOutResponse>> GetTransactionOutsAsync()
        {
            var transactions = new List<TransactionOutResponse>();
            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<TransactionOutResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        transactions = result;
                }
            }
            return transactions;
        }

        public async Task<bool> AddTransactionOutAsync(TransactionOut transactionOut)
        {


            var token = GetToken();
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonSerializer.Serialize(transactionOut);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Console.WriteLine($"Adding TransactionOut: {json}");
                Console.WriteLine($"Content: {content}");

                var response = await client.PostAsync(_apiBaseUrl, content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<List<TransactionOutResponse>> GetTransactionOutResponsesAsync()
        {
            var transactions = new List<TransactionOutResponse>();
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(_apiBaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<TransactionOutResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (result != null)
                        transactions = result;
                }
            }
            return transactions;
        }
    }
}
