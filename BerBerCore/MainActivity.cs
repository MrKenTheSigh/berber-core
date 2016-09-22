using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.Threading;
using Newtonsoft.Json;

using WebSocket4Net;



namespace BerBerCore
{
	[Activity (
		MainLauncher = true,
		Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
		//ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize,
		Exported = true,//for API_19, 按了notification卻不啟動activity的問題
		Icon = "@mipmap/icon"
	)]
	public class MainActivity : Activity
	{
		public RelativeLayout panel;
		private O08WebView webview;

		public CookieManager cookieManager;

		private Vibrator vibrator;

		//public bool enableBackBtn = true;
		//public string backBtnFunc = string.Empty;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			#region splash
			if (Ini.ShowLaunchScreen) {
				StartActivity (typeof (SplashActitvity));
			}

			#endregion

			//
			vibrator = (Vibrator)GetSystemService (VibratorService);

			//
			RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			panel = new RelativeLayout (this);
			SetContentView (panel, lp);

			webview = new O08WebView (this);
			webview.SetWebViewClient (new O08WebViewClient (this));
			webview.SetWebChromeClient (new O08WebChromeClient (webview));//for debug
			webview.Settings.JavaScriptEnabled = true;
			//if ((int)Android.OS.Build.VERSION.SdkInt >= 19) {  //UNDONE test for performance
			//	webview.SetLayerType (LayerType.Hardware, null);
			//} else {
			//	webview.SetLayerType (LayerType.Software, null);
			//}
			//if (useMeta) {
			//	webview.Settings.UseWideViewPort = true;
			//	webview.Settings.LoadWithOverviewMode = true;
			//}
			//if (scalable) {
			//	webview.Settings.BuiltInZoomControls = true;
			//	webview.Settings.DisplayZoomControls = false;
			//}

			var rlp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			panel.AddView (webview, rlp);
			webview.LoadUrl (Ini.URL_MAIN);

			//
			cookieManager = CookieManager.Instance;
			cookieManager.SetAcceptCookie (true);
			if (!string.IsNullOrWhiteSpace (KUserDefault.Cookie)) {
				string [] cookieArr = KUserDefault.Cookie.Split (';');
				for (int i = 0; i < cookieArr.Length; i++) {
					cookieManager.SetCookie (Ini.URL_MAIN, cookieArr [i]);
				}
			}


		}
		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			GF.devLog ("[OnResume]");

			// NOTE 這不會啟動多重的同個 service
			StartService (new Intent (this, typeof (NotificationService)));

			//
			if (Intent.Extras != null) {
				var msg = Intent.Extras.GetString ("msg_from_noti", string.Empty);
				GF.devLog ("[OnResume] extra: " + msg);
			} else { 
				GF.devLog ("[OnResume] no extras");
			}

			// TODO 解析 extra 並做對應動作



		}
		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);
			GF.devLog ("[OnWindowFocusChanged]");
		}
		protected override void OnPause ()
		{
			base.OnPause ();
			GF.devLog ("[OnPause]");

			if (cookieManager != null) {
				GF.devLog ("[OnPause][Cookie] " + cookieManager.GetCookie (Ini.URL_MAIN));
				KUserDefault.Cookie = cookieManager.GetCookie (Ini.URL_MAIN);
			}

		}
		protected override void OnStop ()
		{
			base.OnStop ();
			GF.devLog ("[OnStop]");

			webview.ClearCache (true);

		}
		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			GF.devLog ("[OnDestroy]");

			if (cookieManager != null) {
				GF.devLog ("[OnDestroy][Cookie] " + cookieManager.GetCookie (Ini.URL_MAIN));
				KUserDefault.Cookie = cookieManager.GetCookie (Ini.URL_MAIN);
			}


		}


		//
		public void doVibrate (long [] pattern)
		{
			//pattern: <wait>, <do>, <wait>, <do>, ...
			vibrator.Vibrate (pattern, -1);//-1 means no repeat
		}


	}


}
