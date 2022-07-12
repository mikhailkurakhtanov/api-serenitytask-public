using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Session : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        public DateTime StartDate { get; set; }

        [JsonIgnore]
        public DateTime EndDate { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "creationDateString")]
        public string CreationDateString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "startDateString")]
        public string StartDateString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "endDateString")]
        public string EndDateString { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; set; }

        [JsonProperty(PropertyName = "isHardModeActivated")]
        public bool IsHardModeActived { get; set; } = false;

        [JsonProperty(PropertyName = "sessionMembersJSON")]
        public string SessionMembersJSON { get; set; }

        [JsonProperty(PropertyName = "googleCalendarEventId")]
        public string GoogleCalendarEventId { get; set; }

        [JsonProperty(PropertyName = "ownerId")]
        public Guid OwnerId { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "participantsIds")]
        public IList<string> ParticipantsIds { get; set; }

        #region Virtual

        [ForeignKey("OwnerId")]
        [JsonProperty(PropertyName = "owner")]
        public virtual User Owner { get; set; }

        [JsonProperty(PropertyName = "participants")]
        public virtual ICollection<User> Participants { get; set; }

        #endregion
    }
}