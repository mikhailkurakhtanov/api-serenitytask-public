using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SerenityTask.API.Models.Enums;

namespace SerenityTask.API.Models.Entities
{
    public class HubConnection : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "hubConnectionId")]
        public string HubConnectionId { get; set; }

        [JsonProperty(PropertyName = "browser")]
        public string Browser { get; set; }

        [JsonProperty(PropertyName = "browserVersion")]
        public string BrowserVersion { get; set; }

        [JsonProperty(PropertyName = "os")]
        public string OS { get; set; }

        [JsonProperty(PropertyName = "osVersion")]
        public string OSVersion { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonProperty(PropertyName = "user")]
        public virtual User User { get; set; }
    }
}