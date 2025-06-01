using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class Product
    {
        [Key]
        [JsonPropertyName("id")]
        public int ProductId { get; set; }

        [Required]
        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [Required]
        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [Required]
        [JsonPropertyName("importPrice")]
        public decimal CostPerUnit { get; set; }

        [Required]
        [JsonPropertyName("quantity")]
        public int Stock { get; set; }

        [JsonPropertyName("imageData")]
        public string? ImageData { get; set; } // base64 image data from API

        [JsonPropertyName("productCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("salePrice")]
        public decimal SalePrice { get; set; }

        [JsonPropertyName("promotion")]
        public string? Promotion { get; set; }

        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }

        [JsonPropertyName("supplier")]
        public object? Supplier { get; set; }

        [JsonPropertyName("orderDetails")]
        public List<object> OrderDetails { get; set; } = new List<object>();
    }
}
