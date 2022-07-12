using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Changelog : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "creationDateString")]
        public string CreationDateString { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "changes")]
        public string Changes { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
}