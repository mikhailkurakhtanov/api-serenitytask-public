using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SerenityTask.API.Hubs
{
    public class SessionHub: Hub
    {
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.UserIdentifier;

            await Groups.AddToGroupAsync(connectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }
    }
}