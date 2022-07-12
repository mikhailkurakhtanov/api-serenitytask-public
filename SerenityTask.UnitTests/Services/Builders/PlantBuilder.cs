using SerenityTask.API.Models.Entities;

namespace SerenityTask.UnitTests.Services.Builders
{
    internal class PlantBuilder
    {
        private long _id;

        private string _name;

        private PlantType _plantType;

        private User _user;

        internal PlantBuilder()
        {
            _id = 1;
            _name = "DefaultName";
            _plantType = new PlantTypeBuilder().Build();
            _user = new UserBuilder().Build();
        }

        internal Plant Build()
        {
            return new Plant
            {
                Id = _id,
                Name = _name,
                PlantType = _plantType,
                User = _user
            };
        }

        internal PlantBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        internal PlantBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal PlantBuilder WithPlantType(PlantType plantType)
        {
            _plantType = plantType;
            return this;
        }

        internal PlantBuilder WithUser(User user)
        {
            _user = user;
            return this;
        }
    }
}
