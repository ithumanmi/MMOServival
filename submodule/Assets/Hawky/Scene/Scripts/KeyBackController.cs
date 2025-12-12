using UnityEngine;
using UnityEngine.UI;

namespace Hawky.Scene
{
    public abstract class KeyBackController : PopupController
    {
        [SerializeField] private bool _closeOnEscape = true;
        [SerializeField] private Button _backButton;

        protected override void OnAwake()
        {
            base.OnAwake();

            _backButton?.onClick.AddListener(OnClickBack);
        }

        protected virtual void OnClickBack()
        {
            SceneManager.Instance.Close();
        }

        protected override bool OnKeyBack()
        {
            if (_closeOnEscape)
            {
                base.OnKeyBack();
            }

            return true;
        }
    }
}
