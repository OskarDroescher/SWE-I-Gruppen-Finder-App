using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Speet.Models.HttpRequestModels
{
    public class FilterSettingsRequest
    {
        public List<ActivityCategoryType> ActivityCategories { get; set; }
        public List<GenderRestrictionType> GenderRestrictions { get; set; }
        public int MaxDistance { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public FilterSettingsRequest()
        {
            //Default values on first loadup of DiscoverGroups view
            ActivityCategories = new List<ActivityCategoryType>();
            GenderRestrictions = new List<GenderRestrictionType>();
            MaxDistance = ApplicationConstants.MaxShownSportGroupDistance;
            MaxParticipants = ApplicationConstants.MaxSportGroupParticipants;
        }

        public string GetFilterSettingsQueryString()
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach(var activityCategory in ActivityCategories)
                queryString.Add("ActivityCategories", activityCategory.ToString());

            foreach (var genderRestriction in GenderRestrictions)
                queryString.Add("GenderRestrictions", genderRestriction.ToString());

            queryString.Add("MaxDistance", MaxDistance.ToString());
            queryString.Add("MaxParticipants", MaxParticipants.ToString());

            string minDate = string.Empty;
            if (MinDate.HasValue)
                minDate = MinDate.Value.ToString("yyyy-MM-ddTHH:mm");

            queryString.Add("MinDate", minDate);

            string maxDate = string.Empty;
            if (MaxDate.HasValue)
                maxDate = MaxDate.Value.ToString("yyyy-MM-ddTHH:mm");

            queryString.Add("MaxDate", maxDate);

            return queryString.ToString();
        }
    }
}
