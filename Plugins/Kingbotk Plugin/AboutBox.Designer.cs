using AutoWikiBrowser.Plugins.Kingbotk;
using AutoWikiBrowser.Plugins.Kingbotk.Components;
using AutoWikiBrowser.Plugins.Kingbotk.ManualAssessments;
using AutoWikiBrowser.Plugins.Kingbotk.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using AutoWikiBrowser.Plugins.Kingbotk.Properties;
using WikiFunctions;
using WikiFunctions.Plugin;
namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
	partial class AboutBox : System.Windows.Forms.Form
	{
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) {
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
			this.OKButton = new System.Windows.Forms.Button();
			this.LabelProductName = new System.Windows.Forms.Label();
			this.LabelVersion = new System.Windows.Forms.Label();
			this.LabelCopyright = new System.Windows.Forms.Label();
			this.TextBoxDescription = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.linkKingboy = new System.Windows.Forms.LinkLabel();
			this.Label2 = new System.Windows.Forms.Label();
			this.linkReedy = new System.Windows.Forms.LinkLabel();
			this.LicencingButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			//OKButton
			//
			this.OKButton.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.OKButton.Location = new System.Drawing.Point(348, 16);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(87, 23);
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "&OK";
            this.OKButton.Click += OKButton_Click;
			//
			//LabelProductName
			//
			this.LabelProductName.AutoSize = true;
			this.LabelProductName.BackColor = System.Drawing.Color.Transparent;
			this.LabelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.LabelProductName.Location = new System.Drawing.Point(10, 21);
			this.LabelProductName.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
			this.LabelProductName.MaximumSize = new System.Drawing.Size(0, 17);
			this.LabelProductName.Name = "LabelProductName";
			this.LabelProductName.Size = new System.Drawing.Size(221, 13);
			this.LabelProductName.TabIndex = 4;
			this.LabelProductName.Text = "About the Kingbotk Templating Plugin";
			this.LabelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//LabelVersion
			//
			this.LabelVersion.AutoSize = true;
			this.LabelVersion.BackColor = System.Drawing.Color.Transparent;
			this.LabelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.LabelVersion.Location = new System.Drawing.Point(10, 54);
			this.LabelVersion.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
			this.LabelVersion.MaximumSize = new System.Drawing.Size(0, 17);
			this.LabelVersion.Name = "LabelVersion";
			this.LabelVersion.Size = new System.Drawing.Size(49, 13);
			this.LabelVersion.TabIndex = 5;
			this.LabelVersion.Text = "Version";
			this.LabelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//LabelCopyright
			//
			this.LabelCopyright.AutoSize = true;
			this.LabelCopyright.BackColor = System.Drawing.Color.Transparent;
			this.LabelCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.LabelCopyright.Location = new System.Drawing.Point(10, 88);
			this.LabelCopyright.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
			this.LabelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
			this.LabelCopyright.Name = "LabelCopyright";
			this.LabelCopyright.Size = new System.Drawing.Size(189, 13);
			this.LabelCopyright.TabIndex = 2;
			this.LabelCopyright.Text = "Copyright © SDK Software 2008";
			this.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//TextBoxDescription
			//
			this.TextBoxDescription.BackColor = System.Drawing.Color.Transparent;
			this.TextBoxDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.TextBoxDescription.Location = new System.Drawing.Point(10, 151);
			this.TextBoxDescription.Name = "TextBoxDescription";
			this.TextBoxDescription.Size = new System.Drawing.Size(329, 112);
			this.TextBoxDescription.TabIndex = 6;
			this.TextBoxDescription.Text = "Disclaimer";
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.BackColor = System.Drawing.Color.Transparent;
			this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label1.Location = new System.Drawing.Point(357, 88);
			this.Label1.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
			this.Label1.MaximumSize = new System.Drawing.Size(0, 17);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(65, 13);
			this.Label1.TabIndex = 7;
			this.Label1.Text = "Written by";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//linkKingboy
			//
			this.linkKingboy.AutoSize = true;
			this.linkKingboy.BackColor = System.Drawing.Color.Transparent;
			this.linkKingboy.Location = new System.Drawing.Point(343, 120);
			this.linkKingboy.Name = "linkKingboy";
			this.linkKingboy.Size = new System.Drawing.Size(40, 13);
			this.linkKingboy.TabIndex = 47;
			this.linkKingboy.TabStop = true;
			this.linkKingboy.Text = "Steve";
            this.linkKingboy.LinkClicked += linkKingboy_LinkClicked;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.BackColor = System.Drawing.Color.Transparent;
			this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label2.Location = new System.Drawing.Point(349, 151);
			this.Label2.Margin = new System.Windows.Forms.Padding(7, 0, 3, 0);
			this.Label2.MaximumSize = new System.Drawing.Size(0, 17);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(86, 13);
			this.Label2.TabIndex = 48;
			this.Label2.Text = "with help from";
			this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//linkReedy
			//
			this.linkReedy.AutoSize = true;
			this.linkReedy.BackColor = System.Drawing.Color.Transparent;
			this.linkReedy.Location = new System.Drawing.Point(382, 177);
			this.linkReedy.Name = "linkReedy";
			this.linkReedy.Size = new System.Drawing.Size(31, 13);
			this.linkReedy.TabIndex = 49;
			this.linkReedy.TabStop = true;
			this.linkReedy.Text = "Sam";
            this.linkReedy.LinkClicked += linkReedy_LinkClicked;
			//
			//LicencingButton
			//
			this.LicencingButton.Location = new System.Drawing.Point(348, 45);
			this.LicencingButton.Name = "LicencingButton";
			this.LicencingButton.Size = new System.Drawing.Size(87, 23);
			this.LicencingButton.TabIndex = 50;
			this.LicencingButton.Text = "Licensing";
			this.LicencingButton.UseVisualStyleBackColor = true;
            this.LicencingButton.Click += LicencingButton_Click;
			//
			//AboutBox
			//
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.BackgroundImage = Resources.king_worship;
			this.ClientSize = new System.Drawing.Size(448, 262);
			this.Controls.Add(this.LicencingButton);
			this.Controls.Add(this.linkReedy);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.linkKingboy);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.TextBoxDescription);
			this.Controls.Add(this.LabelProductName);
			this.Controls.Add(this.LabelVersion);
			this.Controls.Add(this.LabelCopyright);
			this.Controls.Add(this.OKButton);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(10, 9, 10, 9);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About the Kingbotk Templating Plugin";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
        #endregion

        private System.Windows.Forms.LinkLabel linkKingboy;
        private System.Windows.Forms.LinkLabel linkReedy;
        private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Label LabelProductName;
		private System.Windows.Forms.Label LabelVersion;
		private System.Windows.Forms.Label LabelCopyright;
		private System.Windows.Forms.Label TextBoxDescription;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Button LicencingButton;
	}
}
