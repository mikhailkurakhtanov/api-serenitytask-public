using System;
using System.Globalization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SerenityTask.API.Extensions
{
    public static class StringExtensions
    {
        public static string GetPasswordHash(this string userPassword)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userPassword,
                salt: new byte[128 / 8],
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }

        public static DateTime GetUtcDateTimeFromString(this string dateString)
        {
            return DateTimeOffset.Parse(dateString, null, DateTimeStyles.AssumeUniversal).UtcDateTime;
        }
    }
}
