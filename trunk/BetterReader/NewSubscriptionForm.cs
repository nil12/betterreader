using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	public partial class NewSubscriptionForm : Form
	{
		private FeedFolder createInFolder = null;
		private FeedSubscriptionTree fst;
		private FeedSubscription fs;

		public FeedFolder CreateInFolder
		{
			get { return createInFolder; }
			set { createInFolder = value; }
		}

		public FeedSubscription FeedSubscription
		{
			get
			{
				return fs;
			}
		}

		public NewSubscriptionForm()
		{
			InitializeComponent();
		}


		public NewSubscriptionForm(FeedSubscriptionTree lFst, FeedFolder lDefaultFolder) : this()
		{
			createInFolder = lDefaultFolder;
			fst = lFst;
			fs = new FeedSubscription();
			fs.UpdateSeconds = 15 * 60;
			feedSubscriptionPropertiesControl1.LoadFromFeedSubscription(fs);
		}

		private void NewSubscriptionForm_Load(object sender, EventArgs e)
		{
			if (fst != null)
			{
				SetFeedSubscriptionTree(fst);
				feedFoldersTV.Invalidate();
			}

		}

		public void SetFeedSubscriptionTree(FeedSubscriptionTree fst)
		{
			bindFolderNodesToTreeView(fst.RootLevelNodes, feedFoldersTV.Nodes);
		}

		private void bindFolderNodesToTreeView(List<FeedSubTreeNodeBase> list, TreeNodeCollection treeNodeCollection)
		{
			foreach (FeedSubTreeNodeBase fstnb in list)
			{
				if (fstnb.GetType() == typeof(FeedFolder))
				{
					FeedFolder ff = (FeedFolder)fstnb;
					TreeNode newNode = treeNodeCollection.Add(ff.Name);
					newNode.Tag = ff;
					if (ff == createInFolder)
					{
						newNode.EnsureVisible();

						feedFoldersTV.SelectedNode = newNode;
					}
					bindFolderNodesToTreeView(ff.ChildNodes, newNode.Nodes);
				}
			}
		}

		private void cancelBTN_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void okBTN_Click(object sender, EventArgs e)
		{
			FeedSubscriptionPropertiesFormValidity v = feedSubscriptionPropertiesControl1.ValidateFeedSubscription();
			if (v.IsValid != true)
			{
				MessageBox.Show("The following problems were found with your feed subscription:\r\n" + v.ErrMsg);
				return;
			}
			feedSubscriptionPropertiesControl1.SaveToFeedSubscription(fs);
			fs.ParentFolder = createInFolder;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void feedFoldersTV_AfterSelect(object sender, TreeViewEventArgs e)
		{
			createInFolder = (FeedFolder)e.Node.Tag;
		}

	}
}