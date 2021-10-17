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
            CreateEditGroupContainer viewContainer = new CreateEditGroupContainer()
            {
                SportGroupToEdit = null,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList()
            };

            return View("CreateEditGroup", viewContainer);
        }

        public IActionResult EditGroup(long groupId)
        {
            SportGroup groupToEdit = _db.SportGroup.Find(groupId);
            if(groupToEdit == null)
                return new EmptyResult();

            CreateEditGroupContainer viewContainer = new CreateEditGroupContainer()
            {
                SportGroupToEdit = groupToEdit,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList()
            };

            return View("CreateEditGroup", viewContainer);
        }

        [HttpGet]
        public IActionResult AddGroup(AddEditGroupRequest request)
        {
            SportGroup newGroup = new SportGroup()
            {
                GroupName = request.GroupName,
                Location = "Not implemented yet",
                MeetupDate = request.MeetupDate,
                MaxParticipants = request.MaxParticipants,
                CreatedBy = GetTestUser(),
                ActivityTags = _db.ActivityTag.Where(at => request.ActivityCategories.Contains(at.ActivityCategory)).ToList(),
                GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction)
        };

            _db.SportGroup.Add(newGroup);
            _db.SaveChanges();

            return Redirect("CreateGroup");
        }

        [HttpGet]
        public IActionResult UpdateGroup(AddEditGroupRequest request, long groupId)
        {
            SportGroup groupToEdit = _db.SportGroup.Find(groupId);
            if (groupToEdit == null)
                return new EmptyResult();

            groupToEdit.GroupName = request.GroupName;
            groupToEdit.Location = "Not implemented yet";
            groupToEdit.MeetupDate = request.MeetupDate;
            groupToEdit.MaxParticipants = request.MaxParticipants;
            groupToEdit.CreatedBy = GetTestUser();
            groupToEdit.ActivityTags = _db.ActivityTag.Where(at => request.ActivityCategories.Contains(at.ActivityCategory)).ToList();
            groupToEdit.GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction);

            _db.SaveChanges();

            return Redirect("MyGroups");
        }

        private User GetTestUser()
        {
            User testUser = _db.User.Find((long)1);
            if(testUser == null)
            {
                User user = new User()
                {
                    GoogleId = 1,
                    Gender = GenderType.Male,
                };

                _db.User.Add(user);
                _db.SaveChanges();
            }

            return testUser;
        }
    }
}
