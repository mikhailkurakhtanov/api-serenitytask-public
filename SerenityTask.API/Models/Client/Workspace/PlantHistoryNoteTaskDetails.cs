using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client.Workspace
{
    public class PlantHistoryNoteTaskDetails
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; } = 0;

        [JsonProperty(PropertyName = "isCompleted")]
        public bool IsCompleted { get; set; }
    }
}