using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserTask = SerenityTask.API.Models.Entities.Task;

namespace SerenityTask.API.Services
{
    public interface ITaskService
    {
        UserTask GetTask(long taskId);

        List<UserTask> GetTasks(Guid currentUserId);

        Task CreateTask(UserTask taskToCreate, Guid currentUserId);

        Task UpdateTask(UserTask changedTask);

        Task CompleteTask(long taskId, Guid currentUserId);

        Task<UserTask> DeleteTask(long taskId);

        ICollection<UserTask> FindTasksByName(string query, Guid currentUserId);
    }
}
