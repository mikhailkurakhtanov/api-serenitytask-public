using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("user-details")]
    public class UserDetailsController : ControllerBase
    {
        private readonly IUserDetailsService _profileService;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public UserDetailsController(IUserDetailsService profileService)
        {
            _profileService = profileService;
        }

        [Authorize]
        [HttpGet("get-timezones")]
        public JsonResult GetTimeZoneTypes()
        {
            var timeZoneTypes = _profileService.GetTimeZoneTypes();
            return new JsonResult(timeZoneTypes);
        }

        [Authorize]
        [HttpPut("update-avatar")]
        public async Task<IActionResult> UpdateAvatar()
        {
            var formCollection = await Request.ReadFormAsync();
            var avatarData = formCollection.Files[0];

            if (avatarData.Length == 0) return BadRequest();

            await _profileService.UpdateAvatar(avatarData, UserId);
            Response.Headers["Access-Control-Allow-Origin"] = Request.Host.Value;
            return Ok();
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfileData([FromBody] UserDetails changedUserDetails)
        {
            if (changedUserDetails == null) return BadRequest();

            await _profileService.UpdateUserDetails(changedUserDetails);
            Response.Headers["Access-Control-Allow-Origin"] = Request.Host.Value;
            return Ok();
        }
    }
}
