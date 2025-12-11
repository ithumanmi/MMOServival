using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsScreen : MenuScreen
{

    [SerializeField] private Button resetDataButton;
    [SerializeField] private Button SignOut;
    [SerializeField] private Button Quit;
    GameData m_SettingsData;

    void OnEnable()
    {
        // sets m_SettingsData
        resetDataButton.onClick.AddListener(OnResetDataClicked);
        Quit.onClick.AddListener(QuitGame);
        SignOut.onClick.AddListener(SceneLoaderManager.instance.Logout);
       
    }

    void OnDisable()
    {
        resetDataButton.onClick.RemoveAllListeners();
        Quit.onClick.RemoveAllListeners();
    }

    private void OnResetDataClicked()
    {
        GameDataManager.instance.OnResetData();

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
