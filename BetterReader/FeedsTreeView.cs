using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using BetterReader.Backend;

namespace BetterReader
{
	class FeedsTreeView : Sloppycode.UI.TreeViewDragDrop
	{

		private Dictionary<object, HiddenNode> hiddenNodes = new Dictionary<object, HiddenNode>();
		private delegate void HideNodeDelegate(TreeNode node);
		private delegate void InsertNodeDelegate(int index, TreeNode node);

		protected override void OnDragDrop(DragEventArgs e)
		{
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
					this.SelectedNode = node.NextVisibleNode;
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

			HiddenNode node = hiddenNodes[tag];
			hiddenNodes.Remove(tag);

			int insertAt = node.index;

			if (node.index > node.parent.Nodes.Count - 1 )
			{
				//the actual index of this node is greater than the number of nodes currently displayed
				//so put this node at the end of the list
				insertAt = node.parent.Nodes.Count;
			}

			if (this.InvokeRequired)
			{
				this.Invoke(new InsertNodeDelegate(node.parent.Nodes.Insert), new object[] { node.index, node.node });
			}
			else
			{
				node.parent.Nodes.Insert(node.index, node.node);
			}
		}

		private class HiddenNode
		{
			internal TreeNode node;
			internal TreeNode parent;
			internal int index;

			internal HiddenNode(TreeNode lNode)
			{
				node = lNode;
				parent = lNode.Parent;
				FeedSubscription fs = (FeedSubscription)lNode.Tag;
				index = fs.Index;
			}
		}
	}
}
