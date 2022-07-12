using Microsoft.AspNetCore.SignalR;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Hubs
{
    public class PlantHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await base.OnConnectedAsync();
        }
    }
}