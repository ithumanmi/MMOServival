using Hawky.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenUrlButton : MonoBehaviour
    {
        [SerializeField] private string _url;

        public string Url => _url;

        private void Awake()
        {
            var button = GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundId.Click_Button);
                Application.OpenURL(_url);
            });
        }

        public void SetUrl(string url)
        {
            _url = url;
        }
    }
}
