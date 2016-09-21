using System;
using System.Collections.Generic;


namespace BerBerCore
{
	
	public class Ini
	{
		public const bool DevMode = true;

		public const int WebViewTimeoutMS = 10000;
		public const int WebViewTimeoutRetry = 3;

		//
		public const string URL_MAIN = "file:///android_asset/Web/index.html";//"https://wsapi.berberui.rocks/demo/ppplayer/index.html";//
		public const string URL_ERR = "file:///android_asset/Web/error_page.html";
		public const string URL_BLANK = "about:blank";

		public const string CMD_PRFIX = "berber:";

		//
		public const bool ShowLaunchScreen = false;
		public const int LaunchScreenTime_MS = 5000;


		//
		public const string GCM_SENDER_ID = "310769733638";//AIzaSyAbwC3z2pJcT5lUzuRPhayaKLo1xIkCyD4(api key)
		#region = Notification =
		public static string PackageName = string.Empty;
		public const string DefaultNotiCntStr = "0,0,0";//用於 KUserDefult. 記錄不同 type 通知的數量. 隨通知 type 的數量調整.
		public static int[] NotiIdList = new int[]{
			0,
			1,
			2
		};
		public const string DefaultNotiTitle = "通知";//收到的 NotiId 不存在於 NotiIdList 時使用
		public static Dictionary<int, string> NotiTitleList = new Dictionary<int, string>
		{
			{NotiIdList[0], "通知"},
			{NotiIdList[1], "通知"},
			{NotiIdList[2], "通知"}
		};
		public static Android.Net.Uri DefaultNotiAudio = Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification);//目前全都用預設
		public static Dictionary<int, Android.Net.Uri> NotiAudioList = new Dictionary<int, Android.Net.Uri>
		{
			{NotiIdList[0], Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification)},//Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.noti_sound_coin.ToString())},
			{NotiIdList[1], Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification)},//Android.Net.Uri.Parse ("android.resource://" + PackageName + "/" + Resource.Raw.coins_drop_1.ToString())},
			{NotiIdList[2], Android.Media.RingtoneManager.GetDefaultUri(Android.Media.RingtoneType.Notification)}
		};
		#endregion



		//
		public Ini ()
		{
		}

		public static bool checkNotiId(int notiId){
			for (int i = 0; i < NotiIdList.Length; i++) {
				if (NotiIdList [i] == notiId) {
					return true;
				}
			}
			return false;
		}

	}

}

