using System;
using System.Threading.Tasks;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Task;
using SerenityTask.API.Models.Client.Workspace;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface IFileService
    {
        Task<UploadFileResponse> UploadFile(UploadFileRequest request, Guid userId);

        Task<TaskHistoryNote> DeleteFile(long fileId, Guid userId);

        Task<string> GetFileLink(long fileId, Guid userId);

        Task DeleteUserDirectory(Guid currentUserId);
    }
}