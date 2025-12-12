using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawky
{
    public static class Utility
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

        public static List<T> GetRandoms<T>(this List<T> numberList, int amount)
        {
            // Check if x does not exceed the size of the list
            numberList = new List<T>(numberList);
            if (amount > numberList.Count)
            {
                Debug.LogWarning("The number of random numbers to retrieve is greater than the size of the list.");
                return new List<T>(numberList);
            }

            // Use Random.Range to get x random numbers from the list
            List<T> result = new List<T>();
            for (int i = 0; i < amount; i++)
            {
                T randomNum = numberList[Random.Range(0, numberList.Count)];
                result.Add(randomNum);
                numberList.Remove(randomNum); // Ensure no duplicate numbers are selected
            }

            return result;
        }
    }

    public static class StringUtility
    {
        public static List<string> ExportToList(this string data, char split = ';')
        {
            return data.Split(split).Select(s => s.Trim()).ToList();
        }
    }
}