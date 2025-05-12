using Microsoft.AspNetCore.Mvc;

namespace WareHouseManager.Controllers
{
    public class WarehouseController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
