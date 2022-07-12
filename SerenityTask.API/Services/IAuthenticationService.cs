using System;
using System.Threading.Tasks;
using SerenityTask.API.Models.Client.Authentication;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Services
{
    public interface IAuthenticationService
    {
        bool CheckUserOnExist(string userEmail);

        LoginResponse ValidateAdminData(LoginForm formData);

        LoginResponse LoginUser(LoginForm loginForm);

        Task<User> RegisterUserAsync(Register formData);

        Task<string> RegisterWithGoogleAsync(SocialUser request);

        Task<string> LoginWithGoogleAsync(SocialUser request);

        Task<bool> CheckAuthorizedUserOnExist(Guid userId);
    }
}
