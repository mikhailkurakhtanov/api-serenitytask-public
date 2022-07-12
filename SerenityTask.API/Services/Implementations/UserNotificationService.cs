using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services.Implementations
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly IHubContext<UserNotificationHub> _userNotificationHub;

        private readonly SerenityTaskDbContext _dbContext;

        public UserNotificationService(IHubContext<UserNotificationHub> userNotificationHub, SerenityTaskDbContext dbContext)
        {
            _userNotificationHub = userNotificationHub;
            _dbContext = dbContext;
        }

        public async Task CreateNotification(UserNotification newUserNotification)
        {
            var receiver = _dbContext.Users.Find(newUserNotification.ReceiverId);
            newUserNotification.CreationDate = DateTime.UtcNow;
            newUserNotification.CreationDateString = newUserNotification.CreationDate.ToString("u");

            var senderName = _dbContext.Users.FirstOrDefault(x => x.Id == newUserNotification.SenderId).Name;

            if (newUserNotification.Type != UserNotificationType.SessionApprovement)
            {
                switch (newUserNotification.Type)
                {
                    case UserNotificationType.FriendRequest:
                        newUserNotification.Message = senderName + " wants to be friends with you";
                        break;
                    case UserNotificationType.Message:
                        newUserNotification.Message = "You've got a new message from " + senderName;
                        break;
                    case UserNotificationType.SessionInvitation:
                        newUserNotification.Message = senderName + " wants to start a study session right now";
                        break;
                }
            }

            _dbContext.UserNotifications.Add(newUserNotification);
            await _dbContext.SaveChangesAsync();

            var newUserNotificationView = GetUserNotificationView(newUserNotification);
            var jsonData = JsonConvert.SerializeObject(newUserNotificationView);

            await _userNotificationHub.Clients.Group($"user_{newUserNotification.ReceiverId}")
                .SendAsync("receiveNewUserNotification", jsonData);
        }

        public async Task DeleteNotification(long userNotificationId)
        {
            var userNotificationToDelete = _dbContext.UserNotifications.Find(userNotificationId);
            if (userNotificationToDelete != null)
            {
                _dbContext.UserNotifications.Remove(userNotificationToDelete);
                await _dbContext.SaveChangesAsync();

                var jsonData = JsonConvert.SerializeObject(userNotificationId);

                await SendDeletedUserNotificationId(userNotificationToDelete.ReceiverId, userNotificationId);
            }
        }

        public async Task SendDeletedUserNotificationId(Guid receiverId, long userNotificationId)
        {
            await _userNotificationHub.Clients
                .Group($"user_{receiverId}")
                .SendAsync("receiveDeletedUserNotificationId", userNotificationId);
        }

        public ICollection<UserNotificationView> GetUserNotificationsViews(Guid currentUserId)
        {
            var userNotificationsViews = new List<UserNotificationView>();
            var userNotifications = _dbContext.UserNotifications.Where(x => x.ReceiverId == currentUserId);

            if (userNotifications.Any())
            {
                foreach (var userNotification in userNotifications)
                {
                    userNotification.CreationDateString = userNotification.CreationDate.ToString("u");

                    var userNotificationView = GetUserNotificationView(userNotification);
                    userNotificationsViews.Add(userNotificationView);
                }

                userNotificationsViews.OrderByDescending(x => x.UserNotification.CreationDate);
            }

            return userNotificationsViews;
        }

        public UserNotificationView GetUserNotificationView(UserNotification userNotification)
        {
            var sender = _dbContext.Users.Find(userNotification.SenderId);

            var userNotificationView = new UserNotificationView
            {
                UserNotification = userNotification,
                SenderName = sender.Name,
                SenderAvatar = sender.UserDetails.Avatar
            };

            return userNotificationView;
        }
    }
}