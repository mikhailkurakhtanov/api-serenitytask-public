using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Requests.Session;
using SerenityTask.API.Services;
using Task = System.Threading.Tasks.Task;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([FromBody] Session sessionToCreate)
        {
            await _sessionService.CreateSession(sessionToCreate, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet("join")]
        public async Task<IActionResult> JoinSession(long sessionId)
        {
            await _sessionService.JoinSession(sessionId, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet("leave")]
        public async Task<IActionResult> LeaveSession(long sessionId)
        {
            await _sessionService.LeaveSession(sessionId, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet("cancel")]
        public async Task<IActionResult> CancelSession(long sessionId)
        {
            await _sessionService.CancelSession(sessionId, UserId);
            return Ok();
        }

        [Authorize]
        [HttpPut("set-ready-status")]
        public async Task<IActionResult> SetReadyStatusForJoinedMember(SetReadyStatusForJoinedMemberRequest request)
        {
            if (request == null) return BadRequest();

            await _sessionService.SetReadyStatusForJoinedMember(request);
            return Ok();
        }

        [Authorize]
        [HttpPut("change-tracked-task")]
        public async Task<IActionResult> ChangeSessionMemberTask(ChangeSessionMemberTaskRequest request)
        {
            if (request == null) return BadRequest();

            await _sessionService.ChangeSessionMemberTask(request, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet("start")]
        public async Task<IActionResult> StartSession(long sessionId)
        {
            await _sessionService.StartSession(sessionId);
            return Ok();
        }

        [Authorize]
        [HttpGet("get-all")]
        public JsonResult GetSessions()
        {
            var userSessions = _sessionService.GetSessions(UserId);
            return new JsonResult(userSessions);
        }

        [Authorize]
        [HttpGet("get-friends-and-session-requests")]
        public JsonResult GetFriendsAndSessionRequests()
        {
            var response = _sessionService.GetFriendsAndSessionRequests(UserId);
            return new JsonResult(response);
        }

        [Authorize]
        [HttpPost("send-request")]
        public async Task SendSessionRequest([FromBody] SessionRequest request)
        {
            if (request != null) await _sessionService.SendSessionRequest(request);
        }

        [Authorize]
        [HttpGet("read")]
        public async Task ReadSessionRequest(long sessionRequestId)
        {
            await _sessionService.ReadSessionRequest(sessionRequestId);
        }

        [Authorize]
        [HttpDelete("reject-request")]
        public async Task RejectSessionRequest(long sessionRequestId)
        {
            await _sessionService.RejectSessionRequest(sessionRequestId);
        }
    }
}