using Hawky.Math;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.FakeLeaderBoard.UI
{
    public class LBItemView : MonoBehaviour
    {
        [SerializeField] private GameObject _selfObject;
        [SerializeField] private GameObject _normalObject;
        [SerializeField] private TMP_Text _idText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _pointText;
        [SerializeField] private Transform _zoomRoot;
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private GameObject _firstObject;
        [SerializeField] private GameObject _secondObject;
        [SerializeField] private GameObject _thirdObject;

        protected Data _data;

        public int Id => _data.id;

        public LayoutElement Layout => _layoutElement;

        private void Awake()
        {
            if (_layoutElement == null)
            {
                _layoutElement = gameObject.AddComponent<LayoutElement>();
            }
        }

        public void SetData(Data data)
        {
            _data = data;

            _idText.text = _data.id.ToString();
            _nameText.text = _data.userName;
            _pointText.text = _data.point.ToString("N0");

            _selfObject.gameObject.SetActive(_data.isSelf);
            _normalObject.gameObject.SetActive(!_data.isSelf);

            // replace rank

            _idText.gameObject.SetActive(data.id > 3);
            _firstObject.gameObject.SetActive(data.id == 1);
            _secondObject.gameObject.SetActive(data.id == 2);
            _thirdObject.gameObject.SetActive(data.id == 3);

            OnSetData();
        }

        protected virtual void OnSetData()
        {

        }

        public void ZoomIn()
        {
            StartCoroutine(ZoomInAnimation());
        }

        public void ZoomOut()
        {
            StartCoroutine(ZoomOutAnimation());
        }

        private IEnumerator ZoomInAnimation()
        {
            var from = 1;
            var to = 1.3f;
            float counter = 0f;
            var timer = 0.3f;

            while (counter < timer)
            {
                counter = Mathf.Clamp(counter + Time.deltaTime, 0, timer);

                var scaleValue = EaseCalculator.GetValue(from, to, timer, counter, EaseType.InOutSine);
                _zoomRoot.localScale = Vector3.one * scaleValue;

                yield return null;
            }
        }

        private IEnumerator ZoomOutAnimation()
        {
            var from = 1.3f;
            var to = 1f;
            float counter = 0f;
            var timer = 0.3f;

            while (counter < timer)
            {
                counter = Mathf.Clamp(counter + Time.deltaTime, 0, timer);

                var scaleValue = EaseCalculator.GetValue(from, to, timer, counter, EaseType.InOutSine);
                _zoomRoot.localScale = Vector3.one * scaleValue;

                yield return null;
            }
        }

        public void ModifyId(int value)
        {
            _data.id += value;

            _idText.text = _data.id.ToString();
        }

        public void SetId(int value)
        {
            _data.id = value;

            _idText.text = _data.id.ToString();
        }

        public void SetPoint(long value)
        {
            _data.point = value;

            _pointText.text = _data.point.ToString("N0");
        }
    }

    public class Data
    {
        public int id;
        public string userName;
        public long point;
        public bool isSelf;
    }
}
