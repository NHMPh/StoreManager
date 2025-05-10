namespace WareHouseManager.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Image { get; set; } // URL or path to the supplier image
    }
}
