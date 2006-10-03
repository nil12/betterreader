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
	public struct FeedSubscriptionPropertiesFormValidity
	{
		public bool IsValid;
		public string ErrMsg;
	}

	public partial class FeedSubscriptionPropertiesControl : UserControl
	{
		internal void LoadFromFeedSubscription(FeedSubscription fs)
		{
			urlTB.Text = fs.FeedUrl;
			feedTitleTB.Text = fs.DisplayName;
			updateMinutesTB.Text = ((int)(fs.UpdateSeconds / 60)).ToString();
			daysToArchiveTB.Text = fs.DaysToArchive.ToString();
			maxItemsTB.Text = fs.MaxItems.ToString();
		}

		internal void SaveToFeedSubscription(FeedSubscription fs)
		{
			fs.FeedUrl = urlTB.Text;
			fs.DisplayName = feedTitleTB.Text;
			fs.UpdateSeconds = int.Parse(updateMinutesTB.Text) * 60;
			fs.DaysToArchive = int.Parse(daysToArchiveTB.Text);
			fs.MaxItems = int.Parse(maxItemsTB.Text);
		}

		internal FeedSubscriptionPropertiesFormValidity ValidateFeedSubscription()
		{
			//I know this isn't the best way to do this but I need a quick fix right now
			FeedSubscriptionPropertiesFormValidity v = new FeedSubscriptionPropertiesFormValidity();
			v.IsValid = true;
			if (urlTB.Text == string.Empty)
			{
				v.ErrMsg += "Feed URL is required.\r\n";
				v.IsValid = false;
			}
			if (feedTitleTB.Text == string.Empty)
			{
				v.ErrMsg += "Feed Title is required.\r\n";
				v.IsValid = false;
			}

			return v;
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

		private void maxItemsTB_TextChanged(object sender, EventArgs e)
		{
			if (!isANumber(maxItemsTB.Text))
			{
				MessageBox.Show("Max Items in Feed must be an integer value.", "Error");
				maxItemsTB.Text = "275";
			}
		}
	}
}
