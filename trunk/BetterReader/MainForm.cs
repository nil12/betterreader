using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BetterReader.Backend;

namespace BetterReader
{
    public partial class MainForm : Form
    {
		
		private delegate void setNodeTextDelegate(TreeNode node, string text);
		private delegate void displayFeedItemsIfSelectedDelegate(TreeNode node, FeedSubscription fs);
        private FeedSubscriptionTree fst;
        private Dictionary<object, TreeNode> treeNodesByTag;
		private Dictionary<FeedItem, ListViewItem> listViewItemsByTag;
		private readonly string settingsDirectory = System.Environment.CurrentDirectory + "\\appSettings\\";
		private readonly string feedSubsFilepath;
		private static string archiveDirectory;
		private readonly string graphicsDirectory = System.Environment.CurrentDirectory + "\\Graphics\\";
		private Font feedsNormalFont;
		private Font feedsBoldFont;
		private Font feedItemsNormalFont;
		private Font feedItemsBoldFont;
		private Graphics formGraphics;
		private Icon redLightIcon, yellowLightIcon, greenLightIcon;
		private TreeNode rightClickedNode;
		private FeedSubscription currentlyDisplayedFeedSubscription;

		internal static string ArchiveDirectory
		{
			get
			{
				return archiveDirectory;
			}
		}

        public MainForm()
        {
			InitializeComponent();
			feedSubsFilepath = settingsDirectory + "FeedSubscriptions.xml";
			archiveDirectory = settingsDirectory + "ArchivedItems\\";
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			formGraphics = this.CreateGraphics();
			redLightIcon = new Icon(graphicsDirectory + "redlight.ico");
			yellowLightIcon = new Icon(graphicsDirectory + "yellowlight.ico");
			greenLightIcon = new Icon(graphicsDirectory + "greenlight.ico");
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			throw e.ExceptionObject as Exception;
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			throw e.Exception;
		}

        private void Form1_Load(object sender, EventArgs e)
		{
			ensureDirectoryExists(settingsDirectory);
			ensureDirectoryExists(archiveDirectory);
			feedsNormalFont = feedsTV.Font;
			feedsBoldFont = new Font(feedsNormalFont, FontStyle.Bold);
			feedItemsNormalFont = feedItemsLV.Font;
			feedItemsBoldFont = new Font(feedItemsNormalFont, FontStyle.Bold);

			if (File.Exists(feedSubsFilepath))
			{
				fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile(feedSubsFilepath);
				bindFSTToTreeView();
				feedReaderBGW.RunWorkerAsync();
			}
        }

		private void beginReads()
		{
			fst.BeginReadAllFeeds(new FeedSubscriptionReadDelegate(feedSubReadCallback));
		}

        private void feedSubReadCallback(FeedSubscription fs)
        {
			TreeNode node = treeNodesByTag[fs];
			setNodePropertiesFromFeedSubscription(fs, node);
        }

		private void setNodePropertiesFromFeedSubscription(FeedSubscription fs, TreeNode node)
		{
			string text;

			if (this.IsDisposed)
			{
				return;
			}

			this.Invoke(new MethodInvoker(feedsTV.BeginUpdate));
			if (fs.Feed.ReadSuccess)
			{
				text = fs.ToString();
				if (fs.Feed.UnreadItems > 0)
				{
					node.NodeFont = feedsBoldFont;
				}
				else
				{
					node.NodeFont = feedsNormalFont;
				}
			}
			else
			{
				if (fs.Feed.ReadException != null)
				{
					text = fs.DisplayName + "(" + fs.Feed.ReadException.ToString() + ")";
				}
				else
				{
					text = "Loading . . .";
				}
			}
			try
			{
				this.Invoke(new setNodeTextDelegate(setNodeText), new object[] { node, text });
				this.Invoke(new displayFeedItemsIfSelectedDelegate(displayFeedItemsIfNodeSelected),
	new object[] { node, fs });

				if (fs.Feed.HasNewItemsFromLastRead && notifyIcon1.Visible)
				{
					//the app is minimized and new items were found so set the notifyIcon to alert status
					notifyIcon1.Icon = redLightIcon;
				}
			}
			catch (InvalidOperationException)
			{
				//this was most likely caused by a feed reading thread returning during shutdown
				//so we'll ignore it
			}

			this.Invoke(new MethodInvoker(feedsTV.EndUpdate));
		}

		private void displayFeedItemsIfNodeSelected(TreeNode node, FeedSubscription fs)
		{
			if (node.IsSelected)
			{
				displayFeedItems(fs);
			}
		}

		private void setNodeText(TreeNode node, string text)
		{
			node.Text = text;
		}

		private void ensureDirectoryExists(string path)
		{
			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);
			}
		}

        private void importOpmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showImportOpmlDialog();
        }

        private void showImportOpmlDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                importOpml(ofd.FileName);
            }
        }

        private void importOpml(string filepath)
        {
            fst = Opml.GetFeedSubscriptionTreeFromOpmlFile(filepath);
			fst.SaveAsFeedSubscriptionsFile(settingsDirectory + "FeedSubscriptions.xml");
			bindFSTToTreeView();
			feedReaderBGW.RunWorkerAsync();
        }

        private void bindFSTToTreeView()
        {
			//feedsTV.SuspendLayout();
			feedsTV.BeginUpdate();
			feedsTV.Nodes.Clear();
            treeNodesByTag = new Dictionary<object, TreeNode>();
			//bindFolderToTreeView(fsc.RootFolder, null);
			bindNodeListToTreeView(fst.RootLevelNodes, null);
			feedsTV.EndUpdate();
			//feedsTV.ResumeLayout();
        }

		private void bindNodeListToTreeView(List<FeedSubTreeNodeBase> nodeList, TreeNode treeNode)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				Type nodeType = fstnb.GetType();
				if (nodeType == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					TreeNode newNode;
					if (treeNode == null)
					{
						//we're at the root level of the tree
						newNode = feedsTV.Nodes.Add(fs.DisplayName);
					}
					else
					{
						newNode = treeNode.Nodes.Add(fs.DisplayName);
					}

					newNode.Tag = fs;
					treeNodesByTag.Add(fs, newNode);
				}
				else if (nodeType == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					TreeNode newNode;
					if (treeNode == null)
					{
						//we're at the root level of the tree
						newNode = feedsTV.Nodes.Add(ff.Name);
					}
					else
					{
						newNode = treeNode.Nodes.Add(ff.Name);
					}

					newNode.Tag = ff;
					
					treeNodesByTag.Add(ff, newNode);
					if (ff.IsExpandedInUI)
					{
						newNode.Expand();
						bool expanded = newNode.IsExpanded;
					}
					bindNodeListToTreeView(ff.ChildNodes, newNode);
				}
				else
				{
					throw new Exception("Form1.bindNodeListToTreeView error: Unrecognized node type: " + nodeType.ToString());
				}
			}
		}


		private void displayFeedItems(FeedSubscription feedSubscription)
		{
			currentlyDisplayedFeedSubscription = feedSubscription;
			feedSubscription.ResetUpdateTimer();
			feedItemsLV.ListViewItemSorter = currentlyDisplayedFeedSubscription.ColumnSorter;
			smartSortCB.Visible = true;
			smartSortCB.Checked = currentlyDisplayedFeedSubscription.ColumnSorter.SmartSortEnabled;

			listViewItemsByTag = new Dictionary<FeedItem, ListViewItem>();
			feedItemsLV.SuspendLayout();
			feedItemsLV.Clear();
			webBrowser1.DocumentText = "";
			feedTitleLBL.Text = feedSubscription.DisplayName;
			feedTitleLBL.Width = splitContainer2.Panel1.Width;
			if (feedSubscription.Feed.ReadSuccess)
			{
				bindFeedItemsToListView(feedSubscription.Feed.FeedItems);
				feedItemsLV.Enabled = true;
			}
			else
			{
				if (feedSubscription.Feed.ReadException == null)
				{
					feedItemsLV.Columns.Add("Title");
					ListViewItem lvi = new ListViewItem("Loading");
					feedItemsLV.Items.Add(lvi);
					feedItemsLV.Enabled = false;
				}
				else
				{
					feedItemsLV.Columns.Add("Error");
					ListViewItem lvi = new ListViewItem(feedSubscription.Feed.ReadException.ToString());
					feedItemsLV.Items.Add(lvi);
					feedItemsLV.Enabled = false;
				}
			}
			feedItemsLV.Sort();
			feedItemsLV.ResumeLayout();
		}

		private void bindFeedItemsToListView(List<FeedItem> feedItems)
		{
			addFeedItemColumnsToListView(currentlyDisplayedFeedSubscription.Feed.IncludedFeedItemProperties);
			if (feedItems.Count < 1)
			{
				feedItemsLV.Items.Add(new ListViewItem("No items found."));
				feedItemsLV.Enabled = false;
				return;
			}

			feedItemsLV.Enabled = true;
			foreach (FeedItem fi in feedItems)
			{
				ListViewItem lvi = new ListViewItem(fi.Title);
				lvi.Tag = fi;
				setListViewItemSubItems(lvi, fi, currentlyDisplayedFeedSubscription.Feed.IncludedFeedItemProperties);
				if (fi.HasBeenRead)
				{
					lvi.Font = feedItemsNormalFont;
				}
				else
				{
					lvi.Font = feedItemsBoldFont;
				}

				feedItemsLV.Items.Add(lvi);
				listViewItemsByTag.Add(fi, lvi);
			}

			setFeedItemColumnWidths();
		}

		private void setFeedItemColumnWidths()
		{
			foreach (ColumnHeader ch in feedItemsLV.Columns)
			{
				ch.Width = -1;
			}
		}


		private void setListViewItemSubItems(ListViewItem lvi, FeedItem fi, FeedItemProperties itemProps)
		{
			if ((itemProps & FeedItemProperties.PubDate) == FeedItemProperties.PubDate)
			{
				lvi.SubItems.Add(fi.PubDate.ToString());
			}

			if ((itemProps & FeedItemProperties.HasBeenRead) == FeedItemProperties.HasBeenRead)
			{
				lvi.SubItems.Add(fi.HasBeenRead.ToString());
			}

			if ((itemProps & FeedItemProperties.DownloadDate) == FeedItemProperties.DownloadDate)
			{
				lvi.SubItems.Add(fi.DownloadDate.ToString());
			}

			if ((itemProps & FeedItemProperties.Category) == FeedItemProperties.Category)
			{
				lvi.SubItems.Add(fi.Category.ToString());
			}

			if ((itemProps & FeedItemProperties.Author) == FeedItemProperties.Author)
			{
				lvi.SubItems.Add(fi.Author.ToString());
			}
		}


		private void addFeedItemColumnsToListView(FeedItemProperties itemProps)
		{
			feedItemsLV.Columns.Add("Title");

			if ((itemProps & FeedItemProperties.PubDate) == FeedItemProperties.PubDate)
			{
				feedItemsLV.Columns.Add("PubDate");
			}

			if ((itemProps & FeedItemProperties.HasBeenRead) == FeedItemProperties.HasBeenRead)
			{
				feedItemsLV.Columns.Add("Read");
			}

			if ((itemProps & FeedItemProperties.DownloadDate) == FeedItemProperties.DownloadDate)
			{
				feedItemsLV.Columns.Add("DownloadDate");
			}

			if ((itemProps & FeedItemProperties.Category) == FeedItemProperties.Category)
			{
				feedItemsLV.Columns.Add("Category");
			}

			if ((itemProps & FeedItemProperties.Author) == FeedItemProperties.Author)
			{
				feedItemsLV.Columns.Add("Author");
			}

		}



		private void displaySelectedFeedItem()
		{
			if (feedItemsLV.SelectedItems.Count > 0)
			{
				FeedItem fi = feedItemsLV.SelectedItems[0].Tag as FeedItem;
				itemTitleLBL.Text = fi.Title;
				itemLinkLBL.Text = fi.LinkUrl;
				itemLinkLBL.Links[0].LinkData = fi.LinkUrl;
				if (fi != null)
				{
					if (fi.EncodedContent != null && fi.EncodedContent.Length > 0)
					{
						webBrowser1.DocumentText = formatDescriptionHTML(fi.EncodedContent);
					}
					else if (fi.Description != null && fi.Description.Length > 0)
					{
						webBrowser1.DocumentText = formatDescriptionHTML(fi.Description);
					}
					else
					{
						webBrowser1.Navigate(fi.LinkUrl);
					}
				}
				fi.HasBeenRead = true;
				fi.ParentFeed.UnreadItems--;
				feedItemsLV.SelectedItems[0].Font = feedItemsNormalFont;
				TreeNode node = treeNodesByTag[fi.ParentFeed.ParentSubscription] as TreeNode;
				setNodeText(node, fi.ParentFeed.ParentSubscription.ToString());
			}
		}


		private void visitLink(LinkLabel.Link link)
		{
			link.Visited = true;
			System.Diagnostics.Process.Start(link.LinkData.ToString());
		}

		private string formatDescriptionHTML(string description)
		{
			return "<html><body style='margin:8px; overflow:auto; font-family:Tahoma; font-size:9pt;'>" +
				description + "</body></html>";
		}


		private void hideFormShowNotifyIcon()
		{
			if (fst.GetUnreadItemCount() > 0)
			{
				notifyIcon1.Icon = yellowLightIcon;
			}
			else
			{
				notifyIcon1.Icon = greenLightIcon;
			}

			this.Hide();
			notifyIcon1.Visible = true;
		}

		private void showFormHideNotifyIcon()
		{
			notifyIcon1.Visible = false;
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}


		private void moveTreeNode(TreeNode movedNode, TreeNode destinationNode)
		{
			fst.MoveNode((FeedSubTreeNodeBase)movedNode.Tag, (FeedSubTreeNodeBase)destinationNode.Tag);
			bindFSTToTreeView();
		}


		private void doDragComplete()
		{
			fst.ReloadFromTreeView(feedsTV);
			saveFeedSubTree();
		}

		private void saveFeedSubTree()
		{
			fst.SaveAsFeedSubscriptionsFile(feedSubsFilepath);
		}


		private bool showUnsubscribeConfirmation()
		{
			if (MessageBox.Show("Unsubscribe from this feed?", "Unsubscribe?") == DialogResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}



		private void addNewFolder(TreeNode parentNode)
		{
			FeedFolder parentFolder = (FeedFolder)parentNode.Tag;
			FeedFolder newFolder = new FeedFolder();
			newFolder.Name = "New Folder";
			newFolder.ParentFolder = parentFolder;
			parentFolder.ChildNodes.Add(newFolder);

			TreeNode newNode = parentNode.Nodes.Add(newFolder.Name);
			newNode.Tag = newFolder;
			saveFeedSubTree();
		}


		private void deleteFolder(TreeNode rightClickedNode)
		{
			FeedFolder ff = (FeedFolder)rightClickedNode.Tag;
			ff.ParentFolder.ChildNodes.Remove(ff);

			rightClickedNode.Parent.Nodes.Remove(rightClickedNode);
			saveFeedSubTree();
		}

		private bool showDeleteFolderConfirmation()
		{
			if (MessageBox.Show("Delete Folder and all children?", "Delete Folder") == DialogResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		private void showNewSubscriptionForm(TreeNode rightClickedNode)
		{

			FeedFolder parentFolder = null;
			if (rightClickedNode != null)
			{
				if (rightClickedNode.Tag.GetType() == typeof(FeedFolder))
				{
					parentFolder = (FeedFolder)rightClickedNode.Tag;
				}
				else if (rightClickedNode.Tag.GetType() == typeof(FeedSubscription))
				{
					parentFolder = ((FeedSubscription)rightClickedNode.Tag).ParentFolder;
				}
			}

			NewSubscriptionForm nsf = new NewSubscriptionForm(fst, parentFolder);

			if (nsf.ShowDialog() == DialogResult.OK)
			{
				FeedSubscription fs = new FeedSubscription();
				fs.FeedUrl = nsf.FeedUrl;
				fs.DisplayName = nsf.FeedTitle;
				fs.UpdateSeconds = nsf.UpdateSeconds;
				fs.ParentFolder = nsf.CreateInFolder;
				addFeedSubscriptionToFolder(parentFolder, fs);

				fs.BeginReadFeed(feedSubReadCallback);
			}
		}

		private void addFeedSubscriptionToFolder(FeedFolder parentFolder, FeedSubscription fs)
		{
			parentFolder.ChildNodes.Add(fs);
			TreeNode parentNode = treeNodesByTag[parentFolder];
			TreeNode newNode = parentNode.Nodes.Add(fs.ToString());
			newNode.Tag = fs;
			treeNodesByTag.Add(fs, newNode);
			saveFeedSubTree();
		}


		private void markFeedRead(FeedSubscription fs)
		{
			fs.MarkAllItemsRead();
			TreeNode node = treeNodesByTag[fs];
			setNodePropertiesFromFeedSubscription(fs, node);
			if (currentlyDisplayedFeedSubscription == fs)
			{
				displayFeedItems(fs);
			}
		}


		private void markFolderRead(FeedFolder ff)
		{
			foreach (FeedSubTreeNodeBase fstnb in ff.ChildNodes)
			{
				Type t = fstnb.GetType();
				if (t == typeof(FeedSubscription))
				{
					markFeedRead((FeedSubscription)fstnb);
				}
				else if (t == typeof(FeedFolder))
				{
					markFolderRead((FeedFolder)fstnb);
				}
				else
				{
					throw new Exception("Error.  Unhandled FeedSubTreeNodeBase type in markFolderRead");
				}
			}
		}


		private void handleHotKey(char pressedKey)
		{
			switch (pressedKey)
			{
				case 'r':
				case 'R':
					markFeedRead(currentlyDisplayedFeedSubscription);
					break;
			}
		}

		private void sortFeedItemsLV(int columnIndex)
		{
			FeedItemsListViewColumnSorter lvwColumnSorter = (FeedItemsListViewColumnSorter)feedItemsLV.ListViewItemSorter;

			// Determine if clicked column is already the column that is being sorted.
			if (columnIndex == lvwColumnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (lvwColumnSorter.Order == SortOrder.Ascending)
				{
					lvwColumnSorter.Order = SortOrder.Descending;
				}
				else
				{
					lvwColumnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				lvwColumnSorter.SortColumn = columnIndex;
				lvwColumnSorter.Order = SortOrder.Ascending;
			}

			// Perform the sort with these new sort options.
			feedItemsLV.Sort();
		}

		//event handlers below

		private void feedReaderBGW_DoWork(object sender, DoWorkEventArgs e)
		{
			beginReads();
		}

		private void feedsTV_AfterSelect(object sender, TreeViewEventArgs e)
		{
			object tag = e.Node.Tag;
			Type t = tag.GetType();
			if (t == typeof(FeedSubscription))
			{
				displayFeedItems(tag as FeedSubscription);
			}
		}

		private void feedItemsLV_SelectedIndexChanged(object sender, EventArgs e)
		{
			displaySelectedFeedItem();
		}

		private void itemLinkLBL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			visitLink(e.Link);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (fst != null)
			{
				fst.Dispose();
			}
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				hideFormShowNotifyIcon();
			}
		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			showFormHideNotifyIcon();
		}

		private void feedsTV_DragComplete(object sender, Sloppycode.UI.DragCompleteEventArgs e)
		{
			doDragComplete();
		}

		private void newSubscriptionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showNewSubscriptionForm(rightClickedNode);
		}


		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			feedsTV.LabelEdit = true;
			Type t = rightClickedNode.Tag.GetType();

			if (t == typeof(FeedSubscription))
			{
				rightClickedNode.Text = ((FeedSubscription)rightClickedNode.Tag).DisplayName;

			}
			rightClickedNode.BeginEdit();
		}

		private void unsubscribeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showUnsubscribeConfirmation())
			{
				rightClickedNode.Parent.Nodes.Remove(rightClickedNode);
				FeedSubTreeNodeBase fstnb = (FeedSubTreeNodeBase)rightClickedNode.Tag;
				fstnb.ParentFolder.ChildNodes.Remove(fstnb);
				saveFeedSubTree();
			}
		}


		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void feedsTV_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			Type t = e.Node.Tag.GetType();

			if (t == typeof(FeedSubscription))
			{
				FeedSubscription fs = (FeedSubscription)e.Node.Tag;
				fs.DisplayName = e.Label;
				e.Node.Text = fs.ToString();
				e.CancelEdit = true;
			}
			else
			{
				FeedFolder ff = (FeedFolder)e.Node.Tag;
				ff.Name = e.Label;
			}

			feedsTV.LabelEdit = false;
			saveFeedSubTree();
		}


		private void feedsTV_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button.CompareTo(MouseButtons.Right) == 0)
			{
				rightClickedNode = feedsTV.GetNodeAt(e.X, e.Y);
				if (rightClickedNode != null)
				{
					if (rightClickedNode.Tag.GetType() == typeof(FeedSubscription))
					{
						feedSubContextMenuStrip.Show(feedsTV, e.X, e.Y);
					}

					if (rightClickedNode.Tag.GetType() == typeof(FeedFolder))
					{
						folderContextMenuStrip.Show(feedsTV, e.X, e.Y);
					}
				}
			}
		}


		private void newFolderToolStripMenuItem_Click(object sender, EventArgs e)
		{
			addNewFolder(rightClickedNode);
		}

		private void feedSubNewFolderContextMenuStripItem_Click(object sender, EventArgs e)
		{
			addNewFolder(rightClickedNode.Parent);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showDeleteFolderConfirmation())
			{
				deleteFolder(rightClickedNode);
			}
		}

		private void markFeedReadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			markFeedRead((FeedSubscription)rightClickedNode.Tag);
		}

		private void markAllReadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			markFolderRead((FeedFolder)rightClickedNode.Tag);
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
			handleHotKey(e.KeyChar);
		}

		private void control_KeyPress(object sender, KeyPressEventArgs e)
		{
			//necessary for hotkey support
			e.Handled = true;
		}

		private void feedItemsLV_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			sortFeedItemsLV(e.Column);
			saveFeedSubTree();
		}

		private void smartSortCB_CheckedChanged(object sender, EventArgs e)
		{
			currentlyDisplayedFeedSubscription.ColumnSorter.SmartSortEnabled = smartSortCB.Checked;
			feedItemsLV.Sort();
			saveFeedSubTree();
		}















	
    }
}