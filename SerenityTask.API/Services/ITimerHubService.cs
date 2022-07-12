using System;
using System.Threading.Tasks;

namespace SerenityTask.API.Services
{
    public interface ITimerHubService
    {
        Task ProcessSessionMemberConnectionStatus(Guid userId, bool isMemberDisconnected);
    }
}