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

        public WarehouseController(ProductRepository productRepository, SupplierRepository supplierRepository, TransactionInRepository transactionInRepository)
        {
            _productRepository = productRepository;
            _supplierRepository = supplierRepository;
            _transactionInRepository = transactionInRepository;
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

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionIn([FromForm] string transactionInJson)
        {
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
    }
}
