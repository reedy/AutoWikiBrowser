namespace WikiFunctions.Lists
{
    partial class ListMaker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtSelectSource = new System.Windows.Forms.TextBox();
            this.lblNumberOfArticles = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtNewArticle = new System.Windows.Forms.TextBox();
            this.lblSourceSelect = new System.Windows.Forms.Label();
            this.btnMakeList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboSourceSelect = new System.Windows.Forms.ComboBox();
            this.mnuListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.filterOutNonMainSpaceArticlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertFromTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphebeticallyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToTextFileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.addSelectedToListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTranscludesHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromWhatlinkshereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromLinksOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromImageLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnRemoveArticle = new System.Windows.Forms.Button();
            this.btnArticlesListClear = new System.Windows.Forms.Button();
            this.btnArticlesListSave = new System.Windows.Forms.Button();
            this.btnRemoveDuplicates = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Lists.ListBox2();
            this.mnuListBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSelectSource
            // 
            this.txtSelectSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSelectSource.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtSelectSource.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtSelectSource.Location = new System.Drawing.Point(78, 30);
            this.txtSelectSource.Name = "txtSelectSource";
            this.txtSelectSource.Size = new System.Drawing.Size(121, 20);
            this.txtSelectSource.TabIndex = 18;
            this.txtSelectSource.DoubleClick += new System.EventHandler(this.txtSelectSource_DoubleClick);
            this.txtSelectSource.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSelectSource_KeyPress);
            // 
            // lblNumberOfArticles
            // 
            this.lblNumberOfArticles.Location = new System.Drawing.Point(141, 56);
            this.lblNumberOfArticles.Name = "lblNumberOfArticles";
            this.lblNumberOfArticles.Size = new System.Drawing.Size(74, 20);
            this.lblNumberOfArticles.TabIndex = 28;
            this.lblNumberOfArticles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(4, 323);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(42, 23);
            this.btnAdd.TabIndex = 25;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtNewArticle
            // 
            this.txtNewArticle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewArticle.Location = new System.Drawing.Point(52, 325);
            this.txtNewArticle.Name = "txtNewArticle";
            this.txtNewArticle.Size = new System.Drawing.Size(144, 20);
            this.txtNewArticle.TabIndex = 24;
            this.txtNewArticle.DoubleClick += new System.EventHandler(this.txtNewArticle_DoubleClick);
            this.txtNewArticle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtNewArticle_MouseMove);
            this.txtNewArticle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewArticle_KeyPress);
            // 
            // lblSourceSelect
            // 
            this.lblSourceSelect.Location = new System.Drawing.Point(0, 29);
            this.lblSourceSelect.Name = "lblSourceSelect";
            this.lblSourceSelect.Size = new System.Drawing.Size(77, 21);
            this.lblSourceSelect.TabIndex = 23;
            this.lblSourceSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMakeList
            // 
            this.btnMakeList.Enabled = false;
            this.btnMakeList.Location = new System.Drawing.Point(78, 55);
            this.btnMakeList.Name = "btnMakeList";
            this.btnMakeList.Size = new System.Drawing.Size(62, 23);
            this.btnMakeList.TabIndex = 20;
            this.btnMakeList.Tag = "Get all pages needing editting";
            this.btnMakeList.Text = "Make list";
            this.btnMakeList.UseVisualStyleBackColor = true;
            this.btnMakeList.Click += new System.EventHandler(this.btnMakeList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Make from";
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboSourceSelect.DropDownHeight = 200;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.IntegralHeight = false;
            this.cmboSourceSelect.Items.AddRange(new object[] {
            "Category",
            "What links here",
            "What transcludes here",
            "Links on page",
            "Text file",
            "Google search",
            "User contribs",
            "Special page",
            "Image file links",
            "Database dump",
            "My Watchlist",
            "Wiki search"});
            this.cmboSourceSelect.Location = new System.Drawing.Point(78, 2);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(121, 21);
            this.cmboSourceSelect.TabIndex = 19;
            this.cmboSourceSelect.Tag = "Source of pages to edit";
            this.cmboSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmboSourceSelect_SelectedIndexChanged);
            // 
            // mnuListBox
            // 
            this.mnuListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterOutNonMainSpaceArticlesToolStripMenuItem,
            this.specialFilterToolStripMenuItem,
            this.convertToTalkPagesToolStripMenuItem,
            this.convertFromTalkPagesToolStripMenuItem,
            this.sortAlphebeticallyMenuItem,
            this.saveListToTextFileToolStripMenuItem1,
            this.toolStripSeparator14,
            this.addSelectedToListToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeToolStripMenuItem,
            this.clearToolStripMenuItem1,
            this.toolStripSeparator1,
            this.openInBrowserToolStripMenuItem});
            this.mnuListBox.Name = "contextMenuStrip2";
            this.mnuListBox.Size = new System.Drawing.Size(209, 242);
            this.mnuListBox.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListBox_Opening);
            // 
            // filterOutNonMainSpaceArticlesToolStripMenuItem
            // 
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Name = "filterOutNonMainSpaceArticlesToolStripMenuItem";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Text = "Filter out non main space";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.ToolTipText = "Filter out articles that are not in the main namespace";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceArticlesToolStripMenuItem_Click);
            // 
            // specialFilterToolStripMenuItem
            // 
            this.specialFilterToolStripMenuItem.Name = "specialFilterToolStripMenuItem";
            this.specialFilterToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.specialFilterToolStripMenuItem.Text = "Filter";
            this.specialFilterToolStripMenuItem.ToolTipText = "Filter articles by namespace";
            this.specialFilterToolStripMenuItem.Click += new System.EventHandler(this.specialFilterToolStripMenuItem_Click);
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            this.convertToTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.convertToTalkPagesToolStripMenuItem.Text = "Convert to talk pages";
            this.convertToTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace to talk";
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            this.convertFromTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.convertFromTalkPagesToolStripMenuItem.Text = "Convert from talk pages";
            this.convertFromTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace of talk pages to associated article";
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // sortAlphebeticallyMenuItem
            // 
            this.sortAlphebeticallyMenuItem.Name = "sortAlphebeticallyMenuItem";
            this.sortAlphebeticallyMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sortAlphebeticallyMenuItem.Text = "Sort alphabetically";
            this.sortAlphebeticallyMenuItem.ToolTipText = "Sorts the list alphabetically";
            this.sortAlphebeticallyMenuItem.Click += new System.EventHandler(this.sortAlphebeticallyMenuItem_Click);
            // 
            // saveListToTextFileToolStripMenuItem1
            // 
            this.saveListToTextFileToolStripMenuItem1.Name = "saveListToTextFileToolStripMenuItem1";
            this.saveListToTextFileToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.saveListToTextFileToolStripMenuItem1.Text = "Save list to file";
            this.saveListToTextFileToolStripMenuItem1.ToolTipText = "Saves list to a text file";
            this.saveListToTextFileToolStripMenuItem1.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem1_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(205, 6);
            // 
            // addSelectedToListToolStripMenuItem
            // 
            this.addSelectedToListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromCategoryToolStripMenuItem,
            this.fromTranscludesHereToolStripMenuItem,
            this.fromWhatlinkshereToolStripMenuItem,
            this.fromLinksOnPageToolStripMenuItem,
            this.fromImageLinksToolStripMenuItem,
            this.fromRedirectsToolStripMenuItem});
            this.addSelectedToListToolStripMenuItem.Name = "addSelectedToListToolStripMenuItem";
            this.addSelectedToListToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.addSelectedToListToolStripMenuItem.Text = "Add selected to list...";
            // 
            // fromCategoryToolStripMenuItem
            // 
            this.fromCategoryToolStripMenuItem.Name = "fromCategoryToolStripMenuItem";
            this.fromCategoryToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromCategoryToolStripMenuItem.Text = "From category";
            this.fromCategoryToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryToolStripMenuItem_Click);
            // 
            // fromTranscludesHereToolStripMenuItem
            // 
            this.fromTranscludesHereToolStripMenuItem.Name = "fromTranscludesHereToolStripMenuItem";
            this.fromTranscludesHereToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromTranscludesHereToolStripMenuItem.Text = "From transclusions";
            this.fromTranscludesHereToolStripMenuItem.Click += new System.EventHandler(this.fromTranscludesHereToolStripMenuItem_Click);
            // 
            // fromWhatlinkshereToolStripMenuItem
            // 
            this.fromWhatlinkshereToolStripMenuItem.Name = "fromWhatlinkshereToolStripMenuItem";
            this.fromWhatlinkshereToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromWhatlinkshereToolStripMenuItem.Text = "From whatlinkshere";
            this.fromWhatlinkshereToolStripMenuItem.Click += new System.EventHandler(this.fromWhatlinkshereToolStripMenuItem_Click);
            // 
            // fromLinksOnPageToolStripMenuItem
            // 
            this.fromLinksOnPageToolStripMenuItem.Name = "fromLinksOnPageToolStripMenuItem";
            this.fromLinksOnPageToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromLinksOnPageToolStripMenuItem.Text = "From links on page";
            this.fromLinksOnPageToolStripMenuItem.Click += new System.EventHandler(this.fromLinksOnPageToolStripMenuItem_Click);
            // 
            // fromImageLinksToolStripMenuItem
            // 
            this.fromImageLinksToolStripMenuItem.Name = "fromImageLinksToolStripMenuItem";
            this.fromImageLinksToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromImageLinksToolStripMenuItem.Text = "From image links";
            this.fromImageLinksToolStripMenuItem.Click += new System.EventHandler(this.fromImageLinksToolStripMenuItem_Click);
            // 
            // fromRedirectsToolStripMenuItem
            // 
            this.fromRedirectsToolStripMenuItem.Name = "fromRedirectsToolStripMenuItem";
            this.fromRedirectsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.fromRedirectsToolStripMenuItem.Text = "From redirects";
            this.fromRedirectsToolStripMenuItem.Click += new System.EventHandler(this.fromRedirectsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(205, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.ToolTipText = "Remove the selected articles";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem1
            // 
            this.clearToolStripMenuItem1.Name = "clearToolStripMenuItem1";
            this.clearToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.clearToolStripMenuItem1.Text = "Clear";
            this.clearToolStripMenuItem1.ToolTipText = "Clear the list";
            this.clearToolStripMenuItem1.Click += new System.EventHandler(this.clearToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open article in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(114, 298);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(38, 22);
            this.btnFilter.TabIndex = 36;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnRemoveArticle
            // 
            this.btnRemoveArticle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveArticle.Location = new System.Drawing.Point(4, 271);
            this.btnRemoveArticle.Name = "btnRemoveArticle";
            this.btnRemoveArticle.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveArticle.TabIndex = 33;
            this.btnRemoveArticle.Text = "Remove selected";
            this.btnRemoveArticle.UseVisualStyleBackColor = true;
            this.btnRemoveArticle.Click += new System.EventHandler(this.btnRemoveArticle_Click);
            // 
            // btnArticlesListClear
            // 
            this.btnArticlesListClear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArticlesListClear.Location = new System.Drawing.Point(156, 297);
            this.btnArticlesListClear.Name = "btnArticlesListClear";
            this.btnArticlesListClear.Size = new System.Drawing.Size(43, 23);
            this.btnArticlesListClear.TabIndex = 34;
            this.btnArticlesListClear.Text = "Clear";
            this.btnArticlesListClear.UseVisualStyleBackColor = true;
            this.btnArticlesListClear.Click += new System.EventHandler(this.btnArticlesListClear_Click);
            // 
            // btnArticlesListSave
            // 
            this.btnArticlesListSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArticlesListSave.Location = new System.Drawing.Point(130, 271);
            this.btnArticlesListSave.Name = "btnArticlesListSave";
            this.btnArticlesListSave.Size = new System.Drawing.Size(69, 23);
            this.btnArticlesListSave.TabIndex = 35;
            this.btnArticlesListSave.Text = "Save list";
            this.btnArticlesListSave.UseVisualStyleBackColor = true;
            this.btnArticlesListSave.Click += new System.EventHandler(this.btnArticlesListSave_Click);
            // 
            // btnRemoveDuplicates
            // 
            this.btnRemoveDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveDuplicates.Location = new System.Drawing.Point(4, 297);
            this.btnRemoveDuplicates.Name = "btnRemoveDuplicates";
            this.btnRemoveDuplicates.Size = new System.Drawing.Size(106, 23);
            this.btnRemoveDuplicates.TabIndex = 37;
            this.btnRemoveDuplicates.Text = "Remove duplicates";
            this.btnRemoveDuplicates.UseVisualStyleBackColor = true;
            this.btnRemoveDuplicates.Click += new System.EventHandler(this.btnRemoveDuplicates_Click);
            // 
            // lbArticles
            // 
            this.lbArticles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(4, 82);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(195, 186);
            this.lbArticles.TabIndex = 22;
            this.lbArticles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbArticles_MouseMove);
            this.lbArticles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbArticles_KeyDown);
            // 
            // ListMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.mnuListBox;
            this.Controls.Add(this.btnRemoveDuplicates);
            this.Controls.Add(this.btnRemoveArticle);
            this.Controls.Add(this.txtSelectSource);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnArticlesListSave);
            this.Controls.Add(this.btnArticlesListClear);
            this.Controls.Add(this.lbArticles);
            this.Controls.Add(this.lblSourceSelect);
            this.Controls.Add(this.txtNewArticle);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnMakeList);
            this.Controls.Add(this.lblNumberOfArticles);
            this.Name = "ListMaker";
            this.Size = new System.Drawing.Size(201, 351);
            this.mnuListBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListBox2 lbArticles;
        private System.Windows.Forms.TextBox txtSelectSource;
        private System.Windows.Forms.Label lblNumberOfArticles;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtNewArticle;
        private System.Windows.Forms.Label lblSourceSelect;
        private System.Windows.Forms.Button btnMakeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboSourceSelect;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceArticlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphebeticallyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListToTextFileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromWhatlinkshereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromLinksOnPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromImageLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromTranscludesHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.ContextMenuStrip mnuListBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromRedirectsToolStripMenuItem;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnRemoveArticle;
        private System.Windows.Forms.Button btnArticlesListClear;
        private System.Windows.Forms.Button btnArticlesListSave;
        private System.Windows.Forms.Button btnRemoveDuplicates;
    }
}
