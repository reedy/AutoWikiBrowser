namespace AutoWikiBrowser
{
    partial class specialFilter
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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCategory = new System.Windows.Forms.CheckBox();
            this.chkTemplate = new System.Windows.Forms.CheckBox();
            this.chkWikipedia = new System.Windows.Forms.CheckBox();
            this.chkUser = new System.Windows.Forms.CheckBox();
            this.chkImage = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.chkNotContains = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.chkArticle = new System.Windows.Forms.CheckBox();
            this.chkArticleTalk = new System.Windows.Forms.CheckBox();
            this.chkUserTalk = new System.Windows.Forms.CheckBox();
            this.chkWikipediaTalk = new System.Windows.Forms.CheckBox();
            this.chkImageTalk = new System.Windows.Forms.CheckBox();
            this.chkMediaWiki = new System.Windows.Forms.CheckBox();
            this.chkMediaWikiTalk = new System.Windows.Forms.CheckBox();
            this.chkTemplateTalk = new System.Windows.Forms.CheckBox();
            this.chkHelp = new System.Windows.Forms.CheckBox();
            this.chkHelpTalk = new System.Windows.Forms.CheckBox();
            this.chkCategoryTalk = new System.Windows.Forms.CheckBox();
            this.chkPortal = new System.Windows.Forms.CheckBox();
            this.chkPortalTalk = new System.Windows.Forms.CheckBox();
            this.contextmenuFilter = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.nonTalkOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.talkSpaceOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.contextmenuFilter.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(315, 222);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(396, 222);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Checked = true;
            this.chkCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategory.Location = new System.Drawing.Point(6, 180);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(71, 17);
            this.chkCategory.TabIndex = 2;
            this.chkCategory.Text = "Category:";
            this.chkCategory.UseVisualStyleBackColor = true;
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Checked = true;
            this.chkTemplate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplate.Location = new System.Drawing.Point(6, 134);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(73, 17);
            this.chkTemplate.TabIndex = 3;
            this.chkTemplate.Text = "Template:";
            this.chkTemplate.UseVisualStyleBackColor = true;
            // 
            // chkWikipedia
            // 
            this.chkWikipedia.AutoSize = true;
            this.chkWikipedia.Checked = true;
            this.chkWikipedia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipedia.Location = new System.Drawing.Point(6, 65);
            this.chkWikipedia.Name = "chkWikipedia";
            this.chkWikipedia.Size = new System.Drawing.Size(76, 17);
            this.chkWikipedia.TabIndex = 6;
            this.chkWikipedia.Text = "Wikipedia:";
            this.chkWikipedia.UseVisualStyleBackColor = true;
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Checked = true;
            this.chkUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUser.Location = new System.Drawing.Point(6, 42);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(51, 17);
            this.chkUser.TabIndex = 7;
            this.chkUser.Text = "User:";
            this.chkUser.UseVisualStyleBackColor = true;
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Checked = true;
            this.chkImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImage.Location = new System.Drawing.Point(6, 88);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(58, 17);
            this.chkImage.TabIndex = 8;
            this.chkImage.Text = "Image:";
            this.chkImage.UseVisualStyleBackColor = true;
            // 
            // txtContains
            // 
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(6, 86);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(178, 20);
            this.txtContains.TabIndex = 11;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(6, 65);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(152, 17);
            this.chkContains.TabIndex = 12;
            this.chkContains.Text = "Filter out titles that contain:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Enabled = false;
            this.chkIsRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIsRegex.Location = new System.Drawing.Point(6, 19);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(120, 17);
            this.chkIsRegex.TabIndex = 13;
            this.chkIsRegex.Text = "Regular expressions";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkNotContains
            // 
            this.chkNotContains.AutoSize = true;
            this.chkNotContains.Location = new System.Drawing.Point(6, 130);
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.Size = new System.Drawing.Size(178, 17);
            this.chkNotContains.TabIndex = 15;
            this.chkNotContains.Text = "Filter out titles that don\'t contain:";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(6, 153);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(178, 20);
            this.txtDoesNotContain.TabIndex = 16;
            // 
            // chkArticle
            // 
            this.chkArticle.AutoSize = true;
            this.chkArticle.Checked = true;
            this.chkArticle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticle.Location = new System.Drawing.Point(6, 19);
            this.chkArticle.Name = "chkArticle";
            this.chkArticle.Size = new System.Drawing.Size(55, 17);
            this.chkArticle.TabIndex = 17;
            this.chkArticle.Text = "Article";
            this.chkArticle.UseVisualStyleBackColor = true;
            // 
            // chkArticleTalk
            // 
            this.chkArticleTalk.AutoSize = true;
            this.chkArticleTalk.Checked = true;
            this.chkArticleTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticleTalk.Location = new System.Drawing.Point(105, 19);
            this.chkArticleTalk.Name = "chkArticleTalk";
            this.chkArticleTalk.Size = new System.Drawing.Size(75, 17);
            this.chkArticleTalk.TabIndex = 18;
            this.chkArticleTalk.Text = "Article talk";
            this.chkArticleTalk.UseVisualStyleBackColor = true;
            // 
            // chkUserTalk
            // 
            this.chkUserTalk.AutoSize = true;
            this.chkUserTalk.Checked = true;
            this.chkUserTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUserTalk.Location = new System.Drawing.Point(105, 42);
            this.chkUserTalk.Name = "chkUserTalk";
            this.chkUserTalk.Size = new System.Drawing.Size(68, 17);
            this.chkUserTalk.TabIndex = 19;
            this.chkUserTalk.Text = "User talk";
            this.chkUserTalk.UseVisualStyleBackColor = true;
            // 
            // chkWikipediaTalk
            // 
            this.chkWikipediaTalk.AutoSize = true;
            this.chkWikipediaTalk.Checked = true;
            this.chkWikipediaTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipediaTalk.Location = new System.Drawing.Point(105, 65);
            this.chkWikipediaTalk.Name = "chkWikipediaTalk";
            this.chkWikipediaTalk.Size = new System.Drawing.Size(93, 17);
            this.chkWikipediaTalk.TabIndex = 20;
            this.chkWikipediaTalk.Text = "Wikipedia talk";
            this.chkWikipediaTalk.UseVisualStyleBackColor = true;
            // 
            // chkImageTalk
            // 
            this.chkImageTalk.AutoSize = true;
            this.chkImageTalk.Checked = true;
            this.chkImageTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImageTalk.Location = new System.Drawing.Point(105, 88);
            this.chkImageTalk.Name = "chkImageTalk";
            this.chkImageTalk.Size = new System.Drawing.Size(75, 17);
            this.chkImageTalk.TabIndex = 21;
            this.chkImageTalk.Text = "Image talk";
            this.chkImageTalk.UseVisualStyleBackColor = true;
            // 
            // chkMediaWiki
            // 
            this.chkMediaWiki.AutoSize = true;
            this.chkMediaWiki.Location = new System.Drawing.Point(6, 111);
            this.chkMediaWiki.Name = "chkMediaWiki";
            this.chkMediaWiki.Size = new System.Drawing.Size(76, 17);
            this.chkMediaWiki.TabIndex = 22;
            this.chkMediaWiki.Text = "MediaWiki";
            this.chkMediaWiki.UseVisualStyleBackColor = true;
            // 
            // chkMediaWikiTalk
            // 
            this.chkMediaWikiTalk.AutoSize = true;
            this.chkMediaWikiTalk.Location = new System.Drawing.Point(105, 111);
            this.chkMediaWikiTalk.Name = "chkMediaWikiTalk";
            this.chkMediaWikiTalk.Size = new System.Drawing.Size(97, 17);
            this.chkMediaWikiTalk.TabIndex = 23;
            this.chkMediaWikiTalk.Text = "MediaWikiTalk";
            this.chkMediaWikiTalk.UseVisualStyleBackColor = true;
            // 
            // chkTemplateTalk
            // 
            this.chkTemplateTalk.AutoSize = true;
            this.chkTemplateTalk.Checked = true;
            this.chkTemplateTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplateTalk.Location = new System.Drawing.Point(105, 134);
            this.chkTemplateTalk.Name = "chkTemplateTalk";
            this.chkTemplateTalk.Size = new System.Drawing.Size(90, 17);
            this.chkTemplateTalk.TabIndex = 24;
            this.chkTemplateTalk.Text = "Template talk";
            this.chkTemplateTalk.UseVisualStyleBackColor = true;
            // 
            // chkHelp
            // 
            this.chkHelp.AutoSize = true;
            this.chkHelp.Location = new System.Drawing.Point(6, 157);
            this.chkHelp.Name = "chkHelp";
            this.chkHelp.Size = new System.Drawing.Size(48, 17);
            this.chkHelp.TabIndex = 25;
            this.chkHelp.Text = "Help";
            this.chkHelp.UseVisualStyleBackColor = true;
            // 
            // chkHelpTalk
            // 
            this.chkHelpTalk.AutoSize = true;
            this.chkHelpTalk.Location = new System.Drawing.Point(105, 157);
            this.chkHelpTalk.Name = "chkHelpTalk";
            this.chkHelpTalk.Size = new System.Drawing.Size(68, 17);
            this.chkHelpTalk.TabIndex = 26;
            this.chkHelpTalk.Text = "Help talk";
            this.chkHelpTalk.UseVisualStyleBackColor = true;
            // 
            // chkCategoryTalk
            // 
            this.chkCategoryTalk.AutoSize = true;
            this.chkCategoryTalk.Checked = true;
            this.chkCategoryTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategoryTalk.Location = new System.Drawing.Point(105, 180);
            this.chkCategoryTalk.Name = "chkCategoryTalk";
            this.chkCategoryTalk.Size = new System.Drawing.Size(88, 17);
            this.chkCategoryTalk.TabIndex = 27;
            this.chkCategoryTalk.Text = "Category talk";
            this.chkCategoryTalk.UseVisualStyleBackColor = true;
            // 
            // chkPortal
            // 
            this.chkPortal.AutoSize = true;
            this.chkPortal.Location = new System.Drawing.Point(6, 203);
            this.chkPortal.Name = "chkPortal";
            this.chkPortal.Size = new System.Drawing.Size(53, 17);
            this.chkPortal.TabIndex = 28;
            this.chkPortal.Text = "Portal";
            this.chkPortal.UseVisualStyleBackColor = true;
            // 
            // chkPortalTalk
            // 
            this.chkPortalTalk.AutoSize = true;
            this.chkPortalTalk.Location = new System.Drawing.Point(105, 203);
            this.chkPortalTalk.Name = "chkPortalTalk";
            this.chkPortalTalk.Size = new System.Drawing.Size(73, 17);
            this.chkPortalTalk.TabIndex = 29;
            this.chkPortalTalk.Text = "Portal talk";
            this.chkPortalTalk.UseVisualStyleBackColor = true;
            // 
            // contextmenuFilter
            // 
            this.contextmenuFilter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.nonTalkOnlyToolStripMenuItem,
            this.talkSpaceOnlyToolStripMenuItem});
            this.contextmenuFilter.Name = "contextmenuFilter";
            this.contextmenuFilter.Size = new System.Drawing.Size(168, 98);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect all";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.deselectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(164, 6);
            // 
            // nonTalkOnlyToolStripMenuItem
            // 
            this.nonTalkOnlyToolStripMenuItem.Name = "nonTalkOnlyToolStripMenuItem";
            this.nonTalkOnlyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.nonTalkOnlyToolStripMenuItem.Text = "Non talk space only";
            this.nonTalkOnlyToolStripMenuItem.Click += new System.EventHandler(this.nonTalkOnlyToolStripMenuItem_Click);
            // 
            // talkSpaceOnlyToolStripMenuItem
            // 
            this.talkSpaceOnlyToolStripMenuItem.Name = "talkSpaceOnlyToolStripMenuItem";
            this.talkSpaceOnlyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.talkSpaceOnlyToolStripMenuItem.Text = "Talk space only";
            this.talkSpaceOnlyToolStripMenuItem.Click += new System.EventHandler(this.talkSpaceOnlyToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.ContextMenuStrip = this.contextmenuFilter;
            this.groupBox1.Controls.Add(this.chkArticle);
            this.groupBox1.Controls.Add(this.chkPortalTalk);
            this.groupBox1.Controls.Add(this.chkCategory);
            this.groupBox1.Controls.Add(this.chkTemplate);
            this.groupBox1.Controls.Add(this.chkPortal);
            this.groupBox1.Controls.Add(this.chkWikipedia);
            this.groupBox1.Controls.Add(this.chkCategoryTalk);
            this.groupBox1.Controls.Add(this.chkUser);
            this.groupBox1.Controls.Add(this.chkHelpTalk);
            this.groupBox1.Controls.Add(this.chkImage);
            this.groupBox1.Controls.Add(this.chkHelp);
            this.groupBox1.Controls.Add(this.chkArticleTalk);
            this.groupBox1.Controls.Add(this.chkTemplateTalk);
            this.groupBox1.Controls.Add(this.chkUserTalk);
            this.groupBox1.Controls.Add(this.chkMediaWikiTalk);
            this.groupBox1.Controls.Add(this.chkWikipediaTalk);
            this.groupBox1.Controls.Add(this.chkMediaWiki);
            this.groupBox1.Controls.Add(this.chkImageTalk);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 232);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Namespaces to keep";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkIsRegex);
            this.groupBox2.Controls.Add(this.txtDoesNotContain);
            this.groupBox2.Controls.Add(this.txtContains);
            this.groupBox2.Controls.Add(this.chkContains);
            this.groupBox2.Controls.Add(this.chkNotContains);
            this.groupBox2.Location = new System.Drawing.Point(279, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 204);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Matches";
            // 
            // specialFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 253);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "specialFilter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Special filter";
            this.TopMost = true;
            this.contextmenuFilter.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCategory;
        private System.Windows.Forms.CheckBox chkTemplate;
        private System.Windows.Forms.CheckBox chkWikipedia;
        private System.Windows.Forms.CheckBox chkUser;
        private System.Windows.Forms.CheckBox chkImage;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContains;
        private System.Windows.Forms.CheckBox chkIsRegex;
        private System.Windows.Forms.CheckBox chkNotContains;
        private System.Windows.Forms.TextBox txtDoesNotContain;
        private System.Windows.Forms.CheckBox chkArticle;
        private System.Windows.Forms.CheckBox chkArticleTalk;
        private System.Windows.Forms.CheckBox chkUserTalk;
        private System.Windows.Forms.CheckBox chkWikipediaTalk;
        private System.Windows.Forms.CheckBox chkImageTalk;
        private System.Windows.Forms.CheckBox chkMediaWiki;
        private System.Windows.Forms.CheckBox chkMediaWikiTalk;
        private System.Windows.Forms.CheckBox chkTemplateTalk;
        private System.Windows.Forms.CheckBox chkHelp;
        private System.Windows.Forms.CheckBox chkHelpTalk;
        private System.Windows.Forms.CheckBox chkCategoryTalk;
        private System.Windows.Forms.CheckBox chkPortal;
        private System.Windows.Forms.CheckBox chkPortalTalk;
        private System.Windows.Forms.ContextMenuStrip contextmenuFilter;
        private System.Windows.Forms.ToolStripMenuItem talkSpaceOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nonTalkOnlyToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}