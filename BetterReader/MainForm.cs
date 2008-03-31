using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BetterReader.Backend;
using BetterReader.UIManagers;

namespace BetterReader
{
    public partial class MainForm : Form
    {
		
		private delegate void displayFeedItemsIfSelectedDelegate(TreeNode node, FeedSubscription fs);
		private FeedSubTreeManager feedSubManager;
		private readonly string settingsDirectory = string.Format("{0}\\appSettings\\", Environment.CurrentDirectory);
		private readonly string feedSubsFilepath;
		private static string archiveDirectory;
		private Icon redLightIcon;
    	private Icon yellowLightIcon;
    	private Icon greenLightIcon;
    	private TreeNode rightClickedNode;
		private const string newUnreadItemsMessage = "You have new, unread items.";
		private const string oldUnreadItemsMessage = "You have unread items.";
		private const string noUnreadItemsMessage = "You have no unread items.";
		private Color controlBackgroundColor = Color.WhiteSmoke;
		private FormWindowState stateBeforeMinimize;
		private FeedItemsListManager feedItemsManager;

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

			feedsTV.BackColor = feedItemsLV.BackColor = controlBackgroundColor;

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			redLightIcon = Icon.FromHandle(((Bitmap)notifyIconImageList.Images[2]).GetHicon());
			yellowLightIcon = Icon.FromHandle(((Bitmap)notifyIconImageList.Images[1]).GetHicon());
			greenLightIcon = Icon.FromHandle(((Bitmap)notifyIconImageList.Images[0]).GetHicon());
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				MessageBox.Show(ex.ToString(), "Error encountered");
			}

			MessageLogger.WriteToEventLog("Error: " + ex.ToString());
		}

		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.ToString(), "Error encoutnered");

			MessageLogger.WriteToEventLog("Error: " + e.Exception.ToString());
		}

        private void MainForm_Load(object sender, EventArgs e)
		{
			webBrowser1.GotFocus += new EventHandler(webBrowser1_GotFocus);
			ensureDirectoryExists(settingsDirectory);
			ensureDirectoryExists(archiveDirectory);
			
			feedSubManager = new FeedSubTreeManager(feedSubsFilepath, feedsTV, hideReadFeedsBTN);
			feedItemsManager = new FeedItemsListManager(feedItemsLV, itemLinkLBL, itemTitleLBL, lastDownloadLBL, 
				showUnreadFirstBTN, markAllReadBTN, feedSubManager);

			restoreWindowSettings();
						
			feedReaderBGW.RunWorkerAsync();

			processCommandLineArgs(Environment.GetCommandLineArgs());
        }

		private void processCommandLineArgs(string[] cmdLine)
		{
			if (cmdLine.Length == 2)
			{
				rightClickedNode = null;
				showNewSubscriptionForm(cmdLine[1]);
			}
		}



		void webBrowser1_GotFocus(object sender, EventArgs e)
		{
			//this is needed for hotkey support
			feedItemsLV.Focus();
		}

		private void displayFeedItems(FeedSubscription feedSubscription)
		{
			feedItemsManager.DisplayFeedItems(feedSubscription);
			clearWebBrowser();
			feedTitleLBL.Text = feedSubscription.DisplayName;
			feedTitleLBL.Width = splitContainer2.Panel1.Width;
		}

		private void restoreWindowSettings()
		{
			hideReadFeedsBTN.Checked = Properties.Settings.Default.HideReadFeeds;

			if (Properties.Settings.Default.MySize != null)
			{
				this.Size = Properties.Settings.Default.MySize;
			}

			if (Properties.Settings.Default.MyLoc != null)
			{
				this.Location = Properties.Settings.Default.MyLoc;
			}

			Visible = false;
			WindowState = FormWindowState.Normal;

			//try
			//{
				splitContainer1.SplitterDistance = Properties.Settings.Default.SplitterDistance1;
				splitContainer2.SplitterDistance = Properties.Settings.Default.SplitterDistance2;
				splitContainer3.SplitterDistance = Properties.Settings.Default.SplitterDistance3;
				splitContainer4.SplitterDistance = Properties.Settings.Default.SplitterDistance4;
				splitContainer5.SplitterDistance = Properties.Settings.Default.SplitterDistance5;
			//}
			//catch { }

			this.WindowState = Properties.Settings.Default.MyState;
			if (this.WindowState != FormWindowState.Minimized)
			{
				Visible = true;
			}
		}


        private void feedSubReadCallback(FeedSubscription fs)
		{
			if (IsDisposed && Disposing == false)
			{
				return;
			}

			TreeNode node = feedSubManager.UpdateNodeFromFeedSubscription(fs);

			if (InvokeRequired)
			{
				feedsTV.Invoke(new displayFeedItemsIfSelectedDelegate(displayFeedItemsIfNodeSelected),
			   new object[] { node, fs });
			}
			else
			{
				displayFeedItemsIfNodeSelected(node, fs);
			}

			if (fs.Feed.HasNewItemsFromLastRead && notifyIcon1.Visible)
			{
				//the app is minimized and new items were found so set the notifyIcon to alert status
				notifyIcon1.Icon = redLightIcon;
				notifyIcon1.Text = newUnreadItemsMessage;
			}
        }

		

		private void displayFeedItemsIfNodeSelected(TreeNode node, FeedSubscription fs)
		{
			if (node.IsSelected)
			{
				displayFeedItems(fs);
			}
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
				feedSubManager.ImportOpml(ofd.FileName);
				feedReaderBGW.RunWorkerAsync();
            }
        }

		

		private void clearWebBrowser()
		{
			itemLinkLBL.Text = "";
			itemTitleLBL.Text = "";
			if (webBrowser1.DocumentText.Length > 0)
			{
				webBrowser1.DocumentText = "";
			}
		}

		private void displaySelectedFeedItem()
		{
			if (feedItemsLV.SelectedItems.Count > 0)
			{
				FeedItem fi = feedItemsLV.SelectedItems[0].Tag as FeedItem;
				fi.ParentFeed.ParentSubscription.ResetUpdateTimer();
//				currentlyDisplayedFeedItemGuid = fi.Guid;
				itemTitleLBL.Text = fi.Title;
				itemTitleLBL.Visible = true;
				itemLinkLBL.Visible = true;
				itemLinkLBL.Text = fi.LinkUrl;
				itemLinkLBL.Links[0].LinkData = fi.LinkUrl;
				itemLinkLBL.LinkVisited = false;

				switch (fi.ParentFeed.ParentSubscription.FeedItemClickAction)
				{
					case FeedItemClickAction.Default:
						displayFeedItemDefaultMethod(fi);
						break;
					case FeedItemClickAction.LoadDescriptionInternalBrowser:
						setWebBrowserText(getFeedItemDescription(fi));
						break;
					case FeedItemClickAction.LoadLinkInternalBrowser:
						setWebBrowserUrlToFeedItemLinkUrl(fi);
						break;
					case FeedItemClickAction.LoadLinkExternalBrowser:
						webBrowser1.DocumentText = formatDescriptionHTML("Link: <a href='" + fi.LinkUrl + "'>" +
							fi.LinkUrl + "</a> loaded in external browser.  This action may be changed by right-clicking " +
							"the Feed Subscription and choosing 'Properties . . .'");
						launchExternalBrowser(fi.LinkUrl);
						break;
				}

				feedItemsManager.MarkFeedItemRead(fi);
			}
		}

		private void displayFeedItemDefaultMethod(FeedItem fi)
		{
			if (fi != null)
			{
				string docText = getFeedItemDescription(fi);

				if (docText != "")
				{
					setWebBrowserText(docText);
				}
				else
				{
					webBrowser1.DocumentText = formatDescriptionHTML("Loading page . . .");
					setWebBrowserUrlToFeedItemLinkUrl(fi);
				}
			}
		}

		private void setWebBrowserUrlToFeedItemLinkUrl(FeedItem fi)
		{
			if (webBrowser1.Url == null || webBrowser1.Url.AbsoluteUri != fi.LinkUrl)
			{
				webBrowser1.Navigate(fi.LinkUrl);
			}
		}

		private void setWebBrowserText(string docText)
		{
			if (webBrowser1.DocumentText.Length != docText.Length)
			{
				webBrowser1.DocumentText = docText;
			}
		}

		private string getFeedItemDescription(FeedItem fi)
		{
			string docText = "";
			if (fi.EncodedContent != null && fi.EncodedContent.Length > 0)
			{
				docText = formatDescriptionHTML(fi.EncodedContent);
			}
			else if (fi.Description != null && fi.Description.Length > 0)
			{
				docText = formatDescriptionHTML(fi.Description);
			}
			return docText;
		}


		private void visitLink(LinkLabel.Link link)
		{
			link.Visited = true;
			launchExternalBrowser(link.LinkData.ToString());
		}

		private static void launchExternalBrowser(string link)
		{
			System.Diagnostics.Process.Start(link);
		}

		private string formatDescriptionHTML(string description)
		{
			return "<html><body style='margin:8px; overflow:auto; font-family:Tahoma; font-size:9pt;'>" +
				description + "</body></html>";
		}


		private void hideFormShowNotifyIcon()
		{
			if (feedSubManager.GetUnreadItemCount() > 0)
			//if (fst != null && fst.GetUnreadItemCount() > 0)
			{
				notifyIcon1.Icon = yellowLightIcon;
				notifyIcon1.Text = oldUnreadItemsMessage;
			}
			else
			{
				notifyIcon1.Icon = greenLightIcon;
				notifyIcon1.Text = noUnreadItemsMessage;
			}

			Hide();
			notifyIcon1.Visible = true;
		}

		private void showFormHideNotifyIcon()
		{
			notifyIcon1.Visible = false;
			Show();
			WindowState = stateBeforeMinimize;
		}

		private bool showUnsubscribeConfirmation()
		{
			if (MessageBox.Show("Unsubscribe from this feed?", "Unsubscribe?", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool showDeleteFolderConfirmation()
		{
			if (MessageBox.Show("Delete Folder and all children?", "Delete Folder", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void showNewSubscriptionForm()
		{
			showNewSubscriptionForm(String.Empty);
		}

		private void showNewSubscriptionForm(string feedUrl)
		{

			FeedFolder parentFolder = null;
			if (rightClickedNode != null)
			{
				//if rightClickedNode isn't null then we need to figure out the proper parent node
				//for the sub form to default to 
				if (rightClickedNode.Tag.GetType() == typeof(FeedFolder))
				{
					parentFolder = (FeedFolder)rightClickedNode.Tag;
				}
				else if (rightClickedNode.Tag.GetType() == typeof(FeedSubscription))
				{
					parentFolder = ((FeedSubscription)rightClickedNode.Tag).ParentFolder;
				}
			}

			NewSubscriptionForm nsf = new NewSubscriptionForm(feedSubManager.FeedSubscriptionTree, parentFolder, feedUrl);

			if (nsf.ShowDialog() == DialogResult.OK)
			{
				FeedSubscription fs = nsf.FeedSubscription;
				fs.ParentFeedSubTree = feedSubManager.FeedSubscriptionTree;
				feedSubManager.AddFeedSubscriptionToFolder(parentFolder, fs);

				fs.BeginReadFeed(feedSubReadCallback);
			}
		}

		private void markFeedRead(FeedSubscription fs)
		{
			fs.MarkAllItemsRead();
			feedSubManager.UpdateNodeFromFeedSubscription(fs);
			if (feedItemsManager.CurrentlyDisplayedFeedSubscription == fs)
			{
				displayFeedItems(fs);
			}
			fs.ResetUpdateTimer();
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
					markFeedRead(feedItemsManager.CurrentlyDisplayedFeedSubscription);
					break;
				case 'm':
				case 'M':
					WindowState = FormWindowState.Minimized;
					break;
				case 'h':
				case 'H':
					hideReadFeedsBTN.Checked = !hideReadFeedsBTN.Checked;
					break;
			}
		}

		



		


		private void showFeedSubPropertiesDialog(FeedSubscription fs)
		{
			SubscriptionPropertiesForm spf = new SubscriptionPropertiesForm(fs);
			if (spf.ShowDialog() == DialogResult.OK)
			{
				feedSubManager.SaveFeedSubTree();
				fs.Feed.FeedItems.PurgeOldItems();
				feedSubManager.UpdateNodeFromFeedSubscription(fs);
				//setNodePropertiesFromFeedSubscription(fs, feedSubManager.TreeNodesByTag[fs]);
				if (fs.Feed.ReadSuccess != true)
				{
					fs.BeginReadFeed(new FeedSubscriptionReadDelegate(feedSubReadCallback));
					fs.ResetUpdateTimer();
				}
			}			
		}

		private void toggleHideShowOnFeedSubNodes()
		{
			if (hideReadFeedsBTN.Checked)
			{
				feedsTV.HideReadNodes();
			}
			else
			{
				feedsTV.ShowHiddenNodes();
			}
		}



		//
		//
		//
		//event handlers below
		//
		//
		//
		private void feedReaderBGW_DoWork(object sender, DoWorkEventArgs e)
		{
			feedSubManager.BeginReadAllFeeds(new FeedSubscriptionReadDelegate(feedSubReadCallback));
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

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			saveFormSettings();

			if (feedSubManager != null)
			{
				feedSubManager.Dispose();
			}
		}

		private void saveFormSettings()
		{
			Properties.Settings.Default.MyState = this.WindowState;
			Properties.Settings.Default.HideReadFeeds = hideReadFeedsBTN.Checked;

			if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
			{
				Properties.Settings.Default.SplitterDistance1 = splitContainer1.SplitterDistance;
				Properties.Settings.Default.SplitterDistance2 = splitContainer2.SplitterDistance;
				Properties.Settings.Default.SplitterDistance3 = splitContainer3.SplitterDistance;
				Properties.Settings.Default.SplitterDistance4 = splitContainer4.SplitterDistance;
				Properties.Settings.Default.SplitterDistance5 = splitContainer5.SplitterDistance;
				Properties.Settings.Default.MySize = this.Size;
				Properties.Settings.Default.MyLoc = this.Location;
			}
			else
			{
				Properties.Settings.Default.MySize = this.RestoreBounds.Size;
				Properties.Settings.Default.MyLoc = this.RestoreBounds.Location;
			}

			Properties.Settings.Default.Save();
		}

		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				hideFormShowNotifyIcon();
			}
			else
			{
				stateBeforeMinimize = WindowState;
			}

		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			showFormHideNotifyIcon();
		}

		private void feedsTV_DragComplete(object sender, Sloppycode.UI.DragCompleteEventArgs e)
		{
			feedSubManager.DoDragComplete();
		}

		private void newSubscriptionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showNewSubscriptionForm();
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
			FeedSubscription fs = rightClickedNode.Tag as FeedSubscription;
			if (fs == null)
			{
				//this should never occur
				MessageBox.Show("Error.  Cannot unsubscribe from this type of node.");
				return;
			}

			if (showUnsubscribeConfirmation())
			{
				TreeNodeCollection parentNodes;
				if (rightClickedNode.Parent == null)
				{
					parentNodes = feedsTV.Nodes;
				}
				else
				{
					parentNodes = rightClickedNode.Parent.Nodes;
				}
				parentNodes.Remove(rightClickedNode);
				fs.Unsubscribe();
				feedSubManager.SaveFeedSubTree();
			}
		}


		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showFeedSubPropertiesDialog((FeedSubscription)rightClickedNode.Tag);
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
			feedSubManager.SaveFeedSubTree();
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
			feedSubManager.AddNewFolder(rightClickedNode);
		}

		private void feedSubNewFolderContextMenuStripItem_Click(object sender, EventArgs e)
		{
			feedSubManager.AddNewFolder(rightClickedNode.Parent);
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (showDeleteFolderConfirmation())
			{
				feedSubManager.DeleteFolder(rightClickedNode);
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


		private void feedItemsLV_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			feedItemsManager.SortFeedItemsLV(e.Column);
			feedSubManager.SaveFeedSubTree();
		}

		private void hideReadFeedsBTN_CheckedChanged(object sender, EventArgs e)
		{
			feedItemsManager.CurrentlyDisplayedFeedSubscription.ColumnSorter.SmartSortEnabled = showUnreadFirstBTN.Checked;
			feedItemsLV.Sort();
			feedSubManager.SaveFeedSubTree();
		}


		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void newFeedSubscriptionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			showNewSubscriptionForm(null);
		}

		private void newFolderToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			feedSubManager.AddNewFolder(null);
		}

		private void copyLinkLocationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FeedSubscription fs = (FeedSubscription)rightClickedNode.Tag;
			Clipboard.SetText(fs.FeedUrl);
		}

		private void feedItemsLV_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			visitLink(itemLinkLBL.Links[0]);
		}

		private void reloadNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FeedSubscription fs = (FeedSubscription)rightClickedNode.Tag;
			fs.BeginReadFeed(feedSubReadCallback);
		}

		private void hideReadFeedsCB_CheckedChanged(object sender, EventArgs e)
		{
			toggleHideShowOnFeedSubNodes();
		}

		private void renameFolderMenuItem1_Click(object sender, EventArgs e)
		{
			feedsTV.LabelEdit = true;
			rightClickedNode.BeginEdit();
		}

		private void markFeedReadButton1_Click(object sender, EventArgs e)
		{
			markFeedRead(feedItemsManager.CurrentlyDisplayedFeedSubscription);
		}

		private void showUnreadFirstBTN_Click(object sender, EventArgs e)
		{
			FeedItemsListViewColumnSorter sorter = (FeedItemsListViewColumnSorter)feedItemsLV.ListViewItemSorter;
			sorter.SmartSortEnabled = showUnreadFirstBTN.Checked;
			feedItemsLV.Sort();
			feedSubManager.SaveFeedSubTree();
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			//this is needed for hotkey support
			feedItemsLV.Focus();
		}

		private void clearBrowserBTN_Click(object sender, EventArgs e)
		{
			clearWebBrowser();
		}
		
		public void StartupNextInstanceHandler(object sender, Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e)
		{
			// do whatever you want here with e.CommandLine...
			if (e.CommandLine.Count > 0)
			{
				showFormHideNotifyIcon();
				string[] cmdLine = new string[e.CommandLine.Count];
				e.CommandLine.CopyTo(cmdLine, 0);
				processCommandLineArgs(cmdLine);
			}
		}



    }
}