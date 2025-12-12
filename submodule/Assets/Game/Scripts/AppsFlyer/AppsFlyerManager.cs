using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AppsFlyerSDK;
using NFramework;
using UnityEngine;


namespace Antada.Libs {
	public interface IAppsFlyerKey {
		string GetDevKey();
		string GetAppID();
	}
	public class AppsFlyerManager : SingletonMono<AppsFlyerManager>, IAppsFlyerConversionData
	{
		public IAppsFlyerKey Delegate;
		public bool getConversionData;
		public void Init()
		{
#if DEVELOPMENT
AppsFlyer.setIsDebug(true);
#endif
			AppsFlyer.initSDK(Delegate.GetDevKey(), Delegate.GetAppID(), getConversionData ? this : null);
			AppsFlyer.startSDK();
		}


#region AppsFlyer CallBacks

		public void onConversionDataSuccess(string conversionData)
		{
			AppsFlyer.AFLog("didReceiveConversionData", conversionData);
			Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
			// add deferred deeplink logic here
		}

		public void onConversionDataFail(string error)
		{
			AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
		}

		public void onAppOpenAttribution(string attributionData)
		{
			AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
			Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
			// add direct deeplink logic here
		}

		public void onAppOpenAttributionFailure(string error)
		{
			AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
		}
#endregion

		public void LogEventTutorial(TutorialEvent tutorial)
		{
			Dictionary<string, string> eventValue = new Dictionary<string, string>();
			eventValue.Add("af_success", tutorial.af_success);
			eventValue.Add("af_tutorial_id", tutorial.af_tutorial_id);
			AppsFlyer.sendEvent(tutorial.EventName, eventValue);
		}

		public void LogEventLevelAchieved(LevelAchievedEvent levelAchieved)
		{
			Dictionary<string, string> eventValue = new Dictionary<string, string>();
			eventValue.Add("af_level", levelAchieved.af_level);
			eventValue.Add("af_score", levelAchieved.af_score);
			AppsFlyer.sendEvent(levelAchieved.EventName, eventValue);
		}

		public void LogEventAchievementUnlocked(AchievementUnlockedEvent unlockedEvent)
		{
			Dictionary<string, string> eventValue = new Dictionary<string, string>();
			eventValue.Add("content_id", unlockedEvent.content_id);
			eventValue.Add("af_level", unlockedEvent.af_level);
			AppsFlyer.sendEvent(unlockedEvent.EventName, eventValue);
		}

		public void LogEventAfIntersShow()
		{
			AppsFlyer.sendEvent("af_inters_show", null);
		}

		public void LogEventAfIntersDisplayed()
		{
			AppsFlyer.sendEvent("af_inters_displayed", null);
		}

		public void LogEventAfIntersApiCalled()
		{
			AppsFlyer.sendEvent("af_inters_api_called", null);
		}

		public void LogEventAfIntersEligible()
		{
			AppsFlyer.sendEvent("af_inters_ad_eligible", null);
		}
		/// <summary>
		/// rewarded ad log
		/// </summary>
		public void LogEventAfRewardedApiCalled()
		{
			AppsFlyer.sendEvent("af_rewarded_api_called", null);
		}

		public void LogEventAfRewardedEligible()
		{
			AppsFlyer.sendEvent("af_rewarded_ad_eligible", null);
		}

		public void LogEventAfRewardedShow()
		{
			AppsFlyer.sendEvent("af_rewarded_show", null);
		}

		public void LogEventAfRewardedDisplayed()
		{
			AppsFlyer.sendEvent("af_rewarded_displayed", null);
		}

		public void LogEventAfRewardedCompleted()
		{
			AppsFlyer.sendEvent("af_rewarded_ad_completed", null);
		}

		private string GetAppsflyerRevenue(decimal amount)
		{
			decimal val = decimal.Multiply(amount, 0.63m);
			return val.ToString();
		}
	}

	public class BaseAppsFlyerEvent
	{
		protected string eventName;
		public string EventName => this.eventName;
	}

	public class TutorialEvent : BaseAppsFlyerEvent
	{
		public string af_success;
		public string af_tutorial_id;
		public TutorialEvent()
		{
			this.eventName = "af_tutorial_completion";
		}
	}

	public class LevelAchievedEvent : BaseAppsFlyerEvent
	{
		public string af_level;
		public string af_score;
		public LevelAchievedEvent()
		{
			this.eventName = "af_level_achieved";
		}
	}

	public class AchievementUnlockedEvent : BaseAppsFlyerEvent
	{
		public string content_id;
		public string af_level;
		public AchievementUnlockedEvent()
		{
			this.eventName = "af_achievement_unlocked";
		}
	}
	
}


