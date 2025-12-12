using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Google.Play.Review;
#endif
using NFramework;
using UnityEngine;

namespace Antada.Libs {

    public interface IRateMeActionProvider
    {
        string StoreAppId();
    }

    public class ReviewClientManager : SingletonMono<ReviewClientManager>
    {
#if UNITY_ANDROID
        private ReviewManager reviewManager;
        private PlayReviewInfo playReviewInfo;
#endif
        public IRateMeActionProvider Delegate;
        private void StartRequestReview()
        {
            StartCoroutine(RequestReview());
        }

        private IEnumerator RequestReview()
        {
#if UNITY_ANDROID
            reviewManager = new ReviewManager();
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                DirectlyOpen();
                yield break;
            }
            playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;
            playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                DirectlyOpen();
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
#else
            yield return null;
#endif
        }

        public void ShowNativeRatingPopup()
        {
#if UNITY_ANDROID
            StartRequestReview();
#elif UNITY_IPHONE || UNITY_IOS
        if (!UnityEngine.iOS.Device.RequestStoreReview())
        {
            Application.OpenURL("https://apps.apple.com/app/id" + Delegate?.StoreAppId());
         }
#endif
        }

        public void  DirectlyOpen()
        {
#if UNITY_ANDROID
            Application.OpenURL("market://details?id="+Application.identifier);
#elif UNITY_IPHONE || UNITY_IOS
            if (!UnityEngine.iOS.Device.RequestStoreReview())
            {
                Application.OpenURL("https://apps.apple.com/app/id" + Delegate?.StoreAppId());
            }
#endif
        }
    }
}


