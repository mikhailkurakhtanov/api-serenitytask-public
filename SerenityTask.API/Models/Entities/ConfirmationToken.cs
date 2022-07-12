using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class ConfirmationToken
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [Required]
        [JsonProperty(PropertyName = "type")]
        public TokenType Type { get; set; }

        [Required]
        [JsonProperty(PropertyName = "creationDate")]
        public DateTime CreationDate { get; set; }

        [Required]
        [JsonProperty(PropertyName = "expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonProperty(PropertyName = "activationDate")]
        public DateTime? ActivationDate { get; set; }

        [JsonProperty(PropertyName = "errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty(PropertyName = "isUsed")]
        public bool IsUsed { get; set; }

        [ForeignKey("UserId")]
        [JsonProperty(PropertyName = "user")]
        public virtual User User { get; set; }
    }

    public enum TokenType
    {
        RegisterConfirmation,
        EmailConfirmationOnChange,
        PasswordConfirmationOnChange
    }
}
