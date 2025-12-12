using Hawky.EventObserver;
using UnityEngine;
using UnityEngine.Events;

namespace Hawky.UI
{
    public class FixVector3ValueByScreen : MonoBehaviour, IRegister
    {
        [SerializeField] private Vector3 _valueA;
        [SerializeField] private Vector3 _valueB;

        [SerializeField] private float _ratioA;
        [SerializeField] private float _ratioB;

        [SerializeField] private UnityEvent<Vector3> TargetApply;

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
            UpdateValue(CameraAspectManager.Ins.Aspect);

            EventObs.Ins.AddRegister(EventName.CAMERA_ASPECT_CHANGED, this);
        }

        private void OnDisable()
        {
            EventObs.Ins.RemoveRegister(EventName.CAMERA_ASPECT_CHANGED, this);
        }
        private void UpdateValue(float ratio)
        {
            float valueRatio = Mathf.InverseLerp(_ratioA, _ratioB, ratio);
            Vector3 updatedValue = Vector3.Lerp(_valueA, _valueB, valueRatio);

            TargetApply?.Invoke(updatedValue);
        }
    }
}
