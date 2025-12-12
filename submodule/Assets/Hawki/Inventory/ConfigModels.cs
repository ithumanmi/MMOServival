
using Hawki.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace Hawki.AllConfig
{
    public partial class ConfigAll
    {
        public List<ItemConfig> ItemConfig;
    }
}

namespace Hawki.Inventory
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

            var list = ItemService.Instance.Parse(purchasePrice);

            return list.FirstOrDefault();
        }
    }
}