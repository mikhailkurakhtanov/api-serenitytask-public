using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Models.Entities;
using System;

namespace SerenityTask.API.DataSeed
{
    public class UserDetailsConfiguration : IEntityTypeConfiguration<UserDetails>
    {
        public void Configure(EntityTypeBuilder<UserDetails> builder)
        {
            builder.ToTable("UserDetails");

            builder.HasData
            (
                new UserDetails
                {
                    Id = 1,
                    Avatar = "assets/img/authorized/workspace/user.png",
                    UserId = Guid.Parse("6073e6de-ed9d-4fbd-b382-a41262ce2ae4")
                }
            );
        }
    }
}
