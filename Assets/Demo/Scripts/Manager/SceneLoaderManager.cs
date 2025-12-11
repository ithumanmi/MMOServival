using CreatorKitCodeInternal;
using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager instance
    {
        get
        {
            if (ins == null)
            {
                ins = Instantiate(CoreGameManager.instance.coreGameInfo.singletonPrefabInfo.sceneLoaderManagerPrefab);
                DontDestroyOnLoad(ins.gameObject);
            }
            return ins;
        }
    }
    private static SceneLoaderManager ins;

    void Awake()
    {
        if (ins != null && ins != this)
        {
            Destroy(this.gameObject);
            return;
        }
        lastSceneName = currentSceneName = string.Empty;
        Hide();
    }
    public static bool IsExist()
    {
        if (instance != null)
        {
            return true;
        }
        return false;
    }
    public enum State
    {
        Hide,
        Show
    }
    public State currentState { get; set; }
    [SerializeField] CanvasGroup myCanvasGroup;
    [SerializeField] Canvas myCanvas;
    [SerializeField] Image imgBg;
    [SerializeField] Slider loadingBar;
    [SerializeField] Text txtLoading;
    [SerializeField] Sprite defaultBg;
    [SerializeField] Image imgFillMask;
    [SerializeField] Image imgFill;
    [ReadOnly] public string lastSceneName;
    [ReadOnly] public string currentSceneName;
    float updateDuration = 3.0f;
    float targetValue = 1.0f;
    float startValue;
    int idTweenMyCanvasGroup = -1;
    public System.Action OnFinished;

    [ContextMenu("Show")]
    void Show(bool _updateNow = true, System.Action _onFinished = null)
    {
        if (idTweenMyCanvasGroup != -1 && LeanTween.descr(idTweenMyCanvasGroup) != null)
        {
            LeanTween.cancel(idTweenMyCanvasGroup);
        }
        idTweenMyCanvasGroup = -1;

        currentState = State.Show;
        myCanvasGroup.blocksRaycasts = true;

        if (_updateNow)
        {
            myCanvasGroup.alpha = 1f;
            if (_onFinished != null)
            {
                _onFinished();
            }
        }
        else
        {
            idTweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 1f, 0.2f).setOnComplete(() => {
                idTweenMyCanvasGroup = -1;
                if (_onFinished != null)
                {
                    _onFinished();
                }
            }).id;
        }
        //txtLoading.text = LocalizeUtil.GetStringLocalize(LocalizeStringKeyEnum.Global_Loading);
    }

    void Hide(bool _updateNow = true)
    {
        if (idTweenMyCanvasGroup != -1 && LeanTween.descr(idTweenMyCanvasGroup) != null)
        {
            LeanTween.cancel(idTweenMyCanvasGroup);
        }
        idTweenMyCanvasGroup = -1;

        currentState = State.Hide;
        myCanvasGroup.blocksRaycasts = false;
        if (_updateNow)
        {
            myCanvasGroup.alpha = 0f;
            if (OnFinished != null)
            {
                OnFinished();
            }
        }
        else
        {
            idTweenMyCanvasGroup = LeanTween.alphaCanvas(myCanvasGroup, 0f, 0.2f).setOnComplete(() => {
                idTweenMyCanvasGroup = -1;
                if (OnFinished != null)
                {
                    OnFinished();
                }
            }).id;
        }
    }
    public Coroutine LoadScene(GlobalEnum.SceneIndex _scene)
    {

        lastSceneName = currentSceneName;
        currentSceneName = _scene.ToString();
        loadingBar.value = loadingBar.minValue;
        startValue = loadingBar.minValue;

        return StartCoroutine(DoActionLoadScene(currentSceneName));
    }
    IEnumerator DoActionLoadScene(string _nameScene)
    {
        Debug.Log("LoadScene " + _nameScene);
        bool _isFinished = false;

        Show(false, () => {
            _isFinished = true;
        });
        yield return new WaitUntil(() => _isFinished);
        yield return Yielders.EndOfFrame;
        CoreGameManager.instance.canShowScene = false;
        DateTime _timeStart = DateTime.UtcNow;
        LeanTween.cancelAll(true);
        //PopupManager.instance.DestroyAll();
        //CoreGameManager.instance.ClearAllCallbackPressBackKey();
        LeanPool.DespawnAll();
        ////MyAudioManager.instance.StopAll();
        //CoreGameManager.instance.currentSceneManager.RemoveAllCallback();
        //CoreGameManager.instance.callbackManager.ClearAllCallbackWhenChangeScene();

        yield return Yielders.EndOfFrame;

        //while (!_asyncLoad.isDone){
        //	yield return null;
        //          //loadingBar.value = Mathf.RoundToInt(_asyncLoad.progress) * 100f;

        //}

        //loadingBar.value = loadingBar.maxValue;


        yield return StartCoroutine(UpdateProgressLoading());

        if (myCanvas.worldCamera == null)
        {
            myCanvas.worldCamera = Camera.main;
        }

        yield return Resources.UnloadUnusedAssets();


        var _asyncLoad = SceneManager.LoadSceneAsync(_nameScene, LoadSceneMode.Single);
        yield return Yielders.EndOfFrame;

        long _timeLoadScene = (long)(DateTime.UtcNow - _timeStart).TotalMilliseconds;

        while (_timeLoadScene < 1000)
        {
            yield return null;
            _timeLoadScene = (long)(DateTime.UtcNow - _timeStart).TotalMilliseconds;
        }
        yield return new WaitUntil(() => CoreGameManager.instance.canShowScene);
        // AudioManager.instance.isStopPlayingNewSound = false;
        Hide();

        yield break;
    }
    IEnumerator UpdateProgressLoading()
    {
        float elapsedTime = 0f;

        while (elapsedTime < updateDuration)
        {
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / updateDuration);

            loadingBar.value = newValue;
            //txtLoading.text = LocalizeUtil.GetStringLocalize(LocalizeStringKeyEnum.Global_Loading) + " " + Mathf.RoundToInt(loadingBar.value * 100f).ToString() + "%";
            txtLoading.text = Mathf.RoundToInt((loadingBar.value * 100f)).ToString() + "%";
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        loadingBar.value = targetValue;
        yield return Yielders.Get(1f);
    }

    public void Logout()
    {
        OnFinished = FirebaseAuthManager.instance.Logout;
        LoadScene(GlobalEnum.SceneIndex.LoginScene);
    }
}
