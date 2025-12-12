using Hawky.EventObserver;
using Hawky.MyCoroutine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Accessibility;

namespace Hawky
{
    public class Singleton<T> where T : class, new()
    {
        private static readonly object _lock = new object();
        private static T _ins;

        public static T Ins
        {
            get
            {
                if (_ins == null)
                {
                    lock (_lock)
                    {
                        if (_ins == null)
                        {
                            _ins = new T();
                        }
                    }
                }
                return _ins;
            }
        }

        protected Singleton() { }
    }

    #region Runtime Singleton

    public class SingletonManager : Singleton<SingletonManager>
    {
        private Dictionary<string, ISingleton> _dictSingleton;
        private bool _init = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeLoadScene()
        {
            SingletonManager.Ins.Init();
        }

        public T GetSingleton<T>() where T : ISingleton
        {
            if (_dictSingleton == null)
            {
                return default(T);
            }

            var type = typeof(T);
            if (_dictSingleton.TryGetValue(type.FullName, out var single))
            {
                return (T)single;
            }

            return default(T);
        }

        public T FindFirst<T>()
        {
            if (_dictSingleton == null)
            {
                return default(T);
            }
            foreach (var singleton in _dictSingleton.Values)
            {
                if (singleton is T)
                {
                    return (T)singleton;
                }
            }

            return default(T);
        }

        public List<T> FindAll<T>()
        {
            var result = new List<T>();

            if (_dictSingleton != null)
            {
                foreach (var singleton in _dictSingleton.Values)
                {
                    if (singleton is T t)
                    {
                        result.Add(t);
                    }
                }
            }

            return result;
        }

        private void Init()
        {
            if (_init)
            {
                return;
            }
            _init = true;

            var allType = TypeUtilities.FindAllDerivedTypesInDomain<ISingleton>();
            _dictSingleton = new Dictionary<string, ISingleton>();

            foreach (var type in allType)
            {
                var fullName = type.FullName;
                var instance = Activator.CreateInstance(type) as ISingleton;

                _dictSingleton.Add(fullName, instance);
                if (instance is IAwakeBehaviour mono)
                {
                    try
                    {
                        mono.Awake();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            foreach (var singleton in _dictSingleton)
            {
                if (singleton.Value is IAllSingletonAwakeComplete mono)
                {
                    try
                    {
                        mono.OnAllSingletonInitComplete();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            foreach (var singleton in _dictSingleton)
            {
                if (singleton.Value is IStartBehaviour mono)
                {
                    try
                    {
                        mono.Start();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
            }

            CoroutineManager.Ins.Start(AfterInit());
        }

        IEnumerator AfterInit()
        {
            yield return null;
            var allUpdateBehaviours = _dictSingleton.ToList().FindAll(x => x.Value is IUpdateBehaviour);
            while (true)
            {
                foreach (var behaviour in allUpdateBehaviours)
                {
                    var updateBehaviour = behaviour.Value as IUpdateBehaviour;
                    try
                    {
                        updateBehaviour.Update();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                yield return null;
            }
        }
    }

    public interface ISingleton
    {

    }

    public abstract class RuntimeSingleton<T> : ISingleton where T : ISingleton
    {
        public static T Ins => SingletonManager.Ins.GetSingleton<T>();
    }

    public abstract class RuntimeEventSingleton<T> : RuntimeSingleton<T>, IRegister, IStartBehaviour where T : ISingleton
    {
        protected virtual void EventList(List<string> eventList)
        {

        }

        public virtual void OnEvent(string eventId, EventBase data)
        {

        }

        public virtual void Start()
        {
            var eventList = new List<string>();

            EventList(eventList);

            eventList.ForEach(x => EventObs.Ins.AddRegister(x, this));
        }
    }

    public interface IMonobehaviour : IAwakeBehaviour, IAllSingletonAwakeComplete, IStartBehaviour, IUpdateBehaviour
    {

    }

    // được gọi ngay sau khi đối tượng được tạo
    public interface IAwakeBehaviour
    {
        void Awake();
    }

    // được gọi sau khi tất cả Awake được gọi
    public interface IAllSingletonAwakeComplete
    {
        void OnAllSingletonInitComplete();
    }

    // được gọi sau khi tất cả AfterAwake được goi
    public interface IStartBehaviour
    {
        void Start();
    }

    // được gọi mỗi frame
    public interface IUpdateBehaviour
    {
        void Update();
    }


    #endregion

    #region MonoSingleton

    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed on application quit. Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError($"[Singleton] Something went really wrong - there should never be more than 1 singleton! Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = $"(Singleton) {typeof(T).ToString()}";

                            DontDestroyOnLoad(singleton);

                            Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log($"[Singleton] Using instance already created: {_instance.gameObject.name}");
                        }
                    }

                    return _instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
    }

    public class InitMonoSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        public void Init()
        {

        }
    }

    #endregion

}
