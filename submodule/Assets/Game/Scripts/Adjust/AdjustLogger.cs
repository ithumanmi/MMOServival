using com.adjust.sdk;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Purchasing;

namespace Antada.Libs
{
    public class AdjustLoggerImpression : ILoggerImpression
    {
        public void ImpressionSuccessEvent(IronSourceImpressionData impressionData)
        {
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceIronSource);
            adjustAdRevenue.setRevenue((double)impressionData.revenue, "USD");
            // optional fields
            adjustAdRevenue.setAdRevenueNetwork(impressionData.adNetwork);
            adjustAdRevenue.setAdRevenueUnit(impressionData.adUnit);
            adjustAdRevenue.setAdRevenuePlacement(impressionData.placement);
            // track Adjust ad revenue
            Adjust.trackAdRevenue(adjustAdRevenue);
        }
    }

    public class AdjustLoggerIntersitial : ILoggerIntersitial
    {
        public void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            AdjustManager.I.LogEventAjInterDisplay();
        }

        public void LogEventIntersShow()
        {
            AdjustManager.I.LogEventAjInterShow();
        }

        public void LogEventInterClick(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventIntersApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventIntersEligible()
        {

        }

        public void LogEventIntersLoadFail(IronSourceError ironSourceError)
        {

        }
    }

    public class AdjustLoggerRewarded : ILoggerRewarded
    {
        public void LogEventRewardedShow()
        {
            AdjustManager.I.LogEventAjRewardedShow();
        }

        public void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData logData)
        {
            AdjustManager.I.LogEventAjRewardedDisplayed();
        }

        public void LogEventRewardedApiCalled(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedClick(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedCompleted(IronSourceAdInfo adInfo)
        {

        }

        public void LogEventRewardedEligible()
        {

        }

        public void LogEventRewardedShowFail(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {

        }
    }

    public class AdjustLoggerOpenAd : ILoggerOpenAd
    {
        public void LogEventOnAdPaid(AdValue adValue)
        {
            var rev = adValue.Value / 1000000f;
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
            adjustAdRevenue.setRevenue(rev, "USD");
            // optional fields
            adjustAdRevenue.setAdRevenueNetwork("appopenads");
            //adjustAdRevenue.setAdRevenueUnit(data.adUnit);
            //adjustAdRevenue.setAdRevenuePlacement(data.placement);
            // track Adjust ad revenue
            Adjust.trackAdRevenue(adjustAdRevenue);
        }

        public void LogEventOpenAdClick()
        {
            throw new NotImplementedException();
        }

        public void LogEventOpenAdLoadFail(LoadAdError error)
        {
            throw new NotImplementedException();
        }

        public void LogEventOpenAdLoadSuccess()
        {
            throw new NotImplementedException();
        }

        public void LogEventOpenAdShow()
        {
            throw new NotImplementedException();
        }
    }

    public class AdjustIAPLogger : IIAPLooger
    {
        public void IAPLog(Product product, string whereToBuy)
        {
            var price = double.Parse(product.metadata.localizedPriceString);

            AdjustEvent adjustEvent = new AdjustEvent($"rev_iap");
            adjustEvent.setRevenue(price, product.metadata.isoCurrencyCode);
            Adjust.trackEvent(adjustEvent);
        }
    }
}


