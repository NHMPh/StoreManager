using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Models;
using WareHouseManager.Repositories;

namespace WareHouseManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;

        public AccountController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _userRepository = new UserRepository(connectionString);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.AuthenticateUserAsync(model.Username, model.Password);
                if (user.user != null && !string.IsNullOrEmpty(user.token))
                {
                    // Store token in session
                    HttpContext.Session.SetString("AuthToken", user.token);
                    if (user.user.Role == "Admin" && model.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (user.user.Role == "WarehouseManager" && model.Role == "WarehouseManager")
                    {
                        return RedirectToAction("Dashboard", "Warehouse");
                    }
                    else if (user.user.Role == "SalesManager" && model.Role == "SalesManager")
                    {
                        return RedirectToAction("Dashboard", "Sales");
                    }
                }
                ModelState.AddModelError("", "Invalid username, password, or role.");
            }
            return View(model);
        }
    }
}
