using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Repositories;
using WareHouseManager.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Linq;

namespace WareHouseManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly UserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // TransactionIn Section
        private readonly TransactionInRepository _transactionInRepository;
        private readonly TransactionOutRepository _transactionOutRepository;

        public AdminController(ProductRepository productRepository, SupplierRepository supplierRepository, CustomerRepository customerRepository, UserRepository userRepository, IHttpContextAccessor httpContextAccessor, TransactionInRepository transactionInRepository, TransactionOutRepository transactionOutRepository)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _transactionInRepository = transactionInRepository;
            _transactionOutRepository = transactionOutRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            Console.WriteLine("Admin Dashboard accessed.");
            // Get token from session (set at login)
            var token = HttpContext.Session.GetString("AuthToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch products from the repository
            var products = await _productRepository.GetProductsAsync() ?? new List<Product>();
            var suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>();
            var customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>();
            var transactionsIn = await _transactionInRepository.GetTransactionInsAsync() ?? new List<TransactionIn>();
            ViewData["AllTransactionsIn"] = transactionsIn;

            // Get TransactionOut responses from repository (assume you have a method for this)
            var transactionsOut = await _transactionOutRepository.GetTransactionOutResponsesAsync() ?? new List<TransactionOutResponse>();
            ViewData["AllTransactionsOut"] = transactionsOut;

            // Get all users and filter managers for warehouse manager section
            var usersJson = await _userRepository.GetUsersAsync();
            var warehouseManagers = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?.Where(u => u.Role != null && u.Role.ToLower() == "manager").ToList() ?? new List<User>();
            var salesManagers = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?.Where(u => u.Role != null && u.Role.ToLower() == "sale").ToList() ?? new List<User>();
            var model = new AdminDashboardViewModel
            {
                Products = products,
                Suppliers = suppliers,
                Customers = customers,
                SalesManagers = salesManagers,
                WarehouseManagers = warehouseManagers
            };
            Console.WriteLine($"Number of products fetched: {products.Count}");
            // Log product details for debugging
            ViewData["AllSuppliers"] = suppliers;
            ViewData["AllProducts"] = products;
            ViewData["ActiveTab"] = GetActiveTab();
            Console.WriteLine($"Active tab: {ViewData["ActiveTab"]}");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.AddProductAsync(product);
                SetActiveTab("product-tab");
                return RedirectToAction("Dashboard");
            }
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
 
            };
            SetActiveTab("product-tab");
            ViewData["ActiveTab"] = "product-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.UpdateProductAsync(product);
                SetActiveTab("product-tab");
                return RedirectToAction("Dashboard");
            }
            else
            {
                Console.WriteLine("ModelState is invalid for product update.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            SetActiveTab("product-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
            };
            ViewData["ActiveTab"] = "product-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromForm] int id)
        {
            await _productRepository.DeleteProductAsync(id);
            SetActiveTab("product-tab");
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSupplier(Supplier supplier)
        {
            await MapSupplierProductsFromForm(supplier);
            if (ModelState.IsValid)
            {
                await _supplierRepository.AddSupplierAsync(supplier);
                SetActiveTab("supplier-tab");
                return RedirectToAction("Dashboard");
            }
            SetActiveTab("supplier-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
            };
            ViewData["ActiveTab"] = "supplier-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSupplier(Supplier supplier)
        {
            await MapSupplierProductsFromForm(supplier);
            if (ModelState.IsValid)
            {
                await _supplierRepository.UpdateSupplierAsync(supplier);
                SetActiveTab("supplier-tab");
                return RedirectToAction("Dashboard");
            }else
            {
                Console.WriteLine("ModelState is invalid for supplier update.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            SetActiveTab("supplier-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
            };
            ViewData["ActiveTab"] = "supplier-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteSupplier")]
        public async Task<IActionResult> DeleteSupplier([FromForm] int id)
        {
            await _supplierRepository.DeleteSupplierAsync(id);
            SetActiveTab("supplier-tab");
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.AddCustomerAsync(customer);
                SetActiveTab("customer-tab");
                return RedirectToAction("Dashboard");
            }
            SetActiveTab("customer-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),

            };
            ViewData["ActiveTab"] = "customer-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.UpdateCustomerAsync(customer);
                SetActiveTab("customer-tab");
                return RedirectToAction("Dashboard");
            }else
            {
                Console.WriteLine("ModelState is invalid for customer update.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            SetActiveTab("customer-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
            };
            ViewData["ActiveTab"] = "customer-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromForm] int id)
        {
            await _customerRepository.DeleteCustomerAsync(id);
            SetActiveTab("customer-tab");
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSalesManager(User manager)
        {
            manager.Role = "Sale";
            if (ModelState.IsValid)
            {
                await _userRepository.CreateUserAsync(manager);
                SetActiveTab("sales-manager-tab");
                return RedirectToAction("Dashboard");
            }
            SetActiveTab("sales-manager-tab");
            var usersJson = await _userRepository.GetUsersAsync();
            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();
            var salesManagers = users.Where(u => u.Role != null && u.Role.ToLower() == "sale").ToList();
            ViewData["ActiveTab"] = "sales-manager-tab";
            // Only set WarehouseManagers to salesManagers for the section, since we're using User for both
            return View("Dashboard", new AdminDashboardViewModel { WarehouseManagers = salesManagers });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSalesManager(User manager)
        {
            manager.Role = "Sale";
            if (ModelState.IsValid && manager.Id > 0)
            {
                await _userRepository.UpdateUserAsync(manager.Id, manager);
                SetActiveTab("sales-manager-tab");
                return RedirectToAction("Dashboard");
            }
            SetActiveTab("sales-manager-tab");
            var usersJson = await _userRepository.GetUsersAsync();
            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();
            var salesManagers = users.Where(u => u.Role != null && u.Role.ToLower() == "sale").ToList();
            ViewData["ActiveTab"] = "sales-manager-tab";
            // Only set WarehouseManagers to salesManagers for the section, since we're using User for both
            return View("Dashboard", new AdminDashboardViewModel { WarehouseManagers = salesManagers });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSalesManager([FromForm] int id)
        {
            if (id > 0)
            {
                await _userRepository.DeleteUserAsync(id);
            }
            SetActiveTab("sales-manager-tab");
            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/TransactionIn
        public async Task<IActionResult> TransactionIn()
        {
            var transactions = await _transactionInRepository.GetTransactionInsAsync();
            return View("TransactionIn/Index", transactions);
        }



        // POST: /Admin/CreateTransactionIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionIn([FromForm] string transactionInJson)
        {
            if (string.IsNullOrWhiteSpace(transactionInJson))
            {
                TempData["Error"] = "Invalid transaction data.";
                return RedirectToAction("Dashboard");
            }
            var transactionIn = System.Text.Json.JsonSerializer.Deserialize<WareHouseManager.Models.TransactionIn>(transactionInJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (transactionIn == null)
            {
                TempData["Error"] = "Failed to parse transaction data.";
                return RedirectToAction("Dashboard");
            }
            var result = await _transactionInRepository.AddTransactionInAsync(transactionIn);
            if (result)
                return RedirectToAction("Dashboard");
            TempData["Error"] = "Failed to create transaction.";
            return RedirectToAction("Dashboard");
        }

        // GET: /Admin/TransactionOut
        public async Task<IActionResult> TransactionOut()
        {
            var transactions = await _transactionOutRepository.GetTransactionOutsAsync();
            return View("TransactionOut/Index", transactions);
        }

        // POST: /Admin/CreateTransactionOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionOut([FromForm] string transactionOutJson)
        {
            if (string.IsNullOrWhiteSpace(transactionOutJson))
            {
                TempData["Error"] = "Invalid transaction data.";
                return RedirectToAction("Dashboard");
            }
            var transactionOut = System.Text.Json.JsonSerializer.Deserialize<WareHouseManager.Models.TransactionOut>(transactionOutJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Console.WriteLine($"TransactionOut JSON: {transactionOutJson}");
            Console.WriteLine($"Parsed TransactionOut: {transactionOut?.Id}, {transactionOut?.CustomerId}, {transactionOut?.Details}, {transactionOut?.TransactionDate}");
            if (transactionOut == null)
            {
                TempData["Error"] = "Failed to parse transaction data.";
                return RedirectToAction("Dashboard");
            }
            var result = await _transactionOutRepository.AddTransactionOutAsync(transactionOut);
            if (result)
                return RedirectToAction("Dashboard");
            TempData["Error"] = "Failed to create transaction out.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWarehouseManager(User user)
        {
            // Always set role to Manager for safety
            user.Role = "Manager";
            if (ModelState.IsValid)
            {
                var result = await _userRepository.CreateUserAsync(user);
                // Optionally, check result for errors
                SetActiveTab("warehouse-manager-tab");
                return RedirectToAction("Dashboard");
            }
            // If invalid, reload dashboard with errors
            SetActiveTab("warehouse-manager-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
                SalesManagers = (System.Text.Json.JsonSerializer.Deserialize<List<User>>(await _userRepository.GetUsersAsync(), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>()).Where(u => u.Role != null && u.Role.ToLower() == "sale").ToList(),
                WarehouseManagers = (System.Text.Json.JsonSerializer.Deserialize<List<User>>(await _userRepository.GetUsersAsync(), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>()).Where(u => u.Role != null && u.Role.ToLower() == "manager").ToList()
            };
            ViewData["ActiveTab"] = "warehouse-manager-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWarehouseManager(User user)
        {
            user.Role = "Manager";
            if (ModelState.IsValid && user.Id > 0)
            {
                var result = await _userRepository.UpdateUserAsync(user.Id, user);
                SetActiveTab("warehouse-manager-tab");
                return RedirectToAction("Dashboard");
            }
            SetActiveTab("warehouse-manager-tab");
            var model = new AdminDashboardViewModel
            {
                Products = await _productRepository.GetProductsAsync() ?? new List<Product>(),
                Suppliers = await _supplierRepository.GetSuppliersAsync() ?? new List<Supplier>(),
                Customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>(),
                SalesManagers = (System.Text.Json.JsonSerializer.Deserialize<List<User>>(await _userRepository.GetUsersAsync(), new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>()).Where(u => u.Role != null && u.Role.ToLower() == "sale").ToList(),
                WarehouseManagers = (
                    System.Text.Json.JsonSerializer.Deserialize<List<User>>(
                        await _userRepository.GetUsersAsync(),
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    ) ?? new List<User>()
                ).Where(u => u.Role != null && u.Role.ToLower() == "manager").ToList()
            };
            ViewData["ActiveTab"] = "warehouse-manager-tab";
            return View("Dashboard", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWarehouseManager([FromForm] int id)
        {
            if (id > 0)
            {
                var result = await _userRepository.DeleteUserAsync(id);
            }
            SetActiveTab("warehouse-manager-tab");
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                SetActiveTab("settings-tab");
                return RedirectToAction("Dashboard");
            }

            // Get admin username from session
            var username = HttpContext.Session.GetString("CurrentUsername") ?? "Admin";
            // Get all users to find admin's id
            var usersJson = await _userRepository.GetUsersAsync();
            var users = System.Text.Json.JsonSerializer.Deserialize<List<User>>(usersJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();
            var admin = users.FirstOrDefault(u => u.Username != null && u.Username.ToLower() == username.ToLower());

           
            if (admin == null)
            {
                TempData["Error"] = "Admin user not found.";
                SetActiveTab("settings-tab");
                return RedirectToAction("Dashboard");
            }


            if(admin.Password == null|| admin.Password !=oldPassword) 
            {
                TempData["Error"] = "Admin user does not have a password set.";
                SetActiveTab("settings-tab");
                return RedirectToAction("Dashboard");
            }
            // Optionally, verify old password here if needed (not possible if not returned by API)
            // Build update object
            var updateUser = new User
            {
                Id = admin.Id,
                Username = admin.Username,
                Password = newPassword,
                Role = admin.Role,
                Name = admin.Name,
                Phone = admin.Phone,
                Email = admin.Email
            };
            var result = await _userRepository.UpdateUserAsync(admin.Id, updateUser);
            if (result.ToLower().Contains("error"))
            {
                TempData["Error"] = "Failed to change password.";
            }
            else
            {
                TempData["Success"] = "Password changed successfully.";
            }
            SetActiveTab("settings-tab");
            return RedirectToAction("Dashboard");
        }

        // Helper to set the active tab in session
        private void SetActiveTab(string tabName)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("ActiveTab", tabName);
            ViewData["ActiveTab"] = tabName;
        }

        // Helper to get the active tab from session
        private string GetActiveTab()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("ActiveTab") ?? "product-tab";
        }

        // Helper to map ProductsString from form to supplier.Products
        private async Task MapSupplierProductsFromForm(Supplier supplier)
        {
            var productsString = Request.Form["ProductsString"].ToString();
            if (!string.IsNullOrWhiteSpace(productsString))
            {
                var productNames = productsString.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList();
                var allProducts = await _productRepository.GetProductsAsync();
                // Only match products with non-null ProductName
                supplier.Products = allProducts
                    .Where(p => !string.IsNullOrEmpty(p.ProductName) && productNames.Contains(p.ProductName!))
                    .ToList();

                supplier.Products = new List<Product>();
            }
            else
            {
                supplier.Products = new List<Product>();
            }
        }

       
    }

    public class AdminDashboardViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<User> WarehouseManagers { get; set; } = new List<User>();
        public List<User> SalesManagers { get; set; } = new List<User>();
    }
}