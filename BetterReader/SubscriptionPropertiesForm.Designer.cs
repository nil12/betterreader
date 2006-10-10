namespace BetterReader
{
	partial class SubscriptionPropertiesForm
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
			this.feedSubscriptionPropertiesControl1 = new BetterReader.FeedSubscriptionPropertiesControl();
			this.okBTN = new System.Windows.Forms.Button();
			this.cancelBTN = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// feedSubscriptionPropertiesControl1
			// 
			this.feedSubscriptionPropertiesControl1.Location = new System.Drawing.Point(12, 12);
			this.feedSubscriptionPropertiesControl1.Name = "feedSubscriptionPropertiesControl1";
			this.feedSubscriptionPropertiesControl1.Size = new System.Drawing.Size(453, 268);
			this.feedSubscriptionPropertiesControl1.TabIndex = 0;
			// 
			// okBTN
			// 
			this.okBTN.Location = new System.Drawing.Point(390, 286);
			this.okBTN.Name = "okBTN";
			this.okBTN.Size = new System.Drawing.Size(75, 23);
			this.okBTN.TabIndex = 1;
			this.okBTN.Text = "OK";
			this.okBTN.UseVisualStyleBackColor = true;
			this.okBTN.Click += new System.EventHandler(this.okBTN_Click);
			// 
			// cancelBTN
			// 
			this.cancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBTN.Location = new System.Drawing.Point(309, 286);
			this.cancelBTN.Name = "cancelBTN";
			this.cancelBTN.Size = new System.Drawing.Size(75, 23);
			this.cancelBTN.TabIndex = 2;
			this.cancelBTN.Text = "Cancel";
			this.cancelBTN.UseVisualStyleBackColor = true;
			this.cancelBTN.Click += new System.EventHandler(this.cancelBTN_Click);
			// 
			// SubscriptionPropertiesForm
			// 
			this.AcceptButton = this.okBTN;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelBTN;
			this.ClientSize = new System.Drawing.Size(482, 319);
			this.ControlBox = false;
			this.Controls.Add(this.cancelBTN);
			this.Controls.Add(this.okBTN);
			this.Controls.Add(this.feedSubscriptionPropertiesControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SubscriptionPropertiesForm";
			this.Text = "Edit Subscription Properties . . .";
			this.ResumeLayout(false);

		}

		#endregion

		private FeedSubscriptionPropertiesControl feedSubscriptionPropertiesControl1;
		private System.Windows.Forms.Button okBTN;
		private System.Windows.Forms.Button cancelBTN;
	}
}