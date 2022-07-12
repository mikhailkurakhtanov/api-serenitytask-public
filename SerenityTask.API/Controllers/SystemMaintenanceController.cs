using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Services;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("system-maintenance")]
    public class SystemMaintenanceController : ControllerBase
    {
        private readonly ISystemMaintenanceService _systemMaintenanceService;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public SystemMaintenanceController(ISystemMaintenanceService systemMaintenanceService)
        {
            _systemMaintenanceService = systemMaintenanceService;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProblemReport([FromBody] ProblemReport newProblemReport)
        {
            if (newProblemReport == null) return BadRequest();

            await _systemMaintenanceService.CreateProblemReport(newProblemReport, UserId);
            return Ok();
        }

        [Authorize]
        [HttpGet("get-changelog")]
        public JsonResult GetChangelog()
        {
            var result = _systemMaintenanceService.GetChangelog();
            return new JsonResult(result);
        }
    }
}