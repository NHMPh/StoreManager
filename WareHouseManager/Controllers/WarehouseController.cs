using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Repositories;
using WareHouseManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WareHouseManager.Controllers
{
    public class WarehouseController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly SupplierRepository _supplierRepository;
        private readonly TransactionInRepository _transactionInRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WarehouseController(ProductRepository productRepository, SupplierRepository supplierRepository, TransactionInRepository transactionInRepository, IHttpContextAccessor httpContextAccessor)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _transactionInRepository = transactionInRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Dashboard()
        {
            var products = await _productRepository.GetProductsAsync();
            var suppliers = await _supplierRepository.GetSuppliersAsync();

            ViewData["AllProducts"] = products;
            ViewData["AllSuppliers"] = suppliers;
            ViewData["ProductController"] = "Warehouse";
            ViewData["SupplierController"] = "Warehouse";
            ViewData["TransactionInController"] = "Warehouse";
            ViewData["ActiveTab"] = GetActiveTab();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionIn([FromForm] string transactionInJson)
        {
            SetActiveTab("transaction-tab");
            if (string.IsNullOrWhiteSpace(transactionInJson))
            {
                TempData["Error"] = "Invalid transaction data.";
                return RedirectToAction("Dashboard");
            }
            var transactionIn = System.Text.Json.JsonSerializer.Deserialize<TransactionIn>(transactionInJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
        private void SetActiveTab(string tabName)
        {
            _httpContextAccessor.HttpContext?.Session.SetString("ActiveTab", tabName);
        }

        // Helper to get the active tab from session
        private string GetActiveTab()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("ActiveTab") ?? "product-tab";
        }
    }
}
