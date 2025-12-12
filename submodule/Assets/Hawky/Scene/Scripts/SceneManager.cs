using Hawky.Backkey;
using Hawky.MyCoroutine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawky.Scene
{
    public class SceneManager : MonoSingleton<SceneManager>, IKeyBack
    {
        private Stack<Controller> _stack = new Stack<Controller>();
        private Stack<Controller> _popupStack = new Stack<Controller>();

        private Dictionary<string, Controller> _controllersByName = new Dictionary<string, Controller>();

        private ISceneLoader _loader;

        public int CurrentPopupCount => _popupStack.Count;

        public Controller CurrentScene => _stack.Peek();

        private void Awake()
        {
            _loader = new SceneLoader();
            BackKeyManager.Ins?.AddListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            BackKeyManager.Ins?.RemoveListener(this);
        }

        public void OnLoaded(Controller controller)
        {
            this._controllersByName[controller.SceneName()] = controller;
        }
        public bool OnKeyBack()
        {
            if (this._popupStack.Count > 0)
            {
                return _popupStack.Peek().KeyBack();
            }

            if (this._stack.Count <= 1)
            {
                return false;
            }

            return _stack.Peek().KeyBack();
        }

        #region Public Method

        public void SetSceneLoader(ISceneLoader sceneLoader)
        {
            _loader = sceneLoader;
        }

        public bool HaveAnyPopup()
        {
            return _popupStack.Count > 0;
        }

        public void OpenScene(string sceneName)
        {
            CoroutineManager.Ins?.Start(OpenScenePrivate(sceneName));
        }

        public void OpenPopup(string sceneName)
        {
            CoroutineManager.Ins?.Start(OpenPopupPrivate(sceneName));
        }

        public void OpenSceneAfterLoadScene(string sceneNameLoad, string sceneNameOpen)
        {
            CoroutineManager.Ins?.Start(OpenSceneAfterLoadScenePrivate(sceneNameLoad, sceneNameOpen));
        }

        public void CloseAndOpen(string sceneName)
        {
            CoroutineManager.Ins?.Start(CloseAndOpenPrivate(sceneName));
        }

        public void Close(string sceneName = null)
        {
            if (string.IsNullOrEmpty(sceneName) && _popupStack.Count > 0)
            {
                ClosePopup();
                return;
            }

            if (_stack.Count <= 1)
            {
                Debug.Log("Can't Close Scene");
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Close(_stack.Peek().SceneName());
                return;
            }

            var currentController = _stack.Peek();

            Controller targetController = null;

            var newStack = new Stack<Controller>();

            while (_stack.Count > 0)
            {
                var checking = _stack.Pop();

                if (checking.SceneName() == sceneName)
                {
                    targetController = checking;

                    break;
                }
                else
                {
                    // lưu vào stack tạm
                    newStack.Push(checking);
                }
            }

            while (newStack.Count > 0)
            {
                _stack.Push(newStack.Pop());
            }

            if (targetController == null)
            {
                return;
            }

            targetController.Hide();
            targetController.Close();

            if (currentController == targetController)
            {
                var newTopController = _stack.Peek();

                newTopController.TopView();
            }
        }
        public void ClosePopup(string popupName = null)
        {
            if (string.IsNullOrEmpty(popupName))
            {
                ClosePopup(_popupStack.Peek().SceneName());
                return;
            }

            if (_popupStack.Count == 0)
            {
                Debug.Log("No Popup to Close");
                return;
            }

            var currentController = _popupStack.Peek();

            Controller targetController = null;

            var newStack = new Stack<Controller>();

            while (_popupStack.Count > 0)
            {
                var checking = _popupStack.Pop();

                if (checking.SceneName() == popupName)
                {
                    targetController = checking;

                    break;
                }
                else
                {
                    newStack.Push(checking);
                }
            }

            while (newStack.Count > 0)
            {
                _popupStack.Push(newStack.Pop());
            }

            if (targetController == null)
            {
                return;
            }

            targetController.Hide();
            targetController.Close();

            if (currentController == targetController)
            {
                if (_popupStack.Count > 0)
                {
                    var newTopController = _popupStack.Peek();

                    newTopController.TopView();
                }
            }

            if (_popupStack.Count == 0)
            {
                if (_stack.Count > 0)
                {
                    var newTopController = _stack.Peek();

                    newTopController.PopupTopView();
                }
            }
        }
        public void CloseAllPopup()
        {
            CloseAllPopupPrivate(true);
        }

        #endregion

        #region Private Method
        private IEnumerator OpenScenePrivate(string sceneName)
        {
            CloseAllPopupPrivate();

            Controller currentController = null;
            if (_stack.Count > 0)
            {
                currentController = _stack.Peek();
            }

            yield return OpenNewSceneTask(sceneName);

            if (currentController != null)
            {
                currentController.Hide();
            }
        }

        private IEnumerator OpenSceneAfterLoadScenePrivate(string sceneNameLoad, string sceneNameOpen)
        {
            CloseAllPopupPrivate();

            Controller currentController = null;
            if (_stack.Count > 0)
            {
                currentController = _stack.Peek();
            }

            Controller controllerLoaded = null;

            yield return OpenNewController(sceneNameLoad, (controller) =>
            {
                controllerLoaded = controller;
            });

            _stack.Push(controllerLoaded);

            controllerLoaded.SetOrder(_stack.Count + 1);

            controllerLoaded.Hide();


            yield return OpenNewSceneTask(sceneNameOpen);

            if (currentController != null)
            {
                currentController.Hide();
            }
        }

        private IEnumerator OpenPopupPrivate(string popupName)
        {
            yield return OpenNewPopupTask(popupName);
        }

        private IEnumerator CloseAndOpenPrivate(string openSceneName)
        {
            CloseAllPopupPrivate();

            string controllerName = null;
            if (_stack.Count > 0)
            {
                controllerName = _stack.Peek().SceneName();
            }

            yield return OpenNewSceneTask(openSceneName);

            if (!string.IsNullOrEmpty(controllerName))
            {
                Close(controllerName);
            }
        }

        private void CloseAllPopupPrivate(bool callOnPopupTopView = false)
        {
            if (_popupStack.Count == 0)
            {
                Debug.Log("No Popup to Close");
                return;
            }

            while (_popupStack.Count != 0)
            {
                var controller = _popupStack.Pop();

                controller.Hide();
                controller.Close();
            }

            if (callOnPopupTopView)
            {
                if (_stack.Count > 0)
                {
                    var newTopController = _stack.Peek();

                    newTopController.PopupTopView();
                }
            }
        }

        private IEnumerator OpenNewSceneTask(string sceneName)
        {
            Controller nextController = null;

            yield return OpenNewController(sceneName, (controller) =>
            {
                nextController = controller;
            });

            _stack.Push(nextController);

            nextController.Show();

            nextController.SetOrder(_stack.Count + 1);
        }

        private IEnumerator OpenNewPopupTask(string sceneName)
        {
            Controller nextController = null;

            yield return OpenNewController(sceneName, (controller) =>
            {
                nextController = controller;
            });

            _popupStack.Push(nextController);

            nextController.Show();

            nextController.SetOrder(10 + _popupStack.Count * 10 + 1);
        }

        private IEnumerator OpenNewController(string sceneName, Action<Controller> onComplete)
        {
            Controller nextController = null;

            yield return LoadNewController(sceneName, (controller) =>
            {
                nextController = controller;
            });

            nextController.Active();

            onComplete?.Invoke(nextController);
        }

        private IEnumerator LoadNewController(string sceneName, Action<Controller> onComplete)
        {
            if (_controllersByName.TryGetValue(sceneName, out var nextController) == false)
            {
                yield return _loader.LoadScene(sceneName);

                nextController = this._controllersByName[sceneName];
            }

            nextController.gameObject.name = sceneName;
            onComplete?.Invoke(nextController);
        }

        #endregion 
    }
}
