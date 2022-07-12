using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Client;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;
using SerenityTask.API.Extensions;

namespace SerenityTask.API.Services.Implementations
{
    public class TimerHubService : ITimerHubService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IHubContext<TimerHub> _timerHub;

        public TimerHubService(IServiceProvider serviceProvider, IHubContext<TimerHub> timerHub)
        {
            _serviceProvider = serviceProvider;
            _timerHub = timerHub;
        }

        public async Task ProcessSessionMemberConnectionStatus(Guid userId, bool isMemberDisconnected)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SerenityTaskDbContext>();

                var currentDate = DateTime.UtcNow;

                var activeUserSession = dbContext.Sessions.FirstOrDefault(x
                    => (x.OwnerId == userId || x.Participants.Any(x => x.Id == userId))
                        && x.StartDate < currentDate && x.EndDate > currentDate);

                if (activeUserSession != null)
                {
                    activeUserSession.CreationDateString = activeUserSession.CreationDate.ToString("u");
                    activeUserSession.StartDateString = activeUserSession.StartDate.ToString("u");
                    activeUserSession.EndDateString = activeUserSession.EndDate.ToString("u");

                    var sessionMembers = JsonConvert
                        .DeserializeObject<List<SessionMember>>(activeUserSession.SessionMembersJSON);

                    if (sessionMembers.Any())
                    {
                        var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();

                        if (sessionMembers.Any(x => x.IsJoined && x.IsReady))
                        {
                            var sessionMemberIndex = sessionMembers.FindIndex(x => x.UserId == userId);
                            if (sessionMemberIndex >= 0)
                            {
                                var isMemberReconnected = !isMemberDisconnected
                                    && sessionMembers[sessionMemberIndex].IsDisconnected;

                                if (isMemberReconnected)
                                {
                                    var jsonData = JsonConvert.SerializeObject(activeUserSession);
                                    await _timerHub.Clients.Group($"user_{userId}").SendAsync("receiveActiveSession", jsonData);
                                }

                                if (isMemberDisconnected && sessionMembers[sessionMemberIndex].SessionMemberTasks != null)
                                {
                                    // finishing time tracking for active member's task in session
                                    var indexOfActiveTask = sessionMembers[sessionMemberIndex].SessionMemberTasks
                                        .FindIndex(x => x.EndTrackingDate == null);

                                    if (indexOfActiveTask >= 0)
                                    {
                                        var activeTimeTrackedTask = sessionMembers[sessionMemberIndex]
                                            .SessionMemberTasks[indexOfActiveTask];

                                        activeTimeTrackedTask.EndTrackingDate = currentDate;
                                        activeTimeTrackedTask.TrackedTime += Math.Floor(currentDate
                                            .Subtract(activeTimeTrackedTask.StartTrackingDate).TotalSeconds);

                                        sessionMembers[sessionMemberIndex]
                                            .SessionMemberTasks[indexOfActiveTask] = activeTimeTrackedTask;
                                    }
                                }

                                sessionMembers[sessionMemberIndex].IsDisconnected = isMemberDisconnected;
                                activeUserSession.SessionMembersJSON = JsonConvert.SerializeObject(sessionMembers);

                                dbContext.Sessions.Update(activeUserSession);
                                await dbContext.SaveChangesAsync();

                                await sessionService.SendActiveSessionMembersData(sessionMembers);

                                if (isMemberReconnected)
                                {
                                    await _timerHub.Clients.Group($"user_{userId}").SendAsync("receiveStartSessionTrigger", true);
                                }
                            }
                        }
                        else
                        {
                            if (userId == activeUserSession.OwnerId)
                            {
                                await sessionService.CancelSession(activeUserSession.Id, userId);
                            }
                            else
                            {
                                await sessionService.LeaveSession(activeUserSession.Id, userId);
                            }
                        }
                    }
                }
            }
        }
    }
}
