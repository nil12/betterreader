using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;

namespace BetterReader
{
	public class SingleInstanceApplication : WindowsFormsApplicationBase
	{
		private SingleInstanceApplication()
		{
			base.IsSingleInstance = true;
		}

		public static void Run(Form f,
			StartupNextInstanceEventHandler startupHandler)
		{
			SingleInstanceApplication app = new SingleInstanceApplication();
			app.MainForm = f;
			app.StartupNextInstance += startupHandler;
			app.Run(Environment.GetCommandLineArgs());
		}
	}
}
