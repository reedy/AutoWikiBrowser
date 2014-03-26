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
using WikiFunctions;

using WikiFunctions.Plugin;
namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
	partial class PluginSettingsControl : System.Windows.Forms.UserControl
	{
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
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
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.GroupBox2 = new System.Windows.Forms.GroupBox();
			this.Label9 = new System.Windows.Forms.Label();
			this.lblRedlink = new System.Windows.Forms.Label();
			this.lblTagged = new System.Windows.Forms.Label();
			this.lblSkipped = new System.Windows.Forms.Label();
			this.lblNoChange = new System.Windows.Forms.Label();
			this.lblBadTag = new System.Windows.Forms.Label();
			this.lblNamespace = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.Label4 = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.ManuallyAssessCheckBox = new System.Windows.Forms.CheckBox();
			this.CleanupCheckBox = new System.Windows.Forms.CheckBox();
			this.Label7 = new System.Windows.Forms.Label();
			this.SkipNoChangesCheckBox = new System.Windows.Forms.CheckBox();
			this.SkipBadTagsCheckBox = new System.Windows.Forms.CheckBox();
			this.lblAWBNudges = new System.Windows.Forms.Label();
			this.ResetTimerButton = new System.Windows.Forms.Button();
			this.ETALabel = new System.Windows.Forms.Label();
			this.OpenBadInBrowserCheckBox = new System.Windows.Forms.CheckBox();
			this.GroupBox4 = new System.Windows.Forms.GroupBox();
			this.PluginMenuStrip = new System.Windows.Forms.MenuStrip();
			this.PluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SetAWBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.BotTimer = new System.Windows.Forms.Timer(this.components);
			this.TimerStats1 = new AutoWikiBrowser.Plugins.Kingbotk.Components.TimerStats();
			this.GroupBox2.SuspendLayout();
			this.GroupBox4.SuspendLayout();
			this.PluginMenuStrip.SuspendLayout();
			this.SuspendLayout();
			//
			//GroupBox2
			//
			this.GroupBox2.Controls.Add(this.Label9);
			this.GroupBox2.Controls.Add(this.lblRedlink);
			this.GroupBox2.Controls.Add(this.lblTagged);
			this.GroupBox2.Controls.Add(this.lblSkipped);
			this.GroupBox2.Controls.Add(this.lblNoChange);
			this.GroupBox2.Controls.Add(this.lblBadTag);
			this.GroupBox2.Controls.Add(this.lblNamespace);
			this.GroupBox2.Controls.Add(this.Label5);
			this.GroupBox2.Controls.Add(this.Label4);
			this.GroupBox2.Controls.Add(this.Label3);
			this.GroupBox2.Controls.Add(this.Label2);
			this.GroupBox2.Controls.Add(this.Label1);
			this.GroupBox2.Location = new System.Drawing.Point(3, 3);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.Size = new System.Drawing.Size(135, 128);
			this.GroupBox2.TabIndex = 36;
			this.GroupBox2.TabStop = false;
			this.GroupBox2.Text = "Statistics";
			this.ToolTip1.SetToolTip(this.GroupBox2, "Lies, damned lies and statistics");
			//
			//Label9
			//
			this.Label9.AutoSize = true;
			this.Label9.Location = new System.Drawing.Point(3, 111);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(46, 13);
			this.Label9.TabIndex = 45;
			this.Label9.Text = "Redlink:";
			this.ToolTip1.SetToolTip(this.Label9, "Talk pages skipped because the article was a redlink");
			//
			//lblRedlink
			//
			this.lblRedlink.AutoSize = true;
			this.lblRedlink.Location = new System.Drawing.Point(73, 111);
			this.lblRedlink.Name = "lblRedlink";
			this.lblRedlink.Size = new System.Drawing.Size(0, 13);
			this.lblRedlink.TabIndex = 44;
			this.ToolTip1.SetToolTip(this.lblRedlink, "Talk pages skipped because the article was a redlink");
			//
			//lblTagged
			//
			this.lblTagged.AutoSize = true;
			this.lblTagged.Location = new System.Drawing.Point(73, 16);
			this.lblTagged.Name = "lblTagged";
			this.lblTagged.Size = new System.Drawing.Size(0, 13);
			this.lblTagged.TabIndex = 37;
			this.ToolTip1.SetToolTip(this.lblTagged, "Number of articles tagged");
			//
			//lblSkipped
			//
			this.lblSkipped.AutoSize = true;
			this.lblSkipped.Location = new System.Drawing.Point(73, 35);
			this.lblSkipped.Name = "lblSkipped";
			this.lblSkipped.Size = new System.Drawing.Size(0, 13);
			this.lblSkipped.TabIndex = 38;
			this.ToolTip1.SetToolTip(this.lblSkipped, "Number of articles skipped");
			//
			//lblNoChange
			//
			this.lblNoChange.AutoSize = true;
			this.lblNoChange.Location = new System.Drawing.Point(73, 54);
			this.lblNoChange.Name = "lblNoChange";
			this.lblNoChange.Size = new System.Drawing.Size(0, 13);
			this.lblNoChange.TabIndex = 39;
			this.ToolTip1.SetToolTip(this.lblNoChange, "Number of articles skipped because no change was made");
			//
			//lblBadTag
			//
			this.lblBadTag.AutoSize = true;
			this.lblBadTag.Location = new System.Drawing.Point(73, 73);
			this.lblBadTag.Name = "lblBadTag";
			this.lblBadTag.Size = new System.Drawing.Size(0, 13);
			this.lblBadTag.TabIndex = 40;
			this.ToolTip1.SetToolTip(this.lblBadTag, "Number of articles skipped because they had an unparseable template");
			//
			//lblNamespace
			//
			this.lblNamespace.AutoSize = true;
			this.lblNamespace.Location = new System.Drawing.Point(73, 92);
			this.lblNamespace.Name = "lblNamespace";
			this.lblNamespace.Size = new System.Drawing.Size(0, 13);
			this.lblNamespace.TabIndex = 41;
			this.ToolTip1.SetToolTip(this.lblNamespace, "Number of articles skipped because they were in an incorrect namespace (e.g. we w" + "on't tag articles with talk page templates)");
			//
			//Label5
			//
			this.Label5.AutoSize = true;
			this.Label5.Location = new System.Drawing.Point(3, 92);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(67, 13);
			this.Label5.TabIndex = 4;
			this.Label5.Text = "Namespace:";
			this.ToolTip1.SetToolTip(this.Label5, "Number of articles skipped because they were in an incorrect namespace (e.g. we w" + "on't tag articles with talk page templates)");
			//
			//Label4
			//
			this.Label4.AutoSize = true;
			this.Label4.Location = new System.Drawing.Point(3, 73);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(51, 13);
			this.Label4.TabIndex = 3;
			this.Label4.Text = "Bad Tag:";
			this.ToolTip1.SetToolTip(this.Label4, "Number of articles skipped because they had an unparseable template");
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(3, 54);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(64, 13);
			this.Label3.TabIndex = 2;
			this.Label3.Text = "No Change:";
			this.ToolTip1.SetToolTip(this.Label3, "Number of articles skipped because no change was made");
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label2.Location = new System.Drawing.Point(3, 35);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(57, 13);
			this.Label2.TabIndex = 1;
			this.Label2.Text = "Skipped:";
			this.ToolTip1.SetToolTip(this.Label2, "Number of articles skipped by the plugin (not  necessarily by AWB too)");
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.Label1.Location = new System.Drawing.Point(3, 16);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(54, 13);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Tagged:";
			this.ToolTip1.SetToolTip(this.Label1, "Number of articles tagged");
			//
			//ManuallyAssessCheckBox
			//
			this.ManuallyAssessCheckBox.AutoSize = true;
			this.ManuallyAssessCheckBox.Location = new System.Drawing.Point(3, 14);
			this.ManuallyAssessCheckBox.Name = "ManuallyAssessCheckBox";
			this.ManuallyAssessCheckBox.Size = new System.Drawing.Size(59, 17);
			this.ManuallyAssessCheckBox.TabIndex = 38;
			this.ManuallyAssessCheckBox.Text = "Assess";
			this.ToolTip1.SetToolTip(this.ManuallyAssessCheckBox, "Assess articles by loading an article list and having the plugin load the talk pa" + "ge after the article has been reviewed");
			this.ManuallyAssessCheckBox.UseVisualStyleBackColor = true;
			//
			//CleanupCheckBox
			//
			this.CleanupCheckBox.AutoSize = true;
			this.CleanupCheckBox.Enabled = false;
			this.CleanupCheckBox.Location = new System.Drawing.Point(59, 14);
			this.CleanupCheckBox.Name = "CleanupCheckBox";
			this.CleanupCheckBox.Size = new System.Drawing.Size(70, 17);
			this.CleanupCheckBox.TabIndex = 39;
			this.CleanupCheckBox.Text = "Clean Up";
			this.ToolTip1.SetToolTip(this.CleanupCheckBox, "Clean-up articles during the assessment process (Unicodify, auto-tag and general " + "fixes)");
			this.CleanupCheckBox.UseVisualStyleBackColor = true;
			//
			//Label7
			//
			this.Label7.AutoSize = true;
			this.Label7.Location = new System.Drawing.Point(149, 19);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(31, 13);
			this.Label7.TabIndex = 45;
			this.Label7.Text = "Skip:";
			this.ToolTip1.SetToolTip(this.Label7, "Yes, the plugin has skip options too, sorry.");
			//
			//SkipNoChangesCheckBox
			//
			this.SkipNoChangesCheckBox.AutoSize = true;
			this.SkipNoChangesCheckBox.Location = new System.Drawing.Point(181, 19);
			this.SkipNoChangesCheckBox.Name = "SkipNoChangesCheckBox";
			this.SkipNoChangesCheckBox.Size = new System.Drawing.Size(46, 17);
			this.SkipNoChangesCheckBox.TabIndex = 46;
			this.SkipNoChangesCheckBox.Text = "N/C";
			this.ToolTip1.SetToolTip(this.SkipNoChangesCheckBox, "Skip the talk page if the plugin doesn't make a change (suggest YES for bots, NO " + "for manual editing)");
			this.SkipNoChangesCheckBox.UseVisualStyleBackColor = true;
			//
			//SkipBadTagsCheckBox
			//
			this.SkipBadTagsCheckBox.AutoSize = true;
			this.SkipBadTagsCheckBox.Location = new System.Drawing.Point(228, 19);
			this.SkipBadTagsCheckBox.Name = "SkipBadTagsCheckBox";
			this.SkipBadTagsCheckBox.Size = new System.Drawing.Size(45, 17);
			this.SkipBadTagsCheckBox.TabIndex = 47;
			this.SkipBadTagsCheckBox.Text = "Bad";
			this.ToolTip1.SetToolTip(this.SkipBadTagsCheckBox, "Skip the talk page if the existing template instance is bad  (suggest YES for bot" + "s, NO for manual editing)");
			this.SkipBadTagsCheckBox.UseVisualStyleBackColor = true;
			//
			//lblAWBNudges
			//
			this.lblAWBNudges.AutoSize = true;
			this.lblAWBNudges.Location = new System.Drawing.Point(67, 146);
			this.lblAWBNudges.Name = "lblAWBNudges";
			this.lblAWBNudges.Size = new System.Drawing.Size(56, 13);
			this.lblAWBNudges.TabIndex = 48;
			this.lblAWBNudges.Text = "Nudges: 0";
			this.ToolTip1.SetToolTip(this.lblAWBNudges, "Number of times AWB got nudged");
			this.lblAWBNudges.Visible = false;
			//
			//ResetTimerButton
			//
			this.ResetTimerButton.Location = new System.Drawing.Point(64, 163);
			this.ResetTimerButton.Name = "ResetTimerButton";
			this.ResetTimerButton.Size = new System.Drawing.Size(69, 23);
			this.ResetTimerButton.TabIndex = 49;
			this.ResetTimerButton.Text = "Reset";
			this.ToolTip1.SetToolTip(this.ResetTimerButton, "Reset the timer");
			this.ResetTimerButton.UseVisualStyleBackColor = true;
			//
			//ETALabel
			//
			this.ETALabel.AutoSize = true;
			this.ETALabel.Location = new System.Drawing.Point(141, 168);
			this.ETALabel.Name = "ETALabel";
			this.ETALabel.Size = new System.Drawing.Size(34, 13);
			this.ETALabel.TabIndex = 50;
			this.ETALabel.Text = "ETC: ";
			this.ToolTip1.SetToolTip(this.ETALabel, "Estimated time of completion");
			this.ETALabel.Visible = false;
			//
			//OpenBadInBrowserCheckBox
			//
			this.OpenBadInBrowserCheckBox.AutoSize = true;
			this.OpenBadInBrowserCheckBox.Location = new System.Drawing.Point(163, 45);
			this.OpenBadInBrowserCheckBox.Name = "OpenBadInBrowserCheckBox";
			this.OpenBadInBrowserCheckBox.Size = new System.Drawing.Size(105, 30);
			this.OpenBadInBrowserCheckBox.TabIndex = 52;
			this.OpenBadInBrowserCheckBox.Text = "Open bad pages" + Microsoft.VisualBasic.Strings.ChrW(13) + Microsoft.VisualBasic.Strings.ChrW(10) + "in browser";
			this.ToolTip1.SetToolTip(this.OpenBadInBrowserCheckBox, "Open in the web browser pages skipped because they have bad tags");
			this.OpenBadInBrowserCheckBox.UseVisualStyleBackColor = true;
			this.OpenBadInBrowserCheckBox.Visible = false;
			//
			//GroupBox4
			//
			this.GroupBox4.Controls.Add(this.CleanupCheckBox);
			this.GroupBox4.Controls.Add(this.ManuallyAssessCheckBox);
			this.GroupBox4.Location = new System.Drawing.Point(144, 81);
			this.GroupBox4.Name = "GroupBox4";
			this.GroupBox4.Size = new System.Drawing.Size(132, 35);
			this.GroupBox4.TabIndex = 41;
			this.GroupBox4.TabStop = false;
			this.GroupBox4.Text = "Wikipedia Assessments";
			//
			//PluginMenuStrip
			//
			this.PluginMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.PluginMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
				this.PluginToolStripMenuItem,
				this.MenuAbout,
				this.MenuHelp
			});
			this.PluginMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.PluginMenuStrip.Name = "PluginMenuStrip";
			this.PluginMenuStrip.Size = new System.Drawing.Size(442, 24);
			this.PluginMenuStrip.TabIndex = 42;
			this.PluginMenuStrip.Text = "MenuStrip1";
			this.PluginMenuStrip.Visible = false;
			//
			//PluginToolStripMenuItem
			//
			this.PluginToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.SetAWBToolStripMenuItem });
			this.PluginToolStripMenuItem.Name = "PluginToolStripMenuItem";
			this.PluginToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
			this.PluginToolStripMenuItem.Text = "Kingbotk Plugin";
			//
			//SetAWBToolStripMenuItem
			//
			this.SetAWBToolStripMenuItem.Name = "SetAWBToolStripMenuItem";
			this.SetAWBToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.SetAWBToolStripMenuItem.Text = "Set AWB";
			this.SetAWBToolStripMenuItem.ToolTipText = "Reset AWB to default values suitable for use with the plugin";
			//
			//MenuAbout
			//
			this.MenuAbout.Name = "MenuAbout";
			this.MenuAbout.Size = new System.Drawing.Size(160, 20);
			this.MenuAbout.Text = "About the Kingbotk plugin";
			//
			//MenuHelp
			//
			this.MenuHelp.Name = "MenuHelp";
			this.MenuHelp.Size = new System.Drawing.Size(170, 20);
			this.MenuHelp.Text = "Help for the Kingbotk plugin";
			//
			//BotTimer
			//
			this.BotTimer.Interval = 600000;
			//
			//TimerStats1
			//
			this.TimerStats1.Location = new System.Drawing.Point(3, 139);
			this.TimerStats1.MaximumSize = new System.Drawing.Size(63, 70);
			this.TimerStats1.Name = "TimerStats1";
			this.TimerStats1.Size = new System.Drawing.Size(61, 68);
			this.TimerStats1.TabIndex = 44;
			this.TimerStats1.Visible = false;
			//
			//PluginSettingsControl
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.Controls.Add(this.OpenBadInBrowserCheckBox);
			this.Controls.Add(this.ResetTimerButton);
			this.Controls.Add(this.ETALabel);
			this.Controls.Add(this.GroupBox2);
			this.Controls.Add(this.lblAWBNudges);
			this.Controls.Add(this.GroupBox4);
			this.Controls.Add(this.SkipNoChangesCheckBox);
			this.Controls.Add(this.PluginMenuStrip);
			this.Controls.Add(this.SkipBadTagsCheckBox);
			this.Controls.Add(this.Label7);
			this.Name = "PluginSettingsControl";
			this.Size = new System.Drawing.Size(276, 349);
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox2.PerformLayout();
			this.GroupBox4.ResumeLayout(false);
			this.GroupBox4.PerformLayout();
			this.PluginMenuStrip.ResumeLayout(false);
			this.PluginMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
        #endregion

        internal System.Windows.Forms.ToolTip ToolTip1;
		internal System.Windows.Forms.GroupBox GroupBox2;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label lblSkipped;
		internal System.Windows.Forms.Label lblNoChange;
		internal System.Windows.Forms.Label lblBadTag;
		internal System.Windows.Forms.Label lblNamespace;
		internal System.Windows.Forms.Label lblTagged;
        internal System.Windows.Forms.CheckBox ManuallyAssessCheckBox;
		internal System.Windows.Forms.CheckBox CleanupCheckBox;
		internal System.Windows.Forms.GroupBox GroupBox4;
		internal System.Windows.Forms.MenuStrip PluginMenuStrip;
		internal System.Windows.Forms.ToolStripMenuItem PluginToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem MenuAbout;
        internal System.Windows.Forms.ToolStripMenuItem MenuHelp;
		internal AutoWikiBrowser.Plugins.Kingbotk.Components.TimerStats TimerStats1;
		internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.CheckBox SkipNoChangesCheckBox;
        internal System.Windows.Forms.CheckBox SkipBadTagsCheckBox;
		internal System.Windows.Forms.Label Label9;
		internal System.Windows.Forms.Label lblRedlink;
		internal System.Windows.Forms.Label lblAWBNudges;
        internal System.Windows.Forms.Button ResetTimerButton;
		internal System.Windows.Forms.Timer BotTimer;
		private System.Windows.Forms.Label ETALabel;
        internal System.Windows.Forms.ToolStripMenuItem SetAWBToolStripMenuItem;
		internal System.Windows.Forms.CheckBox OpenBadInBrowserCheckBox;
	}

}
