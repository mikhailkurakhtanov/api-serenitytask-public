using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FluentFTP;
using Microsoft.AspNetCore.Http;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;
using SerenityTask.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SerenityTask.API.Services.Implementations
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly IHubContext<UserDetailsHub> _userDetailsHub;

        private readonly SerenityTaskDbContext _dbContext;

        public UserDetailsService(IHubContext<UserDetailsHub> userDetailsHub, SerenityTaskDbContext dbContext)
        {
            _userDetailsHub = userDetailsHub;
            _dbContext = dbContext;
        }

        public ICollection<TimeZoneType> GetTimeZoneTypes()
        {
            return _dbContext.TimeZoneTypes.ToList();
        }

        public async Task UpdateAvatar(IFormFile avatarData, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            if (currentUser != null)
            {
                var currentUserDirectory = Path.Combine(Constants.UserStorageUrl, currentUserId.ToString());
                var ftpClient = new FtpClient(Constants.FtpHost, Constants.FtpUser, Constants.FtpPassword);

                var isUserDirectoryExists = await ftpClient.DirectoryExistsAsync(currentUserDirectory);
                if (!isUserDirectoryExists)
                {
                    await ftpClient.CreateDirectoryAsync(currentUserDirectory);
                }

                var currentUserDetails = currentUser.UserDetails;
                var existingAvatarPath = currentUserDetails.Avatar.Remove(0, 8);

                var isExistingAvatarExist = await ftpClient.FileExistsAsync(existingAvatarPath);
                if (isExistingAvatarExist) await ftpClient.DeleteFileAsync(existingAvatarPath);

                var avatarPath = Path.Combine(currentUserDirectory, "avatar" + Path.GetExtension(avatarData.FileName));
                await UploadAvatarToStorage(ftpClient, avatarData, avatarPath);

                currentUserDetails.Avatar = "https://" + avatarPath;

                _dbContext.UserDetails.Update(currentUserDetails);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUserDetails(UserDetails changedUserDetails)
        {
            _dbContext.UserDetails.Update(changedUserDetails);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAchievement(Achievement achievementToUpdate)
        {
            achievementToUpdate.Value += 1;
            _dbContext.Achievements.Update(achievementToUpdate);
            await _dbContext.SaveChangesAsync();

            var jsonData = JsonConvert.SerializeObject(achievementToUpdate);

            await _userDetailsHub.Clients.Group($"user_{achievementToUpdate.UserDetails.UserId}")
                .SendAsync("receiveUpdatedAchievement", jsonData);
        }

        private static async Task UploadAvatarToStorage(FtpClient ftpClient, IFormFile avatarData, string avatarPath)
        {
            var memoryStream = new MemoryStream();
            avatarData.CopyTo(memoryStream);

            await ftpClient.UploadAsync(memoryStream.ToArray(), avatarPath, FtpRemoteExists.Overwrite);
        }
    }
}
