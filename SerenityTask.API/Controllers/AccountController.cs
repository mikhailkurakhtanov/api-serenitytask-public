using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Components;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private Guid UserId => Guid.Parse(User.Claims
            .Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet("change-password")]
        public async Task<JsonResult> ChangePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return null;

            var result = await _accountService.ChangePassword(newPassword, UserId);
            return new JsonResult(result);
        }

        [HttpGet("confirm-password")]
        public async Task<IActionResult> ConfirmPassword(string newPassword, string confirmationToken, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmationToken)) return BadRequest();
            await _accountService.ConfirmPassword(newPassword, confirmationToken, userId);

            return Redirect(Constants.AppUrl);
        }

        [Authorize]
        [HttpGet("check-email")]
        public bool CheckEmailOnAvailability(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var result = _accountService.CheckEmailOnAvailability(email);
            return result;
        }

        [Authorize]
        [HttpGet("change-email")]
        public async Task<JsonResult> ChangeEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail)) return null;

            var result = await _accountService.ChangeEmail(newEmail, UserId);
            return new JsonResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string newEmail, string confirmationToken, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || string.IsNullOrWhiteSpace(confirmationToken)) return BadRequest();
            await _accountService.ConfirmEmail(newEmail, confirmationToken, userId);

            return Redirect(Constants.AppUrl);
        }

        [Authorize]
        [HttpGet("get-notifications")]
        public JsonResult GetSettingsNotifications()
        {
            var result = _accountService.GetSettingsNotifications(UserId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("delete-notification")]
        public async Task<IActionResult> DeleteSettingsNotification(SettingsNotificationType notificationType)
        {
            await _accountService.DeleteSettingsNotification(notificationType);
            return Ok();
        }

        [Authorize]
        [HttpGet("delete")]
        public async Task<IActionResult> DeleteAccount(string currentPassword)
        {
            var result = await _accountService.DeleteAccount(UserId, currentPassword);
            if (!result) return BadRequest();

            return Ok();
        }
    }
}
