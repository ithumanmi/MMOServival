using NFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antada.Libs
{
    public interface IAdUnitIronSourceProvider
    {
        string AppKeyAndroid();
        string AppKeyiOS();
    }

    public interface ILoggerIntersitial
    {
        void LogEventIntersApiCalled(IronSourceAdInfo adInfo);
        void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData logData);
        void LogEventIntersLoadFail(IronSourceError ironSourceError);
        void LogEventInterClick(IronSourceAdInfo adInfo);
        void LogEventIntersEligible();
        void LogEventIntersShow();
    }

    public interface ILoggerRewarded
    {
        void LogEventRewardedApiCalled(IronSourceAdInfo adInfo);
        void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData logData);
        void LogEventRewardedCompleted(IronSourceAdInfo adInfo);
        void LogEventRewardedShowFail(IronSourceError ironSourceError, IronSourceAdInfo adInfo);
        void LogEventRewardedClick(IronSourceAdInfo adInfo);
        void LogEventRewardedEligible();
        void LogEventRewardedShow();
    }

    public interface ILoggerImpression
    {
        void ImpressionSuccessEvent(IronSourceImpressionData impressionData);
    }

    public class LogData
    {
        public string whereToLog;
        public string placement;
    }

    public class AdsManager : SingletonMono<AdsManager>, ISaveable
    {
        public static Action<bool> OnIsRemovedAdsChanged;
        public static Action<bool> BannerAdLoadedStatus;
        public IAdUnitIronSourceProvider Delegate;
        public List<ILoggerIntersitial> loggerIntersitial;
        public List<ILoggerRewarded> loggerRewardedAds;
        public List<ILoggerImpression> loggerImpression;
        [SerializeField] private SaveData _saveData;
        private Action<bool> _OnWatchRewards;
        private Action<bool> _OnCloseInterstitial;
        private bool _isAdShowing;
        private bool _isRewardedDoCloseAction;
        private string _whereToWatchIntesitial;
        private string _placementName;
        private string _whereToWatchRewarded;
        private IronSourceBannerPosition _bannerPosition;
        public static bool? _isInitialized;
        public Action OnCloseInterstitial;


        private string GetAppKey()
        {
#if UNITY_ANDROID
            return Delegate == null ? "" : Delegate.AppKeyAndroid();
#elif UNITY_IOS
            return Delegate == null ? "" : Delegate.AppKeyiOS();
#else
            return "unused";
#endif
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }


        public bool IsAdShowing
        {
            get => _isAdShowing;
        }

        public bool IsRemovedAds
        {
            get => _saveData.isRemovedAds;
            set
            {
                if (_saveData.isRemovedAds != value)
                {
                    _saveData.isRemovedAds = value;
                    DataChanged = true;
                    OnIsRemovedAdsChanged?.Invoke(value);

                    if (value)
                    {
                        HideBannerIron();
                    }
                }
            }
        }

        public GDPRStatus ConsentStatus
        {
            get => _saveData.ConsentStatus;
            set
            {
                if (_saveData.ConsentStatus != value)
                {
                    _saveData.ConsentStatus = value;
                    DataChanged = true;
                    SaveManager.I.Save();
                }
            }
        }

        public void Init(IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.BOTTOM, bool withAdUnits = false)
        {
#if UNITY_IOS
            IronSource.Agent.SetPauseGame(true);
#endif
            _bannerPosition = bannerPosition;
            IronSource.Agent.setMetaData("is_child_directed", "false");
            IronSource.Agent.shouldTrackNetworkState(true);
            if (_saveData.ConsentStatus == GDPRStatus.Unknown)
            {
                NFramework.Logger.Log($"TODO: GDPRStatus.Unknown", this);
                return;
            }

            bool consentValue = (_saveData.ConsentStatus == GDPRStatus.YES);

            IronSource.Agent.setConsent(consentValue);

            if (withAdUnits)
            {
                IronSource.Agent.init(GetAppKey(), IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.BANNER);
            }
            else
            {
                IronSource.Agent.init(GetAppKey());
            }

            if (DeviceInfo.IsDevelopment)
            {
                IronSource.Agent.validateIntegration();
            }
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
        }

        public void AddLoggerIntertitial(ILoggerIntersitial logger)
        {
            if (loggerIntersitial == null)
            {
                loggerIntersitial = new List<ILoggerIntersitial>();
            }

            loggerIntersitial.Add(logger);
        }

        public void AddLoggerRewarded(ILoggerRewarded logger)
        {
            if (loggerRewardedAds == null)
            {
                loggerRewardedAds = new List<ILoggerRewarded>();
            }

            loggerRewardedAds.Add(logger);
        }

        public void AddLoggerImpression(ILoggerImpression logger)
        {
            if (loggerImpression == null)
            {
                loggerImpression = new List<ILoggerImpression>();
            }

            loggerImpression.Add(logger);
        }

        private void SdkInitializationCompletedEvent()
        {
            NFramework.Logger.Log($"Init Ironsource completed, Loading Ads", this);
            IronSource.Agent.loadInterstitial();
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, _bannerPosition);
            HideBannerIron();
        }

        private void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
        {
            if (impressionData != null)
            {
                if (loggerImpression != null)
                {
                    foreach (var logger in loggerImpression)
                    {
                        logger.ImpressionSuccessEvent(impressionData);
                    }
                }
            }
        }

      
        private void OnEnable()
        {
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionSuccessEvent;

            //Add AdInfo Banner Events
            IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

            //Add AdInfo Interstitial Events
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

            //Add AdInfo Rewarded Video Events
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

        }

        private void OnDisable()
        {

            IronSourceEvents.onImpressionDataReadyEvent -= ImpressionSuccessEvent;

            //Add AdInfo Banner Events
            IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
            IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
            IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
            IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;

            //Add AdInfo Interstitial Events
            IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
            IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;

            //Add AdInfo Rewarded Video Events
            IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;
        }
        #region Banner
        /************* Banner AdInfo Delegates *************/
        //Invoked once the banner has loaded
        void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
        {
        }
        //Invoked when the banner loading process has failed.
        void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
        {
            //reload
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, _bannerPosition);
        }
        // Invoked when end user clicks on the banner ad
        void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
        }
        //Notifies the presentation of a full screen content following user click
        void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
        {
        }
        //Notifies the presented screen has been dismissed
        void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
        {
        }
        //Invoked when the user leaves the app
        void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
        {
        }
        #endregion
        #region Interstitial Ads
        /************* Interstitial AdInfo Delegates *************/
        // Invoked when the interstitial ad was loaded succesfully.
        void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log($"InterstitialOnAdReadyEvent: {adInfo.adUnit}");
            if (loggerIntersitial != null)
            {
                foreach (var logger in loggerIntersitial)
                {
                    logger.LogEventIntersApiCalled(adInfo);
                }
            }
        }
        // Invoked when the initialization process has failed.
        void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
        {
            Debug.Log($"InterstitialOnAdLoadFailed: {ironSourceError.getDescription()}");
            IronSource.Agent.loadInterstitial();
            if (loggerIntersitial != null)
            {
                foreach (var logger in loggerIntersitial)
                {
                    logger.LogEventIntersLoadFail(ironSourceError);
                }
            }
        }
        // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
        void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("IntersitialAD AdOpened--adNetwork: " + adInfo.adNetwork);
            if (loggerIntersitial != null)
            {
                foreach (var logger in loggerIntersitial)
                {
                    logger.LogEventIntersDisplayed(adInfo, new LogData()
                    {
                        whereToLog = _whereToWatchIntesitial,
                        placement = _placementName,
                    });
                }
            }
            _isAdShowing = true;
        }
        // Invoked when end user clicked on the interstitial ad
        void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log($"InterstitialOnAdClickedEvent: {adInfo.adUnit}");
            if (loggerIntersitial != null)
            {
                foreach (var logger in loggerIntersitial)
                {
                    logger.LogEventInterClick(adInfo);
                }
            }
        }
        // Invoked when the ad failed to show.
        void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            _isAdShowing = false;
            Debug.Log("IntersitialAD falied to show: ironSourceError: " + ironSourceError.getDescription() + "---" + "adInfo: " + adInfo.adNetwork);
            _OnCloseInterstitial?.Invoke(false);
        }
        // Invoked when the interstitial ad closed and the user went back to the application screen.
        void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log($"InterstitialOnAdClosedEvent: {adInfo.adUnit}");
            _OnCloseInterstitial?.Invoke(true);
            OnCloseInterstitial?.Invoke();
            _isAdShowing = false;
        }
        // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
        // This callback is not supported by all networks, and we recommend using it only if  
        // it's supported by all networks you included in your build. 
        void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
        {
        }
        #endregion
        #region Rewarded Video

        /************* RewardedVideo AdInfo Delegates *************/
        // Indicates that there’s an available ad.
        // The adInfo object includes information about the ad that was loaded successfully
        // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
        void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedApiCalled(adInfo);
                }
            }
        }
        // Indicates that no ads are available to be displayed
        // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
        void RewardedVideoOnAdUnavailable()
        {
        }
        // The Rewarded Video ad view has opened. Your activity will loose focus.
        void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("RewaredAD AdOpened--adNetwork: " + adInfo.adNetwork);
            _isAdShowing = true;
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedDisplayed(adInfo, new LogData()
                    {
                        whereToLog = _whereToWatchRewarded,
                        placement = _placementName,
                    });
                }
            }
        }
        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            _isAdShowing = false;
            StartCoroutine(DelayedRewardAdCloseAction());
        }

        IEnumerator DelayedRewardAdCloseAction()
        {
            yield return new WaitForSeconds(0.3f); // Wait for 0.3 seconds
            if (_isRewardedDoCloseAction)
                _OnWatchRewards?.Invoke(false);
        }
        // The user completed to watch the video, and should be rewarded.
        // The placement parameter will include the reward data.
        // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            _OnWatchRewards?.Invoke(true);
            _isRewardedDoCloseAction = false;
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedCompleted(adInfo);
                }
            }
        }
        // The rewarded video ad was failed to show.
        void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        {
            _OnWatchRewards?.Invoke(false);
            _isAdShowing = false;
            _isRewardedDoCloseAction = false;
            Debug.Log("RewaredAD falied to show: ironSourceError: " + error.ToString() + "---" + "adInfo: " + adInfo.adNetwork);
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedShowFail(error, adInfo);
                }
            }
        }
        // Invoked when the video ad was clicked.
        // This callback is not supported by all networks, and we recommend using it only if
        // it’s supported by all networks you included in your build.
        void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedClick(adInfo);
                }
            }
        }

        #endregion
        public void ShowBannerIron()
        {
#if UNITY_EDITOR || NO_ADS
            return;
#endif
            if (IsRemovedAds)
                return;

            IronSource.Agent.displayBanner();
        }

        public void HideBannerIron() => IronSource.Agent.hideBanner();

        public bool IsInterstitialAvailable() => IronSource.Agent.isInterstitialReady();

        public bool IsRewardAvailable() => IronSource.Agent.isRewardedVideoAvailable();

        public bool ShowInterstitial(Action<bool> callback, string whereToWatch, string placementName = null)
        {
            _whereToWatchIntesitial = whereToWatch;
            _placementName = placementName;
            _OnCloseInterstitial = callback;
#if UNITY_EDITOR || NO_ADS
            _OnCloseInterstitial?.Invoke(true);
            return false;
#endif
            if (IsRemovedAds)
            {
                _OnCloseInterstitial?.Invoke(true);
                return false;
            }

            if (!IsInterstitialAvailable())
            {
                _OnCloseInterstitial?.Invoke(false);
                return false;
            }
            _isAdShowing = true;
            if (loggerIntersitial != null)
            {
                foreach (var logger in loggerIntersitial)
                {
                    logger.LogEventIntersShow();
                    logger.LogEventIntersEligible();
                }
            }
            if (placementName == null)
            {
                IronSource.Agent.showInterstitial();
            }
            else
            {
                IronSource.Agent.showInterstitial(placementName);
            }

            IronSource.Agent.loadInterstitial();

            return true;
        }

        public void ShowReward(Action<bool> callback, string whereToWatch, string placementName = null)
        {
            _whereToWatchRewarded = whereToWatch;
            _placementName = placementName;
            _OnWatchRewards = callback;

#if UNITY_EDITOR || NO_ADS
            _OnWatchRewards?.Invoke(true);
            return;
#endif
            if (Application.isEditor)
            {
                _OnWatchRewards?.Invoke(true);
                return;
            }

            if (!IsRewardAvailable())
            {
                _OnWatchRewards?.Invoke(false);
                return;
            }
            _isAdShowing = true;
            _isRewardedDoCloseAction = true;
            if (loggerRewardedAds != null)
            {
                foreach (var logger in loggerRewardedAds)
                {
                    logger.LogEventRewardedShow();
                    logger.LogEventRewardedEligible();
                }
            }

            if (placementName == null)
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                IronSource.Agent.showRewardedVideo(placementName);
            }
        }

        #region ISaveable
        [Serializable]
        public class SaveData
        {
            public bool isRemovedAds;
            public GDPRStatus ConsentStatus;
        }

        public enum GDPRStatus
        {
            Unknown,
            YES,
            NO
        }

        public string SaveKey => "AdsManager";

        public bool DataChanged { get; set; }

        public object GetData() => _saveData;

        public void SetData(string data)
        {
            if (string.IsNullOrEmpty(data))
                _saveData = new SaveData();
            else
                _saveData = JsonUtility.FromJson<SaveData>(data);
        }

        public void DefaultData(bool noAds = false)
        {
            _saveData = new SaveData()
            {
                ConsentStatus = GDPRStatus.YES,
                isRemovedAds = noAds,
            };
        }

        public void OnAllDataLoaded() { }
        #endregion
    }
}

