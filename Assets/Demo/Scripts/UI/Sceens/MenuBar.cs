using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBar : MenuScreen
{
    [Header("Menu Buttons")]
    [SerializeField] private ButtonMenuBar homeScreenMenuButton;
    [SerializeField] private ButtonMenuBar charScreenMenuButton;
    [SerializeField] private ButtonMenuBar shopScreenMenuButton;
    [SerializeField] private ButtonMenuBar shopEquipmentScreenMenuButton;

    [Header("Marker")]
    [SerializeField] private RectTransform menuMarker;
    ButtonMenuBar currentButton;
    protected int idTween = -1;
    protected override void Awake()
    {
        base.Awake();

        // Register button click callbacks
        homeScreenMenuButton?.button.onClick.AddListener(ShowHomeScreen);
        charScreenMenuButton?.button.onClick.AddListener(ShowCharScreen);
        shopScreenMenuButton?.button.onClick.AddListener(ShowShopScreen);
        shopEquipmentScreenMenuButton?.button.onClick.AddListener(ShowShopEquipmentScreen);

        // Initialize marker position
        currentButton = homeScreenMenuButton;
        currentButton.ActivateButton();

        charScreenMenuButton.UnActivateButton();
        shopScreenMenuButton.UnActivateButton();
        shopEquipmentScreenMenuButton.UnActivateButton();
        MoveMarker(homeScreenMenuButton.rect);
    }

    public void ShowHomeScreen()
    {
        ActivateButton(homeScreenMenuButton);
        m_MainMenuUIManager?.ShowHomeScreen();
        MoveMarker(homeScreenMenuButton.rect);
    }

    public void ShowCharScreen()
    {
        ActivateButton(charScreenMenuButton);
        m_MainMenuUIManager?.ShowCharScreen();
        MoveMarker(charScreenMenuButton.rect);
    }


    public void ShowShopScreen()
    {
        ActivateButton(shopScreenMenuButton);
        m_MainMenuUIManager?.ShowShopScreen();
        MoveMarker(shopScreenMenuButton.rect);
    }

    public void ShowShopEquipmentScreen()
    {
        ActivateButton(shopEquipmentScreenMenuButton);
        m_MainMenuUIManager?.ShowShopEquipmentScreen();
        MoveMarker(shopEquipmentScreenMenuButton.rect);
    }

    public void ActivateButton(ButtonMenuBar menuButton)
    {
        currentButton.UnActivateButton();
        menuButton.ActivateButton();
        currentButton = menuButton;
    }

    public void MoveMarker(RectTransform target)
    {
        if (idTween != -1 && LeanTween.descr(idTween) != null)
        {
            LeanTween.cancel(idTween);

        }
        target.SetAsLastSibling();
        idTween = LeanTween.moveY(menuMarker.gameObject, target.transform.position.y, 0.2f).setEase(LeanTweenType.easeOutBack).setOnComplete(() => {
            idTween = -1;
        }).setIgnoreTimeScale(true).id;
    }

}
