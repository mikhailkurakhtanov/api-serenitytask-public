using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Models.Server;
using SerenityTask.API.Models.Client;
using SerenityTask.API.Services.Implementations;
using SerenityTask.UnitTests.Services.Builders;

namespace SerenityTask.UnitTests.Services
{
    [TestFixture]
    internal class PlantTests
    {
        private readonly PlantService _plantService;

        private readonly SerenityTaskDbContext _dbContext;

        private readonly List<PlantType> _plantTypes = new();

        private readonly List<User> _users = new();

        private readonly List<Plant> _plants = new();

        public PlantTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SerenityTaskDbContext>()
                .UseInMemoryDatabase(databaseName: "PlantTestsData").Options;

            _dbContext = new SerenityTaskDbContext(dbContextOptions);
            _plantService = Substitute.For<PlantService>(_dbContext);

            _users = GetUsersForTests();
            _dbContext.Users.AddRange(_users);

            _plantTypes = GetPlantTypesForTests();
            _dbContext.PlantTypes.AddRange(_plantTypes);

            _plants = GetPlantsForTests();
            _dbContext.Plants.AddRange(_plants);

            _dbContext.SaveChanges();
        }

        private static List<PlantType> GetPlantTypesForTests()
        {
            var firstPlantType = new PlantTypeBuilder().Build();
            var secondPlantType = new PlantTypeBuilder().WithId(2).WithName("Neutral Name").WithMaxLeaves(15).Build();

            return new List<PlantType>() { firstPlantType, secondPlantType };
        }

        private static List<User> GetUsersForTests()
        {
            var firstUser = new UserBuilder().WithId(Guid.Parse("3e25efe5-30a9-4516-99bd-6ac7b30ebbe0")).Build();
            var secondUser = new UserBuilder().WithId(Guid.Parse("dab8833a-da9f-476c-9135-3d8774a9da55")).Build();

            return new List<User>() { firstUser, secondUser };
        }

        private List<Plant> GetPlantsForTests()
        {
            var firstPlant = new PlantBuilder().WithId(1).WithUser(_users[0]).WithPlantType(_plantTypes[0]).Build();
            var secondPlant = new PlantBuilder().WithId(2).WithUser(_users[1]).WithPlantType(_plantTypes[1]).Build();

            return new List<Plant> { firstPlant, secondPlant };
        }

        [Test]
        public void GetPlantTypes_ReturnPlantTypes()
        {
            // Act
            var plantTypes = _plantService.GetPlantTypes();

            // Assert
            Assert.AreEqual(plantTypes, _plantTypes);
        }

        [Test]
        public void GetPlantByUserId_ReturnsPlant_IfPlantExists()
        {
            // Arrange
            var userId = _plants[0].User.Id;

            // Act
            var plantByUserId = _plantService.GetPlantByUserId(userId);

            // Assert
            Assert.IsNotNull(plantByUserId);
        }

        [Test]
        public void GetPlantByUserId_ReturnsNull_IfPlantDoesNotExist()
        {
            // Arrange
            var userId = Guid.Parse("6e19665b-d659-4786-9dff-f942c2a3e70b");

            // Act
            var result = _plantService.GetPlantByUserId(userId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void CreatePlant_ReturnsPlant_IfPlantIsNotNull()
        {
            // Arrange
            var newPlant = new PlantBuilder().WithId(3)
                .WithUser(_users[1]).Build();

            // Act
            var result = _plantService.CreatePlant(newPlant.Name, newPlant.PlantType.Id, newPlant.User.Id).Result;

            // Assert
            Assert.IsNotNull(result);
        }

        // [Test]
        // public void UpdatePlant_ReturnsNothing_IfUpdatingWasSuccessful()
        // {
        //     // Arrange
        //     var plantToUpdate = _plants[0];
        //     plantToUpdate.Name = "AnotherName";

        //     // Act
        //     _plantService.UpdatePlant(plantToUpdate);

        //     // Assert
        //     Assert.DoesNotThrow(() => _plantService.UpdatePlant(plantToUpdate));
        //     Assert.AreEqual(plantToUpdate, _plants[0]);
        // }

        // [Test]
        // public void DeletePlant_ReturnsNothing_IfPlantWasSuccessfullyDeleted()
        // {
        //     // Assert
        //     Assert.DoesNotThrow(() => _plantService.DeletePlant(_plants[0].Id));
        // }

        // [TestCase(false, true)]
        // [TestCase(true, false)]
        // public void ChangeExperience_ThrowsNullReferenceException(bool isExperienceForTaskNotNull, bool isPlantExist)
        // {
        //     // Arrange
        //     var task = new TaskBuilder().Build();

        //     var experienceInfo = new ExperienceInfo
        //     {
        //         Experience = 0,
        //         ExperienceReason = "Reason",
        //         TaskId = task.Id
        //     };

        //     // Assert
        //     // Assert.That(() => _plantService.RisePlantExperience(isExperienceForTaskNotNull ? experienceInfo : null),
        //     //         Throws.TypeOf<NullReferenceException>());
        // }

        [Test]
        public void ChangeExperience_ReturnsPlantExperienceResult()
        {
            // Arrange
            var task = new TaskBuilder().Build();

            // var experienceForTask = new ExperienceInfo
            // {
            //     TaskId = task.Id,
            //     TaskName = task.Name,
            //     ExperienceForTask = 10,
            //     PlantId = _plants[0].Id
            // };

            // // Act
            // var result = _plantService.ChangeExperience(experienceForTask).Result;

            // // Assert
            // Assert.IsNotNull(result);
        }
    }
}
