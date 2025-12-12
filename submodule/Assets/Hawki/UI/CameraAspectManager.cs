using Hawki.EventObserver;
using UnityEngine;

namespace Hawki.EventObserver
{
    public partial class EventName
    {
        public const string CAMERA_ASPECT_CHANGED = "CAMERA_ASPECT_CHANGED";
    }

    public class CameraAspectEvent : EventBase
    {
        public float aspect;
    }
}

namespace Hawki.UI
{
    public class CameraAspectManager : RuntimeSingleton<CameraAspectManager>, IUpdateBehaviour
    {
        float lastAspect;

        private Camera _targetCamera;

        public float Aspect
        {
            get
            {
                var camera = GetCamera();

                if (camera == null)
                {
                    // phương án dự phòng
                    return Screen.width * 1f / Screen.height;
                }
                return camera.aspect;
            }
        }

        public Camera GetCamera()
        {
            return _targetCamera != null ? _targetCamera : Camera.main;
        }

        public void Update()
        {
            var _targetCamera = GetCamera();
            if (_targetCamera == null)
            {
                return;
            }

            var current = _targetCamera.aspect;

            if (lastAspect != current)
            {
                lastAspect = current;

                EventObs.Instance.ExcuteEvent(EventName.CAMERA_ASPECT_CHANGED, new CameraAspectEvent
                {
                    aspect = lastAspect,
                });
            }
        }
    }
}
