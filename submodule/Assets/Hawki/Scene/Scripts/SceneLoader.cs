using System.Collections;
using UnityEngine.SceneManagement;

namespace Hawki.Scene
{
    public class SceneLoader : ISceneLoader
    {
        public IEnumerator LoadScene(string sceneName)
        {
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}
