using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Http;
using FluentFTP;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Enums;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Task;
using SerenityTask.API.Models.Client.Workspace;
using File = SerenityTask.API.Models.File;

namespace SerenityTask.API.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly SerenityTaskDbContext _dbContext;

        private readonly ITaskHistoryNoteService _taskHistoryNoteService;

        public FileService(SerenityTaskDbContext dbContext, ITaskHistoryNoteService taskHistoryNoteService)
        {
            _dbContext = dbContext;
            _taskHistoryNoteService = taskHistoryNoteService;
        }

        public async Task<TaskHistoryNote> DeleteFile(long fileId, Guid currentUserId)
        {
            var fileToDelete = _dbContext.Files.Find(fileId);
            if (fileToDelete == null) return null;

            var ftpClient = new FtpClient(Constants.FtpHost, Constants.FtpUser, Constants.FtpPassword);
            var currentUserFilesDirectory = await GetUserFilesDirectory(ftpClient, currentUserId);
            var filePath = Path.Combine(currentUserFilesDirectory, fileToDelete.Name);
            await ftpClient.DeleteFileAsync(filePath);

            _dbContext.Files.Remove(fileToDelete);
            await _dbContext.SaveChangesAsync();

            return await _taskHistoryNoteService.GetFileChanges(EntityAction.Deleted, fileToDelete);
        }

        public async Task<string> GetFileLink(long fileId, Guid currentUserId)
        {
            var fileToDownload = _dbContext.Files.Find(fileId);
            if (fileToDownload == null) return null;

            var ftpClient = new FtpClient(Constants.FtpHost, Constants.FtpUser, Constants.FtpPassword);
            var currentUserFilesDirectory = await GetUserFilesDirectory(ftpClient, currentUserId);

            var fileLink = "https://" + Path.Combine(currentUserFilesDirectory, fileToDownload.Name);
            return fileLink;
        }

        public async Task<UploadFileResponse> UploadFile(UploadFileRequest uploadFileRequest, Guid currentUserId)
        {
            var fileToUpload = uploadFileRequest.FileData;
            var uniqueFileName = GetUniqueFileName(fileToUpload.FileName);

            var newFile = new File
            {
                Name = uniqueFileName,
                UploadDate = DateTime.UtcNow,
                Extension = Path.GetExtension(fileToUpload.FileName),
                Size = Math.Round(Convert.ToDouble((fileToUpload.Length / 1024f) / 1024f), 2),
                UserId = currentUserId,
                TaskId = uploadFileRequest.TaskId
            };

            var totalStorageSizeForUser = _dbContext.Files
                .Where(x => x.UserId == currentUserId).Select(x => x.Size).Sum();

            if (totalStorageSizeForUser + newFile.Size >= 10) return null;

            await UploadFileToStorage(fileToUpload, uniqueFileName, currentUserId);

            _dbContext.Files.Add(newFile);
            await _dbContext.SaveChangesAsync();

            var historyNote = await _taskHistoryNoteService.GetFileChanges(EntityAction.Uploaded, newFile);
            var uploadFileResponse = new UploadFileResponse
            {
                UploadedFile = newFile,
                TaskHistoryNote = historyNote
            };

            return uploadFileResponse; // return fileInfo and taskHistoryNote
        }

        public async Task DeleteUserDirectory(Guid currentUserId)
        {
            var ftpClient = new FtpClient(Constants.FtpHost, Constants.FtpUser, Constants.FtpPassword);
            var currentUserDirectory = await GetUserDirectory(ftpClient, currentUserId);

            if (!string.IsNullOrWhiteSpace(currentUserDirectory))
            {
                await ftpClient.DeleteDirectoryAsync(currentUserDirectory);
            }
        }

        #region Private Methods

        private static async Task<string> GetUserDirectory(FtpClient ftpClient, Guid currentUserId)
        {
            var currentUserDirectory = Path.Combine(Constants.UserStorageUrl, currentUserId.ToString());
            var isUserDirectoryExists = await ftpClient.DirectoryExistsAsync(currentUserDirectory);

            return isUserDirectoryExists ? currentUserDirectory : null;
        }

        private static async Task<string> GetUserFilesDirectory(FtpClient ftpClient, Guid currentUserId)
        {
            var currentUserDirectory = Path.Combine(Constants.UserStorageUrl, currentUserId.ToString());
            var currentUserFilesDirectory = Path.Combine(currentUserDirectory, "files");

            var isUserDirectoryExists = await ftpClient.DirectoryExistsAsync(currentUserDirectory);
            if (!isUserDirectoryExists) await ftpClient.CreateDirectoryAsync(currentUserDirectory);

            var isFilesDirectoryExists = await ftpClient.DirectoryExistsAsync(currentUserFilesDirectory);
            if (!isFilesDirectoryExists) await ftpClient.CreateDirectoryAsync(currentUserFilesDirectory);

            return currentUserFilesDirectory;
        }

        private static async Task UploadFileToStorage(IFormFile fileData, string fileName, Guid currentUserId)
        {
            var ftpClient = new FtpClient(Constants.FtpHost, Constants.FtpUser, Constants.FtpPassword);
            var currentUserFilesDirectory = await GetUserFilesDirectory(ftpClient, currentUserId);

            var filePath = Path.Combine(currentUserFilesDirectory, fileName);

            var memoryStream = new MemoryStream();
            fileData.CopyTo(memoryStream);

            await ftpClient.UploadAsync(memoryStream.ToArray(), filePath, FtpRemoteExists.Skip);
        }

        private string GetUniqueFileName(string fileName)
        {
            var index = fileName.Length - 1;

            while (true)
            {
                var lastCharacterFromFileName = fileName.Substring(index);
                if (lastCharacterFromFileName.Substring(0, 1) == ".") break;
                index--;
            }

            var fileNameWithoutExtension = fileName.Substring(0, index);
            var existingFilesWithTheSameNameNumber = _dbContext.Files.Where(x => x.Name == fileName).Count();
            if (existingFilesWithTheSameNameNumber == 0)
            {
                return fileNameWithoutExtension + Path.GetExtension(fileName);
            }

            return fileNameWithoutExtension + " ("
                + existingFilesWithTheSameNameNumber + ")" + Path.GetExtension(fileName);
        }

        #endregion
    }
}