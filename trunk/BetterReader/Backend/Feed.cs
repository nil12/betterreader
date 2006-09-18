using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Threading;
using System.Xml.Serialization;
using System.IO;
using System.Web;

namespace BetterReader.Backend
{

	public delegate void FeedReadCompleteDelegate(Feed f);

	public class Feed
	{
		private string feedUrl;
		private string linkUrl;
		private string title;
		private string description;
		private string language;
		private string copyright;
		private string managingEditor;
		private string webMaster;
		private List<FeedItem> feedItems;
		private bool readSuccess;
		private Exception readException;
		private Dictionary<string, string> unsupportedFeedProperties;
		private int unreadItems;
		private FeedSubscription parentSubscription;
		private Dictionary<string, FeedItem> feedItemsByGuid;
		private Guid guid;
		private string archiveFilepath;
		private int itemCountBeforeRead;
		private bool hasNewItemsFromLastRead;
		private FeedItemProperties includedFeedItemProperties;
		private DateTime lastDownloadAttempt;






		#region properties

		public DateTime LastDownloadAttempt
		{
			get { return lastDownloadAttempt; }
			set { lastDownloadAttempt = value; }
		}

		internal FeedItemProperties IncludedFeedItemProperties
		{
			get { return includedFeedItemProperties; }
			set { includedFeedItemProperties = value; }
		}

		public bool HasNewItemsFromLastRead
		{
			get { return hasNewItemsFromLastRead; }
			set { hasNewItemsFromLastRead = value; }
		}

		internal FeedSubscription ParentSubscription
		{
			get { return parentSubscription; }
			set { parentSubscription = value; }
		}


		internal int UnreadItems
		{
			get { return unreadItems; }
			set { unreadItems = value; }
		}



		public string FeedUrl
		{
			get
			{
				return feedUrl;
			}
			set
			{
				feedUrl = value;
			}
		}

		public string LinkUrl
		{
			get
			{
				return linkUrl;
			}
			set
			{
				linkUrl = value;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public string Language
		{
			get
			{
				return language;
			}
			set
			{
				language = value;
			}
		}

		public string Copyright
		{
			get
			{
				return copyright;
			}
			set
			{
				copyright = value;
			}
		}

		public string ManagingEditor
		{
			get
			{
				return managingEditor;
			}
			set
			{
				managingEditor = value;
			}
		}

		public string WebMaster
		{
			get
			{
				return webMaster;
			}
			set
			{
				webMaster = value;
			}
		}

		public List<FeedItem> FeedItems
		{
			get
			{
				return feedItems;
			}
			set
			{
				feedItems = value;
			}
		}


		public Exception ReadException
		{
			get { return readException; }
			set { readException = value; }
		}


		public bool ReadSuccess
		{
			get { return readSuccess; }
			set { readSuccess = value; }
		}

		#endregion

		public Feed(Guid lGuid, string lFeedUrl)
		{
			guid = lGuid;
			feedUrl = lFeedUrl;
			feedItems = new List<FeedItem>();
			feedItemsByGuid = new Dictionary<string, FeedItem>();
			unreadItems = 0;
			archiveFilepath = MainForm.ArchiveDirectory + guid.ToString() + ".xml";
			hasNewItemsFromLastRead = false;
			itemCountBeforeRead = 0;
		}


		private void loadArchivedFeedItems()
		{
			try
			{
				if (File.Exists(archiveFilepath))
				{
					using (TextReader tr = new StreamReader(archiveFilepath))
					{
						XmlSerializer xs = new XmlSerializer(typeof(List<FeedItem>));
						feedItems = (List<FeedItem>)xs.Deserialize(tr);
					}
				}
			}
			catch
			{
				//error reading archive, no big deal
				return;
			}

			foreach (FeedItem fi in feedItems)
			{
				if (feedItemsByGuid.ContainsKey(fi.Guid) == false)
				{
					feedItemsByGuid.Add(fi.Guid, fi);
					includedFeedItemProperties |= fi.IncludedProperties;
					if (fi.HasBeenRead == false)
					{
						unreadItems++;
					}
					fi.ParentFeed = this;
				}
			}
			purgeOldArchivedItems();
		}

		private void purgeOldArchivedItems()
		{
			if (feedItems != null && feedItems.Count > 0)
			{
				foreach (FeedItem fi in feedItems)
				{
					TimeSpan age = ((TimeSpan)(DateTime.Now - fi.DownloadDate));
					if ((int)age.TotalDays > parentSubscription.DaysToArchive)
					{
						feedItems.Remove(fi);
					}
				}
			}
		}

		public void ArchiveFeedItems()
		{
			using (TextWriter tw = new StreamWriter(archiveFilepath))
			{
				XmlSerializer xs = new XmlSerializer(typeof(List<FeedItem>));
				xs.Serialize(tw, feedItems);
			}
		}

		public void BeginRead(FeedReadCompleteDelegate callback)
		{
			readSuccess = false;
			readException = null;
			loadArchivedFeedItems();
			itemCountBeforeRead = feedItems.Count;
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(feedUrl);

			webRequestState state = new webRequestState();
			state.callback = callback;
			state.request = hwr;
			hwr.AllowAutoRedirect = true;
			hwr.MaximumAutomaticRedirections = 10;
			hwr.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.6) Gecko/20060728 Firefox/1.5.0.6";
			IAsyncResult ar = hwr.BeginGetResponse(new AsyncCallback(readCallback), state);
			// this line implements the timeout, if there is a timeout, the 
			//callback fires and the request becomes aborted
			ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle, 
				new WaitOrTimerCallback(timeoutCallback), state, TimeSpan.FromSeconds(30), true);

			lastDownloadAttempt = DateTime.Now;
		}

		private void readCallback(IAsyncResult ar)
		{
			webRequestState state = (webRequestState)ar.AsyncState;
			XmlDocument xmlDoc = new XmlDocument();
			try
			{
				HttpWebResponse response = (HttpWebResponse)state.request.GetResponse();
				xmlDoc.Load(response.GetResponseStream());
				//response.Close();
				loadFromXmlDoc(xmlDoc);
				readException = null;
				readSuccess = true;
			}
			catch (Exception e)
			{
				readException = e;
				readSuccess = false;
				//System.Diagnostics.Debug.WriteLine("Error reading feed: " + e.ToString());
			}

			ArchiveFeedItems();
			if (feedItems.Count > itemCountBeforeRead)
			{
				//new items have been downloaded
				hasNewItemsFromLastRead = true;
			}
			else
			{
				hasNewItemsFromLastRead = false;
			}

			state.callback(this);
		}

		private void loadFromXmlDoc(XmlDocument xmlDoc)
		{
			//feedItems = new List<FeedItem>();
			unsupportedFeedProperties = new Dictionary<string, string>();
			foreach (XmlNode node in xmlDoc)
			{
				switch (node.Name)
				{
					case "rss":
						foreach (XmlNode childNode in node.ChildNodes)
						{
							if (childNode.Name == "channel")
							{
								loadFromRssChannelNode(childNode);
							}
						}
						break;
					case "rdf:RDF":
						loadFromRdfNode(node);
						break;
					case "feed":
						loadFromAtomFeedNode(node);
						break;
				}
			}
		}

		private void loadFromAtomFeedNode(XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				//System.Diagnostics.Debug.WriteLine("node: " + childNode.Name);
				switch (childNode.Name)
				{
					case "title":
						title =  HtmlDecode(innerText);
						break;
					case "link":
						linkUrl = innerText;
						break;
					case "info":
						description = innerText;
						break;
					case "entry":
						FeedItem fi = FeedItem.GetFromAtomEntryNode(childNode);
						addOrUpdateFeedItemsCollection(fi);
						break;
				}
			}
		}

		internal static string HtmlDecode(string innerText)
		{
			return HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(innerText));

		}

		private void addOrUpdateFeedItemsCollection(FeedItem fi)
		{
			if (feedItemsByGuid.ContainsKey(fi.Guid))
			{
				FeedItem oldFI = feedItemsByGuid[fi.Guid];
				fi.HasBeenRead = oldFI.HasBeenRead;
				feedItems.Remove(oldFI);
				feedItemsByGuid.Remove(fi.Guid);
			}
			else
			{
				if (fi.HasBeenRead == false)
				{
					unreadItems++;
				}
			}

			feedItemsByGuid.Add(fi.Guid, fi);
			fi.ParentFeed = this;
			feedItems.Add(fi);
			includedFeedItemProperties |= fi.IncludedProperties;
		}

		private void loadFromRdfNode(XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				//System.Diagnostics.Debug.WriteLine("node: " + childNode.Name);
				switch (childNode.Name)
				{
					case "channel":
						loadFeedInfoFromRdfChannelNode(childNode);
						break;
					case "item":
						FeedItem fi = FeedItem.GetFromRssOrRdfItemNode(childNode);
						addOrUpdateFeedItemsCollection(fi);
						break;
					default:
						if (unsupportedFeedProperties.ContainsKey(childNode.Name))
						{
							unsupportedFeedProperties[childNode.Name] += "|" + innerText;
						}
						else
						{
							unsupportedFeedProperties.Add(childNode.Name, innerText);
						}
						break;
				}
			}
		}

		private void loadFeedInfoFromRdfChannelNode(XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				//System.Diagnostics.Debug.WriteLine("node: " + childNode.Name);
				switch (childNode.Name)
				{
					case "title":
						title = HtmlDecode(innerText);
						break;
					case "link":
						linkUrl = innerText;
						break;
					case "description":
						description = innerText;
						break;
					case "dc:language":
						language = innerText;
						break;
				}
			}
		}

		private void loadFromRssChannelNode(XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				//System.Diagnostics.Debug.WriteLine("node: " + childNode.Name);
				switch (childNode.Name)
				{
					case "title":
						title = innerText;
						break;
					case "link":
						linkUrl = innerText;
						break;
					case "description":
						description = innerText;
						break;
					case "language":
						language = innerText;
						break;
					case "item":
						FeedItem fi = FeedItem.GetFromRssOrRdfItemNode(childNode);
						addOrUpdateFeedItemsCollection(fi);
						break;
					default:
						if (unsupportedFeedProperties.ContainsKey(childNode.Name))
						{
							unsupportedFeedProperties[childNode.Name] += "|" + innerText;
						}
						else
						{
							unsupportedFeedProperties.Add(childNode.Name, innerText);
						}
						break;
				}
			}
		}

		private void timeoutCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				webRequestState wrs = state as webRequestState;
				if (wrs != null)
				{
					try
					{
						HttpWebResponse response = (HttpWebResponse)wrs.request.GetResponse();
						if (response != null)
						{
							response.Close();
						}
					}
					catch { }
				}

				readSuccess = false;
				readException = new Exception("Timeout occurred reading feed.");
				wrs.callback(this);
			}
		}

		public void MarkAllItemsRead()
		{
			foreach (FeedItem fi in feedItems)
			{
				fi.HasBeenRead = true;
			}

			unreadItems = 0;
			ArchiveFeedItems();
		}

		private class webRequestState
		{
			public FeedReadCompleteDelegate callback;
			public HttpWebRequest request;
		}


	}
}
