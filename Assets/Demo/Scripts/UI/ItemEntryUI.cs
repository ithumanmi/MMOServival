using System;
using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CreatorKitCodeInternal 
{
    public class ItemEntryUI : MonoBehaviour, IPointerClickHandler
    {    
        public Image IconeImage;
        public Text ItemCount;

        public int InventoryEntry { get; set; } = -1;
        public EquipmentItem EquipmentItem { get; private set; }
    
        public InventoryUI Owner { get; set; }
        public int Index { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (InventoryEntry != -1)
            {
                if(GameDataManager.instance.InventoryData.Entries[InventoryEntry] != null)
                    Owner.ActiveBuyEquipment(this);
            }        
        }

        public void UpdateEntry()
        {
            var entry = GameDataManager.instance.InventoryData.Entries[InventoryEntry];
            bool isEnabled = entry.Item != null;

            gameObject.SetActive(isEnabled);
            if (isEnabled)
            {
                IconeImage.sprite = entry.Item.ItemSprite;


                if (entry.Count > 0)
                {
                    ItemCount.gameObject.SetActive(true);
                    ItemCount.text = entry.Count.ToString();
                }
                else
                {
                    ItemCount.gameObject.SetActive(false);
                    entry.Count = 0;    
                }
            }
        }

        public void SetupEquipment(EquipmentItem itm)
        {
            EquipmentItem = itm;

            enabled = itm != null;
            IconeImage.enabled = enabled;
            if (enabled)
                IconeImage.sprite = itm.ItemSprite;
        }
    }
}