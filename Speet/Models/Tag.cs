using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class Tag
    {
        [Key]
        public string Description { get; set; }

        public string IconUrl { get; set; }

        public virtual ICollection<SportGroup> AssignedGroups { get; set; }

        public Tag()
        {
            AssignedGroups = new HashSet<SportGroup>();
        }
    }
}
