using System;

namespace BerBerCore
{
	public class JsonClass
	{
		public JsonClass ()
		{
		}
	}

	// TD888:Cmd:[Base64|Raw]:
	// { TD888:["cmd1","cmd2","cmd3".....] }
	public class JMultiCMD
	{
		public System.Collections.Generic.List<string> TD888 = new System.Collections.Generic.List<string>();

	}


	// TD888:ConfirmMsg: 
	// {button: {OK: "func", CANCEL: "func"}, message: "<Message>"}
	public class JConfirmMsg
	{
		public ConfirmButton button;
		public string message;

		public JConfirmMsg ()
		{
			button = new ConfirmButton ();
		}

		public class ConfirmButton
		{
			public string OK;
			public string CANCEL;

			public ConfirmButton ()
			{
			}

		}

	}

	// TD888:AlertMsg: 
	// {button: {OK: "func"}, message: "<Message>"}
	public class JAlertMsg
	{
		public JAlertButton button;
		public string message;

		public JAlertMsg ()
		{
			button = new JAlertButton ();
		}

		public class JAlertButton
		{
			public string OK;

			public JAlertButton ()
			{
			}

		}

	}


}

