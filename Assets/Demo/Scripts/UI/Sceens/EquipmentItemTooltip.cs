using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalEnum;
using static ShopItemTypeIcon;

public class EquipmentItemTooltip : MonoBehaviour
{
    public Text Name;
    public Text DescriptionText;
    public Image imageWeapon;
    public GameObject panel;
    public event Action<ShopEquipmentItemSO> ShopEquipmentClicked;
    public Button btnBuy;
    public bool isHide;
    [SerializeField] private GameIconsSO gameIconsData;

    [SerializeField] private CoinMagnetNotify coinMagnetNotify;

    public ShopEquipmentItemSO m_ShopItemData;

    private void OnEnable()
    {
        btnBuy.onClick.AddListener(OnBuy);
    }
    public void OnHide()
    {
        gameObject.SetActive(false);
        isHide = true;
    }
    public void OnBuy()
    {
        if (isHide) return;
        AudioManager.instance.PlayAltButtonSound();
        var pfx = Poolable.TryGetPoolable<CoinMagnetNotify>(coinMagnetNotify.gameObject);
        pfx.transform.position = btnBuy.transform.position;
        pfx.icon.sprite = gameIconsData.GetShopTypeIcon(ShopItemType.Gold);

        if (GameDataManager.instance.HasSufficientFundsBuyEquipment(m_ShopItemData))
        {
            OnHide();
            ShopEquipmentClicked?.Invoke(m_ShopItemData);
            pfx.text.text = "-" + m_ShopItemData.cost.ToString();
            pfx.text.color = Color.yellow;
        }
        else
        {
            AudioManager.instance.PlayDefaultWarningSound();
            pfx.text.text = "No enough";
            pfx.text.color = Color.red;
        }
        pfx.Move();
    }

}
