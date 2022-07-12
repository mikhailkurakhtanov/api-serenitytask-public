using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class PlantType
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty(PropertyName = "maxLeaves")]
        public int MaxLeaves { get; set; }

        #region Virtual

        [JsonProperty(PropertyName = "plants")]
        public virtual ICollection<Plant> Plants { get; set; }

        #endregion
    }
}
