namespace WareHouseManager.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal CostPerUnit { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; } // URL or path to the product image
        public string Icon { get; set; } // URL or path to the product icon
    }
}
