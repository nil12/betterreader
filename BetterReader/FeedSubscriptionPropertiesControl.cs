using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BetterReader
{
	public partial class FeedSubscriptionPropertiesControl : UserControl
	{
		public string FeedTitle
		{
			get
			{
				return feedTitleTB.Text;
			}
			set
			{
				feedTitleTB.Text = value;
			}
		}

		public string FeedUrl
		{
			get
			{
				return urlTB.Text;
			}
			set
			{
				urlTB.Text = value;
			}
		}

		public int UpdateSeconds
		{
			get
			{
				int minutes = int.Parse(updateMinutesTB.Text);
				return minutes * 60;
			}
			set
			{
				int minutes = value / 60;
				updateMinutesTB.Text = minutes.ToString();
			}
		}


		public FeedSubscriptionPropertiesControl()
		{
			InitializeComponent();
		}

		private void updateMinutesTB_TextChanged(object sender, EventArgs e)
		{
			try
			{
				int val = int.Parse(updateMinutesTB.Text);
			}
			catch (FormatException)
			{
				MessageBox.Show("Update interval must be an integer value.", "Error");
				updateMinutesTB.Text = "15";
			}
		}
	}
}
