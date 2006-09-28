using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace BetterReader.Backend
{
	public class FeedItemCollection : IEnumerable<FeedItem>
	{
		private List<FeedItem> items;
		private int unreadCount;
		private Dictionary<string, FeedItem> feedItemsByGuid;
		private FeedItemProperties includedFeedItemProperties;
		private Feed parentFeed;
		private string archiveFilepath;

		public FeedItemProperties IncludedFeedItemProperties
		{
			get { return includedFeedItemProperties; }
			set { includedFeedItemProperties = value; }
		}

		public int UnreadCount
		{
			get { return unreadCount; }
		}

		public int Count
		{
			get
			{
				return items.Count;
			}
		}

		public FeedItemCollection(Feed lParentFeed, string lArchiveFilepath)
		{
			parentFeed = lParentFeed;
			archiveFilepath = lArchiveFilepath;
			items = new List<FeedItem>();
			feedItemsByGuid = new Dictionary<string, FeedItem>();
		}

		public void Clear()
		{
			items.Clear();
		}

		public void LoadArchivedItems()
		{
			try
			{
				if (File.Exists(archiveFilepath))
				{
					using (TextReader tr = new StreamReader(archiveFilepath))
					{
						XmlSerializer xs = new XmlSerializer(typeof(List<FeedItem>));
						items = (List<FeedItem>)xs.Deserialize(tr);
					}
				}
			}
			catch
			{
				//error reading archive, no big deal
				return;
			}

			foreach (FeedItem fi in items)
			{
				if (feedItemsByGuid.ContainsKey(fi.Guid) == false)
				{
					feedItemsByGuid.Add(fi.Guid, fi);
					includedFeedItemProperties |= fi.IncludedProperties;
					if (fi.HasBeenRead == false)
					{
						unreadCount++;
					}
				}


				fi.ParentFeed = parentFeed;
			}
		}

		//private void purgeOldArchivedItems()
		//{
		//    List<FeedItem> newFeedItems = new List<FeedItem>();
		//    if (items != null && items.Count > 0)
		//    {
		//        foreach (FeedItem fi in items)
		//        {
		//            TimeSpan age = ((TimeSpan)(DateTime.Now - fi.DownloadDate));
		//            if ((int)age.TotalDays < parentFeed.ParentSubscription.DaysToArchive)
		//            {
		//                newFeedItems.Add(fi);
		//            }
		//        }
		//    }

		//    items = newFeedItems;
		//}


		internal void Remove(FeedItem fi)
		{
			if (fi.HasBeenRead == false)
			{
				unreadCount--;
			}

			items.Remove(fi);
		}

		internal void Add(FeedItem fi)
		{
			items.Add(fi);
			if (fi.HasBeenRead == false)
			{
				unreadCount++;
			}
		}

		internal void MarkAllItemsRead()
		{
			foreach (FeedItem fi in items)
			{
				fi.HasBeenRead = true;
			}

			unreadCount = 0;
			ArchiveItems();
		}

		#region IEnumerable<FeedItem> Members

		public IEnumerator<FeedItem> GetEnumerator()
		{
			return items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		#endregion

		internal void AddOrUpdate(FeedItem fi)
		{
			if (feedItemsByGuid.ContainsKey(fi.Guid))
			{
				FeedItem oldFI = feedItemsByGuid[fi.Guid];
				fi.HasBeenRead = oldFI.HasBeenRead;
				items.Remove(oldFI);
				feedItemsByGuid.Remove(fi.Guid);
			}
			else
			{
				if (fi.HasBeenRead == false)
				{
					unreadCount++;
				}
			}

			feedItemsByGuid.Add(fi.Guid, fi);
			fi.ParentFeed = parentFeed;
			items.Add(fi);
			includedFeedItemProperties |= fi.IncludedProperties;
		}

		internal void ArchiveItems()
		{
			using (TextWriter tw = new StreamWriter(archiveFilepath))
			{
				XmlSerializer xs = new XmlSerializer(typeof(List<FeedItem>));
				xs.Serialize(tw, items);
			}
		}


		internal void PurgeOldItems()
		{
			unreadCount = 0;
			List<FeedItem> newFeedItems = new List<FeedItem>();
			if (items != null && items.Count > 0)
			{
				SortByAge();
				int itemsLeft = items.Count;
				//walk through the items backwards saving ones we'll keep in newFeedItems
				for (int i = items.Count - 1; i > -1; i--)
				{
					FeedItem fi = items[i];
					if (itemsLeft > parentFeed.ParentSubscription.MaxItems && fi.HasBeenRead)
					{
						//we've got more items than allowed and this one is unread so get rid of it
						itemsLeft--;
						continue;
					}
					
					TimeSpan age = ((TimeSpan)(DateTime.Now - fi.DownloadDate));
					if ((int)age.TotalDays < parentFeed.ParentSubscription.DaysToArchive)
					{
						newFeedItems.Add(fi);
						if (fi.HasBeenRead == false)
						{
							unreadCount++;
						}
					}
				}
			}

			items = newFeedItems;
		}

		public void SortByAge()
		{
			items.Sort(new AgeComparer());
		}

		private class AgeComparer : IComparer<FeedItem>
		{
			#region IComparer<FeedItem> Members

			/// <summary>
			/// This compares two FeedItems by their age.  Age is determined by first comparing pubDates.  If either 
			/// pubDate is null then that item is assumed to be older.  If neither is null they are compared as DateTimes.
			/// If both are null then downloadDates are compared with the same logic.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(FeedItem x, FeedItem y)
			{
				int retVal;
				DateTime? pubDateX, pubDateY, downDateX, downDateY;

				pubDateX = x.PubDate;
				pubDateY = y.PubDate;
				downDateX = x.DownloadDate;
				downDateY = y.DownloadDate;


				if (pubDateY == null && pubDateX == null)
				{
					//both pubDates are null so compare downDates
					retVal = compareDateTimes(downDateX, downDateY);
				}
				else
				{
					//at least one pubDate is not null so compare them
					retVal = compareDateTimes(pubDateX, pubDateY);
				}

				return retVal;

			}

			#endregion

			private int compareDateTimes(DateTime? x, DateTime? y)
			{
				int retVal;

				if (x == null || y == null)
				{
					if (x != null)
					{
						//item y's pubDate is null but item x's is not so return item y
						retVal = 1;
					}
					else if (y != null)
					{
						//item x's pubDate is null but item y's is not so return item x
						retVal = -1;
					}
					else
					{
						//both are null so return equal
						retVal = 0;
					}
				}
				else
				{
					retVal = DateTime.Compare((DateTime)x, (DateTime)y);
				}

				return retVal;
			}
		}


		internal void MarkItemRead(FeedItem feedItem)
		{
			feedItem.HasBeenRead = true;
			unreadCount--;
		}
	}
}
