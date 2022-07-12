using SerenityTask.API.Models.Entities;

namespace SerenityTask.UnitTests.Services.Builders
{
    internal class PlantTypeBuilder
    {
        private long _id;

        private string _name;

        private int _maxLeaves;

        internal PlantTypeBuilder()
        {
            _id = 1;
            _name = "DefaultName";
            _maxLeaves = 10;
        }

        internal PlantType Build()
        {
            return new PlantType
            {
                Id = _id,
                Name = _name,
                MaxLeaves = _maxLeaves
            };
        }

        internal PlantTypeBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        internal PlantTypeBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        internal PlantTypeBuilder WithMaxLeaves(int maxLeaves)
        {
            _maxLeaves = maxLeaves;
            return this;
        }
    }
}
