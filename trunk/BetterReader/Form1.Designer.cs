namespace BetterReader
{
    partial class Form1
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
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importOpmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.feedsTV = new System.Windows.Forms.TreeView();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.feedItemsLV = new System.Windows.Forms.ListView();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.itemTitleLBL = new System.Windows.Forms.Label();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.feedReaderBGW = new System.ComponentModel.BackgroundWorker();
			this.feedTitleLBL = new System.Windows.Forms.Label();
			this.itemLinkLBL = new System.Windows.Forms.LinkLabel();
			this.mainMenuStrip.SuspendLayout();
			this.mainStatusStrip.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
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
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importOpmlToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// importOpmlToolStripMenuItem
			// 
			this.importOpmlToolStripMenuItem.Name = "importOpmlToolStripMenuItem";
			this.importOpmlToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.importOpmlToolStripMenuItem.Text = "Import Opml . . .";
			this.importOpmlToolStripMenuItem.Click += new System.EventHandler(this.importOpmlToolStripMenuItem_Click);
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
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
			this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
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
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.feedsTV);
			this.splitContainer5.Size = new System.Drawing.Size(310, 685);
			this.splitContainer5.SplitterDistance = 37;
			this.splitContainer5.TabIndex = 0;
			// 
			// feedsTV
			// 
			this.feedsTV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.feedsTV.Location = new System.Drawing.Point(0, 0);
			this.feedsTV.Name = "feedsTV";
			this.feedsTV.Size = new System.Drawing.Size(310, 644);
			this.feedsTV.TabIndex = 0;
			this.feedsTV.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.feedsTV_AfterSelect);
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
			this.splitContainer2.Panel1.Controls.Add(this.feedTitleLBL);
			this.splitContainer2.Panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer2.Size = new System.Drawing.Size(619, 685);
			this.splitContainer2.SplitterDistance = 37;
			this.splitContainer2.TabIndex = 0;
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
			this.splitContainer3.Size = new System.Drawing.Size(619, 644);
			this.splitContainer3.SplitterDistance = 268;
			this.splitContainer3.TabIndex = 0;
			// 
			// feedItemsLV
			// 
			this.feedItemsLV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.feedItemsLV.Location = new System.Drawing.Point(0, 0);
			this.feedItemsLV.MultiSelect = false;
			this.feedItemsLV.Name = "feedItemsLV";
			this.feedItemsLV.Size = new System.Drawing.Size(619, 268);
			this.feedItemsLV.TabIndex = 0;
			this.feedItemsLV.UseCompatibleStateImageBehavior = false;
			this.feedItemsLV.View = System.Windows.Forms.View.Details;
			this.feedItemsLV.SelectedIndexChanged += new System.EventHandler(this.feedItemsLV_SelectedIndexChanged);
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
			this.splitContainer4.Size = new System.Drawing.Size(619, 372);
			this.splitContainer4.SplitterDistance = 49;
			this.splitContainer4.TabIndex = 0;
			// 
			// itemTitleLBL
			// 
			this.itemTitleLBL.AutoSize = true;
			this.itemTitleLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.itemTitleLBL.Location = new System.Drawing.Point(4, 4);
			this.itemTitleLBL.Name = "itemTitleLBL";
			this.itemTitleLBL.Size = new System.Drawing.Size(15, 17);
			this.itemTitleLBL.TabIndex = 0;
			this.itemTitleLBL.Text = "x";
			// 
			// webBrowser1
			// 
			this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser1.Location = new System.Drawing.Point(0, 0);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(619, 319);
			this.webBrowser1.TabIndex = 0;
			// 
			// feedReaderBGW
			// 
			this.feedReaderBGW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.feedReaderBGW_DoWork);
			// 
			// feedTitleLBL
			// 
			this.feedTitleLBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.feedTitleLBL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.feedTitleLBL.Location = new System.Drawing.Point(2, 0);
			this.feedTitleLBL.Name = "feedTitleLBL";
			this.feedTitleLBL.Size = new System.Drawing.Size(617, 17);
			this.feedTitleLBL.TabIndex = 0;
			this.feedTitleLBL.Text = "x";
			this.feedTitleLBL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// itemLinkLBL
			// 
			this.itemLinkLBL.AutoSize = true;
			this.itemLinkLBL.Location = new System.Drawing.Point(3, 33);
			this.itemLinkLBL.Name = "itemLinkLBL";
			this.itemLinkLBL.Size = new System.Drawing.Size(12, 13);
			this.itemLinkLBL.TabIndex = 1;
			this.itemLinkLBL.TabStop = true;
			this.itemLinkLBL.Text = "x";
			this.itemLinkLBL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.itemLinkLBL_LinkClicked);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(933, 731);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.mainStatusStrip);
			this.Controls.Add(this.mainMenuStrip);
			this.MainMenuStrip = this.mainMenuStrip;
			this.Name = "Form1";
			this.Text = "Form1";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.mainStatusStrip.ResumeLayout(false);
			this.mainStatusStrip.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			this.splitContainer5.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView feedsTV;
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

    }
}

