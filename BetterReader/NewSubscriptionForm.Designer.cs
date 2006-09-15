namespace BetterReader
{
	partial class NewSubscriptionForm
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.feedFoldersTV = new System.Windows.Forms.TreeView();
			this.okBTN = new System.Windows.Forms.Button();
			this.cancelBTN = new System.Windows.Forms.Button();
			this.feedSubscriptionPropertiesControl1 = new BetterReader.FeedSubscriptionPropertiesControl();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.feedFoldersTV);
			this.groupBox1.Location = new System.Drawing.Point(12, 132);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(453, 168);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Create In";
			// 
			// feedFoldersTV
			// 
			this.feedFoldersTV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.feedFoldersTV.Location = new System.Drawing.Point(3, 16);
			this.feedFoldersTV.Name = "feedFoldersTV";
			this.feedFoldersTV.Size = new System.Drawing.Size(447, 149);
			this.feedFoldersTV.TabIndex = 0;
			// 
			// okBTN
			// 
			this.okBTN.Location = new System.Drawing.Point(386, 307);
			this.okBTN.Name = "okBTN";
			this.okBTN.Size = new System.Drawing.Size(75, 23);
			this.okBTN.TabIndex = 2;
			this.okBTN.Text = "OK";
			this.okBTN.UseVisualStyleBackColor = true;
			this.okBTN.Click += new System.EventHandler(this.okBTN_Click);
			// 
			// cancelBTN
			// 
			this.cancelBTN.Location = new System.Drawing.Point(305, 307);
			this.cancelBTN.Name = "cancelBTN";
			this.cancelBTN.Size = new System.Drawing.Size(75, 23);
			this.cancelBTN.TabIndex = 3;
			this.cancelBTN.Text = "Cancel";
			this.cancelBTN.UseVisualStyleBackColor = true;
			this.cancelBTN.Click += new System.EventHandler(this.cancelBTN_Click);
			// 
			// feedSubscriptionPropertiesControl1
			// 
			this.feedSubscriptionPropertiesControl1.FeedTitle = "";
			this.feedSubscriptionPropertiesControl1.FeedUrl = "";
			this.feedSubscriptionPropertiesControl1.Location = new System.Drawing.Point(12, 12);
			this.feedSubscriptionPropertiesControl1.Name = "feedSubscriptionPropertiesControl1";
			this.feedSubscriptionPropertiesControl1.Size = new System.Drawing.Size(453, 114);
			this.feedSubscriptionPropertiesControl1.TabIndex = 0;
			this.feedSubscriptionPropertiesControl1.UpdateSeconds = 900;
			// 
			// NewSubscriptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(483, 345);
			this.Controls.Add(this.cancelBTN);
			this.Controls.Add(this.okBTN);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.feedSubscriptionPropertiesControl1);
			this.Name = "NewSubscriptionForm";
			this.Text = "New Subscription";
			this.Load += new System.EventHandler(this.NewSubscriptionForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private FeedSubscriptionPropertiesControl feedSubscriptionPropertiesControl1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TreeView feedFoldersTV;
		private System.Windows.Forms.Button okBTN;
		private System.Windows.Forms.Button cancelBTN;
	}
}