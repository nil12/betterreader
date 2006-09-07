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

		public Feed(string lFeedUrl)
		{
			feedUrl = lFeedUrl;
			feedItems = null;
		}

		public void BeginRead(FeedReadCompleteDelegate callback)
		{
			HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(feedUrl);

			webRequestState state = new webRequestState();
			state.callback = callback;
			state.request = hwr;
			IAsyncResult ar = hwr.BeginGetResponse(new AsyncCallback(readCallback), state);
			// this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
			ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle, new WaitOrTimerCallback(timeoutCallback), callback, TimeSpan.FromSeconds(30), true);



		}

		private void readCallback(IAsyncResult ar)
		{
			webRequestState state = (webRequestState)ar.AsyncState;
			HttpWebResponse response = (HttpWebResponse)state.request.GetResponse();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(response.GetResponseStream);
		}

		private void timeoutCallback(object state, bool timedOut)
		{
		}

		private class webRequestState
		{
			public FeedReadCompleteDelegate callback;
			public HttpWebRequest request;
		}


	}
}
