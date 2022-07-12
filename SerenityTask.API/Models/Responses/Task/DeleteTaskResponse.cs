using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Responses
{
    public class DeleteTaskResponse
    {
        [JsonProperty(PropertyName = "deletedTaskId")]
        public long DeletedTaskId { get; set; }

        [JsonProperty(PropertyName = "parentTask")]
        public Task ParentTask { get; set; }
    }
}