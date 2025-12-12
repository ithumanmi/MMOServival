using Hawky.Ads;
using Hawky.SaveData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawky.Shop
{
    public class ShopUtility
    {
        public static void Spawn(List<ShopConfig> shopConfigs, Transform _root, ref List<ShopItem> _currentItems)
        {
            if (_currentItems != null)
            {
                foreach (var item in _currentItems)
                {
                    ShopItemResourcesLoader.Ins.FreeResources(item);
                }
            }

            shopConfigs = shopConfigs.FindAll(x => !string.IsNullOrEmpty(x.itemType)).OrderBy(x => x.priority).ToList();

            _currentItems = new List<ShopItem>();

            while (shopConfigs.Count > 0)
            {
                var config = shopConfigs.First();

                if (!string.IsNullOrEmpty(config.itemGroup))
                {
                    var itemGroup = config.itemGroup;
                    var allConfigs = shopConfigs.FindAll(x => x.itemGroup == itemGroup && x.priority == config.priority);

                    foreach (var scf in allConfigs)
                    {
                        shopConfigs.Remove(scf);
                    }

                    var groupItem = ShopGroupItemResourcesLoader.Ins.LoadResources(itemGroup, default, _root);

                    if (groupItem == null)
                    {
                        Debug.LogError($"Kh�ng c� Shop Group Item with Name = {config.itemType}");
                        continue;
                    }

                    groupItem.Init(allConfigs, ref _currentItems);
                }
                else
                {
                    shopConfigs.RemoveAt(0);

                    var item = Spawn(config, _root);

                    if (item == null)
                    {
                        continue;
                    }

                    _currentItems.Add(item);
                }

            }
        }

        public static ShopItem Spawn(ShopConfig config, Transform _root)
        {
            if (config.shopId == ShopId.NO_ADS)
            {
                var adsData = SaveDataManager.Ins.GetData<AdsData>();
                if (adsData.noAds)
                {
                    return null;
                }
            }

            var item = ShopItemResourcesLoader.Ins.LoadResources(config.itemType, default, _root);

            if (item == null)
            {
                Debug.LogError($"Kh�ng c� Shop Item with Name = {config.itemType}");
                return null;
            }
            item.Init(config);

            return item;
        }
    }
}