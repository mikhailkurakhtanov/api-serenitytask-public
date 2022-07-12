using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Client
{
    public class UserNotificationView
    {
        [JsonProperty(PropertyName = "userNotification")]
        public UserNotification UserNotification { get; set; }

        [JsonProperty(PropertyName = "senderName")]
        public string SenderName { get; set; }

        [JsonProperty(PropertyName = "senderAvatar")]
        public string SenderAvatar { get; set; }
    }
}