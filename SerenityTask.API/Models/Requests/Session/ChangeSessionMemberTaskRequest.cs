using System;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.Session
{
    public class ChangeSessionMemberTaskRequest
    {
        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }

        [JsonProperty(PropertyName = "sessionId")]
        public long SessionId { get; set; }
    }
}