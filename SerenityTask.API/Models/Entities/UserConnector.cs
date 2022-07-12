using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class UserConnector
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        [JsonProperty(PropertyName = "friendId")]
        public Guid FriendId { get; set; }

        #region Virtual

        [Required]
        [ForeignKey("UserId")]
        [JsonProperty(PropertyName = "user")]
        public virtual User User { get; set; }

        [Required]
        [ForeignKey("FriendId")]
        [JsonProperty(PropertyName = "friend")]
        public virtual User Friend { get; set; }

        #endregion
    }
}