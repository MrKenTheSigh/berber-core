using System;

namespace BerBerCore
{
	public class Utils
	{
		public Utils ()
		{
		}
	}

	#region == KToast ==

	public class KToast
	{
		public static void showCenterToast(Android.Content.Context ctx, string text){
			showCenterToast (ctx, text, Android.Widget.ToastLength.Short);
		}

		public static void showCenterToast(Android.Content.Context ctx, string text, Android.Widget.ToastLength duration){
			Android.App.Activity act = (Android.App.Activity)ctx;
			act.RunOnUiThread (() => {
				Android.Widget.Toast toast = Android.Widget.Toast.MakeText (ctx, text, duration);
				toast.SetGravity(Android.Views.GravityFlags.Center, 0, 0);
				toast.Show();
			});
		}

	}
	#endregion

	#region == KMessagebox ==
	public class KMessageBox
	{
		private static Android.App.AlertDialog.Builder CreateDialog(Android.Content.Context ctx, string title, string message)
		{
			Android.App.AlertDialog.Builder dlg = new Android.App.AlertDialog.Builder (ctx, Android.Resource.Style.ThemeDeviceDefaultDialog);
			return dlg.SetTitle(title).SetMessage(message);
		}
		public static void Show(Android.Content.Context ctx, string title, string message)
		{
			CreateDialog(ctx, title, message).SetPositiveButton(ctx.GetString(Resource.String.ok), delegate { }).Show();
		}
		public static void Show(Android.Content.Context ctx, string title, string message, EventHandler<Android.Content.DialogClickEventArgs> okHandler)
		{
			CreateDialog(ctx, title, message).SetPositiveButton(ctx.GetString(Resource.String.ok), okHandler).Show();
		}
		public static void Confirm(Android.Content.Context ctx, string title, string message, EventHandler<Android.Content.DialogClickEventArgs> okHandler, EventHandler<Android.Content.DialogClickEventArgs> cancelHandler)
		{
			CreateDialog(ctx, title, message).SetPositiveButton(ctx.GetString(Resource.String.ok), okHandler).SetNegativeButton(ctx.GetString(Resource.String.cancel), cancelHandler).Show();
		}
		public static void Edit_Delete(Android.Content.Context ctx, string title, string message, EventHandler<Android.Content.DialogClickEventArgs> editHandler, EventHandler<Android.Content.DialogClickEventArgs> deleteHandler)
		{
			CreateDialog(ctx, title, message).SetPositiveButton(ctx.GetString(Resource.String.edit), editHandler).SetNegativeButton(ctx.GetString(Resource.String.delete), deleteHandler).Show();
		}
		public static void ShowErrorMessage(Android.Content.Context ctx, Exception ex)
		{
			Show(ctx, ctx.GetString(Resource.String.error), ex.Message);
		}
	}
	#endregion


	#region == Err Log ==
	public class LogOperations
	{
		public static void saveLogsToSDCard(Android.Content.Context context, string msg)
		{
			string txt = "\n\n" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + " " + msg;
			string logPath = Android.OS.Environment.ExternalStorageDirectory.ToString() + "/TD888";
			//string productVersion = context.PackageManager.GetPackageInfo("com.kingkey.TD888", Android.Content.PM.PackageInfoFlags.Activities).VersionName;

			try
			{   
				if (!System.IO.File.Exists(logPath))
				{
					System.IO.Directory.CreateDirectory(logPath);
				}

				System.IO.File.AppendAllText(logPath + "/LOG.txt", txt);

			}
			catch //(Exception ex)
			{
				Console.WriteLine ("[TD888] == saveLogsToSDCard Failed ==");
				//saveLogsToSDCard(context, "\n" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\nver: " + productVersion + "\n" + ex.Message + "\n" + ex.StackTrace);
			}
		}

		public static void errReport(Android.Content.Context context, string productID, string productKey, string errorMessage, string source, string stackTrace, string consoleOutput)
		{   
			try
			{
				if (stackTrace == null) stackTrace = string.Empty;

				//WS_TD888.logError(errorMessage + "\n\n" +  stackTrace);
			}
			catch //(Exception ex)
			{
				//saveLogsToSDCard(context, "\n" + ex.Message + "\n" + ex.StackTrace);
				Console.WriteLine ("[TD888] == errReport Failed ==");
			}
		}
	}
	#endregion

}

