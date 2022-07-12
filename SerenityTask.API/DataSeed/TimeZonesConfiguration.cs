using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Models.Entities;
using TimeZoneConverter;

namespace SerenityTask.API.DataSeed
{
    public class TimeZonesConfiguration : IEntityTypeConfiguration<TimeZoneType>
    {
        public void Configure(EntityTypeBuilder<TimeZoneType> builder)
        {
            var systemTimeZones = TimeZoneInfo.GetSystemTimeZones();
            var timeZoneTypes = new List<TimeZoneType>();

            for (var index = 0; index < systemTimeZones.Count; index++)
            {
                var timeZoneType = new TimeZoneType
                {
                    Id = index + 1,
                    TimeZoneId = systemTimeZones[index].Id,
                    TimeZoneIdIANA = TZConvert.WindowsToIana(systemTimeZones[index].Id),
                    DisplayName = systemTimeZones[index].DisplayName
                };

                timeZoneTypes.Add(timeZoneType);
            }

            builder.ToTable("TimeZoneTypes");
            builder.HasData(timeZoneTypes);
        }
    }
}