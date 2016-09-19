using System;
using Android.App;
using Android.Content;
using Android.Preferences;


namespace BerBerCore
{
	public class KUserDefault
	{
		#region notification
		private const string MSG_ID = "MSG_ID";//UNDONE 無GCM模式會用到
		private const string NOTI_CNT_STR = "NOTI_CNT_STR";//

		//for NotificationServiceW
		private const string SERVICE_PUSH_URL = "SERVICE_PUSH_URL";
		#endregion

		private const string DEVICE_TOKEN = "DEVICE_TOKEN";

		private const string COOKIE = "COOKIE";
		private const string LOGINED = "LOGINED";
		private const string ACCT = "ACCT";

		private const string NOTIFY_URL = "NOTIFY_URL";

		private const string SYS_ROTATE = "SYS_ROTATE";
		private const string SYS_FULLSCREEN = "SYS_FULLSCREEN";
		private const string SYS_SHOW_TITLE = "SYS_SHOW_TITLE";
		private const string SYS_SCREEN_SAVER = "SYS_SCREEN_SAVER";
		private const string SYS_PC_VIEW = "SYS_PC_VIEW";
		private const string SYS_SCALABLE = "SYS_SCALABLE";


		private const string TEST_FLAG = "TEST_FLAG";

		private static ISharedPreferences settings;


		public KUserDefault ()
		{

		}

		#region notification
		//
		public static int MsgID {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetInt (MSG_ID, 0);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutInt (MSG_ID, value).Commit();
			}
		}
		//Noti Cnt 
		public static int getNotiCnt(int idx)
		{
			settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
			string noti_cnt_str = settings.GetString (NOTI_CNT_STR, Ini.DefaultNotiCntStr);
			string[] notiCntList = noti_cnt_str.Split (',');
			int cnt = 0;
			if (idx < notiCntList.Length) {
				int.TryParse (notiCntList [idx], out cnt);
			}
			return cnt;
		}
		public static int getSumNotiCnt()
		{
			settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
			string noti_cnt_str = settings.GetString (NOTI_CNT_STR, Ini.DefaultNotiCntStr);
			string[] notiCntList = noti_cnt_str.Split (',');
			int sum = 0;
			int cnt = 0;
			for (int i = 0; i < notiCntList.Length; i++) {
				int.TryParse (notiCntList [i], out cnt);
				sum += cnt;
			}
			return sum;
		}
		private static void editNotiCnt(int idx, int cnt, int edtVal)
		{
			settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
			string noti_cnt_str = settings.GetString (NOTI_CNT_STR, Ini.DefaultNotiCntStr);
			string[] notiCntList = noti_cnt_str.Split (',');

			if (idx < notiCntList.Length) {
				string new_noti_cnt_str = string.Empty;
				for (int i = 0; i < notiCntList.Length; i++) {
					if (i != idx) {
						new_noti_cnt_str += notiCntList [i];
					} else {
						new_noti_cnt_str += (edtVal + cnt).ToString ();
					}

					if (i != notiCntList.Length - 1) {
						new_noti_cnt_str += ",";
					}

				}

				settings.Edit ().PutString (NOTI_CNT_STR, new_noti_cnt_str).Commit ();

			}
		}
		public static void addNotiCnt(int idx, int cnt)
		{
			int old = getNotiCnt (idx);
			editNotiCnt (idx, cnt, old);
		}
		public static void resetNotiCnt(int idx)
		{
			int old = getNotiCnt (idx);
			editNotiCnt (idx, -old, old);
		}
		public static void resetAllNotiCnt()
		{
			settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
			settings.Edit ().PutString (NOTI_CNT_STR, Ini.DefaultNotiCntStr).Commit ();
		}

		//for NotificationServiceW
		public static string ServicePushURL {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetString (SERVICE_PUSH_URL, "about:blank");
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutString (SERVICE_PUSH_URL, value).Commit();
			}
		}

		#endregion

		public static bool Logined {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetBoolean (LOGINED, false);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutBoolean (LOGINED, value).Commit();
			}
		}

		public static string Acct {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetString (ACCT, string.Empty);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutString (ACCT, value).Commit();
			}
		}

		public static string DeviceToken {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetString (DEVICE_TOKEN, string.Empty);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutString (DEVICE_TOKEN, value).Commit();
			}
		}

		public static string NotifyURL {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetString (NOTIFY_URL, string.Empty);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutString (NOTIFY_URL, value).Commit();
			}
		}

		public static string Cookie {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetString (COOKIE, string.Empty);
			}
			set{ 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutString (COOKIE, value).Commit();
			}
		}

		public static int Rotate {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetInt (SYS_ROTATE, 90);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutInt (SYS_ROTATE, value).Commit();
			}
		}

		public static int FullScreen {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetInt (SYS_FULLSCREEN, 2);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutInt (SYS_FULLSCREEN, value).Commit();
			}
		}

		public static int ShowTitle {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetInt (SYS_SHOW_TITLE, 1);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutInt (SYS_SHOW_TITLE, value).Commit();
			}
		}

		public static int ScreenSaver {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetInt (SYS_SCREEN_SAVER, 0);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutInt (SYS_SCREEN_SAVER, value).Commit();
			}
		}

		public static bool PCView {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetBoolean (SYS_PC_VIEW, false);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutBoolean (SYS_PC_VIEW, value).Commit();
			}
		}

		public static bool Scalable {
			get { 
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				return settings.GetBoolean (SYS_SCALABLE, false);
			}
			set{
				settings = PreferenceManager.GetDefaultSharedPreferences (Application.Context);
				settings.Edit ().PutBoolean (SYS_SCALABLE, value).Commit();
			}
		}


	}

}

