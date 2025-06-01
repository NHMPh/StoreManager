using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Text.Json;
using WareHouseManager.Models;

namespace WareHouseManager.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

       public async Task<(LoginModel user, string token)> AuthenticateUserAsync(string username, string password)
        {
            using (var client = new HttpClient())
            {
                var apiUrl = "https://modest-gould.103-28-36-75.plesk.page/api/Auth/login"; 
                var payload = new
                {
                    Username = username,
                    Password = password
                };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response JSON: {json}");
                    using (var doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;
                        var token = root.GetProperty("token").GetString();
                        var user = new LoginModel
                        {
                            Username = "admin",
                            Role = "Admin",
                        };
                        return (user, token);
                    }
                }
            }
            return (null, null);
        }
    }
}
