using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GlobalEnum;
public class ShopEquipmentController : MonoBehaviour
{
    public static event Action<ShopEquipmentItemSO> ShopEquipmentPurchasing;
    public static event Action<List<ShopEquipmentItemSO>> ShopTabRefilled;
    [Tooltip("Path within the Resources folders for MailMessage ScriptableObjects.")]
    [SerializeField] string m_ResourcePath = "GameData/ShopEquipmentItems";

    // ScriptableObject game data from Resources
    public List<ShopEquipmentItemSO> m_ShopEquipmentItems = new List<ShopEquipmentItemSO>();

    // game data filtered in categories
    public List<ShopEquipmentItemSO> m_HeadItems = new List<ShopEquipmentItemSO>();
    public List<ShopEquipmentItemSO> m_TorsoItems = new List<ShopEquipmentItemSO>();
    public List<ShopEquipmentItemSO> m_LegsItems = new List<ShopEquipmentItemSO>();
    public List<ShopEquipmentItemSO> m_FeetItems = new List<ShopEquipmentItemSO>();
    public List<ShopEquipmentItemSO> m_AccessoryItems = new List<ShopEquipmentItemSO>();
    public List<ShopEquipmentItemSO> m_WeaponItems = new List<ShopEquipmentItemSO>();
    public ShopEquipment shopEquipment;
    void Start()
    {
        LoadShopData();
        UpdateView();
    }
    void OnEnable()
    {
        shopEquipment.EquipmentTooltip.ShopEquipmentClicked += OnTryBuyEquipment;
    }

    void OnDisable()
    {
        shopEquipment.EquipmentTooltip.ShopEquipmentClicked -= OnTryBuyEquipment;
    }
    void LoadShopData()
    {
        //load the ScriptableObjects from the Resources directory(default = Resources / GameData / MailMessages)
        m_ShopEquipmentItems.AddRange(Resources.LoadAll<ShopEquipmentItemSO>(m_ResourcePath));

        //sort them by type
        m_HeadItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Head).ToList();
        m_TorsoItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Torso).ToList();
        m_LegsItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Legs).ToList();
        m_FeetItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Feet).ToList();
        m_AccessoryItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Accessory).ToList();
        m_WeaponItems = m_ShopEquipmentItems.Where(c => c.Slot == EquipmentSlot.Weapon).ToList();

        m_HeadItems = SortShopItems(m_HeadItems);
        m_TorsoItems = SortShopItems(m_TorsoItems);
        m_LegsItems = SortShopItems(m_LegsItems);
        m_FeetItems = SortShopItems(m_FeetItems);
        m_AccessoryItems = SortShopItems(m_AccessoryItems);
        m_WeaponItems = SortShopItems(m_WeaponItems);
    }
    List<ShopEquipmentItemSO> SortShopItems(List<ShopEquipmentItemSO> originalList)
    {
        return originalList.OrderBy(x => x.cost).ToList();
    }
    void UpdateView()
    {
        ShopTabRefilled?.Invoke(m_HeadItems);
        ShopTabRefilled?.Invoke(m_TorsoItems);
        ShopTabRefilled?.Invoke(m_LegsItems);
        ShopTabRefilled?.Invoke(m_FeetItems);
        ShopTabRefilled?.Invoke(m_AccessoryItems);
        ShopTabRefilled?.Invoke(m_WeaponItems);
    }
    public void OnTryBuyEquipment(ShopEquipmentItemSO shopItemData)
    {
        if (shopItemData == null)
            return;
        ShopEquipmentPurchasing?.Invoke(shopItemData);
    }
}