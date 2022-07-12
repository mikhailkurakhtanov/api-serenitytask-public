using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SerenityTask.API.Hubs;
using SerenityTask.API.Models.Enums;
using SerenityTask.API.Models.Entities;
using Task = SerenityTask.API.Models.Entities.Task;
using SerenityTask.API.Models.Responses;
using SerenityTask.API.Extensions;

namespace SerenityTask.API.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IHubContext<TaskHub> _taskHub;

        private readonly IUserDetailsService _userDetailsService;

        private readonly ITaskHistoryNoteService _taskHistoryNoteService;

        private readonly SerenityTaskDbContext _dbContext;

        public TaskService(IHubContext<TaskHub> taskHub, IUserDetailsService userDetailsService,
            ITaskHistoryNoteService taskHistoryNoteService, SerenityTaskDbContext dbContext)
        {
            _taskHub = taskHub;
            _userDetailsService = userDetailsService;
            _taskHistoryNoteService = taskHistoryNoteService;
            _dbContext = dbContext;
        }

        public List<Task> GetTasks(Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            var userTasks = _dbContext.Tasks
                .Where(x => x.User == currentUser && !x.IsCompleted && x.ParentTaskId == null).ToList();

            for (var index = 0; index < userTasks.Count; index++)
            {
                userTasks[index].CreationDateString = userTasks[index].CreationDate.ToString("u");

                if (userTasks[index].Date.HasValue)
                {
                    userTasks[index].DateString = userTasks[index].Date.Value.ToString("u");
                }

                if (userTasks[index].Deadline.HasValue)
                {
                    userTasks[index].DeadlineString = userTasks[index].Deadline.Value.ToString("u");
                }
            }

            return userTasks;
        }

        public Task GetTask(long taskId)
        {
            var choosedTask = _dbContext.Tasks.Find(taskId);

            if (choosedTask != null)
            {
                choosedTask.CreationDateString = choosedTask.CreationDate.ToString("u");
                if (choosedTask.Date.HasValue) choosedTask.DateString = choosedTask.Date.Value.ToString("u");
                if (choosedTask.Deadline.HasValue) choosedTask.DeadlineString = choosedTask.Deadline.Value.ToString("u");
            }

            return choosedTask;
        }

        public async System.Threading.Tasks.Task CreateTask(Task taskToCreate, Guid currentUserId)
        {
            var currentUser = _dbContext.Users.Find(currentUserId);
            taskToCreate.User = currentUser;
            taskToCreate.CreationDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(taskToCreate.DateString))
            {
                taskToCreate.Date = taskToCreate.DateString.GetUtcDateTimeFromString();
            }

            _dbContext.Entry(currentUser).State = EntityState.Unchanged;

            if (taskToCreate.ParentTaskId != null)
            {
                taskToCreate.ParentTask = _dbContext.Tasks.Find(taskToCreate.ParentTaskId);

                var historyNote = await _taskHistoryNoteService.GetSubtaskChanges(EntityAction.Created, taskToCreate);
                taskToCreate.ParentTask.History.Add(historyNote);
            }

            taskToCreate.Id = 0; // instance gets new Id after executing code in 50 row, the solution wasn't found

            _dbContext.Tasks.Add(taskToCreate);
            await _dbContext.SaveChangesAsync();

            var jsonData = JsonConvert.SerializeObject(taskToCreate);

            await _taskHub.Clients.Group($"user_{currentUserId}").SendAsync("receiveCreatedTask", jsonData);
        }

        public async Task<Task> DeleteTask(long taskId)
        {
            var taskToDelete = _dbContext.Tasks.Find(taskId);
            if (taskToDelete == null) return null;

            if (taskToDelete.ParentTaskId != null)
            {
                taskToDelete.ParentTask = _dbContext.Tasks.Find(taskToDelete.ParentTaskId);

                var historyNote = await _taskHistoryNoteService.GetSubtaskChanges(EntityAction.Deleted, taskToDelete);
                taskToDelete.ParentTask.History.Add(historyNote);
            }
            else
            {
                if (taskToDelete.Subtasks.Any())
                {
                    foreach (var subtask in taskToDelete.Subtasks)
                    {
                        DeletePlantHistoryForTask(subtask);
                    }

                    _dbContext.Tasks.RemoveRange(taskToDelete.Subtasks);
                }

                if (taskToDelete.History.Any()) _dbContext.TaskHistory.RemoveRange(taskToDelete.History);

                DeletePlantHistoryForTask(taskToDelete);
            }

            _dbContext.Tasks.Remove(taskToDelete);
            await _dbContext.SaveChangesAsync();

            return taskToDelete.ParentTask != null ? taskToDelete.ParentTask : null;
        }

        public async System.Threading.Tasks.Task CompleteTask(long taskId, Guid currentUserId)
        {
            var taskToComplete = _dbContext.Tasks.Find(taskId);
            if (taskToComplete != null)
            {
                taskToComplete.IsCompleted = true;
                _dbContext.Tasks.Update(taskToComplete);

                if (taskToComplete.ParentTaskId != null)
                {
                    taskToComplete.ParentTask = _dbContext.Tasks.Find(taskToComplete.ParentTaskId);

                    var historyNote = await _taskHistoryNoteService
                        .GetSubtaskChanges(EntityAction.Completed, taskToComplete);

                    taskToComplete.ParentTask.History.Add(historyNote);
                }
                else
                {
                    var subtasks = _dbContext.Tasks.Where(x => x.ParentTask == taskToComplete);
                    if (subtasks.Any())
                    {
                        foreach (var subtask in subtasks)
                        {
                            if (!subtask.IsCompleted)
                            {
                                _dbContext.Tasks.Remove(subtask);
                            }
                        }

                        _dbContext.Tasks.UpdateRange(subtasks);
                    }
                }

                await _dbContext.SaveChangesAsync();

                if (taskToComplete.Deadline.HasValue)
                {
                    var currentDate = DateTime.UtcNow;

                    if (taskToComplete.Deadline.Value > currentDate)
                    {
                        var serenitySoulAchievements = taskToComplete.User.UserDetails.Achievements
                            .Where(x => x.Value < x.Type.Goal && x.Type.Name == "Serenity Soul");

                        if (serenitySoulAchievements.Any())
                        {
                            foreach (var achievement in serenitySoulAchievements)
                            {
                                await _userDetailsService.UpdateAchievement(achievement);
                            }
                        }
                    }
                }

                var response = new CompleteTaskResponse
                {
                    CompletedTaskId = taskToComplete.Id,
                    ParentTask = taskToComplete.ParentTask != null ? taskToComplete.ParentTask : null
                };

                var jsonData = JsonConvert.SerializeObject(response);

                await _taskHub.Clients.Group($"user_{currentUserId}").SendAsync("receiveCompletionResult", jsonData);
            }
        }

        public async System.Threading.Tasks.Task UpdateTask(Task taskToUpdate)
        {
            var taskBeforeChanges = _dbContext.Tasks.Find(taskToUpdate.Id);
            taskToUpdate.CreationDate = taskBeforeChanges.CreationDate;

            if (!string.IsNullOrWhiteSpace(taskToUpdate.DateString))
            {
                taskToUpdate.Date = taskToUpdate.DateString.GetUtcDateTimeFromString();
            }

            if (!string.IsNullOrWhiteSpace(taskToUpdate.DeadlineString))
            {
                taskToUpdate.Deadline = taskToUpdate.DeadlineString.GetUtcDateTimeFromString();
            }

            if (taskToUpdate.ParentTaskId != null)
            {
                taskToUpdate.ParentTask = _dbContext.Tasks.Find(taskToUpdate.ParentTaskId);

                var historyNote = await _taskHistoryNoteService
                    .GetSubtaskChanges(EntityAction.Changed, taskToUpdate, taskBeforeChanges);

                if (taskToUpdate.ParentTask.History != null)
                {
                    taskToUpdate.ParentTask.History.Add(historyNote);
                }
                else taskToUpdate.ParentTask.History = new List<TaskHistoryNote> { historyNote };
            }
            else
            {
                var historyNote = await _taskHistoryNoteService.GetTaskChanges(taskBeforeChanges, taskToUpdate);

                if (taskToUpdate.History != null)
                {
                    taskToUpdate.History.Add(historyNote);
                }
                else taskToUpdate.History = new List<TaskHistoryNote> { historyNote };
            }

            _dbContext.Entry(taskBeforeChanges).State = EntityState.Detached;
            _dbContext.Entry(taskToUpdate).State = EntityState.Modified;

            _dbContext.Update(taskToUpdate);
            await _dbContext.SaveChangesAsync();

            var jsonData = JsonConvert.SerializeObject(taskToUpdate);

            await _taskHub.Clients.Group($"user_{taskToUpdate.UserId}").SendAsync("receiveChangedTask", jsonData);
        }

        private void DeletePlantHistoryForTask(Task taskToDelete)
        {
            var plantHistoryByTask = _dbContext.PlantHistory.Where(x => x.Task == taskToDelete).ToList();
            if (plantHistoryByTask.Any()) _dbContext.PlantHistory.RemoveRange(plantHistoryByTask);
        }

        public ICollection<Task> FindTasksByName(string query, Guid currentUserId)
        {
            var tasksByKeywords = _dbContext.Tasks
                .Where(x => x.Name.Contains(query) && !x.IsCompleted && x.UserId == currentUserId).ToList();
            return tasksByKeywords;
        }
    }
}
