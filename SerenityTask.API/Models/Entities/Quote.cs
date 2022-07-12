using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SerenityTask.API.Models.Entities
{
    public class Quote
    {
        [Key]
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "authorName")]
        public string AuthorName { get; set; }

        [Required]
        [JsonProperty(PropertyName = "context")]
        public string Context { get; set; }
    }
}
