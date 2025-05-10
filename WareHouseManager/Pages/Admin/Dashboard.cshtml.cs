using Microsoft.AspNetCore.Mvc.RazorPages;
using WareHouseManager.Models;
using System.Collections.Generic;

namespace WareHouseManager.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        public List<Product> Products { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public void OnGet()
        {
            // Fetch data from the database (dummy data for now)
            Products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Widget", Unit = "Piece", CostPerUnit = 10.5m, Stock = 100 },
                new Product { ProductId = 2, ProductName = "Gadget", Unit = "Box", CostPerUnit = 25.0m, Stock = 50 }
            };

            Suppliers = new List<Supplier>
            {
                new Supplier { SupplierId = 1, SupplierName = "Supplier A", Email = "a@example.com", Mobile = "123456789", Address = "123 Street" },
                new Supplier { SupplierId = 2, SupplierName = "Supplier B", Email = "b@example.com", Mobile = "987654321", Address = "456 Avenue" }
            };
        }
    }
}
