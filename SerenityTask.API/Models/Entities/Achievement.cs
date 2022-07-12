using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Achievement : IBaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "userDetailsId")]
        public long UserDetailsId { get; set; }

        [ForeignKey("UserDetailsId")]
        [JsonIgnore]
        public virtual UserDetails UserDetails { get; set; }

        [JsonProperty(PropertyName = "type")]
        public virtual AchievementType Type { get; set; }
    }
}