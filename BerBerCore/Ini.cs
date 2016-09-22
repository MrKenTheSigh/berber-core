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
		public const string URL_ERR = "file:///android_asset/Web/error.html";
		public const string URL_SPLASH = "file:///android_asset/Web/splash.html";
		public const string URL_BLANK = "about:blank";

		public const string CMD_PRFIX = "berber:";

		//
		public const bool ShowLaunchScreen = true;
		public const int LaunchScreenTime_MS = 3000;

		//
		public Ini ()
		{
		}


	}

}

