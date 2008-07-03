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
            this.UserInputTextBox = new System.Windows.Forms.TextBox();
            this.lblNumberOfArticles = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtNewArticle = new System.Windows.Forms.TextBox();
            this.lblSourceSelect = new System.Windows.Forms.Label();
            this.btnMakeList = new System.Windows.Forms.Button();
            this.lblMakeFrom = new System.Windows.Forms.Label();
            this.cmboSourceSelect = new System.Windows.Forms.ComboBox();
            this.mnuListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHistoryInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemove = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.addSelectedToListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromCategoryrecursiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTranscludesHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromWhatlinkshereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromLinksOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromImageLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.filterOutNonMainSpaceArticlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertFromTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphebeticallyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnArticlesListSave = new System.Windows.Forms.Button();
            this.btnRemoveDuplicates = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBox2();
            this.mnuListBox.SuspendLayout();
            this.mnuRemove.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserInputTextBox
            // 
            this.UserInputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.UserInputTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.UserInputTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.UserInputTextBox.Location = new System.Drawing.Point(67, 30);
            this.UserInputTextBox.Name = "UserInputTextBox";
            this.UserInputTextBox.Size = new System.Drawing.Size(136, 20);
            this.UserInputTextBox.TabIndex = 2;
            this.UserInputTextBox.DoubleClick += new System.EventHandler(this.txtSelectSource_DoubleClick);
            this.UserInputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSelectSource_KeyDown);
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
            this.btnAdd.Size = new System.Drawing.Size(46, 23);
            this.btnAdd.TabIndex = 11;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtNewArticle
            // 
            this.txtNewArticle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewArticle.Location = new System.Drawing.Point(56, 325);
            this.txtNewArticle.Name = "txtNewArticle";
            this.txtNewArticle.Size = new System.Drawing.Size(147, 20);
            this.txtNewArticle.TabIndex = 12;
            this.txtNewArticle.DoubleClick += new System.EventHandler(this.txtNewArticle_DoubleClick);
            this.txtNewArticle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtNewArticle_MouseMove);
            this.txtNewArticle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewArticle_KeyDown);
            // 
            // lblSourceSelect
            // 
            this.lblSourceSelect.Location = new System.Drawing.Point(-3, 22);
            this.lblSourceSelect.Name = "lblSourceSelect";
            this.lblSourceSelect.Size = new System.Drawing.Size(72, 34);
            this.lblSourceSelect.TabIndex = 23;
            this.lblSourceSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMakeList
            // 
            this.btnMakeList.Enabled = false;
            this.btnMakeList.Location = new System.Drawing.Point(99, 56);
            this.btnMakeList.Name = "btnMakeList";
            this.btnMakeList.Size = new System.Drawing.Size(74, 23);
            this.btnMakeList.TabIndex = 3;
            this.btnMakeList.Tag = "Get all pages needing editting";
            this.btnMakeList.Text = "Make list";
            this.btnMakeList.Click += new System.EventHandler(this.btnMakeList_Click);
            // 
            // lblMakeFrom
            // 
            this.lblMakeFrom.AutoSize = true;
            this.lblMakeFrom.Location = new System.Drawing.Point(3, 6);
            this.lblMakeFrom.Name = "lblMakeFrom";
            this.lblMakeFrom.Size = new System.Drawing.Size(60, 13);
            this.lblMakeFrom.TabIndex = 21;
            this.lblMakeFrom.Text = "Make from:";
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboSourceSelect.DropDownHeight = 250;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.IntegralHeight = false;
            this.cmboSourceSelect.Location = new System.Drawing.Point(67, 3);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(136, 21);
            this.cmboSourceSelect.Sorted = true;
            this.cmboSourceSelect.TabIndex = 1;
            this.cmboSourceSelect.Tag = "Source of pages to edit";
            this.cmboSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmboSourceSelect_SelectedIndexChanged);
            // 
            // mnuListBox
            // 
            this.mnuListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem,
            this.openHistoryInBrowserToolStripMenuItem,
            this.toolStripSeparator4,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator3,
            this.selectMnu,
            this.removeToolStripMenuItem,
            this.toolStripSeparator14,
            this.addSelectedToListToolStripMenuItem,
            this.toolStripSeparator5,
            this.moveToTopToolStripMenuItem,
            this.moveToBottomToolStripMenuItem,
            this.toolStripSeparator2,
            this.filterOutNonMainSpaceArticlesToolStripMenuItem,
            this.convertToTalkPagesToolStripMenuItem,
            this.convertFromTalkPagesToolStripMenuItem,
            this.specialFilterToolStripMenuItem,
            this.saveListToFileToolStripMenuItem,
            this.sortAlphebeticallyMenuItem});
            this.mnuListBox.Name = "contextMenuStrip2";
            this.mnuListBox.Size = new System.Drawing.Size(213, 386);
            this.mnuListBox.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListBox_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openInBrowserToolStripMenuItem.Text = "&Open page in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            this.openHistoryInBrowserToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.openHistoryInBrowserToolStripMenuItem.Text = "Open &history in browser";
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(209, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(209, 6);
            // 
            // selectMnu
            // 
            this.selectMnu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.invertSelectionToolStripMenuItem});
            this.selectMnu.Name = "selectMnu";
            this.selectMnu.Size = new System.Drawing.Size(212, 22);
            this.selectMnu.Text = "&Select";
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectAllToolStripMenuItem.Text = "&All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectNoneToolStripMenuItem.Text = "&None";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // invertSelectionToolStripMenuItem
            // 
            this.invertSelectionToolStripMenuItem.Name = "invertSelectionToolStripMenuItem";
            this.invertSelectionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.I)));
            this.invertSelectionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.invertSelectionToolStripMenuItem.Text = "&Inverse";
            this.invertSelectionToolStripMenuItem.Click += new System.EventHandler(this.invertSelectionToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.DropDown = this.mnuRemove;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // mnuRemove
            // 
            this.mnuRemove.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.duplicatesToolStripMenuItem});
            this.mnuRemove.Name = "mnuRemove";
            this.mnuRemove.OwnerItem = this.removeToolStripMenuItem;
            this.mnuRemove.Size = new System.Drawing.Size(143, 70);
            // 
            // selectedToolStripMenuItem
            // 
            this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
            this.selectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.selectedToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.selectedToolStripMenuItem.Text = "Selected";
            this.selectedToolStripMenuItem.ToolTipText = "Remove the selected articles";
            this.selectedToolStripMenuItem.Click += new System.EventHandler(this.selectedToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.clearToolStripMenuItem.Text = "All";
            this.clearToolStripMenuItem.ToolTipText = "Clear the list";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem1_Click);
            // 
            // duplicatesToolStripMenuItem
            // 
            this.duplicatesToolStripMenuItem.Name = "duplicatesToolStripMenuItem";
            this.duplicatesToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.duplicatesToolStripMenuItem.Text = "Duplicates";
            this.duplicatesToolStripMenuItem.Click += new System.EventHandler(this.duplicatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(209, 6);
            // 
            // addSelectedToListToolStripMenuItem
            // 
            this.addSelectedToListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromCategoryToolStripMenuItem,
            this.fromCategoryrecursiveToolStripMenuItem,
            this.fromTranscludesHereToolStripMenuItem,
            this.fromWhatlinkshereToolStripMenuItem,
            this.fromLinksOnPageToolStripMenuItem,
            this.fromImageLinksToolStripMenuItem,
            this.fromRedirectsToolStripMenuItem});
            this.addSelectedToListToolStripMenuItem.Name = "addSelectedToListToolStripMenuItem";
            this.addSelectedToListToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.addSelectedToListToolStripMenuItem.Text = "Add selected to list from...";
            // 
            // fromCategoryToolStripMenuItem
            // 
            this.fromCategoryToolStripMenuItem.Name = "fromCategoryToolStripMenuItem";
            this.fromCategoryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromCategoryToolStripMenuItem.Text = "&Category";
            this.fromCategoryToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryToolStripMenuItem_Click);
            // 
            // fromCategoryrecursiveToolStripMenuItem
            // 
            this.fromCategoryrecursiveToolStripMenuItem.Name = "fromCategoryrecursiveToolStripMenuItem";
            this.fromCategoryrecursiveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromCategoryrecursiveToolStripMenuItem.Text = "C&ategory (recursive)";
            this.fromCategoryrecursiveToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryrecursiveToolStripMenuItem_Click);
            // 
            // fromTranscludesHereToolStripMenuItem
            // 
            this.fromTranscludesHereToolStripMenuItem.Name = "fromTranscludesHereToolStripMenuItem";
            this.fromTranscludesHereToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromTranscludesHereToolStripMenuItem.Text = "&Transclusions";
            this.fromTranscludesHereToolStripMenuItem.Click += new System.EventHandler(this.fromTranscludesHereToolStripMenuItem_Click);
            // 
            // fromWhatlinkshereToolStripMenuItem
            // 
            this.fromWhatlinkshereToolStripMenuItem.Name = "fromWhatlinkshereToolStripMenuItem";
            this.fromWhatlinkshereToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromWhatlinkshereToolStripMenuItem.Text = "&Whatlinkshere page";
            this.fromWhatlinkshereToolStripMenuItem.Click += new System.EventHandler(this.fromWhatlinkshereToolStripMenuItem_Click);
            // 
            // fromLinksOnPageToolStripMenuItem
            // 
            this.fromLinksOnPageToolStripMenuItem.Name = "fromLinksOnPageToolStripMenuItem";
            this.fromLinksOnPageToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromLinksOnPageToolStripMenuItem.Text = "&Links on page";
            this.fromLinksOnPageToolStripMenuItem.Click += new System.EventHandler(this.fromLinksOnPageToolStripMenuItem_Click);
            // 
            // fromImageLinksToolStripMenuItem
            // 
            this.fromImageLinksToolStripMenuItem.Name = "fromImageLinksToolStripMenuItem";
            this.fromImageLinksToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromImageLinksToolStripMenuItem.Text = "Image &file links";
            this.fromImageLinksToolStripMenuItem.Click += new System.EventHandler(this.fromImageLinksToolStripMenuItem_Click);
            // 
            // fromRedirectsToolStripMenuItem
            // 
            this.fromRedirectsToolStripMenuItem.Name = "fromRedirectsToolStripMenuItem";
            this.fromRedirectsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fromRedirectsToolStripMenuItem.Text = "&Redirects";
            this.fromRedirectsToolStripMenuItem.Click += new System.EventHandler(this.fromRedirectsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(209, 6);
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.moveToTopToolStripMenuItem.Text = "Move to top";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.moveToBottomToolStripMenuItem.Text = "Move to bottom";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(209, 6);
            // 
            // filterOutNonMainSpaceArticlesToolStripMenuItem
            // 
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Name = "filterOutNonMainSpaceArticlesToolStripMenuItem";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Text = "Remove non-&main space";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.ToolTipText = "Filter out pages that are not in the main namespace";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceArticlesToolStripMenuItem_Click);
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            this.convertToTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.convertToTalkPagesToolStripMenuItem.Text = "Con&vert to talk pages";
            this.convertToTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace to talk";
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            this.convertFromTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.convertFromTalkPagesToolStripMenuItem.Text = "Convert &from talk pages";
            this.convertFromTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace of talk pages to associated article";
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // specialFilterToolStripMenuItem
            // 
            this.specialFilterToolStripMenuItem.Name = "specialFilterToolStripMenuItem";
            this.specialFilterToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.specialFilterToolStripMenuItem.Text = "Fi&lter";
            this.specialFilterToolStripMenuItem.ToolTipText = "Filter articles by namespace";
            this.specialFilterToolStripMenuItem.Click += new System.EventHandler(this.specialFilterToolStripMenuItem_Click);
            // 
            // saveListToFileToolStripMenuItem
            // 
            this.saveListToFileToolStripMenuItem.Name = "saveListToFileToolStripMenuItem";
            this.saveListToFileToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.saveListToFileToolStripMenuItem.Text = "Save list";
            this.saveListToFileToolStripMenuItem.ToolTipText = "Saves list to a text file";
            this.saveListToFileToolStripMenuItem.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem1_Click);
            // 
            // sortAlphebeticallyMenuItem
            // 
            this.sortAlphebeticallyMenuItem.Name = "sortAlphebeticallyMenuItem";
            this.sortAlphebeticallyMenuItem.Size = new System.Drawing.Size(212, 22);
            this.sortAlphebeticallyMenuItem.Text = "Sort alpha&betically";
            this.sortAlphebeticallyMenuItem.ToolTipText = "Sorts the list alphabetically";
            this.sortAlphebeticallyMenuItem.Click += new System.EventHandler(this.sortAlphebeticallyMenuItem_Click);
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
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(179, 56);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(24, 23);
            this.btnStop.TabIndex = 4;
            this.tooltip.SetToolTip(this.btnStop, "Stop making list");
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(116, 297);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(42, 23);
            this.btnFilter.TabIndex = 9;
            this.btnFilter.Text = "Filter";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.ContextMenuStrip = this.mnuRemove;
            this.btnRemove.Location = new System.Drawing.Point(4, 297);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemoveArticle_Click);
            // 
            // btnArticlesListSave
            // 
            this.btnArticlesListSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArticlesListSave.Location = new System.Drawing.Point(160, 297);
            this.btnArticlesListSave.Name = "btnArticlesListSave";
            this.btnArticlesListSave.Size = new System.Drawing.Size(42, 23);
            this.btnArticlesListSave.TabIndex = 7;
            this.btnArticlesListSave.Text = "Save";
            this.btnArticlesListSave.Click += new System.EventHandler(this.btnArticlesListSave_Click);
            // 
            // btnRemoveDuplicates
            // 
            this.btnRemoveDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveDuplicates.ContextMenuStrip = this.mnuRemove;
            this.btnRemoveDuplicates.Font = new System.Drawing.Font("Wingdings 3", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnRemoveDuplicates.Location = new System.Drawing.Point(82, 297);
            this.btnRemoveDuplicates.Name = "btnRemoveDuplicates";
            this.btnRemoveDuplicates.Size = new System.Drawing.Size(28, 23);
            this.btnRemoveDuplicates.TabIndex = 8;
            this.btnRemoveDuplicates.Text = "q";
            this.btnRemoveDuplicates.Click += new System.EventHandler(this.btnRemoveDuplicates_Click);
            // 
            // lbArticles
            // 
            this.lbArticles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArticles.ContextMenuStrip = this.mnuListBox;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(3, 82);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(200, 212);
            this.lbArticles.TabIndex = 5;
            this.lbArticles.DoubleClick += new System.EventHandler(this.lbArticles_DoubleClick);
            this.lbArticles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbArticles_MouseMove);
            this.lbArticles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbArticles_KeyDown);
            // 
            // ListMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblMakeFrom);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.UserInputTextBox);
            this.Controls.Add(this.lblSourceSelect);
            this.Controls.Add(this.lbArticles);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnRemoveDuplicates);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtNewArticle);
            this.Controls.Add(this.lblNumberOfArticles);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnArticlesListSave);
            this.Controls.Add(this.btnMakeList);
            this.Controls.Add(this.btnFilter);
            this.Name = "ListMaker";
            this.Size = new System.Drawing.Size(205, 349);
            this.mnuListBox.ResumeLayout(false);
            this.mnuRemove.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblMakeFrom;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceArticlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphebeticallyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromWhatlinkshereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromLinksOnPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromImageLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromTranscludesHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ToolTip tooltip;
        public System.Windows.Forms.ContextMenuStrip mnuListBox;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromRedirectsToolStripMenuItem;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnRemove;
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
        public System.Windows.Forms.TextBox UserInputTextBox;
        private System.Windows.Forms.ToolStripMenuItem openHistoryInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        public System.Windows.Forms.ComboBox cmboSourceSelect;
        private System.Windows.Forms.ToolStripMenuItem fromCategoryrecursiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectMnu;
        private System.Windows.Forms.ContextMenuStrip mnuRemove;
    }
}
