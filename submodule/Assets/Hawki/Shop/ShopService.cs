using Hawki.AllConfig;
using Hawki.Config;
using Hawki.Inventory;
using Hawki.SaveData;
using System.Collections.Generic;
using System.Linq;

namespace Hawki.Shop
{
    public class BuyShopRequest
    {
        public string shopId;
    }

    public class BuyShopResponse
    {
        public string shopId;
        public List<ItemData> rewards;
    }

    public class ShopService : RuntimeSingleton<ShopService>, IUpdateBehaviour
    {
        public ShopConfig GetShopConfigById(string shopId)
        {
            var config = ConfigManager.Instance.GetData<ConfigAll>().ShopConfig;

            return config.Find(x => x.shopId == shopId);
        }
        public ShopItemUnitData GetShopDataById(string shopId)
        {
            var shopData = SaveDataManager.Instance.GetData<ShopData>();

            if (shopData.data.ContainsKey(shopId) == false)
            {
                shopData.data.Add(shopId, new ShopItemUnitData()
                {
                    shopId = shopId,
                });

                shopData.Save();
            }

            return shopData.data[shopId];
        }

        public BuyShopResponse BuyShop(BuyShopRequest request)
        {
            var response = new BuyShopResponse();
            response.shopId= request.shopId;

            var shopId = request.shopId;

            var shopData = SaveDataManager.Instance.GetData<ShopData>();

            var _currentConfig = GetShopConfigById(shopId);

            if (shopData.data.ContainsKey(shopId) == false)
            {
                shopData.data.Add(shopId, new ShopItemUnitData()
                {
                    shopId = shopId,
                });
            }

            var shopItemData = shopData.data[shopId];
            shopItemData.buyAmount++;
            shopItemData.buyAmountDaily++;

            var rewards = ItemService.Instance.Parse(_currentConfig.rewards);

            ItemService.Instance.AddItems(new AddItemsRequest
            {
                items = rewards.Select(x => new AddItemRequest
                {
                    itemId = x.itemId,
                    amount = x.itemAmount,
                    position = $"{AddItemPosition.Shop}-{request.shopId}",
                }).ToList(),
            });

            shopData.Save();

            response.rewards = rewards;
            return response;
        }

        public void RefreshDailyShop()
        {
            var shopData = SaveDataManager.Instance.GetData<ShopData>();

            if (shopData.nextTimeDailyRefresh == 0)
            {
                shopData.nextTimeDailyRefresh = TimeUtility.GetBeginNextDay();
                return;
            }

            var current = TimeUtility.Now();

            if (current > shopData.nextTimeDailyRefresh)
            {
                shopData.nextTimeDailyRefresh = TimeUtility.GetBeginNextDay();

                foreach (var unit in shopData.data)
                {
                    unit.Value.buyAmountDaily = 0;
                }

                shopData.Save();
            }

        }

        public void Update()
        {
            RefreshDailyShop();
        }
    }
}
