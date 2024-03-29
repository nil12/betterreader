using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Text;
using System.Globalization;
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
		private CultureInfo info = new System.Globalization.CultureInfo("en-US", true);

		public bool FeedTitleTextBoxEnabled
		{
			get
			{
				return feedTitleTB.Enabled;
			}
			set
			{
				feedTitleTB.Enabled = value;
			}
		}

		internal void LoadFromFeedSubscription(FeedSubscription fs)
		{
			urlTB.Text = fs.FeedUrl;
			feedTitleTB.Text = fs.DisplayName;
			updateMinutesTB.Text = (fs.UpdateSeconds / 60).ToString();
			daysToArchiveTB.Text = fs.DaysToArchive.ToString();
			maxItemsTB.Text = fs.MaxItems.ToString();
			switch (fs.FeedItemClickAction)
			{
				case FeedItemClickAction.Default:
					defaultRB.Checked = true;
					break;
				case FeedItemClickAction.LoadDescriptionInternalBrowser:
					loadDescRB.Checked = true;
					break;
				case FeedItemClickAction.LoadLinkExternalBrowser:
					loadLinkExtRB.Checked = true;
					break;
				case FeedItemClickAction.LoadLinkInternalBrowser:
					loadLinkIntRB.Checked = true;
					break;
			}
		}

		internal void SaveToFeedSubscription(FeedSubscription fs)
		{
			fs.FeedUrl = urlTB.Text;
			fs.DisplayName = feedTitleTB.Text;
			fs.UpdateSeconds = int.Parse(updateMinutesTB.Text) * 60;
			fs.DaysToArchive = int.Parse(daysToArchiveTB.Text);
			fs.MaxItems = int.Parse(maxItemsTB.Text);

			if (defaultRB.Checked)
			{
				fs.FeedItemClickAction = FeedItemClickAction.Default;
			}
			else if (loadDescRB.Checked)
			{
				fs.FeedItemClickAction = FeedItemClickAction.LoadDescriptionInternalBrowser;
			}
			else if (loadLinkExtRB.Checked)
			{
				fs.FeedItemClickAction = FeedItemClickAction.LoadLinkExternalBrowser;
			}
			else if (loadLinkIntRB.Checked)
			{
				fs.FeedItemClickAction = FeedItemClickAction.LoadLinkInternalBrowser;
			}
			else
			{
				//this is a case that really shouldn't occur so set to default
				fs.FeedItemClickAction = FeedItemClickAction.LoadDescriptionInternalBrowser;
			}
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
			if (val == null)
			{
				return false;
			}
			
			if (val.Length > 0)
			{
				double dummyOut;

				return Double.TryParse(val, System.Globalization.NumberStyles.Any,
					info.NumberFormat, out dummyOut);
			}
			else
			{
				return false;
			}
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
