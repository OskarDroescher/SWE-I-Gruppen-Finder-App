using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Speet.Models;
using Speet.Models.ContainerModels;
using Speet.Models.HttpRequestModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Speet.Controllers
{

    public class SportGroupController : Controller
    {
        private readonly DatabaseContext _db;

        public SportGroupController(DatabaseContext db)
        {
            _db = db;
        }

        public IActionResult DiscoverGroups(FilterSettingsRequest filterSettings, int pageIndex = 1)
        {
            if(!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            User user = GetUserFromRequest();
            List<SportGroup> filteredGroups = GetFilteredSportGroups(user, filterSettings);
            List<SportGroup> sortedGroups = filteredGroups.OrderBy(sg => sg.MeetupDate).ToList();
            PaginationInfo paginationInfo = new PaginationInfo(pageIndex, sortedGroups.Count);
            List<SportGroup> groupsOnPage = GetSportGroupsOnPage(paginationInfo, sortedGroups);

            DiscoverGroupsContainer viewContainer = new DiscoverGroupsContainer()
            {
                SportGroupsToDisplay = groupsOnPage,
                FilterSettings = filterSettings,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList(),
                PaginationInfo = paginationInfo
            };
            
            return View(viewContainer); 
        }

        private List<SportGroup> GetFilteredSportGroups(User user, FilterSettingsRequest filterSettings)
        {
            var groupsToDisplayQuery = _db.SportGroup.Where(sg => !sg.IsPrivate);

            groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => !sg.Participants.Contains(user));

            if (filterSettings.ActivityCategories.Count > 0)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => sg.ActivityTags.Any(at => filterSettings.ActivityCategories.Contains(at.ActivityCategory)));

            if (filterSettings.GenderRestrictions.Count > 0)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.GenderRestrictions.Contains(sg.GenderRestrictionTag.GenderRestriction));

            if (filterSettings.MinDate.HasValue)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.MinDate.Value.CompareTo(sg.MeetupDate) <= 0);

            if (filterSettings.MaxDate.HasValue)
                groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => filterSettings.MaxDate.Value.CompareTo(sg.MeetupDate) >= 0);

            groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => sg.MaxParticipants <= filterSettings.MaxParticipants);

            //Filter out full groups
            groupsToDisplayQuery = groupsToDisplayQuery.Where(sg => sg.MaxParticipants > sg.Participants.Count);

            return groupsToDisplayQuery.ToList();
        }

        private List<SportGroup> GetSportGroupsOnPage(PaginationInfo paginationInfo, List<SportGroup> groups)
        {
            int firstGroupOnPageIndex = (paginationInfo.CurrentPageIndex * ApplicationConstants.SportGroupsPerPage - ApplicationConstants.SportGroupsPerPage);
            return groups.Skip(firstGroupOnPageIndex).Take(ApplicationConstants.SportGroupsPerPage).ToList();
        }

        public IActionResult MyGroups(int pageindex = 1)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            User user = GetUserFromRequest();
            List<SportGroup> joinedGroups = user.JoinedGroups.OrderBy(sg => sg.MeetupDate).ToList();
            PaginationInfo paginationInfo = new PaginationInfo(pageindex, joinedGroups.Count);
            List<SportGroup> groupsOnPage = GetSportGroupsOnPage(paginationInfo, joinedGroups);
            MyGroupsContainer viewContainer = new MyGroupsContainer()
            {
                GroupsToDisplay = groupsOnPage,
                UserToDisplay = user,
                PaginationInfo = paginationInfo
            };

            return View(viewContainer);
        }

        public IActionResult Invite(Guid groupId)
        {
            TempData["JoinPopupGroupId"] = groupId.ToString();
            return RedirectToAction("DiscoverGroups");
        }

        public IActionResult CreateGroup()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            User user = GetUserFromRequest();
            CreateEditGroupContainer viewContainer = new CreateEditGroupContainer()
            {
                SportGroupToEdit = null,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList(),
                GroupCreator = user
            };

            return View("CreateEditGroup", viewContainer);
        }

        public IActionResult EditGroup(Guid groupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            SportGroup groupToEdit = _db.SportGroup.Find(groupId);
            if(groupToEdit == null)
                return new EmptyResult();

            User user = GetUserFromRequest();
            if (user.GoogleId != groupToEdit.CreatedBy.GoogleId)
                return new EmptyResult();

            CreateEditGroupContainer viewContainer = new CreateEditGroupContainer()
            {
                SportGroupToEdit = groupToEdit,
                AllActivityTags = _db.ActivityTag.AsNoTracking().ToList(),
                AllGenderRestrictionTags = _db.GenderRestrictionTag.AsNoTracking().ToList(),
                GroupCreator = user
            };

            return View("CreateEditGroup", viewContainer);
        }

        public IActionResult AddGroup(AddEditGroupRequest request)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            if (!IsGroupValid(request))
                return new EmptyResult();

            User groupCreator = GetUserFromRequest();
            SportGroup newGroup = new SportGroup()
            {
                Id = _db.GetUniqueSportGroupId(),
                GroupName = request.GroupName,
                MeetupDate = request.MeetupDate.Value,
                MaxParticipants = request.MaxParticipants,
                CreatedBy = groupCreator,
                ActivityTags = _db.ActivityTag.Where(at => request.ActivityCategories.Contains(at.ActivityCategory)).ToHashSet(),
                GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction),
                MeetupRecurrence = request.MeetupRecurrence,
                IsPrivate = request.IsPrivate,
                Latitude = Convert.ToDouble(request.Latitude, CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(request.Longitude, CultureInfo.InvariantCulture)
            };
            newGroup.Participants.Add(groupCreator);

            _db.SportGroup.Add(newGroup);
            _db.SaveChanges();

            return Redirect("CreateGroup");
        }

        private bool IsGroupValid(AddEditGroupRequest request)
        {
            return (request.GroupName.Length > 0 &&
                request.GroupName.Length <= ApplicationConstants.MaxGroupNameLength &&
                request.ActivityCategories.Count > 0 &&
                request.MeetupDate.HasValue &&
                !string.IsNullOrEmpty(request.Latitude) &&
                !string.IsNullOrEmpty(request.Longitude));
        }

        public IActionResult UpdateGroup(AddEditGroupRequest request, Guid groupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            if (!IsGroupValid(request))
                return new EmptyResult();

            SportGroup groupToUpdate = _db.SportGroup.Find(groupId);
            if (groupToUpdate == null)
                return new EmptyResult();

            User user = GetUserFromRequest();
            if (user.GoogleId != groupToUpdate.CreatedBy.GoogleId)
                return new EmptyResult();

            groupToUpdate.GroupName = request.GroupName;
            groupToUpdate.MeetupDate = request.MeetupDate.Value;
            groupToUpdate.MaxParticipants = request.MaxParticipants;
            groupToUpdate.GenderRestrictionTag = _db.GenderRestrictionTag.Find(request.GenderRestriction);
            groupToUpdate.MeetupRecurrence = request.MeetupRecurrence;
            groupToUpdate.IsPrivate = request.IsPrivate;
            groupToUpdate.Latitude = Convert.ToDouble(request.Latitude, CultureInfo.InvariantCulture);
            groupToUpdate.Longitude = Convert.ToDouble(request.Longitude, CultureInfo.InvariantCulture);

            //Warning: overwriting the groupToEdit.ActivityTags reference directly could throw an exception, thats why the list is just refilled
            groupToUpdate.ActivityTags.Clear();
            List<ActivityTag> requestedActivityTags = request.ActivityCategories.Select(ac => _db.ActivityTag.Find(ac)).ToList();
            requestedActivityTags.ForEach(at => groupToUpdate.ActivityTags.Add(at));

            UpdateParticipants(groupToUpdate, request);

            _db.SaveChanges();

            return Redirect("MyGroups");
        }

        private void UpdateParticipants(SportGroup groupToUpdate, AddEditGroupRequest request)
        {
            List<string> oldParticipantsIds = groupToUpdate.Participants.Select(p => p.GoogleId).ToList();
            List<string> newParticipantsIds = request.ParticipantsIds;
            List<string> participantsIdsToRemove = oldParticipantsIds.Where(op => !newParticipantsIds.Contains(op)).ToList();

            foreach(var participantId in participantsIdsToRemove)
            {
                User userToRemove = _db.User.Find(participantId);
                if(userToRemove != groupToUpdate.CreatedBy)
                    groupToUpdate.Participants.Remove(userToRemove);
            }
        }

        public IActionResult JoinGroup(Guid groupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            SportGroup groupToJoin = _db.SportGroup.Find(groupId);
            if (groupToJoin == null)
                return Json(new { success = false });

            User user = GetUserFromRequest();
            groupToJoin.Participants.Add(user);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult LeaveGroup(Guid groupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            SportGroup groupToLeave = _db.SportGroup.Find(groupId);
            if (groupToLeave == null)
                return Json(new { success = false });

            User user = GetUserFromRequest();
            groupToLeave.Participants.Remove(user);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult DeleteGroup(Guid groupId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            SportGroup groupToDelete = _db.SportGroup.Find(groupId);
            if (groupToDelete == null)
                return Json(new { success = false });

            User user = GetUserFromRequest();
            if (user.GoogleId != groupToDelete.CreatedBy.GoogleId)
                return Json(new { success = false });

            _db.SportGroup.Remove(groupToDelete);
            _db.SaveChanges();

            return Json(new { success = true });
        }

        public ActionResult GetParticipantsPartial(Guid groupId)
        {
            SportGroup sportGroup = _db.SportGroup.Find(groupId);
            if (sportGroup == null)
                return Json(new { success = false });

            ParticipantsPartialContainer container = new ParticipantsPartialContainer()
            {
                SportGroup = sportGroup,
                HttpContext = HttpContext
            };

            return PartialView("~/Views/Shared/_ParticipantsPartial.cshtml", container);
        }

        public ActionResult GetMapPopupPartial(Guid groupId)
        {
            SportGroup sportGroup = _db.SportGroup.Find(groupId);
            if (sportGroup == null)
                return Json(new { success = false });

            return PartialView("~/Views/Shared/_MapPopupPartial.cshtml", sportGroup);
        }

        public ActionResult GetConfirmJoinPartial(Guid groupId)
        {
            SportGroup sportGroup = _db.SportGroup.Find(groupId);
            if (sportGroup == null)
                return Json(new { success = false });

            return PartialView("~/Views/Shared/_ConfirmJoinPartial.cshtml", sportGroup);
        }

        private static readonly Random _rnd = new Random();
        public IActionResult CreateDemoData()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Start", "Site");

            User testUser = _db.User.Find(string.Concat(Enumerable.Repeat("0", 21)));
            if (testUser == null)
            {
                testUser = new User()
                {
                    GoogleId = string.Concat(Enumerable.Repeat("0", 21)),
                    Username = "TestUser",
                    Gender = GenderType.Male,
                    Birthday = DateTime.Today.AddYears(-18),
                    PictureUrl = "https://i.stack.imgur.com/34AD2.jpg"
                };

                _db.User.Add(testUser);
            }

            for(int i = 1; i < 20; i++)
            {
                HashSet<ActivityTag> testActivityTags = new HashSet<ActivityTag>();
                int numberOfTestActivityTags = _rnd.Next(1, 3);
                for (int u = 0; u < numberOfTestActivityTags; u++)
                    testActivityTags.Add(_db.ActivityTag.Find((ActivityCategoryType)_rnd.Next(0, 9)));

                SportGroup testGroup = new SportGroup()
                {
                    Id = _db.GetUniqueSportGroupId(),
                    GroupName = $"Test Gruppe {i}",
                    CreatedBy = testUser,
                    MeetupDate = DateTime.Now.AddDays(_rnd.Next(1, 30)),
                    MaxParticipants = _rnd.Next(2, 20),
                    ActivityTags = testActivityTags,
                    GenderRestrictionTag = _db.GenderRestrictionTag.Find((GenderRestrictionType)_rnd.Next(0, 3)),
                    Latitude = 52 + _rnd.NextDouble(),
                    Longitude = 12 + (2 * _rnd.NextDouble())
                };
                testGroup.Participants.Add(testUser);
                _db.SportGroup.Add(testGroup);
            }

            _db.SaveChanges();
            return Redirect("DiscoverGroups");
        }

        private User GetUserFromRequest()
        {
            string googleId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            return _db.User.Find(googleId);
        }
    }
}
