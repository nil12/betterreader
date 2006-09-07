using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using Rss;
using BetterReader.Backend;

namespace BetterReader
{
    public partial class Form1 : Form
    {
        private FeedSubscriptionCollection fsc;
        private Dictionary<object, TreeNode> treeNodesByTag;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //importOPML(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
            //fsc.SaveAsFeedSubscriptionsFile("FeedSubscriptions.xml");
            fsc = FeedSubscriptionCollection.GetFromFeedSubscriptionsFile("FeedSubscriptions.xml");
			fsc.ReadAllFeeds();
            bindFSCToTreeView();
        }

        private void importOPMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showImportOPMLDialog();
        }

        private void showImportOPMLDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                importOPML(ofd.FileName);
            }
        }

        private void importOPML(string filepath)
        {
            fsc = new FeedSubscriptionCollection();
            fsc.LoadFromOPML(filepath);
            bindFSCToTreeView();
        }

        private void bindFSCToTreeView()
        {
            feedsTV.SuspendLayout();
            treeNodesByTag = new Dictionary<object, TreeNode>();
            bindFolderToTreeView(fsc.RootFolder, null);
            feedsTV.ResumeLayout();
        }

        private void bindFolderToTreeView(FeedFolder folder, TreeNode parentNode)
        {
            TreeNode newNode;
            if (parentNode == null)
            {
                //this is the root
                newNode = feedsTV.Nodes.Add(folder.Name);
            }
            else
            {
                newNode = parentNode.Nodes.Add(folder.Name);
            }
            newNode.Tag = folder;
            treeNodesByTag.Add(folder, newNode);
            foreach (FeedFolder childFolder in folder.ChildFolders)
            {
                bindFolderToTreeView(childFolder, newNode);
            }

            foreach (FeedSubscription sub in folder.ChildSubscriptions)
            {
                TreeNode subNode = newNode.Nodes.Add(sub.DisplayName);
                subNode.Tag = sub;
                treeNodesByTag.Add(sub, subNode);
            }
        }
    }
}