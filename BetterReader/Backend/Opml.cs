using System;
//using System.Collections.Generic;
//using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;


namespace BetterReader.Backend
{
	abstract class Opml
	{
		public static FeedSubscriptionTree GetFeedSubscriptionTreeFromOpmlFile(string filepath)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Auto;
			settings.CloseInput = true;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			settings.ValidationType = ValidationType.None;

			XmlDocument xmlDoc;
			using (TextReader tr = new StreamReader(filepath))
			{
				XmlReader xr = XmlReader.Create(tr, settings);

				xmlDoc = new XmlDocument();
				xmlDoc.Load(xr);
			}

			FeedSubscriptionTree fst = new FeedSubscriptionTree();

			foreach (XmlNode node in xmlDoc.ChildNodes)
			{
				processOpmlNode(fst, node, null);
			}

			return fst;
		
		}

		public static void ExportAsOpml(FeedSubscriptionTree fst, string filepath)
		{
			XElement root = new XElement("opml",
				new XElement("head"));

			XElement body = new XElement("body");
			root.Add(body);

			addNodesToParentNode(fst.RootLevelNodes, body);

			root.Save(filepath);
		}

		private static void addNodesToParentNode(List<FeedSubTreeNodeBase> list, XElement parent)
		{
			XElement xe = null;
			foreach (FeedSubTreeNodeBase fstnb in list)
			{
				Type t = fstnb.GetType();
				if (t == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					xe = new XElement("outline", new XAttribute("text", ff.Name));
					parent.Add(xe);
					addNodesToParentNode(ff.ChildNodes, xe);
					continue;
				}
				else if (t == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					xe = new XElement("outline",
						new XAttribute("title", fs.DisplayName),
						new XAttribute("xmlUrl", fs.FeedUrl));
					parent.Add(xe);
				}
				else
				{
					throw new ApplicationException(string.Format("Error.  Expected type of either FeedFolder or FeedSubscription but got type {0}", t));
				}

			}
		}

		

		private static void processOpmlNode(FeedSubscriptionTree fst, XmlNode node, FeedFolder currentParentFolder)
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
						processOpmlNode(fst, childNode, currentParentFolder);
					}
					break;
				case "outline":
					processOpmlOutlineNode(fst, node, currentParentFolder);
					break;
				case "head":
					return;
			}
		}

		private static void processOpmlOutlineNode(FeedSubscriptionTree fst, XmlNode node, FeedFolder currentParentFolder)
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
					fst.RootLevelNodes.Add(fs);
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
					fst.RootLevelNodes.Add(ff);
				}
				else
				{
					currentParentFolder.ChildNodes.Add(ff);
				}


				foreach (XmlNode childNode in node.ChildNodes)
				{
					processOpmlNode(fst, childNode, ff);
				}
			}
			else
			{
				throw new Exception("FeedSubscriptionTree.processOpmlOutlineNode error: Unrecognized node type: " + nodeType.ToString());
			}

		}
	}
}
