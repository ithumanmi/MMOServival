using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hawky.ResourcesLoader
{
    public abstract class ResourcesLoader<T, LOADTYPE> : RuntimeSingleton<T> where LOADTYPE : ResourcesPool where T : ISingleton, new()
    {
        protected Dictionary<string, LOADTYPE> _dictResourcesLoaded = new Dictionary<string, LOADTYPE>();

        protected Dictionary<string, List<LOADTYPE>> _dictResourcesPool = new Dictionary<string, List<LOADTYPE>>();

        protected Dictionary<string, LOADTYPE> _idUsingResources = new Dictionary<string, LOADTYPE>();

        private static Transform _defaultRootTemp = null;
        private static Transform _defaultRoot
        {
            get
            {
                if (_defaultRootTemp == null)
                {
                    _defaultRootTemp = new GameObject("Resources Root").transform;
                    _defaultRootTemp.transform.position = Vector3.zero;
                }

                return _defaultRootTemp;
            }
        }

        protected abstract string ResourcesPath();

        public LOADTYPE LoadResources(string resourcesId, Model model = null, Transform transform = null)
        {
            if (!_dictResourcesPool.TryGetValue(resourcesId, out var hehe))
            {
                _dictResourcesPool.Add(resourcesId, new List<LOADTYPE>());
            }

            _dictResourcesPool[resourcesId] = _dictResourcesPool[resourcesId].FindAll(x => x != null);

            if (transform == null)
            {
                transform = _defaultRoot;
            }

            LOADTYPE target = null;
            foreach (var view in _dictResourcesPool[resourcesId])
            {
                if (view.free)
                {
                    target = view;
                    target.transform.SetParent(transform, false);
                    break;
                }
            }

            if (target == null)
            {
                var prefab = GetPrefab(resourcesId);

                if (prefab == null)
                {
                    return null;
                }

                target = GameObject.Instantiate(prefab, transform);
                _dictResourcesPool[resourcesId].Add(target);
                target.gameObject.name = resourcesId;
                target.Created();
            }

            target.Init(model);

            target.Use();

            if (model != null && !string.IsNullOrEmpty(model.Id))
            {
                _idUsingResources.Add(model.Id, target);
            }


            return target;
        }

        public void FreeResources(LOADTYPE resources)
        {
            var id = resources.GetId();

            if (!string.IsNullOrEmpty(id))
            {
                _idUsingResources.Remove(id);
            }

            resources.Free();

            resources.transform.SetParent(_defaultRoot, false);
            resources.transform.position = Vector3.zero;
        }

        public void FreeResources(List<LOADTYPE> enumerator)
        {
            foreach (var ele in enumerator)
            {
                FreeResources(ele);
            }

            enumerator.Clear();
        }

        public LOADTYPE UnRefIdUsing(string id)
        {
            if (_idUsingResources.TryGetValue(id, out var resource))
            {
                _idUsingResources.Remove(id);
                return resource;
            }

            return null;
        }

        public LOADTYPE GetInstance(string id)
        {
            if (_idUsingResources.TryGetValue(id, out var resource))
            {
                return resource;
            }

            return default;
        }

        protected LOADTYPE GetPrefab(string resourcesId)
        {
            if (_dictResourcesLoaded.TryGetValue(resourcesId, out var characterView))
            {
                return characterView;
            }

            var path = Path.Combine(ResourcesPath(), resourcesId);
            var loading = Resources.Load<LOADTYPE>(path);

            if (loading != null)
            {
                _dictResourcesLoaded.Add(resourcesId, loading);
                return loading;
            }

            Debug.LogError($"WTF resourcesId gì lạ zậy {resourcesId}");

            return null;
        }


    }
}
