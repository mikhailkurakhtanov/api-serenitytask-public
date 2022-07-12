using System;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Extensions
{
    public static class UserExtension
    {
        public static DateTime ConvertDateTimeToUserDateTime(this User currentUser, DateTime dateToConvert)
        {
            var currentUserTimeZoneInfo = TimeZoneInfo
                    .FindSystemTimeZoneById(currentUser.UserDetails.TimeZone.TimeZoneId);

            return TimeZoneInfo.ConvertTime(dateToConvert, currentUserTimeZoneInfo);
        }
    }
}