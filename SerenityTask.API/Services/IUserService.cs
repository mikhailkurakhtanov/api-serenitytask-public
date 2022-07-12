using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.User;
using SerenityTask.API.Models.Requests;
using SerenityTask.API.Models.Client;

namespace SerenityTask.API.Services
{
    public interface IUserService
    {
        User GetCurrentUser(Guid currentUserId);

        Task UpdateUser(User changedUser);

        bool IsUsernameAvailable(string username);

        bool IsEmailAvailable(string email);

        void CreateUserSettings(User currentUser);

        Task<bool> ConfirmAccount(string userId, string token);

        Task<bool> ConfirmPassword(string userId, string userPassword, string confirmationToken);

        Task<UserSettings> UpdateUserSettings(UserSettings changedUserSettings, Guid currentUserId);

        ICollection<UserCard> GetUserCards(GetUserCardsRequest userSearchOptions, Guid currentUserId);

        Task AcceptFriendRequest(UserNotificationConnector request);

        Task RemoveUserFromFriendsList(Guid friendId, Guid currentUserId);
    }
}
