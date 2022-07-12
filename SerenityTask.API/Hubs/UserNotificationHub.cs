using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SerenityTask.API.Hubs
{
    public class UserNotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }
    }
}