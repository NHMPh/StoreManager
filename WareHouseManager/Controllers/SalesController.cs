using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Repositories;
using WareHouseManager.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WareHouseManager.Controllers
{
    public class SalesController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly TransactionInRepository _transactionInRepository;
        private readonly TransactionOutRepository _transactionOutRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly SupplierRepository _supplierRepository;

        public SalesController(ProductRepository productRepository, TransactionInRepository transactionInRepository, TransactionOutRepository transactionOutRepository, CustomerRepository customerRepository, SupplierRepository supplierRepository)
        {
            _productRepository = productRepository;
            _transactionInRepository = transactionInRepository;
            _transactionOutRepository = transactionOutRepository;
            _customerRepository = customerRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<IActionResult> Dashboard()
        {
            var products = await _productRepository.GetProductsAsync() ?? new List<Product>();
            var customers = await _customerRepository.GetCustomersAsync() ?? new List<Customer>();
            var transactionsOut = await _transactionOutRepository.GetTransactionOutsAsync() ?? new List<TransactionOutResponse>();
            var model = new AdminDashboardViewModel
            {
                Products = products,
                Customers = customers,

            };
            ViewData["AllProducts"] = products;
            ViewData["AllCustomers"] = customers;
            ViewData["AllTransactionsOut"] = transactionsOut;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTransactionOut([FromForm] string transactionOutJson)
        {
            if (string.IsNullOrWhiteSpace(transactionOutJson))
            {
                TempData["Error"] = "Invalid transaction data.";
                return RedirectToAction("Dashboard");
            }
            Console.WriteLine($"Received transactionOutJson: {transactionOutJson}");
            var transactionOut = System.Text.Json.JsonSerializer.Deserialize<TransactionOut>(transactionOutJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.AddProductAsync(product);
                return RedirectToAction("Dashboard");
            }
            TempData["Error"] = "Invalid product data.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.UpdateProductAsync(product);
                return RedirectToAction("Dashboard");
            }
            TempData["Error"] = "Invalid product data.";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromForm] int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return RedirectToAction("Dashboard");
        }
    }
}
