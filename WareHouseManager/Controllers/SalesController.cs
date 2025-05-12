using Microsoft.AspNetCore.Mvc;

namespace WareHouseManager.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
