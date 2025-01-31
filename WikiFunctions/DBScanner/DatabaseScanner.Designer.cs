using System.Windows.Forms;

namespace WikiFunctions.DBScanner
{
    partial class DatabaseScanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseScanner));
            this.txtList = new System.Windows.Forms.TextBox();
            this.openXMLDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.gbText = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.chkArticleDoesContain = new System.Windows.Forms.CheckBox();
            this.txtArticleDoesContain = new System.Windows.Forms.TextBox();
            this.chkArticleDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtArticleDoesNotContain = new System.Windows.Forms.TextBox();
            this.chkArticleRegex = new System.Windows.Forms.CheckBox();
            this.chkArticleCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkSingle = new System.Windows.Forms.CheckBox();
            this.chkMulti = new System.Windows.Forms.CheckBox();
            this.chkIgnoreComments = new System.Windows.Forms.CheckBox();
            this.lblLength = new System.Windows.Forms.Label();
            this.nudLength = new System.Windows.Forms.NumericUpDown();
            this.cmboLength = new System.Windows.Forms.ComboBox();
            this.lblLinks = new System.Windows.Forms.Label();
            this.cmboLinks = new System.Windows.Forms.ComboBox();
            this.nudLinks = new System.Windows.Forms.NumericUpDown();
            this.lblWords = new System.Windows.Forms.Label();
            this.cmboWords = new System.Windows.Forms.ComboBox();
            this.nudWords = new System.Windows.Forms.NumericUpDown();
            this.nudHeadingSpace = new System.Windows.Forms.NumericUpDown();
            this.chkHeading = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tooltip = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.chkNoBold = new System.Windows.Forms.CheckBox();
            this.chkCiteTemplateDates = new System.Windows.Forms.CheckBox();
            this.chkReorderReferences = new System.Windows.Forms.CheckBox();
            this.chkPeopleCategories = new System.Windows.Forms.CheckBox();
            this.chkUnbalancedBrackets = new System.Windows.Forms.CheckBox();
            this.chkBadLinks = new System.Windows.Forms.CheckBox();
            this.chkHasHTML = new System.Windows.Forms.CheckBox();
            this.chkHeaderError = new System.Windows.Forms.CheckBox();
            this.chkUnbulletedLinks = new System.Windows.Forms.CheckBox();
            this.chkABCHeader = new System.Windows.Forms.CheckBox();
            this.txtStartFrom = new System.Windows.Forms.TextBox();
            this.chkSimpleLinks = new System.Windows.Forms.CheckBox();
            this.ArticlesListBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRevisionInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.btnSaveArticleList = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBoxArticle();
            this.btnFilter = new System.Windows.Forms.Button();
            this.lblStartFrom = new System.Windows.Forms.Label();
            this.lblLimitResutls = new System.Windows.Forms.Label();
            this.nudLimitResults = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSaveTxtList = new System.Windows.Forms.Button();
            this.lblListType = new System.Windows.Forms.Label();
            this.rdoBullet = new System.Windows.Forms.RadioButton();
            this.rdoHash = new System.Windows.Forms.RadioButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblPercentageComplete = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.threadPriorityButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.highestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboveNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belowNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerProgessUpdate = new System.Windows.Forms.Timer(this.components);
            this.gbAWBSpecific = new System.Windows.Forms.GroupBox();
            this.flwAWB = new System.Windows.Forms.FlowLayoutPanel();
            this.chkTypo = new System.Windows.Forms.CheckBox();
            this.chkDefaultSort = new System.Windows.Forms.CheckBox();
            this.tbParameters = new System.Windows.Forms.TabControl();
            this.tabDump = new System.Windows.Forms.TabPage();
            this.txtCase = new System.Windows.Forms.TextBox();
            this.lblCase = new System.Windows.Forms.Label();
            this.txtGenerator = new System.Windows.Forms.TextBox();
            this.lblGenerator = new System.Windows.Forms.Label();
            this.lnkBase = new System.Windows.Forms.LinkLabel();
            this.lblBase = new System.Windows.Forms.Label();
            this.txtSitename = new System.Windows.Forms.TextBox();
            this.lblSitename = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDumpLocation = new System.Windows.Forms.TextBox();
            this.lblDBDump = new System.Windows.Forms.Label();
            this.tabNamespace = new System.Windows.Forms.TabPage();
            this.pageNamespaces = new WikiFunctions.Controls.NamespacesControl();
            this.tabTitle = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkTitleCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkTitleContains = new System.Windows.Forms.CheckBox();
            this.chkTitleRegex = new System.Windows.Forms.CheckBox();
            this.txtTitleContains = new System.Windows.Forms.TextBox();
            this.chkTitleDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtTitleNotContains = new System.Windows.Forms.TextBox();
            this.tabRev = new System.Windows.Forms.TabPage();
            this.chkSearchDates = new System.Windows.Forms.CheckBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.tabText = new System.Windows.Forms.TabPage();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.chkIgnoreRedirects = new System.Windows.Forms.CheckBox();
            this.tabAWB = new System.Windows.Forms.TabPage();
            this.tabRestrict = new System.Windows.Forms.TabPage();
            this.chkProtection = new System.Windows.Forms.CheckBox();
            this.MoveDelete = new WikiFunctions.Controls.EditProtectControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flwHelpLinks = new System.Windows.Forms.FlowLayoutPanel();
            this.lblAlso = new System.Windows.Forms.Label();
            this.lnkWmfDumps = new System.Windows.Forms.LinkLabel();
            this.lnkWikiaDumps = new System.Windows.Forms.LinkLabel();
            this.lnkGenDump = new System.Windows.Forms.LinkLabel();
            this.lnkWikiPage = new System.Windows.Forms.LinkLabel();
            this.btnReset = new System.Windows.Forms.Button();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConvert = new System.Windows.Forms.TabPage();
            this.btnPause = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbText.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).BeginInit();
            this.ArticlesListBoxContextMenu.SuspendLayout();
            this.gbOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.gbAWBSpecific.SuspendLayout();
            this.flwAWB.SuspendLayout();
            this.tbParameters.SuspendLayout();
            this.tabDump.SuspendLayout();
            this.tabNamespace.SuspendLayout();
            this.tabTitle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabRev.SuspendLayout();
            this.tabText.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.tabAWB.SuspendLayout();
            this.tabRestrict.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flwHelpLinks.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabConvert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtList
            // 
            resources.ApplyResources(this.txtList, "txtList");
            this.txtList.Name = "txtList";
            // 
            // openXMLDialog
            // 
            this.openXMLDialog.FileName = "current or articles XML file";
            resources.ApplyResources(this.openXMLDialog, "openXMLDialog");
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // gbText
            // 
            this.gbText.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.gbText, "gbText");
            this.gbText.Name = "gbText";
            this.gbText.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.chkArticleDoesContain, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtArticleDoesContain, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleDoesNotContain, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtArticleDoesNotContain, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleRegex, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleCaseSensitive, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.chkSingle, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkMulti, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.chkIgnoreComments, 1, 4);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // chkArticleDoesContain
            // 
            resources.ApplyResources(this.chkArticleDoesContain, "chkArticleDoesContain");
            this.chkArticleDoesContain.Name = "chkArticleDoesContain";
            this.chkArticleDoesContain.CheckedChanged += new System.EventHandler(this.chkDoesContain_CheckedChanged);
            // 
            // txtArticleDoesContain
            // 
            resources.ApplyResources(this.txtArticleDoesContain, "txtArticleDoesContain");
            this.tableLayoutPanel3.SetColumnSpan(this.txtArticleDoesContain, 2);
            this.txtArticleDoesContain.Name = "txtArticleDoesContain";
            // 
            // chkArticleDoesNotContain
            // 
            resources.ApplyResources(this.chkArticleDoesNotContain, "chkArticleDoesNotContain");
            this.chkArticleDoesNotContain.Name = "chkArticleDoesNotContain";
            this.chkArticleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkDoesNotContain_CheckedChanged);
            // 
            // txtArticleDoesNotContain
            // 
            resources.ApplyResources(this.txtArticleDoesNotContain, "txtArticleDoesNotContain");
            this.tableLayoutPanel3.SetColumnSpan(this.txtArticleDoesNotContain, 2);
            this.txtArticleDoesNotContain.Name = "txtArticleDoesNotContain";
            // 
            // chkArticleRegex
            // 
            resources.ApplyResources(this.chkArticleRegex, "chkArticleRegex");
            this.chkArticleRegex.Name = "chkArticleRegex";
            this.chkArticleRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // chkArticleCaseSensitive
            // 
            resources.ApplyResources(this.chkArticleCaseSensitive, "chkArticleCaseSensitive");
            this.chkArticleCaseSensitive.Name = "chkArticleCaseSensitive";
            this.tooltip.SetToolTip(this.chkArticleCaseSensitive, resources.GetString("chkArticleCaseSensitive.ToolTip"));
            // 
            // chkSingle
            // 
            resources.ApplyResources(this.chkSingle, "chkSingle");
            this.chkSingle.Name = "chkSingle";
            this.tooltip.SetToolTip(this.chkSingle, resources.GetString("chkSingle.ToolTip"));
            // 
            // chkMulti
            // 
            resources.ApplyResources(this.chkMulti, "chkMulti");
            this.chkMulti.Name = "chkMulti";
            this.tooltip.SetToolTip(this.chkMulti, resources.GetString("chkMulti.ToolTip"));
            // 
            // chkIgnoreComments
            // 
            resources.ApplyResources(this.chkIgnoreComments, "chkIgnoreComments");
            this.tableLayoutPanel3.SetColumnSpan(this.chkIgnoreComments, 2);
            this.chkIgnoreComments.Name = "chkIgnoreComments";
            this.chkIgnoreComments.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            resources.ApplyResources(this.lblLength, "lblLength");
            this.lblLength.Name = "lblLength";
            // 
            // nudLength
            // 
            resources.ApplyResources(this.nudLength, "nudLength");
            this.nudLength.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudLength.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.nudLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLength.Name = "nudLength";
            this.nudLength.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // cmboLength
            // 
            this.cmboLength.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLength.FormattingEnabled = true;
            this.cmboLength.Items.AddRange(new object[] {
            resources.GetString("cmboLength.Items"),
            resources.GetString("cmboLength.Items1"),
            resources.GetString("cmboLength.Items2")});
            resources.ApplyResources(this.cmboLength, "cmboLength");
            this.cmboLength.Name = "cmboLength";
            this.cmboLength.SelectedIndexChanged += new System.EventHandler(this.cmboLength_SelectedIndexChanged);
            // 
            // lblLinks
            // 
            resources.ApplyResources(this.lblLinks, "lblLinks");
            this.lblLinks.Name = "lblLinks";
            // 
            // cmboLinks
            // 
            this.cmboLinks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLinks.FormattingEnabled = true;
            this.cmboLinks.Items.AddRange(new object[] {
            resources.GetString("cmboLinks.Items"),
            resources.GetString("cmboLinks.Items1"),
            resources.GetString("cmboLinks.Items2")});
            resources.ApplyResources(this.cmboLinks, "cmboLinks");
            this.cmboLinks.Name = "cmboLinks";
            this.cmboLinks.SelectedIndexChanged += new System.EventHandler(this.cmboLinks_SelectedIndexChanged);
            // 
            // nudLinks
            // 
            resources.ApplyResources(this.nudLinks, "nudLinks");
            this.nudLinks.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLinks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLinks.Name = "nudLinks";
            this.nudLinks.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblWords
            // 
            resources.ApplyResources(this.lblWords, "lblWords");
            this.lblWords.Name = "lblWords";
            // 
            // cmboWords
            // 
            this.cmboWords.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboWords.FormattingEnabled = true;
            this.cmboWords.Items.AddRange(new object[] {
            resources.GetString("cmboWords.Items"),
            resources.GetString("cmboWords.Items1"),
            resources.GetString("cmboWords.Items2")});
            resources.ApplyResources(this.cmboWords, "cmboWords");
            this.cmboWords.Name = "cmboWords";
            this.cmboWords.SelectedIndexChanged += new System.EventHandler(this.cmboWords_SelectedIndexChanged);
            // 
            // nudWords
            // 
            resources.ApplyResources(this.nudWords, "nudWords");
            this.nudWords.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWords.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWords.Name = "nudWords";
            this.nudWords.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // nudHeadingSpace
            // 
            resources.ApplyResources(this.nudHeadingSpace, "nudHeadingSpace");
            this.nudHeadingSpace.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeadingSpace.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeadingSpace.Name = "nudHeadingSpace";
            this.nudHeadingSpace.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // chkHeading
            // 
            resources.ApplyResources(this.chkHeading, "chkHeading");
            this.chkHeading.Name = "chkHeading";
            this.tooltip.SetToolTip(this.chkHeading, resources.GetString("chkHeading.ToolTip"));
            this.chkHeading.CheckedChanged += new System.EventHandler(this.chkHeading_CheckedChanged);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.tooltip.SetToolTip(this.btnClear, resources.GetString("btnClear.ToolTip"));
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCopy
            // 
            resources.ApplyResources(this.btnCopy, "btnCopy");
            this.btnCopy.Name = "btnCopy";
            this.tooltip.SetToolTip(this.btnCopy, resources.GetString("btnCopy.ToolTip"));
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnTransfer
            // 
            resources.ApplyResources(this.btnTransfer, "btnTransfer");
            this.btnTransfer.Name = "btnTransfer";
            this.tooltip.SetToolTip(this.btnTransfer, resources.GetString("btnTransfer.ToolTip"));
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnClearList
            // 
            resources.ApplyResources(this.btnClearList, "btnClearList");
            this.btnClearList.Name = "btnClearList";
            this.tooltip.SetToolTip(this.btnClearList, resources.GetString("btnClearList.ToolTip"));
            this.btnClearList.Click += new System.EventHandler(this.lbClear_Click);
            // 
            // chkNoBold
            // 
            resources.ApplyResources(this.chkNoBold, "chkNoBold");
            this.chkNoBold.Name = "chkNoBold";
            this.tooltip.SetToolTip(this.chkNoBold, resources.GetString("chkNoBold.ToolTip"));
            // 
            // chkCiteTemplateDates
            // 
            resources.ApplyResources(this.chkCiteTemplateDates, "chkCiteTemplateDates");
            this.chkCiteTemplateDates.Name = "chkCiteTemplateDates";
            this.tooltip.SetToolTip(this.chkCiteTemplateDates, resources.GetString("chkCiteTemplateDates.ToolTip"));
            // 
            // chkReorderReferences
            // 
            resources.ApplyResources(this.chkReorderReferences, "chkReorderReferences");
            this.chkReorderReferences.Name = "chkReorderReferences";
            this.tooltip.SetToolTip(this.chkReorderReferences, resources.GetString("chkReorderReferences.ToolTip"));
            // 
            // chkPeopleCategories
            // 
            resources.ApplyResources(this.chkPeopleCategories, "chkPeopleCategories");
            this.chkPeopleCategories.Name = "chkPeopleCategories";
            this.tooltip.SetToolTip(this.chkPeopleCategories, resources.GetString("chkPeopleCategories.ToolTip"));
            // 
            // chkUnbalancedBrackets
            // 
            resources.ApplyResources(this.chkUnbalancedBrackets, "chkUnbalancedBrackets");
            this.chkUnbalancedBrackets.Name = "chkUnbalancedBrackets";
            this.tooltip.SetToolTip(this.chkUnbalancedBrackets, resources.GetString("chkUnbalancedBrackets.ToolTip"));
            // 
            // chkBadLinks
            // 
            resources.ApplyResources(this.chkBadLinks, "chkBadLinks");
            this.chkBadLinks.Name = "chkBadLinks";
            this.tooltip.SetToolTip(this.chkBadLinks, resources.GetString("chkBadLinks.ToolTip"));
            // 
            // chkHasHTML
            // 
            resources.ApplyResources(this.chkHasHTML, "chkHasHTML");
            this.chkHasHTML.Name = "chkHasHTML";
            this.tooltip.SetToolTip(this.chkHasHTML, resources.GetString("chkHasHTML.ToolTip"));
            // 
            // chkHeaderError
            // 
            resources.ApplyResources(this.chkHeaderError, "chkHeaderError");
            this.chkHeaderError.Name = "chkHeaderError";
            this.tooltip.SetToolTip(this.chkHeaderError, resources.GetString("chkHeaderError.ToolTip"));
            // 
            // chkUnbulletedLinks
            // 
            resources.ApplyResources(this.chkUnbulletedLinks, "chkUnbulletedLinks");
            this.chkUnbulletedLinks.Name = "chkUnbulletedLinks";
            this.tooltip.SetToolTip(this.chkUnbulletedLinks, resources.GetString("chkUnbulletedLinks.ToolTip"));
            // 
            // chkABCHeader
            // 
            resources.ApplyResources(this.chkABCHeader, "chkABCHeader");
            this.chkABCHeader.Name = "chkABCHeader";
            this.tooltip.SetToolTip(this.chkABCHeader, resources.GetString("chkABCHeader.ToolTip"));
            this.chkABCHeader.CheckedChanged += new System.EventHandler(this.chkABCHeader_CheckedChanged);
            // 
            // txtStartFrom
            // 
            resources.ApplyResources(this.txtStartFrom, "txtStartFrom");
            this.txtStartFrom.Name = "txtStartFrom";
            this.tooltip.SetToolTip(this.txtStartFrom, resources.GetString("txtStartFrom.ToolTip"));
            // 
            // chkSimpleLinks
            // 
            resources.ApplyResources(this.chkSimpleLinks, "chkSimpleLinks");
            this.chkSimpleLinks.Name = "chkSimpleLinks";
            // 
            // ArticlesListBoxContextMenu
            // 
            this.ArticlesListBoxContextMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.ArticlesListBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem,
            this.openRevisionInBrowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.ArticlesListBoxContextMenu.Name = "contextMenuStrip1";
            resources.ApplyResources(this.ArticlesListBoxContextMenu, "ArticlesListBoxContextMenu");
            this.ArticlesListBoxContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openInBrowserToolStripMenuItem, "openInBrowserToolStripMenuItem");
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openRevisionInBrowserToolStripMenuItem
            // 
            resources.ApplyResources(this.openRevisionInBrowserToolStripMenuItem, "openRevisionInBrowserToolStripMenuItem");
            this.openRevisionInBrowserToolStripMenuItem.Name = "openRevisionInBrowserToolStripMenuItem";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.btnSaveArticleList);
            this.gbOutput.Controls.Add(this.lbArticles);
            this.gbOutput.Controls.Add(this.btnFilter);
            this.gbOutput.Controls.Add(this.btnClearList);
            resources.ApplyResources(this.gbOutput, "gbOutput");
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.TabStop = false;
            // 
            // btnSaveArticleList
            // 
            resources.ApplyResources(this.btnSaveArticleList, "btnSaveArticleList");
            this.btnSaveArticleList.Name = "btnSaveArticleList";
            this.btnSaveArticleList.Click += new System.EventHandler(this.btnSaveArticleList_Click);
            // 
            // lbArticles
            // 
            resources.ApplyResources(this.lbArticles, "lbArticles");
            this.lbArticles.ContextMenuStrip = this.ArticlesListBoxContextMenu;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            // 
            // btnFilter
            // 
            resources.ApplyResources(this.btnFilter, "btnFilter");
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblStartFrom
            // 
            resources.ApplyResources(this.lblStartFrom, "lblStartFrom");
            this.lblStartFrom.Name = "lblStartFrom";
            // 
            // lblLimitResutls
            // 
            resources.ApplyResources(this.lblLimitResutls, "lblLimitResutls");
            this.lblLimitResutls.Name = "lblLimitResutls";
            // 
            // nudLimitResults
            // 
            resources.ApplyResources(this.nudLimitResults, "nudLimitResults");
            this.nudLimitResults.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLimitResults.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.nudLimitResults.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLimitResults.Name = "nudLimitResults";
            this.nudLimitResults.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.nudLimitResults.ValueChanged += new System.EventHandler(this.nudLimitResults_ValueChanged);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSaveTxtList
            // 
            resources.ApplyResources(this.btnSaveTxtList, "btnSaveTxtList");
            this.btnSaveTxtList.Name = "btnSaveTxtList";
            this.btnSaveTxtList.Click += new System.EventHandler(this.btnSaveTxtList_Click);
            // 
            // lblListType
            // 
            resources.ApplyResources(this.lblListType, "lblListType");
            this.lblListType.Name = "lblListType";
            // 
            // rdoBullet
            // 
            resources.ApplyResources(this.rdoBullet, "rdoBullet");
            this.rdoBullet.Name = "rdoBullet";
            // 
            // rdoHash
            // 
            resources.ApplyResources(this.rdoHash, "rdoHash");
            this.rdoHash.Checked = true;
            this.rdoHash.Name = "rdoHash";
            this.rdoHash.TabStop = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblPercentageComplete,
            this.lblCount,
            this.toolStripStatusLabel1,
            this.threadPriorityButton});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.SizingGrip = false;
            // 
            // progressBar
            // 
            this.progressBar.Maximum = 200;
            this.progressBar.Name = "progressBar";
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblPercentageComplete
            // 
            this.lblPercentageComplete.Name = "lblPercentageComplete";
            resources.ApplyResources(this.lblPercentageComplete, "lblPercentageComplete");
            // 
            // lblCount
            // 
            resources.ApplyResources(this.lblCount, "lblCount");
            this.lblCount.Name = "lblCount";
            this.lblCount.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // threadPriorityButton
            // 
            this.threadPriorityButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highestToolStripMenuItem,
            this.aboveNormalToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.belowNormalToolStripMenuItem,
            this.lowestToolStripMenuItem});
            this.threadPriorityButton.Name = "threadPriorityButton";
            resources.ApplyResources(this.threadPriorityButton, "threadPriorityButton");
            // 
            // highestToolStripMenuItem
            // 
            this.highestToolStripMenuItem.CheckOnClick = true;
            this.highestToolStripMenuItem.Name = "highestToolStripMenuItem";
            resources.ApplyResources(this.highestToolStripMenuItem, "highestToolStripMenuItem");
            this.highestToolStripMenuItem.Click += new System.EventHandler(this.highestToolStripMenuItem_Click);
            // 
            // aboveNormalToolStripMenuItem
            // 
            this.aboveNormalToolStripMenuItem.CheckOnClick = true;
            this.aboveNormalToolStripMenuItem.Name = "aboveNormalToolStripMenuItem";
            resources.ApplyResources(this.aboveNormalToolStripMenuItem, "aboveNormalToolStripMenuItem");
            this.aboveNormalToolStripMenuItem.Click += new System.EventHandler(this.aboveNormalToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.CheckOnClick = true;
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            resources.ApplyResources(this.normalToolStripMenuItem, "normalToolStripMenuItem");
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // belowNormalToolStripMenuItem
            // 
            this.belowNormalToolStripMenuItem.Checked = true;
            this.belowNormalToolStripMenuItem.CheckOnClick = true;
            this.belowNormalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.belowNormalToolStripMenuItem.Name = "belowNormalToolStripMenuItem";
            resources.ApplyResources(this.belowNormalToolStripMenuItem, "belowNormalToolStripMenuItem");
            this.belowNormalToolStripMenuItem.Click += new System.EventHandler(this.belowNormalToolStripMenuItem_Click);
            // 
            // lowestToolStripMenuItem
            // 
            this.lowestToolStripMenuItem.CheckOnClick = true;
            this.lowestToolStripMenuItem.Name = "lowestToolStripMenuItem";
            resources.ApplyResources(this.lowestToolStripMenuItem, "lowestToolStripMenuItem");
            this.lowestToolStripMenuItem.Click += new System.EventHandler(this.lowestToolStripMenuItem_Click);
            // 
            // timerProgessUpdate
            // 
            this.timerProgessUpdate.Interval = 1000;
            this.timerProgessUpdate.Tick += new System.EventHandler(this.timerProgessUpdate_Tick);
            // 
            // gbAWBSpecific
            // 
            this.gbAWBSpecific.Controls.Add(this.flwAWB);
            resources.ApplyResources(this.gbAWBSpecific, "gbAWBSpecific");
            this.gbAWBSpecific.Name = "gbAWBSpecific";
            this.gbAWBSpecific.TabStop = false;
            // 
            // flwAWB
            // 
            this.flwAWB.Controls.Add(this.chkNoBold);
            this.flwAWB.Controls.Add(this.chkSimpleLinks);
            this.flwAWB.Controls.Add(this.chkBadLinks);
            this.flwAWB.Controls.Add(this.chkHasHTML);
            this.flwAWB.Controls.Add(this.chkUnbulletedLinks);
            this.flwAWB.Controls.Add(this.chkPeopleCategories);
            this.flwAWB.Controls.Add(this.chkTypo);
            this.flwAWB.Controls.Add(this.chkDefaultSort);
            this.flwAWB.Controls.Add(this.chkHeaderError);
            this.flwAWB.Controls.Add(this.chkCiteTemplateDates);
            this.flwAWB.Controls.Add(this.chkUnbalancedBrackets);
            this.flwAWB.Controls.Add(this.chkReorderReferences);
            resources.ApplyResources(this.flwAWB, "flwAWB");
            this.flwAWB.Name = "flwAWB";
            // 
            // chkTypo
            // 
            resources.ApplyResources(this.chkTypo, "chkTypo");
            this.chkTypo.Name = "chkTypo";
            // 
            // chkDefaultSort
            // 
            resources.ApplyResources(this.chkDefaultSort, "chkDefaultSort");
            this.chkDefaultSort.Name = "chkDefaultSort";
            this.chkDefaultSort.UseVisualStyleBackColor = true;
            // 
            // tbParameters
            // 
            resources.ApplyResources(this.tbParameters, "tbParameters");
            this.tbParameters.Controls.Add(this.tabDump);
            this.tbParameters.Controls.Add(this.tabNamespace);
            this.tbParameters.Controls.Add(this.tabTitle);
            this.tbParameters.Controls.Add(this.tabRev);
            this.tbParameters.Controls.Add(this.tabText);
            this.tbParameters.Controls.Add(this.tabAWB);
            this.tbParameters.Controls.Add(this.tabRestrict);
            this.tbParameters.Controls.Add(this.tabPage1);
            this.tbParameters.Name = "tbParameters";
            this.tbParameters.SelectedIndex = 0;
            // 
            // tabDump
            // 
            this.tabDump.Controls.Add(this.txtCase);
            this.tabDump.Controls.Add(this.lblCase);
            this.tabDump.Controls.Add(this.txtGenerator);
            this.tabDump.Controls.Add(this.lblGenerator);
            this.tabDump.Controls.Add(this.lnkBase);
            this.tabDump.Controls.Add(this.lblBase);
            this.tabDump.Controls.Add(this.txtSitename);
            this.tabDump.Controls.Add(this.lblSitename);
            this.tabDump.Controls.Add(this.btnBrowse);
            this.tabDump.Controls.Add(this.txtDumpLocation);
            this.tabDump.Controls.Add(this.lblDBDump);
            resources.ApplyResources(this.tabDump, "tabDump");
            this.tabDump.Name = "tabDump";
            this.tabDump.UseVisualStyleBackColor = true;
            // 
            // txtCase
            // 
            resources.ApplyResources(this.txtCase, "txtCase");
            this.txtCase.Name = "txtCase";
            this.txtCase.ReadOnly = true;
            // 
            // lblCase
            // 
            resources.ApplyResources(this.lblCase, "lblCase");
            this.lblCase.Name = "lblCase";
            // 
            // txtGenerator
            // 
            resources.ApplyResources(this.txtGenerator, "txtGenerator");
            this.txtGenerator.Name = "txtGenerator";
            this.txtGenerator.ReadOnly = true;
            // 
            // lblGenerator
            // 
            resources.ApplyResources(this.lblGenerator, "lblGenerator");
            this.lblGenerator.Name = "lblGenerator";
            // 
            // lnkBase
            // 
            resources.ApplyResources(this.lnkBase, "lnkBase");
            this.lnkBase.Name = "lnkBase";
            this.lnkBase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBase_LinkClicked);
            // 
            // lblBase
            // 
            resources.ApplyResources(this.lblBase, "lblBase");
            this.lblBase.Name = "lblBase";
            // 
            // txtSitename
            // 
            resources.ApplyResources(this.txtSitename, "txtSitename");
            this.txtSitename.Name = "txtSitename";
            this.txtSitename.ReadOnly = true;
            // 
            // lblSitename
            // 
            resources.ApplyResources(this.lblSitename, "lblSitename");
            this.lblSitename.Name = "lblSitename";
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtDumpLocation
            // 
            resources.ApplyResources(this.txtDumpLocation, "txtDumpLocation");
            this.txtDumpLocation.Name = "txtDumpLocation";
            // 
            // lblDBDump
            // 
            resources.ApplyResources(this.lblDBDump, "lblDBDump");
            this.lblDBDump.Name = "lblDBDump";
            // 
            // tabNamespace
            // 
            this.tabNamespace.Controls.Add(this.pageNamespaces);
            resources.ApplyResources(this.tabNamespace, "tabNamespace");
            this.tabNamespace.Name = "tabNamespace";
            this.tabNamespace.UseVisualStyleBackColor = true;
            // 
            // pageNamespaces
            // 
            resources.ApplyResources(this.pageNamespaces, "pageNamespaces");
            this.pageNamespaces.Name = "pageNamespaces";
            // 
            // tabTitle
            // 
            this.tabTitle.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.tabTitle, "tabTitle");
            this.tabTitle.Name = "tabTitle";
            this.tabTitle.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.chkTitleCaseSensitive, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleContains, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleRegex, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleContains, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleDoesNotContain, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleNotContains, 1, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // chkTitleCaseSensitive
            // 
            resources.ApplyResources(this.chkTitleCaseSensitive, "chkTitleCaseSensitive");
            this.chkTitleCaseSensitive.Checked = true;
            this.chkTitleCaseSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTitleCaseSensitive.Name = "chkTitleCaseSensitive";
            // 
            // chkTitleContains
            // 
            resources.ApplyResources(this.chkTitleContains, "chkTitleContains");
            this.chkTitleContains.Name = "chkTitleContains";
            this.chkTitleContains.CheckedChanged += new System.EventHandler(this.chkTitleContains_CheckedChanged);
            // 
            // chkTitleRegex
            // 
            resources.ApplyResources(this.chkTitleRegex, "chkTitleRegex");
            this.chkTitleRegex.Name = "chkTitleRegex";
            // 
            // txtTitleContains
            // 
            resources.ApplyResources(this.txtTitleContains, "txtTitleContains");
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleContains, 2);
            this.txtTitleContains.Name = "txtTitleContains";
            this.txtTitleContains.Leave += new System.EventHandler(this.txtTitleContains_Leave);
            // 
            // chkTitleDoesNotContain
            // 
            resources.ApplyResources(this.chkTitleDoesNotContain, "chkTitleDoesNotContain");
            this.chkTitleDoesNotContain.Name = "chkTitleDoesNotContain";
            this.chkTitleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkTitleDoesNotContain_CheckedChanged);
            // 
            // txtTitleNotContains
            // 
            resources.ApplyResources(this.txtTitleNotContains, "txtTitleNotContains");
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleNotContains, 2);
            this.txtTitleNotContains.Name = "txtTitleNotContains";
            this.txtTitleNotContains.Leave += new System.EventHandler(this.txtTitleNotContains_Leave);
            // 
            // tabRev
            // 
            this.tabRev.Controls.Add(this.chkSearchDates);
            this.tabRev.Controls.Add(this.lblDate);
            this.tabRev.Controls.Add(this.dtpFrom);
            this.tabRev.Controls.Add(this.lblEndDate);
            this.tabRev.Controls.Add(this.dtpTo);
            resources.ApplyResources(this.tabRev, "tabRev");
            this.tabRev.Name = "tabRev";
            this.tabRev.UseVisualStyleBackColor = true;
            // 
            // chkSearchDates
            // 
            resources.ApplyResources(this.chkSearchDates, "chkSearchDates");
            this.chkSearchDates.Name = "chkSearchDates";
            this.chkSearchDates.CheckedChanged += new System.EventHandler(this.chkSearchDates_CheckedChanged);
            // 
            // lblDate
            // 
            resources.ApplyResources(this.lblDate, "lblDate");
            this.lblDate.Name = "lblDate";
            // 
            // dtpFrom
            // 
            resources.ApplyResources(this.dtpFrom, "dtpFrom");
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            // 
            // lblEndDate
            // 
            resources.ApplyResources(this.lblEndDate, "lblEndDate");
            this.lblEndDate.Name = "lblEndDate";
            // 
            // dtpTo
            // 
            this.dtpTo.Checked = false;
            resources.ApplyResources(this.dtpTo, "dtpTo");
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTo.Name = "dtpTo";
            // 
            // tabText
            // 
            this.tabText.Controls.Add(this.gbProperties);
            this.tabText.Controls.Add(this.gbText);
            resources.ApplyResources(this.tabText, "tabText");
            this.tabText.Name = "tabText";
            this.tabText.UseVisualStyleBackColor = true;
            // 
            // gbProperties
            // 
            this.gbProperties.Controls.Add(this.chkIgnoreRedirects);
            this.gbProperties.Controls.Add(this.lblWords);
            this.gbProperties.Controls.Add(this.cmboWords);
            this.gbProperties.Controls.Add(this.nudWords);
            this.gbProperties.Controls.Add(this.lblLinks);
            this.gbProperties.Controls.Add(this.cmboLinks);
            this.gbProperties.Controls.Add(this.nudLinks);
            this.gbProperties.Controls.Add(this.lblLength);
            this.gbProperties.Controls.Add(this.cmboLength);
            this.gbProperties.Controls.Add(this.nudLength);
            resources.ApplyResources(this.gbProperties, "gbProperties");
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.TabStop = false;
            // 
            // chkIgnoreRedirects
            // 
            resources.ApplyResources(this.chkIgnoreRedirects, "chkIgnoreRedirects");
            this.chkIgnoreRedirects.Checked = true;
            this.chkIgnoreRedirects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreRedirects.Name = "chkIgnoreRedirects";
            this.chkIgnoreRedirects.UseVisualStyleBackColor = true;
            // 
            // tabAWB
            // 
            this.tabAWB.Controls.Add(this.gbAWBSpecific);
            this.tabAWB.Controls.Add(this.nudLimitResults);
            this.tabAWB.Controls.Add(this.lblStartFrom);
            this.tabAWB.Controls.Add(this.txtStartFrom);
            this.tabAWB.Controls.Add(this.lblLimitResutls);
            resources.ApplyResources(this.tabAWB, "tabAWB");
            this.tabAWB.Name = "tabAWB";
            this.tabAWB.UseVisualStyleBackColor = true;
            // 
            // tabRestrict
            // 
            this.tabRestrict.Controls.Add(this.chkProtection);
            this.tabRestrict.Controls.Add(this.MoveDelete);
            resources.ApplyResources(this.tabRestrict, "tabRestrict");
            this.tabRestrict.Name = "tabRestrict";
            this.tabRestrict.UseVisualStyleBackColor = true;
            // 
            // chkProtection
            // 
            resources.ApplyResources(this.chkProtection, "chkProtection");
            this.chkProtection.Name = "chkProtection";
            this.chkProtection.UseVisualStyleBackColor = true;
            this.chkProtection.CheckedChanged += new System.EventHandler(this.chkProtection_CheckedChanged);
            // 
            // MoveDelete
            // 
            this.MoveDelete.EditProtectionLevel = "";
            resources.ApplyResources(this.MoveDelete, "MoveDelete");
            this.MoveDelete.MoveProtectionLevel = "";
            this.MoveDelete.Name = "MoveDelete";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flwHelpLinks);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flwHelpLinks
            // 
            this.flwHelpLinks.Controls.Add(this.lblAlso);
            this.flwHelpLinks.Controls.Add(this.lnkWmfDumps);
            this.flwHelpLinks.Controls.Add(this.lnkWikiaDumps);
            this.flwHelpLinks.Controls.Add(this.lnkGenDump);
            this.flwHelpLinks.Controls.Add(this.lnkWikiPage);
            resources.ApplyResources(this.flwHelpLinks, "flwHelpLinks");
            this.flwHelpLinks.Name = "flwHelpLinks";
            // 
            // lblAlso
            // 
            resources.ApplyResources(this.lblAlso, "lblAlso");
            this.lblAlso.Name = "lblAlso";
            // 
            // lnkWmfDumps
            // 
            resources.ApplyResources(this.lnkWmfDumps, "lnkWmfDumps");
            this.lnkWmfDumps.Name = "lnkWmfDumps";
            this.lnkWmfDumps.TabStop = true;
            this.lnkWmfDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWmfDumps_LinkClicked);
            // 
            // lnkWikiaDumps
            // 
            resources.ApplyResources(this.lnkWikiaDumps, "lnkWikiaDumps");
            this.lnkWikiaDumps.Name = "lnkWikiaDumps";
            this.lnkWikiaDumps.TabStop = true;
            this.lnkWikiaDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWikiaDumps_LinkClicked);
            // 
            // lnkGenDump
            // 
            resources.ApplyResources(this.lnkGenDump, "lnkGenDump");
            this.lnkGenDump.Name = "lnkGenDump";
            this.lnkGenDump.TabStop = true;
            this.lnkGenDump.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGenDump_LinkClicked);
            // 
            // lnkWikiPage
            // 
            resources.ApplyResources(this.lnkWikiPage, "lnkWikiPage");
            this.lnkWikiPage.Name = "lnkWikiPage";
            this.lnkWikiPage.TabStop = true;
            this.lnkWikiPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWikiPage_LinkClicked);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Checked = true;
            this.toolStripMenuItem5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConvert);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabConvert
            // 
            this.tabConvert.Controls.Add(this.btnSaveTxtList);
            this.tabConvert.Controls.Add(this.lblListType);
            this.tabConvert.Controls.Add(this.chkABCHeader);
            this.tabConvert.Controls.Add(this.rdoBullet);
            this.tabConvert.Controls.Add(this.rdoHash);
            this.tabConvert.Controls.Add(this.txtList);
            this.tabConvert.Controls.Add(this.chkHeading);
            this.tabConvert.Controls.Add(this.nudHeadingSpace);
            this.tabConvert.Controls.Add(this.btnClear);
            this.tabConvert.Controls.Add(this.btnTransfer);
            this.tabConvert.Controls.Add(this.btnCopy);
            resources.ApplyResources(this.tabConvert, "tabConvert");
            this.tabConvert.Name = "tabConvert";
            this.tabConvert.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            resources.ApplyResources(this.btnPause, "btnPause");
            this.btnPause.Name = "btnPause";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbOutput);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            // 
            // DatabaseScanner
            // 
            this.AcceptButton = this.btnStart;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tbParameters);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnReset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "DatabaseScanner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DatabaseScanner_FormClosing);
            this.Load += new System.EventHandler(this.DatabaseScanner_Load);
            this.gbText.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).EndInit();
            this.ArticlesListBoxContextMenu.ResumeLayout(false);
            this.gbOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gbAWBSpecific.ResumeLayout(false);
            this.flwAWB.ResumeLayout(false);
            this.flwAWB.PerformLayout();
            this.tbParameters.ResumeLayout(false);
            this.tabDump.ResumeLayout(false);
            this.tabDump.PerformLayout();
            this.tabNamespace.ResumeLayout(false);
            this.tabTitle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tabRev.ResumeLayout(false);
            this.tabRev.PerformLayout();
            this.tabText.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.tabAWB.ResumeLayout(false);
            this.tabAWB.PerformLayout();
            this.tabRestrict.ResumeLayout(false);
            this.tabRestrict.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.flwHelpLinks.ResumeLayout(false);
            this.flwHelpLinks.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabConvert.ResumeLayout(false);
            this.tabConvert.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.OpenFileDialog openXMLDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.GroupBox gbText;
        private Button btnClear;
        private Button btnCopy;
        private CheckBox chkHeading;
        private NumericUpDown nudHeadingSpace;
        private Button btnTransfer;
        private Button btnClearList;
        private ContextMenuStrip ArticlesListBoxContextMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private GroupBox gbOutput;
        private RadioButton rdoBullet;
        private RadioButton rdoHash;
        private ToolStripProgressBar progressBar;
        private Timer timerProgessUpdate;
        private ToolStripStatusLabel lblCount;
        private ToolStripMenuItem removeToolStripMenuItem;
        private GroupBox gbAWBSpecific;
        private CheckBox chkArticleCaseSensitive;
        private CheckBox chkArticleRegex;
        private NumericUpDown nudLength;
        private Label lblLinks;
        private ComboBox cmboLength;
        private TextBox txtArticleDoesContain;
        private Label lblLength;
        private TextBox txtArticleDoesNotContain;
        private NumericUpDown nudLinks;
        private CheckBox chkArticleDoesContain;
        private CheckBox chkArticleDoesNotContain;
        private ComboBox cmboLinks;
        private CheckBox chkSingle;
        private CheckBox chkMulti;
        private ToolStripMenuItem openInBrowserToolStripMenuItem;
        private ComboBox cmboWords;
        private NumericUpDown nudWords;
        private Label lblWords;
        private Button btnFilter;
        private WikiFunctions.Controls.Lists.ListBoxArticle lbArticles;
        private CheckBox chkUnbulletedLinks;
        private CheckBox chkHeaderError;
        private CheckBox chkHasHTML;
        private CheckBox chkBadLinks;
        private CheckBox chkSimpleLinks;
        private CheckBox chkNoBold;
        private CheckBox chkCiteTemplateDates;
        private CheckBox chkReorderReferences;
        private CheckBox chkPeopleCategories;
        private CheckBox chkUnbalancedBrackets;
        private CheckBox chkTypo;
        private CheckBox chkABCHeader;
        private Label lblStartFrom;
        private TextBox txtStartFrom;
        private Label lblListType;
        private Button btnStart;
        private NumericUpDown nudLimitResults;
        private Label lblLimitResutls;
        private TabControl tbParameters;
        private TabPage tabDump;
        private TabPage tabNamespace;
        private TabPage tabText;
        private TabPage tabAWB;
        private Button btnBrowse;
        private TextBox txtDumpLocation;
        private Label lblCase;
        private Label lblGenerator;
        private Label lblBase;
        private Label lblSitename;
        private CheckBox chkIgnoreComments;
        private CheckBox chkIgnoreRedirects;
        private LinkLabel lnkBase;
        private ToolStripMenuItem openRevisionInBrowserToolStripMenuItem;
        private ToolStripMenuItem highestToolStripMenuItem;
        private ToolStripMenuItem aboveNormalToolStripMenuItem;
        private ToolStripMenuItem normalToolStripMenuItem;
        private ToolStripMenuItem belowNormalToolStripMenuItem;
        private ToolStripMenuItem lowestToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private Button btnReset;
        private Label lblDBDump;
        private TextBox txtSitename;
        private ToolStripSeparator toolStripSeparator1;
        private GroupBox gbProperties;
        private TextBox txtGenerator;
        private LinkLabel lnkWikiaDumps;
        private LinkLabel lnkWmfDumps;
        private LinkLabel lnkGenDump;
        private LinkLabel lnkWikiPage;
        private StatusStrip statusStrip;
        private CheckBox chkDefaultSort;
        private TabPage tabRev;
        private TabPage tabRestrict;
        private Label lblAlso;
        private TextBox txtCase;
        private TabControl tabControl1;
        private TabPage tabConvert;
        private FlowLayoutPanel flwAWB;
        private TableLayoutPanel tableLayoutPanel3;
        private ToolStripDropDownButton threadPriorityButton;
        private Button btnPause;
        private FlowLayoutPanel flwHelpLinks;
        private SplitContainer splitContainer1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Button btnSaveTxtList;
        private Button btnSaveArticleList;
        private WikiFunctions.Controls.EditProtectControl MoveDelete;
        private CheckBox chkProtection;
        private TabPage tabTitle;
        private TableLayoutPanel tableLayoutPanel2;
        private CheckBox chkTitleCaseSensitive;
        private CheckBox chkTitleContains;
        private CheckBox chkTitleRegex;
        private TextBox txtTitleContains;
        private CheckBox chkTitleDoesNotContain;
        private TextBox txtTitleNotContains;
        private CheckBox chkSearchDates;
        private Label lblDate;
        private DateTimePicker dtpFrom;
        private Label lblEndDate;
        private DateTimePicker dtpTo;
        private ToolStripStatusLabel lblPercentageComplete;
        private WikiFunctions.Controls.NamespacesControl pageNamespaces;
        private TabPage tabPage1;
        private Controls.AWBToolTip tooltip;
    }
}