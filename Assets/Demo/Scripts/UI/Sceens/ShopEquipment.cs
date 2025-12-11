using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;
using static ShopItemTypeIcon;
using UnityEngine.UI;
using Lean.Pool;
using CreatorKitCodeInternal;
using static CreatorKitCodeInternal.InventoryUI;
using static CreatorKitCode.InventorySystem;
using CreatorKitCode;

public class ShopEquipment : MenuScreen
{
    public RectTransform[] HeatSlots;
    public RectTransform[] TorsoSlots;
    public RectTransform[] LegsSlots;
    public RectTransform[] FeetsSlots;
    public RectTransform[] AccessorySlots;
    public RectTransform[] WeaponSlots;

    [Header("Shop Item Prefab")]
    [SerializeField] private ShopEquipmentEntryUI shopItemPrefab;

    public Transform containViewHead;
    public Transform containViewTorso;
    public Transform containViewLegs;
    public Transform containViewFeets;
    public Transform containViewAccessory;
    public Transform containViewWeapon;

    List<Transform> listTransform= new List<Transform>();

    public Canvas DragCanvas;
    public EquipmentItemTooltip EquipmentTooltip;
    ShopEquipmentEntryUI m_HoveredItem;
    public DragData CurrentlyDragged { get; set; }
    public CanvasScaler DragCanvasScaler { get; private set; }

    ShopEquipmentEntryUI[] m_HeadEntries;
    ShopEquipmentEntryUI[] m_TorsoEntries;
    ShopEquipmentEntryUI[] m_LegsEntries;
    ShopEquipmentEntryUI[] m_FeetsEntries;
    ShopEquipmentEntryUI[] m_AccessoryEntries;
    ShopEquipmentEntryUI[] m_WeaponEntries;
    private void Start()
    {
        listTransform.Add(containViewHead);
        listTransform.Add(containViewTorso);
        listTransform.Add(containViewLegs);
        listTransform.Add(containViewFeets);
        listTransform.Add(containViewAccessory);
        listTransform.Add(containViewWeapon);
        OnClick(0);
    }
    public void Init()
    {
        m_HeadEntries = new ShopEquipmentEntryUI[32];
        m_TorsoEntries = new ShopEquipmentEntryUI[32];
        m_LegsEntries = new ShopEquipmentEntryUI[32];
        m_FeetsEntries = new ShopEquipmentEntryUI[32];
        m_AccessoryEntries = new ShopEquipmentEntryUI[32];
        m_WeaponEntries = new ShopEquipmentEntryUI[32];

        //CreateEntries(m_HeadEntries, HeatSlots);
        //CreateEntries(m_TorsoEntries, TorsoSlots);
        //CreateEntries(m_LegsEntries, LegsSlots);
        //CreateEntries(m_FeetsEntries, FeetsSlots);
        //CreateEntries(m_AccessoryEntries, AccessorySlots);
        //CreateEntries(m_WeaponEntries, WeaponSlots);


    }

    public void CreateEntries(ShopEquipmentEntryUI[] m_ItemEntries, RectTransform[] ItemSlots)
    {

        for (int i = 0; i < m_ItemEntries.Length; ++i)
        {
            m_ItemEntries[i] = Instantiate(shopItemPrefab, ItemSlots[i]);
            m_ItemEntries[i].gameObject.SetActive(false);
            m_ItemEntries[i].Owner = this;
            m_ItemEntries[i].InventoryEntry = i;
        }

    }
    private void OnEnable()
    {
        Init();
        ShopEquipmentController.ShopTabRefilled += RefillShopTab;
        m_HoveredItem = null;
        EquipmentTooltip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ShopEquipmentController.ShopTabRefilled -= RefillShopTab;
    }

    public void OnClick(int i)
    {
        AudioManager.instance.PlayAltButtonSound();
        foreach (Transform t in listTransform)
        {
            if(t == listTransform[i])
            {
                t.gameObject.SetActive(true);  
            }else
            {
                t.gameObject.SetActive(false);  
            }
        }
    }
    public void RefillShopTab(List<ShopEquipmentItemSO> shopItems)
    {
        if (shopItems == null || shopItems.Count == 0)
            return;
        EquipmentSlot equipmentSlot = shopItems[0].Slot;
        switch (equipmentSlot)
        {
            case EquipmentSlot.Head:
                 CreateShopItemElement(shopItems, HeatSlots, m_HeadEntries);
                break;
            case EquipmentSlot.Torso:
                 CreateShopItemElement(shopItems, TorsoSlots, m_TorsoEntries);
                break;
            case EquipmentSlot.Legs:
                 CreateShopItemElement(shopItems, LegsSlots, m_LegsEntries);
                break;
            case EquipmentSlot.Feet:
                 CreateShopItemElement(shopItems, FeetsSlots, m_FeetsEntries);
                break;
            case EquipmentSlot.Accessory:
                 CreateShopItemElement(shopItems, AccessorySlots, m_AccessoryEntries);
                break;
            case EquipmentSlot.Weapon:
                 CreateShopItemElement(shopItems, WeaponSlots, m_WeaponEntries);
                break;
        }
    }

    private void CreateShopItemElement(List<ShopEquipmentItemSO> shopItems, RectTransform[] RectTransform, ShopEquipmentEntryUI[] ShopEquipmentEntryUI)
    {

        for (int i = 0; i < shopItems.Count; ++i)
        {
            
            ShopEquipmentEntryUI shopItemElement = LeanPool.Spawn(shopItemPrefab, RectTransform[i].transform, false);
            shopItemElement.SetData(shopItems[i], this);
            ShopEquipmentEntryUI[i] = shopItemElement;
            shopItemElement.InventoryEntry = i;
        }
    }
    public void ActiveBuyEquipment(ShopEquipmentEntryUI exited)
    {
        m_HoveredItem = exited;
        //Tooltip.OnOpen();
        EquipmentTooltip.gameObject.SetActive(true);

        ShopEquipmentItemSO itemUsed = m_HoveredItem.InventoryEntry != -1 ? GetShopEquipmentEntryUI(exited.m_ShopEquipmentItemSO.Slot)[m_HoveredItem.InventoryEntry].m_ShopEquipmentItemSO : m_HoveredItem.m_ShopEquipmentItemSO;

        EquipmentTooltip.Name.text = itemUsed.ItemName;
        EquipmentTooltip.DescriptionText.text = itemUsed.GetDescription();
        EquipmentTooltip.imageWeapon.sprite = itemUsed.ItemSprite;

        EquipmentTooltip.m_ShopItemData = itemUsed;
        EquipmentTooltip.isHide = false;
    }
   
    public ShopEquipmentEntryUI[] GetShopEquipmentEntryUI(EquipmentSlot slot)
    {
        switch(slot) 
        {
            case EquipmentSlot.Head:
                return m_HeadEntries;
            case EquipmentSlot.Torso:
                return m_TorsoEntries;
            case EquipmentSlot.Legs:
                return m_LegsEntries;
            case EquipmentSlot.Feet:
                return m_FeetsEntries;
            case EquipmentSlot.Accessory:
                return m_AccessoryEntries;
            case EquipmentSlot.Weapon:
                return m_WeaponEntries;
        }
        return null;
    }
}
