using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client.Authentication
{
    public class SocialUser
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "authToken")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "idToken")]
        public string TokenId { get; set; }

        [JsonProperty(PropertyName = "response")]
        public GoogleOAuthAccessTokenDetails TokenDetails { get; set; }
    }

    public class GoogleOAuthAccessTokenDetails
    {
        [JsonProperty(PropertyName = "expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }
    }
}