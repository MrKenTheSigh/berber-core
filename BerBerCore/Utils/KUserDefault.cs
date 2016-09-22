using System;
using Android.App;
using Android.Content;
using Android.Preferences;


namespace BerBerCore
{
	public class KUserDefault
	{
		private const string DEVICE_TOKEN = "DEVICE_TOKEN";
		private const string COOKIE = "COOKIE";

		private static ISharedPreferences settings;

		//
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


	}

}

