using Microsoft.AspNetCore.Mvc;
using SportsAndMeets.Models;

namespace SportsAndMeets.Controllers
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
    }
}
