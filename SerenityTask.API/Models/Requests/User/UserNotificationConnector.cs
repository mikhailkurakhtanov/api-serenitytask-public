using System;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests
{
    public class UserNotificationConnector
    {
        [JsonProperty(PropertyName = "senderId")]
        public Guid SenderId { get; set; }

        [JsonProperty(PropertyName = "receiverId")]
        public Guid ReceiverId { get; set; }
    }
}