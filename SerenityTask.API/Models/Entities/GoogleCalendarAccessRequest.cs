using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SerenityTask.API.Models.Entities
{
    public class GoogleCalendarAccessRequest : IBaseEntity
    {
        [Key]
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public bool IsAccessGranted { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}