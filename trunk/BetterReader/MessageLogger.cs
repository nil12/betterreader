//using System;
//using System.Collections.Generic;
//using System.Text;
using System.Diagnostics;

namespace BetterReader
{
	abstract class MessageLogger
	{
		private static EventLog eventLog;

		internal static void WriteToEventLog(string message)
		{
			// Create the source, if it does not already exist.
			if (!EventLog.SourceExists("BetterReader"))
			{
				EventLog.CreateEventSource("BetterReader", "Messages");
			}

			if (eventLog == null)
			{
				// Create an EventLog instance and assign its source.
				eventLog = new EventLog();
				eventLog.Source = "BetterReader";
			}

			eventLog.WriteEntry(message);

		}
	}
}
