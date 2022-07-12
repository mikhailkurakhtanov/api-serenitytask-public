using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SerenityTask.API.Models.Requests.Plant;

namespace SerenityTask.API.Models.Entities
{
    public class PlantHistoryNote
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "actionDate")]
        public DateTime? ActionDate { get; set; }

        [Required]
        [JsonProperty(PropertyName = "receivedExperience")]
        public double ReceivedExperience { get; set; }

        [Required]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "taskDetailsJSON")]
        public string TaskDetailsJSON { get; set; }

        [JsonProperty(PropertyName = "sessionDetailsJSON")]
        public string SessionDetailsJSON { get; set; }

        [JsonProperty(PropertyName = "plantId")]
        public long PlantId { get; set; }

        [JsonProperty(PropertyName = "experienceObjectType")]
        public ExperienceObjectType ExperienceObjectType { get; set; }

        #region Virtual

        [Required]
        [JsonIgnore]
        [ForeignKey("PlantId")]
        public virtual Plant Plant { get; set; }

        [JsonIgnore]
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        #endregion
    }
}
