using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class GoogleCredential : IBaseEntity
    {
        [Key]
        [JsonIgnore]
        public long Id { get; set; }

        [JsonIgnore]
        public string Email { get; set; }

        [JsonIgnore]
        public string Token { get; set; }

        [JsonIgnore]
        public string TokenId { get; set; }

        [JsonIgnore]
        public string TokenType { get; set; }

        [JsonProperty("expiresAt")]
        public long ExpiresAt { get; set; }

        [JsonIgnore]
        public long? ExpiresIn { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public string Scope { get; set; }

        [JsonIgnore]
        public DateTime IssuedUtc { get; set; }

        [JsonIgnore]
        public string CalendarId { get; set; }

        [JsonIgnore]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}