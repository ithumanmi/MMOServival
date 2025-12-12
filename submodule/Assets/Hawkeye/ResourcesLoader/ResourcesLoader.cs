using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hawkeye.ResourcesLoader
{
    public class ResourcesPool : MonoBehaviour
    {
        [HideInInspector]
        public bool free = true;

        public void Created()
        {
            OnCreated();
        }

        public void Free()
        {
            OnFree();
            free = true;
            gameObject.SetActive(false);
        }

        public void Use()
        {
            free = false;
            gameObject.SetActive(true);
            OnUse();
        }

        protected virtual void OnCreated()
        {

        }

        protected virtual void OnFree()
        {

        }

        protected virtual void OnUse()
        {

        }
    }

    public abstract class ResourcesLoader<T, LOADTYPE> : Singleton<T>  where LOADTYPE : ResourcesPool where T : class, new()
    {
        private Dictionary<string, LOADTYPE> dictResourcesLoaded = new Dictionary<string, LOADTYPE>();

        private Dictionary<string, List<LOADTYPE>> dictResourcesPool = new Dictionary<string, List<LOADTYPE>>();

        protected abstract string ResourcesPath();

        public LOADTYPE LoadResources(string resourcesId, Transform transform = null)
        {
            if (!dictResourcesPool.TryGetValue(resourcesId, out var hehe))
            {
                dictResourcesPool.Add(resourcesId, new List<LOADTYPE>());
            }

            LOADTYPE target = null;
            foreach (var view in dictResourcesPool[resourcesId])
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
                dictResourcesPool[resourcesId].Add(target);
                target.gameObject.name = resourcesId;
                target.Created();
            }

            target.Use();

            return target;
        }

        public void FreeResources(LOADTYPE resources)
        {
            resources.Free();
        }

        private LOADTYPE GetPrefab(string resourcesId)
        {
            if (dictResourcesLoaded.TryGetValue(resourcesId, out var characterView))
            {
                return characterView;
            }

            var path = Path.Combine(ResourcesPath(), resourcesId);
            var loading = Resources.Load<LOADTYPE>(path);

            if (loading != null)
            {
                dictResourcesLoaded.Add(resourcesId, loading);
                return loading;
            }

            throw new ArgumentException($"WTF resourcesId gì lạ zậy {resourcesId}");
        }
    }
}
