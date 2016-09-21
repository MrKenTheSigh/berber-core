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
using Android.Support.V4.App;
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




		}
		#endregion

		private void updateNotification (int num, string msg, int notificationID)
		{
			if (KUserDefault.MsgID == 0) {
				return;
			}

			// These are the values that we want to pass to the next activity
			Bundle valuesForActivity = new Bundle ();
			valuesForActivity.PutInt (GF.CALL_FROM_NOTI_TYPE, notificationID);

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent (this, typeof (MainActivity));
			resultIntent.PutExtras (valuesForActivity); // Pass some values to SecondActivity.

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create (this);
			stackBuilder.AddParentStack (Java.Lang.Class.FromType (typeof (MainActivity)));
			stackBuilder.AddNextIntent (resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent (0, (int)PendingIntentFlags.UpdateCurrent);

			//Android.Net.Uri alarmSound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			Android.Net.Uri alarmSound; //= Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.noti_sound_coin);
			string msgText;
			/*
			switch (notificationID) {
				case Ini.NotiIdList[0]:
					alarmSound = Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.noti_sound_coin);
					msgText = GetString (Resource.String.new_message_public);
					break;
				case Ini.NotiIdList[1]:
					alarmSound = Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.coins_drop_1);
					msgText = GetString (Resource.String.new_message_product);
					break;
				case Ini.NotiIdList[2]:
				default:
					alarmSound = Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.noti_sound_coin);
					msgText = GetString (Resource.String.new_message);
					break;
			}
			*/

			if (Ini.checkNotiId (notificationID)) {
				alarmSound = Ini.DefaultNotiAudio;//Ini.NotiAudioList [notificationID];
				msgText = Ini.NotiTitleList [notificationID];
			} else {
				alarmSound = Ini.DefaultNotiAudio;
				msgText = Ini.DefaultNotiTitle;
			}

			GF.devLog ("[GcmService] notificationID: " + notificationID + "  msgText: " + msgText);

			// Build the notification
			NotificationCompat.Builder builder = new NotificationCompat.Builder (this)
				.SetAutoCancel (true) // dismiss the notification from the notification area when the user clicks on it
				.SetContentIntent (resultPendingIntent) // start up this activity when the user clicks the intent.
				.SetContentTitle (num.ToString () + msgText) // Set the title
				.SetSmallIcon (Resource.Drawable.Icon) // This is the icon to display
				.SetSound (alarmSound, 5)
				.SetContentText (msg.Replace ("</br>", " ")); // the message to display.
															  //.SetContentInfo ("info")
															  //.SetNumber(num) // Display the count in the Content Info

			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService (NotificationService);
			notificationManager.Notify (notificationID, builder.Build ());

		}


	}



}

