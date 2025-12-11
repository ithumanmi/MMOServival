using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGameManager : MonoBehaviour
{
   
    public static CoreGameManager instance => ins;
    private static CoreGameManager ins;
    static string TAG = "CoreGameManager";
    public MySceneManager currentSceneManager { get; set; }
    public CallbackManager callbackManager;
    public bool canShowScene;
    public class CallbackManager
    {
        public void ClearAllCallbackWhenChangeScene()
        {

        }
    }
    [Header("Game Info")]
    public CoreGameInfomation coreGameInfo;
    void Awake()
    {
        if (ins != null && ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        ins = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
