using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class User
    {
        [Key]
        public string GoogleId { get; set; }

        public GenderType Gender { get; set; }

        public virtual ICollection<SportGroup> CreatedGroups { get; set; }

        public virtual ICollection<SportGroup> JoinedGroups { get; set; }

        public User()
        {
            CreatedGroups = new HashSet<SportGroup>();
            JoinedGroups = new HashSet<SportGroup>();
        }
    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }
}
