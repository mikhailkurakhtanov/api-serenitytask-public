using System;
using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Client.Workspace
{
    public class CalendarEvent
    {
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        [JsonProperty(PropertyName = "start")]
        public DateTime? Start { get; set; }

        [JsonProperty(PropertyName = "end")]
        public DateTime? End { get; set; }

        [JsonProperty(PropertyName = "isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}