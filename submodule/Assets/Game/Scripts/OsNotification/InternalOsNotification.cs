namespace Antada.Libs.OsNotification
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Libs.OsNotification;
    using System;
    using NFramework;
#if UNITY_IOS
    using Unity.Notifications.iOS;
#endif

    public class NormalDataNotification
    {
        public string weekendTitle;
        public string weekendBody;
        public List<string> normalDaysTitle;
        public List<string> normalDaysBody;
    }

    public class SupportDataNotification
    {
        public List<SupportData> data;
    }

    public class SupportData
    {
        public int time;
        public string title;
        public string body;
        public bool isCondition;
    }


    public interface INotificationDataProvider
    {
        NormalDataNotification GetNormalDataNotification();
        SupportDataNotification GetSupportDataNotification();
    }

    public class InternalOsNotification : SingletonMono<InternalOsNotification>, ISaveable
    {
        private IOsNotificationManager notificationManager;
        [SerializeField] private SaveData _saveData;
        public INotificationDataProvider Delegate;
        private async void Start()
        {
#if UNITY_EDITOR
            this.notificationManager = new EditorNotificationManager();
#elif UNITY_ANDROID
			this.notificationManager = new AndroidNotificationManager();
#elif UNITY_IOS
			this.notificationManager = new MacNotificationManager();
#endif
            if (this.notificationManager != null)
            {
                await this.notificationManager.Init();
                notificationManager.CancelAllScheduleNotification();
                notificationManager.CancelAllDisplayNotification();
            }
        }

        public NotificationStatus ConsentStatus
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

        public void RequestPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (Unity.Notifications.Android.AndroidNotificationCenter.UserPermissionToPost == Unity.Notifications.Android.PermissionStatus.NotRequested)
            {
                Unity.Notifications.Android.PermissionRequest permissionRequest = new Unity.Notifications.Android.PermissionRequest();
                ConsentStatus = NotificationStatus.REQUESTED;
            }
#endif
        }

        public IEnumerator RequestAuthorization()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };
            }
#else
        yield return null;
#endif
        }


        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.N) && Input.GetKeyDown(KeyCode.O))
            {
                OnApplicationPause(true);
            }
            if (Input.GetKey(KeyCode.N) && Input.GetKeyDown(KeyCode.F))
            {
                OnApplicationPause(false);
            }
#endif
        }

        private void OnApplicationFocus(bool focus)
        {
#if UNITY_EDITOR
            OnApplicationPause(true);
#endif
        }

        private void OnApplicationPause(bool isPause)
        {
            if (notificationManager == null || Delegate == null)
                return;

            if (!notificationManager.IsReady())
                return;

            if (isPause)
            {
                ScheduleNormalNotification();
                ScheduleSupportNotification();
            }
            else
            {
                notificationManager.CancelAllScheduleNotification();
                notificationManager.CancelAllDisplayNotification();
            }
        }



#region Normal notification
        private static readonly List<int> NORMAL_NOTIFICATION_DAYS = new List<int> { 1, 2, 4, 6, 8, 10, 12, 15, 18, 22, 26, 30 };

        private static readonly List<int> NORMAL_NOTIFICATION_TIMES = new List<int> { 9, 11, 22 };

        private void ScheduleNormalNotification()
        {
            var now = DateTime.Now;
            foreach (var day in NORMAL_NOTIFICATION_DAYS)
            {
                var targetDay = now.AddDays(day);
                if (Delegate.GetNormalDataNotification() == null)
                    return;

                notificationManager.ScheduleNotification(new NotificationData
                {
                    FireTime = GetRandomFireTime(targetDay),
                    Title = GetNormalNotifcationTitle(targetDay),
                    Body = GetNormalNotifcationBody(targetDay),
                });
            }
        }

        private DateTime GetRandomFireTime(DateTime targetDay) => targetDay.ChangeTime(NORMAL_NOTIFICATION_TIMES.RandomItem(), 0);

        private string GetNormalNotifcationTitle(DateTime targetDay)
        {
            if (targetDay.DayOfWeek == DayOfWeek.Saturday || targetDay.DayOfWeek == DayOfWeek.Sunday)
                return this.Delegate.GetNormalDataNotification().weekendTitle;
            else
                return this.Delegate.GetNormalDataNotification().normalDaysTitle.RandomItem();
        }

        private string GetNormalNotifcationBody(DateTime targetDay)
        {
            if (targetDay.DayOfWeek == DayOfWeek.Saturday || targetDay.DayOfWeek == DayOfWeek.Sunday)
                return this.Delegate.GetNormalDataNotification().weekendBody;
            else
                return this.Delegate.GetNormalDataNotification().normalDaysBody.RandomItem();
        }
#endregion

#region Support notification

        private void ScheduleSupportNotification()
        {
            // Today
            var now = DateTime.Now;
            var supportData = this.Delegate.GetSupportDataNotification();
            if (supportData != null)
            {
                if (supportData.data != null)
                {
                    foreach (var data in supportData.data)
                    {
                        if (data.isCondition && now.Hour < data.time)
                        {
                            notificationManager.ScheduleNotification(new NotificationData
                            {
                                FireTime = now.ChangeTime(data.time, 0),
                                Title = data.title,
                                Body = data.body,
                            });
                        }
                    }

                    // Prepare later day
                    for (int i = 0; i < 30; i++)
                    {
                        var day = now.AddDays(i + 1);
                        foreach (var data in supportData.data)
                        {
                            notificationManager.ScheduleNotification(new NotificationData
                            {
                                FireTime = day.ChangeTime(data.time, 0),
                                Title = data.title,
                                Body = data.body,
                            });
                        }
                    }
                }
            }
        }

        [System.Serializable]
        public class SaveData
        {
            public NotificationStatus ConsentStatus;
        }

        public enum NotificationStatus
        {
            Unknown,
            REQUESTED,
        }

        public bool DataChanged { get; set; }

        public string SaveKey => "NotificationManager";

        public object GetData() => _saveData;

        public void SetData(string data)
        {
            if (string.IsNullOrEmpty(data))
                _saveData = new SaveData();
            else
                _saveData = JsonUtility.FromJson<SaveData>(data);
        }

        public void OnAllDataLoaded() { }
#endregion
    }
}