using MySql.Data.MySqlClient;
using System;
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

        public LoginModel AuthenticateUser(string username, string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT Username, Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LoginModel
                            {
                                Username = reader.GetString("Username"),
                                Role = reader.GetString("Role")
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
