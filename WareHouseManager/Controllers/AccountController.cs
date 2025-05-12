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
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.AuthenticateUser(model.Username, model.Password);
                if (user != null)
                {
                    if (user.Role == "Admin" && model.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (user.Role == "WarehouseManager"&& model.Role == "WarehouseManager")
                    {
                        return RedirectToAction("Dashboard", "Warehouse");
                    }
                    else if (user.Role == "SalesManager"&& model.Role == "SalesManager")
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
