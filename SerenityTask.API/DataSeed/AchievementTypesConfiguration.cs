using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Components;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.DataSeed
{
    public class AchievementTypesConfiguration : IEntityTypeConfiguration<AchievementType>
    {
        public void Configure(EntityTypeBuilder<AchievementType> builder)
        {
            builder.ToTable("AchievementTypes");

            builder.HasData
            (
                new AchievementType
                {
                    Id = 1,
                    Name = "Gardener",
                    Description = "Grow your first plant without loosing level",
                    Rate = AchievementTypeRate.Bronze,
                    Goal = 1,
                    Icon = Constants.AchievementsUrl + "gardener/tier_1.png"
                },
                new AchievementType
                {
                    Id = 2,
                    Name = "Gardener",
                    Description = "Grow your third plant without loosing level",
                    Rate = AchievementTypeRate.Silver,
                    Goal = 3,
                    Icon = Constants.AchievementsUrl + "gardener/tier_2.png"
                },
                new AchievementType
                {
                    Id = 3,
                    Name = "Gardener",
                    Description = "Grow your fifth plant without lossing level",
                    Rate = AchievementTypeRate.Gold,
                    Goal = 5,
                    Icon = Constants.AchievementsUrl + "gardener/tier_3.png"
                },
                new AchievementType
                {
                    Id = 4,
                    Name = "Serenity Soul",
                    Description = "Complete 10 tasks before deadline",
                    Rate = AchievementTypeRate.Bronze,
                    Goal = 10,
                    Icon = Constants.AchievementsUrl + "serenity_soul/tier_1.png"
                },
                new AchievementType
                {
                    Id = 5,
                    Name = "Serenity Soul",
                    Description = "Complete 25 tasks before deadline",
                    Rate = AchievementTypeRate.Silver,
                    Goal = 25,
                    Icon = Constants.AchievementsUrl + "serenity_soul/tier_2.png"
                },
                new AchievementType
                {
                    Id = 6,
                    Name = "Serenity Soul",
                    Description = "Complete 50 tasks before deadline",
                    Rate = AchievementTypeRate.Gold,
                    Goal = 50,
                    Icon = Constants.AchievementsUrl + "serenity_soul/tier_3.png"
                },

                // Ordinary achievements
                new AchievementType
                {
                    Id = 7,
                    Description = "Start growing your first plant",
                    Name = "First Step",
                    Rate = AchievementTypeRate.Ordinary,
                    Goal = 1,
                    Icon = Constants.AchievementsUrl + "ordinary/first_step.png"
                }
            );
        }
    }
}