using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace BetterReader.Backend
{

	[XmlInclude(typeof(FeedFolder))]
	[XmlInclude(typeof(FeedSubscription))]
    public class FeedSubscriptionTree : IDisposable
    {
		private List<FeedSubTreeNodeBase> rootLevelNodes;

		public List<FeedSubTreeNodeBase> RootLevelNodes
		{
			get { return rootLevelNodes; }
			set { rootLevelNodes = value; }
		}

		public FeedSubscriptionTree()
		{
			rootLevelNodes = new List<FeedSubTreeNodeBase>();
		}

        public void SaveAsFeedSubscriptionsFile(string filepath)
        {
            using (TextWriter tw = new StreamWriter(filepath))
            {
				XmlSerializer xs = new XmlSerializer(this.GetType());
				xs.Serialize(tw, this);
				//tw.WriteLine("here");
				tw.Close();
            }
        }

		public void ReloadFromTreeView(System.Windows.Forms.TreeView treeView)
		{
			rootLevelNodes.Clear();
			loadNodesFromTreeNodeCollection(treeView.Nodes, rootLevelNodes);
		}

		private void loadNodesFromTreeNodeCollection(System.Windows.Forms.TreeNodeCollection treeNodes, List<FeedSubTreeNodeBase> feedSubNodes)
		{
			foreach (System.Windows.Forms.TreeNode treeNode in treeNodes)
			{
				FeedSubTreeNodeBase fstnb = treeNode.Tag as FeedSubTreeNodeBase;
				if (fstnb == null)
				{
					throw new Exception("Error loading node: " + treeNode.Text + 
						".  Tag is null or not a FeedSubTreeNodeBase.");
				}

				feedSubNodes.Add(fstnb);


				if (fstnb.GetType() == typeof(FeedFolder))
				{
					FeedFolder ff = (FeedFolder)fstnb;
					ff.ChildNodes.Clear();
					if (treeNode.Nodes.Count > 0)
					{
						loadNodesFromTreeNodeCollection(treeNode.Nodes, ff.ChildNodes);
					}
					else
					{
						throw new Exception("Error loading node: " + treeNode.Text +
							".  Tag is not a FeedFolder node but TreeNode has child nodes.");
					}
				}
			}
		}

        public static FeedSubscriptionTree GetFromFeedSubscriptionsFile(string filepath)
        {
            FeedSubscriptionTree fsc = null;
            using (TextReader tr = new StreamReader(filepath))
            {
				XmlSerializer xs = new XmlSerializer(typeof(FeedSubscriptionTree));
                fsc = xs.Deserialize(tr) as FeedSubscriptionTree;
            }

			fsc.setParentFolderOnNodesInList(fsc.rootLevelNodes, null);
            return fsc;
        }

		private void setParentFolderOnNodesInList(List<FeedSubTreeNodeBase> nodeList, FeedFolder curParent)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				fstnb.ParentFolder = curParent;
				if (fstnb.GetType() == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					setParentFolderOnNodesInList(ff.ChildNodes, ff);
				}
			}
		}

		      
		public void BeginReadAllFeeds(FeedSubscriptionReadDelegate callback)
		{
			beginReadAllFeedsInNodeList(rootLevelNodes, callback);
		}

		private void beginReadAllFeedsInNodeList(List<FeedSubTreeNodeBase> nodeList, FeedSubscriptionReadDelegate callback)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				Type t = fstnb.GetType();
				if (t == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					fs.BeginReadFeed(callback);
				}
				else if (t == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					beginReadAllFeedsInNodeList(ff.ChildNodes, callback);
				}
				else
				{
					throw new Exception("FeedSubscriptionTree.ReadAllFeeds error: unsupported node type: " + t.ToString());
				}
			}
		}

		private void disposeAllFeedsInNodeList(List<FeedSubTreeNodeBase> nodeList)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				Type t = fstnb.GetType();
				if (t == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					if (fs != null)
					{
						fs.Dispose();
					}
				}
				else if (t == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					disposeAllFeedsInNodeList(ff.ChildNodes);
				}
			}
		}

		public int GetUnreadItemCount()
		{
			return getUnreadItemCountFromNodeList(rootLevelNodes, 0);
		}

		private int getUnreadItemCountFromNodeList(List<FeedSubTreeNodeBase> nodeList, int curCount)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				Type t = fstnb.GetType();
				if (t == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					if (fs != null && fs.Feed != null && fs.Feed.FeedItems != null)
					{
						foreach (FeedItem fi in fs.Feed.FeedItems)
						{
							if (fi.HasBeenRead == false)
							{
								curCount++;
							}
						}
					}
				}
				else if (t == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					if (ff.ChildNodes != null)
					{
						curCount += getUnreadItemCountFromNodeList(ff.ChildNodes, curCount);
					}
				}
				else
				{
					throw new Exception("FeedSubscriptionTree.getUnreadItemCountFromNodeList error: Unsupported type: " +
						t.ToString());
				}
			}

			return curCount;
		}


		#region IDisposable Members

		public void Dispose()
		{
			disposeAllFeedsInNodeList(rootLevelNodes);
		}

		#endregion

		public void MoveNode(FeedSubTreeNodeBase movedNode, FeedSubTreeNodeBase destNode)
		{
			if (movedNode.ParentFolder == null)
			{
				rootLevelNodes.Remove(movedNode);
			}
			else
			{
				movedNode.ParentFolder.ChildNodes.Remove(movedNode);
			}
			Type t = destNode.GetType();

			FeedFolder ff = null;

			if (t == typeof(FeedFolder))
			{
				ff = (FeedFolder)destNode;
				ff.ChildNodes.Add(movedNode);
			}
			else if (t == typeof(FeedSubscription))
			{
				ff = destNode.ParentFolder;
				ff.ChildNodes.Insert(ff.ChildNodes.IndexOf(destNode), movedNode);
			}

			movedNode.ParentFolder = ff;
		}
	}
}
