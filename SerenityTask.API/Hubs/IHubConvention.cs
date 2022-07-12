using SerenityTask.API.Models.Requests.Hub;

namespace SerenityTask.API.Hubs
{
    public interface IHubConvention
    {
        string GetConnectionId();

        void UpdateHubConnectionDetails(UpdateHubConnectionDetailsRequest request);
    }
}