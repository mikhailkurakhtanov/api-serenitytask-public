using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

using Google.Apis.Auth;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

using SerenityTask.API.Components;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client.Authentication;
using GoogleCredential = SerenityTask.API.Models.Entities.GoogleCredential;
using SerenityTask.API.Extensions;

namespace SerenityTask.API.Services.Implementations
{
    public class GoogleIntegrationService : IGoogleIntegrationService
    {
        private readonly SerenityTaskDbContext _dbContext;

        static string[] Scopes = { CalendarService.Scope.Calendar };

        public GoogleIntegrationService(SerenityTaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Manually connection through the account settings
        public async Task ConnectAccount(SocialUser data, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);

            // neeeds to get refresh token
            var payload = GoogleJsonWebSignature.ValidateAsync(data.Token);
            if (payload == null) return;


            var existingCredental = _dbContext.GoogleCredentials.FirstOrDefault(x => x.UserId == currentUserId);
            if (existingCredental == null && currentUser != null)
            {
                var newGoogleCredentials = new GoogleCredential
                {
                    Email = data.Email,
                    Token = data.Token,
                    TokenId = data.TokenId,
                    TokenType = data.TokenDetails.TokenType,
                    ExpiresIn = data.TokenDetails.ExpiresIn,
                    ExpiresAt = data.TokenDetails.ExpiresAt,
                    IssuedUtc = DateTime.UtcNow,
                    Scope = data.TokenDetails.Scope,
                    UserId = currentUserId,
                    User = currentUser
                };

                newGoogleCredentials.CalendarId = await CreateAndGetCalendarId(newGoogleCredentials);
                _dbContext.GoogleCredentials.Add(newGoogleCredentials);

                currentUser.IsGoogleCalendarConnected = true;
                _dbContext.Users.Update(currentUser);

                _dbContext.SaveChanges();
            }
        }

        public void UpdateCredential(SocialUser refreshedData)
        {
            var existingCredential = _dbContext.GoogleCredentials.FirstOrDefault(x => x.Email == refreshedData.Email);
            if (existingCredential != null)
            {
                existingCredential.Token = refreshedData.Token;
                existingCredential.TokenId = refreshedData.TokenId;
                existingCredential.ExpiresAt = refreshedData.TokenDetails.ExpiresAt;
                existingCredential.ExpiresIn = refreshedData.TokenDetails.ExpiresIn;
                existingCredential.IssuedUtc = DateTime.UtcNow;

                _dbContext.GoogleCredentials.Update(existingCredential);
                _dbContext.SaveChanges();
            }
        }

        public async Task DisconnectAccount(Guid currentUserId)
        {
            var existingCredential = _dbContext.GoogleCredentials.FirstOrDefault(x => x.UserId == currentUserId);
            if (existingCredential != null)
            {
                await DeleteGoogleCalendar(existingCredential);

                existingCredential.User.IsGoogleCalendarConnected = false;
                _dbContext.Users.Update(existingCredential.User);

                _dbContext.GoogleCredentials.Remove(existingCredential);
                _dbContext.SaveChanges();
            }
        }

        public bool CheckIsAccountUse(string email)
        {
            var existingCredental = _dbContext.GoogleCredentials.FirstOrDefault(x => x.Email == email);
            return existingCredental != null;
        }

        // Automatic connection on the register with Google
        public async Task<string> CreateAndGetCalendarId(GoogleCredential googleCredential)
        {
            var userCredential = GetUserCredential(googleCredential);
            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential
            });

            var newCalendar = new Calendar();
            newCalendar.Summary = "SerenityTask Planner";
            // newCalendar.TimeZone = googleCredential.User.UserDetails.TimeZone.DisplayName;
            newCalendar.Description = "A calendar created by SerenityTask app to provide real time scheduling group"
                + " focus sessions, tasks reminders and more. Read more at https://serenitytask.com/guide";

            var createdCalendar = await calendarService.Calendars.Insert(newCalendar).ExecuteAsync();
            var createdCalendarEntry = await calendarService.CalendarList.Get(createdCalendar.Id).ExecuteAsync();

            if (createdCalendarEntry != null)
            {
                createdCalendarEntry.ColorId = "9";
                await calendarService.CalendarList.Update(createdCalendarEntry, createdCalendar.Id).ExecuteAsync();
            }

            return createdCalendar.Id;
        }

        public async Task RemoveAttendeeFromEvent(GoogleCredential ownerCredential, string eventId, string participantEmail)
        {
            var userCredential = GetUserCredential(ownerCredential);
            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential
            });

            var sessionEvent = await calendarService.Events.Get(ownerCredential.CalendarId, eventId).ExecuteAsync();
            if (sessionEvent != null)
            {
                var attendeeToExclude = sessionEvent.Attendees.FirstOrDefault(x => x.Email == participantEmail);
                if (attendeeToExclude != null)
                {
                    sessionEvent.Attendees.Remove(attendeeToExclude);
                    await calendarService.Events
                        .Update(sessionEvent, ownerCredential.CalendarId, eventId).ExecuteAsync();
                }
            }
        }

        public async Task DeleteEvent(GoogleCredential ownerCredential, string eventId)
        {
            var userCredential = GetUserCredential(ownerCredential);
            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential
            });

            var sessionEvent = await calendarService.Events.Get(ownerCredential.CalendarId, eventId).ExecuteAsync();
            if (sessionEvent != null && sessionEvent.Status.IndexOf("cancelled") < 0)
            {
                await calendarService.Events.Delete(ownerCredential.CalendarId, eventId).ExecuteAsync();
            }
        }

        public async Task<string> CreateAndGetEventId(Guid currentUserId, Session sessionToPlan)
        {
            var existingCredental = sessionToPlan.Owner.GoogleCredential;
            if (existingCredental == null || existingCredental.CalendarId == null) return null;

            var userCredentials = GetUserCredential(existingCredental);
            if (userCredentials == null) return null;

            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredentials
            });

            var existingCalendar = await calendarService.Calendars.Get(existingCredental.CalendarId).ExecuteAsync();
            if (existingCalendar == null) return null;

            var newEvent = new Event();

            // convert time from utc to user calendar's time zone
            newEvent.Start = new EventDateTime
            {
                DateTime = sessionToPlan.Owner.ConvertDateTimeToUserDateTime(sessionToPlan.StartDate)
            };

            newEvent.End = new EventDateTime
            {
                DateTime = sessionToPlan.Owner.ConvertDateTimeToUserDateTime(sessionToPlan.EndDate)
            };

            if (sessionToPlan.Participants.Any())
            {
                newEvent.Summary = "SerenityTask Group Session";

                newEvent.Attendees = new List<EventAttendee>();
                foreach (var participant in sessionToPlan.Participants)
                {
                    if (participant.GoogleCredential != null)
                    {
                        var eventAttendee = new EventAttendee();
                        eventAttendee.Email = participant.GoogleCredential.Email;
                        eventAttendee.DisplayName = participant.Name;

                        newEvent.Attendees.Add(eventAttendee);
                    }
                }
            }
            else
            {
                newEvent.Summary = "SerenityTask Session";
            }

            newEvent.Description = "Don't forget to join session in the SerenityTask app on 5 minutes before and "
                + "wait for other members! Read more at https://pre-alpha.serenitytask.com/";

            var createdEvent = await calendarService.Events
                .Insert(newEvent, existingCredental.CalendarId).ExecuteAsync();

            return createdEvent.Id;
        }

        #region Private

        private GoogleCredential SaveAndGetGoogleCredential(UserCredential userCredential, string googleEmail)
        {
            var tokenResponse = userCredential.Token;

            var newGoogleCredential = new GoogleCredential
            {
                Email = googleEmail,
                Token = tokenResponse.AccessToken,
                TokenType = tokenResponse.TokenType,
                ExpiresIn = tokenResponse.ExpiresInSeconds,
                RefreshToken = tokenResponse.RefreshToken,
                Scope = tokenResponse.Scope,
                IssuedUtc = tokenResponse.IssuedUtc,
                UserId = Guid.Parse(userCredential.UserId)
            };

            _dbContext.GoogleCredentials.Add(newGoogleCredential);

            var currentUser = _dbContext.Users.Find(Guid.Parse(userCredential.UserId));
            if (currentUser != null)
            {
                currentUser.IsGoogleCalendarConnected = true;
                _dbContext.Users.Update(currentUser);
            }

            _dbContext.SaveChanges();

            return newGoogleCredential;
        }

        private async Task DeleteGoogleCalendar(GoogleCredential existingCredental)
        {
            var userCredentials = GetUserCredential(existingCredental);
            if (userCredentials != null)
            {
                var calendarService = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = userCredentials
                });

                var calendarToDelete = await calendarService.Calendars.Get(existingCredental.CalendarId).ExecuteAsync();
                if (calendarToDelete != null)
                {
                    await calendarService.Calendars.Delete(existingCredental.CalendarId).ExecuteAsync();
                }
            }
        }

        private UserCredential GetUserCredential(GoogleCredential googleCredential)
        {
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = Constants.GoogleOAuthClientId,
                    ClientSecret = Constants.GoogleOAuthClientSecret
                },
                Scopes = Scopes
            });

            var tokenResponse = new TokenResponse
            {
                AccessToken = googleCredential.Token,
                RefreshToken = "", //GoogleCredential.RefreshToken,
                IssuedUtc = DateTime.UtcNow, // GoogleCredential.IssuedUtc,
                ExpiresInSeconds = googleCredential.ExpiresIn
            };

            var userCredential = new UserCredential(flow, googleCredential.UserId.ToString(), tokenResponse);
            return userCredential;
        }

        #endregion
    }
}