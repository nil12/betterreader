using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace BetterReader.Backend
{
	[Flags]
	public enum FeedItemProperties
	{
		Title = 1,
		Category = 2,
		Author = 4,
		PubDate = 8,
		DownloadDate = 16,
		HasBeenRead = 32,
		All = Title | Category | Author | PubDate | DownloadDate | HasBeenRead
	}

	public class FeedItem
	{
		private string title;
		private string linkUrl;
		private string category;
		private string author;
		private DateTime? pubDate;
		private string guid;
		private string description;
		private Dictionary<string, string> unsupportedFeedItemProperties;
		private string encodedContent;
		private bool hasBeenRead;
		private Feed parentFeed;
		private DateTime? downloadDate;
		private FeedItemProperties includedProperties;

		public FeedItemProperties IncludedProperties
		{
			get { return includedProperties; }
			set { includedProperties = value; }
		}

		public DateTime? DownloadDate
		{
			get { return downloadDate; }
			set { downloadDate = value; }
		}

		internal Feed ParentFeed
		{
			get { return parentFeed; }
			set { parentFeed = value; }
		}

		public bool HasBeenRead
		{
			get { return hasBeenRead; }
			set { hasBeenRead = value; }
		}

		public string EncodedContent
		{
			get { return encodedContent; }
			set { encodedContent = value; }
		}

		internal Dictionary<string, string> UnsupportedFeedItemProperties
		{
			get { return unsupportedFeedItemProperties; }
			set { unsupportedFeedItemProperties = value; }
		}


		public string Title
		{
			get { return title; }
			set { title = value; }
		}

		public string LinkUrl
		{
			get { return linkUrl; }
			set { linkUrl = value; }
		}

		public string Category
		{
			get { return category; }
			set { category = value; }
		}

		public string Author
		{
			get { return author; }
			set { author = value; }
		}

		public DateTime? PubDate
		{
			get { return pubDate; }
			set { pubDate = value; }
		}

		public string Guid
		{
			get { return guid; }
			set { guid = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public FeedItem()
		{
			unsupportedFeedItemProperties = new Dictionary<string, string>();
			author = "";
			category = "";
			description = "";
			downloadDate = null;
			encodedContent = "";
			guid = "";
			hasBeenRead = false;
			linkUrl = "";
			parentFeed = null;
			pubDate = null;
			title = "";
		}

		private void setIncludedProperties()
		{
			if (title.Length > 0)
			{
				includedProperties = includedProperties | FeedItemProperties.Title;
			}

			if (author.Length > 0)
			{
				includedProperties = includedProperties | FeedItemProperties.Author;
			}

			if (category.Length > 0)
			{
				includedProperties = includedProperties | FeedItemProperties.Category;
			}

			if (pubDate != null)
			{
				includedProperties = includedProperties | FeedItemProperties.PubDate;
			}

			includedProperties = includedProperties | FeedItemProperties.HasBeenRead | FeedItemProperties.DownloadDate;
		}

		public static FeedItem GetFromRssOrRdfItemNode(XmlNode node)
		{
			FeedItem fi = new FeedItem();
			fi.hasBeenRead = false;
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				switch (childNode.Name.ToLower())
				{
					case "title":
						fi.title = Feed.HtmlDecode(innerText);
						break;
					case "link":
						fi.linkUrl = innerText;
						break;
					case "category":
						fi.category = Feed.HtmlDecode(innerText);
						break;
					case "author":
						fi.author = Feed.HtmlDecode(innerText);
						break;
					case "pubdate":
					case "dc:date":
						fi.pubDate = fi.safeDateTimeParse(innerText);
						break;
					case "guid":
						//a linefeed in a guid causes problems with uniquely identifying items
						fi.guid = innerText.Replace("\r\n", "");
						break;
					case "description":
						fi.description = innerText;
						break;
					case "content:encoded":
						fi.encodedContent = innerText;
						break;
					default:
						if (fi.unsupportedFeedItemProperties.ContainsKey(childNode.Name))
						{
							fi.unsupportedFeedItemProperties[childNode.Name] += "|" + innerText;
						}
						else
						{
							fi.unsupportedFeedItemProperties.Add(childNode.Name, innerText);
						}
						break;
				}
			}
			fi.SetGuid();
			fi.downloadDate = DateTime.Now;
			fi.setIncludedProperties();
			return fi;
		}

		private void SetGuid()
		{
			if (guid == null || guid.Length < 1)
			{
				//no guid provided by source so calculate our own
				//string allProps = author + linkUrl + pubDate + title;
				string allProps = linkUrl + title;
				//a linefeed in the guid causes uniquess issues
				guid = allProps.Replace("\r\n", "").ToLower().GetHashCode().ToString();
			}
		}

		public static FeedItem GetFromAtomEntryNode(XmlNode node)
		{
			FeedItem fi = new FeedItem();
			fi.hasBeenRead = false;
			foreach (XmlNode childNode in node.ChildNodes)
			{
				string innerText = childNode.InnerText;
				switch (childNode.Name.ToLower())
				{
					case "title":
						fi.title = Feed.HtmlDecode(innerText);
						break;
					case "link":
						if (innerText != null && innerText != string.Empty)
						{
							fi.linkUrl = innerText;
						}
						else
						{
							fi.linkUrl = childNode.Attributes["href"].Value;
						}
						break;
					case "category":
						fi.category = Feed.HtmlDecode(innerText);
						break;
					case "author":
						foreach (XmlNode authorNode in childNode.ChildNodes)
						{
							if (authorNode.Name == "name")
							{
								fi.author = Feed.HtmlDecode(authorNode.InnerText);
							}
						}
						break;
					case "modified":
						fi.pubDate = fi.safeDateTimeParse(innerText);
						break;
					case "id":
						//a linefeed in the guid causes uniqueness problems
						fi.guid = innerText.Replace("\r\n", "");
						break;
					case "content":
						fi.description = innerText;
						break;
					default:
						if (fi.unsupportedFeedItemProperties.ContainsKey(childNode.Name))
						{
							fi.unsupportedFeedItemProperties[childNode.Name] += "|" + innerText;
						}
						else
						{
							fi.unsupportedFeedItemProperties.Add(childNode.Name, innerText);
						}
						break;
				}
			}
			fi.SetGuid();
			fi.downloadDate = DateTime.Now;
			fi.setIncludedProperties();
			return fi;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == this.GetType())
			{
				FeedItem other = (FeedItem) obj;
				return this.guid.Equals(other.guid);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.guid.GetHashCode();
		}

		#region datetime parsing

		private DateTime? safeDateTimeParse(string dt)
		{
			DateTime? retVal = null;

			try
			{
				retVal = DateTime.Parse(dt);
			}
			catch
			{
				//parse failed so we leave it at null
			}

			if (retVal == null)
			{
				try
				{
					retVal = DateTimeExt.Parse(dt);
				}
				catch
				{
					retVal = null;
				}
			}

			if (retVal == null)
			{
				string[] formats = getDateTimeFormats();

				try
				{
					retVal = DateTime.ParseExact(dt, formats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
				}
				catch
				{
					//failed so leave it at null
				}
			}


			return retVal;
		}

		private string[] getDateTimeFormats()
		{
			//'CRAIGSLIST = 2005-03-24T23:37-08:00
			string dfCraigslist = "yyyy-MM-ddTHH:mmzzz";
			//'CRAIGSLIST II = 2005-04-11T11:20:22-05:00
			string dfCraigslistII = "yyyy-MM-ddTHH:mm:ss-zzz";
			////CRAISGLIST III = 2006-09-17T11:29:39-05:00
			//string dfCraigslistIII = "yyyy-MM-ddTHH:mm
			//'BLOGMAVERICK = 2005-03-30T02:18Z
			string dfBlogMaverick = "yyyy-MM-ddTHH:mmZ";
			//'TOPIX.NET = Thu, 07 Apr 2005 11:28 GMT
			string dfTopix = "ddd, dd MMM yyyy HH:mm Z";
			//'NEWS.COM.COM = Wed, 06 Apr, 2005 12:28:00 PDT
			string dfNewsCom = "ddd, dd MMM, yyyy HH:mm:ss z";
			//'NEWS.COM.COM = Thu, 02 Jun, 2005 4:16:00 PDT
			string dfNewsCom2 = "ddd, dd MMM, yyyy h:mm:ss z";
			//'dateFormats = New String() {"r", "s", "u", "yyyy-MM-ddTHH:mmzzz", "yyyy-MM-ddTHH:mm:sszzz", "yyyyMMddTHHmmss", "ddd, dd MMM yyyy HH:mm Z", "ddd, dd MMM, yyyy HH:mm:ss Z"}
			string[] dateFormats = {
			                       	"r", "s", "u", dfCraigslist, dfCraigslistII, dfBlogMaverick, dfTopix, dfNewsCom, dfNewsCom2,
			                       	"ddd, dd MMM yyyy HH:mm:ss z", "ddd, dd MMM yyyy HH:mm:ss zzzz",
			                       	"ddd, dd MMM yyyy HH:mm:ss -zzzz", "ddd, dd MMM yyyy HH:mm:ss zzz",
			                       	"ddd, dd MMM yyyy HH:mm:ss -zzz", "ddd, dd MMM yyyy HH:mm:ss Z"
			                       };

			return dateFormats;
		}

		#endregion

		internal void MarkRead()
		{
			parentFeed.FeedItems.MarkItemRead(this);
		}
	}
}
