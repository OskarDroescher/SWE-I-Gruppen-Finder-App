using System;
using System.Collections.Generic;

namespace Speet.Models.HttpRequestModels
{
    public class AddEditGroupRequest
    {
        public string GroupName { get; set; }
        public List<ActivityCategoryType> ActivityCategories { get; set; }
        public GenderRestrictionType GenderRestriction { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime? MeetupDate { get; set; }

        public AddEditGroupRequest()
        {
            ActivityCategories = new List<ActivityCategoryType>();
        }
    }
}
