using System;
using System.Threading.Tasks;
using SerenityTask.API.Models;
using SerenityTask.API.Models.Enums;
using SerenityTask.API.Models.Entities;
using Task = SerenityTask.API.Models.Entities.Task;

namespace SerenityTask.API.Services.Implementations;

public class TaskHistoryNoteService : ITaskHistoryNoteService
{
    public readonly SerenityTaskDbContext _dbContext;

    public TaskHistoryNoteService(SerenityTaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private string Action { get; set; }

    public async Task<TaskHistoryNote> GetTaskChanges(Task taskBeforeChanges, Task changedTask)
    {
        if (taskBeforeChanges.Date != changedTask.Date)
        {
            if (taskBeforeChanges.Date == null) Action = "Date was set to " + changedTask.Date.ToString();
            else Action = "Date was changed from "
                + taskBeforeChanges.Date.ToString() + " to " + changedTask.Date.ToString();
        }

        else if (taskBeforeChanges.Deadline != changedTask.Deadline)
        {
            if (taskBeforeChanges.Deadline == null)
            {
                Action = "Deadline was set to " + changedTask.Deadline.ToString();
            }
            else
            {
                Action = "Deadline was changed from " + taskBeforeChanges.Deadline.ToString()
                    + " to " + changedTask.Deadline.ToString();
            }
        }

        else if (taskBeforeChanges.Name != changedTask.Name)
        {
            Action = "Name was changed from " + '\"' + taskBeforeChanges.Name + '\"'
                + " to " + '\"' + changedTask.Name + '\"';
        }

        else if (taskBeforeChanges.TrackedTime < changedTask.TrackedTime)
        {
            Action = "Focus mode: " + (changedTask.TrackedTime - taskBeforeChanges.TrackedTime) + " min were tracked";
        }

        return await GetTaskHistoryNote(changedTask.Id);
    }

    public async Task<TaskHistoryNote> GetFileChanges(EntityAction fileAction, File newFile)
    {
        switch (fileAction)
        {
            case EntityAction.Uploaded:
                Action = "File " + newFile.Name + " was attached";
                break;
            case EntityAction.Deleted:
                Action = "File " + newFile.Name + " was removed";
                break;
        }

        return await GetTaskHistoryNote(newFile.TaskId);
    }

    public async Task<TaskHistoryNote> GetSubtaskChanges(EntityAction subtaskAction, Task newSubtask, Task previousSubtask = null)
    {
        switch (subtaskAction)
        {
            case EntityAction.Created:
                Action = "Subtask " + '\"' + newSubtask.Name + '\"' + " was created";
                break;
            case EntityAction.Changed:
                Action = "Subtask " + '\"' + previousSubtask.Name + '\"'
                    + " was renamed to " + '\"' + newSubtask.Name + '\"';
                break;
            case EntityAction.Deleted:
                Action = "Subtask " + '\"' + newSubtask.Name + '\"' + " was removed";
                break;
            case EntityAction.Completed:
                Action = "Subtask " + '\"' + newSubtask.Name + '\"' + " was completed";
                break;
        }

        return await GetTaskHistoryNote((long)newSubtask.ParentTaskId);
    }

    #region Private

    private async Task<TaskHistoryNote> GetNameChangesNote(string previousName, string newName, long taskId)
    {
        Action = "Task name was changed from " + '\"' + previousName + '\"' + "to " + '\"' + newName + '\"';
        return await GetTaskHistoryNote(taskId);
    }

    private async Task<TaskHistoryNote> GetDateChangesNote(DateTime? previousDate, DateTime? newDate, long taskId)
    {
        while (Action.Length == 0)
        {
            if (previousDate == null) Action = "Applied date to " + newDate.ToString();
            if (newDate == null) Action = "Task was moved to Inbox";
            if (previousDate != newDate) Action = "Date changed from "
                + previousDate.ToString() + " to " + newDate.ToString();
        }

        return await GetTaskHistoryNote(taskId);
    }

    private async Task<TaskHistoryNote> GetDeadlineChangesNote(DateTime? previousDate, DateTime? newDate, long taskId)
    {
        while (Action.Length == 0)
        {
            if (previousDate == null) Action = "Applied deadline to " + newDate.ToString();
            if (newDate == null) Action = "Deadline was removed";
            if (previousDate != newDate) Action = "Deadline changed from "
                + previousDate.ToString() + " to " + newDate.ToString();
        }

        return await GetTaskHistoryNote(taskId);
    }

    private async Task<TaskHistoryNote> GetReminderChangesNote(DateTime? previousReminder, DateTime? newReminder, long taskId)
    {
        while (Action.Length == 0)
        {
            if (previousReminder == null) Action = "Applied reminder to " + newReminder.ToString();
            if (newReminder == null) Action = "Reminder was removed";
            if (previousReminder != newReminder) Action = "Reminder changed from "
                + previousReminder.ToString() + " to " + newReminder.ToString();
        }

        return await GetTaskHistoryNote(taskId);
    }

    private async Task<TaskHistoryNote> GetTrackedTimeChanges(int previousTrackedTime, int newTrackedTime, long taskId)
    {
        Action = "Was tracked " + (newTrackedTime - previousTrackedTime) + " min in focus mode";
        return await GetTaskHistoryNote(taskId);
    }

    private async Task<TaskHistoryNote> GetTaskHistoryNote(long taskId)
    {
        var newTaskHistoryNote = new TaskHistoryNote
        {
            Date = DateTime.UtcNow,
            Action = Action,
            TaskId = taskId
        };

        _dbContext.TaskHistory.Add(newTaskHistoryNote);
        await _dbContext.SaveChangesAsync();

        Action = "";

        return newTaskHistoryNote;
    }

    #endregion
}
