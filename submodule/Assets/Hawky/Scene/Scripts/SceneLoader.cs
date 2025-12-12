using System.Collections;
using UnityEngine.SceneManagement;

namespace Hawky.Scene
{
    public class SceneLoader : ISceneLoader
    {
        public IEnumerator LoadScene(string sceneName)
        {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
