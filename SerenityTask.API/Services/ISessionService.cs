using System;
using System.Collections.Generic;
using SerenityTask.API.Models.Client;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Session;
using SerenityTask.API.Models.Responses.Session;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface ISessionService
    {
        Task CreateSession(Session sessionToCreate, Guid currentUser);

        ICollection<Session> GetSessions(Guid currentUserId);

        Task JoinSession(long sessionId, Guid currentUserId);

        Task StartSession(long sessionId);

        Task SetReadyStatusForJoinedMember(SetReadyStatusForJoinedMemberRequest request);

        Task ChangeSessionMemberTask(ChangeSessionMemberTaskRequest request, Guid currentUserId);

        Task LeaveSession(long sessionId, Guid currentUserId);

        Task CancelSession(long sessionId, Guid currentUserId);

        Task SendActiveSessionMembersData(List<SessionMember> activeSessionMembers);

        GetFriendsAndSessionRequestsResponse GetFriendsAndSessionRequests(Guid currentUserId);

        Task SendSessionRequest(SessionRequest newChatMessage);

        Task RejectSessionRequest(long sessionRequestId);

        Task ReadSessionRequest(long chatMessageId);

        Task SendRejectedSessionRequest(SessionRequest request);
    }
}