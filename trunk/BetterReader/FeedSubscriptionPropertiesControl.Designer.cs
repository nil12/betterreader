namespace BetterReader
{
	partial class FeedSubscriptionPropertiesControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.updateMinutesTB = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.feedTitleTB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.urlTB = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.updateMinutesTB);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.feedTitleTB);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.urlTB);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(453, 114);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Feed Subscription Properties";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(134, 82);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "minutes";
			// 
			// updateMinutesTB
			// 
			this.updateMinutesTB.Location = new System.Drawing.Point(88, 79);
			this.updateMinutesTB.Name = "updateMinutesTB";
			this.updateMinutesTB.Size = new System.Drawing.Size(40, 20);
			this.updateMinutesTB.TabIndex = 5;
			this.updateMinutesTB.Text = "15";
			this.updateMinutesTB.TextChanged += new System.EventHandler(this.updateMinutesTB_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(7, 82);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Update Every:";
			// 
			// feedTitleTB
			// 
			this.feedTitleTB.Location = new System.Drawing.Point(88, 53);
			this.feedTitleTB.Name = "feedTitleTB";
			this.feedTitleTB.Size = new System.Drawing.Size(340, 20);
			this.feedTitleTB.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Feed Title:";
			// 
			// urlTB
			// 
			this.urlTB.Location = new System.Drawing.Point(88, 27);
			this.urlTB.Name = "urlTB";
			this.urlTB.Size = new System.Drawing.Size(340, 20);
			this.urlTB.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Feed URL:";
			// 
			// FeedSubscriptionPropertiesControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "FeedSubscriptionPropertiesControl";
			this.Size = new System.Drawing.Size(453, 114);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox updateMinutesTB;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox feedTitleTB;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox urlTB;
		private System.Windows.Forms.Label label1;
	}
}
