using System.ComponentModel.DataAnnotations;

namespace WareHouseManager.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public decimal CostPerUnit { get; set; }

        [Required]
        public int Stock { get; set; }

        public string Icon { get; set; } // URL or path to the product icon
    }
}
