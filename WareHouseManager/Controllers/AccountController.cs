using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Models;
using WareHouseManager.Repositories;

namespace WareHouseManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = new UserRepository(connectionString, httpContextAccessor);
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
                    else if (user.user.Role == "Manager" && model.Role == "WarehouseManager")
                    {
                        return RedirectToAction("Dashboard", "Warehouse");
                    }
                    else if (user.user.Role == "Sale" && model.Role == "SalesManager")
                    {
                        return RedirectToAction("Dashboard", "Sales");
                    }
                }
                ModelState.AddModelError("", "Invalid username, password, or role.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("Dashboard", "Admin");
            }
            // Get current username from session
            var username = HttpContext.Session.GetString("CurrentUsername");
            if (string.IsNullOrEmpty(username))
            {
                TempData["Error"] = "Session expired. Please log in again.";
                return RedirectToAction("Login");
            }
            // Optionally, verify old password by re-authenticating
            var authResult = await _userRepository.AuthenticateUserAsync(username, oldPassword);
            if (authResult.user == null)
            {
                TempData["Error"] = "Old password is incorrect.";
                return RedirectToAction("Dashboard", "Admin");
            }
            // Build user object for update
            var userObj = new User {
                Id = 1, // Admin id is 1 (hardcoded as per your example)
                Username = username,
                Password = newPassword,
                Role = "Admin"
            };
            var token = HttpContext.Session.GetString("AuthToken");
            await _userRepository.UpdateUserAsync(1, userObj, token);
            TempData["Success"] = "Password changed successfully.";
            return RedirectToAction("Dashboard", "Admin");
        }
    }
}
