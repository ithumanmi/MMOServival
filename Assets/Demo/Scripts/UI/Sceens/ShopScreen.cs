using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalEnum;
using static ShopItemTypeIcon;

public class ShopScreen : MenuScreen
{

    [Header("Shop Item Prefab")]
    [SerializeField] private ShopItemComponent shopItemPrefab;

    [Header("ScriptableObjects")]
    [SerializeField] private GameIconsSO gameIconsData;
  
    public ScrollRect scrollViewCoin;
    public ScrollRect scrollViewGem;
   
    private void OnEnable()
    {

        ShopController.ShopTabRefilled += RefillShopTab;
    }

    private void OnDisable()
    {
        ShopController.ShopTabRefilled -= RefillShopTab;
    }
    private void Start()
    {
        OpenTabCoin();
    }
    public void OpenTabCoin()
    {
        AudioManager.instance.PlayAltButtonSound();
        scrollViewCoin.gameObject.SetActive(true);
        scrollViewGem.gameObject.SetActive(false);
    }

    public void OpenTabGem()
    {
        AudioManager.instance.PlayAltButtonSound();
        scrollViewCoin.gameObject.SetActive(false);
        scrollViewGem.gameObject.SetActive(true);
    }
    public void RefillShopTab(List<ShopItemSO> shopItems)
    {
        if (shopItems == null || shopItems.Count == 0)
            return;
        // Populate the tab with new items
        foreach (ShopItemSO shopItem in shopItems)
        {
            CreateShopItemElement(shopItem);
        }
    }

    private void CreateShopItemElement(ShopItemSO shopItemData)
    {
        if (shopItemData == null || shopItemPrefab == null)
            return;
        Transform _transform = null;
        switch (shopItemData.contentType)
        {
            case ShopItemType.Gold:
                _transform = scrollViewCoin.content;
                break;
            case ShopItemType.Gems:
                _transform = scrollViewGem.content;
                break;
        }

        ShopItemComponent shopItemElement = LeanPool.Spawn(shopItemPrefab, _transform, false);
        shopItemElement.SetData(gameIconsData, shopItemData);

    }
}