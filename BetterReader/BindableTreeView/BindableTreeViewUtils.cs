using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BindableTreeView
{
	public abstract class BindableTreeViewUtils
	{
		public static TreeNodeIndex BindRootNodeToTreeView(IBindableTreeViewNode rootNode, TreeView tv)
		{
			tv.Nodes.Clear();

			TreeNodeIndex nodesByTag = new TreeNodeIndex();
			bindNodeAndChildrenToTreeNodeCollection(rootNode, tv.Nodes, nodesByTag);

			return nodesByTag;

		}

		private static void bindNodeAndChildrenToTreeNodeCollection(IBindableTreeViewNode node, TreeNodeCollection treeNodeCollection, TreeNodeIndex nodesByTag)
		{
			TreeNode newNode = treeNodeCollection.Add(node.TreeViewDisplayString);
			newNode.Tag = node;
			newNode.ImageIndex = node.TreeViewImageIndex;
			newNode.SelectedImageIndex = node.TreeViewSelectedImageIndex;

			if (node.TreeViewFont != null)
			{
				newNode.NodeFont = node.TreeViewFont;
			}

			nodesByTag.Add(node, newNode);

			foreach (IBindableTreeViewNode childNode in node.ChildNodes)
			{
				bindNodeAndChildrenToTreeNodeCollection(childNode, newNode.Nodes, nodesByTag);
			}

		}
	}
}
