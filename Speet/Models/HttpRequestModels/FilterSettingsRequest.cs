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
        public DateTime MaxDate { get; set; }
        public DateTime MinDate { get; set; }

        public FilterSettingsRequest()
        {
            //Default values on first loadup of DiscoverGroups view
            ActivityCategories = new List<ActivityCategoryType>();
            GenderRestrictions = new List<GenderRestrictionType>();
            MaxDistance = 250;
            MaxParticipants = 20;
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

            //Check if default value to get same result in URL bar as on filter update
            string minDateString = MinDate.ToString("yyyy-MM-dd");
            if (minDateString == "0001-01-01")
                minDateString = string.Empty;

            queryString.Add("MinDate", minDateString);

            string maxDateString = MaxDate.ToString("yyyy-MM-dd");
            if (maxDateString == "0001-01-01")
                maxDateString = string.Empty;

            queryString.Add("MaxDate", maxDateString);

            return queryString.ToString();
        }
    }
}
