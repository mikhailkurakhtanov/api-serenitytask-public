using System;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client
{
    public class FriendInfo
    {
        [JsonProperty(PropertyName = "friendId")]
        public Guid FriendId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "telegramUsername")]
        public string TelegramUsername { get; set; }

        [JsonProperty(PropertyName = "discordTag")]
        public string DiscordTag { get; set; }

        [JsonProperty(PropertyName = "isUserOnline")]
        public bool isUserOnline { get; set; }
    }
}