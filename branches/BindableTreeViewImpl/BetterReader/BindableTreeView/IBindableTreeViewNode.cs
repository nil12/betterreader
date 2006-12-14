using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace BindableTreeView
{
	public interface IBindableTreeViewNode
	{
		List<IBindableTreeViewNode> ChildNodes
		{
			get;
			set;
		}

		string TreeViewDisplayString
		{
			get;
		}

		int TreeViewImageIndex
		{
			get;
		}

		int TreeViewSelectedImageIndex
		{
			get;
		}

		Font TreeViewFont
		{
			get;
			set;
		}
		
	}
}
