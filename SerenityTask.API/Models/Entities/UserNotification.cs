using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class UserNotification : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "creationDateString")]
        public string CreationDateString { get; set; }

        [Required]
        [JsonProperty(PropertyName = "type")]
        public UserNotificationType Type { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "isRead")]
        public bool IsRead { get; set; } = false;

        [JsonProperty(PropertyName = "senderId")]
        public Guid SenderId { get; set; }

        [JsonIgnore]
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [JsonProperty(PropertyName = "receiverId")]
        public Guid ReceiverId { get; set; }

        [JsonIgnore]
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }
    }

    public enum UserNotificationType
    {
        FriendRequest = 0,
        Message = 1,
        SessionInvitation = 2,
        SessionApprovement = 3,
        UpcomingSessionReminder = 4
    }
}