using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SerenityTask.API.Models.Client.Authentication
{
    public class AuthenticationOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string SecretKey { get; set; }

        public double TokenLifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        }
    }
}
