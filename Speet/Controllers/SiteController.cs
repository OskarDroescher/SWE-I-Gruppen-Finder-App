using Microsoft.AspNetCore.Mvc;

namespace Speet.Controllers
{
    public class SiteController : Controller
    {
        public IActionResult Start()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
