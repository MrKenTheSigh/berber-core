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
		private KWebView webview;

		private string url_from_noti = string.Empty;


		public CookieManager cookieManager;

		public bool enableBackBtn = true;
		public string backBtnFunc = string.Empty;


		public bool NO_GCM = false;

		private bool isFirstCreate = true;

		private Vibrator vibrator;


		private string URL = "file:///android_asset/Web/index.html";//"https://wsapi.berberui.rocks/demo/ppplayer/index.html";//



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
			RelativeLayout.LayoutParams lp = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
			panel = new RelativeLayout (this);
			SetContentView (panel, lp);

			webview = new KWebView (this);
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

			RelativeLayout.LayoutParams rlp = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
			panel.AddView (webview, rlp);
			webview.LoadUrl (URL);

			//
			cookieManager = CookieManager.Instance;
			cookieManager.SetAcceptCookie (true);
			GF.devLog ("[FirstCreate][Cookie] " + KUserDefault.Cookie);
			if (!string.IsNullOrWhiteSpace (KUserDefault.Cookie)) {
				string [] cookieArr = KUserDefault.Cookie.Split (';');
				for (int i = 0; i < cookieArr.Length; i++) {
					cookieManager.SetCookie (GF.URL_Now, cookieArr [i]);
				}

				//cookieManager.SetCookie(GF.DEFAULT_ADDR, KUserDefault.getCookie());
			}

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
				KUserDefault.Cookie = cookieManager.GetCookie (GF.URL_Now);
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

			GF.devLog ("[OnDestroy][Cookie] " + cookieManager.GetCookie (GF.URL_Now));
			//目前為必存
			KUserDefault.Cookie = cookieManager.GetCookie (GF.URL_Now);

		}


	}


	#region ==== Other Classes ====

	#region = web =

	public class KWebView : WebView
	{
		private MainActivity mainActivity;

		private bool touchEnabled = true;

		public string urlToRetry = GF.PageDefault;


		public KWebView (Context context)
			: base (context)
		{
			mainActivity = (MainActivity)context;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			if (touchEnabled == false) {
				return true;
			}

			return base.OnTouchEvent (e);
		}

		public void setTouchEnabled (bool enabled)
		{
			this.touchEnabled = enabled;
		}
		public void doBackToSurface ()
		{
			setTouchEnabled (true);
		}

		public override void EvaluateJavascript (string script, IValueCallback resultCallback)
		{
			if ((int)Android.OS.Build.VERSION.SdkInt >= 19) {
				base.EvaluateJavascript (script, resultCallback);
			} else {
				this.LoadUrl ("javascript:" + script);
			}
		}

		/*
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}
		*/

	}

	public class KWebViewClient : WebViewClient
	{
		private MainActivity mainActivity;
		private WebView webview;
		private int retryCnt = 1;

		private Thread timeoutChecking = null;
		private bool keepTimeoutChecking = true;
		private const int CD = (int)(Ini.WebViewTimeoutMS / 500);
		private int cd_500ms = CD;

		public KWebViewClient (MainActivity mainActivity)
		{
			this.mainActivity = mainActivity;
		}

		#region = timeout =
		private void startTimeoutChecking ()
		{
			stopTimeoutChecking ();
			if (timeoutChecking == null) {
				timeoutChecking = new Thread (new ThreadStart (delegate {
					try {
						while (keepTimeoutChecking) {
							if (--cd_500ms < 0) {
								GF.devLog ("[WebView][Timeout] " + this.retryCnt);

								string addr = GF.PageDefault;
								addr = addr.Replace (Ini.DefaultPathOrQuery, string.Empty);
								addr = addr.Replace ("http://", string.Empty);
								addr = addr.Replace ("https://", string.Empty);
								KToast.showCenterToast (mainActivity, "伺服器(" + addr + ")無法服務 [" + this.retryCnt + "]", ToastLength.Long);

								if (this.retryCnt >= Ini.WebViewTimeoutRetry) {
									mainActivity.RunOnUiThread (delegate {
										this.webview.LoadUrl (GF.LOCAL_PAGE_ERR);
									});

								} else {
									this.retryCnt++;
									mainActivity.RunOnUiThread (delegate {
										this.webview.Reload ();
									});

								}

								break;//

							} else {
								GF.devLog ("[cd_500ms] " + cd_500ms);
								Thread.Sleep (500);
							}
						}
					} catch {
						timeoutChecking = null;
					}

				}));
				timeoutChecking.Start ();
				GF.devLog ("== TimeoutChecking Started ==");
			}
		}
		private void stopTimeoutChecking ()
		{
			if (timeoutChecking != null) {
				keepTimeoutChecking = false;
				if (timeoutChecking.IsAlive) {
					timeoutChecking.Abort ();
				}
				timeoutChecking = null;
			}
			this.retryCnt = 0;
			this.cd_500ms = CD;
			GF.devLog ("== TimeoutChecking Stoped ==");
		}
		#endregion

		#region other native events
		public override void OnPageStarted (WebView view, string url, Android.Graphics.Bitmap favicon)
		{
			this.webview = view;

			#region = timeout =
			startTimeoutChecking ();
			#endregion

			GF.devLog ("OnPageStarted: " + url);

			view.ClearCache (true);

			base.OnPageStarted (view, url, favicon);
		}
		public override void OnPageFinished (WebView view, string url)
		{
			base.OnPageFinished (view, url);

			#region = timeout =
			stopTimeoutChecking ();
			#endregion

			if (url == GF.LOCAL_PAGE_ERR) {
				KWebView kWebView = (KWebView)view;
				view.EvaluateJavascript ("setUrl('" + kWebView.urlToRetry + "')", null);
			}

		}

		//iframe的url在這裡才收的到. (不會觸發OnPageStarted, OnPageFinished, OnReceivedError)
		public override void OnLoadResource (WebView view, string url)
		{
			base.OnLoadResource (view, url);
			GF.devLog ("OnLoadResource: " + url);

			#region = timeout =
			this.cd_500ms = CD;
			#endregion

		}


		public override void OnReceivedError (WebView view, ClientError errorCode, string description, string failingUrl)
		{
			GF.devLog ("errorCode: " + errorCode);
			GF.devLog ("description: " + description);
			GF.devLog ("failingUrl: " + failingUrl);

			#region = timeout =
			stopTimeoutChecking ();
			#endregion

			//
			KWebView kWebView = (KWebView)view;
			kWebView.urlToRetry = failingUrl;
			kWebView.LoadUrl (GF.LOCAL_PAGE_ERR);

			//
			base.OnReceivedError (view, errorCode, description, failingUrl);
		}
		#endregion

		public override bool ShouldOverrideUrlLoading (WebView _view, string _url)
		{
			GF.devLog ("[ShouldOverrideUrlLoading] " + _url);

			KWebView view = (KWebView)_view;
			string url = System.Web.HttpUtility.UrlDecode (_url).Replace (" ", string.Empty);

			if (url.StartsWith ("berber")) {
				
				string [] cmdArr = url.Split (':');
				processCMD (view, url);


				#region backup
				//if (cmdArr [1] == "Cmd") {
				//	//= Multiple CMD =
				//	//get json
				//	string jsonStr = string.Empty;
				//	if (cmdArr [2] == "Raw") {
				//		jsonStr = url.Remove (0, 14);//remove "TD888:Cmd:Raw:"

				//	} else if (cmdArr [2] == "Base64") {
				//		if (cmdArr.Length == 4) {
				//			string jsonStr_encrypted = cmdArr [3];
				//			jsonStr = System.Text.Encoding.Default.GetString (Convert.FromBase64String (jsonStr_encrypted));
				//		}
				//	}

				//	try {
				//		JMultiCMD jMultiCMD = JsonConvert.DeserializeObject<JMultiCMD> (jsonStr);

				//		//process CMD
				//		for (int i = 0; i < jMultiCMD.TD888.Count; i++) {
				//			GF.devLog ("[jMultiCMD] " + jMultiCMD.TD888 [i]);
				//			processTD888CMD (view, "td888:" + jMultiCMD.TD888 [i]);//為了共用原本的function, 把多重指令開頭附上 td888 (人懶～)
				//		}

				//		//UNDONE testing
				//		/*
				//		new Thread(new ThreadStart(delegate {

				//			for (int i = 0; i < jMultiCMD.TD888.Count; i++) {
				//				GF.devLog ("[jMultiCMD] " + jMultiCMD.TD888 [i]);
				//				mainActivity.RunOnUiThread(delegate {
				//					processTD888CMD (view, "td888:" + jMultiCMD.TD888 [i]);//為了共用原本的function, 把多重指令開頭附上 td888 (人懶～)
				//				});
				//				Thread.Sleep(100);
				//			}

				//		})).Start();
				//		*/

				//	} catch {
				//		KToast.showCenterToast (this.mainActivity, "內部指令錯誤");
				//	}

				//} else {
				//	//= Single CMD =
				//	processTD888CMD (view, url);

				//}
				#endregion

				return true;
			}

			return base.ShouldOverrideUrlLoading (view, url);
		}
		private void processCMD (WebView view, string _url)
		{
			KWebView webview = (KWebView)view;
			string url = _url.Replace (" ", string.Empty);
			string [] cmdArr = url.Split (':');
			string method = string.Empty;
			string parameters = string.Empty;

			if (cmdArr.Length == 2) {
				method = cmdArr [1];
				switch (method) {
					case "yo":
						Console.WriteLine ("yolo");
						break;


				}



			} else if (cmdArr.Length == 3) {
				method = cmdArr [1];
				parameters = cmdArr [2];

			} else {
				return;
			}








			#region parsing 

			#region fix url
			//string cmdStr = url.Remove (0, "td888:".Length);
			//for (int i = 0; i < cmdStr.Length; i++) {
			//	if (cmdStr [i] == ':' || cmdStr [i] == '/') {
			//		cmdType = cmdStr.Substring (0, i);
			//		cmdStr = cmdStr.Remove (0, cmdType.Length + 1);
			//		break;
			//	}
			//}

			////
			//if (string.IsNullOrWhiteSpace (cmdType) && cmdStr == "Close") {
			//	cmdType = "Close";
			//	cmdStr = string.Empty;
			//}

			////
			//if (cmdType == "NewWindow") {
			//	if (cmdStr.StartsWith ("0/")) {
			//		newWinScalable = false;
			//		newWinUseMeta = false;
			//		cmdStr = cmdStr.Remove (0, 2);

			//	} else if (cmdStr.StartsWith ("1/")) {
			//		newWinScalable = true;
			//		newWinUseMeta = false;
			//		cmdStr = cmdStr.Remove (0, 2);

			//	} else if (cmdStr.StartsWith ("2/")) {
			//		newWinScalable = false;
			//		newWinUseMeta = true;
			//		cmdStr = cmdStr.Remove (0, 2);

			//	} else if (cmdStr.StartsWith ("3/")) {
			//		newWinScalable = true;
			//		newWinUseMeta = true;
			//		cmdStr = cmdStr.Remove (0, 2);
			//	}

			//}

			#endregion

			////
			//if ( //先用這個篩選
			//	cmdType != "Close" &&
			//	cmdType != "CloseAll" &&
			//	cmdType != "NewWindow" &&
			//	cmdType != "NewDialog" &&
			//	cmdType != "PlaySound" &&
			//	cmdType != "ConfirmMsg" &&
			//	cmdType != "AlertMsg" &&
			//	cmdType != "VideoPlayback" &&
			//	cmdType != "VideoReframe" &&
			//	cmdType != "ServicePush"
			//) {
			//	if (cmdArr.Length == 2) {
			//		if (cmdArr [1].Contains ("/")) {
			//			//TD888:[CmdType]/[SubCmd_1]
			//			cmdType = cmdArr [1].Split ('/') [0];
			//			subCmdStr_1 = cmdArr [1].Remove (0, cmdType.Length + 1);//+1 for '/'
			//		} else {
			//			//TD888:[CmdType]
			//			cmdType = cmdArr [1];
			//		}

			//	} else if (cmdArr.Length == 3) {
			//		cmdType = cmdArr [1];
			//		if (cmdArr [2].Contains ("/")) {
			//			//TD888:[CmdType]:[SubCmd_1]/[SubCmd_2]
			//			subCmdStr_1 = cmdArr [2].Split ('/') [0];
			//			subCmdStr_2 = cmdArr [2].Split ('/') [1];

			//		} else {
			//			//TD888:[CmdType]:[SubCmd_1]
			//			subCmdStr_1 = cmdArr [2];
			//		}

			//	} else if (cmdArr.Length == 4) {
			//		//TD888:[CmdType]:[SubCmd_1]:[SubCmd_2]
			//		cmdType = cmdArr [1];
			//		subCmdStr_1 = cmdArr [2];
			//		subCmdStr_2 = cmdArr [3];

			//	} else if (cmdArr.Length == 5) {
			//		//TD888:[CmdType]:[HH:MM]:[SubCmd_2]   => for TimePicker only
			//		cmdType = cmdArr [1];
			//		subCmdStr_1 = cmdArr [2] + ":" + cmdArr [3];
			//		subCmdStr_2 = cmdArr [4];

			//	} else if (cmdArr.Length == 6) {
			//		//TD888:[CmdType]:[HH:MM:SS]:[SubCmd_2]   => for TimePicker only
			//		cmdType = cmdArr [1];
			//		subCmdStr_1 = cmdArr [2] + ":" + cmdArr [3] + ":" + cmdArr [4];
			//		subCmdStr_2 = cmdArr [5];

			//	} else {
			//		cmdType = string.Empty;
			//	}

			//}


			#endregion

			//switch (cmdType) {
			//	#region Close
			//	//case "Close":
			//	//	//
			//	//	GF.devLog ("[CMD][close]");
			//	//	this.mainActivity.backAction_webview ();

			//	//	//關閉後, 當下最表層的webview載入url
			//	//	if (!string.IsNullOrWhiteSpace (cmdStr)) {
			//	//		//load subCmdStr as URL
			//	//		GF.devLog ("[CMD][close] load: " + cmdStr);
			//	//		this.mainActivity.webViewList [this.mainActivity.webViewList.Count - 1].LoadUrl (fixUri (cmdStr).ToString ());
			//	//	}

			//	//	//
			//	//	GF.devLog ("[Close][Cookie] " + this.mainActivity.cookieManager.GetCookie (GF.URL_Now));

			//	//	break;
			//	#endregion


			//}
		}

		//
		public System.Uri fixUri (string cmdStr)
		{
			System.Uri theUri = new Uri (new Uri (GF.PageDefault), cmdStr);
			GF.devLog ("[FixedURI] " + theUri.ToString ());
			return theUri;
		}

		//
		//async void scanQRCode (WebView view, string subCmdStr_1)
		//{
		//	var scanner = new MobileBarcodeScanner (this.mainActivity);
		//	var result = await scanner.Scan ();
		//	var resultStr = string.Empty;

		//	if (result != null) {
		//		GF.devLog ("Scanned Barcode: " + result.Text);
		//		resultStr = result.Text;
		//	}

		//	view.EvaluateJavascript (subCmdStr_1 + " = '" + resultStr + "'", null);

		//}


	}

	class KWebChromeClient : WebChromeClient
	{
		private WebView webView;

		public KWebChromeClient (WebView webView)
		{
			this.webView = webView;
		}

		public override bool OnConsoleMessage (ConsoleMessage consoleMessage)
		{
			//GF.devLog ("[WebView Console] " + consoleMessage.ToString());
			//GF.devLog ("[WebView Console] LineNumber: " + consoleMessage.LineNumber());
			//GF.devLog ("[WebView Console] Message: " + consoleMessage.Message());
			//GF.devLog ("[WebView Console] SourceId: " + consoleMessage.SourceId());

			return base.OnConsoleMessage (consoleMessage);
		}

		/*
		public override bool OnJsAlert (WebView view, string url, string message, JsResult result)
		{
			string tUrl = string.Empty, tMsg = string.Empty;
			if (url != null)
				tUrl = url;
			if (message != null)
				tMsg = message;

			MessageBox.Show (Application.Context, tUrl, tMsg);

			return base.OnJsAlert (view, url, message, result);
		}
		*/
	}

	#endregion


#endregion

}
