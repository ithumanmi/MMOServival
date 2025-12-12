using Antada.Libs;
using Falcon;
using Falcon.FalconAnalytics.Scripts.Enum;
using Hawki;
using Hawki.EventObserver;
using Hawki.GameFlow;
using Hawki.IAP;
using Hawki.Inventory;
using Hawki.MyCoroutine;
using System;
using System.Collections;
using UnityEngine.Purchasing;

namespace Hawki_Antada_Falcon.Falcon
{
    public class FalconEvent : RuntimeSingleton<FalconEvent>, IStartBehaviour, IRegister, ILoggerIntersitial, ILoggerRewarded, IIAPLooger
    {
        public void Start()
        {
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_COMPLETED_STAGE, this);
            EventObs.Instance.AddRegister(EventName.EARN_VIRTUAL_CURRENCY, this);
            EventObs.Instance.AddRegister(EventName.SPEND_VIRTUAL_CURRENCY, this);

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
                case EventName.GAMEFLOW_COMPLETED_STAGE:
                    var d1 = data as GameFlowCompletedStageEvent;
                    HandleCompletedStageEvent(d1);
                    break;
                case EventName.EARN_VIRTUAL_CURRENCY:
                    var d2 = data as EarnVirtualCurrencyEvent;
                    HandleEarnVirtualCurrencyEvent(d2);
                    break;
                case EventName.SPEND_VIRTUAL_CURRENCY:
                    var d3 = data as SpendVirtualCurrencyEvent;
                    HandleSpendVirtualCurrencyEvent(d3);
                    break;
            }
        }

        private void HandleCompletedStageEvent(GameFlowCompletedStageEvent d1)
        {
            DWHLog.Log.LevelLog(d1.stage, TimeSpan.FromSeconds(d1.duration), 0, string.Empty, d1.isWin ? LevelStatus.Pass : LevelStatus.Fail);
        }

        private void HandleEarnVirtualCurrencyEvent(EarnVirtualCurrencyEvent d2)
        {
            var service = SingletonManager.Instance.FindFirst<IGameFlowService>();

            var stage = service == null ? 0 : service.MaxStage();
            DWHLog.Log.ResourceLog(stage, FlowType.Source, d2.position, d2.position, d2.itemId, (long)d2.itemAmount);
        }

        private void HandleSpendVirtualCurrencyEvent(SpendVirtualCurrencyEvent d2)
        {
            var service = SingletonManager.Instance.FindFirst<IGameFlowService>();
            var stage = service == null ? 0 : service.MaxStage();

            DWHLog.Log.ResourceLog(stage, FlowType.Sink, d2.position, d2.position, d2.itemId, (long)d2.itemAmount);
        }

        public void LogEventRewardedApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            var service = SingletonManager.Instance.FindFirst<IGameFlowService>();
            var stage = service == null ? 0 : service.MaxStage();
            DWHLog.Log.AdsLog(stage, AdType.Reward, logData.whereToLog);
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

        }

        public void LogEventIntersApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            var service = SingletonManager.Instance.FindFirst<IGameFlowService>();
            var stage = service == null ? 0 : service.MaxStage();
            DWHLog.Log.AdsLog(stage, AdType.Interstitial, logData.whereToLog);
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

        }

        public void IAPLog(Product product, string whereToBuy)
        {
            var service = SingletonManager.Instance.FindFirst<IGameFlowService>();
            var stage = service == null ? 0 : service.MaxStage();
            DWHLog.Log.InAppLog(stage, product.definition.id, product.metadata.isoCurrencyCode,
                        IAPService.Instance.GetRevenue(product.metadata.localizedPrice), product.transactionID, "", whereToBuy);
        }
    }
}
