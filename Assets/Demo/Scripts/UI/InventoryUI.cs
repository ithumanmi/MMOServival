using System.Collections;
using System.Collections.Generic;
using CreatorKitCode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CreatorKitCode.InventorySystem;

namespace CreatorKitCodeInternal 
{
    /// <summary>
    /// Handle all the UI code related to the inventory (drag'n'drop of object, using objects, equipping object etc.)
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        public class DragData
        {
            public ItemEntryUI DraggedEntry;
            public RectTransform OriginalParent;
        }
    
        public RectTransform[] ItemSlots;
    
        public ItemEntryUI ItemEntryPrefab;
        //public ItemTooltip Tooltip;
 
        public EquipmentUI EquipementUI;


        public Canvas DragCanvas;
    
        public DragData CurrentlyDragged { get; set; }
        public CanvasScaler DragCanvasScaler { get; private set; }

        public ItemTooltip ItemtTooltip;

        public ItemEntryUI[] m_ItemEntries;
        ItemEntryUI m_HoveredItem;
    
        public void Init()
        {
            CurrentlyDragged = null;

            DragCanvasScaler = DragCanvas.GetComponentInParent<CanvasScaler>();
        
            m_ItemEntries = new ItemEntryUI[ItemSlots.Length];

            for (int i = 0; i < m_ItemEntries.Length; ++i)
            {
                m_ItemEntries[i] = Instantiate(ItemEntryPrefab, ItemSlots[i]);
                m_ItemEntries[i].gameObject.SetActive(false);
                m_ItemEntries[i].Owner = this;
                m_ItemEntries[i].InventoryEntry = i;
            }
        
            EquipementUI.Init(this);
        }

        void OnEnable()
        {
            m_HoveredItem = null;
            ItemtTooltip.gameObject.SetActive(false);
            //Tooltip.gameObject.SetActive(false);
            ItemtTooltip.ItemEquipmentClicked+= ObjectDoubleClicked;
        }

        public void Load()
        {
            //EquipementUI.UpdateEquipment(m_Data.Equipment, m_Data.Stats);
            EquipementUI.UpdateEquipment(GameDataManager.instance.EquipmentData);

            for (int i = 0; i < m_ItemEntries.Length; ++i)
            {
                m_ItemEntries[i].UpdateEntry();
            }
            GameDataManager.instance.m_SaveManager.SaveGame();
        }
        public void ActiveBuyEquipment(ItemEntryUI exited)
        {
            AudioManager.instance.PlayAltButtonSound();
            m_HoveredItem = exited;
            //Tooltip.OnOpen();
            ItemtTooltip.gameObject.SetActive(true);
            EquipmentItem _tempEquipmentItem = GameDataManager.instance.InventoryData.Entries[m_HoveredItem.InventoryEntry].Item as EquipmentItem;
            EquipmentItem itemUsed = m_HoveredItem.InventoryEntry != -1 ? _tempEquipmentItem : m_HoveredItem.EquipmentItem;


            ItemtTooltip.Name.text = itemUsed.ItemName;
            ItemtTooltip.DescriptionText.text = itemUsed.GetDescription();
            ItemtTooltip.imageWeapon.sprite = itemUsed.ItemSprite;

            ItemtTooltip.indexItem = exited.InventoryEntry;
        }
        public void ObjectDoubleClicked(int index)
        {
            Debug.Log("ObjectDoubleClicked");
            InventoryEntry usedItem = GameDataManager.instance.InventoryData.Entries[index];
            if(GameDataManager.instance.InventoryData.UseItem(usedItem))
                //SFXManager.PlaySound(SFXManager.Use.Sound2D, new SFXManager.PlayData() {Clip = usedItem.Item is EquipmentItem ? SFXManager.ItemEquippedSound : SFXManager.ItemUsedSound} );
        
            Load();
        }
      
    }
}