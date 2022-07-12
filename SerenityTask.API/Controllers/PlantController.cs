using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SerenityTask.API.Services;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;
using SerenityTask.API.Models.Requests.Plant;

namespace SerenityTask.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        private readonly SerenityTaskDbContext _dbContext;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);

        public PlantController(IPlantService plantService, SerenityTaskDbContext dbContext)
        {
            _plantService = plantService;
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("get-plant-types")]
        public JsonResult GetPlantTypes()
        {
            var result = _plantService.GetPlantTypes();
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("get")]
        public JsonResult Get()
        {
            var result = _plantService.GetPlantByUserId(UserId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("get-plant-history")]
        public JsonResult GetPlantHistory(long plantId)
        {
            var result = _plantService.GetPlantHistory(plantId);
            return new JsonResult(result);
        }

        [Authorize]
        [HttpGet("create")]
        public async Task<JsonResult> Create(string plantName, long plantTypeId)
        {
            var createdTask = await _plantService.CreatePlant(plantName, plantTypeId, UserId);
            return new JsonResult(createdTask);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Plant plantToUpdate)
        {
            if (plantToUpdate == null) return BadRequest();
            await _plantService.UpdatePlant(plantToUpdate);

            return Ok();
        }

        [Authorize]
        [HttpGet("delete")]
        public async Task<IActionResult> Delete(long plantId)
        {
            await _plantService.DeletePlant(plantId);
            return Ok();
        }

        [Authorize]
        [HttpPost("change-experience")]
        public async Task ChangePlantExperience([FromBody] ChangePlantExperienceRequest request)
        {
            await _plantService.ChangePlantExperience(request, UserId);
        }
    }
}
