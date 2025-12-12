using System.Collections.Generic;
using DG.Tweening;
using Hawky.EventObserver;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hawky.Loading
{
    public class LoadingDisplay : EventBehaviour
    {
        [SerializeField] private string _loadingId;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private Image _slideImage;
        [SerializeField] private Slider _slider;

        [SerializeField] private RectTransform _slideRect;
        [SerializeField] private float _minSize;
        [SerializeField] private float _sizeRect;

        public void SetLoadingId(string id)
        {
            _loadingId = id;
        }

        protected virtual void OnLoadingBegin(LoadingBeginEvent data)
        {
            // Set tất cả Value về giá trị ban đầu
            if (_progressText != null)
                _progressText.text = "0%";

            if (_loadingText != null)
                _loadingText.text = data.text;

            if (_slideImage != null)
                _slideImage.fillAmount = 0;

            if (_slider != null)
                _slider.value = 0;

            if (_slideRect != null)
            {
                _slideRect.DOKill();
                _slideRect.sizeDelta = new Vector2(0, _slideRect.rect.size.y);
            }
        }

        protected virtual void OnLoadingUpdate(LoadingUpdateEvent data)
        {
            float progress = (float)data.currentPoint / data.totalPoint * 100f;

            if (_progressText != null)
                _progressText.text = $"{progress}%";

            if (_loadingText != null)
                _loadingText.text = data.text;

            if (_slideImage != null)
                _slideImage.fillAmount = (float)data.currentPoint / data.totalPoint;

            if (_slider != null)
                _slider.value = (float)data.currentPoint / data.totalPoint;

            if (_slideRect != null)
            {
                _slideRect.DOSizeDelta(new Vector2(GenSlideRectPercent(data.currentPoint, data.totalPoint), _slideRect.rect.size.y), 0.3f);
            }
        }

        protected virtual void OnLoadingEnd(LoadingEndEvent data)
        {
            if (_progressText != null)
                _progressText.text = "100%";

            if (_loadingText != null)
                _loadingText.text = "";

            if (_slideImage != null)
                _slideImage.fillAmount = 1;

            if (_slider != null)
                _slider.value = 1;

            if (_slideRect != null)
            {
                _slideRect.DOKill();
                _slideRect.sizeDelta = new Vector2(GenSlideRectPercent(1f, 1f), _slideRect.rect.size.y);
            }
        }

        public void ResetAll()
        {
            // Set tất cả UI về giá trị ban đầu
            if (_progressText != null)
                _progressText.text = "0%";

            if (_loadingText != null)
                _loadingText.text = "";

            if (_slideImage != null)
                _slideImage.fillAmount = 0;

            if (_slider != null)
                _slider.value = 0;
        }

        private float GenSlideRectPercent(float progress, float target)
        {
            return Mathf.Clamp(progress * _sizeRect / target, _minSize, _sizeRect);
        }

        #region Event

        public override void OnEvent(string eventName, EventBase data)
        {
            switch (eventName)
            {
                case EventName.LOADING_BEGIN:
                case EventName.LOADING_UPDATE:
                case EventName.LOADING_END:
                    var loadingEvent = data as LoadingEventBase;
                    if (loadingEvent.loadingId == _loadingId)
                    {
                        switch (eventName)
                        {
                            case EventName.LOADING_BEGIN:
                                OnLoadingBegin(data as LoadingBeginEvent);
                                break;
                            case EventName.LOADING_UPDATE:
                                OnLoadingUpdate(data as LoadingUpdateEvent);
                                break;
                            case EventName.LOADING_END:
                                OnLoadingEnd(data as LoadingEndEvent);
                                break;
                        }
                    }
                    break;
            }
        }

        protected override void EventList(List<string> eventList)
        {
            eventList.Add(EventName.LOADING_BEGIN);
            eventList.Add(EventName.LOADING_UPDATE);
            eventList.Add(EventName.LOADING_END);
        }

        #endregion
    }
}
