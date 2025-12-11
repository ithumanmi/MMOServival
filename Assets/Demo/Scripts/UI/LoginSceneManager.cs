using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MySceneManager
{
    public override GlobalEnum.SceneIndex mySceneIndex => GlobalEnum.SceneIndex.LoginScene;
    public static LoginSceneManager instance;
    public Button buttonPlayAsGuest;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registrationPanel;

    private void Awake()
    {
        CreateInstance();
    }

    void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        buttonPlayAsGuest?.onClick.AddListener(GoHomeScene);
        CoreGameManager.instance.canShowScene = true; 
        OpenLoginPanel();

    }
    public void GoHomeScene()
    {
        SceneLoaderManager.instance.LoadScene(GlobalEnum.SceneIndex.Lobby);
    }
    void OnDestroy()
    {
        instance = null;
    }

    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        registrationPanel.SetActive(false);
    }

    public void OpenRegistrationPanel() 
    {
        loginPanel.SetActive(false);
        registrationPanel.SetActive(true);
    }

}
