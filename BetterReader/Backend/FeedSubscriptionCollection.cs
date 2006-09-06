using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace BetterReader.Backend
{
    public class FeedSubscriptionCollection
    {
        private string filepath;
        private FeedFolder currentWorkingFolder;
        private FeedFolder rootFolder;

        public FeedFolder RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        public void LoadFromOPML(string lFilepath)
        {
            filepath = lFilepath;
            LoadFromOPML();
        }

        public void LoadFromOPML()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationType = ValidationType.None;

            XmlDocument xmlDoc = null;
            using (TextReader tr = new StreamReader(filepath))
            {
                XmlReader xr = XmlReader.Create(tr, settings);

                xmlDoc = new XmlDocument();
                xmlDoc.Load(xr);
            }

            currentWorkingFolder = null;
            rootFolder = null;
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                processOPMLNode(node);
            }
        }

        public void SaveAsFeedSubscriptionsFile(string lFilepath)
        {
            using (TextWriter tw = new StreamWriter(lFilepath))
            {
                XmlSerializer xs = new XmlSerializer(this.GetType());
                xs.Serialize(tw, this);
            }
        }

        public static FeedSubscriptionCollection GetFromFeedSubscriptionsFile(string lFilepath)
        {
            FeedSubscriptionCollection fsc = null;
            using (TextReader tr = new StreamReader(lFilepath))
            {
                XmlSerializer xs = new XmlSerializer(typeof(FeedSubscriptionCollection));
                fsc = xs.Deserialize(tr) as FeedSubscriptionCollection;
            }

            return fsc;
        }


        public void ExportAsOPML(string lFilepath)
        {
            throw new Exception("Error.  ExportAsOPML not implemented yet.");
        }

        private void processOPMLNode(XmlNode node)
        {
            if (node.Name == null && node.Name.Length < 1)
            {
                return;
            }

            switch (node.Name.ToLower())
            {
                case "body":
                case "opml":
                case "xml":
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        processOPMLNode(childNode);
                    }
                    break;
                case "outline":
                    processOPMLOutlineNode(node);
                    break;
                case "head":
                    return;
            }
        }

        private void processOPMLOutlineNode(XmlNode node)
        {
            XmlAttribute titleAttr = node.Attributes["title"];
            string title = "";
            if (titleAttr != null)
            {
                title = titleAttr.Value;
            }

            if (title.Length > 0)
            {
                //this is a feed sub node
                FeedSubscription fs = new FeedSubscription();
                fs.DisplayName = node.Attributes["text"].Value;
                fs.FeedUrl = node.Attributes["xmlUrl"].Value;
                fs.ParentFolder = currentWorkingFolder;
                if (currentWorkingFolder != null)
                {
                    currentWorkingFolder.ChildSubscriptions.Add(fs);
                }
                else
                {
                    throw new ApplicationException("Error.  No root folder defined in OPML file.");
                }

                return;
            }
            else
            {
                //this is a folder node
                FeedFolder ff = new FeedFolder();
                ff.Name = node.Attributes["text"].Value;
                ff.ParentFolder = currentWorkingFolder;
                if (rootFolder == null)
                {
                    //this must be the root folder
                    rootFolder = ff;
                }
                else
                {
                    currentWorkingFolder.ChildFolders.Add(ff);
                }

                currentWorkingFolder = ff;
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    processOPMLNode(childNode);
                }
            }
        }
    }
}
