
﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Speet.Models;
using System.Threading.Tasks;

namespace Speet.Controllers
{

    public class SportGroupController : Controller
    {
        private readonly DatabaseContext _db;

        public SportGroupController(DatabaseContext db)
        {
            _db = db;
        }

        public IActionResult DiscoverGroups()
        {
            return View();
        }

        public IActionResult MyGroups()
        {
            return View();
        }

        public IActionResult CreateGroup()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
