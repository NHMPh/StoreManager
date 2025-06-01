using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class TransactionIn
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("transactionDate")]
        public DateTime TransactionDate { get; set; }

        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }

        [JsonPropertyName("supplier")]
        public Supplier? Supplier { get; set; }

        [JsonPropertyName("details")]
        public List<TransactionInDetail> Details { get; set; } = new List<TransactionInDetail>();
    }

    public class TransactionInDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("transactionInId")]
        public int TransactionInId { get; set; }

        [JsonPropertyName("transactionIn")]
        public TransactionIn? TransactionIn { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("product")]
        public Product? Product { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }
}
