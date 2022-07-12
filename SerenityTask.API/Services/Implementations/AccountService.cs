using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SerenityTask.API.Extensions;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;
using SerenityTask.API.Models;

namespace SerenityTask.API.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IFileService _fileService;

        private readonly IEmailService _emailService;

        private readonly SerenityTaskDbContext _dbContext;

        public AccountService(IFileService fileService, IEmailService emailService, SerenityTaskDbContext dbContext)
        {
            _fileService = fileService;
            _emailService = emailService;
            _dbContext = dbContext;
        }

        public async Task<SettingsNotification> ChangePassword(string newPassword, Guid userId)
        {
            var currentUser = _dbContext.Users.Find(userId);

            var tokenNotification = await _emailService.SendPasswordConfirmationEmail(currentUser, newPassword);
            return tokenNotification;
        }

        public bool CheckEmailOnAvailability(string userEmail)
        {
            var existingUser = _dbContext.Users.FirstOrDefault(x => x.Email == userEmail);
            return existingUser == null;
        }

        public async Task<SettingsNotification> ChangeEmail(string newEmail, Guid userId)
        {
            var currentUser = _dbContext.Users.Find(userId);

            var tokenNotification = await _emailService.SendEmailConfirmationEmail(currentUser, newEmail);
            return tokenNotification;
        }

        public async Task ConfirmEmail(string newEmail, string token, Guid userId)
        {
            var currentUser = _dbContext.Users.Find(userId);
            if (currentUser.SettingsNotifications.Any(x => x.Type == SettingsNotificationType.Email))
            {
                var passwordNotifications = currentUser.SettingsNotifications.Where(x => x.Type == SettingsNotificationType.Email);
                _dbContext.SettingsNotifications.RemoveRange(passwordNotifications);
            }

            await DeleteSettingsNotification(SettingsNotificationType.Email);

            var newNotification = new SettingsNotification
            {
                Type = SettingsNotificationType.Email,
                User = currentUser
            };

            var existingConfirmationToken = _dbContext.ConfirmationTokens
                .Where(x => x.Token == token).FirstOrDefault();

            if (existingConfirmationToken == null)
            {
                newNotification.Result = false;
                newNotification.Message = "Confirmation token does not exist. Try to change your email again if it needs.";
            }
            else if (existingConfirmationToken.IsUsed)
            {
                newNotification.Result = false;
                newNotification.Message = "This confirmation link was used. Try to change your email again if it needs.";
            }
            else if (DateTime.UtcNow > existingConfirmationToken.ExpirationDate)
            {
                newNotification.Result = false;
                newNotification.Message = "This confirmation link is expired. Try to change your email again if it needs.";
            }
            else
            {
                newNotification.Result = true;
                newNotification.Message = "Email was successfully changed";

                currentUser.Email = newEmail;
                _dbContext.Users.Update(currentUser);

                existingConfirmationToken.IsUsed = true;
                _dbContext.ConfirmationTokens.Update(existingConfirmationToken);
            }

            _dbContext.SettingsNotifications.Add(newNotification);

            await _dbContext.SaveChangesAsync();
        }

        public async Task ConfirmPassword(string newPasswordHash, string token, Guid userId)
        {
            var currentUser = _dbContext.Users.Find(userId);
            if (currentUser.SettingsNotifications.Any(x => x.Type == SettingsNotificationType.Password))
            {
                var passwordNotifications = currentUser.SettingsNotifications.Where(x => x.Type == SettingsNotificationType.Password);
                _dbContext.SettingsNotifications.RemoveRange(passwordNotifications);
            }

            await DeleteSettingsNotification(SettingsNotificationType.Password);

            var newNotification = new SettingsNotification
            {
                Type = SettingsNotificationType.Password,
                User = currentUser
            };

            var existingConfirmationToken = _dbContext.ConfirmationTokens
                .Where(x => x.Token == token).FirstOrDefault();

            if (existingConfirmationToken == null)
            {
                newNotification.Result = false;
                newNotification.Message = "Confirmation token does not exist. Try to change your password again if it needs.";
            }
            else if (existingConfirmationToken.IsUsed)
            {
                newNotification.Result = false;
                newNotification.Message = "This confirmation link was used. Try to change your password again if it needs.";
            }
            else if (DateTime.UtcNow > existingConfirmationToken.ExpirationDate)
            {
                newNotification.Result = false;
                newNotification.Message = "This confirmation link is expired. Try to change your password again if it needs.";
            }
            else
            {
                newNotification.Result = true;
                newNotification.Message = "Password was successfully changed";

                existingConfirmationToken.IsUsed = true;
                _dbContext.ConfirmationTokens.Update(existingConfirmationToken);

                currentUser.PasswordHash = newPasswordHash;
                _dbContext.Users.Update(currentUser);
            }

            _dbContext.SettingsNotifications.Add(newNotification);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteAccount(Guid userId, string currentPassword)
        {
            var passwordHash = currentPassword.GetPasswordHash();
            var currentUser = _dbContext.Users.Find(userId);
            if (currentUser == null || currentUser.PasswordHash != passwordHash) return false;

            await _fileService.DeleteUserDirectory(userId);

            if (currentUser.UserDetails != null)
            {
                _dbContext.Achievements.RemoveRange(currentUser.UserDetails.Achievements);
                _dbContext.UserDetails.Remove(currentUser.UserDetails);
            }

            if (currentUser.UserSettings != null) _dbContext.UserSettings.Remove(currentUser.UserSettings);
            if (currentUser.HubConnections.Any()) _dbContext.HubConnections.RemoveRange(currentUser.HubConnections);
            if (currentUser.GoogleCredential != null) _dbContext.GoogleCredentials.Remove(currentUser.GoogleCredential);

            if (currentUser.GoogleCalendarAccessRequest != null)
                _dbContext.GoogleCalendarAccessRequests.Remove(currentUser.GoogleCalendarAccessRequest);

            var userConnectorsToRemove = _dbContext.UserConnectors
                .Where(x => x.FriendId == currentUser.Id).ToList();

            if (currentUser.UserConnectors.Any() && userConnectorsToRemove.Any())
            {
                _dbContext.UserConnectors.RemoveRange(currentUser.UserConnectors);
                _dbContext.UserConnectors.RemoveRange(userConnectorsToRemove);
            }

            var plantsHistoryNotes = currentUser.Plants.Select(x => x.PlantHistory);
            if (plantsHistoryNotes.Any())
            {
                var plantsHistoryNotesToRemove = new List<PlantHistoryNote>();
                foreach (var plantHistoryNotes in plantsHistoryNotes)
                    plantsHistoryNotesToRemove.AddRange(plantHistoryNotes);

                _dbContext.PlantHistory.RemoveRange(plantsHistoryNotesToRemove);
                _dbContext.Plants.RemoveRange(currentUser.Plants);
            }

            if (currentUser.AccountConfirmationTokens.Any())
                _dbContext.ConfirmationTokens.RemoveRange(currentUser.AccountConfirmationTokens);

            if (currentUser.UserNotifications.Any())
                _dbContext.UserNotifications.RemoveRange(currentUser.UserNotifications);

            if (currentUser.SettingsNotifications != null)
                _dbContext.SettingsNotifications.RemoveRange(currentUser.SettingsNotifications);

            var tasksFiles = currentUser.Tasks.Select(x => x.Files);
            if (tasksFiles.Any())
            {
                var tasksFilesToRemove = new List<File>();
                foreach (var taskFiles in tasksFiles) tasksFilesToRemove.AddRange(taskFiles);

                _dbContext.Files.RemoveRange(tasksFilesToRemove);
            }

            var tasksHistoryNotes = currentUser.Tasks.Select(x => x.History);
            if (tasksHistoryNotes.Any())
            {
                var tasksHistoryNotesToRemove = new List<TaskHistoryNote>();
                foreach (var taskHistory in tasksHistoryNotes) tasksHistoryNotesToRemove.AddRange(taskHistory);

                _dbContext.TaskHistory.RemoveRange(tasksHistoryNotesToRemove);
            }

            _dbContext.Tasks.RemoveRange(currentUser.Tasks);
            _dbContext.Users.Remove(currentUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public List<SettingsNotification> GetSettingsNotifications(Guid userId)
        {
            var currentUser = _dbContext.Users.Find(userId);

            var settingsNotifications = _dbContext.SettingsNotifications.Where(x => x.User == currentUser).ToList();
            return settingsNotifications;
        }

        public async Task DeleteSettingsNotification(SettingsNotificationType settingsNotificationType)
        {
            var notificationToDelete = _dbContext.SettingsNotifications.Where(x => x.Type == settingsNotificationType);
            _dbContext.SettingsNotifications.RemoveRange(notificationToDelete);

            await _dbContext.SaveChangesAsync();
        }
    }
}
