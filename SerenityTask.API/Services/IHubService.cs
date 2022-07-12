using System;
using SerenityTask.API.Models.Requests.Hub;

namespace SerenityTask.API.Services
{
    public interface IHubService
    {
        void SetActivityStatusForUser(Guid currentUserId, bool isUserOnline);

        void SaveHubConnection(string userId, string hubConnectionId);

        void RemoveHubConnection(string hubConnectionId);

        void UpdateHubConnectionDetails(UpdateHubConnectionDetailsRequest request);
    }
}