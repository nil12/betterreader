using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace BindableTreeView
{
	public abstract class BindableTreeViewUtils
	{
		/// <summary>
		/// Clears out the TreeView control and then binds to the passed IBindableTreeViewNode and all of its children.
		/// </summary>
		/// <param name="rootNode">The IBindableTreeViewNode to start with.</param>
		/// <param name="tv">The TreeView to bind.</param>
		/// <returns>And index of TreeNodes by Tag where the Tag is the object that was bound to that node.</returns>
		public static TreeNodeIndex BindRootNodeToTreeView(IBindableTreeViewNode rootNode, TreeView tv)
		{
			tv.Nodes.Clear();

			TreeNodeIndex nodesByTag = new TreeNodeIndex();
			bindNodeAndChildrenToTreeNodeCollection(rootNode, tv.Nodes, nodesByTag);

			return nodesByTag;

		}

		/// <summary>
		/// Binds a list of IBindableTreeViewNodes to the root level Nodes collection of the specified TreeView control.
		/// If the existingIndex argument is null then a new TreeNodeIndex object will be created.  If not the TreeNodeIndex 
		/// passed in will be appended to.
		/// </summary>
		/// <param name="nodes">The list of IBindableTreeViewNodes to bind to.</param>
		/// <param name="tv">The TreeView to bind to.</param>
		/// <param name="existingIndex">The index of nodes already in the TreeView.  If null a new index will be created.</param>
		/// <returns>An index of TreeNodes by Tag where the Tag is the object that was bound to that node.</returns>
		public static TreeNodeIndex BindNodeListToTreeView<T>(List<T> nodes, TreeView tv, TreeNodeIndex existingIndex)
			where T : IBindableTreeViewNode
		{
			if (existingIndex == null)
			{
				existingIndex = new TreeNodeIndex();
			}

			foreach (IBindableTreeViewNode node in nodes)
			{
				bindNodeAndChildrenToTreeNodeCollection(node, tv.Nodes, existingIndex);
			}

			return existingIndex;
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
