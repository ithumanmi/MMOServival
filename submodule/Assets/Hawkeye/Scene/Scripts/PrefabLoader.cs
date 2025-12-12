using UniRx.Async;
using UnityEngine;
using System.IO;

namespace Hawkeye.Scene
{
    public class PrefabLoader : ISceneLoader
    {
        private string _rootPrefab;
        private Transform _rootTrans;
        public PrefabLoader(string rootResources, Transform rootTransfrom)
        {
            _rootPrefab = rootResources;
            _rootTrans = rootTransfrom;
        }
        public async UniTask LoadScene(string sceneName)
        {
            var path = Path.Combine(_rootPrefab, sceneName);

            var resultLoad = await Resources.LoadAsync(path);

            if (resultLoad)

            if (resultLoad != null)
            {
                GameObject.Instantiate(resultLoad, _rootTrans);
            }
        }
    }
}
