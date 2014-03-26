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
namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
{
	partial class GenericTemplateSettings : System.Windows.Forms.UserControl
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
			this.StubClassCheckBox = new System.Windows.Forms.CheckBox();
			this.AutoStubCheckBox = new System.Windows.Forms.CheckBox();
			this.TemplateNameTextBox = new System.Windows.Forms.TextBox();
			this.AlternateNamesTextBox = new System.Windows.Forms.TextBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.AutoStubSupportYNCheckBox = new System.Windows.Forms.CheckBox();
			this.SkipRegexTextBox = new System.Windows.Forms.TextBox();
			this.SkipRegexCheckBox = new System.Windows.Forms.CheckBox();
			this.GetRedirectsButton = new System.Windows.Forms.Button();
			this.HasAlternateNamesCheckBox = new System.Windows.Forms.CheckBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.LinkLabel1 = new System.Windows.Forms.LinkLabel();
			this.GroupBox2 = new System.Windows.Forms.GroupBox();
			this.TipLabel = new System.Windows.Forms.Label();
			this.GroupBox3 = new System.Windows.Forms.GroupBox();
			this.ImportanceCheckedListBox = new System.Windows.Forms.CheckedListBox();
			this.PropertiesButton = new System.Windows.Forms.Button();
			this.Timer1 = new System.Windows.Forms.Timer(this.components);
			this.GroupBox2.SuspendLayout();
			this.GroupBox3.SuspendLayout();
			this.SuspendLayout();
			//
			//StubClassCheckBox
			//
			this.StubClassCheckBox.AutoSize = true;
			this.StubClassCheckBox.Location = new System.Drawing.Point(85, 19);
			this.StubClassCheckBox.Name = "StubClassCheckBox";
			this.StubClassCheckBox.Size = new System.Drawing.Size(76, 17);
			this.StubClassCheckBox.TabIndex = 5;
			this.StubClassCheckBox.Text = "Stub-Class";
			this.ToolTip1.SetToolTip(this.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)");
			this.StubClassCheckBox.UseVisualStyleBackColor = true;
			//
			//AutoStubCheckBox
			//
			this.AutoStubCheckBox.AutoSize = true;
			this.AutoStubCheckBox.Enabled = false;
			this.AutoStubCheckBox.Location = new System.Drawing.Point(6, 19);
			this.AutoStubCheckBox.Name = "AutoStubCheckBox";
			this.AutoStubCheckBox.Size = new System.Drawing.Size(73, 17);
			this.AutoStubCheckBox.TabIndex = 4;
			this.AutoStubCheckBox.Text = "Auto-Stub";
			this.ToolTip1.SetToolTip(this.AutoStubCheckBox, "class=Stub|auto=yes");
			this.AutoStubCheckBox.UseVisualStyleBackColor = true;
			//
			//TemplateNameTextBox
			//
			this.TemplateNameTextBox.Location = new System.Drawing.Point(109, 9);
			this.TemplateNameTextBox.Name = "TemplateNameTextBox";
			this.TemplateNameTextBox.Size = new System.Drawing.Size(90, 20);
			this.TemplateNameTextBox.TabIndex = 0;
			this.ToolTip1.SetToolTip(this.TemplateNameTextBox, "The usual (preferred) name of the template. e.g. {{Target}}");
			//
			//AlternateNamesTextBox
			//
			this.AlternateNamesTextBox.Enabled = false;
			this.AlternateNamesTextBox.Location = new System.Drawing.Point(84, 58);
			this.AlternateNamesTextBox.Name = "AlternateNamesTextBox";
			this.AlternateNamesTextBox.Size = new System.Drawing.Size(136, 20);
			this.AlternateNamesTextBox.TabIndex = 2;
			this.ToolTip1.SetToolTip(this.AlternateNamesTextBox, "Enter the alternate names of the template. If there is more than one alternate na" + "me seperate them with a vertical bar | and NO SPACES, e.g. WikiProjectBiography|" + "BiographyWikiProject banner");
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(3, 16);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(60, 13);
			this.Label3.TabIndex = 11;
			this.Label3.Text = "Importance";
			this.ToolTip1.SetToolTip(this.Label3, "The name of your importance= parameter (importance, priority, or not supported)");
			//
			//AutoStubSupportYNCheckBox
			//
			this.AutoStubSupportYNCheckBox.AutoSize = true;
			this.AutoStubSupportYNCheckBox.Location = new System.Drawing.Point(168, 35);
			this.AutoStubSupportYNCheckBox.Name = "AutoStubSupportYNCheckBox";
			this.AutoStubSupportYNCheckBox.Size = new System.Drawing.Size(73, 17);
			this.AutoStubSupportYNCheckBox.TabIndex = 15;
			this.AutoStubSupportYNCheckBox.Text = "Auto-Stub";
			this.ToolTip1.SetToolTip(this.AutoStubSupportYNCheckBox, "Do you have an auto=yes parameter?");
			this.AutoStubSupportYNCheckBox.UseVisualStyleBackColor = true;
			//
			//SkipRegexTextBox
			//
			this.SkipRegexTextBox.Enabled = false;
			this.SkipRegexTextBox.Location = new System.Drawing.Point(97, 58);
			this.SkipRegexTextBox.Name = "SkipRegexTextBox";
			this.SkipRegexTextBox.Size = new System.Drawing.Size(144, 20);
			this.SkipRegexTextBox.TabIndex = 9;
			this.ToolTip1.SetToolTip(this.SkipRegexTextBox, "Advanced. Enter a REGULAR EXPRESSION, and the plugin will skip if the talk page c" + "ontains it.");
			//
			//SkipRegexCheckBox
			//
			this.SkipRegexCheckBox.AutoSize = true;
			this.SkipRegexCheckBox.Location = new System.Drawing.Point(97, 35);
			this.SkipRegexCheckBox.Name = "SkipRegexCheckBox";
			this.SkipRegexCheckBox.Size = new System.Drawing.Size(65, 17);
			this.SkipRegexCheckBox.TabIndex = 16;
			this.SkipRegexCheckBox.Text = "Skip RE";
			this.ToolTip1.SetToolTip(this.SkipRegexCheckBox, "Check this if you want to supply a regular expression to tell the plugin when to " + "skip pages");
			this.SkipRegexCheckBox.UseVisualStyleBackColor = true;
			//
			//GetRedirectsButton
			//
			this.GetRedirectsButton.Enabled = false;
			this.GetRedirectsButton.Location = new System.Drawing.Point(226, 56);
			this.GetRedirectsButton.Name = "GetRedirectsButton";
			this.GetRedirectsButton.Size = new System.Drawing.Size(33, 23);
			this.GetRedirectsButton.TabIndex = 5;
			this.GetRedirectsButton.Text = "Get";
			this.GetRedirectsButton.UseVisualStyleBackColor = true;
			//
			//HasAlternateNamesCheckBox
			//
			this.HasAlternateNamesCheckBox.AutoSize = true;
			this.HasAlternateNamesCheckBox.Location = new System.Drawing.Point(30, 35);
			this.HasAlternateNamesCheckBox.Name = "HasAlternateNamesCheckBox";
			this.HasAlternateNamesCheckBox.Size = new System.Drawing.Size(217, 17);
			this.HasAlternateNamesCheckBox.TabIndex = 4;
			this.HasAlternateNamesCheckBox.Text = "Template has alternate names (redirects)";
			this.HasAlternateNamesCheckBox.UseVisualStyleBackColor = true;
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(18, 58);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(49, 26);
			this.Label2.TabIndex = 3;
            this.Label2.Text = "Alternate" + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10) + "Names";
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(23, 12);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(82, 13);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Template Name";
			//
			//LinkLabel1
			//
			this.LinkLabel1.AutoSize = true;
			this.LinkLabel1.Location = new System.Drawing.Point(225, 12);
			this.LinkLabel1.Name = "LinkLabel1";
			this.LinkLabel1.Size = new System.Drawing.Size(29, 13);
			this.LinkLabel1.TabIndex = 2;
			this.LinkLabel1.TabStop = true;
			this.LinkLabel1.Text = "Help";
			//
			//GroupBox2
			//
			this.GroupBox2.Controls.Add(this.StubClassCheckBox);
			this.GroupBox2.Controls.Add(this.AutoStubCheckBox);
			this.GroupBox2.Location = new System.Drawing.Point(18, 180);
			this.GroupBox2.Name = "GroupBox2";
			this.GroupBox2.Size = new System.Drawing.Size(171, 45);
			this.GroupBox2.TabIndex = 2;
			this.GroupBox2.TabStop = false;
			this.GroupBox2.Text = "Configuration";
			//
			//TipLabel
			//
			this.TipLabel.AutoSize = true;
			this.TipLabel.Location = new System.Drawing.Point(15, 228);
			this.TipLabel.Name = "TipLabel";
			this.TipLabel.Size = new System.Drawing.Size(255, 39);
			this.TipLabel.TabIndex = 7;
            this.TipLabel.Text = "Tip: The plugin also adds parameter insertion options" + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10) + "to the context menu of the" + " edit box. Just right" + char.ConvertFromUtf32(13) + char.ConvertFromUtf32(10) + "click inside the edit box to access them.";
			this.TipLabel.Visible = false;
			//
			//GroupBox3
			//
			this.GroupBox3.Controls.Add(this.SkipRegexCheckBox);
			this.GroupBox3.Controls.Add(this.SkipRegexTextBox);
			this.GroupBox3.Controls.Add(this.AutoStubSupportYNCheckBox);
			this.GroupBox3.Controls.Add(this.Label3);
			this.GroupBox3.Controls.Add(this.ImportanceCheckedListBox);
			this.GroupBox3.Location = new System.Drawing.Point(12, 87);
			this.GroupBox3.Name = "GroupBox3";
			this.GroupBox3.Size = new System.Drawing.Size(252, 87);
			this.GroupBox3.TabIndex = 8;
			this.GroupBox3.TabStop = false;
			this.GroupBox3.Text = "Template Properties";
			//
			//ImportanceCheckedListBox
			//
			this.ImportanceCheckedListBox.CheckOnClick = true;
			this.ImportanceCheckedListBox.FormattingEnabled = true;
			this.ImportanceCheckedListBox.Items.AddRange(new object[] {
				"Importance",
				"Priority",
				"None"
			});
			this.ImportanceCheckedListBox.Location = new System.Drawing.Point(6, 29);
			this.ImportanceCheckedListBox.Name = "ImportanceCheckedListBox";
			this.ImportanceCheckedListBox.Size = new System.Drawing.Size(86, 49);
			this.ImportanceCheckedListBox.TabIndex = 9;
			//
			//PropertiesButton
			//
			this.PropertiesButton.Location = new System.Drawing.Point(195, 193);
			this.PropertiesButton.Name = "PropertiesButton";
			this.PropertiesButton.Size = new System.Drawing.Size(75, 23);
			this.PropertiesButton.TabIndex = 17;
			this.PropertiesButton.Text = "Properties";
			this.PropertiesButton.UseVisualStyleBackColor = true;
			//
			//GenericTemplateSettings
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.GetRedirectsButton);
			this.Controls.Add(this.HasAlternateNamesCheckBox);
			this.Controls.Add(this.PropertiesButton);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.GroupBox3);
			this.Controls.Add(this.AlternateNamesTextBox);
			this.Controls.Add(this.TipLabel);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.GroupBox2);
			this.Controls.Add(this.LinkLabel1);
			this.Controls.Add(this.TemplateNameTextBox);
			this.MaximumSize = new System.Drawing.Size(276, 349);
			this.MinimumSize = new System.Drawing.Size(276, 349);
			this.Name = "GenericTemplateSettings";
			this.Size = new System.Drawing.Size(276, 349);
			this.GroupBox2.ResumeLayout(false);
			this.GroupBox2.PerformLayout();
			this.GroupBox3.ResumeLayout(false);
			this.GroupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

        #endregion
        internal System.Windows.Forms.ToolTip ToolTip1;
		internal System.Windows.Forms.GroupBox GroupBox2;
		internal System.Windows.Forms.CheckBox StubClassCheckBox;
		internal System.Windows.Forms.CheckBox AutoStubCheckBox;
		internal System.Windows.Forms.Label TipLabel;
		internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TemplateNameTextBox;
        internal System.Windows.Forms.LinkLabel LinkLabel1;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.TextBox AlternateNamesTextBox;
        internal System.Windows.Forms.CheckBox HasAlternateNamesCheckBox;
		internal System.Windows.Forms.GroupBox GroupBox3;
		internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.CheckedListBox ImportanceCheckedListBox;
        internal System.Windows.Forms.CheckBox AutoStubSupportYNCheckBox;
		internal System.Windows.Forms.TextBox SkipRegexTextBox;
        internal System.Windows.Forms.CheckBox SkipRegexCheckBox;
		internal System.Windows.Forms.Button PropertiesButton;
		internal System.Windows.Forms.Timer Timer1;

		internal System.Windows.Forms.Button GetRedirectsButton;
	}
}
