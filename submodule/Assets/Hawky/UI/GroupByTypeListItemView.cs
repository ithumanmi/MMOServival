using Hawky.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hawky.UI
{
    public class GroupByTypeListItemView : ListItemView
    {
        [SerializeField] private ListItemView _basicListItemView;

        private List<ListItemView> pool = new List<ListItemView>();

        public override void Init(List<ItemData> itemData)
        {
            Dictionary<string, List<ItemData>> dictData = new Dictionary<string, List<ItemData>>();

            foreach (var item in itemData)
            {
                if (dictData.ContainsKey(item.itemType) == false)
                {
                    dictData.Add(item.itemType, new List<ItemData>());
                }

                dictData[item.itemType].Add(item);
            }

            ContainerUtilities.UpdateContainer(pool, _basicListItemView, _basicListItemView.transform.parent, dictData.ToList(), (view, data) =>
            {
                view.Init(data.Value);
            });
        }
    }

}