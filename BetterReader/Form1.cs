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
            importOPML(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
            fsc.SaveAsFeedSubscriptionsFile("FeedSubscriptions.xml");
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
            TreeNode newNode = feedsTV.Nodes.Add(fsc.RootFolder.Name);
            newNode.Tag = fsc.RootFolder;
            treeNodesByTag.Add(fsc.RootFolder, newNode);

            foreach (FeedFolder folder in fsc.RootFolder.ChildFolders)
            {
                bindFolderToTreeView(folder, newNode);
            }
            feedsTV.ResumeLayout();
        }

        private void bindFolderToTreeView(FeedFolder folder, TreeNode parentNode)
        {
            TreeNode newNode = feedsTV.Nodes.Add(folder.Name);
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