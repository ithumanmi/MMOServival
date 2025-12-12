using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Hawkeye.Scene
{
    public interface IController
    {
        void LockUserInterface();
        void UnlockUserInterface();

    }
    public abstract class Controller : MonoBehaviour, IController
    {
        public bool IsAsyncComplete;

        [SerializeField] private Canvas[] canvas;
        [SerializeField] private Canvas canvasShield;
        [SerializeField] private CanvasGroup[] allCanvas;

        private void Awake()
        {
            SceneManager.Instance.OnLoaded(this);

            OnAwake();
        }

        public abstract string SceneName();

        public void Active()
        {
            Debug.Log($"[Scene] Active Scene: {SceneName()}");
            this.gameObject.SetActive(true);

            OnActive();
        }

        public void Show()
        {
            Debug.Log($"[Scene] Show Scene: {SceneName()}");

            OnShown();
        }

        public void TopView()
        {
            Debug.Log($"[Scene] TopView Scene: {SceneName()}");
            this.gameObject.SetActive(true);

            this.OnTopView();
        }

        public void PopupTopView()
        {
            Debug.Log($"[Scene] Popup TopView Scene: {SceneName()}");

            this.OnPopupTopView();
        }

        public void Hide()
        {
            Debug.Log($"[Scene] Hide Scene: {SceneName()}");
            this.gameObject.SetActive(false);

            this.OnHidden();
        }

        public void Close()
        {
            Debug.Log($"[Scene] Close Scene: {SceneName()}");
            this.gameObject.SetActive(false);

            this.OnClose();
        }

        public bool KeyBack()
        {
            if (!lockValueTemp)
            {
                return this.OnKeyBack();
            }

            return true;
        }

        public void SetOrder(int dep)
        {
            if (canvasShield != null)
            {
                canvasShield.sortingOrder = dep;
            }

            if (canvas != null)
            {
                var id = 1;
                foreach (var canvas in canvas)
                {
                    canvas.sortingOrder = dep + id;
                    id++;
                }
            }
        }

        bool lockValueTemp;
        public void LockUserInterface()
        {
            if (lockValueTemp)
            {
                return;
            }

            if (this.allCanvas != null) 
            { 
                foreach (var canvas in this.allCanvas)
                {
                    canvas.blocksRaycasts = false;
                }
            }

            lockValueTemp = true;
        }

        public void UnlockUserInterface()
        {
            if (!lockValueTemp)
            {
                return;
            }

            if (this.allCanvas != null)
            {
                foreach (var canvas in this.allCanvas)
                {
                    canvas.blocksRaycasts = true;
                }
            }

            lockValueTemp = false;
        }

        /// <summary>
        /// Còn phải xem nữa ????
        /// </summary>
        protected virtual void OnAwake()
        {

        }

        /// <summary>
        /// Được gọi khi Scene/Popup trước đó được tắt đi và scene hiện tại tự động bật lên
        /// </summary>
        protected virtual void OnTopView()
        {

        }

        /// <summary>
        /// Được gọi khi Popup trước đó được tắt đi
        /// Nếu Popup được tắt gián tiếp (Bởi lệnh mở scene khác thì sẽ không vào CallBack này, sử dung OnTopView thay thế)
        /// </summary>
        protected virtual void OnPopupTopView()
        {

        }

        /// <summary>
        /// Được gọi khi scene khác nhảy lên stack
        /// đồng thời scene hiện tại sẽ ẩn đi hoàn toàn
        /// </summary>
        protected virtual void OnHidden()
        {

        }

        /// <summary>
        /// Được gọi khi scene khác nhảy lên stack
        /// Đồng thời scene này được đóng hẵn
        /// </summary>
        protected virtual void OnClose()
        {

        }
        /// <summary>
        /// Được gọi khi scene này được gọi chủ động (đầu stack)
        /// </summary>
        protected virtual void OnActive()
        {

        }
        /// <summary>
        /// Gọi sau OnActive hoặc OnTopView hoặc OnPopupTopView
        /// </summary>
        protected virtual void OnShown()
        {

        }

        protected virtual bool OnKeyBack()
        {
            SceneManager.Instance.Close();

            return true;
        }
    }
}
