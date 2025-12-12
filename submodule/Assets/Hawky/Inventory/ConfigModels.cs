
using Hawky.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace Hawky.AllConfig
{
    public partial class ConfigAll
    {
        public List<ItemConfig> ItemConfig;
    }
}

namespace Hawky.Inventory
{
    public partial class ItemConfig
    {
        public string itemId;
        public string itemType;
        public string purchasePrice;

        public ItemData GetPurchasePrice()
        {
            if (string.IsNullOrEmpty(purchasePrice))
            {
                return null;
            }

            var list = ItemService.Parse(purchasePrice);

            return list.FirstOrDefault();
        }
    }
}