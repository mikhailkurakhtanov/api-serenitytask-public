using System.Threading.Tasks;
using SerenityTask.API.Models;
using SerenityTask.API.Models.Enums;
using SerenityTask.API.Models.Entities;
using Task = SerenityTask.API.Models.Entities.Task;

namespace SerenityTask.API.Services;

public interface ITaskHistoryNoteService
{
    Task<TaskHistoryNote> GetTaskChanges(Task taskBeforeChanges, Task changedTask);

    Task<TaskHistoryNote> GetFileChanges(EntityAction fileAction, File newFile);

    Task<TaskHistoryNote> GetSubtaskChanges(EntityAction subtaskAction, Task newSubtask, Task previousSubtask = null);
}