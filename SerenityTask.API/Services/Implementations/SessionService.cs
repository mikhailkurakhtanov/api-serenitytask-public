using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SerenityTask.API.Extensions;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Client;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Plant;
using SerenityTask.API.Models.Requests.Session;
using SerenityTask.API.Models.Responses.Session;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IHubContext<ChatHub> _chatHub;

        private readonly IHubContext<TimerHub> _timerHub;

        private readonly IHubContext<SessionHub> _sessionHub;

        private readonly IHubContext<UserNotificationHub> _userNotificationHub;

        private readonly IPlantService _plantService;

        private readonly IGoogleIntegrationService _googleIntegrationService;

        private readonly IUserNotificationService _userNotificationService;

        private readonly SerenityTaskDbContext _dbContext;

        public SessionService(IHubContext<ChatHub> chatHub, IHubContext<SessionHub> sessionHub,
            IHubContext<TimerHub> timerHub, IHubContext<UserNotificationHub> userNotificationHub,
            IPlantService plantService, IGoogleIntegrationService googleIntegrationService,
            IUserNotificationService userNotificationService, SerenityTaskDbContext dbContext)
        {
            _chatHub = chatHub;
            _timerHub = timerHub;
            _sessionHub = sessionHub;
            _userNotificationHub = userNotificationHub;
            _plantService = plantService;
            _googleIntegrationService = googleIntegrationService;
            _userNotificationService = userNotificationService;
            _dbContext = dbContext;
        }

        public async Task CreateSession(Session sessionToCreate, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(sessionToCreate.OwnerId);

            sessionToCreate.CreationDate = DateTime.UtcNow;
            sessionToCreate.StartDate = sessionToCreate.StartDateString.GetUtcDateTimeFromString();
            sessionToCreate.EndDate = sessionToCreate.EndDateString.GetUtcDateTimeFromString();

            if (sessionToCreate.ParticipantsIds.Any())
            {
                var participantsIdsGuid = new List<Guid>();

                foreach (var participantId in sessionToCreate.ParticipantsIds)
                {
                    var participantIdGuid = Guid.Parse(participantId);
                    participantsIdsGuid.Add(participantIdGuid);
                }

                sessionToCreate.Participants = _dbContext.Users
                    .Where(x => participantsIdsGuid.Any(y => x.Id == y)).ToList();

                sessionToCreate.SessionMembersJSON = CreateAndGetSessionMembersJSON(sessionToCreate);
            }

            _dbContext.Sessions.Add(sessionToCreate);
            await _dbContext.SaveChangesAsync();

            _dbContext.Entry(sessionToCreate).Reference(c => c.Owner).Load();
            _dbContext.Entry(sessionToCreate).Collection(c => c.Participants).Load();

            if (sessionToCreate.Owner.GoogleCredential != null)
            {
                sessionToCreate.GoogleCalendarEventId = await _googleIntegrationService
                    .CreateAndGetEventId(currentUserId, sessionToCreate);

                _dbContext.Sessions.Update(sessionToCreate);
                await CreateNewSessionNotification(sessionToCreate);

                await _dbContext.SaveChangesAsync();
            }

            var jsonData = JsonConvert.SerializeObject(sessionToCreate);

            await _sessionHub.Clients
                .Group($"user_{sessionToCreate.Owner.Id}")
                .SendAsync("receiveCreatedSession", jsonData);

            foreach (var participant in sessionToCreate.Participants)
            {
                await _sessionHub.Clients.Group($"user_{participant.Id}").SendAsync("receiveCreatedSession", jsonData);
            }
        }

        public async Task LeaveSession(long sessionId, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            var sessionToLeave = _dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);

            if (currentUser != null && sessionToLeave != null)
            {
                if (sessionToLeave.Owner.GoogleCredential != null)
                {
                    await _googleIntegrationService
                        .DeleteEvent(sessionToLeave.Owner.GoogleCredential, sessionToLeave.GoogleCalendarEventId);
                }

                var newRequest = new ChangePlantExperienceRequest
                {
                    ChangingType = ExperienceChangingType.Reduce,
                    ObjectType = ExperienceObjectType.Session,
                    ReasonType = ExperienceReasonType.Session_Leaved,
                    SessionId = sessionToLeave.Id
                };

                await _plantService.ChangePlantExperience(newRequest, currentUserId);

                await _sessionHub.Clients.Group($"user_{sessionToLeave.OwnerId}")
                    .SendAsync("receiveDisbandSessionId", sessionId);

                foreach (var participant in sessionToLeave.Participants)
                {
                    await _sessionHub.Clients.Group($"user_{participant.Id}")
                        .SendAsync("receiveDisbandSessionId", sessionId);
                }

                sessionToLeave.Participants.Remove(currentUser);
                _dbContext.Sessions.Update(sessionToLeave);

                if (sessionToLeave.Participants.Count == 0) _dbContext.Sessions.Remove(sessionToLeave);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task JoinSession(long sessionId, Guid currentUserId)
        {
            var sessionToJoin = _dbContext.Sessions.Find(sessionId);
            if (sessionToJoin != null)
            {
                await _sessionHub.Clients.Group($"user_{currentUserId}").SendAsync("receiveJoinedSessionId", sessionId);

                sessionToJoin.CreationDateString = sessionToJoin.CreationDate.ToString("u");
                sessionToJoin.StartDateString = sessionToJoin.StartDate.ToString("u");
                sessionToJoin.EndDateString = sessionToJoin.EndDate.ToString("u");

                var currentParticipant = sessionToJoin.OwnerId == currentUserId
                    ? sessionToJoin.Owner : sessionToJoin.Participants.FirstOrDefault(x => x.Id == currentUserId);

                var sessionMembers = JsonConvert.DeserializeObject<List<SessionMember>>(sessionToJoin.SessionMembersJSON);
                var joinedSessionMemberIndex = sessionMembers.FindIndex(x => x.UserId == currentParticipant.Id);
                if (joinedSessionMemberIndex >= 0)
                {
                    sessionMembers[joinedSessionMemberIndex].IsJoined = true;
                    sessionToJoin.SessionMembersJSON = JsonConvert.SerializeObject(sessionMembers);

                    _dbContext.Sessions.Update(sessionToJoin);
                    await _dbContext.SaveChangesAsync();
                }

                var jsonData = JsonConvert.SerializeObject(sessionToJoin);
                await _timerHub.Clients.Group($"user_{currentUserId}").SendAsync("receiveActiveSession", jsonData);

                await SendActiveSessionMembersData(sessionMembers);
            }
        }

        public async Task ChangeSessionMemberTask(ChangeSessionMemberTaskRequest request, Guid currentUserId)
        {
            var session = _dbContext.Sessions.Find(request.SessionId);
            if (session == null) return;

            var currentDate = DateTime.UtcNow;
            if (currentDate > session.EndDate) return;

            var sessionMembers = JsonConvert.DeserializeObject<List<SessionMember>>(session.SessionMembersJSON);
            if (!sessionMembers.Any()) return;

            var indexOfMember = sessionMembers.FindIndex(x => x.UserId == currentUserId);
            if (indexOfMember < 0) return;

            sessionMembers[indexOfMember].TaskName = request.TaskName;
            sessionMembers[indexOfMember].TaskId = request.TaskId;

            if (sessionMembers[indexOfMember].SessionMemberTasks == null)
            {
                sessionMembers[indexOfMember].SessionMemberTasks = new List<SessionMemberTask>();
            }

            var indexOfTrackedTask = sessionMembers[indexOfMember].SessionMemberTasks.FindIndex(x => x.TaskId == request.TaskId);
            if (indexOfTrackedTask < 0)
            {
                var sessionMemberTasks = sessionMembers[indexOfMember].SessionMemberTasks;
                var indexOfActiveTask = sessionMembers[indexOfMember].SessionMemberTasks
                    .FindIndex(x => x.EndTrackingDate == null);

                if (indexOfActiveTask >= 0)
                {
                    sessionMemberTasks[indexOfActiveTask].EndTrackingDate = currentDate;
                    sessionMemberTasks[indexOfActiveTask].TrackedTime = Math.Floor(currentDate
                        .Subtract(sessionMemberTasks[indexOfActiveTask].StartTrackingDate).TotalSeconds);
                }

                sessionMembers[indexOfMember].SessionMemberTasks = sessionMemberTasks;

                var sessionMemberTask = new SessionMemberTask
                {
                    TaskId = request.TaskId,
                    StartTrackingDate = DateTime.UtcNow
                };

                sessionMembers[indexOfMember].SessionMemberTasks.Add(sessionMemberTask);
            }
            else
            {
                sessionMembers[indexOfMember].SessionMemberTasks[indexOfTrackedTask].StartTrackingDate = currentDate;
                sessionMembers[indexOfMember].SessionMemberTasks[indexOfTrackedTask].EndTrackingDate = null;
            }

            session.SessionMembersJSON = JsonConvert.SerializeObject(sessionMembers);

            _dbContext.Sessions.Update(session);
            await _dbContext.SaveChangesAsync();

            await SendActiveSessionMembersData(sessionMembers);
        }

        public async Task StartSession(long sessionId)
        {
            var sessionToStart = _dbContext.Sessions.Find(sessionId);
            if (sessionToStart != null)
            {
                var sessionMembers = JsonConvert.DeserializeObject<List<SessionMember>>(sessionToStart.SessionMembersJSON);
                if (sessionMembers.Any())
                {
                    foreach (var sessionMember in sessionMembers)
                    {
                        await _timerHub.Clients.Group($"user_{sessionMember.UserId}")
                            .SendAsync("receiveStartSessionTrigger", true);
                    }
                }
            }
        }

        public async Task SetReadyStatusForJoinedMember(SetReadyStatusForJoinedMemberRequest request)
        {
            var session = _dbContext.Sessions.Find(request.SessionId);
            if (session != null)
            {
                var sessionMembers = JsonConvert.DeserializeObject<List<SessionMember>>(session.SessionMembersJSON);
                var joinedSessionMemberIndex = sessionMembers.FindIndex(x => x.UserId == request.UserId);
                if (joinedSessionMemberIndex >= 0)
                {
                    sessionMembers[joinedSessionMemberIndex].IsReady = request.IsReady;
                    session.SessionMembersJSON = JsonConvert.SerializeObject(sessionMembers);

                    _dbContext.Sessions.Update(session);
                    await _dbContext.SaveChangesAsync();

                    await SendActiveSessionMembersData(sessionMembers);
                }
            }
        }

        public async Task CancelSession(long sessionId, Guid currentUserId)
        {
            var sessionToClose = _dbContext.Sessions.Find(sessionId);
            if (sessionToClose != null)
            {
                if (sessionToClose.Owner.GoogleCredential != null)
                {
                    await _googleIntegrationService
                        .DeleteEvent(sessionToClose.Owner.GoogleCredential, sessionToClose.GoogleCalendarEventId);
                }

                var newRequest = new ChangePlantExperienceRequest
                {
                    ChangingType = ExperienceChangingType.Reduce,
                    ObjectType = ExperienceObjectType.Session,
                    ReasonType = ExperienceReasonType.Session_Canceled,
                    SessionId = sessionToClose.Id
                };

                await _plantService.ChangePlantExperience(newRequest, currentUserId);

                var currentDate = DateTime.UtcNow;
                var sessionStartDate = sessionToClose.Owner.ConvertDateTimeToUserDateTime(sessionToClose.StartDate);
                var sessionEndDate = sessionToClose.Owner.ConvertDateTimeToUserDateTime(sessionToClose.EndDate);

                await _sessionHub.Clients.Group($"user_{currentUserId}")
                    .SendAsync(currentDate > sessionStartDate && currentDate < sessionEndDate
                        ? "receiveRemovedSessionId" : "receiveDisbandSessionId", sessionId);

                await _timerHub.Clients.Group($"user_{currentUserId}").SendAsync("receiveCancelledSessionId", sessionId);

                foreach (var participant in sessionToClose.Participants)
                {
                    await _sessionHub.Clients.Group($"user_{participant.Id}")
                        .SendAsync(currentDate > sessionStartDate && currentDate < sessionEndDate
                            ? "receiveRemovedSessionId" : "receiveDisbandSessionId", sessionId);

                    await _timerHub.Clients.Group($"user_{participant.Id}").SendAsync("receiveCancelledSessionId", sessionId);
                }

                foreach (var participant in sessionToClose.Participants)
                {
                    sessionToClose.Participants.Remove(participant);
                }

                _dbContext.Sessions.Remove(sessionToClose);
                await _dbContext.SaveChangesAsync();
            }
        }

        public ICollection<Session> GetSessions(Guid currentUserId)
        {
            var userSessions = _dbContext.Sessions
                .Where(x => x.OwnerId == currentUserId || x.Participants.Any(x => x.Id == currentUserId))
                .OrderBy(x => x.StartDate).ToList();

            for (var index = 0; index < userSessions.Count; index++)
            {
                userSessions[index].CreationDateString = userSessions[index].CreationDate.ToString("u");
                userSessions[index].StartDateString = userSessions[index].StartDate.ToString("u");
                userSessions[index].EndDateString = userSessions[index].EndDate.ToString("u");
            }

            return userSessions;
        }

        public GetFriendsAndSessionRequestsResponse GetFriendsAndSessionRequests(Guid currentUserId)
        {
            var friendsInfo = new List<FriendInfo>();
            var sessionRequests = new List<List<SessionRequest>>();

            var userConnectors = _dbContext.UserConnectors.Where(x => x.UserId == currentUserId);
            if (!userConnectors.Any()) return null;

            foreach (var userConnector in userConnectors)
            {
                var friendInfo = new FriendInfo
                {
                    FriendId = userConnector.FriendId,
                    Name = userConnector.Friend.Name,
                    Avatar = userConnector.Friend.UserDetails.Avatar,
                    TelegramUsername = userConnector.Friend.UserDetails.TelegramUsername,
                    DiscordTag = userConnector.Friend.UserDetails.DiscordTag,
                    isUserOnline = userConnector.Friend.IsUserOnline
                };
                friendsInfo.Add(friendInfo);

                var sessionRequestsWithFriend = _dbContext.SessionRequests
                    .Where(x => (x.SenderId == userConnector.FriendId && x.ReceiverId == currentUserId)
                        || (x.SenderId == currentUserId && x.ReceiverId == userConnector.FriendId))
                    .OrderBy(x => x.SendingDate).ToList();

                if (sessionRequestsWithFriend.Any())
                {
                    var currentUser = _dbContext.Users.Find(currentUserId);

                    for (var requestIndex = 0; requestIndex < sessionRequestsWithFriend.Count(); requestIndex++)
                    {
                        sessionRequestsWithFriend[requestIndex].SendingDateString
                            = sessionRequestsWithFriend[requestIndex].SendingDate.ToString("u");

                        sessionRequestsWithFriend[requestIndex].StartString
                            = sessionRequestsWithFriend[requestIndex].Start.ToString("u");

                        sessionRequestsWithFriend[requestIndex].EndString
                            = sessionRequestsWithFriend[requestIndex].End.ToString("u");
                    }

                    sessionRequests.Add(sessionRequestsWithFriend.ToList());
                }
            }

            var getFriendsAndSessionRequestsResponse = new GetFriendsAndSessionRequestsResponse
            {
                FriendsInfo = friendsInfo.OrderByDescending(x => x.isUserOnline)?.ToList(),
                SessionRequests = sessionRequests.OrderBy(x => x.First().SendingDate)?.ToList()
            };

            return getFriendsAndSessionRequestsResponse;
        }

        public async Task SendSessionRequest(SessionRequest newSessionRequest)
        {
            newSessionRequest.SendingDate = DateTime.UtcNow;
            newSessionRequest.Start = DateTime.Parse(newSessionRequest.StartString).ToUniversalTime();
            newSessionRequest.End = DateTime.Parse(newSessionRequest.EndString).ToUniversalTime();

            newSessionRequest.SendingDateString = newSessionRequest.SendingDate.ToString("u");
            newSessionRequest.StartString = newSessionRequest.Start.ToString("u");
            newSessionRequest.EndString = newSessionRequest.End.ToString("u");

            _dbContext.SessionRequests.Add(newSessionRequest);
            await _dbContext.SaveChangesAsync();

            _dbContext.Entry(newSessionRequest).Reference(c => c.Sender).Load();
            _dbContext.Entry(newSessionRequest).Reference(c => c.Receiver).Load();

            var jsonData = JsonConvert.SerializeObject(newSessionRequest);

            await _chatHub.Clients
                .Group($"user_{newSessionRequest.SenderId}")
                .SendAsync("receiveNewSessionRequest", jsonData);

            await _chatHub.Clients
                .Group($"user_{newSessionRequest.ReceiverId}")
                .SendAsync("receiveNewSessionRequest", jsonData);
        }

        public async Task RejectSessionRequest(long sessionRequestId)
        {
            var sessionRequestToReject = _dbContext.SessionRequests.Find(sessionRequestId);
            if (sessionRequestToReject != null)
            {
                _dbContext.SessionRequests.Remove(sessionRequestToReject);
                _dbContext.SaveChanges();

                await SendRejectedSessionRequest(sessionRequestToReject);
            }
        }

        public async Task SendRejectedSessionRequest(SessionRequest request)
        {
            var jsonData = JsonConvert.SerializeObject(request);

            await _chatHub.Clients
                .Group($"user_{request.SenderId}")
                .SendAsync("receiveRejectedSessionRequest", jsonData);

            await _chatHub.Clients
                .Group($"user_{request.ReceiverId}")
                .SendAsync("receiveRejectedSessionRequest", jsonData);
        }

        public async Task ReadSessionRequest(long chatMessageId)
        {
            var sessionRequestToUpdate = _dbContext.SessionRequests.Find(chatMessageId);
            if (sessionRequestToUpdate != null)
            {
                sessionRequestToUpdate.IsRead = true;

                _dbContext.SessionRequests.Update(sessionRequestToUpdate);
                await _dbContext.SaveChangesAsync();

                sessionRequestToUpdate.SendingDateString = sessionRequestToUpdate.SendingDate.ToString("u");
                sessionRequestToUpdate.StartString = sessionRequestToUpdate.Start.ToString("u");
                sessionRequestToUpdate.EndString = sessionRequestToUpdate.End.ToString("u");

                var jsonData = JsonConvert.SerializeObject(sessionRequestToUpdate);

                await _chatHub.Clients
                    .Group($"user_{sessionRequestToUpdate.SenderId}")
                    .SendAsync("receiveReadSessionRequest", jsonData);

                await _chatHub.Clients
                    .Group($"user_{sessionRequestToUpdate.ReceiverId}")
                    .SendAsync("receiveReadSessionRequest", jsonData);
            }
        }

        #region Private

        private string CreateAndGetSessionMembersJSON(Session sessionToCreate)
        {
            var sessionMembers = new List<SessionMember>();

            var sessionOwner = _dbContext.Users.Find(sessionToCreate.OwnerId);
            var sessionMemberOwner = new SessionMember
            {
                Name = sessionOwner.Name,
                Avatar = sessionOwner.UserDetails.Avatar,
                TaskName = "Not selected",
                UserId = sessionOwner.Id
            };
            sessionMembers.Add(sessionMemberOwner);

            foreach (var participant in sessionToCreate.Participants)
            {
                var sessionMemberParticipant = new SessionMember
                {
                    Name = participant.Name,
                    Avatar = participant.UserDetails.Avatar,
                    TaskName = "Not selected",
                    SessionMemberTasks = new List<SessionMemberTask>(),
                    UserId = participant.Id
                };

                sessionMembers.Add(sessionMemberParticipant);
            }

            return JsonConvert.SerializeObject(sessionMembers);
        }

        public async Task SendActiveSessionMembersData(List<SessionMember> activeSessionMembers)
        {
            var activeSessionMembersIds = activeSessionMembers.Select(x => x.UserId);

            foreach (var sessionMemberId in activeSessionMembersIds)
            {
                foreach (var item in activeSessionMembers)
                {
                    var activeSessionMemberJson = JsonConvert.SerializeObject(item);

                    await _timerHub.Clients.Group($"user_{sessionMemberId}")
                        .SendAsync("receiveSessionMember", activeSessionMemberJson);
                }
            }
        }

        private async Task CreateNewSessionNotification(Session createdSession)
        {
            var newUserNotificationMessage = "Session with "
                + string.Join(", ", createdSession.Participants.Select(x => x.Name))
                + " was planned to " + createdSession.StartDate.ToString("MMMM dd, yyyy H:mm")
                + " Duration: " + GetSessionDurationFormatted(createdSession);

            var newUserNotificationForOwner = new UserNotification
            {
                Message = newUserNotificationMessage,
                Type = UserNotificationType.SessionApprovement,
                SenderId = createdSession.Owner.Id,
                ReceiverId = createdSession.Owner.Id
            };

            var newUserNotificationForParticipant = new UserNotification
            {
                Message = newUserNotificationMessage,
                Type = UserNotificationType.SessionApprovement,
                SenderId = createdSession.Owner.Id,
                ReceiverId = createdSession.Participants.First().Id
            };

            await _userNotificationService.CreateNotification(newUserNotificationForOwner);
            await _userNotificationService.CreateNotification(newUserNotificationForParticipant);
        }

        private string GetSessionDurationFormatted(Session session)
        {
            var durationHours = 0;
            var sessionDuration = session.Duration;

            while (sessionDuration - 60 > 0)
            {
                sessionDuration -= 60;
                durationHours++;
            }

            var durationMinutes = sessionDuration;

            if (durationHours > 0 && durationMinutes > 0) return durationHours + " Hours " + durationMinutes + " Minutes";
            if (durationHours > 0) return durationHours + " Hours";
            return durationMinutes + " Minutes";
        }

        #endregion
    }
}