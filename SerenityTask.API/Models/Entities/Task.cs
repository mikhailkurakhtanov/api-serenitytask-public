using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Task
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }

        [JsonIgnore]
        public DateTime? Date { get; set; }

        [JsonIgnore]
        public DateTime? Deadline { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "creationDateString")]
        public string CreationDateString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "dateString")]
        public string DateString { get; set; }

        [NotMapped]
        [JsonProperty(PropertyName = "deadlineString")]
        public string DeadlineString { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public int Priority { get; set; } = 0;

        [JsonProperty(PropertyName = "trackedTime")]
        public int TrackedTime { get; set; } = 0;

        [JsonProperty(PropertyName = "isCompleted")]
        public bool IsCompleted { get; set; } = false;

        [JsonProperty(PropertyName = "parentTaskId")]
        public long? ParentTaskId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public Guid UserId { get; set; }

        #region Virtual

        [JsonIgnore]
        [ForeignKey("ParentTaskId")]
        public virtual Task ParentTask { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskHistoryNote> History { get; set; }

        [JsonProperty(PropertyName = "files")]
        public virtual ICollection<File> Files { get; set; }

        [JsonProperty(PropertyName = "subtasks")]
        public virtual ICollection<Task> Subtasks { get; set; }

        [JsonIgnore]
        public virtual ICollection<PlantHistoryNote> PlantHistory { get; set; }

        #endregion
    }
}
