using Hawky.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.UI
{
    public class BasicListItemView : ListItemView
    {
        [SerializeField] private ItemView _prefab;

        private List<ItemView> pool = new List<ItemView>();

        public override void Init(List<ItemData> itemData)
        {
            ContainerUtilities.UpdateContainer(pool, _prefab, _prefab.transform.parent, itemData, (view, data) =>
            {
                view.Init(data.itemId, data.itemAmount);
            });
        }
    }
}