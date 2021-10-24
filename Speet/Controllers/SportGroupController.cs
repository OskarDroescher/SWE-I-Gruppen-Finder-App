using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Speet.Models;
using Speet.Models.ContainerModels;
using Speet.Models.HttpRequestModels;
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
            List<SportGroup> filteredGroups = GetFilteredSportGroups(filterSettings);
            List<SportGroup> groupsOnPage = GetSportGroupsOnPage(pageIndex, filteredGroups);

            DiscoverGroupsContainer viewContainer = new DiscoverGroupsContainer()
            {
                SportGroupsToDisplay = groupsOnPage,
                FilterSettings = filterSettings,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList(),
                PaginationInfo = new PaginationInfo(pageIndex, filteredGroups.Count)
            };
            if (User.Identity.IsAuthenticated) { 
                return View(viewContainer); 
            }
            else
            {
                return RedirectToAction("Start", "SportGroup");
            }
            
        }

        public IActionResult Start()
        {
            return View();
        }

        private List<SportGroup> GetFilteredSportGroups(FilterSettingsRequest filterSettings)
        {
            var groupsToDisplayQuery = _db.SportGroup.AsQueryable();

            if (filterSettings.ActivityCategories.Count > 0)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => sg.ActivityTags.Any(at => filterSettings.ActivityCategories.Contains(at.ActivityCategory)));

            if (filterSettings.GenderRestrictions.Count > 0)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.GenderRestrictions.Contains(sg.GenderRestrictionTag.GenderRestriction));

            if (filterSettings.MinDate.HasValue)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.MinDate.Value.CompareTo(sg.MeetupDate) <= 0);

            if (filterSettings.MaxDate.HasValue)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.MaxDate.Value.CompareTo(sg.MeetupDate) >= 0);

            groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => sg.MaxParticipants <= filterSettings.MaxParticipants);

            return groupsToDisplayQuery.ToList();
        }

        private List<SportGroup> GetSportGroupsOnPage(int pageIndex, List<SportGroup> groups)
        {
            int firstGroupOnPageIndex = (pageIndex * ApplicationConstants.SportGroupsPerPage - ApplicationConstants.SportGroupsPerPage);
            return groups.Skip(firstGroupOnPageIndex).Take(ApplicationConstants.SportGroupsPerPage).ToList();
        }

        public IActionResult MyGroups(int pageindex = 1)
        {
            User user = GetTestUser();
            List<SportGroup> groupsOnPage = GetSportGroupsOnPage(pageindex, user.JoinedGroups.ToList());
            MyGroupsContainer viewContainer = new MyGroupsContainer()
            {
                GroupsToDisplay = groupsOnPage,
                UserToDisplay = user,
                PaginationInfo = new PaginationInfo(pageindex, user.JoinedGroups.Count)
            };

            return View(viewContainer);
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

        [HttpPost]
        public IActionResult AddGroup(AddEditGroupRequest request)
        {
            User groupCreator = GetTestUser();
            SportGroup newGroup = new SportGroup()
            {
                GroupName = request.GroupName,
                Location = "Not implemented yet",
                MeetupDate = request.MeetupDate,
                MaxParticipants = request.MaxParticipants,
                CreatedBy = groupCreator,
                ActivityTags = _db.ActivityTag.Where(at => request.ActivityCategories.Contains(at.ActivityCategory)).ToList(),
                GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction)
            };
            newGroup.Participants.Add(groupCreator);

            _db.SportGroup.Add(newGroup);
            _db.SaveChanges();

            return Redirect("CreateGroup");
        }

        [HttpPost] //Put verb not supported until HTML version 4
        public IActionResult UpdateGroup(AddEditGroupRequest request, long groupId)
        {
            SportGroup groupToEdit = _db.SportGroup.Find(groupId);
            if (groupToEdit == null)
                return new EmptyResult();

            groupToEdit.GroupName = request.GroupName;
            groupToEdit.Location = "Not implemented yet";
            groupToEdit.MeetupDate = request.MeetupDate;
            groupToEdit.MaxParticipants = request.MaxParticipants;
            groupToEdit.GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction);

            //Warning: overwriting the groupToEdit.ActivityTags reference directly could throw an exception, thats why the list is just refilled
            groupToEdit.ActivityTags.Clear();
            List<ActivityTag> requestedActivityTags = request.ActivityCategories.Select(ac => _db.ActivityTag.Find(ac)).ToList();
            requestedActivityTags.ForEach(at => groupToEdit.ActivityTags.Add(at));

            _db.SaveChanges();

            return Redirect("MyGroups");
        }

        [HttpDelete]
        public IActionResult DeleteGroup(long groupId)
        {
            SportGroup groupToDelete = _db.SportGroup.Find(groupId);
            if (groupToDelete == null)
                return Json(new { success = false });

            _db.SportGroup.Remove(groupToDelete);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        private User GetTestUser()
        {
            User testUser = _db.User.Find((long)1);
            if(testUser == null)
            {
                testUser = new User()
                {
                    GoogleId = 1,
                    Gender = GenderType.Male,
                };

                _db.User.Add(testUser);
                _db.SaveChanges();
            }

            return testUser;
        }
    }
}
