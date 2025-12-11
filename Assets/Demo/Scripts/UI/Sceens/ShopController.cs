using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalEnum;

public class ShopController : MonoBehaviour
{
    // notify ShopScreen (pass ShopItem data + screen pos of buy button)
    public static event Action<ShopItemSO> ShopItemPurchasing;
    public static event Action<List<ShopItemSO>> ShopTabRefilled;
    [Tooltip("Path within the Resources folders for MailMessage ScriptableObjects.")]
    [SerializeField] string m_ResourcePath = "GameData/ShopItems";

    // ScriptableObject game data from Resources
    [SerializeField] List<ShopItemSO> m_ShopItems = new List<ShopItemSO>();

    // game data filtered in categories
    [SerializeField] List<ShopItemSO> m_GoldShopItems = new List<ShopItemSO>();
    [SerializeField] List<ShopItemSO> m_GemShopItems = new List<ShopItemSO>();
    void OnEnable()
    {
        ShopItemComponent.ShopItemClicked += OnTryBuyItem;
    }

    void OnDisable()
    {
        ShopItemComponent.ShopItemClicked -= OnTryBuyItem;
    }
    void Start()
    {
        LoadShopData();
        UpdateView();
    }
    void LoadShopData()
    {
        //load the ScriptableObjects from the Resources directory(default = Resources / GameData / MailMessages)
        m_ShopItems.AddRange(Resources.LoadAll<ShopItemSO>(m_ResourcePath));

        //sort them by type
        m_GoldShopItems = m_ShopItems.Where(c => c.contentType == ShopItemType.Gold).ToList();
        m_GemShopItems = m_ShopItems.Where(c => c.contentType == ShopItemType.Gems).ToList();

        m_GoldShopItems = SortShopItems(m_GoldShopItems);
        m_GemShopItems = SortShopItems(m_GemShopItems);
    }

    List<ShopItemSO> SortShopItems(List<ShopItemSO> originalList)
    {
        return originalList.OrderBy(x => x.cost).ToList();
    }
    void UpdateView()
    {
        ShopTabRefilled?.Invoke(m_GemShopItems);
        ShopTabRefilled?.Invoke(m_GoldShopItems);
    }
    // try to buy the item, pass the screen coordinate of the buy button
    public void OnTryBuyItem(ShopItemSO shopItemData)
    {
        if (shopItemData == null)
            return;
        Debug.Log("OntryBuyItem");
        // notify other objects we are trying to buy an item
        ShopItemPurchasing?.Invoke(shopItemData);
    }
}
