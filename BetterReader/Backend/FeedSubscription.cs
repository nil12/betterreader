using System;
using System.Collections.Generic;
using System.Text;

namespace BetterReader.Backend
{
   public  class FeedSubscription
    {
        private string feedUrl;

        public string FeedUrl
        {
            get { return feedUrl; }
            set { feedUrl = value; }
        }
        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        private FeedFolder parentFolder;

        internal FeedFolder ParentFolder
        {
            get { return parentFolder; }
            set { parentFolder = value; }
        }
        private int updateSeconds;

        public int UpdateSeconds
        {
            get { return updateSeconds; }
            set { updateSeconds = value; }
        }

        private Feed feed;

        internal Feed Feed
        {
            get { return feed; }
            set { feed = value; }
        }
    }
}
