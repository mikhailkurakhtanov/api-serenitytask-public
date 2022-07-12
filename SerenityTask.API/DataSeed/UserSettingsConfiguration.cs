using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.DataSeed
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.ToTable("UserSettings");

            builder.HasData
            (
                new UserSettings
                {
                    Id = 1,
                    TimerSettings = "{ \"mode\": 0, \"type\": 0 }",
                    UserId = Guid.Parse("6073e6de-ed9d-4fbd-b382-a41262ce2ae4")
                }
            );
        }
    }
}