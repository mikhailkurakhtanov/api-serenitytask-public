using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SerenityTask.API.Models.Entities;
using Task = System.Threading.Tasks.Task;
using SerenityTask.API.Models.Requests.Plant;
using SerenityTask.API.Models.Client.Workspace;

namespace SerenityTask.API.Services
{
    public interface IPlantService
    {
        ICollection<PlantType> GetPlantTypes();

        Plant GetPlantByUserId(Guid currentUserId);

        List<PlantHistoryView> GetPlantHistory(long plantId);

        Task<Plant> CreatePlant(string plantName, long plantTypeId, Guid currentUserId);

        Task UpdatePlant(Plant plantToUpdate);

        Task DeletePlant(long plantId);

        Task ChangePlantExperience(ChangePlantExperienceRequest request, Guid currentUserId);
    }
}
