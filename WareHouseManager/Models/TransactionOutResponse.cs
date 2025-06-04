using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class TransactionOutResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; }
        [JsonPropertyName("customerName")]
        public string CustomerName { get; set; } = string.Empty;
        [JsonPropertyName("details")]
        public List<TransactionOutDetailResponse> Details { get; set; } = new List<TransactionOutDetailResponse>();
    }

    public class TransactionOutDetailResponse
    {
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
