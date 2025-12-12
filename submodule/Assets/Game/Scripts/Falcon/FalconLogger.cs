using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Falcon;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine.Purchasing;

namespace Antada.Libs
{
    public interface IUserDataProvider
    {
        int GetMaxPassedLevel();
    }

    public abstract class FalconLogger
    {
        public IUserDataProvider userData;
    }


    public class FalconIntersitialLogger : FalconLogger, ILoggerIntersitial
    {
        public void LogEventInterClick(IronSourceAdInfo adInfo)
        {
            
        }

        public void LogEventIntersApiCalled(IronSourceAdInfo adInfo)
        {
            
        }

        public void LogEventIntersDisplayed(IronSourceAdInfo adInfo, LogData data)
        {
            DWHLog.Log.AdsLog(userData == null ? -1 : userData.GetMaxPassedLevel(), AdType.Interstitial, data.whereToLog);
        }

        public void LogEventIntersEligible()
        {
            
        }

        public void LogEventIntersLoadFail(IronSourceError ironSourceError)
        {
            
        }

        public void LogEventIntersShow()
        {
            
        }
    }

    public class FalconRewardedLogger : FalconLogger, ILoggerRewarded
    {
        public void LogEventRewardedApiCalled(IronSourceAdInfo adInfo)
        {
            
        }

        public void LogEventRewardedClick(IronSourceAdInfo adInfo)
        {
            
        }

        public void LogEventRewardedCompleted(IronSourceAdInfo adInfo)
        {
            
        }

        public void LogEventRewardedDisplayed(IronSourceAdInfo adInfo, LogData data)
        {
            DWHLog.Log.AdsLog(userData == null ? -1 : userData.GetMaxPassedLevel(), AdType.Reward, data.whereToLog);
        }

        public void LogEventRewardedEligible()
        {
            
        }

        public void LogEventRewardedShow()
        {
            
        }

        public void LogEventRewardedShowFail(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
        {
            
        }
    }

    public class FalconIAPLogger : FalconLogger, IIAPLooger
    {
        public void IAPLog(Product product, string whereToBuy)
        {
            DWHLog.Log.InAppLog(userData == null ? -1 : userData.GetMaxPassedLevel(), product.definition.id, product.metadata.isoCurrencyCode,
                        GetIAPRevenue(product.metadata.localizedPrice), product.transactionID, "", whereToBuy);
        }

        private string GetIAPRevenue(decimal amount)
        {
            decimal val = decimal.Multiply(amount, 0.63m);
            return val.ToString();
        }
    }
}

