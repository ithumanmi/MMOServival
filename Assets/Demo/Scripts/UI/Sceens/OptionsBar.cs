using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class OptionsBar : MenuScreen
{
    [Header("Buttons")]
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button shopGemButton;
    [SerializeField] private Button shopGoldButton;

    [Header("Labels")]
    [SerializeField] private Text m_GoldLabel;
    [SerializeField] private Text gemCountText;

    [Header("Lerp Settings")]
    [SerializeField] private float k_LerpTime = 0.6f;

    public MainMenuUIManager mainMenuUIManager;

    private void OnEnable()
    {
        GameDataManager.FundsUpdated += OnFundsUpdated;
    }

    private void OnDisable()
    {
        GameDataManager.FundsUpdated -= OnFundsUpdated;
    }

    protected override void Awake()
    {
        base.Awake();
        // Register button callbacks
        optionsButton?.onClick.AddListener(ShowOptionsScreen);
        shopGemButton?.onClick.AddListener(OpenGemShop);
        shopGoldButton?.onClick.AddListener(OpenGoldShop);
    }

    private void ShowOptionsScreen()
    {
        //AudioManager.PlayDefaultButtonSound();
        mainMenuUIManager?.ShowSettingsScreen();
    }

    private void OpenGoldShop()
    {
        //AudioManager.PlayDefaultButtonSound();
        mainMenuUIManager?.ShowShopScreen();
    }

    private void OpenGemShop()
    {
        //AudioManager.PlayDefaultButtonSound();
        mainMenuUIManager?.ShowShopScreen();
    }

    public void SetGold(uint gold)
    {

        try
        {
            uint startValue = (uint)Int32.Parse(m_GoldLabel.text.ToString());
            StartCoroutine(LerpRoutine(m_GoldLabel, startValue, gold, k_LerpTime));
        }
        catch (Exception ex) { }
    }

    public void SetGems(uint gems)
    {
        try
        {
            uint startValue = (uint)Int32.Parse(gemCountText.text.ToString());
            StartCoroutine(LerpRoutine(gemCountText, startValue, gems, k_LerpTime));
        }
        catch (Exception ex) { }
    }

    private void OnFundsUpdated(GameData gameData)
    {
        SetGold(gameData.gold);
        SetGems(gameData.gems);
    }

    private IEnumerator LerpRoutine(Text label, uint startValue, uint endValue, float duration)
    {
        float lerpValue = startValue;
        float t = 0f;

        while (Mathf.Abs((float)endValue - lerpValue) > 0.05f)
        {
            t += Time.deltaTime / duration;
            lerpValue = Mathf.Lerp(startValue, endValue, t);
            label.text = Mathf.RoundToInt(lerpValue).ToString();
            yield return null;
        }

        label.text = endValue.ToString();
    }
}
