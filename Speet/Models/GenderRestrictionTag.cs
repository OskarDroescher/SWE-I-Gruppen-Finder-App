using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class GenderRestrictionTag
    {
        [Key]
        public GenderRestrictionType GenderRestriction { get; set; }

        public string IconUrl { get; set; }

        public virtual ICollection<SportGroup> AssignedGroups { get; set; }

        public GenderRestrictionTag()
        {
            AssignedGroups = new HashSet<SportGroup>();
        }
    }

    public enum GenderRestrictionType
    {
        NoRestriction,
        MaleOnly,
        FemaleOnly,
        OtherOnly
    }
}
