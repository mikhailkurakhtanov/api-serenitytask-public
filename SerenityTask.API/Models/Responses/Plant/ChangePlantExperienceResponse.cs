using Newtonsoft.Json;
using SerenityTask.API.Models.Client.Workspace;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Responses.Plant
{
    public class ChangePlantExperienceResponse
    {
        [JsonProperty(PropertyName = "experience")]
        public double Experience { get; set; }

        [JsonProperty(PropertyName = "maxExperience")]
        public int MaxExperience { get; set; }

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; }

        [JsonProperty(PropertyName = "plantHistoryView")]
        public PlantHistoryView PlantHistoryView { get; set; }
    }
}
