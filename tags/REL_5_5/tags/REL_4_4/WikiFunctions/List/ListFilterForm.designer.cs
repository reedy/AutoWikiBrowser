namespace WikiFunctions.Lists
{
    partial class ListFilterForm
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
            this.btnOk = new System.Windows.Forms.Button();
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
            this.gbNamespaces = new System.Windows.Forms.GroupBox();
            this.flwTalk = new System.Windows.Forms.FlowLayoutPanel();
            this.chkTalk = new System.Windows.Forms.CheckBox();
            this.flwContent = new System.Windows.Forms.FlowLayoutPanel();
            this.chkContents = new System.Windows.Forms.CheckBox();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.gbSets = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGetList = new System.Windows.Forms.Button();
            this.lbRemove = new WikiFunctions.Controls.Lists.ListBox2();
            this.cbOpType = new System.Windows.Forms.ComboBox();
            this.gbMisc = new System.Windows.Forms.GroupBox();
            this.flwOther = new System.Windows.Forms.FlowLayoutPanel();
            this.chkSortAZ = new System.Windows.Forms.CheckBox();
            this.chkRemoveDups = new System.Windows.Forms.CheckBox();
            this.gbNamespaces.SuspendLayout();
            this.flwTalk.SuspendLayout();
            this.flwContent.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.gbSets.SuspendLayout();
            this.gbMisc.SuspendLayout();
            this.flwOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(655, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "Apply";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(655, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Checked = true;
            this.chkCategory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategory.Location = new System.Drawing.Point(3, 164);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(68, 17);
            this.chkCategory.TabIndex = 14;
            this.chkCategory.Tag = "14";
            this.chkCategory.Text = "Category";
            this.chkCategory.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Checked = true;
            this.chkTemplate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplate.Location = new System.Drawing.Point(3, 118);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(70, 17);
            this.chkTemplate.TabIndex = 10;
            this.chkTemplate.Tag = "10";
            this.chkTemplate.Text = "Template";
            this.chkTemplate.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkWikipedia
            // 
            this.chkWikipedia.AutoSize = true;
            this.chkWikipedia.Checked = true;
            this.chkWikipedia.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipedia.Location = new System.Drawing.Point(3, 49);
            this.chkWikipedia.Name = "chkWikipedia";
            this.chkWikipedia.Size = new System.Drawing.Size(94, 17);
            this.chkWikipedia.TabIndex = 4;
            this.chkWikipedia.Tag = "4";
            this.chkWikipedia.Text = "(Project name)";
            this.chkWikipedia.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Checked = true;
            this.chkUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUser.Location = new System.Drawing.Point(3, 26);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(48, 17);
            this.chkUser.TabIndex = 2;
            this.chkUser.Tag = "2";
            this.chkUser.Text = "User";
            this.chkUser.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Checked = true;
            this.chkImage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImage.Location = new System.Drawing.Point(3, 72);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(58, 17);
            this.chkImage.TabIndex = 6;
            this.chkImage.Tag = "6";
            this.chkImage.Text = "Image:";
            this.chkImage.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // txtContains
            // 
            this.txtContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(6, 41);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(180, 20);
            this.txtContains.TabIndex = 1;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(6, 19);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(145, 17);
            this.chkContains.TabIndex = 0;
            this.chkContains.Text = "Remove titles &containing:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            this.chkIsRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Enabled = false;
            this.chkIsRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.chkIsRegex.Location = new System.Drawing.Point(65, 113);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(121, 17);
            this.chkIsRegex.TabIndex = 4;
            this.chkIsRegex.Text = "&Regular expressions";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkNotContains
            // 
            this.chkNotContains.AutoSize = true;
            this.chkNotContains.Location = new System.Drawing.Point(6, 67);
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.Size = new System.Drawing.Size(130, 17);
            this.chkNotContains.TabIndex = 2;
            this.chkNotContains.Text = "Keep titles co&ntaining:";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(6, 87);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(180, 20);
            this.txtDoesNotContain.TabIndex = 3;
            // 
            // chkArticle
            // 
            this.chkArticle.AutoSize = true;
            this.chkArticle.Checked = true;
            this.chkArticle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticle.Location = new System.Drawing.Point(3, 3);
            this.chkArticle.Name = "chkArticle";
            this.chkArticle.Size = new System.Drawing.Size(54, 17);
            this.chkArticle.TabIndex = 0;
            this.chkArticle.Tag = "0";
            this.chkArticle.Text = "(main)";
            this.chkArticle.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkArticleTalk
            // 
            this.chkArticleTalk.AutoSize = true;
            this.chkArticleTalk.Checked = true;
            this.chkArticleTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkArticleTalk.Location = new System.Drawing.Point(3, 3);
            this.chkArticleTalk.Name = "chkArticleTalk";
            this.chkArticleTalk.Size = new System.Drawing.Size(47, 17);
            this.chkArticleTalk.TabIndex = 1;
            this.chkArticleTalk.Tag = "1";
            this.chkArticleTalk.Text = "Talk";
            this.chkArticleTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkUserTalk
            // 
            this.chkUserTalk.AutoSize = true;
            this.chkUserTalk.Checked = true;
            this.chkUserTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUserTalk.Location = new System.Drawing.Point(3, 26);
            this.chkUserTalk.Name = "chkUserTalk";
            this.chkUserTalk.Size = new System.Drawing.Size(68, 17);
            this.chkUserTalk.TabIndex = 3;
            this.chkUserTalk.Tag = "3";
            this.chkUserTalk.Text = "User talk";
            this.chkUserTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkWikipediaTalk
            // 
            this.chkWikipediaTalk.AutoSize = true;
            this.chkWikipediaTalk.Checked = true;
            this.chkWikipediaTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWikipediaTalk.Location = new System.Drawing.Point(3, 49);
            this.chkWikipediaTalk.Name = "chkWikipediaTalk";
            this.chkWikipediaTalk.Size = new System.Drawing.Size(79, 17);
            this.chkWikipediaTalk.TabIndex = 5;
            this.chkWikipediaTalk.Tag = "5";
            this.chkWikipediaTalk.Text = "Project talk";
            this.chkWikipediaTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkImageTalk
            // 
            this.chkImageTalk.AutoSize = true;
            this.chkImageTalk.Checked = true;
            this.chkImageTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImageTalk.Location = new System.Drawing.Point(3, 72);
            this.chkImageTalk.Name = "chkImageTalk";
            this.chkImageTalk.Size = new System.Drawing.Size(75, 17);
            this.chkImageTalk.TabIndex = 7;
            this.chkImageTalk.Tag = "7";
            this.chkImageTalk.Text = "Image talk";
            this.chkImageTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkMediaWiki
            // 
            this.chkMediaWiki.AutoSize = true;
            this.chkMediaWiki.Location = new System.Drawing.Point(3, 95);
            this.chkMediaWiki.Name = "chkMediaWiki";
            this.chkMediaWiki.Size = new System.Drawing.Size(76, 17);
            this.chkMediaWiki.TabIndex = 8;
            this.chkMediaWiki.Tag = "8";
            this.chkMediaWiki.Text = "MediaWiki";
            this.chkMediaWiki.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkMediaWikiTalk
            // 
            this.chkMediaWikiTalk.AutoSize = true;
            this.chkMediaWikiTalk.Location = new System.Drawing.Point(3, 95);
            this.chkMediaWikiTalk.Name = "chkMediaWikiTalk";
            this.chkMediaWikiTalk.Size = new System.Drawing.Size(96, 17);
            this.chkMediaWikiTalk.TabIndex = 9;
            this.chkMediaWikiTalk.Tag = "9";
            this.chkMediaWikiTalk.Text = "MediaWiki talk";
            this.chkMediaWikiTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkTemplateTalk
            // 
            this.chkTemplateTalk.AutoSize = true;
            this.chkTemplateTalk.Checked = true;
            this.chkTemplateTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTemplateTalk.Location = new System.Drawing.Point(3, 118);
            this.chkTemplateTalk.Name = "chkTemplateTalk";
            this.chkTemplateTalk.Size = new System.Drawing.Size(90, 17);
            this.chkTemplateTalk.TabIndex = 11;
            this.chkTemplateTalk.Tag = "11";
            this.chkTemplateTalk.Text = "Template talk";
            this.chkTemplateTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkHelp
            // 
            this.chkHelp.AutoSize = true;
            this.chkHelp.Location = new System.Drawing.Point(3, 141);
            this.chkHelp.Name = "chkHelp";
            this.chkHelp.Size = new System.Drawing.Size(48, 17);
            this.chkHelp.TabIndex = 12;
            this.chkHelp.Tag = "12";
            this.chkHelp.Text = "Help";
            this.chkHelp.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkHelpTalk
            // 
            this.chkHelpTalk.AutoSize = true;
            this.chkHelpTalk.Location = new System.Drawing.Point(3, 141);
            this.chkHelpTalk.Name = "chkHelpTalk";
            this.chkHelpTalk.Size = new System.Drawing.Size(68, 17);
            this.chkHelpTalk.TabIndex = 13;
            this.chkHelpTalk.Tag = "13";
            this.chkHelpTalk.Text = "Help talk";
            this.chkHelpTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkCategoryTalk
            // 
            this.chkCategoryTalk.AutoSize = true;
            this.chkCategoryTalk.Checked = true;
            this.chkCategoryTalk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCategoryTalk.Location = new System.Drawing.Point(3, 164);
            this.chkCategoryTalk.Name = "chkCategoryTalk";
            this.chkCategoryTalk.Size = new System.Drawing.Size(88, 17);
            this.chkCategoryTalk.TabIndex = 15;
            this.chkCategoryTalk.Tag = "15";
            this.chkCategoryTalk.Text = "Category talk";
            this.chkCategoryTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // chkPortal
            // 
            this.chkPortal.AutoSize = true;
            this.chkPortal.Location = new System.Drawing.Point(3, 187);
            this.chkPortal.Name = "chkPortal";
            this.chkPortal.Size = new System.Drawing.Size(53, 17);
            this.chkPortal.TabIndex = 100;
            this.chkPortal.Tag = "16";
            this.chkPortal.Text = "Portal";
            this.chkPortal.Click += new System.EventHandler(this.Content_CheckedChanged);
            // 
            // chkPortalTalk
            // 
            this.chkPortalTalk.AutoSize = true;
            this.chkPortalTalk.Location = new System.Drawing.Point(3, 187);
            this.chkPortalTalk.Name = "chkPortalTalk";
            this.chkPortalTalk.Size = new System.Drawing.Size(73, 17);
            this.chkPortalTalk.TabIndex = 101;
            this.chkPortalTalk.Tag = "17";
            this.chkPortalTalk.Text = "Portal talk";
            this.chkPortalTalk.Click += new System.EventHandler(this.Talk_CheckedChanged);
            // 
            // gbNamespaces
            // 
            this.gbNamespaces.Controls.Add(this.flwTalk);
            this.gbNamespaces.Controls.Add(this.chkTalk);
            this.gbNamespaces.Controls.Add(this.flwContent);
            this.gbNamespaces.Controls.Add(this.chkContents);
            this.gbNamespaces.Location = new System.Drawing.Point(12, 12);
            this.gbNamespaces.Name = "gbNamespaces";
            this.gbNamespaces.Size = new System.Drawing.Size(242, 260);
            this.gbNamespaces.TabIndex = 0;
            this.gbNamespaces.TabStop = false;
            this.gbNamespaces.Text = "Namespaces to keep";
            // 
            // flwTalk
            // 
            this.flwTalk.Controls.Add(this.chkArticleTalk);
            this.flwTalk.Controls.Add(this.chkUserTalk);
            this.flwTalk.Controls.Add(this.chkWikipediaTalk);
            this.flwTalk.Controls.Add(this.chkImageTalk);
            this.flwTalk.Controls.Add(this.chkMediaWikiTalk);
            this.flwTalk.Controls.Add(this.chkTemplateTalk);
            this.flwTalk.Controls.Add(this.chkHelpTalk);
            this.flwTalk.Controls.Add(this.chkCategoryTalk);
            this.flwTalk.Controls.Add(this.chkPortalTalk);
            this.flwTalk.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwTalk.Location = new System.Drawing.Point(123, 42);
            this.flwTalk.Name = "flwTalk";
            this.flwTalk.Size = new System.Drawing.Size(100, 209);
            this.flwTalk.TabIndex = 3;
            // 
            // chkTalk
            // 
            this.chkTalk.AutoSize = true;
            this.chkTalk.Checked = true;
            this.chkTalk.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chkTalk.Location = new System.Drawing.Point(110, 19);
            this.chkTalk.Name = "chkTalk";
            this.chkTalk.Size = new System.Drawing.Size(47, 17);
            this.chkTalk.TabIndex = 2;
            this.chkTalk.Tag = "1001";
            this.chkTalk.Text = "&Talk";
            this.chkTalk.CheckedChanged += new System.EventHandler(this.chkTalk_CheckedChanged);
            // 
            // flwContent
            // 
            this.flwContent.Controls.Add(this.chkArticle);
            this.flwContent.Controls.Add(this.chkUser);
            this.flwContent.Controls.Add(this.chkWikipedia);
            this.flwContent.Controls.Add(this.chkImage);
            this.flwContent.Controls.Add(this.chkMediaWiki);
            this.flwContent.Controls.Add(this.chkTemplate);
            this.flwContent.Controls.Add(this.chkHelp);
            this.flwContent.Controls.Add(this.chkCategory);
            this.flwContent.Controls.Add(this.chkPortal);
            this.flwContent.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwContent.Location = new System.Drawing.Point(17, 42);
            this.flwContent.Name = "flwContent";
            this.flwContent.Size = new System.Drawing.Size(100, 209);
            this.flwContent.TabIndex = 1;
            // 
            // chkContents
            // 
            this.chkContents.AutoSize = true;
            this.chkContents.Checked = true;
            this.chkContents.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chkContents.Location = new System.Drawing.Point(6, 19);
            this.chkContents.Name = "chkContents";
            this.chkContents.Size = new System.Drawing.Size(63, 17);
            this.chkContents.TabIndex = 0;
            this.chkContents.Tag = "1000";
            this.chkContents.Text = "&Content";
            this.chkContents.CheckedChanged += new System.EventHandler(this.chkContents_CheckedChanged);
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.chkIsRegex);
            this.gbSearch.Controls.Add(this.txtDoesNotContain);
            this.gbSearch.Controls.Add(this.chkNotContains);
            this.gbSearch.Controls.Add(this.txtContains);
            this.gbSearch.Controls.Add(this.chkContains);
            this.gbSearch.Location = new System.Drawing.Point(260, 12);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(192, 141);
            this.gbSearch.TabIndex = 1;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Matches";
            // 
            // gbSets
            // 
            this.gbSets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSets.Controls.Add(this.btnClear);
            this.gbSets.Controls.Add(this.btnGetList);
            this.gbSets.Controls.Add(this.lbRemove);
            this.gbSets.Controls.Add(this.cbOpType);
            this.gbSets.Location = new System.Drawing.Point(458, 12);
            this.gbSets.Name = "gbSets";
            this.gbSets.Size = new System.Drawing.Size(191, 260);
            this.gbSets.TabIndex = 3;
            this.gbSets.TabStop = false;
            this.gbSets.Text = "Set operations";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(102, 231);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(83, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetList
            // 
            this.btnGetList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetList.Location = new System.Drawing.Point(6, 231);
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.Size = new System.Drawing.Size(83, 23);
            this.btnGetList.TabIndex = 2;
            this.btnGetList.Text = "&Open file";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
            // 
            // lbRemove
            // 
            this.lbRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRemove.FormattingEnabled = true;
            this.lbRemove.Location = new System.Drawing.Point(6, 44);
            this.lbRemove.Name = "lbRemove";
            this.lbRemove.Size = new System.Drawing.Size(179, 186);
            this.lbRemove.TabIndex = 1;
            // 
            // cbOpType
            // 
            this.cbOpType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpType.FormattingEnabled = true;
            this.cbOpType.Items.AddRange(new object[] {
            "Symmetric difference",
            "Intersection"});
            this.cbOpType.Location = new System.Drawing.Point(6, 19);
            this.cbOpType.Name = "cbOpType";
            this.cbOpType.Size = new System.Drawing.Size(179, 21);
            this.cbOpType.TabIndex = 0;
            // 
            // gbMisc
            // 
            this.gbMisc.Controls.Add(this.flwOther);
            this.gbMisc.Location = new System.Drawing.Point(260, 159);
            this.gbMisc.Name = "gbMisc";
            this.gbMisc.Size = new System.Drawing.Size(192, 63);
            this.gbMisc.TabIndex = 2;
            this.gbMisc.TabStop = false;
            this.gbMisc.Text = "Other";
            // 
            // flwOther
            // 
            this.flwOther.Controls.Add(this.chkSortAZ);
            this.flwOther.Controls.Add(this.chkRemoveDups);
            this.flwOther.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwOther.Location = new System.Drawing.Point(3, 16);
            this.flwOther.Name = "flwOther";
            this.flwOther.Size = new System.Drawing.Size(186, 44);
            this.flwOther.TabIndex = 0;
            // 
            // chkSortAZ
            // 
            this.chkSortAZ.AutoSize = true;
            this.chkSortAZ.Checked = true;
            this.chkSortAZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortAZ.Location = new System.Drawing.Point(3, 3);
            this.chkSortAZ.Name = "chkSortAZ";
            this.chkSortAZ.Size = new System.Drawing.Size(112, 17);
            this.chkSortAZ.TabIndex = 0;
            this.chkSortAZ.Text = "Sort alpha&betically";
            this.chkSortAZ.UseVisualStyleBackColor = true;
            // 
            // chkRemoveDups
            // 
            this.chkRemoveDups.AutoSize = true;
            this.chkRemoveDups.Checked = true;
            this.chkRemoveDups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveDups.Location = new System.Drawing.Point(3, 26);
            this.chkRemoveDups.Name = "chkRemoveDups";
            this.chkRemoveDups.Size = new System.Drawing.Size(117, 17);
            this.chkRemoveDups.TabIndex = 1;
            this.chkRemoveDups.Text = "Remove &duplicates";
            this.chkRemoveDups.UseVisualStyleBackColor = true;
            // 
            // ListFilterForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(742, 286);
            this.Controls.Add(this.gbNamespaces);
            this.Controls.Add(this.gbSearch);
            this.Controls.Add(this.gbSets);
            this.Controls.Add(this.gbMisc);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(750, 310);
            this.Name = "ListFilterForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter";
            this.Load += new System.EventHandler(this.specialFilter_Load);
            this.VisibleChanged += new System.EventHandler(this.SpecialFilter_VisibleChanged);
            this.gbNamespaces.ResumeLayout(false);
            this.gbNamespaces.PerformLayout();
            this.flwTalk.ResumeLayout(false);
            this.flwTalk.PerformLayout();
            this.flwContent.ResumeLayout(false);
            this.flwContent.PerformLayout();
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gbSets.ResumeLayout(false);
            this.gbMisc.ResumeLayout(false);
            this.flwOther.ResumeLayout(false);
            this.flwOther.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
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
        private System.Windows.Forms.GroupBox gbNamespaces;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.GroupBox gbSets;
        private System.Windows.Forms.Button btnGetList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox gbMisc;
        private System.Windows.Forms.CheckBox chkRemoveDups;
        private WikiFunctions.Controls.Lists.ListBox2 lbRemove;
        private System.Windows.Forms.ComboBox cbOpType;
        private System.Windows.Forms.CheckBox chkSortAZ;
        private System.Windows.Forms.FlowLayoutPanel flwTalk;
        private System.Windows.Forms.FlowLayoutPanel flwContent;
        private System.Windows.Forms.CheckBox chkContents;
        private System.Windows.Forms.CheckBox chkTalk;
        private System.Windows.Forms.FlowLayoutPanel flwOther;
    }
}
