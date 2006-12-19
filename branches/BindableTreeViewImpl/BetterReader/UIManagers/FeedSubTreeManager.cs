using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using BetterReader.Backend;

namespace BetterReader.UIManagers
{
	/// <summary>
	/// FeedSubTreeManager is a helper class to mediate between the UI elements and the FeedSubscriptionTree data structure.
	/// The main purpose of this class is to consolidate the code related to the relationship between the UI and the tree 
	/// structure.
	/// </summary>
	public class FeedSubTreeManager : IDisposable
	{
		private delegate void setFeedSubNodeTextDelegate(TreeNode node, FeedSubscription fs);
		private FeedSubscriptionTree fst;
		private Dictionary<object, TreeNode> treeNodesByTag;
		private ToolStripButton hideReadFeedsBTN; 
		private Font feedsNormalFont;
		private Font feedsBoldFont;

		public ToolStripButton HideReadFeedsBTN
		{
			get { return hideReadFeedsBTN; }
			set { hideReadFeedsBTN = value; }
		}

		public Dictionary<object, TreeNode> TreeNodesByTag
		{
			get { return treeNodesByTag; }
			set { treeNodesByTag = value; }
		}

		public FeedSubscriptionTree FeedSubscriptionTree
		{
			get { return fst; }
			set { fst = value; }
		}
		private string feedSubsFilepath;

		public string FeedSubsFilePath
		{
			get { return feedSubsFilepath; }
			set { feedSubsFilepath = value; }
		}

		private FeedsTreeView feedsTreeView;

		internal FeedsTreeView FeedsTreeView
		{
			get { return feedsTreeView; }
			set { feedsTreeView = value; }
		}

		public FeedSubTreeManager(string lFeedSubsFilePath, FeedsTreeView lFeedsTreeView, ToolStripButton lHideReadFeedsBTN)
		{
			feedSubsFilepath = lFeedSubsFilePath;
			feedsTreeView = lFeedsTreeView;
			hideReadFeedsBTN = lHideReadFeedsBTN;
			feedsNormalFont = feedsTreeView.Font;
			feedsBoldFont = new Font(feedsNormalFont, FontStyle.Bold);

			if (File.Exists(feedSubsFilepath))
			{
				fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile(feedSubsFilepath);
				BindTreeView();
			}
			else
			{
				fst = new FeedSubscriptionTree();
			}
		}

		public void BeginReadAllFeeds(FeedSubscriptionReadDelegate feedSubscriptionReadDelegate)
		{
			fst.BeginReadAllFeeds(feedSubscriptionReadDelegate);
		}

		internal void ImportOpml(string filepath)
		{
			fst = Opml.GetFeedSubscriptionTreeFromOpmlFile(filepath);
			fst.SaveAsFeedSubscriptionsFile(feedSubsFilepath);
			BindTreeView();
		}

		internal void BindTreeView()
		{
			feedsTreeView.Nodes.Clear();
			treeNodesByTag = new Dictionary<object, TreeNode>();
			feedsTreeView.DataBind(fst.RootLevelNodes);
			//bindNodeListToTreeView(fst.RootLevelNodes, null);
			feedsTreeView.ExpandAll();
		}

		//private void bindNodeListToTreeView(List<FeedSubTreeNodeBase> nodeList, TreeNode treeNode)
		//{
		//    foreach (FeedSubTreeNodeBase fstnb in nodeList)
		//    {
		//        Type nodeType = fstnb.GetType();
		//        if (nodeType == typeof(FeedSubscription))
		//        {
		//            FeedSubscription fs = fstnb as FeedSubscription;
		//            TreeNode newNode;
		//            if (treeNode == null)
		//            {
		//                //we're at the root level of the tree
		//                newNode = feedsTreeView.Nodes.Add(fs.DisplayName);
		//            }
		//            else
		//            {
		//                newNode = treeNode.Nodes.Add(fs.DisplayName);
		//            }

		//            newNode.Tag = fs;
		//            newNode.ImageIndex = 1;
		//            newNode.SelectedImageIndex = 1;
		//            TreeNodesByTag.Add(fs, newNode);
		//        }
		//        else if (nodeType == typeof(FeedFolder))
		//        {
		//            FeedFolder ff = fstnb as FeedFolder;
		//            TreeNode newNode;
		//            if (treeNode == null)
		//            {
		//                //we're at the root level of the tree
		//                newNode = feedsTreeView.Nodes.Add(ff.Name);
		//            }
		//            else
		//            {
		//                newNode = treeNode.Nodes.Add(ff.Name);
		//            }

		//            newNode.Tag = ff;
		//            newNode.ImageIndex = 0;
		//            newNode.SelectedImageIndex = 0;

		//            TreeNodesByTag.Add(ff, newNode);
		//            if (ff.IsExpandedInUI)
		//            {
		//                newNode.Expand();
		//                bool expanded = newNode.IsExpanded;
		//            }
		//            bindNodeListToTreeView(ff.ChildNodes, newNode);
		//        }
		//        else
		//        {
		//            throw new Exception("Form1.bindNodeListToTreeView error: Unrecognized node type: " + nodeType.ToString());
		//        }
		//    }
		//}

		internal int GetUnreadItemCount()
		{
			return fst.GetUnreadItemCount();
		}

		internal void moveNode(TreeNode movedNode, TreeNode destinationNode)
		{
			fst.MoveNode((FeedSubTreeNodeBase)movedNode.Tag, (FeedSubTreeNodeBase)destinationNode.Tag);
			BindTreeView();
		}

		internal void DoDragComplete()
		{
			bool readFeedsHidden = hideReadFeedsBTN.Checked;

			if (readFeedsHidden)
			{
				//if the read feeds are hidden we need to temporarily unhide them
				//so that we can accurately get the state of the full tree from the treeview
				hideReadFeedsBTN.Checked = false;
			}

			fst.ReloadFromTreeView(feedsTreeView);
			SaveFeedSubTree();

			hideReadFeedsBTN.Checked = readFeedsHidden;
		}

		internal void SaveFeedSubTree()
		{
			fst.SaveAsFeedSubscriptionsFile(feedSubsFilepath);
		}

		internal void AddNewFolder(TreeNode parentNode)
		{
			FeedFolder parentFolder = null;
			TreeNode newNode = null;

			FeedFolder newFolder = new FeedFolder();

			newFolder.Name = "New Folder";

			if (fst == null)
			{
				//this is most likely a new user with no subscriptions who has chosen to start
				//with a new folder
				fst = new FeedSubscriptionTree();
			}

			if (parentNode == null)
			{
				newFolder.ParentFolder = null;
				fst.RootLevelNodes.Add(newFolder);
				newNode = feedsTreeView.Nodes.Add(newFolder.Name);
			}
			else
			{
				parentFolder = (FeedFolder)parentNode.Tag;

				newFolder.ParentFolder = parentFolder;
				parentFolder.ChildNodes.Add(newFolder);

				newNode = parentNode.Nodes.Add(newFolder.Name);
			}

			newNode.Tag = newFolder;
			SaveFeedSubTree();
		}

		internal void DeleteFolder(TreeNode rightClickedNode)
		{
			FeedFolder ff = (FeedFolder)rightClickedNode.Tag;
			if (ff.ParentFolder == null)
			{
				fst.RootLevelNodes.Remove(ff);
				feedsTreeView.Nodes.Remove(rightClickedNode);
			}
			else
			{
				ff.ParentFolder.ChildNodes.Remove(ff);
				rightClickedNode.Parent.Nodes.Remove(rightClickedNode);
			}
			SaveFeedSubTree();
		}

		internal void AddFeedSubscriptionToFolder(FeedFolder parentFolder, FeedSubscription fs)
		{
			if (parentFolder == null)
			{
				fst.RootLevelNodes.Add(fs);
			}
			else
			{
				parentFolder.ChildNodes.Add(fs);
			}

			TreeNode newNode = new TreeNode(fs.ToString());
			newNode.Tag = fs;
			newNode.ImageIndex = 1;
			if (TreeNodesByTag != null && parentFolder != null && TreeNodesByTag.ContainsKey(parentFolder))
			{
				TreeNode parentNode = TreeNodesByTag[parentFolder];
				parentNode.Nodes.Add(newNode);
			}
			else
			{
				feedsTreeView.Nodes.Add(newNode);
			}


			if (TreeNodesByTag == null)
			{
				TreeNodesByTag = new Dictionary<object, TreeNode>();
			}
			TreeNodesByTag.Add(fs, newNode);
			SaveFeedSubTree();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (fst != null)
			{
				fst.Dispose();
			}
		}

		#endregion

		internal TreeNode UpdateNodeFromFeedSubscription(FeedSubscription fs)
		{
			TreeNode node = TreeNodesByTag[fs];
			setNodePropertiesFromFeedSubscription(fs, node);
			return node;
		}

		private void setNodePropertiesFromFeedSubscription(FeedSubscription fs, TreeNode node)
		{

			lock (feedsTreeView)
			{
				try
				{
					if (feedsTreeView.InvokeRequired)
					{
						feedsTreeView.Invoke(new setFeedSubNodeTextDelegate(setFeedSubNodeText), new object[] { node, fs });
						
					}
					else
					{
						setFeedSubNodeText(node, fs);
					}

					
				}
				catch (InvalidOperationException e)
				{
					//this was most likely caused by a feed reading thread returning during shutdown
					//so we'll ignore it
					string x = e.ToString();
				}
			}
		}

		private void setFeedSubNodeText(TreeNode node, FeedSubscription fs)
		{
			string text;

			if (fs.Feed.ReadSuccess)
			{
				text = fs.ToString();
				if (fs.Feed.UnreadCount > 0)
				{
					node.NodeFont = feedsBoldFont;
				}
				else
				{
					node.NodeFont = feedsNormalFont;
				}

				if (fs.Feed.UnreadCount == 0 && hideReadFeedsBTN.Checked)
				{
					//user has selected hideReadFeeds option and this feed has no unread items
					feedsTreeView.HideNode(node);
				}
				else
				{
					feedsTreeView.ShowNode(fs);
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

			node.Text = text;
		}
	}
}
