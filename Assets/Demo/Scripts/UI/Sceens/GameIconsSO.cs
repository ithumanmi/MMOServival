using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;
[Serializable]
public struct ShopItemTypeIcon
{

    public Sprite icon;
    public ShopItemType shopItemType;
}

[CreateAssetMenu(fileName = "Assets/Demo/Resources/GameData/Icons/Icons", menuName = "UIToolkitDemo/Icons", order = 10)]
public class GameIconsSO : ScriptableObject
{
    public List<ShopItemTypeIcon> shopItemTypeIcons;

    public Sprite GetShopTypeIcon(ShopItemType shopItemType)
    {
        if (shopItemTypeIcons == null || shopItemTypeIcons.Count == 0)
            return null;

        ShopItemTypeIcon match = shopItemTypeIcons.Find(x => x.shopItemType == shopItemType);
        return match.icon;
    }
}

