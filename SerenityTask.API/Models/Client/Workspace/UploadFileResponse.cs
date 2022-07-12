using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models.Client.Workspace
{
    public class UploadFileResponse
    {
        [JsonProperty(PropertyName = "uploadedFile")]
        public File UploadedFile { get; set; }

        [JsonProperty(PropertyName = "taskHistoryNote")]
        public TaskHistoryNote TaskHistoryNote { get; set; }
    }
}