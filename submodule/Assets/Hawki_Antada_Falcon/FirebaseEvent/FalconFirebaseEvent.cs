#if ANTADA_FALCON

using Antada.Libs;
using Hawki;
using Hawki.EventObserver;
using Hawki.GameFlow;
using Hawki.Inventory;
using Hawki.MyCoroutine;
using Hawki.SaveData;
using System.Collections;
using UnityEngine;

namespace Hawki_Antada_Falcon.FirebaseEvent
{
    public class FalconFirebaseEvent : RuntimeSingleton<FalconFirebaseEvent>, IStartBehaviour, IRegister, ILoggerIntersitial, ILoggerRewarded
    {
        private float _timeStartStage;
        public void Start()
        {
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_COMPLETED_STAGE, this);
            EventObs.Instance.AddRegister(EventName.GAMEFLOW_START_STAGE, this);
            EventObs.Instance.AddRegister(EventName.EARN_VIRTUAL_CURRENCY, this);
            EventObs.Instance.AddRegister(EventName.SPEND_VIRTUAL_CURRENCY, this);

            CoroutineManager.Instance.Start(TryInitAds());
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

        public void OnEvent(string eventId, EventBase data)
        {
            switch (eventId)
            {
                case EventName.GAMEFLOW_COMPLETED_STAGE:
                    var d1 = data as GameFlowCompletedStageEvent;
                    HandleCompletedStageEvent(d1);
                    break;
                case EventName.GAMEFLOW_START_STAGE:
                    var d2 = data as GameFlowStartStageEvent;
                    HandleStartStageEvent(d2);
                    break;
                case EventName.EARN_VIRTUAL_CURRENCY:
                    var d3 = data as EarnVirtualCurrencyEvent;
                    HandleEarnVirtualCurrency(d3);
                    break;
                case EventName.SPEND_VIRTUAL_CURRENCY:
                    var d4 = data as SpendVirtualCurrencyEvent;
                    HandleSpendVirtualCurrency(d4);
                    break;
            }
        }

        private bool TrySendEvent(AnalyticEventBase e)
        {
            var firebaseManager = FirebaseManager.I;

            if (firebaseManager == null)
            {
                Debug.LogError("không có FirebaseManager để gửi");
                return false;
            }

            firebaseManager.LogEventIgnoreAllNull(e);

            return true;
        }

        private void HandleCompletedStageEvent(GameFlowCompletedStageEvent data)
        {
            // save data
            var fireBaseData = SaveDataManager.Instance.GetData<FalconFirebaseEventData>();

            if (!data.isWin)
            {
                if (fireBaseData.stageCountFailed.ContainsKey(data.stage) == false)
                {
                    fireBaseData.stageCountFailed.Add(data.stage, 0);
                }
                fireBaseData.stageCountFailed[data.stage]++;
            }

            var firstWin = fireBaseData.currentStageCheckPoint < data.stage;

            if (firstWin)
            {
                fireBaseData.currentStageCheckPoint = data.stage;
            }

            var timePlayed = _timeStartStage == 0 ? 0 : Time.realtimeSinceStartup - _timeStartStage;
            _timeStartStage = 0;

            fireBaseData.Save();

            // log event
            if (data.isWin)
            {
                TrySendEvent(new LevelCompleteAnalyticEvent(data.stage, ((int)timePlayed).ToString()));

                // complete_stage_{0}
                if (firstWin)
                {
                    var eventData = new SimpleAnalyticEvent(string.Format(AnalyticEventName.STAGE_PASSED, data.stage));
                    TrySendEvent(eventData);
                };
            } else
            {
                // level failed
                TrySendEvent(new LevelFailAnalyticEvent(data.stage, fireBaseData.stageCountFailed[data.stage]));
            }
        }


        private void HandleStartStageEvent(GameFlowStartStageEvent data)
        {
            _timeStartStage = Time.realtimeSinceStartup;

            var gold = ItemService.Instance.ExportAllCurrency();

            TrySendEvent(new LevelStartAnalyticEvent(data.stage, gold));
        }

        private void HandleEarnVirtualCurrency(EarnVirtualCurrencyEvent data)
        {
            TrySendEvent(new EarnVirtualCurrencyAnalyticEvent(data.itemId, (long)data.itemAmount, data.position));
        }

        private void HandleSpendVirtualCurrency(SpendVirtualCurrencyEvent data)
        {
            TrySendEvent(new SpendVirtualCurrencyAnalyticEvent(data.itemId, (long)data.itemAmount, data.newReward));
        }

        public void LogEventRewardedApiCalled(IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_LOAD));
        }

        public void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_SHOW_SUCCESS));
        }

        public void LogEventRewardedCompleted(IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_COMPLETE));
        }

        public void LogEventRewardedShowFail(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_SHOW_FAIL));
        }

        public void LogEventRewardedClick(IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_CLICK));
        }

        public void LogEventRewardedEligible()
        {

        }

        public void LogEventRewardedShow()
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_REWARD_SHOW_SUCCESS));
        }

        public void LogEventIntersApiCalled(IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_INTER_LOAD_SUCCESS));
        }

        public void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {

        }

        public void LogEventIntersLoadFail(IronSourceError ironSourceError)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_INTER_LOAD_FAIL));
        }

        public void LogEventInterClick(IronSourceAdInfo adInfo)
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_INTER_CLICK));
        }

        public void LogEventIntersEligible()
        {

        }

        public void LogEventIntersShow()
        {
            TrySendEvent(new SimpleAnalyticEvent(AnalyticEventName.ADS_INTER_SHOW));
        }
    }
}

#endif
