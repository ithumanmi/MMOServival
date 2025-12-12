using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Hawky.SearchableDropdown
{
    public class SearchableDropDown : MonoBehaviour
    {
        [SerializeField] private Button _blockerButton;
        [SerializeField] private GameObject _buttonsPrefab = null;
        [SerializeField] private int _maxScrollRectSize = 180;
        [SerializeField] private List<string> _avlOptions = new List<string>();

        private Button _ddButton = null;
        private TMP_InputField _inputField = null;
        private ScrollRect _scrollRect = null;
        private Transform _content = null;
        private RectTransform _scrollRectTrans;
        private bool _isContentHidden = true;
        private List<Button> _initializedButtons = new List<Button>();

        public delegate void OnValueChangedDel(string val);
        public OnValueChangedDel OnValueChangedEvt;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            _ddButton = this.GetComponentInChildren<Button>();
            _scrollRect = this.GetComponentInChildren<ScrollRect>();
            _inputField = this.GetComponentInChildren<TMP_InputField>();
            _scrollRectTrans = _scrollRect.GetComponent<RectTransform>();
            _content = _scrollRect.content;

            //blocker is a button added and scaled it to screen size so that we can close the dd on clicking outside
            _blockerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            _blockerButton.gameObject.SetActive(false);
            _blockerButton.transform.SetParent(this.GetComponentInParent<Canvas>().transform);

            _blockerButton.onClick.AddListener(OnBlockerButtClick);
            _ddButton.onClick.AddListener(OnDDButtonClick);
            _scrollRect.onValueChanged.AddListener(OnScrollRectvalueChange);
            _inputField.onValueChanged.AddListener(OnInputvalueChange);
            _inputField.onEndEdit.AddListener(OnEndEditing);

            AddItemToScrollRect(_avlOptions);
        }

        public string GetValue()
        {
            return _inputField.text;
        }

        public void ResetDropDown()
        {
            _inputField.text = string.Empty;

        }

        public void AddItemToScrollRect(List<string> options)
        {
            foreach (var option in options)
            {
                var buttObj = Instantiate(_buttonsPrefab, _content);
                buttObj.GetComponentInChildren<TMP_Text>().text = option;
                buttObj.name = option;
                buttObj.SetActive(true);
                var butt = buttObj.GetComponent<Button>();
                butt.onClick.AddListener(delegate { OnItemSelected(buttObj); });
                _initializedButtons.Add(butt);
            }
            ResizeScrollRect();
            _scrollRect.gameObject.SetActive(false);
        }

        private void OnEndEditing(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                Debug.Log("No value entered ");
                return;
            }
            StartCoroutine(CheckIfValidInput(arg));
        }

        IEnumerator CheckIfValidInput(string arg)
        {
            yield return new WaitForSeconds(1);
            if (!_avlOptions.Contains(arg))
            {
                _inputField.text = string.Empty;
            }
            OnValueChangedEvt?.Invoke(_inputField.text);
        }

        private void ResizeScrollRect()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_content.transform);
            var length = _content.GetComponent<RectTransform>().sizeDelta.y;

            _scrollRectTrans.sizeDelta = length > _maxScrollRectSize ? new Vector2(_scrollRectTrans.sizeDelta.x,
                _maxScrollRectSize) : new Vector2(_scrollRectTrans.sizeDelta.x, length + 5);
        }

        private void OnInputvalueChange(string arg0)
        {
            if (!_avlOptions.Contains(arg0))
            {
                FilterDropdown(arg0);
            }
        }

        public void FilterDropdown(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                foreach (var button in _initializedButtons)
                    button.gameObject.SetActive(true);
                ResizeScrollRect();
                _scrollRect.gameObject.SetActive(false);
                return;
            }

            var count = 0;
            foreach (var button in _initializedButtons)
            {
                if (!button.name.ToLower().Contains(input.ToLower()))
                {
                    button.gameObject.SetActive(false);
                }
                else
                {
                    button.gameObject.SetActive(true);
                    count++;
                }
            }

            SetScrollActive(count > 0);
            ResizeScrollRect();
        }

        private void OnScrollRectvalueChange(Vector2 arg0)
        {

        }

        private void OnItemSelected(GameObject obj)
        {
            _inputField.text = obj.name;
            foreach (var button in _initializedButtons)
                button.gameObject.SetActive(true);
            _isContentHidden = false;
            OnDDButtonClick();
            StopAllCoroutines();
            StartCoroutine(CheckIfValidInput(obj.name));
        }

        private void OnDDButtonClick()
        {
            if (GetActiveButtons() <= 0)
                return;
            ResizeScrollRect();
            SetScrollActive(_isContentHidden);
        }
        private void OnBlockerButtClick()
        {
            SetScrollActive(false);
        }

        private void SetScrollActive(bool status)
        {
            _scrollRect.gameObject.SetActive(status);
            _blockerButton.gameObject.SetActive(status);
            _isContentHidden = !status;
            _ddButton.transform.localScale = status ? new Vector3(1, -1, 1) : new Vector3(1, 1, 1);
        }

        private float GetActiveButtons()
        {
            var count = _content.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);
            var length = _buttonsPrefab.GetComponent<RectTransform>().sizeDelta.y * count;
            return length;
        }
    }
}
