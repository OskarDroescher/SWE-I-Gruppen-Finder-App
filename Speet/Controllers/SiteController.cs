using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Speet.Controllers
{
    public class SiteController : Controller
    {
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
