#if UNITY_ANDROID

namespace Antada.Libs.OsNotification
{
	using Unity.Notifications.Android;
	using UniRx.Async;

	public class AndroidNotificationManager : IOsNotificationManager
	{
		private const string ChannelId = "game_channel0";
		private bool isReady;

		public AndroidNotificationManager()
		{
			this.isReady = false;
		}

		public async UniTask Init()
		{
			// Set up channels (mostly for Android) Android > 8.0
			this.RegisterChannel(ChannelId, "Default Game Channel", "Generic notifications");
			this.isReady = true;
		}

		public bool IsReady()
		{
			return this.isReady;
		}

		public void ScheduleNotification(NotificationData data)
		{
			if (!this.isReady)
			{
				return;
			}

			var androidNotification = new AndroidNotification
			{
				Title = data.Title,
				Text = data.Body,
				FireTime = data.FireTime,
				SmallIcon = "icon_0",
				LargeIcon = "icon_1",
			};

			AndroidNotificationCenter.SendNotification(androidNotification, ChannelId);
		}

		public void CancelAllScheduleNotification()
		{
			if (!this.isReady)
			{
				return;
			}

			AndroidNotificationCenter.CancelAllScheduledNotifications();
		}

		public void CancelAllDisplayNotification()
		{
			if (!this.isReady)
			{
				return;
			}

			AndroidNotificationCenter.CancelAllDisplayedNotifications();
		}

		private void RegisterChannel(string id, string channelName, string description)
		{
			var chanel = new AndroidNotificationChannel(id, channelName, description, Importance.Default);
			AndroidNotificationCenter.RegisterNotificationChannel(chanel);
		}
	}
}

#endif