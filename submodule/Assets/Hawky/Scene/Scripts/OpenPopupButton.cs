using Hawky.Scene;
using Hawky.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenPopupButton : MonoBehaviour
    {
        [SerializeField] private string _popupName;

        private void Awake()
        {
            var button = GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundId.Click_Button);
                SceneManager.Instance.OpenPopup(_popupName);
            });
        }

        public void SetPopupName(string popupName)
        {
            this._popupName = popupName;
        }
    }
}
