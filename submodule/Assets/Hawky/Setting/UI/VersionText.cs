using TMPro;
using UnityEngine;

namespace Hawky.Setting.UI
{
    public class VersionText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void Start()
        {
            _text.text = $"v{Application.version}";
        }

        private void OnValidate()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }
        }
    }
}
