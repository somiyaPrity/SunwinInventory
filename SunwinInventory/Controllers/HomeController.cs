using Microsoft.AspNetCore.Mvc;

namespace SunwinInventory.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}