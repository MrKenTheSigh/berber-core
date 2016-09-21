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

namespace BerBerCore
{
	[BroadcastReceiver]
	[IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted },
		Categories = new[] { Android.Content.Intent.CategoryDefault }
	)]
	public class BootUpReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Intent intent)
		{
			//Toast.MakeText (context, "Received intent!", ToastLength.Short).Show ();

			GF.devLog ("[BootUpReceiver][OnReceive]");

			if ((intent.Action != null) && 
				(intent.Action == Android.Content.Intent.ActionBootCompleted))   
			{             
				
				context.ApplicationContext.StartService(new Intent(context, typeof(NotificationService)));

			} 

		}
	}
}

