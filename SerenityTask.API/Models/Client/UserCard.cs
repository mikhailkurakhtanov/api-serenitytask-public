using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Client
{
    public class UserCard
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "age")]
        public string Age { get; set; }

        [JsonProperty(PropertyName = "timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty(PropertyName = "interests")]
        public IList<string> Interests { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public IList<string> Languages { get; set; }

        [JsonProperty(PropertyName = "lookingFor")]
        public string LookingFor { get; set; }

        [JsonProperty(PropertyName = "isUserOnline")]
        public bool IsUserOnline { get; set; }

        [JsonProperty(PropertyName = "achievements")]
        public ICollection<Achievement> Achievements { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }
    }
}