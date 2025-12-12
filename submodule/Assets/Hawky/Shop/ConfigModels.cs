
using Hawky.Shop;
using System.Collections.Generic;

namespace Hawky.AllConfig
{
    public partial class ConfigAll
    {
        public List<ShopConfig> ShopConfig;
    }
}

namespace Hawky.Shop
{
    public partial class ShopConfig
    {
        public string shopId;
        public string itemType;
        public string itemGroup;
        public string price;
        public int dailyLimit = int.MaxValue;
        public string rewards;
        public int priority;
    }
}