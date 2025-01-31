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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
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
            resources.ApplyResources(this.txtWarning, "txtWarning");
            this.txtWarning.Name = "txtWarning";
            this.txtWarning.ReadOnly = true;
            this.txtWarning.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.BackColor = System.Drawing.SystemColors.Control;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // linkAWBPage
            // 
            resources.ApplyResources(this.linkAWBPage, "linkAWBPage");
            this.linkAWBPage.Name = "linkAWBPage";
            this.linkAWBPage.TabStop = true;
            this.linkAWBPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkAWBPage_LinkClicked);
            // 
            // lblDevs
            // 
            this.lblDevs.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblDevs, "lblDevs");
            this.lblDevs.Name = "lblDevs";
            // 
            // lblAWBVersion
            // 
            resources.ApplyResources(this.lblAWBVersion, "lblAWBVersion");
            this.lblAWBVersion.Name = "lblAWBVersion";
            // 
            // lblDetails
            // 
            this.lblDetails.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblDetails, "lblDetails");
            this.lblDetails.Name = "lblDetails";
            // 
            // lblOriginalDevs
            // 
            this.lblOriginalDevs.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblOriginalDevs, "lblOriginalDevs");
            this.lblOriginalDevs.Name = "lblOriginalDevs";
            // 
            // UsageStatsLabel
            // 
            resources.ApplyResources(this.UsageStatsLabel, "UsageStatsLabel");
            this.UsageStatsLabel.Name = "UsageStatsLabel";
            this.UsageStatsLabel.TabStop = true;
            this.UsageStatsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UsageStatsLabel_LinkClicked);
            // 
            // lblRevision
            // 
            resources.ApplyResources(this.lblRevision, "lblRevision");
            this.lblRevision.Name = "lblRevision";
            // 
            // flwDevs
            // 
            this.flwDevs.Controls.Add(this.MagioladitisLabel);
            this.flwDevs.Controls.Add(this.MaxSemLabel);
            this.flwDevs.Controls.Add(this.ReedyLabel);
            this.flwDevs.Controls.Add(this.RjLabel);
            resources.ApplyResources(this.flwDevs, "flwDevs");
            this.flwDevs.Name = "flwDevs";
            // 
            // MagioladitisLabel
            // 
            resources.ApplyResources(this.MagioladitisLabel, "MagioladitisLabel");
            this.MagioladitisLabel.Name = "MagioladitisLabel";
            this.MagioladitisLabel.TabStop = true;
            this.MagioladitisLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Magioladitis;
            // 
            // MaxSemLabel
            // 
            resources.ApplyResources(this.MaxSemLabel, "MaxSemLabel");
            this.MaxSemLabel.Name = "MaxSemLabel";
            this.MaxSemLabel.TabStop = true;
            this.MaxSemLabel.WhichDeveloper = WikiFunctions.Controls.Developers.MaxSem;
            // 
            // ReedyLabel
            // 
            resources.ApplyResources(this.ReedyLabel, "ReedyLabel");
            this.ReedyLabel.Name = "ReedyLabel";
            this.ReedyLabel.TabStop = true;
            this.ReedyLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Reedy;
            // 
            // RjLabel
            // 
            resources.ApplyResources(this.RjLabel, "RjLabel");
            this.RjLabel.Name = "RjLabel";
            this.RjLabel.TabStop = true;
            this.RjLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Rjwilmsi;
            // 
            // flwOriginalDevs
            // 
            this.flwOriginalDevs.Controls.Add(this.BluemooseLink);
            this.flwOriginalDevs.Controls.Add(this.LigulemLink);
            resources.ApplyResources(this.flwOriginalDevs, "flwOriginalDevs");
            this.flwOriginalDevs.Name = "flwOriginalDevs";
            // 
            // BluemooseLink
            // 
            resources.ApplyResources(this.BluemooseLink, "BluemooseLink");
            this.BluemooseLink.Name = "BluemooseLink";
            this.BluemooseLink.TabStop = true;
            // 
            // LigulemLink
            // 
            resources.ApplyResources(this.LigulemLink, "LigulemLink");
            this.LigulemLink.Name = "LigulemLink";
            this.LigulemLink.TabStop = true;
            this.LigulemLink.WhichDeveloper = WikiFunctions.Controls.Developers.Ligulem;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.linkPhabricator);
            this.flowLayoutPanel3.Controls.Add(this.UsageStatsLabel);
            resources.ApplyResources(this.flowLayoutPanel3, "flowLayoutPanel3");
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            // 
            // linkPhabricator
            // 
            resources.ApplyResources(this.linkPhabricator, "linkPhabricator");
            this.linkPhabricator.Name = "linkPhabricator";
            this.linkPhabricator.TabStop = true;
            this.linkPhabricator.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkPhabricator_LinkClicked);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.lblAWBVersion);
            this.flowLayoutPanel1.Controls.Add(this.lblRevision);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.kingboykLabel);
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // kingboykLabel
            // 
            resources.ApplyResources(this.kingboykLabel, "kingboykLabel");
            this.kingboykLabel.Name = "kingboykLabel";
            this.kingboykLabel.TabStop = true;
            this.kingboykLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Kingboyk;
            // 
            // lblContribs
            // 
            this.lblContribs.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblContribs, "lblContribs");
            this.lblContribs.Name = "lblContribs";
            // 
            // txtVersions
            // 
            resources.ApplyResources(this.txtVersions, "txtVersions");
            this.txtVersions.Name = "txtVersions";
            this.txtVersions.ReadOnly = true;
            this.txtVersions.TabStop = false;
            // 
            // AboutBox
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.CancelButton = this.okButton;
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
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
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
