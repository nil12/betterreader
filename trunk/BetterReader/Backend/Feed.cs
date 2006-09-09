using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Threading;

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



		public Feed(string lFeedUrl)
		{
			feedUrl = lFeedUrl;
			feedItems = new List<FeedItem>();
		}

		public void BeginRead(FeedReadCompleteDelegate callback)
		{
			readSuccess = false;
			readException = null;
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(feedUrl);

			webRequestState state = new webRequestState();
			state.callback = callback;
			state.request = hwr;
			IAsyncResult ar = hwr.BeginGetResponse(new AsyncCallback(readCallback), state);
			// this line implements the timeout, if there is a timeout, the 
			//callback fires and the request becomes aborted
			ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle, 
				new WaitOrTimerCallback(timeoutCallback), state, TimeSpan.FromSeconds(30), true);

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


			state.callback(this);
		}

		private void loadFromXmlDoc(XmlDocument xmlDoc)
		{
			feedItems = new List<FeedItem>();
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
						FeedItem fi = FeedItem.GetFromRssItemNode(childNode);
						feedItems.Add(fi);
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

		private class webRequestState
		{
			public FeedReadCompleteDelegate callback;
			public HttpWebRequest request;
		}


	}
}
