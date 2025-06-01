using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class TransactionOut
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }
        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; }
        [JsonPropertyName("details")]
        public List<TransactionOutDetail> Details { get; set; } = new List<TransactionOutDetail>();
    }

    public class TransactionOutDetail
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
        [JsonPropertyName("product")]
        public Product? Product { get; set; }
    }
}
