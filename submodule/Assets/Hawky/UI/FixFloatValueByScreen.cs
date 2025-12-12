using Hawky.EventObserver;
using UnityEngine;
using UnityEngine.Events;

namespace Hawky.UI
{
    public class FixFloatValueByScreen
        : MonoBehaviour, IRegister
    {
        [SerializeField] private float _valueA;
        [SerializeField] private float _valueB;

        [SerializeField] private float _ratioA;
        [SerializeField] private float _ratioB;

        [SerializeField] private UnityEvent<float> TargetApply;

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateValue(CameraAspectManager.Ins.Aspect);
        }
#endif

        public void OnEvent(string eventId, EventBase data)
        {
            switch (eventId)
            {
                case EventName.CAMERA_ASPECT_CHANGED:
                    var data1 = data as CameraAspectEvent;

                    UpdateValue(data1.aspect);
                    break;
            }
        }

        private void OnEnable()
        {
            UpdateValue(Screen.width * 1f / Screen.height);

            EventObs.Ins.AddRegister(EventName.CAMERA_ASPECT_CHANGED, this);
        }

        private void OnDisable()
        {
            EventObs.Ins.RemoveRegister(EventName.CAMERA_ASPECT_CHANGED, this);
        }
        private void UpdateValue(float ratio)
        {
            var valueRatio = Mathf.InverseLerp(_ratioA, _ratioB, ratio);
            float updatedValue = Mathf.Lerp(_valueA, _valueB, valueRatio);

            TargetApply?.Invoke(updatedValue);
        }
    }
}
