using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BetterReader.Backend
{
    public delegate void FeedSubscriptionReadDelegate(FeedSubscription fs);
   public class FeedSubscription : FeedSubTreeNodeBase
    {
		private string feedUrl;
		private string displayName;
		private FeedFolder parentFolder;
		private int updateSeconds;
		private Feed feed;
	   private FeedSubscriptionReadDelegate callback;

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

	   public void BeginReadFeed(FeedSubscriptionReadDelegate lCallback)
	   {
		   callback = lCallback;
           feed.BeginRead(new FeedReadCompleteDelegate(feedReadCallback));
	   }

       public void feedReadCallback(Feed f)
       {
		   callback(this);
       }

	   public new static FeedSubscription GetFromOpmlXmlNode(XmlNode node)
	   {
		   FeedSubscription fs = new FeedSubscription();
		   fs.DisplayName = node.Attributes["text"].Value;
		   fs.FeedUrl = node.Attributes["xmlUrl"].Value;
		   return fs;
	   }

       public override string ToString()
       {
           return displayName + "(" + feed.FeedItems.Count.ToString() + ")";
       }

    }
}
