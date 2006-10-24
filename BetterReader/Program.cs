using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace BetterReader
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainForm());
			MainForm mainForm = new MainForm();
			SingleInstanceApplication.Run(mainForm,
			new StartupNextInstanceEventHandler(mainForm.StartupNextInstanceHandler));
        }
    }
}