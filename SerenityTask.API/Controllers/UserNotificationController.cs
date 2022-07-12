using System;
using System.Linq;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;


namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("user-notification")]
    public class UserNotificationController : ControllerBase
    {
        private readonly IUserNotificationService _userNotificationService;

        private Guid UserId => Guid.Parse(User.Claims
            .Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public UserNotificationController(IUserNotificationService userNotificationService)
        {
            _userNotificationService = userNotificationService;
        }

        [Authorize]
        [HttpGet("get-views")]
        public JsonResult GetUserNotificationsViews() {
            var userNotificationsViews = _userNotificationService.GetUserNotificationsViews(UserId);
            return new JsonResult(userNotificationsViews);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task CreateUserNotification([FromBody] UserNotification newUserNotification)
        {
            if (newUserNotification != null) await _userNotificationService.CreateNotification(newUserNotification);
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task DeleteUserNotification(long userNotificationToDeleteId)
        {
            await _userNotificationService.DeleteNotification(userNotificationToDeleteId);
        }

    }
}