using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class SettingsNotification
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }

        [Required]
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [Required]
        [JsonProperty(PropertyName = "type")]
        public SettingsNotificationType Type { get; set; }

        #region Virtual

        [Required]
        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion
    }

    public enum SettingsNotificationType
    {
        Email = 0,
        Password = 1
    }
}
