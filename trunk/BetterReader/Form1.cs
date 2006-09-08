using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BetterReader.Backend;

namespace BetterReader
{
    public partial class Form1 : Form
    {
        private FeedSubscriptionTree fst;
        private Dictionary<object, TreeNode> treeNodesByTag;
		private readonly string settingsDirectory = System.Environment.CurrentDirectory + "\\appSettings\\";
		private readonly string feedSubsFilepath;

        public Form1()
        {
			InitializeComponent();
			feedSubsFilepath = settingsDirectory + "FeedSubscriptions.xml";
        }

        private void Form1_Load(object sender, EventArgs e)
		{
			ensureDirectoryExists(settingsDirectory);
			if (File.Exists(feedSubsFilepath))
			{
				fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile(feedSubsFilepath);
				bindFSTToTreeView();
				fst.ReadAllFeeds();
			}
            //importOpml(@"C:\Documents and Settings\skain\Desktop\rssowl.opml");
            //fsc.SaveAsFeedSubscriptionsFile("FeedSubscriptions.xml");
			//fst = FeedSubscriptionTree.GetFromFeedSubscriptionsFile("FeedSubscriptions.xml");
			////fst.ReadAllFeeds();
			//bindFSTToTreeView();
        }

		private void ensureDirectoryExists(string path)
		{
			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);
			}
		}

        private void importOpmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showImportOpmlDialog();
        }

        private void showImportOpmlDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                importOpml(ofd.FileName);
            }
        }

        private void importOpml(string filepath)
        {
            fst = new FeedSubscriptionTree();
            fst.LoadFromOpml(filepath);
			fst.SaveAsFeedSubscriptionsFile(settingsDirectory + "FeedSubscriptions.xml");
            bindFSTToTreeView();
        }

        private void bindFSTToTreeView()
        {
            feedsTV.SuspendLayout();
			feedsTV.Nodes.Clear();
            treeNodesByTag = new Dictionary<object, TreeNode>();
			//bindFolderToTreeView(fsc.RootFolder, null);
			bindNodeListToTreeView(fst.RootLevelNodes, null);
            feedsTV.ResumeLayout();
        }

		private void bindNodeListToTreeView(List<FeedSubTreeNodeBase> nodeList, TreeNode treeNode)
		{
			foreach (FeedSubTreeNodeBase fstnb in nodeList)
			{
				Type nodeType = fstnb.GetType();
				if (nodeType == typeof(FeedSubscription))
				{
					FeedSubscription fs = fstnb as FeedSubscription;
					TreeNode newNode;
					if (treeNode == null)
					{
						//we're at the root level of the tree
						newNode = feedsTV.Nodes.Add(fs.DisplayName);
					}
					else
					{
						newNode = treeNode.Nodes.Add(fs.DisplayName);
					}

					newNode.Tag = fs;
				}
				else if (nodeType == typeof(FeedFolder))
				{
					FeedFolder ff = fstnb as FeedFolder;
					TreeNode newNode;
					if (treeNode == null)
					{
						//we're at the root level of the tree
						newNode = feedsTV.Nodes.Add(ff.Name);
					}
					else
					{
						newNode = treeNode.Nodes.Add(ff.Name);
					}

					newNode.Tag = ff;

					bindNodeListToTreeView(ff.ChildNodes, newNode);
				}
				else
				{
					throw new Exception("Form1.bindNodeListToTreeView error: Unrecognized node type: " + nodeType.ToString());
				}
			}
		}

		//private void bindFolderToTreeView(FeedFolder folder, TreeNode parentNode)
		//{
		//    //TreeNode newNode;
		//    //if (parentNode == null)
		//    //{
		//    //    //this is the root
		//    //    newNode = feedsTV.Nodes.Add(folder.Name);
		//    //}
		//    //else
		//    //{
		//    //    newNode = parentNode.Nodes.Add(folder.Name);
		//    //}
		//    //newNode.Tag = folder;
		//    //treeNodesByTag.Add(folder, newNode);
		//    //foreach (FeedFolder childFolder in folder.ChildNodes)
		//    //{
		//    //    bindFolderToTreeView(childFolder, newNode);
		//    //}

		//    //foreach (FeedSubscription sub in folder.ChildSubscriptions)
		//    //{
		//    //    TreeNode subNode = newNode.Nodes.Add(sub.DisplayName);
		//    //    subNode.Tag = sub;
		//    //    treeNodesByTag.Add(sub, subNode);
		//    //}
		//}
    }
}