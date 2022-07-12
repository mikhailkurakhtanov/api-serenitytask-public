using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class TaskHistoryNote : IBaseEntity
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public long TaskId { get; set; }

        [ForeignKey("TaskId")]
        [JsonProperty(PropertyName = "task")]
        public virtual Task Task { get; set; }
    }
}