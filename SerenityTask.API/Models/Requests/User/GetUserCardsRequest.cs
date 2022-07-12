using System.Collections.Generic;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.User
{
    public class GetUserCardsRequest
    {
        [JsonProperty(PropertyName = "interests")]
        public List<string> Interests { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public List<string> Languages { get; set; }

        [JsonProperty(PropertyName = "timezones")]
        public List<string> Timezones { get; set; }

        [JsonProperty(PropertyName = "isUserOnline")]
        public bool? IsUserOnline { get; set; }

        [JsonProperty(PropertyName = "usernameOrEmail")]
        public string UsernameOrEmail { get; set; }
    }
}