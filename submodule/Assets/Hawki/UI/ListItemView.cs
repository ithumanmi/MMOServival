using Hawki.Inventory;
using Hawki.ResourcesLoader;
using System.Collections.Generic;

namespace Hawki.UI
{
    public abstract class ListItemView : ResourcesPool
    {
        public abstract void Init(List<ItemData> itemData);
    }
}
