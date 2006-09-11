using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BetterReader.Backend
{
	public class FeedItem
	{
		private string title;
		private string linkUrl;
		private string category;
		private string author;
		private string pubDate;
		private string guid;
		private string description;
		private Dictionary<string, string> unsupportedFeedItemProperties;
		private string encodedContent;
		private bool hasBeenRead;
		private Feed parentFeed;
		private DateTime downloadDate;

		public DateTime DownloadDate
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
			get
			{
				return title;
			}
			set
			{
				title = value;
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

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				author = value;
			}
		}

		public string PubDate
		{
			get
			{
				return pubDate;
			}
			set
			{
				pubDate = value;
			}
		}

		public string Guid
		{
			get
			{
				return guid;
			}
			set
			{
				guid = value;
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

		public FeedItem()
		{
			unsupportedFeedItemProperties = new Dictionary<string, string>();
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
						fi.title = innerText;
						break;
					case "link":
						fi.linkUrl = innerText;
						break;
					case "category":
						fi.category = innerText;
						break;
					case "author":
						fi.author = innerText;
						break;
					case "pubdate":
						fi.pubDate = innerText;
						break;
					case "guid":
						fi.guid = innerText;
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
			return fi;
		}

		private void SetGuid()
		{
			if (guid == null || guid.Length < 1)
			{
				//no guid provided by source so calculate our own
				string allProps = this.author + this.category + this.description + this.encodedContent +
					this.linkUrl + this.pubDate + this.title;
				this.guid = allProps.GetHashCode().ToString();
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
						fi.title = innerText;
						break;
					case "link":
						fi.linkUrl = innerText;
						break;
					case "category":
						fi.category = innerText;
						break;
					case "author":
						foreach (XmlNode authorNode in childNode.ChildNodes)
						{
							if (authorNode.Name == "name")
							{
								fi.author = authorNode.InnerText;
							}
						}
						break;
					case "modified":
						fi.pubDate = innerText;
						break;
					case "id":
						fi.guid = innerText;
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
			return fi;
		}
	}
}
