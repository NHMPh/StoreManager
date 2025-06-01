using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class Supplier
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(100)]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100)]
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Mobile is required.")]
        [StringLength(20)]
        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200)]
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("products")]
        public List<Product>? Products { get; set; }
    }
}
