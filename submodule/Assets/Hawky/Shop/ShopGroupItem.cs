using Hawky.ResourcesLoader;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.Shop
{
    public class ShopGroupItem : ResourcesPool
    {
        [SerializeField] private RectTransform _root;

        public void Init(List<ShopConfig> shopConfigs, ref List<ShopItem> shopItems)
        {
            foreach (var shopConfig in shopConfigs)
            {
                var item = ShopUtility.Spawn(shopConfig, _root);

                if (item == null)
                {
                    continue;
                }

                shopItems.Add(item);
            }
        }
    }
}
