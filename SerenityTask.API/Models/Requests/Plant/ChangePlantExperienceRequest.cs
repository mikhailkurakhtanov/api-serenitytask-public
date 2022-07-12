using System;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.Plant
{
    public class ChangePlantExperienceRequest
    {
        [JsonProperty(PropertyName = "changingType")]
        public ExperienceChangingType ChangingType { get; set; }

        [JsonProperty(PropertyName = "objectName")]
        public ExperienceObjectType ObjectType { get; set; }

        [JsonProperty(PropertyName = "reasonType")]
        public ExperienceReasonType ReasonType { get; set; }

        [JsonProperty(PropertyName = "plantId")]
        public long PlantId { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long? TaskId { get; set; }

        [JsonProperty(PropertyName = "trackedTimeInMinutes")]
        public long? TrackedTimeInMinutes { get; set; }

        [JsonProperty(PropertyName = "sessionId")]
        public long? SessionId { get; set; }

        [JsonProperty(PropertyName = "guiltyMemberId")]
        public Guid? GuiltyMemberId { get; set; }
    }

    public enum ExperienceChangingType
    {
        Rise = 1,
        Reduce = 2
    }

    public enum ExperienceObjectType
    {
        Task = 1,
        Session = 2
    }

    public enum ExperienceReasonType
    {
        Task_Completed = 1,
        Task_TrackedTime = 2,
        Task_Deleted = 3,
        Task_ChangedDeadline = 4,
        Task_ExpiredDeadline = 5,

        Session_Finished = 11,
        Session_Canceled = 12,
        Session_Leaved = 13,
        Session_Interrupted = 14
    }
}