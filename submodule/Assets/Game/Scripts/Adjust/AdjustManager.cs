using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using NFramework;
using UnityEngine;

namespace Antada.Libs
{
    public interface IAdjustTokenProvider
    {
        string GetToken();
    }

    public class AdjustManager : SingletonMono<AdjustManager>
    {
        public IAdjustTokenProvider Delegate;
        public void Init()
        {
#if UNITY_EDITOR || DEVELOPMENT
            AdjustConfig config = new AdjustConfig(Delegate == null ? "" : Delegate.GetToken(), AdjustEnvironment.Sandbox);
            config.setLogLevel(AdjustLogLevel.Verbose);
#else
            AdjustConfig config = new AdjustConfig(Delegate == null ? "" : Delegate.GetToken(), AdjustEnvironment.Production);
            config.setLogLevel(AdjustLogLevel.Suppress);
#endif
            Adjust.start(config);
        }

        public void LogEventAjInterShow()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_inters_show");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjInterDisplay()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_inters_displayed");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjRewardedShow()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_rewarded_show");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjRewardedDisplayed()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_rewarded_displayed");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjPurchase()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_purchase");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjContentId()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_content_id");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjPurchaseOrders()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_purchase_orders");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjCurrencyCode()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_currency_code");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjLevelStart()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_level_start");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjLevelAchieved()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_level_achieved");
            Adjust.trackEvent(adjustEvent);
        }

        public void LogEventAjLevelFail()
        {
            AdjustEvent adjustEvent = new AdjustEvent("aj_level_fail");
            Adjust.trackEvent(adjustEvent);
        }
    }
}


