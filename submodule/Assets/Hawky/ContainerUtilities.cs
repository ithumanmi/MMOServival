using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawky
{
    public class ContainerUtilities
    {
        public static void UpdateContainer<VIEW, DATA>(List<VIEW> pool, VIEW prefab, Transform container, List<DATA> data, Action<VIEW, DATA> onUpdate, Predicate<DATA> condition = null) where VIEW : Component
        {
            UpdateContainer(pool, prefab, container, data, (item, data, index) =>
            {
                onUpdate?.Invoke(item, data);

            }, condition);
        }

        public static void UpdateContainer<VIEW, DATA>(List<VIEW> pool, VIEW prefab, Transform container, DATA[] data, Action<VIEW, DATA, int> onUpdate, Predicate<DATA> condition = null) where VIEW : Component
        {
            var listData = data.ToList();
            UpdateContainer(pool, prefab, container, listData, (item, data, index) =>
            {
                onUpdate?.Invoke(item, data, index);

            }, condition);
        }

        public static void UpdateContainer<VIEW, DATA>(List<VIEW> pool, VIEW prefab, Transform container, DATA[] data, Action<VIEW, DATA> onUpdate, Predicate<DATA> condition = null) where VIEW : Component
        {
            var listData = data.ToList();
            UpdateContainer(pool, prefab, container, listData, (item, data, index) =>
            {
                onUpdate?.Invoke(item, data);

            }, condition);
        }

        public static void UpdateContainer<VIEW>(List<VIEW> pool, VIEW prefab, Transform container, int amount, Action<VIEW, int> onUpdate) where VIEW : Component
        {
            var data = Enumerable.Range(0, amount).ToList();
            UpdateContainer(pool, prefab, container, data, (item, data, index) =>
            {
                onUpdate?.Invoke(item, index);
            }, null);
        }

        public static void UpdateContainer<VIEW, DATA>(List<VIEW> pool, VIEW prefab, Transform container, List<DATA> data, Action<VIEW, DATA, int> onUpdate, Predicate<DATA> condition = null) where VIEW : Component
        {
            pool.ForEach(x => x.gameObject.SetActive(false));
            prefab.gameObject.SetActive(false);
            List<DATA> fixedData = new List<DATA>();
            if (condition == null)
            {
                fixedData.AddRange(data);
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (condition.Invoke(data[i]))
                    {
                        fixedData.Add(data[i]);
                    }
                }
            }

            if (fixedData.Count > pool.Count)
            {
                int countInPool = pool.Count;
                for (int i = 0; i < fixedData.Count - countInPool; i++)
                {
                    pool.Add(GameObject.Instantiate(prefab, container));
                }
            }

            for (int i = 0; i < fixedData.Count; i++)
            {
                pool[i].transform.SetAsLastSibling();
                pool[i].gameObject.SetActive(true);
                onUpdate?.Invoke(pool[i], fixedData[i], i);
            }

            for (int i = fixedData.Count; i < pool.Count; i++)
            {
                pool[i].gameObject.SetActive(false);
            }
        }
    }

}
