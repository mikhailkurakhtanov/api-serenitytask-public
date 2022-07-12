using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Extensions;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.DataSeed
{
    public class UsersConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasData
            (
                new User
                {
                    Id = Guid.Parse("6073e6de-ed9d-4fbd-b382-a41262ce2ae4"),
                    Name = "Mikhail",
                    Email = "mikhail@csfullstack.com",
                    Username = "mikhail@csfullstack.com",
                    PasswordHash = "J5wryYsr8wEeD7Zd".GetPasswordHash(),
                    IsEmailConfirmed = true,
                    RoleId = 1
                }
            );
        }
    }
}
