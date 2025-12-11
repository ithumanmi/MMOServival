using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GlobalEnum;
using static ShopItemTypeIcon;

public class ShopItemComponent : MonoBehaviour
{

    public static event Action<ShopItemSO> ShopItemClicked;

    [Header("UI References")]
    [SerializeField] private RectTransform parentContainer;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image productImage;
    [SerializeField] private Image bannerImage;
    [SerializeField] private Text bannerLabel;
    [SerializeField] private Text costText;
    [SerializeField] private Image costIcon;
    [SerializeField] private Image discountBadge;
    [SerializeField] private Text discountLabel;
    [SerializeField] private Text discountCostText;
    [SerializeField] private Text contentValueText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Image iconContentValue;
    [SerializeField] private CoinMagnetNotify coinMagnetNotify;
    public uint contentValue;
    [Header("ScriptableObjects")]
    GameIconsSO m_GameIconsData;
    ShopItemSO m_ShopItemData;

    private void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
    }
    public void SetData(GameIconsSO gameIconsData, ShopItemSO shopItemData)
    {
        m_GameIconsData = gameIconsData;
        m_ShopItemData = shopItemData;
        SetGameData();
    }
    private void SetGameData()
    {
        if (m_ShopItemData == null)
        {
            Debug.LogWarning("Missing data in ShopItemComponent!");
            return;
        }

        // Set basic data
        descriptionText.text = m_ShopItemData.itemName;
        productImage.sprite = m_ShopItemData.sprite;

        // Set banner visibility
        bannerImage.gameObject.SetActive(HasBanner(m_ShopItemData));
        bannerLabel.gameObject.SetActive(HasBanner(m_ShopItemData));
        bannerLabel.text = m_ShopItemData.promoBannerText;
        contentValue = m_ShopItemData.contentValue;
        contentValueText.text = contentValue.ToString();    
        // Set cost and discount
        FormatBuyButton();
    }

    private void FormatBuyButton()
    {
        string currencyPrefix = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
        string decimalPlaces = (m_ShopItemData.CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0";
    
        costIcon.gameObject.SetActive(false);
        if (m_ShopItemData.contentType == ShopItemType.Gold)
        {
            costIcon.gameObject.SetActive(true);
            costIcon.sprite = m_GameIconsData.GetShopTypeIcon(ShopItemType.Gems);
        }

        if (m_ShopItemData.cost > 0.00001f)
        {
            costText.text = currencyPrefix + m_ShopItemData.cost.ToString(decimalPlaces);
            iconContentValue.sprite = m_GameIconsData.GetShopTypeIcon(m_ShopItemData.contentType);

            discountBadge.gameObject.SetActive(IsDiscounted(m_ShopItemData));
            discountLabel.text = m_ShopItemData.discount + "%";
            discountCostText.text = currencyPrefix +
                (((100 - m_ShopItemData.discount) / 100f) * m_ShopItemData.cost).ToString(decimalPlaces);
        }
        else
        {
            // Mark as free
            costText.text = "Free";
            discountBadge.gameObject.SetActive(false);
        }
    }

    private bool IsDiscounted(ShopItemSO shopItem)
    {
        return shopItem.discount > 0;
    }

    private bool HasBanner(ShopItemSO shopItem)
    {
        return !string.IsNullOrEmpty(shopItem.promoBannerText);
    }

    private void OnBuyButtonClicked()
    {
        if (m_ShopItemData == null) return;

        // Trigger the event
        ShopItemClicked?.Invoke(m_ShopItemData);
        var pfx = Poolable.TryGetPoolable<CoinMagnetNotify>(coinMagnetNotify.gameObject);
        pfx.transform.position = buyButton.transform.position;
        pfx.icon.sprite = m_GameIconsData.GetShopTypeIcon(m_ShopItemData.contentType);

        if (GameDataManager.instance.HasSufficientFunds(m_ShopItemData)){
            pfx.text.text = "+" + contentValue.ToString();
            pfx.text.color = Color.yellow;
        }
        else
        {
            AudioManager.instance.PlayDefaultWarningSound();
            pfx.text.text = "No enough";
            pfx.text.color = Color.red;
        }
        pfx.Move();
        AudioManager.instance.PlayDefaultButtonSound();
    }
  

}
