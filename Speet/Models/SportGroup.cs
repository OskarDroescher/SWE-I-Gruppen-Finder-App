using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Speet.Models
{
    public class SportGroup
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public DateTime MeetupDate { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public MeetupRecurrenceType MeetupRecurrence { get; set; }

        [Required]
        public bool IsPrivate { get; set; }

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

    public enum MeetupRecurrenceType
    {
        OneTime,
        Daily,
        Weekly,
        Monthly
    }
}
