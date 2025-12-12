using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hawky.UI
{
    public class HoldUIHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action OnHoldBegin;
        public Action<float> OnHold;
        public Action OnHoldEnd;

        private bool isHolding = false;
        private float holdDuration = 0f;

        void Update()
        {
            if (isHolding)
            {
                holdDuration += Time.deltaTime;
                OnHold?.Invoke(holdDuration);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            holdDuration = 0f;
            OnHoldBegin?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            holdDuration = 0f;
            OnHoldEnd?.Invoke();
        }
    }
}