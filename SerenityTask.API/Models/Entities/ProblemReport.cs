using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class ProblemReport : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "creationDateString")]
        public string CreationDateString { get; set; }

        [MaxLength(30)]
        [Required]
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        [Required]
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "isViewed")]
        public bool IsViewed { get; set; } = false;

        [JsonProperty(PropertyName = "responseToUser")]
        public string ResponseToUser { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}