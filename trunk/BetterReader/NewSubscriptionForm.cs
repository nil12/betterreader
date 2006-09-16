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

		public FeedFolder CreateInFolder
		{
			get { return createInFolder; }
			set { createInFolder = value; }
		}

		public string FeedUrl
		{
			get
			{
				return feedSubscriptionPropertiesControl1.FeedUrl;
			}
			set
			{
				feedSubscriptionPropertiesControl1.FeedUrl = value;
			}
		}

		public string FeedTitle
		{
			get
			{
				return feedSubscriptionPropertiesControl1.FeedTitle;
			}
			set
			{
				feedSubscriptionPropertiesControl1.FeedTitle = value;
			}
		}

		public int UpdateSeconds
		{
			get
			{
				return feedSubscriptionPropertiesControl1.UpdateSeconds;
			}
			set
			{
				feedSubscriptionPropertiesControl1.UpdateSeconds = value;
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
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void feedFoldersTV_AfterSelect(object sender, TreeViewEventArgs e)
		{
			createInFolder = (FeedFolder)e.Node.Tag;
		}

	}
}