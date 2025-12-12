using Hawki.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace Hawki.UI
{
    public class CustomGroupByTypeListItemView : ListItemView
    {
        [SerializeField] private string _prefix = string.Empty;
        [SerializeField] private RectTransform _root;

        private List<ListItemView> _current = new List<ListItemView>();

        protected override void OnFree()
        {
            base.OnFree();

            ClearCache();
        }

        private void ClearCache()
        {
            foreach (var item in _current)
            {
                ShopItemListItemViewByTypeResourcesLoader.Instance.FreeResources(item);
            }

            _current.Clear();
        }

        public override void Init(List<ItemData> itemData)
        {
            ClearCache();

            Dictionary<string, List<ItemData>> dictData = new Dictionary<string, List<ItemData>>();

            foreach (var item in itemData)
            {
                if (dictData.ContainsKey(item.itemType) == false)
                {
                    dictData.Add(item.itemType, new List<ItemData>());
                }

                dictData[item.itemType].Add(item);
            }

            foreach (var listItemPair in dictData)
            {
                var name = $"{_prefix}{listItemPair.Key}";

                var listItemView = ShopItemListItemViewByTypeResourcesLoader.Instance.LoadResources(name, default, _root);

                if (listItemView == null)
                {
                    Debug.LogError($"Không có config của Type = {name}");
                    continue;
                }

                listItemView.Init(listItemPair.Value);

                _current.Add(listItemView);
            }
        }
    }
}
