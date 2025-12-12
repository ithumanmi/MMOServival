using Hawky.SaveData;
using System.Collections.Generic;

namespace Hawky.Shop
{
    public class ShopData : SaveDataBase<ShopData>
    {
        public Dictionary<string, ShopItemUnitData> data;
        public long nextTimeDailyRefresh;

        public override void Default()
        {
            base.Default();
        }
        public override void OnLoad()
        {
            base.OnLoad();

            if (data == null)
            {
                data = new Dictionary<string, ShopItemUnitData>();
            }
        }
    }

    public class ShopItemUnitData
    {
        public string shopId;
        public int buyAmount;
        public int buyAmountDaily;
    }
}