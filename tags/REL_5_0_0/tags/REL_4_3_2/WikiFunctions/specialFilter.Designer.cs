namespace WikiFunctions.Lists
{
    partial class SpecialFilter
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
            this.btnNonTalk = new System.Windows.Forms.Button();
            this.btnTalkOnly = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbOpType = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGetList = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkRemoveDups = new System.Windows.Forms.CheckBox();
            this.lbRemove = new WikiFunctions.Controls.Lists.ListBox2();
            this.contextmenuFilter.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(258, 263);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(339, 263);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Checked = true;
            this.chkCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategory.Location = new System.Drawing.Point(12, 180);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(68, 17);
            this.chkCategory.TabIndex = 15;
            this.chkCategory.Text = "Category";
            this.chkCategory.UseVisualStyleBackColor = true;
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Checked = true;
            this.chkTemplate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplate.Location = new System.Drawing.Point(12, 134);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(70, 17);
            this.chkTemplate.TabIndex = 11;
            this.chkTemplate.Text = "Template";
            this.chkTemplate.UseVisualStyleBackColor = true;
            // 
            // chkWikipedia
            // 
            this.chkWikipedia.AutoSize = true;
            this.chkWikipedia.Checked = true;
            this.chkWikipedia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipedia.Location = new System.Drawing.Point(12, 65);
            this.chkWikipedia.Name = "chkWikipedia";
            this.chkWikipedia.Size = new System.Drawing.Size(73, 17);
            this.chkWikipedia.TabIndex = 5;
            this.chkWikipedia.Text = "Wikipedia";
            this.chkWikipedia.UseVisualStyleBackColor = true;
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Checked = true;
            this.chkUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUser.Location = new System.Drawing.Point(12, 42);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(48, 17);
            this.chkUser.TabIndex = 3;
            this.chkUser.Text = "User";
            this.chkUser.UseVisualStyleBackColor = true;
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Checked = true;
            this.chkImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImage.Location = new System.Drawing.Point(12, 88);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(58, 17);
            this.chkImage.TabIndex = 7;
            this.chkImage.Text = "Image:";
            this.chkImage.UseVisualStyleBackColor = true;
            // 
            // txtContains
            // 
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(8, 49);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(178, 20);
            this.txtContains.TabIndex = 2;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(8, 24);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(152, 17);
            this.chkContains.TabIndex = 1;
            this.chkContains.Text = "Filter out titles that contain:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Enabled = false;
            this.chkIsRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.chkIsRegex.Location = new System.Drawing.Point(65, 125);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(121, 17);
            this.chkIsRegex.TabIndex = 4;
            this.chkIsRegex.Text = "Regular expressions";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkNotContains
            // 
            this.chkNotContains.AutoSize = true;
            this.chkNotContains.Location = new System.Drawing.Point(8, 76);
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.Size = new System.Drawing.Size(178, 17);
            this.chkNotContains.TabIndex = 3;
            this.chkNotContains.Text = "Filter out titles that don\'t contain:";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(8, 99);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(178, 20);
            this.txtDoesNotContain.TabIndex = 4;
            // 
            // chkArticle
            // 
            this.chkArticle.AutoSize = true;
            this.chkArticle.Checked = true;
            this.chkArticle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticle.Location = new System.Drawing.Point(12, 19);
            this.chkArticle.Name = "chkArticle";
            this.chkArticle.Size = new System.Drawing.Size(49, 17);
            this.chkArticle.TabIndex = 1;
            this.chkArticle.Text = "Main";
            this.chkArticle.UseVisualStyleBackColor = true;
            // 
            // chkArticleTalk
            // 
            this.chkArticleTalk.AutoSize = true;
            this.chkArticleTalk.Checked = true;
            this.chkArticleTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticleTalk.Location = new System.Drawing.Point(111, 19);
            this.chkArticleTalk.Name = "chkArticleTalk";
            this.chkArticleTalk.Size = new System.Drawing.Size(75, 17);
            this.chkArticleTalk.TabIndex = 2;
            this.chkArticleTalk.Text = "Article talk";
            this.chkArticleTalk.UseVisualStyleBackColor = true;
            // 
            // chkUserTalk
            // 
            this.chkUserTalk.AutoSize = true;
            this.chkUserTalk.Checked = true;
            this.chkUserTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUserTalk.Location = new System.Drawing.Point(111, 42);
            this.chkUserTalk.Name = "chkUserTalk";
            this.chkUserTalk.Size = new System.Drawing.Size(68, 17);
            this.chkUserTalk.TabIndex = 4;
            this.chkUserTalk.Text = "User talk";
            this.chkUserTalk.UseVisualStyleBackColor = true;
            // 
            // chkWikipediaTalk
            // 
            this.chkWikipediaTalk.AutoSize = true;
            this.chkWikipediaTalk.Checked = true;
            this.chkWikipediaTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipediaTalk.Location = new System.Drawing.Point(111, 65);
            this.chkWikipediaTalk.Name = "chkWikipediaTalk";
            this.chkWikipediaTalk.Size = new System.Drawing.Size(93, 17);
            this.chkWikipediaTalk.TabIndex = 6;
            this.chkWikipediaTalk.Text = "Wikipedia talk";
            this.chkWikipediaTalk.UseVisualStyleBackColor = true;
            // 
            // chkImageTalk
            // 
            this.chkImageTalk.AutoSize = true;
            this.chkImageTalk.Checked = true;
            this.chkImageTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImageTalk.Location = new System.Drawing.Point(111, 88);
            this.chkImageTalk.Name = "chkImageTalk";
            this.chkImageTalk.Size = new System.Drawing.Size(75, 17);
            this.chkImageTalk.TabIndex = 8;
            this.chkImageTalk.Text = "Image talk";
            this.chkImageTalk.UseVisualStyleBackColor = true;
            // 
            // chkMediaWiki
            // 
            this.chkMediaWiki.AutoSize = true;
            this.chkMediaWiki.Location = new System.Drawing.Point(12, 111);
            this.chkMediaWiki.Name = "chkMediaWiki";
            this.chkMediaWiki.Size = new System.Drawing.Size(76, 17);
            this.chkMediaWiki.TabIndex = 9;
            this.chkMediaWiki.Text = "MediaWiki";
            this.chkMediaWiki.UseVisualStyleBackColor = true;
            // 
            // chkMediaWikiTalk
            // 
            this.chkMediaWikiTalk.AutoSize = true;
            this.chkMediaWikiTalk.Location = new System.Drawing.Point(111, 111);
            this.chkMediaWikiTalk.Name = "chkMediaWikiTalk";
            this.chkMediaWikiTalk.Size = new System.Drawing.Size(97, 17);
            this.chkMediaWikiTalk.TabIndex = 10;
            this.chkMediaWikiTalk.Text = "MediaWikiTalk";
            this.chkMediaWikiTalk.UseVisualStyleBackColor = true;
            // 
            // chkTemplateTalk
            // 
            this.chkTemplateTalk.AutoSize = true;
            this.chkTemplateTalk.Checked = true;
            this.chkTemplateTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplateTalk.Location = new System.Drawing.Point(111, 134);
            this.chkTemplateTalk.Name = "chkTemplateTalk";
            this.chkTemplateTalk.Size = new System.Drawing.Size(90, 17);
            this.chkTemplateTalk.TabIndex = 12;
            this.chkTemplateTalk.Text = "Template talk";
            this.chkTemplateTalk.UseVisualStyleBackColor = true;
            // 
            // chkHelp
            // 
            this.chkHelp.AutoSize = true;
            this.chkHelp.Location = new System.Drawing.Point(12, 157);
            this.chkHelp.Name = "chkHelp";
            this.chkHelp.Size = new System.Drawing.Size(48, 17);
            this.chkHelp.TabIndex = 13;
            this.chkHelp.Text = "Help";
            this.chkHelp.UseVisualStyleBackColor = true;
            // 
            // chkHelpTalk
            // 
            this.chkHelpTalk.AutoSize = true;
            this.chkHelpTalk.Location = new System.Drawing.Point(111, 157);
            this.chkHelpTalk.Name = "chkHelpTalk";
            this.chkHelpTalk.Size = new System.Drawing.Size(68, 17);
            this.chkHelpTalk.TabIndex = 14;
            this.chkHelpTalk.Text = "Help talk";
            this.chkHelpTalk.UseVisualStyleBackColor = true;
            // 
            // chkCategoryTalk
            // 
            this.chkCategoryTalk.AutoSize = true;
            this.chkCategoryTalk.Checked = true;
            this.chkCategoryTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategoryTalk.Location = new System.Drawing.Point(111, 180);
            this.chkCategoryTalk.Name = "chkCategoryTalk";
            this.chkCategoryTalk.Size = new System.Drawing.Size(88, 17);
            this.chkCategoryTalk.TabIndex = 16;
            this.chkCategoryTalk.Text = "Category talk";
            this.chkCategoryTalk.UseVisualStyleBackColor = true;
            // 
            // chkPortal
            // 
            this.chkPortal.AutoSize = true;
            this.chkPortal.Location = new System.Drawing.Point(12, 203);
            this.chkPortal.Name = "chkPortal";
            this.chkPortal.Size = new System.Drawing.Size(53, 17);
            this.chkPortal.TabIndex = 17;
            this.chkPortal.Text = "Portal";
            this.chkPortal.UseVisualStyleBackColor = true;
            // 
            // chkPortalTalk
            // 
            this.chkPortalTalk.AutoSize = true;
            this.chkPortalTalk.Location = new System.Drawing.Point(111, 203);
            this.chkPortalTalk.Name = "chkPortalTalk";
            this.chkPortalTalk.Size = new System.Drawing.Size(73, 17);
            this.chkPortalTalk.TabIndex = 18;
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
            this.contextmenuFilter.Size = new System.Drawing.Size(179, 98);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.deselectAllToolStripMenuItem.Text = "Deselect all";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.deselectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(175, 6);
            // 
            // nonTalkOnlyToolStripMenuItem
            // 
            this.nonTalkOnlyToolStripMenuItem.Name = "nonTalkOnlyToolStripMenuItem";
            this.nonTalkOnlyToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.nonTalkOnlyToolStripMenuItem.Text = "Non talk space only";
            this.nonTalkOnlyToolStripMenuItem.Click += new System.EventHandler(this.nonTalkOnlyToolStripMenuItem_Click);
            // 
            // talkSpaceOnlyToolStripMenuItem
            // 
            this.talkSpaceOnlyToolStripMenuItem.Name = "talkSpaceOnlyToolStripMenuItem";
            this.talkSpaceOnlyToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.talkSpaceOnlyToolStripMenuItem.Text = "Talk space only";
            this.talkSpaceOnlyToolStripMenuItem.Click += new System.EventHandler(this.talkSpaceOnlyToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.ContextMenuStrip = this.contextmenuFilter;
            this.groupBox1.Controls.Add(this.btnNonTalk);
            this.groupBox1.Controls.Add(this.btnTalkOnly);
            this.groupBox1.Controls.Add(this.btnSelectNone);
            this.groupBox1.Controls.Add(this.btnSelectAll);
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
            this.groupBox1.Size = new System.Drawing.Size(220, 285);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Namespaces to keep";
            // 
            // btnNonTalk
            // 
            this.btnNonTalk.Location = new System.Drawing.Point(15, 250);
            this.btnNonTalk.Name = "btnNonTalk";
            this.btnNonTalk.Size = new System.Drawing.Size(92, 23);
            this.btnNonTalk.TabIndex = 22;
            this.btnNonTalk.Text = "Non-Talk Only";
            this.btnNonTalk.UseVisualStyleBackColor = true;
            this.btnNonTalk.Click += new System.EventHandler(this.btnNonTalk_Click);
            // 
            // btnTalkOnly
            // 
            this.btnTalkOnly.Location = new System.Drawing.Point(113, 250);
            this.btnTalkOnly.Name = "btnTalkOnly";
            this.btnTalkOnly.Size = new System.Drawing.Size(92, 23);
            this.btnTalkOnly.TabIndex = 21;
            this.btnTalkOnly.Text = "Talk Only";
            this.btnTalkOnly.UseVisualStyleBackColor = true;
            this.btnTalkOnly.Click += new System.EventHandler(this.btnTalkOnly_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(113, 222);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(92, 23);
            this.btnSelectNone.TabIndex = 20;
            this.btnSelectNone.Text = "Deselect all";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.btnSelectNone_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(15, 222);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(92, 23);
            this.btnSelectAll.TabIndex = 19;
            this.btnSelectAll.Text = "Select all";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkIsRegex);
            this.groupBox2.Controls.Add(this.txtDoesNotContain);
            this.groupBox2.Controls.Add(this.txtContains);
            this.groupBox2.Controls.Add(this.chkContains);
            this.groupBox2.Controls.Add(this.chkNotContains);
            this.groupBox2.Location = new System.Drawing.Point(238, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 151);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Matches";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbOpType);
            this.groupBox3.Controls.Add(this.lbRemove);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.btnGetList);
            this.groupBox3.Location = new System.Drawing.Point(437, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 285);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Set operations";
            // 
            // cbOpType
            // 
            this.cbOpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpType.FormattingEnabled = true;
            this.cbOpType.Items.AddRange(new object[] {
            "Find difference",
            "Find intersection"});
            this.cbOpType.Location = new System.Drawing.Point(8, 19);
            this.cbOpType.Name = "cbOpType";
            this.cbOpType.Size = new System.Drawing.Size(169, 21);
            this.cbOpType.TabIndex = 6;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(95, 250);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetList
            // 
            this.btnGetList.Location = new System.Drawing.Point(14, 251);
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.Size = new System.Drawing.Size(75, 23);
            this.btnGetList.TabIndex = 1;
            this.btnGetList.Text = "Open file";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkRemoveDups);
            this.groupBox4.Location = new System.Drawing.Point(239, 169);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(192, 55);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other";
            // 
            // chkRemoveDups
            // 
            this.chkRemoveDups.AutoSize = true;
            this.chkRemoveDups.Location = new System.Drawing.Point(7, 20);
            this.chkRemoveDups.Name = "chkRemoveDups";
            this.chkRemoveDups.Size = new System.Drawing.Size(119, 17);
            this.chkRemoveDups.TabIndex = 5;
            this.chkRemoveDups.Text = "Remove Duplicates";
            this.chkRemoveDups.UseVisualStyleBackColor = true;
            // 
            // lbRemove
            // 
            this.lbRemove.FormattingEnabled = true;
            this.lbRemove.Location = new System.Drawing.Point(8, 45);
            this.lbRemove.Name = "lbRemove";
            this.lbRemove.Size = new System.Drawing.Size(169, 199);
            this.lbRemove.TabIndex = 5;
            // 
            // SpecialFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 309);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SpecialFilter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Special filter";
            this.VisibleChanged += new System.EventHandler(this.SpecialFilter_VisibleChanged);
            this.Load += new System.EventHandler(this.specialFilter_Load);
            this.contextmenuFilter.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnGetList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkRemoveDups;
        private WikiFunctions.Controls.Lists.ListBox2 lbRemove;
        private System.Windows.Forms.ComboBox cbOpType;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnNonTalk;
        private System.Windows.Forms.Button btnTalkOnly;
    }
}