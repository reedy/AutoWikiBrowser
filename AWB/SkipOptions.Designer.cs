namespace AutoWikiBrowser
{
    partial class SkipOptions
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
            this.components = new System.ComponentModel.Container();
            this.btnClose = new System.Windows.Forms.Button();
            this.CheckAll = new System.Windows.Forms.CheckBox();
            this.CheckNone = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CheckBoxPanel = new System.Windows.Forms.Panel();
            this.chkUserTalkTemplates = new System.Windows.Forms.CheckBox();
            this.chkDefaultSortAdded = new System.Windows.Forms.CheckBox();
            this.chkNoBadLink = new System.Windows.Forms.CheckBox();
            this.chkNoBoldTitle = new System.Windows.Forms.CheckBox();
            this.chkNoBulletedLink = new System.Windows.Forms.CheckBox();
            this.chkPeopleCategories = new System.Windows.Forms.CheckBox();
            this.chkCiteTemplateDates = new System.Windows.Forms.CheckBox();
            this.chkNoHeaderError = new System.Windows.Forms.CheckBox();
            this.chkNoUnicode = new System.Windows.Forms.CheckBox();
            this.chkNoTag = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.CheckBoxPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(100, 274);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CheckAll
            // 
            this.CheckAll.AutoSize = true;
            this.CheckAll.Location = new System.Drawing.Point(56, 8);
            this.CheckAll.Name = "CheckAll";
            this.CheckAll.Size = new System.Drawing.Size(36, 17);
            this.CheckAll.TabIndex = 11;
            this.CheckAll.Text = "all";
            this.CheckAll.UseVisualStyleBackColor = true;
            this.CheckAll.CheckedChanged += new System.EventHandler(this.CheckAll_CheckedChanged);
            // 
            // CheckNone
            // 
            this.CheckNone.AutoSize = true;
            this.CheckNone.Location = new System.Drawing.Point(98, 8);
            this.CheckNone.Name = "CheckNone";
            this.CheckNone.Size = new System.Drawing.Size(50, 17);
            this.CheckNone.TabIndex = 12;
            this.CheckNone.Text = "none";
            this.CheckNone.UseVisualStyleBackColor = true;
            this.CheckNone.CheckedChanged += new System.EventHandler(this.CheckNone_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Check";
            // 
            // CheckBoxPanel
            // 
            this.CheckBoxPanel.Controls.Add(this.chkUserTalkTemplates);
            this.CheckBoxPanel.Controls.Add(this.chkDefaultSortAdded);
            this.CheckBoxPanel.Controls.Add(this.chkNoBadLink);
            this.CheckBoxPanel.Controls.Add(this.chkNoBoldTitle);
            this.CheckBoxPanel.Controls.Add(this.chkNoBulletedLink);
            this.CheckBoxPanel.Controls.Add(this.chkPeopleCategories);
            this.CheckBoxPanel.Controls.Add(this.chkCiteTemplateDates);
            this.CheckBoxPanel.Controls.Add(this.chkNoHeaderError);
            this.CheckBoxPanel.Controls.Add(this.chkNoUnicode);
            this.CheckBoxPanel.Controls.Add(this.chkNoTag);
            this.CheckBoxPanel.Location = new System.Drawing.Point(12, 31);
            this.CheckBoxPanel.Name = "CheckBoxPanel";
            this.CheckBoxPanel.Size = new System.Drawing.Size(163, 237);
            this.CheckBoxPanel.TabIndex = 14;
            // 
            // chkUserTalkTemplates
            // 
            this.chkUserTalkTemplates.AutoSize = true;
            this.chkUserTalkTemplates.Location = new System.Drawing.Point(3, 164);
            this.chkUserTalkTemplates.Name = "chkUserTalkTemplates";
            this.chkUserTalkTemplates.Size = new System.Drawing.Size(152, 17);
            this.chkUserTalkTemplates.TabIndex = 18;
            this.chkUserTalkTemplates.Tag = 8;
            this.chkUserTalkTemplates.Text = "User talk templates subst\'d";
            this.chkUserTalkTemplates.UseVisualStyleBackColor = true;
            // 
            // chkDefaultSortAdded
            // 
            this.chkDefaultSortAdded.AutoSize = true;
            this.chkDefaultSortAdded.Location = new System.Drawing.Point(3, 141);
            this.chkDefaultSortAdded.Name = "chkDefaultSortAdded";
            this.chkDefaultSortAdded.Size = new System.Drawing.Size(124, 17);
            this.chkDefaultSortAdded.TabIndex = 17;
            this.chkDefaultSortAdded.Tag = 3;
            this.chkDefaultSortAdded.Text = "{{defaultsort}} added";
            this.chkDefaultSortAdded.UseVisualStyleBackColor = true;
            // 
            // chkNoBadLink
            // 
            this.chkNoBadLink.AutoSize = true;
            this.chkNoBadLink.Location = new System.Drawing.Point(3, 49);
            this.chkNoBadLink.Name = "chkNoBadLink";
            this.chkNoBadLink.Size = new System.Drawing.Size(94, 17);
            this.chkNoBadLink.TabIndex = 13;
            this.chkNoBadLink.Tag = 6;
            this.chkNoBadLink.Text = "Bad links fixed";
            this.chkNoBadLink.UseVisualStyleBackColor = true;
            // 
            // chkNoBoldTitle
            // 
            this.chkNoBoldTitle.AutoSize = true;
            this.chkNoBoldTitle.Location = new System.Drawing.Point(3, 3);
            this.chkNoBoldTitle.Name = "chkNoBoldTitle";
            this.chkNoBoldTitle.Size = new System.Drawing.Size(93, 17);
            this.chkNoBoldTitle.TabIndex = 11;
            this.chkNoBoldTitle.Tag = 4;
            this.chkNoBoldTitle.Text = "Title boldened";
            this.chkNoBoldTitle.UseVisualStyleBackColor = true;
            // 
            // chkNoBulletedLink
            // 
            this.chkNoBulletedLink.AutoSize = true;
            this.chkNoBulletedLink.Location = new System.Drawing.Point(3, 26);
            this.chkNoBulletedLink.Name = "chkNoBulletedLink";
            this.chkNoBulletedLink.Size = new System.Drawing.Size(123, 17);
            this.chkNoBulletedLink.TabIndex = 12;
            this.chkNoBulletedLink.Tag = 5;
            this.chkNoBulletedLink.Text = "External link bulleted";
            this.chkNoBulletedLink.UseVisualStyleBackColor = true;
            // 
            // chkPeopleCategories
            // 
            this.chkPeopleCategories.AutoSize = true;
            this.chkPeopleCategories.Location = new System.Drawing.Point(3, 210);
            this.chkPeopleCategories.Name = "chkPeopleCategories";
            this.chkPeopleCategories.Size = new System.Drawing.Size(148, 17);
            this.chkPeopleCategories.TabIndex = 20;
            this.chkPeopleCategories.Tag = 10;
            this.chkPeopleCategories.Text = "Human category changes";
            this.chkPeopleCategories.UseVisualStyleBackColor = true;
            // 
            // chkCiteTemplateDates
            // 
            this.chkCiteTemplateDates.AutoSize = true;
            this.chkCiteTemplateDates.Location = new System.Drawing.Point(3, 187);
            this.chkCiteTemplateDates.Name = "chkCiteTemplateDates";
            this.chkCiteTemplateDates.Size = new System.Drawing.Size(158, 17);
            this.chkCiteTemplateDates.TabIndex = 19;
            this.chkCiteTemplateDates.Tag = 9;
            this.chkCiteTemplateDates.Text = "Citation template dates fixed";
            this.chkCiteTemplateDates.UseVisualStyleBackColor = true;
            // 
            // chkNoHeaderError
            // 
            this.chkNoHeaderError.AutoSize = true;
            this.chkNoHeaderError.Location = new System.Drawing.Point(3, 118);
            this.chkNoHeaderError.Name = "chkNoHeaderError";
            this.chkNoHeaderError.Size = new System.Drawing.Size(110, 17);
            this.chkNoHeaderError.TabIndex = 16;
            this.chkNoHeaderError.Tag = 3;
            this.chkNoHeaderError.Text = "Header error fixed";
            this.chkNoHeaderError.UseVisualStyleBackColor = true;
            // 
            // chkNoUnicode
            // 
            this.chkNoUnicode.AutoSize = true;
            this.chkNoUnicode.Location = new System.Drawing.Point(3, 72);
            this.chkNoUnicode.Name = "chkNoUnicode";
            this.chkNoUnicode.Size = new System.Drawing.Size(96, 17);
            this.chkNoUnicode.TabIndex = 14;
            this.chkNoUnicode.Tag = 1;
            this.chkNoUnicode.Text = "Unicodification";
            this.chkNoUnicode.UseVisualStyleBackColor = true;
            // 
            // chkNoTag
            // 
            this.chkNoTag.AutoSize = true;
            this.chkNoTag.Location = new System.Drawing.Point(3, 95);
            this.chkNoTag.Name = "chkNoTag";
            this.chkNoTag.Size = new System.Drawing.Size(110, 17);
            this.chkNoTag.TabIndex = 15;
            this.chkNoTag.Tag = 2;
            this.chkNoTag.Text = "Auto tag changes";
            this.chkNoTag.UseVisualStyleBackColor = true;
            // 
            // SkipOptions
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(187, 303);
            this.Controls.Add(this.CheckBoxPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CheckNone);
            this.Controls.Add(this.CheckAll);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skip if no...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.CheckBoxPanel.ResumeLayout(false);
            this.CheckBoxPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox CheckAll;
        private System.Windows.Forms.CheckBox CheckNone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel CheckBoxPanel;
        private System.Windows.Forms.CheckBox chkUserTalkTemplates;
        private System.Windows.Forms.CheckBox chkDefaultSortAdded;
        private System.Windows.Forms.CheckBox chkNoBadLink;
        private System.Windows.Forms.CheckBox chkNoBoldTitle;
        private System.Windows.Forms.CheckBox chkNoBulletedLink;
        private System.Windows.Forms.CheckBox chkPeopleCategories;
        private System.Windows.Forms.CheckBox chkCiteTemplateDates;
        private System.Windows.Forms.CheckBox chkNoHeaderError;
        private System.Windows.Forms.CheckBox chkNoUnicode;
        private System.Windows.Forms.CheckBox chkNoTag;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}