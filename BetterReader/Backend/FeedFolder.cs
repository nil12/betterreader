using System;
using System.Collections.Generic;
using System.Text;

namespace BetterReader.Backend
{
    class FeedFolder
    {
        private string name;
        private List<FeedFolder> childFolders;
        private List<FeedSubscription> childSubscriptions;
        private FeedFolder parentFolder;

        internal FeedFolder ParentFolder
        {
            get { return parentFolder; }
            set { parentFolder = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        internal List<FeedFolder> ChildFolders
        {
            get { return childFolders; }
            set { childFolders = value; }
        }

        internal List<FeedSubscription> ChildSubscriptions
        {
            get { return childSubscriptions; }
            set { childSubscriptions = value; }
        }

        public FeedFolder()
        {
            childFolders = new List<FeedFolder>();
            childSubscriptions = new List<FeedSubscription>();
        }
    }
}
