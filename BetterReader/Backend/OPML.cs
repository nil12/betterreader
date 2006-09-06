using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace BetterReader.Backend
{
    class OPML
    {
        private string filepath;
        private FeedFolder currentWorkingFolder;
        private FeedFolder rootFolder;

        internal FeedFolder RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; }
        }

        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        public void Load(string lFilepath)
        {
            filepath = lFilepath;
            Load();
        }

        public void Load()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CloseInput = true;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ValidationType = ValidationType.None;

            TextReader tr = new StreamReader(filepath);
            XmlReader xr = XmlReader.Create(tr, settings);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xr);

            currentWorkingFolder = null;
            rootFolder = null;
            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                processNode(node);
            }
        }

        public void SaveAsSubscriptionsFile(string lFilepath)
        {
            using (TextWriter tw = new StreamWriter(lFilepath, false))
            {
                using (XmlWriter xw = XmlWriter.Create(tw))
                {
                    xw.WriteStartDocument();
                    xw.WriteStartElement("FeedSubscriptions");
                    writeFolderNodeAsSubFile(rootFolder, xw);
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
            }
        }

        private void writeFolderNodeAsSubFile(FeedFolder folder, XmlWriter xw)
        {
            xw.WriteStartElement("FeedFolder");
            xw.WriteAttributeString("Name", folder.Name);
            foreach (FeedFolder subFolder in folder.ChildFolders)
            {
                writeFolderNodeAsSubFile(subFolder, xw);
            }

            foreach (FeedSubscription feedSub in folder.ChildSubscriptions)
            {
                xw.WriteStartElement("FeedSubscription");
                xw.WriteAttributeString("DisplayName", feedSub.DisplayName);
                xw.WriteAttributeString("FeedUrl", feedSub.FeedUrl);
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
        }

        public void ExportAsOPML(string lFilepath)
        {
            throw new Exception("Error.  ExportAsOPML not implemented yet.");
        }

        private void processNode(XmlNode node)
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
                        processNode(childNode);
                    }
                    break;
                case "outline":
                    processOutlineNode(node);
                    break;
                case "head":
                    return;
            }
        }

        private void processOutlineNode(XmlNode node)
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
                    processNode(childNode);
                }
            }
        }
    }
}
