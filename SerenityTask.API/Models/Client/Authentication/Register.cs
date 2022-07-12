using Newtonsoft.Json;

namespace SerenityTask.API.Models.Client.Authentication
{
    public class Register
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "areConditionsAgreed")]
        public bool AreConditionsAgreed { get; set; }
    }
}
