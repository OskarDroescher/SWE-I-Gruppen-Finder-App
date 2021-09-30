using System;
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
        public byte NumberOfParticipants { get; set; }

        [Required]
        public DateTime MeetupDate { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
