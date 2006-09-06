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
        private OPML opml;
        private Dictionary<object, TreeNode> treeNodesByTag;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            importOPML(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
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
            opml = new OPML();
            opml.Load(filepath);
            bindOPMLToTreeView();
            opml.SaveAsSubscriptionsFile("subscriptions.xml");
        }

        private void bindOPMLToTreeView()
        {
            feedsTV.SuspendLayout();
            treeNodesByTag = new Dictionary<object, TreeNode>();
            TreeNode newNode = feedsTV.Nodes.Add(opml.RootFolder.Name);
            newNode.Tag = opml.RootFolder;
            treeNodesByTag.Add(opml.RootFolder, newNode);

            foreach (FeedFolder folder in opml.RootFolder.ChildFolders)
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