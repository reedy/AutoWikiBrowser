using System;
using System.Windows.Forms;
using System.Xml;

using WikiFunctions;

using WikiFunctions.Plugin;
namespace AutoWikiBrowser.Plugins.Kingbotk.Components
{
	partial class GenericTemplatePropertiesForm : System.Windows.Forms.Form
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
			this.components = new System.ComponentModel.Container();
			this.OK_Button = new System.Windows.Forms.Button();
			this.NameLabel = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.MainRegexTextBox = new System.Windows.Forms.TextBox();
			this.AmIReadyLabel = new System.Windows.Forms.Label();
			this.HasAltNamesLabel = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			this.PreferredTemplateNameRegexTextBox = new System.Windows.Forms.TextBox();
			this.Label3 = new System.Windows.Forms.Label();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SecondChanceRegexTextBox = new System.Windows.Forms.TextBox();
			this.SkipRegexTextBox = new System.Windows.Forms.TextBox();
			this.SkipLabel = new System.Windows.Forms.Label();
			this.ImportanceLabel = new System.Windows.Forms.Label();
			this.AutoStubLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			//
			//OK_Button
			//
			this.OK_Button.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.OK_Button.Location = new System.Drawing.Point(362, 263);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(67, 23);
			this.OK_Button.TabIndex = 0;
			this.OK_Button.Text = "OK";
            this.OK_Button.Click += OK_Button_Click;
			//
			//NameLabel
			//
			this.NameLabel.AutoSize = true;
			this.NameLabel.Location = new System.Drawing.Point(12, 21);
			this.NameLabel.Name = "NameLabel";
			this.NameLabel.Size = new System.Drawing.Size(134, 13);
			this.NameLabel.TabIndex = 1;
			this.NameLabel.Text = "Preferred Template Name: ";
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(12, 67);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(121, 13);
			this.Label1.TabIndex = 2;
			this.Label1.Text = "Main regular expression:";
			//
			//MainRegexTextBox
			//
			this.MainRegexTextBox.Location = new System.Drawing.Point(145, 67);
			this.MainRegexTextBox.Multiline = true;
			this.MainRegexTextBox.Name = "MainRegexTextBox";
			this.MainRegexTextBox.ReadOnly = true;
			this.MainRegexTextBox.Size = new System.Drawing.Size(281, 36);
			this.MainRegexTextBox.TabIndex = 3;
			this.ToolTip1.SetToolTip(this.MainRegexTextBox, "The main regular expression used to search for existing template instances");
			//
			//AmIReadyLabel
			//
			this.AmIReadyLabel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.AmIReadyLabel.AutoSize = true;
			this.AmIReadyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));
			this.AmIReadyLabel.Location = new System.Drawing.Point(12, 268);
			this.AmIReadyLabel.Name = "AmIReadyLabel";
			this.AmIReadyLabel.Size = new System.Drawing.Size(95, 13);
			this.AmIReadyLabel.TabIndex = 4;
			this.AmIReadyLabel.Text = "AmIReadyLabel";
			//
			//HasAltNamesLabel
			//
			this.HasAltNamesLabel.AutoSize = true;
			this.HasAltNamesLabel.Location = new System.Drawing.Point(12, 43);
			this.HasAltNamesLabel.Name = "HasAltNamesLabel";
			this.HasAltNamesLabel.Size = new System.Drawing.Size(204, 13);
			this.HasAltNamesLabel.TabIndex = 5;
			this.HasAltNamesLabel.Text = "Template has alternate names (redirects): ";
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(12, 112);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(221, 13);
			this.Label2.TabIndex = 6;
			this.Label2.Text = "Template name-checking regular expression: ";
			//
			//PreferredTemplateNameRegexTextBox
			//
			this.PreferredTemplateNameRegexTextBox.Location = new System.Drawing.Point(239, 112);
			this.PreferredTemplateNameRegexTextBox.Name = "PreferredTemplateNameRegexTextBox";
			this.PreferredTemplateNameRegexTextBox.ReadOnly = true;
			this.PreferredTemplateNameRegexTextBox.Size = new System.Drawing.Size(187, 20);
			this.PreferredTemplateNameRegexTextBox.TabIndex = 7;
			this.ToolTip1.SetToolTip(this.PreferredTemplateNameRegexTextBox, "The regex used to check that the template instance bears the preferred name");
			//
			//Label3
			//
			this.Label3.AutoSize = true;
			this.Label3.Location = new System.Drawing.Point(12, 146);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(174, 13);
			this.Label3.TabIndex = 8;
			this.Label3.Text = "Second-chance regular expression:";
			//
			//SecondChanceRegexTextBox
			//
			this.SecondChanceRegexTextBox.Location = new System.Drawing.Point(192, 146);
			this.SecondChanceRegexTextBox.Multiline = true;
			this.SecondChanceRegexTextBox.Name = "SecondChanceRegexTextBox";
			this.SecondChanceRegexTextBox.ReadOnly = true;
			this.SecondChanceRegexTextBox.Size = new System.Drawing.Size(234, 36);
			this.SecondChanceRegexTextBox.TabIndex = 9;
			this.ToolTip1.SetToolTip(this.SecondChanceRegexTextBox, "Looser regex used to search for bad tags");
			//
			//SkipRegexTextBox
			//
			this.SkipRegexTextBox.Location = new System.Drawing.Point(192, 191);
			this.SkipRegexTextBox.Multiline = true;
			this.SkipRegexTextBox.Name = "SkipRegexTextBox";
			this.SkipRegexTextBox.ReadOnly = true;
			this.SkipRegexTextBox.Size = new System.Drawing.Size(234, 36);
			this.SkipRegexTextBox.TabIndex = 11;
			this.ToolTip1.SetToolTip(this.SkipRegexTextBox, "We skip if the talk page contains a match for this regex");
			//
			//SkipLabel
			//
			this.SkipLabel.AutoSize = true;
			this.SkipLabel.Location = new System.Drawing.Point(12, 191);
			this.SkipLabel.Name = "SkipLabel";
			this.SkipLabel.Size = new System.Drawing.Size(88, 13);
			this.SkipLabel.TabIndex = 10;
			this.SkipLabel.Text = "Skip if contains:  ";
			//
			//ImportanceLabel
			//
			this.ImportanceLabel.AutoSize = true;
			this.ImportanceLabel.Location = new System.Drawing.Point(12, 237);
			this.ImportanceLabel.Name = "ImportanceLabel";
			this.ImportanceLabel.Size = new System.Drawing.Size(116, 13);
			this.ImportanceLabel.TabIndex = 12;
			this.ImportanceLabel.Text = "Importance parameter: ";
			//
			//AutoStubLabel
			//
			this.AutoStubLabel.AutoSize = true;
			this.AutoStubLabel.Location = new System.Drawing.Point(236, 237);
			this.AutoStubLabel.Name = "AutoStubLabel";
			this.AutoStubLabel.Size = new System.Drawing.Size(110, 13);
			this.AutoStubLabel.TabIndex = 13;
			this.AutoStubLabel.Text = "Auto-Stub parameter: ";
			//
			//GenericTemplatePropertiesForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(441, 298);
			this.Controls.Add(this.AutoStubLabel);
			this.Controls.Add(this.ImportanceLabel);
			this.Controls.Add(this.SkipRegexTextBox);
			this.Controls.Add(this.SkipLabel);
			this.Controls.Add(this.SecondChanceRegexTextBox);
			this.Controls.Add(this.Label3);
			this.Controls.Add(this.PreferredTemplateNameRegexTextBox);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.HasAltNamesLabel);
			this.Controls.Add(this.AmIReadyLabel);
			this.Controls.Add(this.MainRegexTextBox);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.NameLabel);
			this.Controls.Add(this.OK_Button);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GenericTemplatePropertiesForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generic Template Properties";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
        #endregion

        internal System.Windows.Forms.Button OK_Button;
		internal System.Windows.Forms.Label NameLabel;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.TextBox MainRegexTextBox;
		internal System.Windows.Forms.Label AmIReadyLabel;
		internal System.Windows.Forms.Label HasAltNamesLabel;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.TextBox PreferredTemplateNameRegexTextBox;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.ToolTip ToolTip1;
		internal System.Windows.Forms.TextBox SecondChanceRegexTextBox;
		internal System.Windows.Forms.Label SkipLabel;
		internal System.Windows.Forms.TextBox SkipRegexTextBox;
		internal System.Windows.Forms.Label ImportanceLabel;
		internal System.Windows.Forms.Label AutoStubLabel;
	}
}
