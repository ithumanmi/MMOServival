#if UNITY_IOS

namespace Antada.Libs.OsNotification
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UniRx.Async;
	using Unity.Notifications.iOS;
	using System;

	public class MacNotificationManager : IOsNotificationManager
	{
		private bool isReady;

		public MacNotificationManager()
		{
			this.isReady = false;
		}

		public async UniTask Init()
		{
			using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true))
			{
				while (!req.IsFinished)
				{
					await UniTask.Yield();
				}

				if (req.Granted)
				{
					this.isReady = true;
				}
			}
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

			var timeTrigger = new iOSNotificationTimeIntervalTrigger()
			{
				TimeInterval = data.FireTime - DateTime.Now,
				Repeats = false,
			};

			var notification = new iOSNotification()
			{
				Title = data.Title,
				Body = data.Body,
				ShowInForeground = true,
				ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
				Trigger = timeTrigger,
			};

			iOSNotificationCenter.ScheduleNotification(notification);
		}

		public void CancelAllScheduleNotification()
		{
			if (!this.isReady)
			{
				return;
			}

			iOSNotificationCenter.RemoveAllScheduledNotifications();
		}

		public void CancelAllDisplayNotification()
		{
			if (!this.isReady)
			{
				return;
			}

			iOSNotificationCenter.RemoveAllDeliveredNotifications();
		}
	}
}

#endif