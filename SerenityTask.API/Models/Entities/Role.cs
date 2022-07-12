using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SerenityTask.API.Models.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        #region Virtual

        public virtual ICollection<User> Users { get; set; }

        #endregion
    }
}
