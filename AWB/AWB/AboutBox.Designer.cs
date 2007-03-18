/*
    Autowikibrowser
    Copyright (C) 2006 Martin Richards

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
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.linkBluemoose = new System.Windows.Forms.LinkLabel();
            this.linkAWBPage = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblAWBVersion = new System.Windows.Forms.Label();
            this.lblOSVersion = new System.Windows.Forms.Label();
            this.lblIEVersion = new System.Windows.Forms.Label();
            this.lblNETVersion = new System.Windows.Forms.Label();
            this.linkLigulem = new System.Windows.Forms.LinkLabel();
            this.lblTimeAndEdits = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkMets501 = new System.Windows.Forms.LinkLabel();
            this.linkMaxSem = new System.Windows.Forms.LinkLabel();
            this.linkBugs = new System.Windows.Forms.LinkLabel();
            this.linkFeatureRequests = new System.Windows.Forms.LinkLabel();
            this.linkReedy = new System.Windows.Forms.LinkLabel();
            this.linkKingboy = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(15, 368);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(248, 116);
            this.textBoxDescription.TabIndex = 23;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "WarningMessage";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(189, 490);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 22);
            this.okButton.TabIndex = 24;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkBluemoose
            // 
            this.linkBluemoose.AutoSize = true;
            this.linkBluemoose.Location = new System.Drawing.Point(32, 30);
            this.linkBluemoose.Name = "linkBluemoose";
            this.linkBluemoose.Size = new System.Drawing.Size(84, 13);
            this.linkBluemoose.TabIndex = 25;
            this.linkBluemoose.TabStop = true;
            this.linkBluemoose.Text = "User:Bluemoose";
            this.linkBluemoose.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkBluemoose_LinkClicked);
            // 
            // linkAWBPage
            // 
            this.linkAWBPage.AutoSize = true;
            this.linkAWBPage.Location = new System.Drawing.Point(32, 192);
            this.linkAWBPage.Name = "linkAWBPage";
            this.linkAWBPage.Size = new System.Drawing.Size(115, 13);
            this.linkAWBPage.TabIndex = 26;
            this.linkAWBPage.TabStop = true;
            this.linkAWBPage.Text = "AutoWikiBrowser page";
            this.linkAWBPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Created by:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Details:";
            // 
            // lblAWBVersion
            // 
            this.lblAWBVersion.Location = new System.Drawing.Point(15, 292);
            this.lblAWBVersion.Name = "lblAWBVersion";
            this.lblAWBVersion.Size = new System.Drawing.Size(163, 13);
            this.lblAWBVersion.TabIndex = 29;
            this.lblAWBVersion.Text = "AWB version";
            // 
            // lblOSVersion
            // 
            this.lblOSVersion.AutoSize = true;
            this.lblOSVersion.Location = new System.Drawing.Point(15, 327);
            this.lblOSVersion.Name = "lblOSVersion";
            this.lblOSVersion.Size = new System.Drawing.Size(59, 13);
            this.lblOSVersion.TabIndex = 30;
            this.lblOSVersion.Text = "OS version";
            // 
            // lblIEVersion
            // 
            this.lblIEVersion.AutoSize = true;
            this.lblIEVersion.Location = new System.Drawing.Point(15, 309);
            this.lblIEVersion.Name = "lblIEVersion";
            this.lblIEVersion.Size = new System.Drawing.Size(54, 13);
            this.lblIEVersion.TabIndex = 31;
            this.lblIEVersion.Text = "IE version";
            // 
            // lblNETVersion
            // 
            this.lblNETVersion.AutoSize = true;
            this.lblNETVersion.Location = new System.Drawing.Point(15, 345);
            this.lblNETVersion.Name = "lblNETVersion";
            this.lblNETVersion.Size = new System.Drawing.Size(69, 13);
            this.lblNETVersion.TabIndex = 32;
            this.lblNETVersion.Text = ".NET version";
            // 
            // linkLigulem
            // 
            this.linkLigulem.AutoSize = true;
            this.linkLigulem.Location = new System.Drawing.Point(32, 50);
            this.linkLigulem.Name = "linkLigulem";
            this.linkLigulem.Size = new System.Drawing.Size(68, 13);
            this.linkLigulem.TabIndex = 33;
            this.linkLigulem.TabStop = true;
            this.linkLigulem.Text = "User:Ligulem";
            this.linkLigulem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLigulem_LinkClicked);
            // 
            // lblTimeAndEdits
            // 
            this.lblTimeAndEdits.AutoSize = true;
            this.lblTimeAndEdits.Location = new System.Drawing.Point(15, 275);
            this.lblTimeAndEdits.Name = "lblTimeAndEdits";
            this.lblTimeAndEdits.Size = new System.Drawing.Size(0, 13);
            this.lblTimeAndEdits.TabIndex = 34;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Actively maintained by:";
            // 
            // linkMets501
            // 
            this.linkMets501.AutoSize = true;
            this.linkMets501.Location = new System.Drawing.Point(32, 109);
            this.linkMets501.Name = "linkMets501";
            this.linkMets501.Size = new System.Drawing.Size(73, 13);
            this.linkMets501.TabIndex = 38;
            this.linkMets501.TabStop = true;
            this.linkMets501.Text = "User:Mets501";
            this.linkMets501.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMets501_LinkClicked);
            // 
            // linkMaxSem
            // 
            this.linkMaxSem.AutoSize = true;
            this.linkMaxSem.Location = new System.Drawing.Point(32, 89);
            this.linkMaxSem.Name = "linkMaxSem";
            this.linkMaxSem.Size = new System.Drawing.Size(73, 13);
            this.linkMaxSem.TabIndex = 37;
            this.linkMaxSem.TabStop = true;
            this.linkMaxSem.Text = "User:MaxSem";
            this.linkMaxSem.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMaxSem_LinkClicked);
            // 
            // linkBugs
            // 
            this.linkBugs.AutoSize = true;
            this.linkBugs.Location = new System.Drawing.Point(32, 212);
            this.linkBugs.Name = "linkBugs";
            this.linkBugs.Size = new System.Drawing.Size(31, 13);
            this.linkBugs.TabIndex = 40;
            this.linkBugs.TabStop = true;
            this.linkBugs.Text = "Bugs";
            this.linkBugs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // linkFeatureRequests
            // 
            this.linkFeatureRequests.AutoSize = true;
            this.linkFeatureRequests.Location = new System.Drawing.Point(32, 232);
            this.linkFeatureRequests.Name = "linkFeatureRequests";
            this.linkFeatureRequests.Size = new System.Drawing.Size(91, 13);
            this.linkFeatureRequests.TabIndex = 42;
            this.linkFeatureRequests.TabStop = true;
            this.linkFeatureRequests.Text = "Feature Requests";
            this.linkFeatureRequests.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel7_LinkClicked);
            // 
            // linkReedy
            // 
            this.linkReedy.AutoSize = true;
            this.linkReedy.Location = new System.Drawing.Point(32, 129);
            this.linkReedy.Name = "linkReedy";
            this.linkReedy.Size = new System.Drawing.Size(84, 13);
            this.linkReedy.TabIndex = 44;
            this.linkReedy.TabStop = true;
            this.linkReedy.Text = "User:Reedy Boy";
            this.linkReedy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReedy_LinkClicked);
            // 
            // linkKingboy
            // 
            this.linkKingboy.AutoSize = true;
            this.linkKingboy.Location = new System.Drawing.Point(32, 149);
            this.linkKingboy.Name = "linkKingboy";
            this.linkKingboy.Size = new System.Drawing.Size(76, 13);
            this.linkKingboy.TabIndex = 46;
            this.linkKingboy.TabStop = true;
            this.linkKingboy.Text = "User:Kingboyk";
            this.linkKingboy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkKingboy_LinkClicked);
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(276, 524);
            this.Controls.Add(this.linkKingboy);
            this.Controls.Add(this.linkReedy);
            this.Controls.Add(this.linkFeatureRequests);
            this.Controls.Add(this.linkBugs);
            this.Controls.Add(this.linkMets501);
            this.Controls.Add(this.linkMaxSem);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTimeAndEdits);
            this.Controls.Add(this.linkLigulem);
            this.Controls.Add(this.lblNETVersion);
            this.Controls.Add(this.lblIEVersion);
            this.Controls.Add(this.lblOSVersion);
            this.Controls.Add(this.lblAWBVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkAWBPage);
            this.Controls.Add(this.linkBluemoose);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textBoxDescription);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkBluemoose;
        private System.Windows.Forms.LinkLabel linkAWBPage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblAWBVersion;
        private System.Windows.Forms.Label lblOSVersion;
        private System.Windows.Forms.Label lblIEVersion;
        private System.Windows.Forms.Label lblNETVersion;
        private System.Windows.Forms.LinkLabel linkLigulem;
        private System.Windows.Forms.Label lblTimeAndEdits;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkMets501;
        private System.Windows.Forms.LinkLabel linkMaxSem;
        private System.Windows.Forms.LinkLabel linkBugs;
        private System.Windows.Forms.LinkLabel linkFeatureRequests;
        private System.Windows.Forms.LinkLabel linkReedy;
        private System.Windows.Forms.LinkLabel linkKingboy;
    }
}
