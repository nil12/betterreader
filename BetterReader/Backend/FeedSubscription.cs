using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BetterReader.Backend
{
   public class FeedSubscription : FeedSubTreeNodeBase
    {
		private string feedUrl;
		private string displayName;
		private FeedFolder parentFolder;
		private int updateSeconds;
		private Feed feed;

		public string FeedUrl
		{
			get 
			{ 
				return feedUrl; 
			}
			set 
			{
				feedUrl = value;
				feed = new Feed(feedUrl);
			}
		}

		public string DisplayName
		{
			get { return displayName; }
			set { displayName = value; }
		}

		internal FeedFolder ParentFolder
		{
			get { return parentFolder; }
			set { parentFolder = value; }
		}

		public int UpdateSeconds
		{
			get { return updateSeconds; }
			set { updateSeconds = value; }
		}


		internal Feed Feed
		{
			get { return feed; }
			set { feed = value; }
		}

	   public void ReadFeed()
	   {
		   //feed.Read();
	   }

	   public new static FeedSubscription GetFromOpmlXmlNode(XmlNode node)
	   {
		   FeedSubscription fs = new FeedSubscription();
		   fs.DisplayName = node.Attributes["text"].Value;
		   fs.FeedUrl = node.Attributes["xmlUrl"].Value;
		   return fs;
	   }

    }
}
