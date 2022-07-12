using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Requests.Task
{
    public class UploadFileRequest
    {
        [JsonProperty(PropertyName = "fileData")]
        public IFormFile FileData { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }
    }
}