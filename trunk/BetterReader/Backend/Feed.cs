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
		private FeedItemCollection feedItems;
		private bool readSuccess;
		private Exception readException;
		private Dictionary<string, string> unsupportedFeedProperties;
		//private int unreadItems;
		private FeedSubscription parentSubscription;
		private Guid guid;
		private string archiveFilepath;
		private int itemCountBeforeRead;
		private bool hasNewItemsFromLastRead;
		private DateTime lastDownloadAttempt;
		
		#region properties

		public DateTime LastDownloadAttempt
		{
			get { return lastDownloadAttempt; }
			set { lastDownloadAttempt = value; }
		}

		internal FeedItemProperties IncludedFeedItemProperties
		{
			get { return feedItems.IncludedFeedItemProperties; }
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


		internal int UnreadCount
		{
			get { return feedItems.UnreadCount; }
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

		public FeedItemCollection FeedItems
		{
			get
			{
				return feedItems;
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
			//unreadItems = 0;
			archiveFilepath = MainForm.ArchiveDirectory + guid.ToString() + ".xml";
			feedItems = new FeedItemCollection(this, archiveFilepath);
			hasNewItemsFromLastRead = false;
			itemCountBeforeRead = 0;
		}


		private void loadArchivedFeedItems()
		{
			feedItems.LoadArchivedItems();
		}


		public void BeginRead(FeedReadCompleteDelegate callback)
		{
			readSuccess = false;
			readException = null;
			feedItems.LoadArchivedItems();
			itemCountBeforeRead = feedItems.Count;
			HttpWebRequest hwr = null;
			try
			{
				hwr = (HttpWebRequest)WebRequest.Create(feedUrl);
			}
			catch (UriFormatException)
			{
				//this feed has an improperly formatted URL
				this.readSuccess = false;
				this.readException = new Exception("Error.  The Feed URL: " + this.feedUrl + " is invalid.");
				callback(this);
				return;
			}

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
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)state.request.GetResponse();
				xmlDoc.Load(response.GetResponseStream());
				loadFromXmlDoc(xmlDoc);
				readException = null;
				readSuccess = true;
			}
			catch (Exception e)
			{
				readException = e;
				readSuccess = false;
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}

			feedItems.PurgeOldItems();
			feedItems.ArchiveItems();

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
			feedItems.AddOrUpdate(fi);
		}

		private void loadFromRdfNode(XmlNode node)
		{
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
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
			feedItems.MarkAllItemsRead();
		}

		private class webRequestState
		{
			public FeedReadCompleteDelegate callback;
			public HttpWebRequest request;
		}


	}
}
