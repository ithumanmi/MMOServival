using System.Collections.Generic;

namespace Hawki
{
    public static class ListUtility
    {
        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static List<T> RandomItem<T>(this IList<T> list, int count)
        {
            if (count >= list.Count)
            {
                // Return a copy of the entire list if count is greater or equal to the list size
                return new List<T>(list);
            }

            List<T> randomItems = new List<T>();
            HashSet<int> selectedIndices = new HashSet<int>();

            while (randomItems.Count < count)
            {
                int randomIndex = UnityEngine.Random.Range(0, list.Count);

                // Ensure the index is unique
                if (selectedIndices.Add(randomIndex))
                {
                    randomItems.Add(list[randomIndex]);
                }
            }

            return randomItems;
        }
    }
}
