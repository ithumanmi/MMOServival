using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Hawky.UI
{
    public enum ScrollDerection
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class ScrollDirectionDetector : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        private Vector2 _startPoint;
        private RectTransform _rectTransform;

        public UnityEvent<ScrollDerection> onScrollDerection;

        private bool _scrolled;
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_scrolled)
            {
                return;
            }
            Vector2 currentPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out currentPosition);

            Vector2 direction = currentPosition - _startPoint;

            if (direction.magnitude != 0)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                    {
                        onScrollDerection?.Invoke(ScrollDerection.Right);
                    }
                    else
                    {
                        onScrollDerection?.Invoke(ScrollDerection.Left);
                    }
                }
                else
                {
                    if (direction.y > 0)
                    {
                        onScrollDerection?.Invoke(ScrollDerection.Up);
                    }
                    else
                    {
                        onScrollDerection?.Invoke(ScrollDerection.Down);
                    }
                }
                _scrolled = true;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _scrolled = false;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _startPoint);
        }
    }
}
