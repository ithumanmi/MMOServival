using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MySceneManager
{
    public static MainMenuUIManager instance;
    public override GlobalEnum.SceneIndex mySceneIndex => GlobalEnum.SceneIndex.Lobby;
    [SerializeField] HomeScreen m_HomeModalScreen;
    [SerializeField] CharScreen m_CharModalScreen;
    [SerializeField] ShopScreen m_ShopModalScreen;
    [SerializeField] ShopEquipment m_ShopEquipmentModalScreen;

    [Header("Toolbars")]
    [Tooltip("Toolbars remain active at all times unless explicitly disabled.")]
    [SerializeField] OptionsBar m_OptionsToolbar;
    [SerializeField] MenuBar m_MenuToolbar;

    [Header("Full-screen overlays")]
    [Tooltip("Full-screen overlays block other controls until dismissed.")]
    [SerializeField] MenuScreen m_InventoryScreen;
    [SerializeField] SettingsScreen m_SettingsScreen;

    List<MenuScreen> m_AllModalScreens = new List<MenuScreen>();


    public Button btnLoad;
    public Button btnSave;

    void OnEnable()
    {
        SetupModalScreens();
        ShowHomeScreen();
    }

    void Start()
    {
        Time.timeScale = 1f;
        instance = this;
        if (CoreGameManager.instance != null)
        {
            CoreGameManager.instance.currentSceneManager = instance;
            CoreGameManager.instance.canShowScene = true;
        }
        GameDataManager.instance.ReloadData();
    }

    void SetupModalScreens()
    {
        if (m_HomeModalScreen != null)
            m_AllModalScreens.Add(m_HomeModalScreen);

        if (m_CharModalScreen != null)
            m_AllModalScreens.Add(m_CharModalScreen);

        if (m_ShopModalScreen != null)
            m_AllModalScreens.Add(m_ShopModalScreen);

        if (m_ShopEquipmentModalScreen != null)
            m_AllModalScreens.Add(m_ShopEquipmentModalScreen);

        if (m_SettingsScreen != null)
            m_AllModalScreens.Add(m_SettingsScreen);
    }

    // shows one screen at a time
    void ShowModalScreen(MenuScreen modalScreen)
    {
        foreach (MenuScreen m in m_AllModalScreens)
        {
            if (m == modalScreen)
            {
                m?.ShowScreen();
            }
            else
            {
                m?.HideScreen();
            }
        }
    }

    // methods to toggle screens on/off

    // modal screen methods 
    public void ShowHomeScreen()
    {
        ShowModalScreen(m_HomeModalScreen);
    }

    // note: screens with tabbed menus default to showing the first tab
    public void ShowCharScreen()
    {
        ShowModalScreen(m_CharModalScreen);
    }

    public void ShowShopScreen()
    {
        ShowModalScreen(m_ShopModalScreen);
    }
    public void ShowShopEquipmentScreen()
    {
        ShowModalScreen(m_ShopEquipmentModalScreen);
    }
    public void ShowSettingsScreen()
    {
        m_SettingsScreen?.ShowScreen();
    }

    public void ShowInventoryScreen()
    {
        m_InventoryScreen?.ShowScreen();
    }
}
