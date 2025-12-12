using System.Collections;

namespace Hawky.Scene
{
    public interface ISceneLoader
    {
        IEnumerator LoadScene(string sceneName);
    }
}
