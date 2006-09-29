namespace BetterReader
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
			if (webBrowser1 != null)
			{
				webBrowser1.Dispose();
			}

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importOpmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newFeedSubscriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newFolderToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.hideReadFeedsBTN = new System.Windows.Forms.ToolStripButton();
			this.feedsTV = new BetterReader.FeedsTreeView();
			this.feedsImageList = new System.Windows.Forms.ImageList(this.components);
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.toolStrip2 = new System.Windows.Forms.ToolStrip();
			this.lastDownloadLBL = new System.Windows.Forms.ToolStripLabel();
			this.showUnreadFirstBTN = new System.Windows.Forms.ToolStripButton();
			this.feedTitleLBL = new System.Windows.Forms.Label();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.feedItemsLV = new System.Windows.Forms.ListView();
			this.feedItemsImageList = new System.Windows.Forms.ImageList(this.components);
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.itemLinkLBL = new System.Windows.Forms.LinkLabel();
			this.itemTitleLBL = new System.Windows.Forms.Label();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.feedSubContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newSubscriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unsubscribeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.feedSubNewFolderContextMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.markFeedReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyLinkLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reloadNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.feedReaderBGW = new System.ComponentModel.BackgroundWorker();
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.notifyIconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.folderContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.markAllReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.notifyIconImageList = new System.Windows.Forms.ImageList(this.components);
			this.mainMenuStrip.SuspendLayout();
			this.mainStatusStrip.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.toolStrip2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.feedSubContextMenuStrip.SuspendLayout();
			this.notifyIconContextMenuStrip.SuspendLayout();
			this.folderContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(933, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "menuStrip1";
			this.mainMenuStrip.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importOpmlToolStripMenuItem,
            this.newFeedSubscriptionToolStripMenuItem,
            this.newFolderToolStripMenuItem1});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// importOpmlToolStripMenuItem
			// 
			this.importOpmlToolStripMenuItem.Name = "importOpmlToolStripMenuItem";
			this.importOpmlToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			this.importOpmlToolStripMenuItem.Text = "Import Opml . . .";
			this.importOpmlToolStripMenuItem.Click += new System.EventHandler(this.importOpmlToolStripMenuItem_Click);
			// 
			// newFeedSubscriptionToolStripMenuItem
			// 
			this.newFeedSubscriptionToolStripMenuItem.Name = "newFeedSubscriptionToolStripMenuItem";
			this.newFeedSubscriptionToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
			this.newFeedSubscriptionToolStripMenuItem.Text = "New Feed Subscription . . .";
			this.newFeedSubscriptionToolStripMenuItem.Click += new System.EventHandler(this.newFeedSubscriptionToolStripMenuItem_Click);
			// 
			// newFolderToolStripMenuItem1
			// 
			this.newFolderToolStripMenuItem1.Name = "newFolderToolStripMenuItem1";
			this.newFolderToolStripMenuItem1.Size = new System.Drawing.Size(215, 22);
			this.newFolderToolStripMenuItem1.Text = "New Folder";
			this.newFolderToolStripMenuItem1.Click += new System.EventHandler(this.newFolderToolStripMenuItem1_Click);
			// 
			// mainStatusStrip
			// 
			this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
			this.mainStatusStrip.Location = new System.Drawing.Point(0, 709);
			this.mainStatusStrip.Name = "mainStatusStrip";
			this.mainStatusStrip.Size = new System.Drawing.Size(933, 22);
			this.mainStatusStrip.TabIndex = 1;
			this.mainStatusStrip.Text = "statusStrip1";
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer5);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(933, 685);
			this.splitContainer1.SplitterDistance = 310;
			this.splitContainer1.TabIndex = 2;
			this.splitContainer1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// splitContainer5
			// 
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.BackColor = System.Drawing.SystemColors.Info;
			this.splitContainer5.Panel1.Controls.Add(this.toolStrip1);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.feedsTV);
			this.splitContainer5.Size = new System.Drawing.Size(310, 685);
			this.splitContainer5.SplitterDistance = 57;
			this.splitContainer5.TabIndex = 0;
			this.splitContainer5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideReadFeedsBTN});
			this.toolStrip1.Location = new System.Drawing.Point(0, 32);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(310, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// hideReadFeedsBTN
			// 
			this.hideReadFeedsBTN.CheckOnClick = true;
			this.hideReadFeedsBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.hideReadFeedsBTN.Image = ((System.Drawing.Image)(resources.GetObject("hideReadFeedsBTN.Image")));
			this.hideReadFeedsBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.hideReadFeedsBTN.Name = "hideReadFeedsBTN";
			this.hideReadFeedsBTN.Size = new System.Drawing.Size(92, 22);
			this.hideReadFeedsBTN.Text = "Hide Read Feeds";
			this.hideReadFeedsBTN.CheckedChanged += new System.EventHandler(this.hideReadFeedsCB_CheckedChanged);
			// 
			// feedsTV
			// 
			this.feedsTV.AllowDrop = true;
			this.feedsTV.BackColor = System.Drawing.Color.WhiteSmoke;
			this.feedsTV.Cursor = System.Windows.Forms.Cursors.Default;
			this.feedsTV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.feedsTV.DragCursor = null;
			this.feedsTV.DragCursorType = Sloppycode.UI.DragCursorType.None;
			this.feedsTV.DragImageIndex = 0;
			this.feedsTV.DragMode = System.Windows.Forms.DragDropEffects.Move;
			this.feedsTV.DragNodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.feedsTV.DragNodeOpacity = 0.3;
			this.feedsTV.DragOverNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.feedsTV.DragOverNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.feedsTV.HideSelection = false;
			this.feedsTV.ImageIndex = 0;
			this.feedsTV.ImageList = this.feedsImageList;
			this.feedsTV.Location = new System.Drawing.Point(0, 0);
			this.feedsTV.Name = "feedsTV";
			this.feedsTV.SelectedImageIndex = 0;
			this.feedsTV.Size = new System.Drawing.Size(310, 624);
			this.feedsTV.TabIndex = 0;
			this.feedsTV.MouseClick += new System.Windows.Forms.MouseEventHandler(this.feedsTV_MouseClick);
			this.feedsTV.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.feedsTV_AfterLabelEdit);
			this.feedsTV.DragComplete += new Sloppycode.UI.DragCompleteEventHandler(this.feedsTV_DragComplete);
			this.feedsTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.feedsTV_AfterSelect);
			this.feedsTV.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// feedsImageList
			// 
			this.feedsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("feedsImageList.ImageStream")));
			this.feedsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.feedsImageList.Images.SetKeyName(0, "Folder.ico");
			this.feedsImageList.Images.SetKeyName(1, "Web_HTML.ico");
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Info;
			this.splitContainer2.Panel1.Controls.Add(this.toolStrip2);
			this.splitContainer2.Panel1.Controls.Add(this.feedTitleLBL);
			this.splitContainer2.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(619, 685);
			this.splitContainer2.SplitterDistance = 58;
			this.splitContainer2.TabIndex = 0;
			this.splitContainer2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// toolStrip2
			// 
			this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lastDownloadLBL,
            this.showUnreadFirstBTN});
			this.toolStrip2.Location = new System.Drawing.Point(0, 33);
			this.toolStrip2.Name = "toolStrip2";
			this.toolStrip2.Size = new System.Drawing.Size(619, 25);
			this.toolStrip2.TabIndex = 1;
			this.toolStrip2.Text = "toolStrip2";
			// 
			// lastDownloadLBL
			// 
			this.lastDownloadLBL.Name = "lastDownloadLBL";
			this.lastDownloadLBL.Size = new System.Drawing.Size(78, 22);
			this.lastDownloadLBL.Text = "toolStripLabel1";
			this.lastDownloadLBL.Visible = false;
			// 
			// showUnreadFirstBTN
			// 
			this.showUnreadFirstBTN.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.showUnreadFirstBTN.Image = ((System.Drawing.Image)(resources.GetObject("showUnreadFirstBTN.Image")));
			this.showUnreadFirstBTN.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.showUnreadFirstBTN.Name = "showUnreadFirstBTN";
			this.showUnreadFirstBTN.Size = new System.Drawing.Size(99, 22);
			this.showUnreadFirstBTN.Text = "Show Unread First";
			this.showUnreadFirstBTN.Visible = false;
			// 
			// feedTitleLBL
			// 
			this.feedTitleLBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.feedTitleLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.feedTitleLBL.Location = new System.Drawing.Point(2, 11);
			this.feedTitleLBL.Name = "feedTitleLBL";
			this.feedTitleLBL.Size = new System.Drawing.Size(617, 17);
			this.feedTitleLBL.TabIndex = 0;
			this.feedTitleLBL.Text = "BetterReader";
			this.feedTitleLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.feedItemsLV);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer3.Size = new System.Drawing.Size(619, 623);
			this.splitContainer3.SplitterDistance = 338;
			this.splitContainer3.TabIndex = 0;
			this.splitContainer3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// feedItemsLV
			// 
			this.feedItemsLV.AllowColumnReorder = true;
			this.feedItemsLV.BackColor = System.Drawing.Color.WhiteSmoke;
			this.feedItemsLV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.feedItemsLV.FullRowSelect = true;
			this.feedItemsLV.HideSelection = false;
			this.feedItemsLV.Location = new System.Drawing.Point(0, 0);
			this.feedItemsLV.MultiSelect = false;
			this.feedItemsLV.Name = "feedItemsLV";
			this.feedItemsLV.Size = new System.Drawing.Size(619, 338);
			this.feedItemsLV.SmallImageList = this.feedItemsImageList;
			this.feedItemsLV.TabIndex = 0;
			this.feedItemsLV.UseCompatibleStateImageBehavior = false;
			this.feedItemsLV.View = System.Windows.Forms.View.Details;
			this.feedItemsLV.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.feedItemsLV_MouseDoubleClick);
			this.feedItemsLV.SelectedIndexChanged += new System.EventHandler(this.feedItemsLV_SelectedIndexChanged);
			this.feedItemsLV.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.feedItemsLV_ColumnClick);
			this.feedItemsLV.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// feedItemsImageList
			// 
			this.feedItemsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("feedItemsImageList.ImageStream")));
			this.feedItemsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.feedItemsImageList.Images.SetKeyName(0, "ARW01UP.ICO");
			this.feedItemsImageList.Images.SetKeyName(1, "ARW01DN.ICO");
			this.feedItemsImageList.Images.SetKeyName(2, "UtilityText.ico");
			this.feedItemsImageList.Images.SetKeyName(3, "Read.ico");
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.BackColor = System.Drawing.SystemColors.Info;
			this.splitContainer4.Panel1.Controls.Add(this.itemLinkLBL);
			this.splitContainer4.Panel1.Controls.Add(this.itemTitleLBL);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.webBrowser1);
			this.splitContainer4.Size = new System.Drawing.Size(619, 281);
			this.splitContainer4.SplitterDistance = 36;
			this.splitContainer4.TabIndex = 0;
			this.splitContainer4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			// 
			// itemLinkLBL
			// 
			this.itemLinkLBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.itemLinkLBL.AutoSize = true;
			this.itemLinkLBL.Location = new System.Drawing.Point(3, 20);
			this.itemLinkLBL.Name = "itemLinkLBL";
			this.itemLinkLBL.Size = new System.Drawing.Size(0, 13);
			this.itemLinkLBL.TabIndex = 1;
			this.itemLinkLBL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.itemLinkLBL_LinkClicked);
			// 
			// itemTitleLBL
			// 
			this.itemTitleLBL.AutoSize = true;
			this.itemTitleLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.itemTitleLBL.Location = new System.Drawing.Point(4, 4);
			this.itemTitleLBL.Name = "itemTitleLBL";
			this.itemTitleLBL.Size = new System.Drawing.Size(0, 17);
			this.itemTitleLBL.TabIndex = 0;
			// 
			// webBrowser1
			// 
			this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser1.Location = new System.Drawing.Point(0, 0);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(619, 241);
			this.webBrowser1.TabIndex = 0;
			// 
			// feedSubContextMenuStrip
			// 
			this.feedSubContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSubscriptionToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.unsubscribeToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.feedSubNewFolderContextMenuStripItem,
            this.markFeedReadToolStripMenuItem,
            this.copyLinkLocationToolStripMenuItem,
            this.reloadNowToolStripMenuItem});
			this.feedSubContextMenuStrip.Name = "feedsContextMenuStrip";
			this.feedSubContextMenuStrip.Size = new System.Drawing.Size(189, 180);
			// 
			// newSubscriptionToolStripMenuItem
			// 
			this.newSubscriptionToolStripMenuItem.Name = "newSubscriptionToolStripMenuItem";
			this.newSubscriptionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.newSubscriptionToolStripMenuItem.Text = "New Subscription . . .";
			this.newSubscriptionToolStripMenuItem.Click += new System.EventHandler(this.newSubscriptionToolStripMenuItem_Click);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
			// 
			// unsubscribeToolStripMenuItem
			// 
			this.unsubscribeToolStripMenuItem.Name = "unsubscribeToolStripMenuItem";
			this.unsubscribeToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.unsubscribeToolStripMenuItem.Text = "Unsubscribe";
			this.unsubscribeToolStripMenuItem.Click += new System.EventHandler(this.unsubscribeToolStripMenuItem_Click);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.propertiesToolStripMenuItem.Text = "Properties . . .";
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// feedSubNewFolderContextMenuStripItem
			// 
			this.feedSubNewFolderContextMenuStripItem.Name = "feedSubNewFolderContextMenuStripItem";
			this.feedSubNewFolderContextMenuStripItem.Size = new System.Drawing.Size(188, 22);
			this.feedSubNewFolderContextMenuStripItem.Text = "New Folder";
			this.feedSubNewFolderContextMenuStripItem.Click += new System.EventHandler(this.feedSubNewFolderContextMenuStripItem_Click);
			// 
			// markFeedReadToolStripMenuItem
			// 
			this.markFeedReadToolStripMenuItem.Name = "markFeedReadToolStripMenuItem";
			this.markFeedReadToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.markFeedReadToolStripMenuItem.Text = "Mark Feed Read";
			this.markFeedReadToolStripMenuItem.Click += new System.EventHandler(this.markFeedReadToolStripMenuItem_Click);
			// 
			// copyLinkLocationToolStripMenuItem
			// 
			this.copyLinkLocationToolStripMenuItem.Name = "copyLinkLocationToolStripMenuItem";
			this.copyLinkLocationToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.copyLinkLocationToolStripMenuItem.Text = "Copy Link Location";
			this.copyLinkLocationToolStripMenuItem.Click += new System.EventHandler(this.copyLinkLocationToolStripMenuItem_Click);
			// 
			// reloadNowToolStripMenuItem
			// 
			this.reloadNowToolStripMenuItem.Name = "reloadNowToolStripMenuItem";
			this.reloadNowToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.reloadNowToolStripMenuItem.Text = "Reload Now";
			this.reloadNowToolStripMenuItem.Click += new System.EventHandler(this.reloadNowToolStripMenuItem_Click);
			// 
			// feedReaderBGW
			// 
			this.feedReaderBGW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.feedReaderBGW_DoWork);
			// 
			// notifyIcon1
			// 
			this.notifyIcon1.ContextMenuStrip = this.notifyIconContextMenuStrip;
			this.notifyIcon1.Text = "notifyIcon1";
			this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
			// 
			// notifyIconContextMenuStrip
			// 
			this.notifyIconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.notifyIconContextMenuStrip.Name = "notifyIconContextMenuStrip";
			this.notifyIconContextMenuStrip.Size = new System.Drawing.Size(104, 26);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// folderContextMenuStrip
			// 
			this.folderContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderToolStripMenuItem,
            this.renameToolStripMenuItem1,
            this.deleteToolStripMenuItem,
            this.markAllReadToolStripMenuItem});
			this.folderContextMenuStrip.Name = "folderContextMenuStrip";
			this.folderContextMenuStrip.Size = new System.Drawing.Size(153, 114);
			// 
			// newFolderToolStripMenuItem
			// 
			this.newFolderToolStripMenuItem.Name = "newFolderToolStripMenuItem";
			this.newFolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.newFolderToolStripMenuItem.Text = "New Folder";
			this.newFolderToolStripMenuItem.Click += new System.EventHandler(this.newFolderToolStripMenuItem_Click);
			// 
			// renameToolStripMenuItem1
			// 
			this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
			this.renameToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
			this.renameToolStripMenuItem1.Text = "Rename";
			this.renameToolStripMenuItem1.Click += new System.EventHandler(this.renameToolStripMenuItem1_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// markAllReadToolStripMenuItem
			// 
			this.markAllReadToolStripMenuItem.Name = "markAllReadToolStripMenuItem";
			this.markAllReadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.markAllReadToolStripMenuItem.Text = "Mark All Read";
			this.markAllReadToolStripMenuItem.Click += new System.EventHandler(this.markAllReadToolStripMenuItem_Click);
			// 
			// notifyIconImageList
			// 
			this.notifyIconImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("notifyIconImageList.ImageStream")));
			this.notifyIconImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.notifyIconImageList.Images.SetKeyName(0, "greenlight.ico");
			this.notifyIconImageList.Images.SetKeyName(1, "yellowlight.ico");
			this.notifyIconImageList.Images.SetKeyName(2, "redlight.ico");
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(933, 731);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.mainStatusStrip);
			this.Controls.Add(this.mainMenuStrip);
			this.KeyPreview = true;
			this.MainMenuStrip = this.mainMenuStrip;
			this.Name = "MainForm";
			this.Text = "BetterReader";
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.mainStatusStrip.ResumeLayout(false);
			this.mainStatusStrip.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel1.PerformLayout();
			this.splitContainer5.Panel2.ResumeLayout(false);
			this.splitContainer5.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.toolStrip2.ResumeLayout(false);
			this.toolStrip2.PerformLayout();
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			this.feedSubContextMenuStrip.ResumeLayout(false);
			this.notifyIconContextMenuStrip.ResumeLayout(false);
			this.folderContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
		private FeedsTreeView feedsTV;
		private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importOpmlToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker feedReaderBGW;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.ListView feedItemsLV;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.Label itemTitleLBL;
		private System.Windows.Forms.Label feedTitleLBL;
		private System.Windows.Forms.LinkLabel itemLinkLBL;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenuStrip feedSubContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem newSubscriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unsubscribeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip folderContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem markAllReadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem feedSubNewFolderContextMenuStripItem;
		private System.Windows.Forms.ToolStripMenuItem newFeedSubscriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem markFeedReadToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip notifyIconContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newFolderToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem copyLinkLocationToolStripMenuItem;
		private System.Windows.Forms.ImageList feedItemsImageList;
		private System.Windows.Forms.ToolStripMenuItem reloadNowToolStripMenuItem;
		private System.Windows.Forms.ImageList feedsImageList;
		private System.Windows.Forms.ImageList notifyIconImageList;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton hideReadFeedsBTN;
		private System.Windows.Forms.ToolStrip toolStrip2;
		private System.Windows.Forms.ToolStripLabel lastDownloadLBL;
		private System.Windows.Forms.ToolStripButton showUnreadFirstBTN;

    }
}

