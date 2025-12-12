using UniRx.Async;

namespace Hawkeye.Scene
{
    public interface ISceneLoader
    {
        UniTask LoadScene(string sceneName);
    }
}
