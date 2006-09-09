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
    public partial class Form1 : Form
    {
		
		private delegate void setNodeTextDelegate(TreeNode node, string text);
        private FeedSubscriptionTree fst;
        private Dictionary<object, TreeNode> treeNodesByTag;
		private readonly string settingsDirectory = System.Environment.CurrentDirectory + "\\appSettings\\";
		private readonly string feedSubsFilepath;
		private Font feedsNormalFont;
		private Font feedsBoldFont;
		private Font feedItemsNormalFont;
		private Font feedItemsBoldFont;
		private Graphics formGraphics;

        public Form1()
        {
			InitializeComponent();
			feedSubsFilepath = settingsDirectory + "FeedSubscriptions.xml";
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			formGraphics = this.CreateGraphics();
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
			feedsNormalFont = feedsTV.Font;
			feedsBoldFont = new Font(feedsNormalFont, FontStyle.Bold);
			feedItemsNormalFont = feedItemsLV.Font;
			feedItemsBoldFont = new Font(feedItemsNormalFont, FontStyle.Bold);

			if (File.Exists(feedSubsFilepath))
			{
				fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile(feedSubsFilepath);
				bindFSTAndBeginReads();
				//feedReaderBGW.RunWorkerAsync();
			}
            //importOpml(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
            //fsc.SaveAsFeedSubscriptionsFile("FeedSubscriptions.xml");
			//fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile("FeedSubscriptions.xml");
			////fst.ReadAllFeeds();
			//bindFSTToTreeView();
        }

		private void bindFSTAndBeginReads()
		{
			bindFSTToTreeView();
			fst.BeginReadAllFeeds(new FeedSubscriptionReadDelegate(feedSubReadCallback));
		}

        private void feedSubReadCallback(FeedSubscription fs)
        {
			TreeNode node = treeNodesByTag[fs];
			string text;

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
				//throw fs.Feed.ReadException;
				text = fs.DisplayName + "(" + fs.Feed.ReadException.ToString() + ")";
			}
			try
			{
				this.Invoke(new setNodeTextDelegate(setNodeText), new object[] { node, text });
			}
			catch (InvalidOperationException) 
			{
				//this was most likely caused by a feed reading thread returning during shutdown
				//so we'll ignore it
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
            fst = new FeedSubscriptionTree();
            fst.LoadFromOpml(filepath);
			fst.SaveAsFeedSubscriptionsFile(settingsDirectory + "FeedSubscriptions.xml");
			feedReaderBGW.RunWorkerAsync();
        }

        private void bindFSTToTreeView()
        {
            feedsTV.SuspendLayout();
			feedsTV.Nodes.Clear();
            treeNodesByTag = new Dictionary<object, TreeNode>();
			//bindFolderToTreeView(fsc.RootFolder, null);
			bindNodeListToTreeView(fst.RootLevelNodes, null);
            feedsTV.ResumeLayout();
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


		private void feedReaderBGW_DoWork(object sender, DoWorkEventArgs e)
		{
			bindFSTAndBeginReads();
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




	
    }
}