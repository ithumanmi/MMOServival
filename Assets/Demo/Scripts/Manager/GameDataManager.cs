using CreatorKitCode;
using CreatorKitCodeInternal;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GlobalEnum;

public class GameDataManager : Singleton<GameDataManager>
{

    protected override void Awake()
    {
        base.Awake();
        m_SaveManager = GetComponent<SaveManager>();

    }


    public static event Action<GameData> PotionsUpdated;
    public static event Action<GameData> FundsUpdated;
    public static event Action<ShopItemType, uint, Vector2> RewardProcessed;
    public static event Action<ShopItemSO> TransactionProcessed;
    public static event Action<ShopItemSO> TransactionFailed;
    public static event Action<bool> LevelUpButtonEnabled;
    public static event Action<bool> CharacterLeveledUp;
    [SerializeField] GameData m_GameData;

    [Header("Inventory Data")]
    [SerializeField] InventorySystem m_InventoryData;

    [Header("Equipment")]
    [SerializeField] EquipmentSystem m_EquipmentData;


    public GameData GameData { set => m_GameData = value; get => m_GameData; }
    public InventorySystem InventoryData { set => m_InventoryData = value; get => m_InventoryData; }
    public EquipmentSystem EquipmentData { set => m_EquipmentData = value; get => m_EquipmentData; }

    public SaveManager m_SaveManager;


    void OnEnable()
    {
        ShopController.ShopItemPurchasing += OnPurchaseItem;
        ShopEquipmentController.ShopEquipmentPurchasing += OnPurchaseEquipment;
        SceneLoaderManager.instance.OnFinished += ReloadData;
        //SettingsScreen.SettingsUpdated += OnSettingsUpdated;

        //MailController.RewardClaimed += OnRewardClaimed;

        //CharScreenController.CharacterShown += OnCharacterShown;
        //CharScreenController.LevelPotionUsed += OnLevelPotionUsed;

    }

    void OnDisable()
    {
        ShopController.ShopItemPurchasing -= OnPurchaseItem;
        ShopEquipmentController.ShopEquipmentPurchasing -= OnPurchaseEquipment;
        //SettingsScreen.SettingsUpdated -= OnSettingsUpdated;
        SceneLoaderManager.instance.OnFinished -= ReloadData;
        //MailController.RewardClaimed -= OnRewardClaimed;

        //CharScreenController.CharacterShown -= OnCharacterShown;
        //CharScreenController.LevelPotionUsed -= OnLevelPotionUsed;

    }

    public void Start()
    {
        //if saved data exists, load saved data


        ReloadData();

        // flag that GameData is loaded the first time

    }
    public void ReloadData()
    {
        Debug.Log("ReloadData");
        m_SaveManager?.LoadGame(); 
        EquipmentData.SetWeapon();
        UpdateFunds();

    }
    // transaction methods 
    void UpdateFunds()
    {
        if (m_GameData != null)
            FundsUpdated?.Invoke(m_GameData);
    }


    public bool HasSufficientFunds(ShopItemSO shopItem)
    {
        if (shopItem == null)
            return false;
        Debug.Log("HasSufficientFunds");
        CurrencyType currencyType = shopItem.CostInCurrencyType;

        float discountedPrice = (((100 - shopItem.discount) / 100f) * shopItem.cost);

        switch (currencyType)
        {
            case CurrencyType.Gold:
                return m_GameData.gold >= discountedPrice;

            case CurrencyType.Gems:
                return m_GameData.gems >= discountedPrice;

            case CurrencyType.USD:
                return true;

            default:
                return false;
        }
    }
    public bool HasSufficientFundsBuyEquipment(ShopEquipmentItemSO shopItem)
    {
        if (shopItem == null)
            return false;
        CurrencyType currencyType = shopItem.CostInCurrencyType;
        float discountedPrice =  shopItem.cost;
        return m_GameData.gold >= discountedPrice;
     
    }


    void PayTransaction(ShopItemSO shopItem)
    {

        if (shopItem == null)
            return;
        Debug.Log("PayTransaction");
        AudioManager.instance.PlayDefaultTransactionSound();
        CurrencyType currencyType = shopItem.CostInCurrencyType;

        float discountedPrice = (((100 - shopItem.discount) / 100f) * shopItem.cost);

        switch (currencyType)
        {
            case CurrencyType.Gold:
                m_GameData.gold -= (uint)discountedPrice;
                break;

            case CurrencyType.Gems:
                m_GameData.gems -= (uint)discountedPrice;
                break;

            // non-monetized placeholder - invoke in-app purchase logic/interface for a real application
            case CurrencyType.USD:
                break;
        }
    }



    void ReceivePurchasedGoods(ShopItemSO shopItem)
    {
        if (shopItem == null)
            return;
        Debug.Log("RêcivePurchaseGoods");
        ShopItemType contentType = shopItem.contentType;
        uint contentValue = shopItem.contentValue;
        Debug.Log(m_GameData.gold);
        Debug.Log(contentValue);
        ReceiveContent(contentType, contentValue);
    }

    // for gifts or purchases
    void ReceiveContent(ShopItemType contentType, uint contentValue)
    {
        Debug.Log("ReceiveContent");
        switch (contentType)
        {
            case ShopItemType.Gold:
                m_GameData.gold += contentValue;
                UpdateFunds();
                break;

            case ShopItemType.Gems:
                m_GameData.gems += contentValue;
                UpdateFunds();
                break;

        }
    }


    // event-handling methods

    // buying item from ShopScreen, pass button screen position 
    void OnPurchaseItem(ShopItemSO shopItem)
    {
        if (shopItem == null)
            return;
        Debug.Log("OnPurchaseItem");
        // invoke transaction succeeded or failed
        if (HasSufficientFunds(shopItem))
        {
            PayTransaction(shopItem);
            ReceivePurchasedGoods(shopItem);
            TransactionProcessed?.Invoke(shopItem);

            ////AudioManager.PlayDefaultTransactionSound();
        }
        else
        {
            // notify listeners (PopUpText, sound, etc.)
            TransactionFailed?.Invoke(shopItem);
            //AudioManager.PlayDefaultWarningSound();
        }
    }
    public void OnPurchaseEquipment(ShopEquipmentItemSO shopEquipmentItemSO)

    {
        if(shopEquipmentItemSO == null) return;
        if (GameData.gold < shopEquipmentItemSO.cost) return;
        AudioManager.instance.PlayDefaultTransactionSound();
        m_GameData.gold -= (uint)shopEquipmentItemSO.cost;
        UpdateFunds();
        m_InventoryData.AddItem(shopEquipmentItemSO);
    }
 
    void OnSettingsUpdated(GameData gameData)
    {

        if (gameData == null)
            return;

        m_GameData.sfxVolume = gameData.sfxVolume;
        m_GameData.musicVolume = gameData.musicVolume;
        m_GameData.dropdownSelection = gameData.dropdownSelection;
        m_GameData.isSlideToggled = gameData.isSlideToggled;
        m_GameData.isToggled = gameData.isToggled;
        m_GameData.theme = gameData.theme;
        m_GameData.username = gameData.username;
        m_GameData.buttonSelection = gameData.buttonSelection;
    }

    // attempt to level up the character using a potion
    void OnLevelPotionUsed(CharacterData charData)
    {
        if (charData == null)
            return;

        bool isLeveled = false;
        //if (CanLevelUp(charData))
        //{
        //    PayLevelUpPotions(charData.GetXPForNextLevel());
        //    isLeveled = true;
        //    AudioManager.PlayVictorySound();
        //}
        //else
        //{
        //    AudioManager.PlayDefaultWarningSound();
        //}
        // notify other objects if level up succeeded or failed
        CharacterLeveledUp?.Invoke(isLeveled);
    }

    public void OnResetData()
    {
        m_GameData.gold = 0;
        m_GameData.gems = 0;
        EquipmentData.ResetWeapon();
        EquipmentData.m_HeadSlot = null;
        EquipmentData.m_TorsoSlot = null;
        EquipmentData.m_LegsSlot = null;
        EquipmentData.m_FeetSlot = null;
        EquipmentData.m_AccessorySlot = null;
        foreach (var equipment in InventoryData.Entries)
        {
            equipment.Item = null;
            equipment.Count = 0;
        }
        m_SaveManager.SaveGame();
        ReloadData();
    }




}
