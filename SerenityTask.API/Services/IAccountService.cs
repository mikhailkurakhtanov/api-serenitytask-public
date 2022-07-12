using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface IAccountService
    {
        bool CheckEmailOnAvailability(string userEmail);

        Task<SettingsNotification> ChangeEmail(string newEmail, Guid userId);

        Task<SettingsNotification> ChangePassword(string newPassword, Guid userId);

        Task ConfirmEmail(string newEmail, string token, Guid userId);

        Task ConfirmPassword(string newPassword, string token, Guid userId);

        List<SettingsNotification> GetSettingsNotifications(Guid userId);

        Task DeleteSettingsNotification(SettingsNotificationType notificationId);

        Task<bool> DeleteAccount(Guid userId, string currentPassword);
    }
}
