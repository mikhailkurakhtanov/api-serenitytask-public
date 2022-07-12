using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.User;
using SerenityTask.API.Models.Requests;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("get")]
        public User Get()
        {
            var currentUserId = Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var currentUser = _userService.GetCurrentUser(currentUserId);

            return currentUser;
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(User changedUser)
        {
            if (changedUser == null) return BadRequest();

            await _userService.UpdateUser(changedUser);
            return Ok();
        }

        [HttpGet("confirm-account")]
        public async Task<IActionResult> ConfirmAccount(string userId, string confirmationToken)
        {
            if (await _userService.ConfirmAccount(userId, confirmationToken))
            {
                return Redirect(Constants.AppUrl);
            }

            return BadRequest();
        }

        [HttpGet("confirm-password")]
        public async Task<IActionResult> ConfirmPassword(string userId, string newPasswordHash, string confirmationToken)
        {
            if (string.IsNullOrWhiteSpace(userId)) return BadRequest();
            await _userService.ConfirmPassword(userId, newPasswordHash, confirmationToken);

            return Redirect(Constants.AppUrl);
        }

        [HttpGet("check-username")]
        public bool CheckUsernameOnExist(string username)
        {
            var checkResult = _userService.IsUsernameAvailable(username);
            return checkResult;
        }

        [HttpGet("check-email")]
        public bool CheckEmailOnExist(string email)
        {
            var checkResult = _userService.IsEmailAvailable(email);
            return checkResult;
        }

        [Authorize]
        [HttpPost("get-user-cards")]
        public JsonResult GetUserCards([FromBody] GetUserCardsRequest getUserCardsRequest)
        {
            var currentUserId = Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var userDetails = _userService.GetUserCards(getUserCardsRequest, currentUserId);

            return new JsonResult(userDetails);
        }

        [Authorize]
        [HttpPut("update-settings")]
        public async Task<JsonResult> UpdateUserSettings([FromBody] UserSettings changedUserSettings)
        {
            var currentUserId = Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var updatedUserSettings = await _userService.UpdateUserSettings(changedUserSettings, currentUserId);

            return new JsonResult(updatedUserSettings);
        }

        [Authorize]
        [HttpPost("accept-friend-request")]
        public async Task AcceptFriendRequest(UserNotificationConnector request)
        {
            if (request != null) await _userService.AcceptFriendRequest(request);
        }

        [Authorize]
        [HttpGet("remove-friend")]
        public async Task RemoveUserFromFriendsList(Guid friendToRemoveId)
        {
            var currentUserId = Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
            await _userService.RemoveUserFromFriendsList(friendToRemoveId, currentUserId);
        }
    }
}
