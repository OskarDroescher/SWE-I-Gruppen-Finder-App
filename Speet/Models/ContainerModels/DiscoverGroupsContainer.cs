﻿using Speet.Models.HttpRequestModels;
using System.Collections.Generic;

namespace Speet.Models.ContainerModels
{
    public class DiscoverGroupsContainer
    {
        public List<SportGroup> SportGroupsToDisplay { get; set; }
        public FilterSettingsRequest FilterSettings { get; set; }
        public List<ActivityTag> AllActivityTags { get; set; }
        public List<GenderRestrictionTag> AllGenderRestrictionTags { get; set; }
        public int PageIndex { get; set; }
        public int[] NextPageIndexes { get; set; }
        public int[] PreviousPageIndexes { get; set; }
    }
}
