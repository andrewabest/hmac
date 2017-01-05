using System;

namespace Hmac
{
    internal static class DateTimeExtensions
    {
        internal static string ToTimeStamp(this DateTime dateTime)
        {
            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = dateTime - epochStart;
            return Convert.ToUInt64(timeSpan.TotalSeconds).ToString();
        }
    }
}