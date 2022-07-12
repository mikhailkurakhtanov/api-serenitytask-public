using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Responses
{
    public class CompleteTaskResponse
    {
        [JsonProperty(PropertyName = "completedTaskId")]
        public long CompletedTaskId { get; set; }

        [JsonProperty(PropertyName = "parentTask")]
        public Task ParentTask { get; set; }
    }
}