using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Services
{
    public interface IUserDetailsService
    {
        ICollection<TimeZoneType> GetTimeZoneTypes();

        Task UpdateAvatar(IFormFile fileData, Guid currentUserId);

        Task UpdateUserDetails(UserDetails changedUserDetails);

        Task UpdateAchievement(Achievement achievementToUpdate);
    }
}
