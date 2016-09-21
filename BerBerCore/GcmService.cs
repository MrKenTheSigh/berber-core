using System;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Util;
using Android.OS;
using Android.Support.V4.App;
using Android.Media;

using Gcm.Client;

//VERY VERY VERY IMPORTANT NOTE!!!!
// Your package name MUST NOT start with an uppercase letter.
// Android does not allow permissions to start with an upper case letter
// If it does you will get a very cryptic error in logcat and it will not be obvious why you are crying!
// So please, for the love of all that is kind on this earth, use a LOWERCASE first letter in your Package Name!!!!
namespace BerBerCore
{
	//You must subclass this!
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project ID.
		//  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
		//  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
		//  where 785671162406 is the project id, which is the SENDER_ID to use!
		public static string[] SENDER_IDS = new string[] {Ini.GCM_SENDER_ID};

		public const string TAG = "PushSharp-GCM";
	}

	[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{
		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		const string TAG = "GCM-SAMPLE";

		protected override void OnRegistered (Context context, string registrationId)
		{
			Log.Verbose(TAG, "GCM Registered: " + registrationId);
			//Eg: Send back to the server
			//	var result = wc.UploadString("http://your.server.com/api/register/", "POST", 
			//		"{ 'registrationId' : '" + registrationId + "' }");
			
			KUserDefault.DeviceToken = registrationId;

			//createNotification("GCM Registered...", "The device has been Registered, Tap to View!");
		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			Log.Verbose(TAG, "GCM Unregistered: " + registrationId);
			//Remove from the web service
			//	var wc = new WebClient();
			//	var result = wc.UploadString("http://your.server.com/api/unregister/", "POST",
			//		"{ 'registrationId' : '" + lastRegistrationId + "' }");

			//createNotification("GCM Unregistered...", "The device has been unregistered, Tap to View!");
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			Log.Info(TAG, "GCM Message Received!");

			//var msg = new StringBuilder();
			string msg = string.Empty;
			int type = 0;
			//long notiID = 0;
			string url = string.Empty;//"www.google.com";

			if (intent != null && intent.Extras != null)
			{
				foreach (var key in intent.Extras.KeySet()) {
					//msg.AppendLine (key + "=" + intent.Extras.Get (key).ToString ());

					if (key == "msg") {
						msg = intent.Extras.Get (key).ToString ();
					} else if (key == "type") {
						int.TryParse (intent.Extras.Get (key).ToString (), out type);
					} else if (key == "url") {
						url = intent.Extras.Get (key).ToString ();
					}

				}
			}

			GF.devLog ("[type] ============== " + type.ToString() + " =========");
			GF.devLog ("[url] ============== " + url + " =========");

			//WS_TD888.device_setReceived (notiID);


			KUserDefault.addNotiCnt (type, 1);
			if (type < Ini.NotiIdList.Length) {
				updateNotification (KUserDefault.getNotiCnt (type), msg, Ini.NotiIdList [type], url);
				BadgeControl.setBadge (context, KUserDefault.getSumNotiCnt());
			}
			/*
			switch (type) {
				case 0:
					KUserDefault.addNotiCnt_0 (1);
					updateNotification (KUserDefault.getNotiCnt_0 (), msg, Ini.NotiIdList[0]);
					break;

				case 1:
					KUserDefault.addNotiCnt_1 (1);
					updateNotification (KUserDefault.getNotiCnt_1 (), msg, Ini.NotiIdList[1]);
					break;

				case 2:
					KUserDefault.addNotiCnt_2 (1);
					updateNotification (KUserDefault.getNotiCnt_2 (), msg, Ini.NotiIdList[2]);
					break;

			}

			if (type <= 2) {
				GF.devLog ("[NotiCnt] " + KUserDefault.getNotiCnt_0 () + ", " + KUserDefault.getNotiCnt_1 () + ", " + KUserDefault.getNotiCnt_2 ());
				BadgeControl.setBadge (context, KUserDefault.getNotiCnt_0 () + KUserDefault.getNotiCnt_1 () + KUserDefault.getNotiCnt_2 ());
			}
			*/

			/*
			//Store the message
			var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
			var edit = prefs.Edit();
			edit.PutString("last_msg", msg.ToString());
			edit.Commit();

			createNotification("GCM Sample", "Message Received for GCM Sample... Tap to View!");


			Log.Info(TAG, "GCM Message Received!");
			*/

			//GF.devLog ("[== Noti Msg ==] " + msg.ToString ());
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			Log.Warn(TAG, "Recoverable Error: " + errorId);

			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError (Context context, string errorId)
		{
			Log.Error(TAG, "GCM Error: " + errorId);
		}

		/*
		void createNotification(string title, string desc)
		{
			//Create notification
			var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

			//Create an intent to show ui
			var uiIntent = new Intent(this, typeof(MainActivity));

			//Create the notification
			var notification = new Notification(Android.Resource.Drawable.SymActionEmail, title);

			//Auto cancel will remove the notification once the user touches it
			notification.Flags = NotificationFlags.AutoCancel;

			//Set the notification info
			//we use the pending intent, passing our ui intent over which will get called
			//when the notification is tapped.
			notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

			//Show the notification
			notificationManager.Notify(1, notification);
		}
		*/

		private void updateNotification(int num, string msg, int notificationID, string url)
		{
			// These are the values that we want to pass to the next activity
			Bundle valuesForActivity = new Bundle();
			valuesForActivity.PutInt(GF.CALL_FROM_NOTI_TYPE, notificationID);
			valuesForActivity.PutString(GF.URL_FROM_NOTI, url);

			// Create the PendingIntent with the back stack
			// When the user clicks the notification, SecondActivity will start up.
			Intent resultIntent = new Intent(this, typeof(MainActivity));
			resultIntent.PutExtras(valuesForActivity); // Pass some values to SecondActivity.

			Android.Support.V4.App.TaskStackBuilder stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
			stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
			stackBuilder.AddNextIntent(resultIntent);

			PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

			//Android.Net.Uri alarmSound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
			Android.Net.Uri alarmSound; //= Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.noti_sound_coin);
			string msgText;
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
				//.SetNumber (num); // Display the count in the Content Info

			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(notificationID, builder.Build());

		}


	}
}

