using System;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.Session
{
    public class SetReadyStatusForJoinedMemberRequest
    {
        [JsonProperty(PropertyName = "isReady")]
        public bool IsReady { get; set; }

        [JsonProperty(PropertyName = "sessionId")]
        public long SessionId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }
    }
}