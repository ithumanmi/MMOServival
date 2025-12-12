using Hawkeye.Backkey;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace Hawkeye.Scene
{
    public class SceneManager : MonoSingleton<SceneManager>, IKeyBack
    {
        private Stack<Controller> _stack = new Stack<Controller>();
        private Stack<Controller> _popupStack = new Stack<Controller>();

        private Dictionary<string, Controller> _controllersByName = new Dictionary<string, Controller>();

        private ISceneLoader _loader;

        public int CurrentPopupCount => _popupStack.Count;

        private void Awake()
        {
            _loader = new SceneLoader();

            BackKeyManager.Instance.AddListener(this);
        }

        public void SetSceneLoader(ISceneLoader sceneLoader)
        {
            _loader = sceneLoader;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            BackKeyManager.Instance?.RemoveListener(this);
        }

        public void OnLoaded(Controller controller)
        {
            this._controllersByName[controller.SceneName()] = controller;
        }

        /// <summary>
        /// Mở một Scene mới đề lên scene hiện tại
        /// Đồng thời đóng tắt cả Popup đang ở scene hiện tại
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public async UniTask OpenScene(string sceneName)
        {
            CloseAllPopupPrivate();

            Controller currentController = null;
            if (_stack.Count > 0)
            {
                currentController = _stack.Peek();
            }

            await OpenNewSceneTask(sceneName);

            if (currentController != null)
            {
                currentController.Hide();
            }
        }

        public async UniTask OpenSceneAfterLoadScene(string sceneNameLoad, string sceneNameOpen)
        {
            CloseAllPopupPrivate();

            Controller currentController = null;
            if (_stack.Count > 0)
            {
                currentController = _stack.Peek();
            }

            var controllerLoaded = await OpenNewController(sceneNameLoad);

            _stack.Push(controllerLoaded);

            controllerLoaded.SetOrder(_stack.Count + 1);

            controllerLoaded.Hide();


            await OpenNewSceneTask(sceneNameOpen);

            if (currentController != null)
            {
                currentController.Hide();
            }
        }

        public async UniTask OpenPopup(string popupName)
        {
            await OpenNewPopupTask(popupName);
        }

        /// <summary>
        /// Đóng scene hiện tại và mở scene mới ngay lập tức (không gọi callBack)
        /// </summary>
        /// <param name="openSceneName"></param>
        /// <returns></returns>
        public async UniTask CloseAndOpen(string openSceneName)
        {
            CloseAllPopupPrivate();

            string controllerName = null;
            if (_stack.Count > 0)
            {
                controllerName = _stack.Peek().SceneName();
            }

            await OpenNewSceneTask(openSceneName);

            if (!string.IsNullOrEmpty(controllerName))
            {
                Close(controllerName);
            }
        }

        /// <summary>
        /// Tắt một Scene, nếu scene này đang trên cùng thì mở scene gần nhất lên
        /// </summary>
        /// <param name="sceneName"></param>
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

        /// <summary>
        /// Tắt một Popup, đồng thời gọi OnPopupTopView lên scene hiện tại
        /// </summary>
        /// <param name="popupName"></param>
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

        private async UniTask OpenNewSceneTask(string sceneName)
        {
            Controller nextController = await OpenNewController(sceneName);

            _stack.Push(nextController);

            nextController.Show();

            nextController.SetOrder(_stack.Count + 1);
        }

        private async UniTask OpenNewPopupTask(string sceneName)
        {
            Controller nextController = await OpenNewController(sceneName); 

            _popupStack.Push(nextController);

            nextController.Show();

            nextController.SetOrder(10 + _popupStack.Count * 10 + 1);
        }

        private async UniTask<Controller> OpenNewController(string sceneName)
        {
            Controller nextController = await LoadNewController(sceneName);

            nextController.Active();

            await UniTask.WaitUntil(() => nextController.IsAsyncComplete);

            return nextController;
        }

        private async UniTask<Controller> LoadNewController(string sceneName)
        {
            if (_controllersByName.TryGetValue(sceneName, out var nextController) == false)
            {
                await _loader.LoadScene(sceneName);

                nextController = this._controllersByName[sceneName];
            }

            return nextController;
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
    }
}
