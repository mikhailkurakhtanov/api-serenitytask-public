using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class UserDetails
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; set; }

        [JsonProperty(PropertyName = "age")]
        public string Age { get; set; }

        [JsonProperty(PropertyName = "interests")]
        public string Interests { get; set; }

        [JsonProperty(PropertyName = "languages")]
        public string Languages { get; set; }

        [JsonProperty(PropertyName = "discordTag")]
        public string DiscordTag { get; set; }

        [JsonProperty(PropertyName = "lookingFor")]
        public string LookingFor { get; set; }

        [JsonProperty(PropertyName = "telegramUsername")]
        public string TelegramUsername { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        [ForeignKey("TimeZoneId")]
        [JsonProperty(PropertyName = "timeZone")]
        public virtual TimeZoneType TimeZone { get; set; }

        [JsonProperty(PropertyName = "achievements")]
        public virtual ICollection<Achievement> Achievements { get; set; }
    }
}
