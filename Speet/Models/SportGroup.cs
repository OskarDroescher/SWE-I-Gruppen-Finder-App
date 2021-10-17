using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class SportGroup
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public DateTime MeetupDate { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual ICollection<User> Participants { get; set; }

        public virtual ICollection<ActivityTag> ActivityTags { get; set; }

        public virtual GenderRestrictionTag GenderRestrictionTag { get; set; }

        public SportGroup()
        {
            Participants = new HashSet<User>();
            ActivityTags = new HashSet<ActivityTag>();
        }
    }
}
