using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BetterReader.Backend
{
    public class FeedFolder : FeedSubTreeNodeBase
    {
        private string name;
        private List<FeedSubTreeNodeBase> childNodes;
        private FeedFolder parentFolder;

        internal FeedFolder ParentFolder
        {
            get { return parentFolder; }
            set { parentFolder = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<FeedSubTreeNodeBase> ChildNodes
        {
            get { return childNodes; }
            set { childNodes = value; }
        }


        public FeedFolder()
        {
			childNodes = new List<FeedSubTreeNodeBase>();
        }

		public new static FeedFolder GetFromOpmlXmlNode(XmlNode node)
		{
			FeedFolder ff = new FeedFolder();
			ff.Name = node.Attributes["text"].Value;
			return ff;
		}
    }
}
