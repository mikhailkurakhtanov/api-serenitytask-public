using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client.Authentication
{
    public class LoginResponse
    {
        [JsonProperty(PropertyName = "isPasswordCorrect")]
        public bool IsPasswordCorrect { get; set; } = false;

        [JsonProperty(PropertyName = "isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; } = false;

        [JsonProperty(PropertyName = "authorizationToken")]
        public string AuthorizationToken { get; set; } = "";
    }
}
