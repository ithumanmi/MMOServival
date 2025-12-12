using UnityEngine.UI;

namespace Hawky.UI
{
    public abstract class CustomButtom : CustomComponent<Button>
    {
        protected Button _button => _component;
        private void Awake()
        {
            var btn = _button;

            if (btn != null)
            {
                btn.onClick.AddListener(OnTap);
            }
        }
        protected abstract void OnTap();
    }
}
