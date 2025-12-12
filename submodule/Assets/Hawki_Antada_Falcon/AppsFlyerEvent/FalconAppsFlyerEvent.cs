#if ANTADA_FALCON

using Antada.Libs;
using Hawki;
using Hawki.EventObserver;
using Hawki.GameFlow;
using Hawki.IAP;
using Hawki.MyCoroutine;
using Hawki.SaveData;
using System.Collections;
using UnityEngine.Purchasing;

namespace Hawki_Antada_Falcon.AppsFlyerEvent
{
    public class FalconAppsFlyerEvent : RuntimeSingleton<FalconAppsFlyerEvent>, IStartBehaviour, ILoggerIntersitial, IIAPLooger, ILoggerRewarded, IRegister
    {
        public void Start()
        {
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_END_TUTORIAL, this);
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_UNLOCKED, this);
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_COMPLETED_STAGE, this);

            CoroutineManager.Instance.Start(TryInitAds());
            CoroutineManager.Instance.Start(TryInitIAP());
        }

        IEnumerator TryInitAds()
        {
            while (true)
            {
                var adsManager = AdsManager.I;

                if (adsManager != null)
                {
                    adsManager.AddLoggerIntertitial(this);
                    adsManager.AddLoggerRewarded(this);
                    break;
                };

                yield return null;
            }
        }

        IEnumerator TryInitIAP()
        {
            while (true)
            {
                var iapManager = IAPManager.I;

                if (iapManager != null)
                {
                    iapManager.AddLogger(this);
                    break;
                };

                yield return null;
            }
        }

        public void OnEvent(string eventId, EventBase data)
        {
            switch (eventId)
            {
                case EventName.GAMEFLOW_END_TUTORIAL:
                    var gameFlowEndTutorialEvent = data as GameFlowEndTutorialEvent;
                    TrySendEvent(new TutorialCompletionAnalyticEvent(gameFlowEndTutorialEvent.success.ToString(), gameFlowEndTutorialEvent.tutorialId));
                    break;
                case EventName.GAMEFLOW_UNLOCKED:
                    var gameFlowUnlockedEvent = data as GameFlowUnlockedEvent;
                    TrySendEvent(new AchievementUnlockedAnalyticEvent(gameFlowUnlockedEvent.contentId, gameFlowUnlockedEvent.currentLevel.ToString()));
                    break;
                case EventName.GAMEFLOW_COMPLETED_STAGE:
                    var gameFlowCompletedStageEvent = data as GameFlowCompletedStageEvent;
                    HandleCompletedStageEvent(gameFlowCompletedStageEvent);
                    break;
            }
        }

        private void HandleCompletedStageEvent(GameFlowCompletedStageEvent data)
        {
            // save data
            var fireBaseData = SaveDataManager.Instance.GetData<FalconAppsFlyerEventData>();


            var firstWin = fireBaseData.currentStageAchieved < data.stage;

            if (firstWin)
            {
                fireBaseData.currentStageAchieved = data.stage;
            }

            fireBaseData.Save();

            // log event
            if (data.isWin)
            {
                // complete_stage_{0}
                if (firstWin)
                {
                    var eventData = new LevelAchievedAnalyticEvent(data.stage.ToString(), data.score.ToString());
                    TrySendEvent(eventData);
                };
            }
        }

        public void LogEventRewardedApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            TrySendEvent(new RewardDisplayedAnalyticEvent());
        }

        public void LogEventRewardedCompleted(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedShowFail(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedClick(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedEligible()
        {

        }

        public void LogEventRewardedShow()
        {
            TrySendEvent(new RewardShowAnalyticEvent());
        }

        public void LogEventIntersApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            TrySendEvent(new InterDisplayedAnalyticEvent());
        }

        public void LogEventIntersLoadFail(IronSourceError ironSourceError)
        {

        }

        public void LogEventInterClick(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventIntersEligible()
        {

        }

        public void LogEventIntersShow()
        {
            TrySendEvent(new InterShowAnalyticEvent());
        }

        private void TrySendEvent(BaseAnalyticEvent eventBase)
        {
            AppsFlyerSDK.AppsFlyer.sendEvent(eventBase.EventName, eventBase.GetParams());
        }

        public void IAPLog(Product product, string whereToBuy)
        {
            var productOs = IAPManager.I.GetProductSO(product.definition.id);

            if (productOs == null)
            {
                return;
            }

            TrySendEvent(new PurchaseAnalyticEvent(product.metadata.isoCurrencyCode, product.definition.id, IAPService.Instance.GetRevenue(product.metadata.localizedPrice)));
        }
    }
}

#endif
