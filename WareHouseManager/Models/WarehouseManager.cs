namespace WareHouseManager.Models
{
    public class WarehouseManager
    {
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Image { get; set; } // URL or path to the manager image
    }
}
