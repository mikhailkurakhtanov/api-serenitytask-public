using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client
{
    public class SessionMember
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }

        [JsonProperty(PropertyName = "isJoined")]
        public bool IsJoined { get; set; } = false;

        [JsonProperty(PropertyName = "isReady")]
        public bool IsReady { get; set; } = false;

        [JsonProperty(PropertyName = "isDisconnected")]
        public bool IsDisconnected { get; set; } = false;

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public List<SessionMemberTask> SessionMemberTasks { get; set; }
    }

    public class SessionMemberTask
    {
        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }

        [JsonProperty(PropertyName = "startTrackingDate")]
        public DateTime StartTrackingDate { get; set; }

        [JsonProperty(PropertyName = "endTrackingDate")]
        public DateTime? EndTrackingDate { get; set; }

        [JsonProperty(PropertyName = "trackedTime")]
        public double TrackedTime { get; set; } = 0;
    }
}