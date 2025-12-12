using Hawky.Math;
using Hawky.MyCoroutine;
using System.Collections;
using UnityEngine;

namespace Hawky.Scene
{
    public abstract class PopupController : Controller
    {
        [SerializeField] private Transform _rootAnimation;
        [SerializeField] private EaseType _openEase = EaseType.OutBounce;

        protected Transform Root => _rootAnimation;

        private ReturnData _returnData;

        protected virtual void OnEnable()
        {
            if (_rootAnimation != null)
            {
                CoroutineManager.Ins.Start(AnimationOpen());
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnActive()
        {
            base.OnActive();

            _returnData = GetReturnData();
        }

        protected override void OnShown()
        {
            base.OnShown();
        }

        private IEnumerator AnimationOpen()
        {
            var locker = LockUserInterface();

            var counter = 0f;
            var target = 0.5f;
            while (counter < target)
            {
                var scaleValue = EaseCalculator.GetValue(0.5f, 1, target, counter, _openEase);
                _rootAnimation.localScale = Vector3.one * scaleValue;

                yield return null;
                counter += Time.deltaTime;
            }

            UnlockUserInterface(locker);
        }

        protected override void OnHidden()
        {
            base.OnHidden();

            if (_returnData != null)
            {
                if (_returnData.Returned == ReturnId.WAIT)
                {
                    _returnData.Returned = ReturnId.NONE;
                }
            }
        }

        protected virtual ReturnData GetReturnData()
        {
            return null;
        }
    }
}
