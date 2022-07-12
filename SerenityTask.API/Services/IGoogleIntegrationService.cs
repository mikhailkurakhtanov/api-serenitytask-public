using System;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client.Authentication;
using Task = System.Threading.Tasks.Task;
using System.Threading.Tasks;

namespace SerenityTask.API.Services
{
    public interface IGoogleIntegrationService
    {
        Task ConnectAccount(SocialUser request, Guid currentUserId);

        void UpdateCredential(SocialUser data);

        Task DisconnectAccount(Guid currentUserId);

        Task<string> CreateAndGetCalendarId(GoogleCredential googleCredential);

        Task<string> CreateAndGetEventId(Guid currentUserId, Session sessionToPlan);

        Task RemoveAttendeeFromEvent(GoogleCredential ownerCredential, string eventId, string participantEmail);

        Task DeleteEvent(GoogleCredential ownerCredential, string eventId);

        bool CheckIsAccountUse(string email);
    }
}