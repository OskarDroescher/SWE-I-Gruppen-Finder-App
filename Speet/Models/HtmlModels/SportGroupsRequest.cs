using System;
using System.Collections.Generic;

namespace Speet.Models.HtmlModels
{
    public class SportGroupsRequest
    {
        public List<ActivityCategoryType> ActivityCategories { get; set; }
        public List<GenderRestrictionType> GenderRestrictions { get; set; }
        public int MaxDistance { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }
    }
}
