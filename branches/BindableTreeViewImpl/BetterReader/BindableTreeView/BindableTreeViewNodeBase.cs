using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BindableTreeView
{
	public abstract class BindableTreeViewNodeBase : IBindableTreeViewNode
	{
		private List<IBindableTreeViewNode> childNodes;
		private Font treeViewFont;

		#region IBindableTreeViewNode Members

		public List<IBindableTreeViewNode> ChildNodes
		{
			get
			{
				return childNodes;
			}
			set
			{
				childNodes = value;
			}
		}

		public virtual string TreeViewDisplayString
		{
			get 
			{
				return this.ToString();
			}
		}

		public virtual int TreeViewImageIndex
		{
			get
			{
				return 0;
			}
		}

		public virtual int TreeViewSelectedImageIndex
		{
			get
			{
				return 0;
			}
		}

		public virtual Font TreeViewFont
		{
			get
			{
				return treeViewFont;
			}
			set
			{
				treeViewFont = value;
			}
		}

		#endregion
	}
}
