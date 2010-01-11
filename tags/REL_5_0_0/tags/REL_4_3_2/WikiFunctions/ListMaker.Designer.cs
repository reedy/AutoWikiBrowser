using System.Drawing;
using WikiFunctions.Lists;

namespace WikiFunctions.Controls.Lists
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListMaker));
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHistoryInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnRemoveArticle = new System.Windows.Forms.Button();
            this.btnArticlesListClear = new System.Windows.Forms.Button();
            this.btnArticlesListSave = new System.Windows.Forms.Button();
            this.btnRemoveDuplicates = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBox2();
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
            this.txtSelectSource.Location = new System.Drawing.Point(71, 30);
            this.txtSelectSource.Name = "txtSelectSource";
            this.txtSelectSource.Size = new System.Drawing.Size(116, 20);
            this.txtSelectSource.TabIndex = 2;
            this.txtSelectSource.DoubleClick += new System.EventHandler(this.txtSelectSource_DoubleClick);
            this.txtSelectSource.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSelectSource_KeyDown);
            // 
            // lblNumberOfArticles
            // 
            this.lblNumberOfArticles.Location = new System.Drawing.Point(3, 57);
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
            this.btnAdd.Size = new System.Drawing.Size(37, 23);
            this.btnAdd.TabIndex = 11;
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
            this.txtNewArticle.Size = new System.Drawing.Size(139, 20);
            this.txtNewArticle.TabIndex = 12;
            this.txtNewArticle.DoubleClick += new System.EventHandler(this.txtNewArticle_DoubleClick);
            this.txtNewArticle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtNewArticle_MouseMove);
            this.txtNewArticle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewArticle_KeyDown);
            // 
            // lblSourceSelect
            // 
            this.lblSourceSelect.Location = new System.Drawing.Point(-3, 22);
            this.lblSourceSelect.Name = "lblSourceSelect";
            this.lblSourceSelect.Size = new System.Drawing.Size(75, 34);
            this.lblSourceSelect.TabIndex = 23;
            this.lblSourceSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMakeList
            // 
            this.btnMakeList.Enabled = false;
            this.btnMakeList.Location = new System.Drawing.Point(83, 56);
            this.btnMakeList.Name = "btnMakeList";
            this.btnMakeList.Size = new System.Drawing.Size(74, 23);
            this.btnMakeList.TabIndex = 3;
            this.btnMakeList.Tag = "Get all pages needing editting";
            this.btnMakeList.Text = "Make list";
            this.btnMakeList.UseVisualStyleBackColor = true;
            this.btnMakeList.Click += new System.EventHandler(this.btnMakeList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Make from";
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboSourceSelect.DropDownHeight = 250;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.IntegralHeight = false;
            this.cmboSourceSelect.Items.AddRange(new object[] {
            "Category",
            "Category (recursive)",
            "What links here",
            "What transcludes page",
            "Links on page",
            "Images on page",
            "Transclusions on page",
            "Text file",
            "Google search",
            "User contribs",
            "User contribs (all)",
            "Special page",
            "Image file links",
            "Database dump",
            "My watchlist",
            "Wiki search",
            "Redirects"});
            this.cmboSourceSelect.Location = new System.Drawing.Point(71, 2);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(116, 21);
            this.cmboSourceSelect.TabIndex = 1;
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
            this.toolStripSeparator2,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator3,
            this.selectAllToolStripMenuItem,
            this.invertSelectionToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.toolStripSeparator14,
            this.addSelectedToListToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeToolStripMenuItem,
            this.clearToolStripMenuItem1,
            this.toolStripSeparator1,
            this.moveToTopToolStripMenuItem,
            this.moveToBottomToolStripMenuItem,
            this.toolStripSeparator4,
            this.openInBrowserToolStripMenuItem,
            this.openHistoryInBrowserToolStripMenuItem});
            this.mnuListBox.Name = "contextMenuStrip2";
            this.mnuListBox.Size = new System.Drawing.Size(209, 458);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.cutToolStripMenuItem.Text = "Cut selection";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.copyToolStripMenuItem.Text = "Copy selection";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(205, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.invertSelectionToolStripMenuItem.Text = "Invert selection";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.selectNoneToolStripMenuItem.Text = "Select none";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
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
            this.fromCategoryToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromCategoryToolStripMenuItem.Text = "From category";
            this.fromCategoryToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryToolStripMenuItem_Click);
            // 
            // fromTranscludesHereToolStripMenuItem
            // 
            this.fromTranscludesHereToolStripMenuItem.Name = "fromTranscludesHereToolStripMenuItem";
            this.fromTranscludesHereToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromTranscludesHereToolStripMenuItem.Text = "From transclusions";
            this.fromTranscludesHereToolStripMenuItem.Click += new System.EventHandler(this.fromTranscludesHereToolStripMenuItem_Click);
            // 
            // fromWhatlinkshereToolStripMenuItem
            // 
            this.fromWhatlinkshereToolStripMenuItem.Name = "fromWhatlinkshereToolStripMenuItem";
            this.fromWhatlinkshereToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromWhatlinkshereToolStripMenuItem.Text = "From whatlinkshere";
            this.fromWhatlinkshereToolStripMenuItem.Click += new System.EventHandler(this.fromWhatlinkshereToolStripMenuItem_Click);
            // 
            // fromLinksOnPageToolStripMenuItem
            // 
            this.fromLinksOnPageToolStripMenuItem.Name = "fromLinksOnPageToolStripMenuItem";
            this.fromLinksOnPageToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromLinksOnPageToolStripMenuItem.Text = "From links on page";
            this.fromLinksOnPageToolStripMenuItem.Click += new System.EventHandler(this.fromLinksOnPageToolStripMenuItem_Click);
            // 
            // fromImageLinksToolStripMenuItem
            // 
            this.fromImageLinksToolStripMenuItem.Name = "fromImageLinksToolStripMenuItem";
            this.fromImageLinksToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromImageLinksToolStripMenuItem.Text = "From image file links";
            this.fromImageLinksToolStripMenuItem.Click += new System.EventHandler(this.fromImageLinksToolStripMenuItem_Click);
            // 
            // fromRedirectsToolStripMenuItem
            // 
            this.fromRedirectsToolStripMenuItem.Name = "fromRedirectsToolStripMenuItem";
            this.fromRedirectsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
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
            this.removeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedToolStripMenuItem,
            this.duplicatesToolStripMenuItem});
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.ToolTipText = "Remove the selected articles";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // selectedToolStripMenuItem
            // 
            this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
            this.selectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.selectedToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.selectedToolStripMenuItem.Text = "Selected";
            this.selectedToolStripMenuItem.Click += new System.EventHandler(this.selectedToolStripMenuItem_Click);
            // 
            // duplicatesToolStripMenuItem
            // 
            this.duplicatesToolStripMenuItem.Name = "duplicatesToolStripMenuItem";
            this.duplicatesToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.duplicatesToolStripMenuItem.Text = "Duplicates";
            this.duplicatesToolStripMenuItem.Click += new System.EventHandler(this.duplicatesToolStripMenuItem_Click);
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
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.moveToTopToolStripMenuItem.Text = "Move to top";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.moveToBottomToolStripMenuItem.Text = "Move to bottom";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(205, 6);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open article in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            this.openHistoryInBrowserToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.openHistoryInBrowserToolStripMenuItem.Text = "Open history in browser";
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt|CSV (Comma Seperated Values" +
                ")|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(163, 56);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(24, 23);
            this.btnStop.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnStop, "Stop making list");
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(101, 297);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(41, 22);
            this.btnFilter.TabIndex = 9;
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
            this.btnRemoveArticle.Size = new System.Drawing.Size(115, 23);
            this.btnRemoveArticle.TabIndex = 6;
            this.btnRemoveArticle.Text = "Remove selected";
            this.btnRemoveArticle.UseVisualStyleBackColor = true;
            this.btnRemoveArticle.Click += new System.EventHandler(this.btnRemoveArticle_Click);
            // 
            // btnArticlesListClear
            // 
            this.btnArticlesListClear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArticlesListClear.Location = new System.Drawing.Point(148, 297);
            this.btnArticlesListClear.Name = "btnArticlesListClear";
            this.btnArticlesListClear.Size = new System.Drawing.Size(43, 23);
            this.btnArticlesListClear.TabIndex = 10;
            this.btnArticlesListClear.Text = "Clear";
            this.btnArticlesListClear.UseVisualStyleBackColor = true;
            this.btnArticlesListClear.Click += new System.EventHandler(this.btnArticlesListClear_Click);
            // 
            // btnArticlesListSave
            // 
            this.btnArticlesListSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArticlesListSave.Location = new System.Drawing.Point(125, 271);
            this.btnArticlesListSave.Name = "btnArticlesListSave";
            this.btnArticlesListSave.Size = new System.Drawing.Size(69, 23);
            this.btnArticlesListSave.TabIndex = 7;
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
            this.btnRemoveDuplicates.Size = new System.Drawing.Size(91, 23);
            this.btnRemoveDuplicates.TabIndex = 8;
            this.btnRemoveDuplicates.Text = "Remove dupes";
            this.btnRemoveDuplicates.UseVisualStyleBackColor = true;
            this.btnRemoveDuplicates.Click += new System.EventHandler(this.btnRemoveDuplicates_Click);
            // 
            // lbArticles
            // 
            this.lbArticles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArticles.ContextMenuStrip = this.mnuListBox;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(4, 82);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(190, 186);
            this.lbArticles.TabIndex = 5;
            this.lbArticles.DoubleClick += new System.EventHandler(this.lbArticles_DoubleClick);
            this.lbArticles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbArticles_MouseMove);
            this.lbArticles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbArticles_KeyDown);
            // 
            // ListMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRemoveArticle);
            this.Controls.Add(this.btnRemoveDuplicates);
            this.Controls.Add(this.txtSelectSource);
            this.Controls.Add(this.lbArticles);
            this.Controls.Add(this.btnArticlesListClear);
            this.Controls.Add(this.btnArticlesListSave);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.txtNewArticle);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.lblSourceSelect);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblNumberOfArticles);
            this.Controls.Add(this.btnMakeList);
            this.Name = "ListMaker";
            this.Size = new System.Drawing.Size(196, 350);
            this.mnuListBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WikiFunctions.Controls.Lists.ListBox2 lbArticles;
        private System.Windows.Forms.Label lblNumberOfArticles;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtNewArticle;
        private System.Windows.Forms.Label lblSourceSelect;
        private System.Windows.Forms.Button btnMakeList;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ToolStripMenuItem selectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem duplicatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertSelectionToolStripMenuItem;
        public System.Windows.Forms.TextBox txtSelectSource;
        public System.Windows.Forms.ComboBox cmboSourceSelect;
        private System.Windows.Forms.ToolStripMenuItem openHistoryInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}
