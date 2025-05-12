using Microsoft.AspNetCore.Mvc;
using WareHouseManager.Repositories;
using WareHouseManager.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace WareHouseManager.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProductRepository _productRepository;

        public AdminController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _productRepository = new ProductRepository(connectionString);
        }

        public IActionResult Dashboard()
        {
            // Fetch products from the database
            List<Product> products = _productRepository.GetProducts();
            // Pass the products to the view
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Dashboard(Product product)
        {
            ///Does not work
            System.Diagnostics.Debug.WriteLine("222222222222222222222222222222222222222222222222222222");

            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("dsadadasdasdasdasdsadasdadadasdasdd111111111111111111111111111111");

                // Save the product to the database
                _productRepository.AddProduct(product);

                // Redirect to the Dashboard to refresh the product list
                return RedirectToAction("Dashboard");
            }

            // If the model is invalid, return the view with the product data
            return View(product);
        }



    }
}