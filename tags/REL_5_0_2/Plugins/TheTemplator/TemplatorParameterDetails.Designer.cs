namespace AutoWikiBrowser.Plugins.TheTemplator
{
	partial class TemplatorParameterDetails
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
			this.OK = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.paramName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.paramRegex = new System.Windows.Forms.TextBox();
			this.regexHelp = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// OK
			// 
			this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK.Location = new System.Drawing.Point(88, 124);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(75, 23);
			this.OK.TabIndex = 5;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(169, 124);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 6;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Parameter name";
			// 
			// paramName
			// 
			this.paramName.Location = new System.Drawing.Point(12, 25);
			this.paramName.Name = "paramName";
			this.paramName.Size = new System.Drawing.Size(234, 20);
			this.paramName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 53);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(158, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "&Regular expression to recognise";
			// 
			// paramRegex
			// 
			this.paramRegex.Location = new System.Drawing.Point(12, 69);
			this.paramRegex.Name = "paramRegex";
			this.paramRegex.Size = new System.Drawing.Size(234, 20);
			this.paramRegex.TabIndex = 3;
			// 
			// regexHelp
			// 
			this.regexHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.regexHelp.AutoSize = true;
			this.regexHelp.Location = new System.Drawing.Point(126, 92);
			this.regexHelp.Name = "regexHelp";
			this.regexHelp.Size = new System.Drawing.Size(120, 13);
			this.regexHelp.TabIndex = 4;
			this.regexHelp.TabStop = true;
			this.regexHelp.Text = "Regular expression help";
			this.regexHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.regexHelp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.regexHelp_LinkClicked);
			// 
			// TemplatorParameterDetails
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(256, 159);
			this.Controls.Add(this.regexHelp);
			this.Controls.Add(this.paramRegex);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.paramName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TemplatorParameterDetails";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Parameter details";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox paramName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox paramRegex;
		private System.Windows.Forms.LinkLabel regexHelp;
	}
}