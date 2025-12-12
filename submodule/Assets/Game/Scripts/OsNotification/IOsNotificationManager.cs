namespace Antada.Libs.OsNotification
{
	using UniRx.Async;

	public interface IOsNotificationManager
	{
		UniTask Init();
		bool IsReady();
		void ScheduleNotification(NotificationData data);
		void CancelAllScheduleNotification();
		void CancelAllDisplayNotification();
	}
}