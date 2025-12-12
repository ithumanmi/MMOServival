using System;

namespace Hawky
{
    public static class TimeUtility
    {
        public const long ONE_SECOND = 1000;
        public const long ONE_MINUTE = ONE_SECOND * 60;
        public const long ONE_HOUR = ONE_MINUTE * 60;
        public const long ONE_DAY = ONE_HOUR * 24;
        public static long Now()
        {
            return ConvertToUnixTime(DateTime.UtcNow);
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.UtcNow;
        }

        public static long ConvertToUnixTime(DateTime dateTime)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epochStart).TotalMilliseconds;
        }

        public static long GetBeginNextDay(int next = 1)
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime nextDay = currentDate.AddDays(next);
            nextDay = nextDay.Date;

            return ((DateTimeOffset)nextDay).ToUnixTimeMilliseconds();
        }

        public static long GetBeginNextMonth(int offset = 1)
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime nextMonth = currentDate.AddMonths(offset);
            nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            return ((DateTimeOffset)nextMonth).ToUnixTimeMilliseconds();
        }

        public static long GetBeginDayFromUnixTime(long unixTime)
        {
            return unixTime - unixTime % ONE_DAY;
        }

        public static bool BeforeNow(long time)
        {
            long currentTimeMillis = Now();
            return time < currentTimeMillis;
        }

        public static bool AfterNow(long time)
        {
            long currentTimeMillis = Now();
            return time > currentTimeMillis;
        }

        public static string OffsetNowFormat1(long miliseconds)
        {
            long currentTimeMillis = Now();
            long remainingMillis = miliseconds - currentTimeMillis;
            return TimeFormat1((float)remainingMillis / 1000);
        }

        public static string TimeFormat1(float seconds)
        {
            if (seconds <= 0)
            {
                return "00:00";
            }

            int totalSeconds = (int)(seconds);
            int minute = totalSeconds / 60;
            int second = totalSeconds % 60;

            return $"{minute:00}:{second:00}";
        }

        public static DateTime ToDateTime(this long miliseconds)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return epoch.AddMilliseconds(miliseconds);
        }

        public static DateTime ToDateTime(this int seconds)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return epoch.AddSeconds(seconds);
        }

        public static string TimeFormat2(long miliseconds)
        {
            if (miliseconds <= 0)
            {
                return "00:00";
            }

            var timeSpan = TimeSpan.FromMilliseconds(miliseconds);

            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
    }
}
