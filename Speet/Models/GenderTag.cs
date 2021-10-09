using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class GenderTag
    {
        [Key]
        public GenderRestrictionType GenderRestriction { get; set; }

        public string IconUrl { get; set; }

        public virtual ICollection<SportGroup> AssignedGroups { get; set; }

        public GenderTag()
        {
            AssignedGroups = new HashSet<SportGroup>();
        }
    }

    public enum GenderRestrictionType
    {
        MaleOnly,
        FemaleOnly,
        OtherOnly
    }
}
