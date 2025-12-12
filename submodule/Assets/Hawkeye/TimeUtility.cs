using System;
using System.Collections.Generic;
using System.Linq;

namespace Hawkeye
{
    public class TimeUtility
    {
        public const long ONE_SECOND = 1000;
        public const long ONE_MINUTE = ONE_SECOND * 60;
        public const long ONE_HOUR = ONE_MINUTE * 60;
        public const long ONE_DAY = ONE_HOUR * 24;
        public static long Now()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
        }

        public static long GetBeginNextDay(int next = 1)
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime nextDay = currentDate.AddDays(next);
            nextDay = nextDay.Date;

            return ((DateTimeOffset)nextDay).ToUnixTimeMilliseconds();
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

        public static string GetTimeRemainingFormatted(long endTimeMillis)
        {
            long currentTimeMillis = Now();
            long remainingMillis = endTimeMillis - currentTimeMillis;

            if (remainingMillis < 0)
            {
                return "00:00";
            }

            int totalSeconds = (int)(remainingMillis / 1000);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            return $"{minutes:00}:{seconds:00}";
        }
    }

    public class Utility
    {
        public static int GetRandomIndex(List<int> weights)
        {
            if (weights == null || weights.Count == 0)
            {
                return -1;
            }

            int totalWeight = weights.Sum();

            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            int selectedIndex = -1;
            int currentWeight = 0;

            for (int i = 0; i < weights.Count; i++)
            {
                currentWeight += weights[i];
                if (randomValue < currentWeight)
                {
                    selectedIndex = i;
                    break;
                }
            }

            return selectedIndex;
        }
    }
}
