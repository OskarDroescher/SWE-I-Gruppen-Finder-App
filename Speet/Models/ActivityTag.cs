using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class ActivityTag
    {
        [Key]
        public ActivityCategoryType ActivityCategory { get; set; }

        public string IconUrl { get; set; }

        public virtual ICollection<SportGroup> AssignedGroups { get; set; }

        public ActivityTag()
        {
            AssignedGroups = new HashSet<SportGroup>();
        }
    }

    public enum ActivityCategoryType
    {
        TrackAndField,
        Football,
        Basketball,
        Swimming,
        WaterPolo,
        Cycling,
        Badminton,
        TableTennis,
        Boating,
        Other
    }
}
