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
            this.lblNumOfPages = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.lblUserInput = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblSourceSelect = new System.Windows.Forms.Label();
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
            this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterOutNonMainSpaceArticlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.addSelectedToListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoriesOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromCategoryrecursiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromImageLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imagesOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromLinksOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTranscludesHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromWhatlinkshereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.convertToTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertFromTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.specialFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBox2();
            this.mnuListBox.SuspendLayout();
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
            this.UserInputTextBox.Size = new System.Drawing.Size(138, 20);
            this.UserInputTextBox.TabIndex = 3;
            this.UserInputTextBox.Text = "Specify the item that the generator should use to make the list";
            this.UserInputTextBox.DoubleClick += new System.EventHandler(this.txtSelectSource_DoubleClick);
            this.UserInputTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSelectSource_KeyDown);
            // 
            // lblNumOfPages
            // 
            this.lblNumOfPages.Location = new System.Drawing.Point(3, 57);
            this.lblNumOfPages.Name = "lblNumOfPages";
            this.lblNumOfPages.Size = new System.Drawing.Size(74, 20);
            this.lblNumOfPages.TabIndex = 4;
            this.lblNumOfPages.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(185, 81);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(20, 21);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "+";
            this.tooltip.SetToolTip(this.btnAdd, "Append to list as a page");
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtPage
            // 
            this.txtPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPage.Location = new System.Drawing.Point(3, 82);
            this.txtPage.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(179, 20);
            this.txtPage.TabIndex = 7;
            this.txtPage.DoubleClick += new System.EventHandler(this.txtNewArticle_DoubleClick);
            this.txtPage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.txtNewArticle_MouseMove);
            this.txtPage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewArticle_KeyDown);
            // 
            // lblUserInput
            // 
            this.lblUserInput.Location = new System.Drawing.Point(-3, 22);
            this.lblUserInput.Name = "lblUserInput";
            this.lblUserInput.Size = new System.Drawing.Size(72, 34);
            this.lblUserInput.TabIndex = 2;
            this.lblUserInput.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(75, 51);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(74, 25);
            this.btnGenerate.TabIndex = 5;
            this.btnGenerate.Text = "Make list";
            this.tooltip.SetToolTip(this.btnGenerate, "Generate a list from the selected options above");
            this.btnGenerate.Click += new System.EventHandler(this.btnMakeList_Click);
            // 
            // lblSourceSelect
            // 
            this.lblSourceSelect.AutoSize = true;
            this.lblSourceSelect.Location = new System.Drawing.Point(3, 6);
            this.lblSourceSelect.Name = "lblSourceSelect";
            this.lblSourceSelect.Size = new System.Drawing.Size(44, 13);
            this.lblSourceSelect.TabIndex = 0;
            this.lblSourceSelect.Text = "Source:";
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboSourceSelect.DropDownHeight = 250;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.DropDownWidth = 200;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.IntegralHeight = false;
            this.cmboSourceSelect.Location = new System.Drawing.Point(46, 3);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(159, 21);
            this.cmboSourceSelect.Sorted = true;
            this.cmboSourceSelect.TabIndex = 1;
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
            this.convertToTalkPagesToolStripMenuItem,
            this.convertFromTalkPagesToolStripMenuItem,
            this.toolStripSeparator1,
            this.specialFilterToolStripMenuItem,
            this.saveListToFileToolStripMenuItem,
            this.sortAlphaMenuItem});
            this.mnuListBox.Name = "contextMenuStrip2";
            this.mnuListBox.Size = new System.Drawing.Size(276, 370);
            this.mnuListBox.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListBox_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+P";
            this.openInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.P)));
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.openInBrowserToolStripMenuItem.Text = "&Open page in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            this.openHistoryInBrowserToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Shift+H";
            this.openHistoryInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.H)));
            this.openHistoryInBrowserToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.openHistoryInBrowserToolStripMenuItem.Text = "Open &history in browser";
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(272, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.pasteToolStripMenuItem.Text = "&Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(272, 6);
            // 
            // selectMnu
            // 
            this.selectMnu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.invertSelectionToolStripMenuItem});
            this.selectMnu.Name = "selectMnu";
            this.selectMnu.Size = new System.Drawing.Size(275, 22);
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
            this.removeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectedToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.duplicatesToolStripMenuItem,
            this.filterOutNonMainSpaceArticlesToolStripMenuItem});
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // selectedToolStripMenuItem
            // 
            this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
            this.selectedToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.selectedToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.selectedToolStripMenuItem.Text = "&Selected";
            this.selectedToolStripMenuItem.ToolTipText = "Remove the selected articles";
            this.selectedToolStripMenuItem.Click += new System.EventHandler(this.selectedToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.clearToolStripMenuItem.Text = "&All";
            this.clearToolStripMenuItem.ToolTipText = "Clear the list";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem1_Click);
            // 
            // duplicatesToolStripMenuItem
            // 
            this.duplicatesToolStripMenuItem.Name = "duplicatesToolStripMenuItem";
            this.duplicatesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.duplicatesToolStripMenuItem.Text = "&Duplicates";
            this.duplicatesToolStripMenuItem.Click += new System.EventHandler(this.duplicatesToolStripMenuItem_Click);
            // 
            // filterOutNonMainSpaceArticlesToolStripMenuItem
            // 
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Name = "filterOutNonMainSpaceArticlesToolStripMenuItem";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Text = "Non-&main space";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.ToolTipText = "Filter out pages that are not in the main namespace";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceArticlesToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(272, 6);
            // 
            // addSelectedToListToolStripMenuItem
            // 
            this.addSelectedToListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.categoriesOnPageToolStripMenuItem,
            this.fromCategoryToolStripMenuItem,
            this.fromCategoryrecursiveToolStripMenuItem,
            this.fromImageLinksToolStripMenuItem,
            this.imagesOnPageToolStripMenuItem,
            this.fromLinksOnPageToolStripMenuItem,
            this.fromTranscludesHereToolStripMenuItem,
            this.fromRedirectsToolStripMenuItem,
            this.fromWhatlinkshereToolStripMenuItem});
            this.addSelectedToListToolStripMenuItem.Name = "addSelectedToListToolStripMenuItem";
            this.addSelectedToListToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.addSelectedToListToolStripMenuItem.Text = "Add selected to list from...";
            // 
            // categoriesOnPageToolStripMenuItem
            // 
            this.categoriesOnPageToolStripMenuItem.Name = "categoriesOnPageToolStripMenuItem";
            this.categoriesOnPageToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.categoriesOnPageToolStripMenuItem.Text = "Categories on page";
            this.categoriesOnPageToolStripMenuItem.Click += new System.EventHandler(this.categoriesOnPageToolStripMenuItem_Click);
            // 
            // fromCategoryToolStripMenuItem
            // 
            this.fromCategoryToolStripMenuItem.Name = "fromCategoryToolStripMenuItem";
            this.fromCategoryToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromCategoryToolStripMenuItem.Text = "&Category";
            this.fromCategoryToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryToolStripMenuItem_Click);
            // 
            // fromCategoryrecursiveToolStripMenuItem
            // 
            this.fromCategoryrecursiveToolStripMenuItem.Name = "fromCategoryrecursiveToolStripMenuItem";
            this.fromCategoryrecursiveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromCategoryrecursiveToolStripMenuItem.Text = "C&ategory (recursive)";
            this.fromCategoryrecursiveToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryrecursiveToolStripMenuItem_Click);
            // 
            // fromImageLinksToolStripMenuItem
            // 
            this.fromImageLinksToolStripMenuItem.Name = "fromImageLinksToolStripMenuItem";
            this.fromImageLinksToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromImageLinksToolStripMenuItem.Text = "Image &file links";
            this.fromImageLinksToolStripMenuItem.Click += new System.EventHandler(this.fromImageLinksToolStripMenuItem_Click);
            // 
            // imagesOnPageToolStripMenuItem
            // 
            this.imagesOnPageToolStripMenuItem.Name = "imagesOnPageToolStripMenuItem";
            this.imagesOnPageToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.imagesOnPageToolStripMenuItem.Text = "Images on page";
            this.imagesOnPageToolStripMenuItem.Click += new System.EventHandler(this.imagesOnPageToolStripMenuItem_Click);
            // 
            // fromLinksOnPageToolStripMenuItem
            // 
            this.fromLinksOnPageToolStripMenuItem.Name = "fromLinksOnPageToolStripMenuItem";
            this.fromLinksOnPageToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromLinksOnPageToolStripMenuItem.Text = "&Links on page";
            this.fromLinksOnPageToolStripMenuItem.Click += new System.EventHandler(this.fromLinksOnPageToolStripMenuItem_Click);
            // 
            // fromTranscludesHereToolStripMenuItem
            // 
            this.fromTranscludesHereToolStripMenuItem.Name = "fromTranscludesHereToolStripMenuItem";
            this.fromTranscludesHereToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromTranscludesHereToolStripMenuItem.Text = "&Transclusions";
            this.fromTranscludesHereToolStripMenuItem.Click += new System.EventHandler(this.fromTranscludesHereToolStripMenuItem_Click);
            // 
            // fromRedirectsToolStripMenuItem
            // 
            this.fromRedirectsToolStripMenuItem.Name = "fromRedirectsToolStripMenuItem";
            this.fromRedirectsToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromRedirectsToolStripMenuItem.Text = "&Redirects";
            this.fromRedirectsToolStripMenuItem.Click += new System.EventHandler(this.fromRedirectsToolStripMenuItem_Click);
            // 
            // fromWhatlinkshereToolStripMenuItem
            // 
            this.fromWhatlinkshereToolStripMenuItem.Name = "fromWhatlinkshereToolStripMenuItem";
            this.fromWhatlinkshereToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.fromWhatlinkshereToolStripMenuItem.Text = "&What links here page";
            this.fromWhatlinkshereToolStripMenuItem.Click += new System.EventHandler(this.fromWhatlinkshereToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(272, 6);
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.moveToTopToolStripMenuItem.Text = "Move to top";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.moveToBottomToolStripMenuItem.Text = "Move to bottom";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(272, 6);
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            this.convertToTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.convertToTalkPagesToolStripMenuItem.Text = "Con&vert to talk pages";
            this.convertToTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace to talk";
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            this.convertFromTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.convertFromTalkPagesToolStripMenuItem.Text = "Convert &from talk pages";
            this.convertFromTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace of talk pages to associated article";
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(272, 6);
            // 
            // specialFilterToolStripMenuItem
            // 
            this.specialFilterToolStripMenuItem.Name = "specialFilterToolStripMenuItem";
            this.specialFilterToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.specialFilterToolStripMenuItem.Text = "Fi&lter";
            this.specialFilterToolStripMenuItem.ToolTipText = "Filter articles by namespace";
            this.specialFilterToolStripMenuItem.Click += new System.EventHandler(this.specialFilterToolStripMenuItem_Click);
            // 
            // saveListToFileToolStripMenuItem
            // 
            this.saveListToFileToolStripMenuItem.Name = "saveListToFileToolStripMenuItem";
            this.saveListToFileToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.saveListToFileToolStripMenuItem.Text = "Save list";
            this.saveListToFileToolStripMenuItem.ToolTipText = "Saves list to a text file";
            this.saveListToFileToolStripMenuItem.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem1_Click);
            // 
            // sortAlphaMenuItem
            // 
            this.sortAlphaMenuItem.Name = "sortAlphaMenuItem";
            this.sortAlphaMenuItem.Size = new System.Drawing.Size(275, 22);
            this.sortAlphaMenuItem.Text = "Sort alpha&betically";
            this.sortAlphaMenuItem.ToolTipText = "Sorts the list alphabetically";
            this.sortAlphaMenuItem.Click += new System.EventHandler(this.sortAlphebeticallyMenuItem_Click);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Image = ((System.Drawing.Image)(resources.GetObject("btnStop.Image")));
            this.btnStop.Location = new System.Drawing.Point(155, 51);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(24, 25);
            this.btnStop.TabIndex = 6;
            this.tooltip.SetToolTip(this.btnStop, "Abort generating the list");
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilter.Location = new System.Drawing.Point(107, 321);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(98, 25);
            this.btnFilter.TabIndex = 12;
            this.btnFilter.Text = "Filter";
            this.tooltip.SetToolTip(this.btnFilter, "Apply a filter to substract or add to the list");
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(3, 321);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(98, 25);
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "&Remove";
            this.tooltip.SetToolTip(this.btnRemove, "Remove the selected item");
            this.btnRemove.Click += new System.EventHandler(this.btnRemoveArticle_Click);
            // 
            // lbArticles
            // 
            this.lbArticles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArticles.ContextMenuStrip = this.mnuListBox;
            this.lbArticles.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(3, 105);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(202, 212);
            this.lbArticles.TabIndex = 9;
            this.lbArticles.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbArticles_DrawItem);
            this.lbArticles.DoubleClick += new System.EventHandler(this.lbArticles_DoubleClick);
            this.lbArticles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbArticles_MouseMove);
            this.lbArticles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbArticles_KeyDown);
            // 
            // ListMaker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbArticles);
            this.Controls.Add(this.txtPage);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lblNumOfPages);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.UserInputTextBox);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.lblSourceSelect);
            this.Controls.Add(this.lblUserInput);
            this.Name = "ListMaker";
            this.Size = new System.Drawing.Size(205, 349);
            this.mnuListBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WikiFunctions.Controls.Lists.ListBox2 lbArticles;
        private System.Windows.Forms.Label lblNumOfPages;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtPage;
        private System.Windows.Forms.Label lblUserInput;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblSourceSelect;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceArticlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphaMenuItem;
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
        private System.Windows.Forms.ToolTip tooltip;
        public System.Windows.Forms.ContextMenuStrip mnuListBox;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromRedirectsToolStripMenuItem;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnRemove;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem imagesOnPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoriesOnPageToolStripMenuItem;

    }
}
