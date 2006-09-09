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
    public class FeedSubscriptionTree
    {
		private List<FeedSubTreeNodeBase> rootLevelNodes;
		public string x = "xxx";

		public List<FeedSubTreeNodeBase> RootLevelNodes
		{
			get { return rootLevelNodes; }
			set { rootLevelNodes = value; }
		}
		//private string filepath;
		//private FeedFolder rootFolder;

		//public FeedFolder RootFolder
		//{
		//    get { return rootFolder; }
		//    set { rootFolder = value; }
		//}

		//public string Filepath
		//{
		//    get { return filepath; }
		//    set { filepath = value; }
		//}


		public void LoadFromOpml(string filepath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationType = ValidationType.None;

            XmlDocument xmlDoc = null;
            using (TextReader tr = new StreamReader(filepath))
            {
                XmlReader xr = XmlReader.Create(tr, settings);

                xmlDoc = new XmlDocument();
                xmlDoc.Load(xr);
            }

			rootLevelNodes = new List<FeedSubTreeNodeBase>();

            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                processOpmlNode(node, null);
            }
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

        public static FeedSubscriptionTree GetFromFeedSubscriptionsFile(string filepath)
        {
            FeedSubscriptionTree fsc = null;
            using (TextReader tr = new StreamReader(filepath))
            {
				XmlSerializer xs = new XmlSerializer(typeof(FeedSubscriptionTree));
                fsc = xs.Deserialize(tr) as FeedSubscriptionTree;
            }

            return fsc;
        }


        public void ExportAsOpml(string filepath)
        {
            throw new Exception("Error.  ExportAsOpml not implemented yet.");
        }

        private void processOpmlNode(XmlNode node, FeedFolder currentParentFolder)
        {
            if (node.Name == null && node.Name.Length < 1)
            {
                return;
            }

            switch (node.Name.ToLower())
            {
                case "body":
                case "opml":
                case "xml":
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        processOpmlNode(childNode, currentParentFolder);
                    }
                    break;
                case "outline":
                    processOpmlOutlineNode(node, currentParentFolder);
                    break;
                case "head":
                    return;
            }
        }

        private void processOpmlOutlineNode(XmlNode node, FeedFolder currentParentFolder)
        {
			FeedSubTreeNodeBase fstnb = FeedSubTreeNodeBase.GetFromOpmlXmlNode(node);

			Type nodeType = fstnb.GetType();

			if (nodeType == typeof(FeedSubscription))
			{
				FeedSubscription fs = fstnb as FeedSubscription;
				fs.ParentFolder = currentParentFolder;
				if (currentParentFolder == null)
				{
					//this feed sub is at the root level of the tree
					this.rootLevelNodes.Add(fs);
				}
				else
				{
					currentParentFolder.ChildNodes.Add(fs);
				}
				//Debug.WriteLine("Processed Sub Node: " + fs.DisplayName + ":" + currentParentFolder == null ? "root" : currentParentFolder.Name);
			}
			else if (nodeType == typeof(FeedFolder))
			{
				FeedFolder ff = fstnb as FeedFolder;
				ff.ParentFolder = currentParentFolder;
				if (currentParentFolder == null)
				{
					//this feed folder is at the root level of the tree
					this.rootLevelNodes.Add(ff);
				}
				else
				{
					currentParentFolder.ChildNodes.Add(ff);
				}


				foreach (XmlNode childNode in node.ChildNodes)
				{
					processOpmlNode(childNode, ff);
				}
			}
			else
			{
				throw new Exception("FeedSubscriptionTree.processOpmlOutlineNode error: Unrecognized node type: " + nodeType.ToString());
			}
		
        }

		public void BeginReadAllFeeds(FeedSubscriptionReadDelegate callback)
		{
			readAllFeedsInNodeList(rootLevelNodes, callback);
		}

		private void readAllFeedsInNodeList(List<FeedSubTreeNodeBase> nodeList, FeedSubscriptionReadDelegate callback)
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
					readAllFeedsInNodeList(ff.ChildNodes, callback);
				}
				else
				{
					throw new Exception("FeedSubscriptionTree.ReadAllFeeds error: unsupported node type: " + t.ToString());
				}
			}
		}

    }
}
