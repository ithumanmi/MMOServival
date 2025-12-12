using System.Collections;
using System.IO;
using UnityEngine;

namespace Hawky.Scene
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
        public IEnumerator LoadScene(string sceneName)
        {
            var path = Path.Combine(_rootPrefab, sceneName);

            var resultLoad = Resources.LoadAsync(path);

            yield return resultLoad;

            if (resultLoad.asset != null)
            {
                GameObject prefab = (GameObject)resultLoad.asset;
                GameObject.Instantiate(prefab, _rootTrans);
            }
            else
            {
                Debug.LogError("Failed to load prefab from resources at path: " + path);
            }
        }
    }
}
