using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SerenityTask.API.Services;

namespace SerenityTask.API.Hubs
{
    public class TimerHub : Hub
    {
        private readonly ITimerHubService _timerHubService;

        public TimerHub(ITimerHubService timerHubService)
        {
            _timerHubService = timerHubService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.UserIdentifier;

            await Groups.AddToGroupAsync(connectionId, $"user_{userId}");
            await _timerHubService.ProcessSessionMemberConnectionStatus(Guid.Parse(userId), false);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            var userId = Context.UserIdentifier;

            await _timerHubService.ProcessSessionMemberConnectionStatus(Guid.Parse(userId), true);
            await base.OnDisconnectedAsync(exception);
        }
    }
}