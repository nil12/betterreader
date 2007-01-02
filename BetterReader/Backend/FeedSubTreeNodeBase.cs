using System;
//using System.Collections.Generic;
//using System.Text;
using System.Xml;

namespace BetterReader.Backend
{
	public abstract class FeedSubTreeNodeBase
	{

		private FeedFolder parentFolder;
		private FeedSubscriptionTree parentFeedSubTree;

		internal FeedSubscriptionTree ParentFeedSubTree
		{
			get { return parentFeedSubTree; }
			set { parentFeedSubTree = value; }
		}

		internal FeedFolder ParentFolder
		{
			get { return parentFolder; }
			set { parentFolder = value; }
		}

		public int Index
		{
			get
			{
				if (parentFolder != null)
				{
					return parentFolder.ChildNodes.IndexOf(this);
				}
				else
				{
					return parentFeedSubTree.RootLevelNodes.IndexOf(this);
				}
			}
		}


		public static FeedSubTreeNodeBase GetFromOpmlXmlNode(XmlNode node)
		{
			if (node.Name != "outline")
			{
				throw new Exception("FeedSubTreeNodeBase.GetFromXmlNode: node passed is not a 'container' node.");
			}

			XmlAttribute xa = node.Attributes["title"];

			if (xa != null)
			{
				//node is a feed sub
				return FeedSubscription.GetFromOpmlXmlNode(node);
			}
			else
			{
				//node is a feed folder
				return FeedFolder.GetFromOpmlXmlNode(node);
			}
		}
	}
}
