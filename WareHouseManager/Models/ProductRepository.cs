using MySql.Data.MySqlClient;
using System;
using WareHouseManager.Models;

namespace WareHouseManager.Repositories
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetProducts()
        {
            var products = new List<Product>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT ProductId, ProductName, Unit, CostPerUnit, Stock, Icon FROM Products";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(new Product
                            {
                                ProductId = reader.GetInt32("ProductId"),
                                ProductName = reader.GetString("ProductName"),
                                Unit = reader.GetString("Unit"),
                                CostPerUnit = reader.GetDecimal("CostPerUnit"),
                                Stock = reader.GetInt32("Stock"),
                                Icon = reader.IsDBNull(reader.GetOrdinal("Icon")) ? null : reader.GetString("Icon")
                            });
                        }
                    }
                }
            }
        


            return products;
        }
        public void AddProduct(Product product)
        {
            Console.WriteLine("Adding product to database:");
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Products (ProductName, Unit, CostPerUnit, Stock, Icon) VALUES (@ProductName, @Unit, @CostPerUnit, @Stock, @Icon)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Unit", product.Unit);
                    command.Parameters.AddWithValue("@CostPerUnit", product.CostPerUnit);
                    command.Parameters.AddWithValue("@Stock", product.Stock);
                    command.Parameters.AddWithValue("@Icon", product.Icon ?? (object)DBNull.Value);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
