using System;
using System.Collections.Generic;
//using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;
using Sloppycode.UI;

namespace BetterReader
{
	public class FeedsTreeView : TreeViewDragDrop
	{
		private Dictionary<object, HiddenNode> hiddenNodes = new Dictionary<object, HiddenNode>();

		private delegate void HideNodeDelegate(TreeNode node);

		private delegate void InsertNodeDelegate(int index, TreeNode node);

		protected override void OnDragDrop(DragEventArgs e)
		{
			//DisableBackgroundErase = false;
			handleCustomCursor();

			// Check it's a treenode being dragged

			TreeNode dragNode = getTreeNodeFromDragEventArgs(e);
			if (dragNode != null)
			{
				// Get the target node from the mouse coords
				TreeNode targetNode = getTargetNodeFromDragEventArgs(e);

				// De-color it
				decolorNode(targetNode);


				if (dragIsValid(dragNode, targetNode))
				{
					Type t = targetNode.Tag.GetType();

					if (t == typeof (FeedSubscription))
					{
						moveNodeToBeforeTargetNode(dragNode, targetNode);
					}
					else
					{
						moveNodeToNewParent(dragNode, targetNode);
					}

					doPostDragTasks(dragNode, targetNode);
				}
			}
			//this.Invalidate();
			//DisableBackgroundErase = true;
		}

		private void moveNodeToBeforeTargetNode(TreeNode dragNode, TreeNode targetNode)
		{
			TreeNodeCollection dragNodeParentNodesCollection, targetNodeParentNodesCollection;
			dragNodeParentNodesCollection = getParentNodesCollection(dragNode);
			targetNodeParentNodesCollection = getParentNodesCollection(targetNode);


			dragNodeParentNodesCollection.Remove(dragNode);
			targetNodeParentNodesCollection.Insert(targetNode.Index, dragNode);
		}

		private TreeNodeCollection getParentNodesCollection(TreeNode node)
		{
			TreeNodeCollection parentNodesCollection;
			if (node == null || node.Parent == null)
			{
				parentNodesCollection = Nodes;
			}
			else
			{
				parentNodesCollection = node.Parent.Nodes;
			}
			return parentNodesCollection;
		}


		public void HideNode(TreeNode node)
		{
			if (hiddenNodes.ContainsKey(node.Tag))
			{
				//the node is already hidden
				return;
			}

			HiddenNode hn = new HiddenNode(node);
			hiddenNodes.Add(node.Tag, hn);
			TreeNodeCollection parentNodesCollection = getParentNodesCollection(node);
			if (InvokeRequired)
			{
				Invoke(new HideNodeDelegate(parentNodesCollection.Remove), new object[] {node});
			}
			else
			{
				if (SelectedNode == node)
				{
					SelectedNode = null;
				}

				parentNodesCollection.Remove(node);
			}
		}

		public void ShowNode(object tag)
		{
			if (hiddenNodes.ContainsKey(tag) == false)
			{
				//node is not hidden
				return;
			}

			//first get a reference to the hiddenNode then remove it from the hiddenNodes collection (since it will no
			//longer be hidden when we're done here
			HiddenNode hiddenNode = hiddenNodes[tag];
			hiddenNodes.Remove(tag);

			//this loop will determine the correct placement of the newly shown node within the parent's node collection
			//we'll step through each of the parent nodes node collection and examine the tag of each node
			//we'll use the indexes from the FeedSubscriptionTree that the tag and the hidden node are bound to to 
			//determine the proper ordering
			int insertAt = hiddenNode.feedSubIndex;
			TreeNodeCollection parentNodesCollection;
			if (hiddenNode.parentTreeNode != null)
			{
				parentNodesCollection = hiddenNode.parentTreeNode.Nodes;
			}
			else
			{
				parentNodesCollection = Nodes;
			}

			for (int i = 0; i < parentNodesCollection.Count; i++)
			{
				TreeNode node = parentNodesCollection[i];
				FeedSubTreeNodeBase curTag = (FeedSubTreeNodeBase) node.Tag;

				if (curTag.Index > hiddenNode.feedSubIndex)
				{
					insertAt = i;
					break;
				}
			}

			if (InvokeRequired)
			{
				Invoke(new InsertNodeDelegate(parentNodesCollection.Insert), new object[] {insertAt, hiddenNode.treeNode});
			}
			else
			{
				parentNodesCollection.Insert(insertAt, hiddenNode.treeNode);
			}
		}


		internal void HideReadNodes()
		{
			hideNodesInList(Nodes);
		}

		private void hideNodesInList(TreeNodeCollection treeNodeCollection)
		{
			List<TreeNode> nodesToHide = new List<TreeNode>();

			foreach (TreeNode node in treeNodeCollection)
			{
				Type t = node.Tag.GetType();

				if (t == typeof (FeedSubscription))
				{
					FeedSubscription fs = (FeedSubscription) node.Tag;
					if (fs.Feed.UnreadCount == 0)
					{
						nodesToHide.Add(node);
					}
				}

				if (t == typeof (FeedFolder))
				{
					hideNodesInList(node.Nodes);
				}
			}

			foreach (TreeNode node in nodesToHide)
			{
				HideNode(node);
			}
		}

		internal void ShowHiddenNodes()
		{
			object[] keys = new object[hiddenNodes.Count];
			hiddenNodes.Keys.CopyTo(keys, 0);
			foreach (object key in keys)
			{
				ShowNode(key);
			}
			Invalidate();
		}

		private class HiddenNode
		{
			internal TreeNode treeNode;
			internal TreeNode parentTreeNode;
			internal int feedSubIndex;

			internal HiddenNode(TreeNode lNode)
			{
				treeNode = lNode;
				parentTreeNode = lNode.Parent;
				FeedSubscription fs = (FeedSubscription) lNode.Tag;
				feedSubIndex = fs.Index;
			}
		}
	}
}