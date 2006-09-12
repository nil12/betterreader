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
		private delegate void noArgsDelegate();
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
				//beginReads();
				feedReaderBGW.RunWorkerAsync();
			}
            //importOpml(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
            //fsc.SaveAsFeedSubscriptionsFile("FeedSubscriptions.xml");
			//fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile("FeedSubscriptions.xml");
			////fst.ReadAllFeeds();
			//bindFSTToTreeView();
        }

		private void beginReads()
		{
			fst.BeginReadAllFeeds(new FeedSubscriptionReadDelegate(feedSubReadCallback));
		}

        private void feedSubReadCallback(FeedSubscription fs)
        {
			TreeNode node = treeNodesByTag[fs];
			string text;

			this.Invoke(new noArgsDelegate(feedsTV.BeginUpdate));
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

			this.Invoke(new noArgsDelegate(feedsTV.EndUpdate));
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
			feedItemsLV.ResumeLayout();
		}

		private void bindFeedItemsToListView(List<FeedItem> feedItems)
		{
			feedItemsLV.Columns.Add("Title");
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
				if (fi.HasBeenRead)
				{
					lvi.Font = feedItemsNormalFont;
				}
				else
				{
					lvi.Font = feedItemsBoldFont;
				}

				setColumnWidth(feedItemsLV.Columns[0], lvi);
				feedItemsLV.Items.Add(lvi);
				listViewItemsByTag.Add(fi, lvi);
			}
		}

		private void setColumnWidth(ColumnHeader column, ListViewItem lvi)
		{
			SizeF size = formGraphics.MeasureString(lvi.Text, lvi.Font);

			int newWidth = (int)size.Width + 50;
			if (column.Width < newWidth)
			{
				column.Width = newWidth;
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

		private void feedsTV_ItemDrag(object sender, ItemDragEventArgs e)
		{
			DoDragDrop(e.Item, DragDropEffects.Move);
		}

		private void feedsTV_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
		}

		private void feedsTV_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false))
			{
				Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
				TreeNode destinationNode = ((TreeView)sender).GetNodeAt(pt);
				TreeNode movedNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
				moveTreeNode(movedNode, destinationNode);
			}
		}






	
    }
}