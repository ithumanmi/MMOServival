using Hawki.Inventory;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hawki.UI
{
    public enum TextStyle
    {
        Default,
        X,
    }

    public class ItemView : ListItemView
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TextStyle _textStyle;
        public void Init(string itemId, float number)
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = IconManager.Instance.GetIcon(itemId);
            }

            if (_numberText != null)
            {
                switch (_textStyle)
                {
                    case TextStyle.X:
                        _numberText.text = $"x{((int)number).ToString()}";
                        break;
                    default:
                        _numberText.text = ((int)number).ToString();
                        break;
                }
            }
        }

        public override void Init(List<ItemData> itemData)
        {
            if (itemData.Count > 0)
            {
                Init(itemData[0].itemId, itemData[0].itemAmount);
            }
        }
    }
}

