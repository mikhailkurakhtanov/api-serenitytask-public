using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

using Microsoft.EntityFrameworkCore;

using SerenityTask.API.Extensions;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.User;
using SerenityTask.API.Models.Requests;
using Microsoft.AspNetCore.SignalR;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Client;
using Newtonsoft.Json;

namespace SerenityTask.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IEmailService _emailService;

        private readonly ISessionService _sessionService;

        private readonly IUserNotificationService _userNotificationsService;

        private readonly IHubContext<ChatHub> _chatHub;

        private readonly SerenityTaskDbContext _dbContext;

        public UserService(IEmailService emailService, ISessionService sessionService,
        IUserNotificationService userNotificationService, IHubContext<ChatHub> chatHub, SerenityTaskDbContext dbContext)
        {
            _emailService = emailService;
            _sessionService = sessionService;
            _userNotificationsService = userNotificationService;
            _chatHub = chatHub;
            _dbContext = dbContext;
        }

        public User GetCurrentUser(Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            if (currentUser.Files.Any()) currentUser.Files.OrderByDescending(x => x.UploadDate);

            return currentUser;
        }

        public async Task UpdateUser(User changedUser)
        {
            _dbContext.Users.Update(changedUser);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ConfirmAccount(string userId, string confirmationToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(confirmationToken)) return false;

            var currentUser = _dbContext.Users.Find(Guid.Parse(userId));
            if (currentUser == null) return false;

            if (!await CheckConfirmationData(currentUser, confirmationToken)) return false;

            _emailService.SendWelcomeEmail(currentUser.Email, currentUser.Name);

            currentUser.IsEmailConfirmed = true;
            _dbContext.Update(currentUser);

            CreateUserSettings(currentUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ConfirmPassword(string userId, string newPasswordHash, string confirmationToken)
        {
            var currentUser = _dbContext.Users.Find(Guid.Parse(userId));
            if (currentUser == null) return false;

            if (!await CheckConfirmationData(currentUser, confirmationToken)) return false;

            currentUser.PasswordHash = newPasswordHash;
            _dbContext.Update(currentUser);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public bool IsUsernameAvailable(string username)
        {
            var existingUser = _dbContext.Users.Where(x => x.Username == username);
            return !existingUser.Any();
        }

        public bool IsEmailAvailable(string email)
        {
            var existingUser = _dbContext.Users.Where(x => x.Email == email);
            return !existingUser.Any();
        }

        public ICollection<UserCard> GetUserCards(GetUserCardsRequest userSearchOptions, Guid currentUserId)
        {
            var usersDetailsFiltered = new List<UserDetails>();
            var userCards = new List<UserCard>();
            var userConnectors = _dbContext.UserConnectors.Where(x => x.UserId == currentUserId);

            var otherUsersDetails = _dbContext.UserDetails.Where(x => x.UserId != currentUserId && x.User.RoleId != 1
                && !userConnectors.Any(y => x.UserId == y.FriendId)).OrderByDescending(x => x.User.IsUserOnline);

            if (!string.IsNullOrWhiteSpace(userSearchOptions.UsernameOrEmail))
            {
                usersDetailsFiltered = otherUsersDetails.Where(x => x.User.Username == userSearchOptions.UsernameOrEmail
                    || x.User.Email == userSearchOptions.UsernameOrEmail).ToList();
            }
            else
            {
                foreach (var userDetails in otherUsersDetails)
                {
                    var areInterestsValid = false;
                    var areLanguagesValid = false;
                    var areTimeZonesValid = false;
                    var isUserOnlineValid = false;

                    // checking by requested interests
                    var userInterests = userDetails.Interests?.Split(", ");
                    if (userInterests != null && userSearchOptions.Interests.Any())
                    {
                        areInterestsValid = userInterests
                            .Where(x => userSearchOptions.Interests.Any(y => x.Contains(y))).Any();
                    }
                    else areInterestsValid = true;

                    // checking by requested languages
                    var userLanguages = userDetails.Languages?.Split(", ");
                    if (userLanguages != null && userSearchOptions.Languages.Any())
                    {
                        areLanguagesValid = userLanguages
                            .Where(x => userSearchOptions.Languages.Any(y => x.Contains(y))).Any();
                    }
                    else areLanguagesValid = true;

                    // checking by requested timezones
                    if (userSearchOptions.Timezones.Any())
                    {
                        areTimeZonesValid = userSearchOptions.Timezones.Contains(userDetails.TimeZone.TimeZoneId);
                    }
                    else areTimeZonesValid = true;

                    // checking by requesting user status
                    if (userSearchOptions.IsUserOnline != null)
                    {
                        isUserOnlineValid = userSearchOptions.IsUserOnline == userDetails.User.IsUserOnline;
                    }
                    else isUserOnlineValid = true;

                    if (areInterestsValid && areLanguagesValid && areTimeZonesValid && isUserOnlineValid)
                    {
                        usersDetailsFiltered.Add(userDetails);
                    }
                }
            }

            if (usersDetailsFiltered.Any())
            {
                foreach (var userDetails in usersDetailsFiltered)
                {
                    var userCard = new UserCard
                    {
                        Name = userDetails.User.Name,
                        Avatar = userDetails.Avatar,
                        Age = userDetails.Age,
                        Interests = userDetails.Interests?.Split(", "),
                        LookingFor = userDetails.LookingFor,
                        TimeZone = userDetails.TimeZone?.DisplayName,
                        Languages = userDetails.Languages?.Split(", "),
                        IsUserOnline = userDetails.User.IsUserOnline,
                        Achievements = userDetails.Achievements?.Where(x => x.Value == x.Type.Goal).ToList(),
                        UserId = userDetails.UserId
                    };

                    userCards.Add(userCard);
                }
            }

            return userCards;
        }

        public async Task<UserSettings> UpdateUserSettings(UserSettings changedUserSettings, Guid currentUserId)
        {
            changedUserSettings.User = GetCurrentUser(currentUserId);

            _dbContext.Entry(changedUserSettings.User).State = EntityState.Unchanged;
            _dbContext.UserSettings.Update(changedUserSettings);

            await _dbContext.SaveChangesAsync();
            return changedUserSettings;
        }

        public async Task AcceptFriendRequest(UserNotificationConnector request)
        {
            var userConnectorForSender = new UserConnector
            {
                UserId = request.SenderId,
                FriendId = request.ReceiverId
            };
            _dbContext.UserConnectors.Add(userConnectorForSender);

            var userConnectorForReceiver = new UserConnector
            {
                UserId = request.ReceiverId,
                FriendId = request.SenderId
            };
            _dbContext.UserConnectors.Add(userConnectorForReceiver);

            await _dbContext.SaveChangesAsync();
            _dbContext.Entry(userConnectorForSender).Reference(c => c.Friend).Load();
            _dbContext.Entry(userConnectorForReceiver).Reference(c => c.Friend).Load();

            var friendInfoForSender = new FriendInfo
            {
                FriendId = userConnectorForSender.Friend.Id,
                Name = userConnectorForSender.Friend.Name,
                Avatar = userConnectorForSender.Friend.UserDetails.Avatar,
                TelegramUsername = userConnectorForSender.Friend.UserDetails.TelegramUsername,
                DiscordTag = userConnectorForSender.Friend.UserDetails.DiscordTag,
                isUserOnline = userConnectorForSender.Friend.IsUserOnline
            };

            var jsonDataForSender = JsonConvert.SerializeObject(friendInfoForSender);
            await _chatHub.Clients
                    .Group($"user_{userConnectorForSender.UserId}")
                    .SendAsync("receiveNewFriend", jsonDataForSender);

            var friendInfoForReceiver = new FriendInfo
            {
                FriendId = userConnectorForReceiver.Friend.Id,
                Name = userConnectorForReceiver.Friend.Name,
                Avatar = userConnectorForReceiver.Friend.UserDetails.Avatar,
                TelegramUsername = userConnectorForReceiver.Friend.UserDetails.TelegramUsername,
                DiscordTag = userConnectorForReceiver.Friend.UserDetails.DiscordTag,
                isUserOnline = userConnectorForReceiver.Friend.IsUserOnline
            };

            var jsonDataForReceiver = JsonConvert.SerializeObject(friendInfoForReceiver);
            await _chatHub.Clients
                .Group($"user_{userConnectorForReceiver.UserId}")
                .SendAsync("receiveNewFriend", jsonDataForReceiver);
        }

        public async Task RemoveUserFromFriendsList(Guid friendId, Guid currentUserId)
        {
            var userNotifications = _dbContext.UserNotifications.Where(x =>
                (x.ReceiverId == friendId && x.SenderId == currentUserId)
                    || (x.ReceiverId == currentUserId && x.SenderId == friendId));

            if (userNotifications.Any())
            {
                _dbContext.UserNotifications.RemoveRange(userNotifications);

                foreach (var userNotification in userNotifications)
                {
                    await _userNotificationsService
                        .SendDeletedUserNotificationId(userNotification.ReceiverId, userNotification.Id);
                }
            }

            var sessionRequests = _dbContext.SessionRequests.Where(x =>
                    (x.SenderId == friendId && x.ReceiverId == currentUserId)
                        || (x.SenderId == currentUserId && x.ReceiverId == friendId));

            if (sessionRequests.Any())
            {
                _dbContext.SessionRequests.RemoveRange(sessionRequests);

                foreach (var request in sessionRequests)
                {
                    await _sessionService.SendRejectedSessionRequest(request);
                }
            }

            // var sessionsByUser = _sessionService.GetSessions(currentUserId);
            // foreach (var session in sessionsByUser)
            // {
            //     if (session.Participants.Any() && session.Participants.FirstOrDefault(x => x.Id == friendId) != null)
            //     {
            //         await _sessionService.CancelSession(session.Id, currentUserId);
            //     }
            // }

            // var sessionsByFriend = _sessionService.GetSessions(friendId);
            // foreach (var session in sessionsByFriend)
            // {
            //     if (session.Participants.Any() && session.Participants.FirstOrDefault(x => x.Id == currentUserId) != null)
            //     {
            //         await _sessionService.CancelSession(session.Id, friendId);
            //     }
            // }

            var userConnectors = _dbContext.UserConnectors.Where(x =>
                (x.FriendId == friendId && x.UserId == currentUserId)
                    || (x.FriendId == currentUserId && x.UserId == friendId));

            if (userConnectors.Any())
            {
                _dbContext.UserConnectors.RemoveRange(userConnectors);
                await _dbContext.SaveChangesAsync();
            }


            await _chatHub.Clients.Group($"user_{currentUserId}").SendAsync("removeFriend", friendId);
            await _chatHub.Clients.Group($"user_{friendId}").SendAsync("removeFriend", currentUserId);
        }

        public void CreateUserSettings(User currentUser)
        {
            var userSettings = new UserSettings
            {
                TimerSettings = "{ \"mode\": 0, \"type\": 0 }",
                User = currentUser
            };

            _dbContext.UserSettings.Add(userSettings);
        }

        private async Task<bool> CheckConfirmationData(User currentUser, string confirmationToken)
        {
            var confirmationTokenInstance = currentUser.AccountConfirmationTokens.FirstOrDefault(x => x.Token == confirmationToken);
            if (confirmationTokenInstance.ExpirationDate > DateTime.UtcNow)
            {
                _dbContext.Remove(confirmationTokenInstance);
                await _dbContext.SaveChangesAsync();

                return true;
            }

            confirmationTokenInstance.ErrorMessage = "Token was expired. " + Constants.AdviceToContactToSupport;
            _dbContext.Update(confirmationTokenInstance);
            await _dbContext.SaveChangesAsync();

            return false;
        }
    }
}
