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

using System.Threading;


namespace BerBerCore
{
	[Activity(
		Theme = "@style/Theme.Splash", 
		//MainLauncher = true, 
		NoHistory = true,
		//ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait
		ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize
	)]		
	public class SplashActitvity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			//目前沒有landscape的圖示, 暫不做此調整
			/*
			KUserDefault.Rotate = Ini.Rotate;
			if(KUserDefault.Rotate == 90){
				//portrait
				RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
			}else{
				//landscape
				RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
			}
			*/

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

