using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class User
    {
        [Key]
        public string GoogleId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime? Birthday { get; set; }

        [Required]
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
