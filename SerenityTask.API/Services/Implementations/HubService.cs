using System;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Hub;
using SerenityTask.API.Models.Client;

namespace SerenityTask.API.Services.Implementations
{
    public class HubService : IHubService
    {
        private readonly IServiceProvider _serviceProvider;

        public HubService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SaveHubConnection(string userId, string hubConnectionId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SerenityTaskDbContext>();
                var existingConnection = dbContext.HubConnections
                    .FirstOrDefault(x => x.HubConnectionId == hubConnectionId);

                if (existingConnection == null)
                {
                    var hubConnection = new HubConnection
                    {
                        HubConnectionId = hubConnectionId,
                        UserId = Guid.Parse(userId)
                    };

                    dbContext.HubConnections.Add(hubConnection);
                    dbContext.SaveChanges();
                }
            }
        }

        public void RemoveHubConnection(string hubConnectionId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SerenityTaskDbContext>();
                var existingConnection = dbContext.HubConnections
                    .FirstOrDefault(x => x.HubConnectionId == hubConnectionId);

                if (existingConnection != null)
                {
                    var otherConnections = dbContext.HubConnections
                        .Where(x => x.HubConnectionId != hubConnectionId && x.UserId == existingConnection.UserId);

                    if (!otherConnections.Any()) SetActivityStatusForUser(existingConnection.UserId, false);

                    dbContext.HubConnections.Remove(existingConnection);
                    dbContext.SaveChanges();
                }
            }
        }

        public void UpdateHubConnectionDetails(UpdateHubConnectionDetailsRequest request)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SerenityTaskDbContext>();
                var existingConnection = dbContext.HubConnections
                    .FirstOrDefault(x => x.HubConnectionId == request.HubConnectionId);

                if (existingConnection != null)
                {
                    existingConnection.Browser = request.Browser;
                    existingConnection.BrowserVersion = request.BrowserVersion;
                    existingConnection.OS = request.OS;
                    existingConnection.OSVersion = request.OSVersion;

                    dbContext.HubConnections.Update(existingConnection);
                    dbContext.SaveChanges();
                }
            }
        }

        public async void SetActivityStatusForUser(Guid currentUserId, bool isUserConnected)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var chatHub = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();

                var dbContext = scope.ServiceProvider.GetRequiredService<SerenityTaskDbContext>();
                var loggedInUser = dbContext.Users.Find(currentUserId);

                if (loggedInUser != null)
                {
                    loggedInUser.IsUserOnline = isUserConnected;

                    dbContext.Users.Update(loggedInUser);

                    var userConnectors = dbContext.UserConnectors.Where(x => x.FriendId == currentUserId);
                    if (userConnectors.Any())
                    {
                        foreach (var userConnector in userConnectors)
                        {
                            var friendInfo = new FriendInfo
                            {
                                FriendId = currentUserId,
                                Name = loggedInUser.Name,
                                Avatar = loggedInUser.UserDetails.Avatar,
                                TelegramUsername = loggedInUser.UserDetails.TelegramUsername,
                                DiscordTag = loggedInUser.UserDetails.DiscordTag,
                                isUserOnline = loggedInUser.IsUserOnline
                            };

                            var jsonData = JsonConvert.SerializeObject(friendInfo);

                            await chatHub.Clients.Group($"user_{userConnector.UserId}")
                                .SendAsync("receiveFriendStatusChanges", jsonData);
                        }
                    }

                    dbContext.SaveChanges();
                }
            }
        }
    }
}