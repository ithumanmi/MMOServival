using Hawky.Inventory;
using Hawky.ResourcesLoader;
using System.Collections.Generic;

namespace Hawky.UI
{
    public abstract class ListItemView : ResourcesPool
    {
        public abstract void Init(List<ItemData> itemData);
    }
}
