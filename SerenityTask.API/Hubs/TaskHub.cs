using Microsoft.AspNetCore.SignalR;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Hubs
{
    public class TaskHub : Hub
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