using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class User
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [Required]
        [JsonProperty(PropertyName = "passwordHash")]
        public string PasswordHash { get; set; }

        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }

        [JsonProperty(PropertyName = "isUserOnline")]
        public bool IsUserOnline { get; set; }

        [JsonProperty(PropertyName = "roleId")]
        public int RoleId { get; set; }

        [JsonProperty(PropertyName = "isGoogleCalendarConnected")]
        public bool IsGoogleCalendarConnected { get; set; } = false;

        #region Virtual

        [JsonIgnore]
        public virtual ICollection<Session> OwnedSessions { get; set; }

        [JsonIgnore]
        public virtual ICollection<Session> Sessions { get; set; }

        [JsonProperty("googleCredential")]
        public virtual GoogleCredential GoogleCredential { get; set; }

        [Required]
        [ForeignKey("RoleId")]
        [JsonIgnore]
        public virtual Role Role { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserConnector> UserConnectors { get; set; }

        [JsonProperty(PropertyName = "userDetails")]
        public virtual UserDetails UserDetails { get; set; }

        [JsonProperty(PropertyName = "userSettings")]
        public virtual UserSettings UserSettings { get; set; }

        [JsonIgnore]
        public virtual ICollection<Plant> Plants { get; set; }

        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<File> Files { get; set; }

        [JsonIgnore]
        public virtual ICollection<ConfirmationToken> AccountConfirmationTokens { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual ICollection<SystemNotification> SystemNotifications { get; set; }

        public virtual ICollection<SettingsNotification> SettingsNotifications { get; set; }

        [JsonIgnore]
        public virtual ICollection<HubConnection> HubConnections { get; set; }

        [JsonIgnore]
        public virtual ICollection<SessionRequest> SendedSessionRequests { get; set; }

        [JsonIgnore]
        public virtual ICollection<SessionRequest> ReceivedSessionRequests { get; set; }

        [JsonIgnore]
        public virtual GoogleCalendarAccessRequest GoogleCalendarAccessRequest { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProblemReport> ProblemReports { get; set; }

        #endregion
    }
}
