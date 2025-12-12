using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hawkeye.Scene
{
    public abstract class PopupController : Controller
    {
        [SerializeField] private Transform _rootAnimation;
        [SerializeField] private Ease _openEase = Ease.OutElastic;
        protected virtual void OnEnable()
        {
            UnlockUserInterface();

            if (_rootAnimation != null)
            {
                LockUserInterface();
                _rootAnimation.DOKill();
                _rootAnimation.DOScale(1, 0.5f)
                    .From(0)
                    .SetEase(_openEase)
                    .OnComplete(() =>
                    {
                        UnlockUserInterface();
                    });
            }
        }

        protected virtual void OnAnimationHidePopupComplete()
        {

        }

    }
}
