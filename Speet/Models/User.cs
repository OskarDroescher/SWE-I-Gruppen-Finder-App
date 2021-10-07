using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class User
    {
        [Key]
        public long GoogleId { get; set; }

        public virtual ICollection<SportGroup> CreatedGroups { get; set; }

        public virtual ICollection<SportGroup> JoinedGroups { get; set; }

        public User()
        {
            CreatedGroups = new HashSet<SportGroup>();
            JoinedGroups = new HashSet<SportGroup>();
        }
    }
}
