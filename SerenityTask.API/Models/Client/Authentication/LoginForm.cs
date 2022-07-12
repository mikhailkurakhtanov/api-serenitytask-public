using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client.Authentication
{
    public class LoginForm
    {
        [Required]
        [JsonProperty(PropertyName = "login")]
        public string Login { get; set; }

        [Required]
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "rememberMe")]
        public bool RememberMe { get; set; }
    }
}
