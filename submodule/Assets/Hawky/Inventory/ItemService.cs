using Hawky.AllConfig;
using Hawky.Config;
using Hawky.EventObserver;
using Hawky.Localization;
using Hawky.SaveData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawky.Inventory
{
    public class AddItemsRequest
    {
        public List<AddItemRequest> items;

        public AddItemsRequest()
        {
            items = new List<AddItemRequest>();
        }
    }

    public class AddItemsResponse
    {
        public List<AddItemResponse> items;

        public AddItemsResponse()
        {
            items = new List<AddItemResponse>();
        }
    }

    public class AddItemRequest
    {
        public string itemId;
        public float amount;
        public string position = AddItemPosition.DEFAULT;
    }

    public class AddItemResponse
    {
        public string itemId;
        public float oldAmount;
        public float newAmount;
        public string position;
    }

    public class UseItemsRequest
    {
        public List<UseItemRequest> items;
    }

    public class UseItemsResponse
    {
        public List<UseItemResponse> items;
    }

    public class UseItemRequest
    {
        public string itemId;
        public float amount;
        public string position = UseItemPosition.DEFAULT;
        public string newReward;
    }

    public class UseItemResponse
    {
        public float oldAmount;
        public float newAmount;
        public string position;
    }
    public partial class ItemService : RuntimeSingleton<ItemService>
    {
        public AddItemResponse AddItem(AddItemRequest request)
        {
            var inventory = SaveDataManager.Ins.GetData<InventoryData>();

            var response = new AddItemResponse();
            response.position = request.position;

            if (inventory.items.TryGetValue(request.itemId, out var rs))
            {
                response.oldAmount = rs.number;
                rs.number = rs.number + request.amount;
            }
            else
            {
                response.oldAmount = 0;
                rs = new ItemModel
                {
                    itemID = request.itemId,
                    number = request.amount,
                };

                inventory.items.Add(request.itemId, rs);
            }
            response.newAmount = rs.number;

            inventory.Save();

            EventObs.Ins.ExcuteEvent(EventName.EARN_VIRTUAL_CURRENCY, new EarnVirtualCurrencyEvent(request.itemId, request.amount, rs.number, request.position));

            return response;
        }

        public UseItemsResponse UseItems(UseItemsRequest request)
        {
            var response = new UseItemsResponse();

            foreach (var item in request.items)
            {
                response.items.Add(UseItem(item));
            }

            return response;
        }

        public UseItemResponse UseItem(UseItemRequest request)
        {
            var inventory = SaveDataManager.Ins.GetData<InventoryData>();

            var response = new UseItemResponse();
            response.position = request.position;

            if (inventory.items.TryGetValue(request.itemId, out var rs))
            {
                response.oldAmount = rs.number;
                rs.number = rs.number - request.amount;
            }
            else
            {
                response.oldAmount = 0;
                rs = new ItemModel
                {
                    itemID = request.itemId,
                    number = -request.amount,
                };

                inventory.items.Add(request.itemId, rs);
            }
            response.newAmount = rs.number;

            inventory.Save();

            EventObs.Ins.ExcuteEvent(EventName.SPEND_VIRTUAL_CURRENCY, new SpendVirtualCurrencyEvent(request.itemId, (long)request.amount, rs.number, request.position, request.newReward));

            return response;
        }

        public AddItemsResponse AddItems(AddItemsRequest request)
        {
            var response = new AddItemsResponse();

            foreach (var item in request.items)
            {
                response.items.Add(AddItem(item));
            }

            return response;
        }

        public bool EnoughItem(string itemId, float itemAmount)
        {
            var inventory = SaveDataManager.Ins.GetData<InventoryData>();

            if (inventory.items.TryGetValue(itemId, out var rs))
            {
                return rs.number >= itemAmount;
            }

            return false;
        }

        public float GetAmount(string itemId)
        {
            var inventory = SaveDataManager.Ins.GetData<InventoryData>();

            if (inventory.items.TryGetValue(itemId, out var rs))
            {
                return rs.number;
            }

            return 0;
        }

        public List<ItemData> MinusFromInventory(List<ItemData> rewards)
        {
            var result = new List<ItemData>();

            foreach (var reward in rewards)
            {
                var newData = reward.Clone();

                newData.itemAmount = GetAmount(reward.itemId) - reward.itemAmount;

                result.Add(newData);
            }

            return result;
        }

        public static List<ItemData> Parse(string data)
        {
            List<ItemData> rewardList = new List<ItemData>();

            if (!string.IsNullOrEmpty(data))
            {
                string[] rewardEntries = data.Split(';');

                foreach (string entry in rewardEntries)
                {
                    string[] parts = entry.Split('-');

                    if (parts.Length == 3)
                    {
                        ItemData rewardData = new ItemData
                        {
                            itemType = parts[0],
                            itemId = parts[1],
                            itemAmount = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture)
                        };

                        rewardList.Add(rewardData);
                    }
                    else if (parts.Length == 2)
                    {
                        ItemData rewardData = new ItemData
                        {
                            itemType = "",
                            itemId = parts[0],
                            itemAmount = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture)
                        };

                        rewardList.Add(rewardData);
                    }
                    else
                    {
                        // Handle the case where the format is not as expected
                        Debug.LogWarning("Invalid reward format: " + entry);
                    }
                }
            }

            return rewardList.FindAll(x => x.itemAmount > 0);
        }

        public static List<ItemData> Merge(List<ItemData> rewards)
        {
            var result = new List<ItemData>();

            foreach (var reward in rewards)
            {
                var rew = result.Find(x => x.itemType == reward.itemType && x.itemId == reward.itemId);

                if (rew == null)
                {
                    rew = reward.Clone();
                    result.Add(rew);
                    continue;
                }

                rew.itemAmount += reward.itemAmount;
            }

            return result.FindAll(x => x.itemAmount > 0);
        }

        public static List<ItemData> Multiple(List<ItemData> rewardsNeedToClaim, int mulilple)
        {
            var result = new List<ItemData>();

            foreach (var reward in rewardsNeedToClaim)
            {
                var newData = reward.Clone();

                newData.itemAmount = reward.itemAmount * mulilple;

                result.Add(newData);
            }

            return result.FindAll(x => x.itemAmount > 0);
        }

        public string ItemName(string itemId)
        {
            return Hawky.Localization.LocalizationManager.Ins.GetString($"STRING_NAME_{itemId.ToUpper()}");
        }

        public string ExportAllCurrency()
        {
            var inventory = SaveDataManager.Ins.GetData<InventoryData>();
            var configAll = ConfigManager.Ins.GetData<ConfigAll>();

            var itemConfigRef = configAll.ItemConfig.ToDictionary(x => x.itemId);

            List<string> result = new List<string>();

            foreach (var item in inventory.items)
            {
                var itemId = item.Key;

                if (itemConfigRef.TryGetValue(itemId, out var itemConfig))
                {
                    if (itemConfig.itemType == ItemType.CUR)
                    {
                        result.Add(ToString(item.Value.itemID, item.Value.number));
                    }
                }
            }

            return string.Join(';', result.ToArray());
        }

        public static List<ItemData> Clone(List<ItemData> items)
        {
            return items.Select(x => x.Clone()).ToList();
        }

        public static ItemData Clone(ItemData item)
        {
            return item.Clone();
        }

        public static string ToString(string itemId, float itemAmount)
        {
            return $"{itemId}-{(long)itemAmount}";
        }
    }
}
