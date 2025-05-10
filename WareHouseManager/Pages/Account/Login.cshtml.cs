using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WareHouseManager.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Role { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            // Dummy authentication logic
            if (Username == "admin" && Password == "admin123" && Role == "Admin")
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else if (Username == "manager" && Password == "manager123" && Role == "WarehouseManager")
            {
                return RedirectToPage("/Warehouse/Dashboard");
            }
            else if (Username == "sales" && Password == "sales123" && Role == "SalesManager")
            {
                return RedirectToPage("/Sales/Dashboard");
            }
            else
            {
                ErrorMessage = "Invalid username, password, or role.";
                return Page();
            }
        }
    }
}
