using System;
using Newtonsoft.Json;
using SerenityTask.API.Models.Requests.Plant;

namespace SerenityTask.API.Models.Client.Workspace
{
    public class PlantHistoryView
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonIgnore]
        public DateTime? ActionDate { get; set; }

        [JsonProperty(PropertyName = "actionDateString")]
        public string ActionDateString { get; set; }

        [JsonProperty(PropertyName = "receivedExperience")]
        public double ReceivedExperience { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "taskDetails")]
        public PlantHistoryNoteTaskDetails TaskDetails { get; set; }

        [JsonProperty(PropertyName = "sessionDetails")]
        public PlantHistoryNoteSessionDetails SessionDetails { get; set; }

        [JsonProperty(PropertyName = "plantId")]
        public long PlantId { get; set; }

        [JsonProperty(PropertyName = "experienceObjectType")]
        public ExperienceObjectType ExperienceObjectType { get; set; }
    }
}