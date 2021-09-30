using Microsoft.AspNetCore.Mvc;
using Speet.Models;

namespace Speet.Controllers
{
    public class SportGroupController : Controller
    {
        private readonly DatabaseContext _db;

        public SportGroupController(DatabaseContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DiscoverGroups()
        {
            return View();
        }
    }
}
