using System.Collections.Generic;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class AchievementType : IBaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "goal")]
        public int Goal { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "rate")]
        public AchievementTypeRate Rate { get; set; }

        [JsonIgnore]
        public virtual ICollection<Achievement> Achievements { get; set; }
    }

    public enum AchievementTypeRate
    {
        Ordinary = 0,
        Bronze = 1,
        Silver = 2,
        Gold = 3
    }
}