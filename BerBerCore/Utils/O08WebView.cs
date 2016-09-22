using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace BerBerCore
{

	public class O08WebView : WebView
	{
		private MainActivity mainActivity;

		private bool touchEnabled = true;

		public O08WebView (Context context)
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

		public override void EvaluateJavascript (string script, IValueCallback resultCallback)
		{
			if ((int)Android.OS.Build.VERSION.SdkInt >= 19) {
				base.EvaluateJavascript (script, resultCallback);
			} else {
				this.LoadUrl ("javascript:" + script);
			}
		}


		//protected override void Dispose (bool disposing)
		//{
		//	base.Dispose (disposing);
		//}


		public void setTouchEnabled (bool enabled)
		{
			this.touchEnabled = enabled;
		}
		public void doBackToSurface ()
		{
			setTouchEnabled (true);
		}


	}

	public class O08WebViewClient : WebViewClient
	{
		private MainActivity mainActivity;
		private WebView webview;
		private int retryCnt = 1;

		private Thread timeoutChecking = null;
		private bool keepTimeoutChecking = true;
		private const int CD = (int)(Ini.WebViewTimeoutMS / 500);
		private int cd_500ms = CD;

		public O08WebViewClient (MainActivity mainActivity)
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

								KToast.showCenterToast (mainActivity, "伺服器無法服務 [" + this.retryCnt + "]", ToastLength.Long);

								if (this.retryCnt >= Ini.WebViewTimeoutRetry) {
									mainActivity.RunOnUiThread (delegate {
										this.webview.LoadUrl (Ini.URL_ERR);
									});

								} else {
									this.retryCnt++;
									mainActivity.RunOnUiThread (delegate {
										this.webview.Reload ();
									});

								}
								break;

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

			stopTimeoutChecking ();

			if (url == Ini.URL_ERR) {
				this.webview.LoadUrl (Ini.URL_MAIN);
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

		public override void OnReceivedError (WebView view, IWebResourceRequest request, WebResourceError error)
		{
			GF.devLog ("[WebView][OnReceivedError]: " + error);	

			stopTimeoutChecking ();

			this.webview.LoadUrl (Ini.URL_ERR);

			//
			base.OnReceivedError (view, request, error);
		}

		#endregion

		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			GF.devLog ("[ShouldOverrideUrlLoading] " + url);

			string _url = System.Web.HttpUtility.UrlDecode (url).Replace (" ", string.Empty);

			if (_url.StartsWith (Ini.CMD_PRFIX, StringComparison.CurrentCulture)) {
				processCMD (_url);
				return true;
			}

			return base.ShouldOverrideUrlLoading (view, url);
		}
		private void processCMD (string url)
		{
			string jsonStr = url.Remove (0, Ini.CMD_PRFIX.Length);
			GF.devLog ("[jsonStr] " + jsonStr);

			JObject restoredObject = JsonConvert.DeserializeObject<JObject> (jsonStr);

			GF.devLog ("[method] " + restoredObject ["method"]);
			switch (restoredObject ["method"].ToString()) {
				case "yo":
					GF.devLog ("[data] N/A");
					break;

				case "yo2":
					GF.devLog ("[data] " + restoredObject ["data"]);
					GF.devLog ("[data.name] " + restoredObject ["data"] ["name"]);
					GF.devLog ("[data.phone] " + restoredObject ["data"] ["phone"]);
					break;

				case "startService":
					mainActivity.StartService (new Intent (mainActivity, typeof (NotificationService)));
					break;

				case "stopService":
					mainActivity.StopService (new Intent (mainActivity, typeof (NotificationService)));
					break;

				case "notification":
					var title = restoredObject ["data"] ["title"].ToString();
					var content = restoredObject ["data"] ["content"].ToString ();
					NotificationToolbox.updateNotification (mainActivity, title, content, "this is extra");
					break;

				case "vibrate":
					var jArr = restoredObject ["data"] as JArray;
					long [] pattern = jArr.Select (jv => jv.Value<long> ()).ToArray ();
					mainActivity.doVibrate (pattern);
					break;

			}


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


		//public void updateNotification (string title, string content, string extra) {
		//	//NOTE ref https://developer.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/local_notifications_in_android/

		//	Intent intent = new Intent (mainActivity, typeof (MainActivity));
		//	intent.PutExtra ("msg_from_noti", extra);

		//	TaskStackBuilder stackBuilder = TaskStackBuilder.Create (mainActivity);
		//	stackBuilder.AddParentStack (Java.Lang.Class.FromType (typeof (MainActivity)));
		//	stackBuilder.AddNextIntent (intent);

		//	const int pendingIntentId = 0;
		//	PendingIntent pendingIntent =
		//		stackBuilder.GetPendingIntent (pendingIntentId, PendingIntentFlags.OneShot);

		//	Notification.Builder builder = new Notification.Builder (mainActivity)
		//		.SetContentIntent (pendingIntent)
		//		.SetContentTitle (title)
		//		.SetContentText (content)
		//		.SetSmallIcon (Resource.Drawable.Icon);
			
		//	Notification notification = builder.Build ();

		//	NotificationManager notificationManager =
		//		mainActivity.GetSystemService (Context.NotificationService) as NotificationManager;

		//	const int notificationId = 0;
		//	notificationManager.Notify (notificationId, notification);

		//}

	}

	class O08WebChromeClient : WebChromeClient
	{
		private WebView webView;

		public O08WebChromeClient (WebView webView)
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



}
