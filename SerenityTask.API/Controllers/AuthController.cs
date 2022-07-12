using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Client.Authentication;
using IAuthenticationService = SerenityTask.API.Services.IAuthenticationService;
using SerenityTask.API.Models.Requests.Authentication;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IEmailService _emailService;

        private readonly IAuthenticationService _authenticationService;

        public AuthController(IEmailService emailService, IAuthenticationService authenticationService)
        {
            _emailService = emailService;
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<User> Register([FromBody] Register formData)
        {
            return await _authenticationService.RegisterUserAsync(formData);
        }

        [HttpPost("register-with-google")]
        public async Task<JsonResult> RegisterWithGoogle([FromBody] SocialUser request)
        {
            if (request == null) return null;

            var authorizationToken = await _authenticationService.RegisterWithGoogleAsync(request);
            return new JsonResult(authorizationToken);
        }

        [HttpPost("login-with-google")]
        public async Task<JsonResult> LoginWithGoogle([FromBody] SocialUser request)
        {
            if (request == null) return null;

            var authorizationToken = await _authenticationService.LoginWithGoogleAsync(request);
            return new JsonResult(authorizationToken);
        }

        [HttpPost("login")]
        public JsonResult Login([FromBody] LoginForm formData)
        {
            var loginResponse = _authenticationService.LoginUser(formData);
            return new JsonResult(loginResponse);
        }

        [HttpPost("validate-admin")]
        public JsonResult ValidateAdminData([FromBody] LoginForm formData)
        {
            var loginResponse = _authenticationService.ValidateAdminData(formData);
            return new JsonResult(loginResponse);
        }

        [Authorize]
        [HttpGet("check-authorized")]
        public async Task<bool> CheckAuthorizedUserOnExist()
        {
            var currentUserId = Guid.Parse(User.Claims
                .Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

            return await _authenticationService.CheckAuthorizedUserOnExist(currentUserId);
        }

        [HttpPost("recover-password")]
        public async Task<IActionResult> SendRecoverPasswordMessage(ChangeAccountPasswordRequest request)
        {
            if (request == null) return BadRequest();

            await _emailService.SendPasswordRecoveryConfirmationEmail(request);
            return Ok();
        }
    }
}
