using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	public partial class SubscriptionPropertiesForm : Form
	{
		private FeedSubscription fs;

		public FeedSubscription FeedSubscription
		{
			get { return fs; }
			set { fs = value; }
		}


		public SubscriptionPropertiesForm(FeedSubscription lFs)
		{
			InitializeComponent();
			fs = lFs;
			feedSubscriptionPropertiesControl1.LoadFromFeedSubscription(fs);
		}


		private void okBTN_Click(object sender, EventArgs e)
		{
			feedSubscriptionPropertiesControl1.SaveToFeedSubscription(fs);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void cancelBTN_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}