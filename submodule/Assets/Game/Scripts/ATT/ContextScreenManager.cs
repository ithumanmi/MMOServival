using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NFramework;
#if UNITY_IOS
using UnityEngine.iOS;
using Unity.Advertisement.IosSupport;
#endif

namespace Antada.Libs {
    public class ContextScreenManager : SingletonMono<ContextScreenManager>
    {
        public int attStatus;
        private bool IsATTRequest;
        public void Init()
        {
            Debug.Log("Request ATT");
#if UNITY_IOS && !UNITY_EDITOR
        // check with iOS to see if the user has accepted or declined tracking
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        Version currentVersion = new Version(Device.systemVersion);
        Version ios14 = new Version("14.5");

        if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED && currentVersion >= ios14)
        {
            IsATTRequest = true;
            ATTrackingStatusBinding.RequestAuthorizationTracking(AuthorizationTrackingReceived);
        }
#else
            Debug.Log("Unity iOS Support: App Tracking Transparency status not checked, because the platform is not iOS.");
#endif
        }
        private void AuthorizationTrackingReceived(int status)
        {
            this.attStatus = status;
            Debug.LogFormat("Tracking status received: {0}", status);
        }
        public bool IsRequestATT()
        {
            return IsATTRequest;
        }
    }
}

