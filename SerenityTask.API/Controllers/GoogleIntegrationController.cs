using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Client.Authentication;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("integration/google")]
    public class GoogleIntegrationController : ControllerBase
    {
        private readonly IGoogleIntegrationService _googleService;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public GoogleIntegrationController(IGoogleIntegrationService googleService)
        {
            _googleService = googleService;
        }

        [Authorize]
        [HttpPost("connect")]
        public async Task<IActionResult> ConnectAccount([FromBody] SocialUser request)
        {
            if (request == null) return BadRequest();

            await _googleService.ConnectAccount(request, UserId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("disconnect")]
        public async Task<IActionResult> DisconnectAccount()
        {
            await _googleService.DisconnectAccount(UserId);
            return Ok();
        }

        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateCredential([FromBody] SocialUser updatedCredential)
        {
            if (updatedCredential == null) return BadRequest();

            _googleService.UpdateCredential(updatedCredential);
            return Ok();
        }
    }
}