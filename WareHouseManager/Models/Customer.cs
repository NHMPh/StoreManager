using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WareHouseManager.Models
{
    public class Customer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100)]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20)]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200)]
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("orders")]
        public List<object>? Orders { get; set; } = new List<object>();
    }
}
