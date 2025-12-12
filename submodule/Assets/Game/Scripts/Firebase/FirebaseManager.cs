using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using System;
using NFramework;
using UnityEngine;

namespace Antada.Libs
{
    public class FirebaseManager : SingletonMono<FirebaseManager>
    {
        private Firebase.FirebaseApp _app;
        private bool _showLog;
        public bool _isInitialized;

        public void Init()
        {
#if DEVELOPMENT
        _showLog = true;
        Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
#else
            _showLog = false;
#endif
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    _app = Firebase.FirebaseApp.DefaultInstance;
                    Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    this._isInitialized = true;
                    NFramework.Logger.Log("Firebase is Initialized");
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                    this._isInitialized = false;
                }
            });
        }

        public void FetchDataAsync(Dictionary<string, object> defaults, Action<bool> callback = null, ulong MinimumFetchInterval = 10000)
        {
            if (!_isInitialized)
                return;

            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error setting defaults: " + task.Exception);
                    callback?.Invoke(false); // Invoke callback with false in case of an error
                    return;
                }
                var configSettings = new ConfigSettings()
                {
                    MinimumFetchIntervalInMilliseconds = MinimumFetchInterval
                };
                FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(configSettings).ContinueWithOnMainThread(task2 =>
                {
                    if (task2.IsFaulted)
                    {
                        Debug.LogError("Error setting config settings: " + task2.Exception);
                        callback?.Invoke(false); // Invoke callback with false in case of an error
                        return;
                    }
                    FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread((x) =>
                    {
                        if (x.IsFaulted)
                        {
                            Debug.LogError("Error fetching config values: " + x.Exception);
                            callback?.Invoke(false); // Invoke callback with false in case of an error
                            return;
                        }
                        FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(task1 =>
                        {
                            if (task1.IsFaulted)
                            {
                                Debug.LogError("Error activating config values: " + task1.Exception);
                                callback?.Invoke(false); // Invoke callback with false in case of an error
                                return;
                            }
                            callback?.Invoke(true);
                        });
                    });
                });
            });
        }

        public void SetLogDebug()
        {
#if DEVELOPMENT
            _showLog = true;
            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Debug;
#else
            _showLog = false;
#endif
        }

        #region Firebase Public Function

        public void LogEventIgnoreAllNull(AnalyticEventBase analyticEvent)
        {
            try
            {
                LogEventIgnoreNull(analyticEvent.EventName, analyticEvent.GetParams());
            }
            catch (System.Exception ex)
            {
                NFramework.Logger.LogError(ex.ToString());
            }
        }

        public void LogEvent(string eventName, Dictionary<string, object> paramaters = null)
        {
            try
            {
                LogEventIgnoreNull(eventName, paramaters);
            }
            catch (System.Exception ex)
            {
                NFramework.Logger.LogError(ex.ToString());
            }
        }

        public void SetFirebaseUserID(string userID)
        {
            if (this._isInitialized == false)
            {
                NFramework.Logger.LogError("Analytic not initialized");
                return;
            }
            Firebase.Analytics.FirebaseAnalytics.SetUserId(userID);
        }

        public void SetFirebaseUserPropeties(string userkey, string uservalue)
        {
            if (this._isInitialized == false)
            {
                NFramework.Logger.LogError("Analytic not initialized");
                return;
            }
            if (this._showLog)
            {
                var message = $"<b><color=#00ff69>User Properties: Analytic</color></b>: {userkey} {uservalue}";
                NFramework.Logger.Log(message);
            }
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(userkey, uservalue);
        }

        

        #endregion
        #region Firebase Private function

        private void LogEventIgnoreNull(string eventName, Dictionary<string, object> eventParams = null)
        {
            if (this._isInitialized == false)
            {
                NFramework.Logger.LogError("Analytic not initialized");
                return;
            }

            var result = new Dictionary<string, object>();

            if (eventParams != null)
            {
                foreach (var pair in eventParams)
                {
                    if (!string.IsNullOrEmpty(pair.Key) && pair.Value != null)
                    {
                        result.Add(pair.Key, pair.Value);
                    }
                }
            }

            var message = $"<b><color=#00ff69>Analytic</color></b>: {eventName}";
            if (this._showLog)
            {
                if (eventParams != null)
                {
                    message += ". Params:\n";
                    foreach (var pair in result)
                    {
                        message += $"-{pair.Key}: {pair.Value}\n";
                    }
                }

                NFramework.Logger.Log(message);
            }

            LogEventBase(eventName, result);
        }
        private void LogEventBase(string eventName, Dictionary<string, object> eventParams = null)
        {
            if (this._isInitialized == false)
            {
                return;
            }
            var parameters = new List<Parameter>();
            foreach (var pair in eventParams)
            {
                switch (pair.Value)
                {
                    case int intValue:
                        parameters.Add(new Parameter(pair.Key, System.Convert.ToInt64(intValue)));
                        break;
                    case long longValue:
                        parameters.Add(new Parameter(pair.Key, longValue));
                        break;
                    case float floatValue:
                        parameters.Add(new Parameter(pair.Key, System.Convert.ToDouble(floatValue)));
                        break;
                    case double doubleValue:
                        parameters.Add(new Parameter(pair.Key, doubleValue));
                        break;
                    case string stringValue:
                        parameters.Add(new Parameter(pair.Key, stringValue));
                        break;
                    case bool boolValue:
                        parameters.Add(new Parameter(pair.Key, boolValue.ToString()));
                        break;
                    default:
                        NFramework.Logger.LogError("Invalid type for key " + pair.Key + ": " + pair.Value);
                        break;
                }
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, parameters.ToArray());
        }

        #endregion
    }
    public class AnalyticEventBase
    {
        protected string eventName;
        public string EventName => this.eventName;

        public virtual Dictionary<string, object> GetParams()
        {
            var dict = new Dictionary<string, object>();
            var fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            for (var i = 0; i < fields.Length; i++)
            {
                dict.Add(ToCamelCase(fields[i].Name), fields[i].GetValue(this));
            }

            return dict;
        }

        private string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }
    }

    public class EventLevel : AnalyticEventBase
    {
        public string level;
    }

    public class EventCurrency : AnalyticEventBase
    {
        public string virtual_currency_name;
        public long value;
    }

    public class EventRewardAds : AnalyticEventBase
    {
        public string placement;
    }
    public class EventLevelStart : EventLevel
    {
        public string current_gold;
        public EventLevelStart()
        {
            this.eventName = "level_start";
        }
    }
    public class EventLevelComplete : EventLevel
    {
        public string time_played;
        public EventLevelComplete()
        {
            this.eventName = "level_complete";
        }
    }

    public class EventLevelFail : EventLevel
    {
        public string failcount;
        public EventLevelFail()
        {
            this.eventName = "level_fail";
        }
    }

    public class EventSpendVirtualCurrency : EventCurrency
    {
        public string item_name;
        public EventSpendVirtualCurrency()
        {
            this.eventName = "spend_virtual_currency";
        }
    }

    public class EventEarnVirtualCurrency : EventCurrency
    {
        public string source;
        public EventEarnVirtualCurrency()
        {
            this.eventName = "earn_virtual_currency";
        }
    }

    public class EventAdsRewardLoad : EventRewardAds
    {
        public EventAdsRewardLoad()
        {
            this.eventName = "ads_reward_load";
        }
    }

    public class EventAdsRewardClick : EventRewardAds
    {
        public EventAdsRewardClick()
        {
            this.eventName = "ads_reward_click";
        }
    }

    public class EventAdsRewardShowSuccess : EventRewardAds
    {
        public EventAdsRewardShowSuccess()
        {
            this.eventName = "ads_reward_show_success";
        }
    }

    public class EventAdsRewardShowFail : EventRewardAds
    {
        public string errormsg;
        public EventAdsRewardShowFail()
        {
            this.eventName = "ads_reward_show_fail";
        }
    }

    public class EventAdsRewardComplete : EventRewardAds
    {
        public EventAdsRewardComplete()
        {
            this.eventName = "ads_reward_complete";
        }
    }

    public class EventAdsInterLoadFail : AnalyticEventBase
    {
        public string errormsg;
        public EventAdsInterLoadFail()
        {
            this.eventName = "ad_inter_load_fail";
        }
    }

    public class EventAdsInterLoadSuccess : AnalyticEventBase
    {
        public EventAdsInterLoadSuccess()
        {
            this.eventName = "ad_inter_load_success";
        }
    }

    public class EventAdsInterShow : AnalyticEventBase
    {
        public EventAdsInterShow()
        {
            this.eventName = "ad_inter_show";
        }
    }

    public class EventAdsInterClick : AnalyticEventBase
    {
        public EventAdsInterClick()
        {
            this.eventName = "ad_inter_click";
        }
    }

    public class EventOpenAdsLoadFail : AnalyticEventBase
    {
        public string errormsg;
        public EventOpenAdsLoadFail()
        {
            this.eventName = "ad_open_ads_load_fail";
        }
    }
    public class EventOpenAdsLoadSuccess : AnalyticEventBase
    {
        public EventOpenAdsLoadSuccess()
        {
            this.eventName = "ad_open_ads_load_success";
        }
    }

    public class EventOpenAdsShow : AnalyticEventBase
    {
        public EventOpenAdsShow()
        {
            this.eventName = "ad_open_ads_show";
        }
    }

    public class EventOpenAdsClick : AnalyticEventBase
    {
        public EventOpenAdsClick()
        {
            this.eventName = "ad_open_ads_click";
        }
    }

    public class EventOpenAdsPaid : AnalyticEventBase
    {
        public EventOpenAdsPaid()
        {
            this.eventName = "ad_open_ads_paid";
        }
    }

    public class EventGDPR : AnalyticEventBase
    {
        public string gdpr_type;
        public EventGDPR()
        {
            this.eventName = "gdpr_consent";
        }
    }

    public class EventATT : AnalyticEventBase
    {
        public int status;
        public EventATT()
        {
            this.eventName = "att_consent";
        }
    }

    public class EventTutorialBegin : AnalyticEventBase
    {
        public EventTutorialBegin()
        {
            this.eventName = "tutorial_begin";
        }
    }

    public class EventTutorialComplete : AnalyticEventBase
    {
        public EventTutorialComplete()
        {
            this.eventName = "tutorial_complete";
        }
    }

    public class EventCheckPoint : AnalyticEventBase
    {
        public string content;

        public EventCheckPoint(string eventName, string content)
        {
            this.eventName = eventName;
            this.content = content;
        }
    }
}

