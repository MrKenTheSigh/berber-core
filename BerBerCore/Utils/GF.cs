using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using Android.Content;
using Android.Content.PM;
using Android.App;
using Android.Net.Wifi;//for getMacAddress


namespace BerBerCore
{
	public class GF
	{
		#region 未重製, 加密及通知相關
		public const string IV = "81feaa24";

		public const int INTENT_LOGIN = 1;
		public const int INTENT_CMD_LOGIN = 1;

		public const int NOTIFICATION_ID_NONE = -1;
		public const string CALL_FROM_NOTI_TYPE = "CALL_FROM_NOTI_TYPE";
		#endregion
		public const string URL_FROM_NOTI = "URL_FROM_NOTI";


		public GF ()
		{
		}

		public static void devLog (string msg)
		{
			if (Ini.DevMode) {
				Console.WriteLine ("[BerBerCore] " + msg);
			}
		}

		public static string getVER (Context context)
		{
			string [] arrVer = context.PackageManager.GetPackageInfo (context.PackageName, 0).VersionName.Split ('.');
			devLog ("[Ver] " + context.PackageManager.GetPackageInfo (context.PackageName, 0).VersionName);
			if (arrVer.Length == 3) {
				return arrVer [2];
			}
			return "0";
		}

		//
		public static string getMD5 (string str)
		{
			byte [] data = System.Text.Encoding.ASCII.GetBytes (str);
			System.Security.Cryptography.MD5CryptoServiceProvider md5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider ();
			return System.Convert.ToBase64String (md5Provider.ComputeHash (data));
		}

		public static string Encrypt3DES (string msg, string key, string iv)
		{
			string resutlt = string.Empty;
			byte [] arrMsg = System.Text.Encoding.UTF8.GetBytes (msg);

			var provider = new System.Security.Cryptography.TripleDESCryptoServiceProvider () {
				Key = System.Text.Encoding.UTF8.GetBytes (GF.getMD5 (key)), //Convert.FromBase64String(GF.getMD5(key)),
				IV = System.Text.Encoding.UTF8.GetBytes (iv),
				Mode = System.Security.Cryptography.CipherMode.CBC,
				Padding = System.Security.Cryptography.PaddingMode.PKCS7
			};
			System.Security.Cryptography.ICryptoTransform ct = provider.CreateEncryptor ();

			try {
				var arrResult = ct.TransformFinalBlock (arrMsg, 0, arrMsg.Length);
				resutlt = Convert.ToBase64String (arrResult);
			} catch {

			}

			return resutlt;
		}
		public static string Decrypt3DES (string msg, string key, string iv)
		{
			string result = string.Empty;
			byte [] arrMsg = Convert.FromBase64String (msg);

			var provider = new System.Security.Cryptography.TripleDESCryptoServiceProvider () {
				Key = System.Text.Encoding.UTF8.GetBytes (GF.getMD5 (key)), //Convert.FromBase64String(GF.getMD5(key)),
				IV = System.Text.Encoding.UTF8.GetBytes (iv),
				Mode = System.Security.Cryptography.CipherMode.CBC,
				Padding = System.Security.Cryptography.PaddingMode.PKCS7
			};
			System.Security.Cryptography.ICryptoTransform ct = provider.CreateDecryptor ();

			try {
				var arrResult = ct.TransformFinalBlock (arrMsg, 0, arrMsg.Length);
				result = System.Text.Encoding.UTF8.GetString (arrResult);
			} catch {
				//Console.Write ("X");
			}

			return result;
		}

	}

	public class BadgeControl
	{

		public static void setBadge (Context context, int count)
		{
			String launcherClassName = getLauncherClassName (context);
			if (launcherClassName == null) {
				return;
			}

			Intent intent = new Intent ("android.intent.action.BADGE_COUNT_UPDATE");
			intent.PutExtra ("badge_count", count);
			intent.PutExtra ("badge_count_package_name", context.PackageName);
			intent.PutExtra ("badge_count_class_name", launcherClassName);
			context.SendBroadcast (intent);
		}


		public static String getLauncherClassName (Context context)
		{

			Intent intent = new Intent (Intent.ActionMain);
			intent.AddCategory (Intent.CategoryLauncher);

			System.Collections.Generic.IList<ResolveInfo> resolveInfos = context.PackageManager.QueryIntentActivities (intent, 0);
			foreach (ResolveInfo resolveInfo in resolveInfos) {
				String pkgName = resolveInfo.ActivityInfo.ApplicationInfo.PackageName;
				if (pkgName.ToLower () == context.PackageName.ToLower ()) {
					String className = resolveInfo.ActivityInfo.Name;
					return className;
				}
			}
			return null;
		}
	}


}

