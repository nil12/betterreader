using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;

namespace BetterReader.Backend
{
    public delegate void FeedSubscriptionReadDelegate(FeedSubscription fs);
	public class FeedSubscription : FeedSubTreeNodeBase, IDisposable
    {
		private string feedUrl;
		private string displayName;
		private FeedFolder parentFolder;
		private int updateSeconds;
		private Feed feed;
		private FeedSubscriptionReadDelegate callback;
		private Timer updateTimer;

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
				feed.ParentSubscription = this;
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

		private void feedReadCallback(Feed f)
		{
			startUpdateTimer();
			callback(this);
		}

		public void ResetUpdateTimer()
		{
			startUpdateTimer();
		}

		private void startUpdateTimer()
		{
			if (updateTimer != null)
			{
				updateTimer.Dispose();
			}

			updateTimer = new Timer(new TimerCallback(timerCallback), null, updateSeconds * 1000, 
				Timeout.Infinite);

		}

		private void timerCallback(object state)
		{
			BeginReadFeed(callback);
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
		   return displayName + "(" + feed.UnreadItems.ToString() + "/" + feed.FeedItems.Count.ToString() + ")";
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (updateTimer != null)
			{
				updateTimer.Dispose();
			}
		}

		#endregion
	}
}
