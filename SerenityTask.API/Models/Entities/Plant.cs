using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Plant
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "creationDate")]
        public DateTime? CreationDate { get; set; } = DateTime.UtcNow;

        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; } = 0;

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; } = 0;

        [JsonProperty(PropertyName = "currentExperience")]
        public double CurrentExperience { get; set; } = 0;

        [JsonProperty(PropertyName = "maxExperience")]
        public int MaxExperience { get; set; } = 10;

        [JsonProperty(PropertyName = "totalDeadLeaves")]
        public int TotalDeadLeaves { get; set; } = 0;

        [JsonProperty(PropertyName = "isDead")]
        public bool IsDead { get; set; } = false;

        [JsonProperty(PropertyName = "isGrowthFinished")]
        public bool IsGrowthFinished { get; set; } = false;

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        #region Virtual

        [ForeignKey("PlantTypeId")]
        [JsonProperty(PropertyName = "plantType")]
        public virtual PlantType PlantType { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<PlantHistoryNote> PlantHistory { get; set; }

        #endregion
    }
}
