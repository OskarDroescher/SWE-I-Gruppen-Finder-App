using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Speet.Models;
using Speet.Models.ContainerModels;
using Speet.Models.HttpRequestModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Speet.Controllers
{

    public class SportGroupController : Controller
    {
        private readonly DatabaseContext _db;

        public SportGroupController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult DiscoverGroups(FilterSettingsRequest filterSettings, int pageIndex = 1)
        {
            List<SportGroup> groupsToDisplay = _db.SportGroup.ToList();
            DiscoverGroupsContainer viewContainer = new DiscoverGroupsContainer()
            {
                SportGroupsToDisplay = groupsToDisplay,
                FilterSettings = filterSettings,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList(),
                PageIndex = pageIndex,
                NextPageIndexes = new int[] {2, 3, 4, 5},
                PreviousPageIndexes = new int[] {}
            };

            return View(viewContainer);
        }

        public IActionResult MyGroups()
        {
            return View();
        }

        public IActionResult CreateGroup()
        {
            return View();
        }
    }
}
