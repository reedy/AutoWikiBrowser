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
            this.btnClose = new System.Windows.Forms.Button();
            this.chkNoUnicode = new System.Windows.Forms.CheckBox();
            this.chkNoTag = new System.Windows.Forms.CheckBox();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.chkUserTalkTemplates = new System.Windows.Forms.CheckBox();
            this.chkCiteTemplateDates = new System.Windows.Forms.CheckBox();
            this.chkDefaultSortAdded = new System.Windows.Forms.CheckBox();
            this.chkNoBadLink = new System.Windows.Forms.CheckBox();
            this.chkNoBulletedLink = new System.Windows.Forms.CheckBox();
            this.chkNoBoldTitle = new System.Windows.Forms.CheckBox();
            this.chkNoHeaderError = new System.Windows.Forms.CheckBox();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(101, 325);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkNoUnicode
            // 
            this.chkNoUnicode.AutoSize = true;
            this.chkNoUnicode.Location = new System.Drawing.Point(6, 88);
            this.chkNoUnicode.Name = "chkNoUnicode";
            this.chkNoUnicode.Size = new System.Drawing.Size(96, 17);
            this.chkNoUnicode.TabIndex = 4;
            this.chkNoUnicode.Tag = 1;
            this.chkNoUnicode.Text = "Unicodification";
            this.chkNoUnicode.UseVisualStyleBackColor = true;
            // 
            // chkNoTag
            // 
            this.chkNoTag.AutoSize = true;
            this.chkNoTag.Location = new System.Drawing.Point(6, 111);
            this.chkNoTag.Name = "chkNoTag";
            this.chkNoTag.Size = new System.Drawing.Size(90, 17);
            this.chkNoTag.TabIndex = 5;
            this.chkNoTag.Tag = 2;
            this.chkNoTag.Text = "Tag changed";
            this.chkNoTag.UseVisualStyleBackColor = true;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.chkUserTalkTemplates);
            this.gbOptions.Controls.Add(this.chkDefaultSortAdded);
            this.gbOptions.Controls.Add(this.chkNoBadLink);
            this.gbOptions.Controls.Add(this.chkNoBulletedLink);
            this.gbOptions.Controls.Add(this.chkNoBoldTitle);
            this.gbOptions.Controls.Add(this.chkNoHeaderError);
            this.gbOptions.Controls.Add(this.chkNoTag);
            this.gbOptions.Controls.Add(this.chkNoUnicode);
            this.gbOptions.Controls.Add(this.chkCiteTemplateDates);
            this.gbOptions.Location = new System.Drawing.Point(12, 12);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(164, 307);
            this.gbOptions.TabIndex = 0;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Skip if no...";
            // 
            // chkUserTalkTemplates
            // 
            this.chkUserTalkTemplates.AutoSize = true;
            this.chkUserTalkTemplates.Location = new System.Drawing.Point(6, 180);
            this.chkUserTalkTemplates.Name = "chkUserTalkTemplates";
            this.chkUserTalkTemplates.Size = new System.Drawing.Size(152, 17);
            this.chkUserTalkTemplates.TabIndex = 8;
            this.chkUserTalkTemplates.Tag = 8;
            this.chkUserTalkTemplates.Text = "User talk templates subst\'d";
            this.chkUserTalkTemplates.UseVisualStyleBackColor = true;
            // 
            // cCiteTemplateDates
            // 
            this.chkCiteTemplateDates.AutoSize = true;
            this.chkCiteTemplateDates.Location = new System.Drawing.Point(6, 203);
            this.chkCiteTemplateDates.Name = "chkCiteTemplateDates";
            this.chkCiteTemplateDates.Size = new System.Drawing.Size(152, 17);
            this.chkCiteTemplateDates.TabIndex = 9;
            this.chkCiteTemplateDates.Tag = 9;
            this.chkCiteTemplateDates.Text = "Citation template dates fixed";
            this.chkCiteTemplateDates.UseVisualStyleBackColor = true;
            // 
            // chkDefaultSortAdded
            // 
            this.chkDefaultSortAdded.AutoSize = true;
            this.chkDefaultSortAdded.Location = new System.Drawing.Point(6, 157);
            this.chkDefaultSortAdded.Name = "chkDefaultSortAdded";
            this.chkDefaultSortAdded.Size = new System.Drawing.Size(124, 17);
            this.chkDefaultSortAdded.TabIndex = 7;
            this.chkDefaultSortAdded.Tag = 3;
            this.chkDefaultSortAdded.Text = "{{defaultsort}} added";
            this.chkDefaultSortAdded.UseVisualStyleBackColor = true;
            // 
            // chkNoBadLink
            // 
            this.chkNoBadLink.AutoSize = true;
            this.chkNoBadLink.Location = new System.Drawing.Point(6, 65);
            this.chkNoBadLink.Name = "chkNoBadLink";
            this.chkNoBadLink.Size = new System.Drawing.Size(94, 17);
            this.chkNoBadLink.TabIndex = 3;
            this.chkNoBadLink.Tag = 6;
            this.chkNoBadLink.Text = "Bad links fixed";
            this.chkNoBadLink.UseVisualStyleBackColor = true;
            // 
            // chkNoBulletedLink
            // 
            this.chkNoBulletedLink.AutoSize = true;
            this.chkNoBulletedLink.Location = new System.Drawing.Point(6, 42);
            this.chkNoBulletedLink.Name = "chkNoBulletedLink";
            this.chkNoBulletedLink.Size = new System.Drawing.Size(123, 17);
            this.chkNoBulletedLink.TabIndex = 2;
            this.chkNoBulletedLink.Tag = 5;
            this.chkNoBulletedLink.Text = "External link bulleted";
            this.chkNoBulletedLink.UseVisualStyleBackColor = true;
            // 
            // chkNoBoldTitle
            // 
            this.chkNoBoldTitle.AutoSize = true;
            this.chkNoBoldTitle.Location = new System.Drawing.Point(6, 19);
            this.chkNoBoldTitle.Name = "chkNoBoldTitle";
            this.chkNoBoldTitle.Size = new System.Drawing.Size(93, 17);
            this.chkNoBoldTitle.TabIndex = 1;
            this.chkNoBoldTitle.Tag = 4;
            this.chkNoBoldTitle.Text = "Title boldened";
            this.chkNoBoldTitle.UseVisualStyleBackColor = true;
            // 
            // chkNoHeaderError
            // 
            this.chkNoHeaderError.AutoSize = true;
            this.chkNoHeaderError.Location = new System.Drawing.Point(6, 134);
            this.chkNoHeaderError.Name = "chkNoHeaderError";
            this.chkNoHeaderError.Size = new System.Drawing.Size(110, 17);
            this.chkNoHeaderError.TabIndex = 6;
            this.chkNoHeaderError.Tag = 3;
            this.chkNoHeaderError.Text = "Header error fixed";
            this.chkNoHeaderError.UseVisualStyleBackColor = true;
            // 
            // SkipOptions
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(188, 355);
            this.Controls.Add(this.gbOptions);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skip options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkNoUnicode;
        private System.Windows.Forms.CheckBox chkNoTag;
        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox chkNoHeaderError;
        private System.Windows.Forms.CheckBox chkNoBoldTitle;
        private System.Windows.Forms.CheckBox chkNoBulletedLink;
        private System.Windows.Forms.CheckBox chkNoBadLink;
        private System.Windows.Forms.CheckBox chkDefaultSortAdded;
        private System.Windows.Forms.CheckBox chkUserTalkTemplates;
        private System.Windows.Forms.CheckBox chkCiteTemplateDates;
    }
}