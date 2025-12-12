using Hawky.SaveData;
using System.Collections.Generic;

namespace Hawky.Inventory
{
    public class ItemData
    {
        public string itemType;
        public string itemId;
        public float itemAmount;

        public ItemData Clone()
        {
            ItemData data = new ItemData();
            data.itemType = itemType;
            data.itemId = itemId;
            data.itemAmount = itemAmount;
            return data;
        }
    }

    public class ItemModel
    {
        public string itemID;
        public float number;
    }

    public partial class InventoryData : SaveDataBase<InventoryData>
    {
        public Dictionary<string, ItemModel> items;

        public override void Default()
        {
            base.Default();

            items = new Dictionary<string, ItemModel>();
        }

        public override void OnLoad()
        {
            base.OnLoad();

            if (items == null)
            {
                items = new Dictionary<string, ItemModel>();
            }
        }
    }
}
