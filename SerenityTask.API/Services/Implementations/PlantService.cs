using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Responses.Plant;
using SerenityTask.API.Models.Requests.Plant;
using SerenityTask.API.Models.Client;
using SerenityTask.API.Models.Client.Workspace;
using SerenityTask.API.Extensions;

namespace SerenityTask.API.Services.Implementations
{
    public class PlantService : IPlantService
    {
        private readonly IHubContext<PlantHub> _plantHub;

        private readonly ITaskService _taskService;

        private readonly IUserDetailsService _userDetailsService;

        private readonly SerenityTaskDbContext _dbContext;

        private DateTime CurrentDate { get; set; }

        public PlantService(IHubContext<PlantHub> plantHub, ITaskService taskService,
        IUserDetailsService userDetailsService, SerenityTaskDbContext dbContext)
        {
            _plantHub = plantHub;
            _taskService = taskService;
            _userDetailsService = userDetailsService;
            _dbContext = dbContext;
        }

        public ICollection<PlantType> GetPlantTypes()
        {
            var plantTypes = _dbContext.PlantTypes.ToList();
            return plantTypes;
        }

        public async Task<Plant> CreatePlant(string plantName, long plantTypeId, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            var plantType = _dbContext.PlantTypes.Find(plantTypeId);

            var newPlant = new Plant
            {
                Name = plantName,
                PlantType = plantType,
                User = currentUser,
            };

            var existingPlants = currentUser.Plants.ToList();
            if (!existingPlants.Any())
            {
                var firstSeedAchievement = currentUser.UserDetails.Achievements.FirstOrDefault(x => x.Type.Name == "First Step");
                if (firstSeedAchievement != null && firstSeedAchievement.Value == 0)
                {
                    await _userDetailsService.UpdateAchievement(firstSeedAchievement);
                }
            }

            _dbContext.Plants.Add(newPlant);
            await _dbContext.SaveChangesAsync();

            return newPlant;
        }

        public async Task UpdatePlant(Plant plantToUpdate)
        {
            _dbContext.Plants.Update(plantToUpdate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePlant(long plantId)
        {
            var plantToDelete = _dbContext.Plants.Find(plantId);
            if (plantToDelete != null)
            {
                _dbContext.Plants.Remove(plantToDelete);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Plant GetPlantByUserId(Guid currentUserId)
        {
            var userPlant = _dbContext.Plants.FirstOrDefault(x => x.UserId == currentUserId && !x.IsGrowthFinished);
            return userPlant;
        }

        public List<PlantHistoryView> GetPlantHistory(long plantId)
        {
            var plantHistory = _dbContext.PlantHistory.Where(x => x.PlantId == plantId);
            if (!plantHistory.Any()) return null;

            var plantHistoryViewList = new List<PlantHistoryView>();

            foreach (var note in plantHistory)
            {
                var plantHistoryView = new PlantHistoryView
                {
                    Id = note.Id,
                    ActionDateString = note.ActionDate.Value.ToString("u"),
                    ReceivedExperience = note.ReceivedExperience,
                    ExperienceObjectType = note.ExperienceObjectType,
                    Description = note.Description,
                    PlantId = note.PlantId
                };

                if (!string.IsNullOrWhiteSpace(note.TaskDetailsJSON))
                {
                    plantHistoryView.TaskDetails = JsonConvert.DeserializeObject<PlantHistoryNoteTaskDetails>(note.TaskDetailsJSON);
                }
                else if (!string.IsNullOrWhiteSpace(note.SessionDetailsJSON))
                {
                    plantHistoryView.SessionDetails = JsonConvert.DeserializeObject<PlantHistoryNoteSessionDetails>(note.SessionDetailsJSON);
                }

                plantHistoryViewList.Add(plantHistoryView);
            }

            return plantHistoryViewList;
        }

        public async Task ChangePlantExperience(ChangePlantExperienceRequest request, Guid currentUserId)
        {
            var currentUserPlant = GetPlantByUserId(currentUserId);
            if (currentUserPlant != null)
            {
                CurrentDate = DateTime.UtcNow;

                switch (request.ObjectType)
                {
                    case ExperienceObjectType.Task:
                        if (request.TaskId != null) await ChangePlantExperienceByTask(request, currentUserPlant);
                        break;
                    case ExperienceObjectType.Session:
                        if (request.SessionId != null)
                        {
                            await ChangePlantExperienceBySession(request, currentUserPlant, currentUserId);
                        }
                        break;
                }
            }
        }

        private async Task ChangePlantExperienceByTask(ChangePlantExperienceRequest request, Plant currentUserPlant)
        {
            var currentTask = _dbContext.Tasks.Find(request.TaskId);
            if (currentTask != null)
            {
                var response = new ChangePlantExperienceResponse();

                var taskDetailsForNote = new PlantHistoryNoteTaskDetails
                {
                    Id = currentTask.Id,
                    Name = currentTask.Name,
                    Priority = currentTask.Priority,
                    IsCompleted = currentTask.IsCompleted
                };

                var newPlantHistoryNote = new PlantHistoryNote
                {
                    ActionDate = CurrentDate,
                    PlantId = currentUserPlant.Id,
                    TaskDetailsJSON = JsonConvert.SerializeObject(taskDetailsForNote),
                    ExperienceObjectType = ExperienceObjectType.Task
                };

                switch (request.ChangingType)
                {
                    case ExperienceChangingType.Rise:
                        double experienceToRise = 0;

                        switch (request.ReasonType)
                        {
                            case ExperienceReasonType.Task_Completed:
                                experienceToRise = currentTask.Priority == 0 ? 1 : 2;
                                if (currentTask.Deadline != null && currentTask.Deadline > CurrentDate)
                                {
                                    experienceToRise *= 2;
                                    newPlantHistoryNote.ReceivedExperience = experienceToRise;
                                    newPlantHistoryNote.Description = "Rised " + experienceToRise
                                        + " exp. for completing the task before a deadline";
                                }
                                else
                                {
                                    newPlantHistoryNote.ReceivedExperience = experienceToRise;
                                    newPlantHistoryNote.Description = "Rised " + experienceToRise
                                        + " exp. for completing the task";
                                }

                                break;

                            case ExperienceReasonType.Task_TrackedTime:
                                if (request.TrackedTimeInMinutes != null)
                                {
                                    while (request.TrackedTimeInMinutes > 0)
                                    {
                                        experienceToRise += currentTask.Priority == 0 ? 0.1 : 0.2;
                                        request.TrackedTimeInMinutes -= 10;
                                    }

                                    newPlantHistoryNote.ReceivedExperience = experienceToRise;
                                    newPlantHistoryNote.Description = "Rised " + experienceToRise
                                        + " exp. for tracking time of the task";
                                }
                                break;
                        }

                        await RisePlantExperience(experienceToRise, request, currentUserPlant);
                        break;

                    case ExperienceChangingType.Reduce:
                        var experienceToReduce = 0;

                        switch (request.ReasonType)
                        {
                            case ExperienceReasonType.Task_Deleted:
                                experienceToReduce = currentTask.Priority == 0 ? 2 : 4;
                                newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                newPlantHistoryNote.Description = "Reduced "
                                    + experienceToReduce + " exp. for deadline expiration";
                                break;

                            case ExperienceReasonType.Task_ChangedDeadline:
                                experienceToReduce = currentTask.Priority == 0 ? 4 : 8;
                                newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                newPlantHistoryNote.Description = "Reduced "
                                    + experienceToReduce + " exp. for deadline changing";
                                break;

                            case ExperienceReasonType.Task_ExpiredDeadline:
                                var daysAfterDeadline = (CurrentDate.Date - currentTask.Deadline.Value.Date).Days;
                                experienceToReduce = (currentTask.Priority == 0 ? 1 : 2) * daysAfterDeadline;
                                newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                newPlantHistoryNote.Description = "Reduced "
                                    + experienceToReduce + " exp. for deadline expiration";
                                break;
                        }

                        // check if currentUserPLant was changing
                        await ReducePlantExperience(experienceToReduce, request, currentUserPlant);
                        break;
                }

                _dbContext.PlantHistory.Add(newPlantHistoryNote);
                await _dbContext.SaveChangesAsync();

                response.Level = currentUserPlant.Level;
                response.Experience = currentUserPlant.CurrentExperience;
                response.MaxExperience = currentUserPlant.MaxExperience;

                var plantHistoryView = new PlantHistoryView
                {
                    Id = newPlantHistoryNote.Id,
                    ActionDateString = newPlantHistoryNote.ActionDate.Value.ToString("u"),
                    ReceivedExperience = newPlantHistoryNote.ReceivedExperience,
                    Description = newPlantHistoryNote.Description,
                    ExperienceObjectType = newPlantHistoryNote.ExperienceObjectType,
                    TaskDetails = taskDetailsForNote
                };

                response.PlantHistoryView = plantHistoryView;
                await SendChangePlantExperienceResponse(response, currentUserPlant.User.Id);
            }
        }

        private async Task ChangePlantExperienceBySession(ChangePlantExperienceRequest request, Plant currentUserPlant, Guid currentUserId)
        {
            var currentSession = _dbContext.Sessions.Find(request.SessionId);
            if (currentSession != null)
            {
                var sessionMembers = JsonConvert.DeserializeObject<List<SessionMember>>(currentSession.SessionMembersJSON);
                if (sessionMembers.Any())
                {
                    var response = new ChangePlantExperienceResponse();
                    var sessionDetailsForNote = new PlantHistoryNoteSessionDetails();

                    var newPlantHistoryNote = new PlantHistoryNote
                    {
                        ActionDate = CurrentDate,
                        PlantId = currentUserPlant.Id,
                        ExperienceObjectType = ExperienceObjectType.Session,
                        SessionDetailsJSON = JsonConvert.SerializeObject(sessionDetailsForNote)
                    };

                    var newPlantHistoryNoteForInjuredMembers = new PlantHistoryNote
                    {
                        ActionDate = CurrentDate,
                        ExperienceObjectType = ExperienceObjectType.Session,
                        SessionDetailsJSON = JsonConvert.SerializeObject(sessionDetailsForNote)
                    };

                    switch (request.ChangingType)
                    {
                        case ExperienceChangingType.Rise:
                            double experienceToRise = 0;

                            switch (request.ReasonType)
                            {
                                case ExperienceReasonType.Session_Finished:
                                    foreach (var sessionMember in sessionMembers)
                                    {
                                        if (sessionMember.SessionMemberTasks.Any())
                                        {
                                            var sessionMemberAsUser = _dbContext.Users.Find(sessionMember.UserId);
                                            if (sessionMemberAsUser != null)
                                            {
                                                var sessionMemberPlant = sessionMemberAsUser.Plants
                                                    .FirstOrDefault(x => !x.IsGrowthFinished);

                                                if (sessionMemberPlant != null)
                                                {
                                                    newPlantHistoryNote.PlantId = sessionMemberPlant.Id;

                                                    foreach (var sessionMemberTask in sessionMember.SessionMemberTasks)
                                                    {
                                                        var trackedTimeTask = _dbContext.Tasks.Find(sessionMemberTask.TaskId);
                                                        if (trackedTimeTask != null)
                                                        {
                                                            trackedTimeTask.TrackedTime += Convert
                                                                .ToInt32(sessionMemberTask.TrackedTime / 60);

                                                            var trackedTimeInMinutes = trackedTimeTask.TrackedTime;
                                                            while (trackedTimeInMinutes >= 10 && trackedTimeInMinutes > 0)
                                                            {
                                                                experienceToRise += trackedTimeTask.Priority == 0 ? 0.1 : 0.2;
                                                                trackedTimeInMinutes -= 10;
                                                            }

                                                            await _taskService.UpdateTask(trackedTimeTask);
                                                            await RisePlantExperience(experienceToRise, request, currentUserPlant);

                                                            newPlantHistoryNote.ReceivedExperience = experienceToRise;
                                                            newPlantHistoryNote.Description = "Rised " + experienceToRise
                                                                + " exp. for tracking time of the task during the focus session";

                                                            _dbContext.PlantHistory.Add(newPlantHistoryNote);
                                                            await _dbContext.SaveChangesAsync();

                                                            await CreateAndSendChangePlantExperienceResponse(currentUserPlant, newPlantHistoryNote, sessionDetailsForNote);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case ExperienceChangingType.Reduce:
                            double experienceToReduce = 0;
                            double experienceToReduceForInjuredMembers = 0;

                            switch (request.ReasonType)
                            {
                                case ExperienceReasonType.Session_Canceled:
                                    experienceToReduce = currentUserId == currentSession.OwnerId
                                        ? currentSession.Duration * 0.1 * 2
                                        : currentSession.Duration * 0.1;

                                    newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                    newPlantHistoryNote.Description = "Reduced " + experienceToReduce
                                        + " exp. for cancelling the session";

                                    experienceToReduceForInjuredMembers = currentSession.Duration * 0.1;
                                    newPlantHistoryNoteForInjuredMembers.ReceivedExperience = experienceToReduceForInjuredMembers * -1;
                                    newPlantHistoryNoteForInjuredMembers.Description = "Reduced " + experienceToReduceForInjuredMembers
                                        + " exp. because " + currentUserPlant.User.Name + " had cancelled the session";

                                    break;

                                case ExperienceReasonType.Session_Leaved:
                                    experienceToReduce = currentUserId == currentSession.OwnerId
                                        ? currentSession.Duration * 0.1 * 2
                                        : currentSession.Duration * 0.1;

                                    newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                    newPlantHistoryNote.Description = "Reduced " + experienceToReduce
                                        + " exp. for leaving the session";

                                    experienceToReduceForInjuredMembers = currentSession.Duration * 0.1;
                                    newPlantHistoryNoteForInjuredMembers.ReceivedExperience = experienceToReduceForInjuredMembers * -1;
                                    newPlantHistoryNoteForInjuredMembers.Description = "Reduced " + experienceToReduceForInjuredMembers
                                        + " exp. because " + currentUserPlant.User.Name + " had leaved the session";

                                    break;

                                case ExperienceReasonType.Session_Interrupted:
                                    var guiltyMember = _dbContext.Users.Find(request.GuiltyMemberId);
                                    var guiltyMemberPlant = guiltyMember.Plants.FirstOrDefault(x => !x.IsGrowthFinished);
                                    if (guiltyMemberPlant != null)
                                    {

                                        experienceToReduce = guiltyMember.Id == currentSession.OwnerId
                                            ? currentSession.Duration * 0.1 * 2
                                            : currentSession.Duration * 0.1;

                                        newPlantHistoryNote.ReceivedExperience = experienceToReduce * -1;
                                        newPlantHistoryNote.Description = "Reduced " + experienceToReduce
                                            + " exp. for interrupting the session";

                                        await ReducePlantExperience(experienceToReduce, request, guiltyMemberPlant);

                                        _dbContext.PlantHistory.Add(newPlantHistoryNote);
                                        await _dbContext.SaveChangesAsync();

                                        await CreateAndSendChangePlantExperienceResponse(guiltyMemberPlant, newPlantHistoryNote, sessionDetailsForNote);
                                    }

                                    var injuredMembersPlants = new List<Plant>();
                                    if (currentSession.OwnerId == guiltyMember.Id)
                                    {
                                        var participantsPlants = currentSession.Participants.Select(x
                                            => x.Plants.FirstOrDefault(x => !x.IsGrowthFinished));

                                        injuredMembersPlants.AddRange(participantsPlants);
                                    }
                                    else
                                    {
                                        injuredMembersPlants.Add(currentSession.Owner.Plants
                                            .FirstOrDefault(x => !x.IsGrowthFinished));

                                        var participantsPlants = currentSession.Participants
                                            .Where(x => x.Id != guiltyMember.Id)
                                            .Select(x => x.Plants.FirstOrDefault(x => !x.IsGrowthFinished));

                                        injuredMembersPlants.AddRange(participantsPlants);
                                    }

                                    experienceToReduceForInjuredMembers = currentSession.Duration * 0.1;
                                    newPlantHistoryNoteForInjuredMembers.ReceivedExperience = experienceToReduceForInjuredMembers * -1;
                                    newPlantHistoryNoteForInjuredMembers.Description = "Reduced " + experienceToReduceForInjuredMembers
                                        + " exp. because " + guiltyMember.Name + " had interrupted the session";

                                    foreach (var injuredMemberPlant in injuredMembersPlants)
                                    {
                                        await ReducePlantExperience(experienceToReduceForInjuredMembers, request, injuredMemberPlant);

                                        _dbContext.PlantHistory.Add(newPlantHistoryNoteForInjuredMembers);
                                        await _dbContext.SaveChangesAsync();

                                        await CreateAndSendChangePlantExperienceResponse(injuredMemberPlant,
                                            newPlantHistoryNoteForInjuredMembers, sessionDetailsForNote);
                                    }

                                    break;
                            }

                            if (currentUserId == currentSession.OwnerId)
                            {
                                foreach (var participant in currentSession.Participants)
                                {
                                    var activePlant = participant.Plants.FirstOrDefault(x => !x.IsGrowthFinished);
                                    if (activePlant != null)
                                    {
                                        await ReducePlantExperience(experienceToReduce, request, activePlant);

                                        newPlantHistoryNoteForInjuredMembers.PlantId = activePlant.Id;
                                        _dbContext.PlantHistory.Add(newPlantHistoryNoteForInjuredMembers);
                                        await _dbContext.SaveChangesAsync();

                                        await CreateAndSendChangePlantExperienceResponse(activePlant,
                                            newPlantHistoryNoteForInjuredMembers, sessionDetailsForNote);
                                    }
                                }
                            }
                            else
                            {
                                var sessionMembersPlants = new List<Plant>();

                                var activeOwnersPlant = currentSession.Owner.Plants.FirstOrDefault(x => !x.IsGrowthFinished);
                                if (activeOwnersPlant != null)
                                {
                                    sessionMembersPlants.Add(activeOwnersPlant);
                                }

                                foreach (var participant in currentSession.Participants)
                                {
                                    if (participant.Id != currentUserId)
                                    {
                                        var activeParticipantsPlant = participant.Plants
                                            .FirstOrDefault(x => !x.IsGrowthFinished);

                                        if (activeParticipantsPlant != null)
                                        {
                                            sessionMembersPlants.Add(activeParticipantsPlant);
                                        }
                                    }
                                }

                                foreach (var membersPlant in sessionMembersPlants)
                                {
                                    await ReducePlantExperience(experienceToReduce, request, membersPlant);

                                    newPlantHistoryNoteForInjuredMembers.PlantId = membersPlant.Id;
                                    _dbContext.PlantHistory.Add(newPlantHistoryNoteForInjuredMembers);
                                    await _dbContext.SaveChangesAsync();

                                    await CreateAndSendChangePlantExperienceResponse(membersPlant,
                                        newPlantHistoryNoteForInjuredMembers, sessionDetailsForNote);
                                }
                            }

                            if (request.ReasonType != ExperienceReasonType.Session_Interrupted)
                            {
                                await ReducePlantExperience(experienceToReduce, request, currentUserPlant);

                                _dbContext.PlantHistory.Add(newPlantHistoryNote);
                                await _dbContext.SaveChangesAsync();

                                await CreateAndSendChangePlantExperienceResponse(currentUserPlant, newPlantHistoryNote, sessionDetailsForNote);
                            }

                            break;
                    }
                }
            }
        }

        private async Task CreateAndSendChangePlantExperienceResponse(Plant userPlant,
            PlantHistoryNote plantHistoryNote, PlantHistoryNoteSessionDetails sessionDetails)
        {
            var response = new ChangePlantExperienceResponse();
            response.Level = userPlant.Level;
            response.Experience = userPlant.CurrentExperience;
            response.MaxExperience = userPlant.MaxExperience;

            var plantHistoryView = new PlantHistoryView
            {
                Id = plantHistoryNote.Id,
                ActionDateString = plantHistoryNote.ActionDate.Value.ToString("u"),
                ReceivedExperience = plantHistoryNote.ReceivedExperience,
                Description = plantHistoryNote.Description,
                ExperienceObjectType = plantHistoryNote.ExperienceObjectType,
                SessionDetails = sessionDetails
            };

            response.PlantHistoryView = plantHistoryView;
            await SendChangePlantExperienceResponse(response, userPlant.User.Id);
        }

        private async Task RisePlantExperience(double experienceToRise, ChangePlantExperienceRequest request, Plant currentPlant)
        {
            currentPlant.CurrentExperience += experienceToRise;

            if (currentPlant.CurrentExperience >= currentPlant.MaxExperience)
            {
                if (currentPlant.Level + 1 == currentPlant.PlantType.MaxLeaves)
                {
                    currentPlant.Level = currentPlant.PlantType.MaxLeaves;
                    currentPlant.IsGrowthFinished = true;

                    if (currentPlant.TotalDeadLeaves == 0)
                    {
                        var gardenerAchievements = currentPlant.User.UserDetails.Achievements
                            .Where(x => x.Value < x.Type.Goal && x.Type.Name == "Gardener");

                        if (gardenerAchievements.Any())
                        {
                            foreach (var achievement in gardenerAchievements)
                            {
                                await _userDetailsService.UpdateAchievement(achievement);
                            }
                        }
                    }
                }
                else
                {
                    currentPlant.Level += 1;
                    currentPlant.CurrentExperience = currentPlant.CurrentExperience - currentPlant.MaxExperience;
                    currentPlant.MaxExperience *= 2;
                }
            }

            _dbContext.Plants.Update(currentPlant);
            await _dbContext.SaveChangesAsync();
        }

        private async Task ReducePlantExperience(double experienceToReduce, ChangePlantExperienceRequest request, Plant currentPlant)
        {
            currentPlant.CurrentExperience -= experienceToReduce;

            if (currentPlant.CurrentExperience < 0 && currentPlant.Level > 1)
            {
                while (currentPlant.CurrentExperience < 0)
                {
                    currentPlant.Level -= 1;
                    currentPlant.MaxExperience /= 2;
                    currentPlant.CurrentExperience = currentPlant.MaxExperience + currentPlant.CurrentExperience;

                    if (currentPlant.Level < 0 && request.TaskId != null)
                    {
                        var taskHistoryNoteAction = "The plant \"" + currentPlant.Name + "\""
                            + " was dead because of " + GetTaskHistoryNoteActionByReasonType(request.ReasonType);

                        var taskHistoryNote = new TaskHistoryNote
                        {
                            Date = CurrentDate,
                            Action = taskHistoryNoteAction,
                            TaskId = request.TaskId.Value
                        };

                        _dbContext.TaskHistory.Add(taskHistoryNote);

                        currentPlant.IsDead = true;
                        break;
                    }
                }
            }

            if (currentPlant.CurrentExperience < 0 && currentPlant.Level == 0)
            {
                currentPlant.IsDead = true;
                currentPlant.IsGrowthFinished = true;
            }

            _dbContext.Plants.Update(currentPlant);
            await _dbContext.SaveChangesAsync();
        }

        private string GetTaskHistoryNoteActionByReasonType(ExperienceReasonType reasonType)
        {
            var taskHistoryNoteAction = "";
            switch (reasonType)
            {
                case ExperienceReasonType.Task_Deleted:
                    taskHistoryNoteAction = "the deleted task";
                    break;
                case ExperienceReasonType.Task_ChangedDeadline:
                    taskHistoryNoteAction = "the changed deadline of the task";
                    break;
                case ExperienceReasonType.Task_ExpiredDeadline:
                    taskHistoryNoteAction = "the expired deadline of the task";
                    break;
                case ExperienceReasonType.Session_Canceled:
                    taskHistoryNoteAction = "the cancelled session";
                    break;
                case ExperienceReasonType.Session_Interrupted:
                    taskHistoryNoteAction = "the interrupted session";
                    break;
                case ExperienceReasonType.Session_Leaved:
                    taskHistoryNoteAction = "the leaved session";
                    break;
            }

            return taskHistoryNoteAction;
        }

        private async Task SendChangePlantExperienceResponse(ChangePlantExperienceResponse response, Guid currentUserId)
        {
            var jsonData = JsonConvert.SerializeObject(response);
            await _plantHub.Clients.Group($"user_{currentUserId}").SendAsync("receivePlantChanges", jsonData);
        }
    }
}
