using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Requests.Task;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<JsonResult> UploadFile([FromForm] UploadFileRequest uploadFileRequest)
        {
            var uploadFileResponse = await _fileService.UploadFile(uploadFileRequest, UserId);
            return new JsonResult(uploadFileResponse);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<JsonResult> DeleteFile(long fileId)
        {
            var taskHistoryNote = await _fileService.DeleteFile(fileId, UserId);
            return new JsonResult(taskHistoryNote);
        }

        [Authorize]
        [HttpGet("get-link")]
        public async Task<JsonResult> GetFileLink(long fileId)
        {
            var fileLink = await _fileService.GetFileLink(fileId, UserId);
            return new JsonResult(fileLink);
        }
    }
}