using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Models
{
    public class File
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "uploadDate")]
        public DateTime? UploadDate { get; set; }

        [JsonProperty(PropertyName = "extension")]
        public string Extension { get; set; }

        [JsonProperty(PropertyName = "size")]
        public double Size { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        #region Virtual

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [JsonIgnore]
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }

        #endregion
    }
}