using System;
//using System.Collections.Generic;
//using System.Text;
using System.Xml;
using System.Threading;

namespace BetterReader.Backend
{
	public delegate void FeedSubscriptionReadDelegate(FeedSubscription fs);

	public class FeedSubscription : FeedSubTreeNodeBase, IDisposable
	{
		private string feedUrl;
		private string displayName;
		private int updateSeconds;
		private Feed feed;
		private FeedSubscriptionReadDelegate callback;
		private Timer updateTimer;
		private Guid guid;
		private int daysToArchive;
		private int maxItems;
		private FeedItemsListViewColumnSorter columnSorter;
		private FeedItemClickAction feedItemClickAction;

		public FeedItemClickAction FeedItemClickAction
		{
			get { return feedItemClickAction; }
			set { feedItemClickAction = value; }
		}


		public int MaxItems
		{
			get { return maxItems; }
			set { maxItems = value; }
		}

		public FeedItemsListViewColumnSorter ColumnSorter
		{
			get { return columnSorter; }
			set { columnSorter = value; }
		}

		public int DaysToArchive
		{
			get { return daysToArchive; }
			set { daysToArchive = value; }
		}

		public Guid Guid
		{
			get { return guid; }
			set { guid = value; }
		}

		public string FeedUrl
		{
			get { return feedUrl; }
			set
			{
				feedUrl = value;
				if (feed == null || feed.FeedUrl != feedUrl)
				{
					feed = new Feed(guid, feedUrl);
					feed.ParentSubscription = this;
				}
			}
		}

		public string DisplayName
		{
			get { return displayName; }
			set { displayName = value; }
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

		public FeedSubscription()
		{
			guid = Guid.NewGuid();
			daysToArchive = 2;
			updateSeconds = 15*60;
			maxItems = 100;
			columnSorter = new FeedItemsListViewColumnSorter();
			feedItemClickAction = FeedItemClickAction.Default;
		}

		public void BeginReadFeed(FeedSubscriptionReadDelegate lCallback)
		{
			if (updateTimer != null)
			{
				updateTimer.Dispose();
			}
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

			updateTimer = new Timer(new TimerCallback(timerCallback), null, updateSeconds*1000,
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
			return displayName + "(" + feed.UnreadCount.ToString() + "/" + feed.FeedItems.Count.ToString() + ")";
		}

		public void MarkAllItemsRead()
		{
			feed.MarkAllItemsRead();
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

		internal void Unsubscribe()
		{
			feed.FeedItems.DeleteArchivedItems();
			if (ParentFolder == null)
			{
				ParentFeedSubTree.RootLevelNodes.Remove(this);
			}
			else
			{
				ParentFolder.ChildNodes.Remove(this);
			}
		}
	}
}
