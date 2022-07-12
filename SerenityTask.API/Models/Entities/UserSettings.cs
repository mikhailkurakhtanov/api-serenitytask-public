using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class UserSettings : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "timerSettings")]
        public string TimerSettings { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        #region Virtual

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion
    }
}