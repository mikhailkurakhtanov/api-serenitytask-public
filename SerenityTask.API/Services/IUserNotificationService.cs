using System;
using System.Collections.Generic;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface IUserNotificationService
    {
        ICollection<UserNotificationView> GetUserNotificationsViews(Guid currentUserId);

        UserNotificationView GetUserNotificationView(UserNotification userNotification);

        Task CreateNotification(UserNotification newUserNotification);

        Task DeleteNotification(long userNotificationId);

        Task SendDeletedUserNotificationId(Guid receiverId, long userNotificationId);
    }
}