using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

using System.Threading;


namespace BerBerCore
{
	[Activity(
		Theme = "@android:style/Theme.NoTitleBar.Fullscreen",
		NoHistory = true,
		ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize
	)]		
	public class SplashActitvity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			var lp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			var panel = new RelativeLayout (this);
			SetContentView (panel, lp);

			var webview = new WebView (this);
			webview.SetWebViewClient (new WebViewClient ());
			//webview.SetWebChromeClient (new O08WebChromeClient (webview));//for debug
			webview.Settings.JavaScriptEnabled = true;

			lp = new RelativeLayout.LayoutParams (ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
			panel.AddView (webview, lp);

			webview.LoadUrl (Ini.URL_SPLASH);

		}

		public override void OnWindowFocusChanged (bool hasFocus)
		{
			base.OnWindowFocusChanged (hasFocus);

			new Thread (new ThreadStart (delegate {
				Thread.Sleep (Ini.LaunchScreenTime_MS);
				this.Finish ();
			})).Start ();

		}

	}
}

