using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class SessionRequest : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonIgnore]
        public DateTime SendingDate { get; set; }

        [JsonIgnore]
        public DateTime Start { get; set; }

        [JsonIgnore]
        public DateTime End { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "sendingDateString")]
        public string SendingDateString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "startString")]
        public string StartString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "endString")]
        public string EndString { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public long Duration { get; set; }

        [JsonProperty(PropertyName = "isRead")]
        public bool IsRead { get; set; } = false;

        [JsonProperty(PropertyName = "senderId")]
        public Guid SenderId { get; set; }

        [JsonProperty(PropertyName = "receiverId")]
        public Guid ReceiverId { get; set; }

        #region Virtual

        [JsonIgnore]
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [JsonIgnore]
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        #endregion
    }
}