using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class SystemNotification : IBaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTime CreationDate { get; set; }

        [Required]
        [JsonProperty(PropertyName = "type")]
        public SystemNotificationType Type { get; set; }

        [JsonProperty(PropertyName = "isRead")]
        public bool IsRead { get; set; } = false;

        [Required]
        [ForeignKey("ReceiverId")]
        [JsonProperty(PropertyName = "receiver")]
        public virtual User Receiver { get; set; }
    }

    public enum SystemNotificationType
    {
        TaskReminder = 0,
        AppUpdate = 1,
        NewsFromAdmin = 2
    }
}