using System.Collections.Generic;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class TimeZoneType : IBaseEntity
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "timeZoneId")]
        public string TimeZoneId { get; set; }

        [JsonProperty(PropertyName = "timeZoneIdIANA")]
        public string TimeZoneIdIANA { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        #region Virtual

        [JsonIgnore]
        public virtual ICollection<UserDetails> UserDetails { get; set; }

        #endregion
    }
}