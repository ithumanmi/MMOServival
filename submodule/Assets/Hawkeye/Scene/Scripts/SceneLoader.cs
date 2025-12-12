using UniRx.Async;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

namespace Hawkeye.Scene
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadScene(string sceneName)
        {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
