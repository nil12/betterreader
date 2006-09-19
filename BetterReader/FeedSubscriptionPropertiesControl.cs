using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	public partial class FeedSubscriptionPropertiesControl : UserControl
	{
		internal void LoadFromFeedSubscription(FeedSubscription fs)
		{
			urlTB.Text = fs.FeedUrl;
			feedTitleTB.Text = fs.DisplayName;
			updateMinutesTB.Text = ((int)(fs.UpdateSeconds / 60)).ToString();
			daysToArchiveTB.Text = fs.DaysToArchive.ToString();
		}

		internal void SaveToFeedSubscription(FeedSubscription fs)
		{
			fs.FeedUrl = urlTB.Text;
			fs.DisplayName = feedTitleTB.Text;
			fs.UpdateSeconds = int.Parse(updateMinutesTB.Text) * 60;
			fs.DaysToArchive = int.Parse(daysToArchiveTB.Text);
		}



		public FeedSubscriptionPropertiesControl()
		{
			InitializeComponent();
		}

		private void updateMinutesTB_TextChanged(object sender, EventArgs e)
		{
			if (! isANumber(updateMinutesTB.Text))
			{
				MessageBox.Show("Update interval must be an integer value.", "Error");
				updateMinutesTB.Text = "15";
			}
		}

		private void daysToArchiveTB_TextChanged(object sender, EventArgs e)
		{
			if (!isANumber(daysToArchiveTB.Text))
			{
				MessageBox.Show("Days to Archive must be an integer value.", "Error");
				daysToArchiveTB.Text = "2";
			}
		}

		private bool isANumber(string val)
		{
			try
			{
				int iVal = int.Parse(val);
			}
			catch
			{
				return false;
			}

			return true;
		}
	}
}
