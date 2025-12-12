#if UNITY_EDITOR

namespace Antada.Libs.OsNotification
{
	using UniRx.Async;

	public class EditorNotificationManager : IOsNotificationManager
	{
		public async UniTask Init()
		{
		}

		public bool IsReady()
		{
			return true;
		}

		public void ScheduleNotification(NotificationData data)
		{
            //UnityEngine.Debug.LogError($"{data.FireTime.ToString()}-{data.Title}-{data.Body}");
		}

		public void CancelAllScheduleNotification()
		{
		}

		public void CancelAllDisplayNotification()
		{
		}
	}
}

#endif