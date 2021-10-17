using System.Collections.Generic;

namespace Speet.Models.ContainerModels
{
    public class CreateEditGroupContainer
    {
        public SportGroup SportGroupToEdit { get; set; }
        public List<ActivityTag> AllActivityTags { get; set; }
        public List<GenderRestrictionTag> AllGenderRestrictionTags { get; set; }
    }
}
