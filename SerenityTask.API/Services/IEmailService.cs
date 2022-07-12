using System.Threading.Tasks;
using SerenityTask.API.Models.Client.Authentication;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Authentication;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface IEmailService
    {
        void SendWelcomeEmail(string userEmail, string username);

        void SendRegisterConfirmationEmail(Register formData);

        Task SendPasswordRecoveryConfirmationEmail(ChangeAccountPasswordRequest request);

        Task<SettingsNotification> SendEmailConfirmationEmail(User currentUser, string newUserEmail);

        Task<SettingsNotification> SendPasswordConfirmationEmail(User currentUser, string newUserPassword);
    }
}
