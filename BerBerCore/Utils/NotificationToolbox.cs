using System;
using Android.App;
using Android.Media;
using Android.Content;

namespace BerBerCore
{
	public class NotificationToolbox
	{
		public NotificationToolbox ()
		{
		}


		public static void updateNotification (Context ctx, string title, string content, string extra)
		{
			//NOTE ref https://developer.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/local_notifications_in_android/

			var intent = new Intent (ctx, typeof (MainActivity));
			intent.PutExtra ("msg_from_noti", extra);

			TaskStackBuilder stackBuilder = TaskStackBuilder.Create (ctx);
			stackBuilder.AddParentStack (Java.Lang.Class.FromType (typeof (MainActivity)));
			stackBuilder.AddNextIntent (intent);

			const int pendingIntentId = 0;
			PendingIntent pendingIntent = stackBuilder.GetPendingIntent (pendingIntentId, PendingIntentFlags.OneShot);

			Notification.Builder builder = new Notification.Builder (ctx)
				.SetContentIntent (pendingIntent)
				.SetContentTitle (title)
				.SetContentText (content)
				.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
				.SetDefaults (NotificationDefaults.Lights | NotificationDefaults.Vibrate) // 如果設定了 NotificationDefaults.Sound, 自訂的音效就會被預設的取代

				//.SetSound (RingtoneManager.GetDefaultUri (RingtoneType.Alarm))//.SetSound (alarmSound, 5) // 更多聲音控制去參考 diantou
				.SetSound(Android.Net.Uri.Parse ("android.resource://" + Application.Context.PackageName + "/" + Resource.Raw.SMW_Coin))

				//.SetWhen (Java.Lang.JavaSystem.CurrentTimeMillis () + 120000)// 這只是設定通知上顯示的時間, 並非此通知被發出的時間
				.SetSmallIcon (Resource.Drawable.Icon);

			Notification notification = builder.Build ();

			var notificationManager = ctx.GetSystemService (Context.NotificationService) as NotificationManager;

			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);

		}


	}
}
