using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.DataSeed
{
    public class PlantTypesConfiguration : IEntityTypeConfiguration<PlantType>
    {
        public void Configure(EntityTypeBuilder<PlantType> builder)
        {
            builder.ToTable("PlantTypes");

            builder.HasData
            (
                new PlantType
                {
                    Id = 1,
                    Name = "Sorana",
                    MaxLeaves = 5
                },
                new PlantType
                {
                    Id = 2,
                    Name = "Selnera",
                    MaxLeaves = 17
                }
            );
        }
    }
}
