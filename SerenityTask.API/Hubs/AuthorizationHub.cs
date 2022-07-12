using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SerenityTask.API.Models.Requests.Hub;
using SerenityTask.API.Services;

namespace SerenityTask.API.Hubs
{
    public class AuthorizationHub : Hub, IHubConvention
    {
        private readonly IHubService _hubService;

        public AuthorizationHub(IHubService hubService)
        {
            _hubService = hubService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.UserIdentifier;

            _hubService.SaveHubConnection(userId, connectionId);
            _hubService.SetActivityStatusForUser(Guid.Parse(userId), true);

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            string userId = Context.UserIdentifier;

            _hubService.RemoveHubConnection(connectionId);
            _hubService.SetActivityStatusForUser(Guid.Parse(userId), false);

            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public void UpdateHubConnectionDetails(UpdateHubConnectionDetailsRequest request)
        {
            if (request != null && request.HubConnectionId != null) _hubService.UpdateHubConnectionDetails(request);
        }
    }
}