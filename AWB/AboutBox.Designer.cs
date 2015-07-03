/*
    Autowikibrowser
    Copyright (C) 2007 Martin Richards

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

namespace AutoWikiBrowser
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.txtWarning = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.linkAWBPage = new System.Windows.Forms.LinkLabel();
            this.lblDevs = new System.Windows.Forms.Label();
            this.lblAWBVersion = new System.Windows.Forms.Label();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblOriginalDevs = new System.Windows.Forms.Label();
            this.UsageStatsLabel = new System.Windows.Forms.LinkLabel();
            this.lblRevision = new System.Windows.Forms.Label();
            this.flwDevs = new System.Windows.Forms.FlowLayoutPanel();
            this.MagioladitisLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.MaxSemLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.ReedyLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.RjLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.flwOriginalDevs = new System.Windows.Forms.FlowLayoutPanel();
            this.BluemooseLink = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.LigulemLink = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.linkPhabricator = new System.Windows.Forms.LinkLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.kingboykLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.lblContribs = new System.Windows.Forms.Label();
            this.txtVersions = new System.Windows.Forms.TextBox();
            this.flwDevs.SuspendLayout();
            this.flwOriginalDevs.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtWarning
            // 
            this.txtWarning.Location = new System.Drawing.Point(234, 97);
            this.txtWarning.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.txtWarning.Multiline = true;
            this.txtWarning.Name = "txtWarning";
            this.txtWarning.ReadOnly = true;
            this.txtWarning.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtWarning.Size = new System.Drawing.Size(254, 159);
            this.txtWarning.TabIndex = 10;
            this.txtWarning.TabStop = false;
            this.txtWarning.Text = "WarningMessage";
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.SystemColors.Control;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(413, 264);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkAWBPage
            // 
            this.linkAWBPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkAWBPage.Location = new System.Drawing.Point(12, 9);
            this.linkAWBPage.Name = "linkAWBPage";
            this.linkAWBPage.Size = new System.Drawing.Size(197, 29);
            this.linkAWBPage.TabIndex = 1;
            this.linkAWBPage.TabStop = true;
            this.linkAWBPage.Text = "AutoWikiBrowser";
            this.linkAWBPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAWBPage_LinkClicked);
            // 
            // lblDevs
            // 
            this.lblDevs.BackColor = System.Drawing.Color.Transparent;
            this.lblDevs.Location = new System.Drawing.Point(9, 126);
            this.lblDevs.Name = "lblDevs";
            this.lblDevs.Size = new System.Drawing.Size(174, 13);
            this.lblDevs.TabIndex = 5;
            this.lblDevs.Text = "Now developed and maintained by:";
            // 
            // lblAWBVersion
            // 
            this.lblAWBVersion.AutoSize = true;
            this.lblAWBVersion.Location = new System.Drawing.Point(3, 0);
            this.lblAWBVersion.Name = "lblAWBVersion";
            this.lblAWBVersion.Size = new System.Drawing.Size(48, 13);
            this.lblAWBVersion.TabIndex = 0;
            this.lblAWBVersion.Text = "Version: ";
            // 
            // lblDetails
            // 
            this.lblDetails.BackColor = System.Drawing.Color.Transparent;
            this.lblDetails.Location = new System.Drawing.Point(12, 227);
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.Size = new System.Drawing.Size(42, 13);
            this.lblDetails.TabIndex = 7;
            this.lblDetails.Text = "Details:";
            // 
            // lblOriginalDevs
            // 
            this.lblOriginalDevs.BackColor = System.Drawing.Color.Transparent;
            this.lblOriginalDevs.Location = new System.Drawing.Point(7, 81);
            this.lblOriginalDevs.Name = "lblOriginalDevs";
            this.lblOriginalDevs.Size = new System.Drawing.Size(100, 13);
            this.lblOriginalDevs.TabIndex = 3;
            this.lblOriginalDevs.Text = "Original developers:";
            // 
            // UsageStatsLabel
            // 
            this.UsageStatsLabel.AutoSize = true;
            this.UsageStatsLabel.Location = new System.Drawing.Point(3, 13);
            this.UsageStatsLabel.Name = "UsageStatsLabel";
            this.UsageStatsLabel.Size = new System.Drawing.Size(81, 13);
            this.UsageStatsLabel.TabIndex = 2;
            this.UsageStatsLabel.TabStop = true;
            this.UsageStatsLabel.Text = "Usage statistics";
            this.UsageStatsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UsageStatsLabel_LinkClicked);
            // 
            // lblRevision
            // 
            this.lblRevision.AutoSize = true;
            this.lblRevision.Location = new System.Drawing.Point(3, 13);
            this.lblRevision.Name = "lblRevision";
            this.lblRevision.Size = new System.Drawing.Size(35, 13);
            this.lblRevision.TabIndex = 1;
            this.lblRevision.Text = "SVN: ";
            // 
            // flwDevs
            // 
            this.flwDevs.Controls.Add(this.MagioladitisLabel);
            this.flwDevs.Controls.Add(this.MaxSemLabel);
            this.flwDevs.Controls.Add(this.ReedyLabel);
            this.flwDevs.Controls.Add(this.RjLabel);
            this.flwDevs.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwDevs.Location = new System.Drawing.Point(28, 142);
            this.flwDevs.Name = "flwDevs";
            this.flwDevs.Size = new System.Drawing.Size(200, 48);
            this.flwDevs.TabIndex = 6;
            // 
            // MagioladitisLabel
            // 
            this.MagioladitisLabel.AutoSize = true;
            this.MagioladitisLabel.Location = new System.Drawing.Point(3, 0);
            this.MagioladitisLabel.Name = "MagioladitisLabel";
            this.MagioladitisLabel.Size = new System.Drawing.Size(62, 13);
            this.MagioladitisLabel.TabIndex = 38;
            this.MagioladitisLabel.TabStop = true;
            this.MagioladitisLabel.Text = "Magioladitis";
            this.MagioladitisLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Magioladitis;
            // 
            // MaxSemLabel
            // 
            this.MaxSemLabel.AutoSize = true;
            this.MaxSemLabel.Location = new System.Drawing.Point(3, 13);
            this.MaxSemLabel.Name = "MaxSemLabel";
            this.MaxSemLabel.Size = new System.Drawing.Size(48, 13);
            this.MaxSemLabel.TabIndex = 38;
            this.MaxSemLabel.TabStop = true;
            this.MaxSemLabel.Text = "MaxSem";
            this.MaxSemLabel.WhichDeveloper = WikiFunctions.Controls.Developers.MaxSem;
            // 
            // ReedyLabel
            // 
            this.ReedyLabel.AutoSize = true;
            this.ReedyLabel.Location = new System.Drawing.Point(3, 26);
            this.ReedyLabel.Name = "ReedyLabel";
            this.ReedyLabel.Size = new System.Drawing.Size(38, 13);
            this.ReedyLabel.TabIndex = 37;
            this.ReedyLabel.TabStop = true;
            this.ReedyLabel.Text = "Reedy";
            this.ReedyLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Reedy;
            // 
            // RjLabel
            // 
            this.RjLabel.AutoSize = true;
            this.RjLabel.Location = new System.Drawing.Point(71, 0);
            this.RjLabel.Name = "RjLabel";
            this.RjLabel.Size = new System.Drawing.Size(44, 13);
            this.RjLabel.TabIndex = 39;
            this.RjLabel.TabStop = true;
            this.RjLabel.Text = "Rjwilmsi";
            this.RjLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Rjwilmsi;
            // 
            // flwOriginalDevs
            // 
            this.flwOriginalDevs.Controls.Add(this.BluemooseLink);
            this.flwOriginalDevs.Controls.Add(this.LigulemLink);
            this.flwOriginalDevs.Location = new System.Drawing.Point(28, 97);
            this.flwOriginalDevs.Name = "flwOriginalDevs";
            this.flwOriginalDevs.Size = new System.Drawing.Size(200, 22);
            this.flwOriginalDevs.TabIndex = 4;
            // 
            // BluemooseLink
            // 
            this.BluemooseLink.AutoSize = true;
            this.BluemooseLink.Location = new System.Drawing.Point(3, 0);
            this.BluemooseLink.Name = "BluemooseLink";
            this.BluemooseLink.Size = new System.Drawing.Size(59, 13);
            this.BluemooseLink.TabIndex = 3;
            this.BluemooseLink.TabStop = true;
            this.BluemooseLink.Text = "Bluemoose";
            // 
            // LigulemLink
            // 
            this.LigulemLink.AutoSize = true;
            this.LigulemLink.Location = new System.Drawing.Point(68, 0);
            this.LigulemLink.Name = "LigulemLink";
            this.LigulemLink.Size = new System.Drawing.Size(43, 13);
            this.LigulemLink.TabIndex = 4;
            this.LigulemLink.TabStop = true;
            this.LigulemLink.Text = "Ligulem";
            this.LigulemLink.WhichDeveloper = WikiFunctions.Controls.Developers.Ligulem;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.linkPhabricator);
            this.flowLayoutPanel3.Controls.Add(this.UsageStatsLabel);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(28, 243);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(200, 44);
            this.flowLayoutPanel3.TabIndex = 8;
            // 
            // linkPhabricator
            // 
            this.linkPhabricator.AutoSize = true;
            this.linkPhabricator.Location = new System.Drawing.Point(3, 0);
            this.linkPhabricator.Name = "linkPhabricator";
            this.linkPhabricator.Size = new System.Drawing.Size(157, 13);
            this.linkPhabricator.TabIndex = 14;
            this.linkPhabricator.TabStop = true;
            this.linkPhabricator.Text = "Phabricator (bugs and requests)";
            this.linkPhabricator.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkPhabricator_LinkClicked);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.lblAWBVersion);
            this.flowLayoutPanel1.Controls.Add(this.lblRevision);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 41);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(216, 37);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.kingboykLabel);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(28, 209);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(200, 15);
            this.flowLayoutPanel2.TabIndex = 12;
            // 
            // kingboykLabel
            // 
            this.kingboykLabel.AutoSize = true;
            this.kingboykLabel.Location = new System.Drawing.Point(3, 0);
            this.kingboykLabel.Name = "kingboykLabel";
            this.kingboykLabel.Size = new System.Drawing.Size(51, 13);
            this.kingboykLabel.TabIndex = 38;
            this.kingboykLabel.TabStop = true;
            this.kingboykLabel.Text = "Kingboyk";
            this.kingboykLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Kingboyk;
            // 
            // lblContribs
            // 
            this.lblContribs.BackColor = System.Drawing.Color.Transparent;
            this.lblContribs.Location = new System.Drawing.Point(9, 193);
            this.lblContribs.Name = "lblContribs";
            this.lblContribs.Size = new System.Drawing.Size(174, 13);
            this.lblContribs.TabIndex = 11;
            this.lblContribs.Text = "Contributors and past developers:";
            // 
            // txtVersions
            // 
            this.txtVersions.Location = new System.Drawing.Point(237, 41);
            this.txtVersions.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.txtVersions.Multiline = true;
            this.txtVersions.Name = "txtVersions";
            this.txtVersions.ReadOnly = true;
            this.txtVersions.Size = new System.Drawing.Size(251, 50);
            this.txtVersions.TabIndex = 13;
            this.txtVersions.TabStop = false;
            this.txtVersions.Text = "Internet Explorer version:\r\n.NET version:\r\nWindows version:";
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(500, 299);
            this.Controls.Add(this.txtVersions);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.lblContribs);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.txtWarning);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.flwDevs);
            this.Controls.Add(this.lblDevs);
            this.Controls.Add(this.flwOriginalDevs);
            this.Controls.Add(this.lblOriginalDevs);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.linkAWBPage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.TopMost = true;
            this.flwDevs.ResumeLayout(false);
            this.flwDevs.PerformLayout();
            this.flwOriginalDevs.ResumeLayout(false);
            this.flwOriginalDevs.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtWarning;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkAWBPage;
        private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblAWBVersion;
        private System.Windows.Forms.Label lblDevs;
        private System.Windows.Forms.Label lblOriginalDevs;
        private System.Windows.Forms.LinkLabel UsageStatsLabel;
        private System.Windows.Forms.Label lblRevision;
        private System.Windows.Forms.FlowLayoutPanel flwDevs;
        private System.Windows.Forms.FlowLayoutPanel flwOriginalDevs;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private WikiFunctions.Controls.DeveloperLinkLabel BluemooseLink;
        private WikiFunctions.Controls.DeveloperLinkLabel LigulemLink;
        private WikiFunctions.Controls.DeveloperLinkLabel MagioladitisLabel;
        private WikiFunctions.Controls.DeveloperLinkLabel MaxSemLabel;
        private WikiFunctions.Controls.DeveloperLinkLabel ReedyLabel;
        private WikiFunctions.Controls.DeveloperLinkLabel RjLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private WikiFunctions.Controls.DeveloperLinkLabel kingboykLabel;
        private System.Windows.Forms.Label lblContribs;
        private System.Windows.Forms.TextBox txtVersions;
        private System.Windows.Forms.LinkLabel linkPhabricator;
    }
}
