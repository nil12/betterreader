using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	public class FeedsTreeView : Sloppycode.UI.TreeViewDragDrop
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

					if (t == typeof(FeedSubscription))
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
			dragNode.Parent.Nodes.Remove(dragNode);
			targetNode.Parent.Nodes.Insert(targetNode.Index, dragNode);
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
			if (this.InvokeRequired)
			{
				this.Invoke(new HideNodeDelegate(node.Parent.Nodes.Remove), new object[] { node });
			}
			else
			{
				if (this.SelectedNode == node)
				{
					this.SelectedNode = null;
				}
				node.Parent.Nodes.Remove(node);
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
			for (int i = 0; i < hiddenNode.parentTreeNode.Nodes.Count; i++)
			{
				TreeNode node = hiddenNode.parentTreeNode.Nodes[i];
				FeedSubTreeNodeBase curTag = (FeedSubTreeNodeBase)node.Tag;

				if (curTag.Index > hiddenNode.feedSubIndex)
				{
					insertAt = i;
					break;
				}
			}

			if (this.InvokeRequired)
			{
				this.Invoke(new InsertNodeDelegate(hiddenNode.parentTreeNode.Nodes.Insert), new object[] { insertAt, hiddenNode.treeNode });
			}
			else
			{
				hiddenNode.parentTreeNode.Nodes.Insert(insertAt, hiddenNode.treeNode);
			}
		}


		internal void HideReadNodes()
		{
			hideNodesInList(this.Nodes);
		}

		private void hideNodesInList(TreeNodeCollection treeNodeCollection)
		{
			List<TreeNode> nodesToHide = new List<TreeNode>();

			foreach (TreeNode node in treeNodeCollection)
			{
				Type t = node.Tag.GetType();

				if (t == typeof(FeedSubscription))
				{
					FeedSubscription fs = (FeedSubscription)node.Tag;
					if (fs.Feed.UnreadCount == 0)
					{
						nodesToHide.Add(node);
					}
				}

				if (t == typeof(FeedFolder))
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
			this.Invalidate();
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
				FeedSubscription fs = (FeedSubscription)lNode.Tag;
				feedSubIndex = fs.Index;
			}
		}

	}
}
