using System.Collections;

namespace Hawki.Scene
{
    public interface ISceneLoader
    {
        IEnumerator LoadScene(string sceneName);
    }
}
