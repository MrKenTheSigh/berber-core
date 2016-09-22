using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using Android.Webkit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocket4Net;


namespace BerBerCore
{
	[Service]
	public class NotificationService : Service
	{
		private WebSocket webSocket;


		public override void OnCreate ()
		{
			base.OnCreate ();


			webSocket = new WebSocket ("ws://echo.websocket.org");//"ws://localhost:2012/");
			webSocket.Opened += websocket_Opened;
			webSocket.Error += websocket_Error;
			webSocket.Closed += websocket_Closed;
			webSocket.MessageReceived += websocket_MessageReceived;

			webSocket.Open ();


		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			GF.devLog ("[SERVICE] OnStartCommand");

			//updateNotification(result[2].ToDoListCount, result[2].ToDoListTitle, GF.NOTIFICATION_ID_2);
			//BadgeControl.setBadge (ApplicationContext, KUserDefault.getNotiCnt_0 () + KUserDefault.getNotiCnt_1 () + KUserDefault.getNotiCnt_2 ());

			return StartCommandResult.Sticky;//base.OnStartCommand (intent, flags, startId);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();

			GF.devLog ("[SERVICE] OnDestroy");

			webSocket.Close ();

		}

		public override IBinder OnBind (Intent intent)
		{
			GF.devLog ("[SERVICE] OnBind");
			return null;
		} 

		#region websocket
		private void websocket_Opened (object sender, EventArgs e)
		{
			GF.devLog ("[WS][opened]");

			var t = new Thread (() => {
				for (int i = 0; i < 10; i++) {
					webSocket.Send ("Hello World! [" + i + "]");
					Thread.Sleep (10000);
				}
				//StopSelf ();

			});
			t.Start ();


		}
		private void websocket_Error (object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
			GF.devLog ("[WS][err]");

			//TODO 取得 error code, 處理重新連線
			//TODO 通知

		}
		private void websocket_Closed (object sender, EventArgs e)
		{
			GF.devLog ("[WS][closed]");
		}
		private void websocket_MessageReceived (object sender, MessageReceivedEventArgs e)
		{
			var msg = e.Message;
			GF.devLog ("[WS][msg] " + msg);

			//TODO 用以下方式轉換資料
			//JObject restoredObject = JsonConvert.DeserializeObject<JObject> (msg);

			//GF.devLog ("[method] " + restoredObject ["method"]);
			//switch (restoredObject ["method"].ToString ()) {
			//	case "yo":
			//		GF.devLog ("[data] N/A");
			//		break;

			//	case "yo2":
			//		GF.devLog ("[data] " + restoredObject ["data"]);
			//		GF.devLog ("[data.name] " + restoredObject ["data"] ["name"]);
			//		GF.devLog ("[data.phone] " + restoredObject ["data"] ["phone"]);
			//		break;


			//}


			//TODO 取得資料後用發通知
			//  是否把 extra 用於 json ?!
			//updateNotification (title, content, extra);



		}
		#endregion

		public void updateNotification (string title, string content, string extra)
		{
			//NOTE ref https://developer.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/local_notifications_in_android/

			Intent intent = new Intent (this, typeof (MainActivity));
			intent.PutExtra ("msg_from_noti", extra);

			TaskStackBuilder stackBuilder = TaskStackBuilder.Create (this);
			stackBuilder.AddParentStack (Java.Lang.Class.FromType (typeof (MainActivity)));
			stackBuilder.AddNextIntent (intent);

			const int pendingIntentId = 0;
			PendingIntent pendingIntent =
				stackBuilder.GetPendingIntent (pendingIntentId, PendingIntentFlags.OneShot);

			Notification.Builder builder = new Notification.Builder (this)
				.SetContentIntent (pendingIntent)
				.SetContentTitle (title)
				.SetContentText (content)
				//.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
				//.SetSound (alarmSound, 5) // 更多聲音控制去參考 diantou
				.SetSmallIcon (Resource.Drawable.Icon);

			Notification notification = builder.Build ();

			NotificationManager notificationManager =
				GetSystemService (Context.NotificationService) as NotificationManager;

			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);

		}

	}



}

