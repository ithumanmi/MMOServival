using Hawky.Inventory;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.UI
{
    public enum TextStyle
    {
        Default,
        X,
        Plus,
        Minus,
    }

    public class ItemView : ListItemView
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TextStyle _textStyle;
        [SerializeField] private Transform _iconTransform;
        public void Init(string itemId, float number)
        {
            if (_iconImage != null)
            {
                _iconImage.sprite = IconManager.Ins.GetIcon(itemId);

                if (_iconImage.sprite == null)
                {
                    _iconImage.sprite = IconManager.Ins.GetIcon("ICON_DEFAULT");
                }
            }

            if (_numberText != null)
            {
                switch (_textStyle)
                {
                    case TextStyle.X:
                        _numberText.text = $"x{((int)number).ToString()}";
                        break;
                    case TextStyle.Plus:
                        _numberText.text = $"+{((int)number).ToString()}";
                        break;
                    case TextStyle.Minus:
                        _numberText.text = $"-{((int)number).ToString()}";
                        break;
                    default:
                        _numberText.text = ((int)number).ToString();
                        break;
                }
            }
        }

        public Transform GetIconTransform()
        {
            if (_iconTransform == null)
            {
                _iconTransform = _iconImage.transform;
            }
            return _iconTransform;
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

