using CreatorKitCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CreatorKitCode.EquipmentItem;
using static GlobalEnum;
[CreateAssetMenu(fileName = "ShopEquipmentItemGameData", menuName = "UIToolkitDemo/ShopEqipmentItem", order = 4)]
public class ShopEquipmentItemSO : Weapon
{
    public Sprite spriteBorder;
    public float cost;
    public CurrencyType CostInCurrencyType;

    public override string GetDescription()
    {
        string desc = "";


        bool requireStrength = MinimumStrength > 0;
        bool requireDefense = MinimumDefense > 0;
        bool requireAgility = MinimumAgility > 0;

        if (requireStrength || requireAgility || requireDefense)
        {
            desc += "\nRequire : \n";
            desc += $"\ncost : {cost}";


            if (requireStrength)
                desc += $"\nStrength : {MinimumStrength}";

            if (requireAgility)
            {
                if (requireStrength) desc += " & ";
                desc += $"\nDefense : {MinimumDefense}";
            }

            if (requireDefense)
            {
                if (requireStrength || requireAgility) desc += " & ";
                desc += $"\nAgility : {MinimumAgility}";
            }
        }

        return desc;
    }

}
