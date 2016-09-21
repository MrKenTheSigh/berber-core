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

		private string url_from_noti = string.Empty;


		public CookieManager cookieManager;

		public bool enableBackBtn = true;
		public string backBtnFunc = string.Empty;


		private bool isFirstCreate = true;

		private Vibrator vibrator;


		//test
		//private WebSocket webSocket;



		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			#region splash
			if (Ini.ShowLaunchScreen) {
				StartActivity (typeof (SplashActitvity));
			}

			#endregion

			#region from notification
			//this.url_from_noti = Intent.GetStringExtra (GF.URL_FROM_NOTI);
			//GF.devLog ("[OnCreate] url_from_noti: " + this.url_from_noti);

			//KUserDefault.resetAllNotiCnt ();
			//BadgeControl.setBadge (this, KUserDefault.getSumNotiCnt ());
			#endregion

			//
			vibrator = (Vibrator)GetSystemService (Context.VibratorService);


			//
			RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			panel = new RelativeLayout (this);
			SetContentView (panel, lp);

			webview = new O08WebView (this);
			//web.SetBackgroundColor(Color.Azure);

			webview.SetWebViewClient (new KWebViewClient ((MainActivity)this));
			webview.SetWebChromeClient (new KWebChromeClient (webview));//for debug
			webview.Settings.JavaScriptEnabled = true;
			if ((int)Android.OS.Build.VERSION.SdkInt >= 19) {  //UNDONE test for performance
				webview.SetLayerType (LayerType.Hardware, null);
			} else {
				webview.SetLayerType (LayerType.Software, null);
			}
			//if (useMeta) {
			//	webview.Settings.UseWideViewPort = true;
			//	webview.Settings.LoadWithOverviewMode = true;
			//}
			//if (scalable) {
			//	webview.Settings.BuiltInZoomControls = true;
			//	webview.Settings.DisplayZoomControls = false;
			//}

			RelativeLayout.LayoutParams rlp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			panel.AddView (webview, rlp);
			webview.LoadUrl (Ini.URL_MAIN);

			//
			cookieManager = CookieManager.Instance;
			cookieManager.SetAcceptCookie (true);
			GF.devLog ("[FirstCreate][Cookie] " + KUserDefault.Cookie);
			if (!string.IsNullOrWhiteSpace (KUserDefault.Cookie)) {
				string [] cookieArr = KUserDefault.Cookie.Split (';');
				for (int i = 0; i < cookieArr.Length; i++) {
					cookieManager.SetCookie (Ini.URL_MAIN, cookieArr [i]);
				}

				//cookieManager.SetCookie(GF.DEFAULT_ADDR, KUserDefault.getCookie());
			}




			////test
			//webSocket = new WebSocket ("ws://echo.websocket.org");//"ws://localhost:2012/");
			//webSocket.Opened += websocket_Opened;
			//webSocket.Error += websocket_Error;
			//webSocket.Closed += websocket_Closed;
			//webSocket.MessageReceived += websocket_MessageReceived;

			//webSocket.Open ();

		}
		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);


			//#region = adjust webview =
			//int naviFix = 0;
			//if (KUserDefault.ShowTitle == 1) {
			//	naviFix = naviBarH;
			//}

			//RelativeLayout.LayoutParams rlp = new RelativeLayout.LayoutParams (
			//	Resources.DisplayMetrics.WidthPixels,
			//	Resources.DisplayMetrics.HeightPixels - naviFix
			//);
			//rlp.TopMargin = naviFix;
			//webview.LayoutParameters = rlp;
			//#endregion

		}

		protected override void OnResume ()
		{
			base.OnResume ();

			GF.devLog ("Resume");


			if (Intent.Extras != null) {
				var msg = Intent.Extras.GetString ("msg_from_noti", string.Empty);
				GF.devLog ("[OnResume] extra: " + msg);
			} else { 
				GF.devLog ("[OnResume] no extras");
			}


				#region == Service for NO_GCM ==
				//UNDONE 不支援GCM的設備, 須主動項server更新資訊. 待重製
				/*
			if(NO_GCM){
				if(KUserDefault.getLogined()){
					StopService(new Intent (this, typeof(NotificationService)));
					StartService (new Intent (this, typeof(NotificationService)));
				}else{
					StopService(new Intent (this, typeof(NotificationService)));

					if(((RegisterState)KUserDefault.getRegisterState()) != RegisterState.REGISTERED_DOMAIN){	
						//WS_TD888.device_register ();
					}

				}
			}
			*/
				#endregion

		}
		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);

			GF.devLog ("OnWindowFocusChanged");

		}
		protected override void OnPause ()
		{
			base.OnPause ();

			GF.devLog ("Pause");

			if (cookieManager != null) {
				KUserDefault.Cookie = cookieManager.GetCookie (Ini.URL_MAIN);
			}

		}
		protected override void OnStop ()
		{
			base.OnStop ();

			webview.ClearCache (true);

		}
		protected override void OnDestroy ()
		{
			base.OnDestroy ();

			GF.devLog ("[OnDestroy][Cookie] " + cookieManager.GetCookie (Ini.URL_MAIN));
			//目前為必存
			KUserDefault.Cookie = cookieManager.GetCookie (Ini.URL_MAIN);

		}


		////TODO 此測試是要用在 service 的. 在背景執行並持續與 server 保持連線, 以取得推送來的資料. (理論上是用於不支援 GCM 的地方)
		//private void websocket_Opened (object sender, EventArgs e)
		//{
		//	GF.devLog ("[WS][opened]");
		//	webSocket.Send ("Hello World!");
		//}
		//private void websocket_Error (object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		//{
		//	GF.devLog ("[WS][err]");
		//}
		//private void websocket_Closed (object sender, EventArgs e)
		//{
		//	GF.devLog ("[WS][closed]");
		//}
		//private void websocket_MessageReceived (object sender, MessageReceivedEventArgs e)
		//{
		//	var msg = e.Message; 
		//	GF.devLog ("[WS][msg] " + msg);

		//}


	}


}
