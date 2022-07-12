using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.Hub
{
    public class UpdateHubConnectionDetailsRequest
    {
        [JsonProperty(PropertyName = "hubConnectionId")]
        public string HubConnectionId { get; set; }

        [JsonProperty(PropertyName = "browser")]
        public string Browser { get; set; }

        [JsonProperty(PropertyName = "browserVersion")]
        public string BrowserVersion { get; set; }

        [JsonProperty(PropertyName = "os")]
        public string OS { get; set; }

        [JsonProperty(PropertyName = "osVersion")]
        public string OSVersion { get; set; }
    }
}