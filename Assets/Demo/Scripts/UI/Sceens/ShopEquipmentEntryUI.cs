using CreatorKitCode;
using CreatorKitCodeInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ShopItemTypeIcon;
using static UnityEngine.UI.GridLayoutGroup;

public class ShopEquipmentEntryUI : MonoBehaviour, IPointerClickHandler
{
    public Image IconeImage;
    public Image IconeBorder;
    public ShopEquipmentItemSO m_ShopEquipmentItemSO;
   
    public ShopEquipment Owner { get; set; }
    public int InventoryEntry { get; set; } = -1;
    
    public void SetData( ShopEquipmentItemSO ShopEquipmentItemSO, ShopEquipment Owner)
    {
        m_ShopEquipmentItemSO = ShopEquipmentItemSO;
        this.Owner = Owner;
        SetGameData();
    }
    private void SetGameData()
    {
        if (m_ShopEquipmentItemSO == null)
        {
            Debug.LogWarning("Missing data in ShopItemComponent!");
            return;
        }

        IconeImage.sprite = m_ShopEquipmentItemSO.ItemSprite;
        IconeBorder.sprite = m_ShopEquipmentItemSO.spriteBorder;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlayAltButtonSound();
        Owner.ActiveBuyEquipment(this);
    }

}
