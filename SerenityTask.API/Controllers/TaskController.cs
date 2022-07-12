using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using UserTask = SerenityTask.API.Models.Entities.Task;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        private Guid UserId => Guid.Parse(User.Claims
            .Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet("get-all")]
        public JsonResult GetTasks()
        {
            var result = _taskService.GetTasks(UserId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("get")]
        public JsonResult GetTask(long taskId)
        {
            var result = _taskService.GetTask(taskId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task CreateTask([FromBody] UserTask taskToCreate)
        {
            await _taskService.CreateTask(taskToCreate, UserId);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task UpdateTask([FromBody] UserTask changedTask)
        {
            await _taskService.UpdateTask(changedTask);
        }

        [Authorize]
        [HttpGet("delete")]
        public async Task<JsonResult> DeleteTask(long taskId)
        {
            var result = await _taskService.DeleteTask(taskId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("complete")]
        public async Task CompleteTask(long taskId)
        {
            await _taskService.CompleteTask(taskId, UserId);
        }

        [Authorize]
        [HttpGet("find-by-name")]
        public JsonResult FindTasksByName(string query)
        {
            var tasksByName = _taskService.FindTasksByName(query, UserId);
            return new JsonResult(tasksByName);
        }
    }
}
