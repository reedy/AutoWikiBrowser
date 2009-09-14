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
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.chkNoBold = new System.Windows.Forms.CheckBox();
            this.chkCiteTemplateDates = new System.Windows.Forms.CheckBox();
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
            this.openRevisionInBowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbOutput = new System.Windows.Forms.GroupBox();
            this.btnSaveArticleList = new System.Windows.Forms.Button();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBox2();
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
            this.flwHelpLinks = new System.Windows.Forms.FlowLayoutPanel();
            this.lblAlso = new System.Windows.Forms.Label();
            this.lnkWmfDumps = new System.Windows.Forms.LinkLabel();
            this.lnkWikiaDumps = new System.Windows.Forms.LinkLabel();
            this.lnkGenDump = new System.Windows.Forms.LinkLabel();
            this.btnAbout = new System.Windows.Forms.Button();
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
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
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
            this.flwHelpLinks.SuspendLayout();
            this.tabNamespace.SuspendLayout();
            this.tabTitle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabRev.SuspendLayout();
            this.tabText.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.tabAWB.SuspendLayout();
            this.tabRestrict.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabConvert.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtList
            // 
            this.txtList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtList.Location = new System.Drawing.Point(3, 51);
            this.txtList.MaxLength = 0;
            this.txtList.Multiline = true;
            this.txtList.Name = "txtList";
            this.txtList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtList.Size = new System.Drawing.Size(276, 189);
            this.txtList.TabIndex = 7;
            this.txtList.WordWrap = false;
            // 
            // openXMLDialog
            // 
            this.openXMLDialog.FileName = "current or articles XML file";
            this.openXMLDialog.Filter = "XML file|*.xml";
            this.openXMLDialog.Title = "Open \"current\" or \"Pages\" XML file";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "Text file|*.txt";
            // 
            // gbText
            // 
            this.gbText.Controls.Add(this.tableLayoutPanel3);
            this.gbText.Location = new System.Drawing.Point(6, 6);
            this.gbText.Name = "gbText";
            this.gbText.Size = new System.Drawing.Size(277, 141);
            this.gbText.TabIndex = 1;
            this.gbText.TabStop = false;
            this.gbText.Text = "Te&xt Searching";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.chkArticleDoesContain, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtArticleDoesContain, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleDoesNotContain, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.txtArticleDoesNotContain, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleRegex, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkArticleCaseSensitive, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.chkSingle, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkMulti, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.chkIgnoreComments, 1, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(271, 125);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // chkArticleDoesContain
            // 
            this.chkArticleDoesContain.AutoSize = true;
            this.chkArticleDoesContain.Location = new System.Drawing.Point(3, 3);
            this.chkArticleDoesContain.Name = "chkArticleDoesContain";
            this.chkArticleDoesContain.Size = new System.Drawing.Size(70, 17);
            this.chkArticleDoesContain.TabIndex = 0;
            this.chkArticleDoesContain.Text = "&Contains:";
            this.chkArticleDoesContain.CheckedChanged += new System.EventHandler(this.chkDoesContain_CheckedChanged);
            // 
            // txtArticleDoesContain
            // 
            this.txtArticleDoesContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.txtArticleDoesContain, 2);
            this.txtArticleDoesContain.Enabled = false;
            this.txtArticleDoesContain.Location = new System.Drawing.Point(98, 3);
            this.txtArticleDoesContain.MaxLength = 0;
            this.txtArticleDoesContain.Name = "txtArticleDoesContain";
            this.txtArticleDoesContain.Size = new System.Drawing.Size(171, 20);
            this.txtArticleDoesContain.TabIndex = 1;
            // 
            // chkArticleDoesNotContain
            // 
            this.chkArticleDoesNotContain.AutoSize = true;
            this.chkArticleDoesNotContain.Location = new System.Drawing.Point(3, 29);
            this.chkArticleDoesNotContain.Name = "chkArticleDoesNotContain";
            this.chkArticleDoesNotContain.Size = new System.Drawing.Size(89, 17);
            this.chkArticleDoesNotContain.TabIndex = 2;
            this.chkArticleDoesNotContain.Text = "&Not contains:";
            this.chkArticleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkDoesNotContain_CheckedChanged);
            // 
            // txtArticleDoesNotContain
            // 
            this.txtArticleDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.SetColumnSpan(this.txtArticleDoesNotContain, 2);
            this.txtArticleDoesNotContain.Enabled = false;
            this.txtArticleDoesNotContain.Location = new System.Drawing.Point(98, 29);
            this.txtArticleDoesNotContain.MaxLength = 0;
            this.txtArticleDoesNotContain.Name = "txtArticleDoesNotContain";
            this.txtArticleDoesNotContain.Size = new System.Drawing.Size(171, 20);
            this.txtArticleDoesNotContain.TabIndex = 3;
            // 
            // chkArticleRegex
            // 
            this.chkArticleRegex.AutoSize = true;
            this.chkArticleRegex.Location = new System.Drawing.Point(98, 55);
            this.chkArticleRegex.Name = "chkArticleRegex";
            this.chkArticleRegex.Size = new System.Drawing.Size(57, 17);
            this.chkArticleRegex.TabIndex = 4;
            this.chkArticleRegex.Text = "&Regex";
            this.chkArticleRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // chkArticleCaseSensitive
            // 
            this.chkArticleCaseSensitive.AutoSize = true;
            this.chkArticleCaseSensitive.Location = new System.Drawing.Point(98, 78);
            this.chkArticleCaseSensitive.Name = "chkArticleCaseSensitive";
            this.chkArticleCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkArticleCaseSensitive.TabIndex = 5;
            this.chkArticleCaseSensitive.Text = "Case &sensitive";
            this.tooltip.SetToolTip(this.chkArticleCaseSensitive, "Changes case sensitivity");
            // 
            // chkSingle
            // 
            this.chkSingle.AutoSize = true;
            this.chkSingle.Enabled = false;
            this.chkSingle.Location = new System.Drawing.Point(198, 55);
            this.chkSingle.Name = "chkSingle";
            this.chkSingle.Size = new System.Drawing.Size(71, 17);
            this.chkSingle.TabIndex = 6;
            this.chkSingle.Text = "S&ingleline";
            this.tooltip.SetToolTip(this.chkSingle, "Changes meaing of \".\"  so it matches all characters, as opposed to all apart from" +
                    " newlines");
            // 
            // chkMulti
            // 
            this.chkMulti.AutoSize = true;
            this.chkMulti.Enabled = false;
            this.chkMulti.Location = new System.Drawing.Point(198, 78);
            this.chkMulti.Name = "chkMulti";
            this.chkMulti.Size = new System.Drawing.Size(64, 17);
            this.chkMulti.TabIndex = 7;
            this.chkMulti.Text = "&Multiline";
            this.tooltip.SetToolTip(this.chkMulti, "Changes meaning of \"^\" and \"$\" so they represent the beginning and end respective" +
                    "ly of every line, rather just of the entire string");
            // 
            // chkIgnoreComments
            // 
            this.chkIgnoreComments.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.chkIgnoreComments, 2);
            this.chkIgnoreComments.Location = new System.Drawing.Point(98, 101);
            this.chkIgnoreComments.Name = "chkIgnoreComments";
            this.chkIgnoreComments.Size = new System.Drawing.Size(140, 17);
            this.chkIgnoreComments.TabIndex = 18;
            this.chkIgnoreComments.Text = "Ignore <!-- comments -->";
            this.chkIgnoreComments.UseVisualStyleBackColor = true;
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(6, 20);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(61, 13);
            this.lblLength.TabIndex = 8;
            this.lblLength.Text = "C&haracters:";
            this.lblLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // nudLength
            // 
            this.nudLength.Enabled = false;
            this.nudLength.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nudLength.Location = new System.Drawing.Point(155, 17);
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
            this.nudLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nudLength.Size = new System.Drawing.Size(60, 20);
            this.nudLength.TabIndex = 10;
            this.nudLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
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
            "not counted",
            "at least",
            "at most"});
            this.cmboLength.Location = new System.Drawing.Point(69, 17);
            this.cmboLength.Name = "cmboLength";
            this.cmboLength.Size = new System.Drawing.Size(80, 21);
            this.cmboLength.TabIndex = 9;
            this.cmboLength.SelectedIndexChanged += new System.EventHandler(this.cmboLength_SelectedIndexChanged);
            // 
            // lblLinks
            // 
            this.lblLinks.AutoSize = true;
            this.lblLinks.Location = new System.Drawing.Point(6, 46);
            this.lblLinks.Name = "lblLinks";
            this.lblLinks.Size = new System.Drawing.Size(35, 13);
            this.lblLinks.TabIndex = 11;
            this.lblLinks.Text = "&Links:";
            this.lblLinks.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmboLinks
            // 
            this.cmboLinks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLinks.FormattingEnabled = true;
            this.cmboLinks.Items.AddRange(new object[] {
            "not counted",
            "at least",
            "at most"});
            this.cmboLinks.Location = new System.Drawing.Point(69, 43);
            this.cmboLinks.Name = "cmboLinks";
            this.cmboLinks.Size = new System.Drawing.Size(80, 21);
            this.cmboLinks.TabIndex = 12;
            this.cmboLinks.SelectedIndexChanged += new System.EventHandler(this.cmboLinks_SelectedIndexChanged);
            // 
            // nudLinks
            // 
            this.nudLinks.Enabled = false;
            this.nudLinks.Location = new System.Drawing.Point(155, 44);
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
            this.nudLinks.Size = new System.Drawing.Size(60, 20);
            this.nudLinks.TabIndex = 13;
            this.nudLinks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLinks.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblWords
            // 
            this.lblWords.AutoSize = true;
            this.lblWords.Location = new System.Drawing.Point(6, 73);
            this.lblWords.Name = "lblWords";
            this.lblWords.Size = new System.Drawing.Size(41, 13);
            this.lblWords.TabIndex = 14;
            this.lblWords.Text = "&Words:";
            this.lblWords.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmboWords
            // 
            this.cmboWords.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboWords.FormattingEnabled = true;
            this.cmboWords.Items.AddRange(new object[] {
            "not counted",
            "at least",
            "at most"});
            this.cmboWords.Location = new System.Drawing.Point(69, 70);
            this.cmboWords.Name = "cmboWords";
            this.cmboWords.Size = new System.Drawing.Size(80, 21);
            this.cmboWords.TabIndex = 15;
            this.cmboWords.SelectedIndexChanged += new System.EventHandler(this.cmboWords_SelectedIndexChanged);
            // 
            // nudWords
            // 
            this.nudWords.Enabled = false;
            this.nudWords.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWords.Location = new System.Drawing.Point(154, 70);
            this.nudWords.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWords.Name = "nudWords";
            this.nudWords.Size = new System.Drawing.Size(60, 20);
            this.nudWords.TabIndex = 16;
            this.nudWords.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudWords.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // nudHeadingSpace
            // 
            this.nudHeadingSpace.Enabled = false;
            this.nudHeadingSpace.Location = new System.Drawing.Point(127, 5);
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
            this.nudHeadingSpace.Size = new System.Drawing.Size(43, 20);
            this.nudHeadingSpace.TabIndex = 1;
            this.nudHeadingSpace.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // chkHeading
            // 
            this.chkHeading.AutoSize = true;
            this.chkHeading.Location = new System.Drawing.Point(3, 6);
            this.chkHeading.Name = "chkHeading";
            this.chkHeading.Size = new System.Drawing.Size(115, 17);
            this.chkHeading.TabIndex = 0;
            this.chkHeading.Text = "Add &heading every";
            this.tooltip.SetToolTip(this.chkHeading, "Add numbered heading");
            this.chkHeading.CheckedChanged += new System.EventHandler(this.chkHeading_CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(201, 246);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.tooltip.SetToolTip(this.btnClear, "Clear");
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(6, 246);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "Copy";
            this.tooltip.SetToolTip(this.btnCopy, "Copy to clipboard");
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransfer.Location = new System.Drawing.Point(201, 3);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 2;
            this.btnTransfer.Text = "Ma&ke";
            this.tooltip.SetToolTip(this.btnTransfer, "Turn list into wiki formatted text, which can be saved or copied.");
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnClearList
            // 
            this.btnClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearList.Location = new System.Drawing.Point(227, 278);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(75, 23);
            this.btnClearList.TabIndex = 3;
            this.btnClearList.Text = "Clear";
            this.tooltip.SetToolTip(this.btnClearList, "Clear the list");
            this.btnClearList.Click += new System.EventHandler(this.lbClear_Click);
            // 
            // chkNoBold
            // 
            this.chkNoBold.AutoSize = true;
            this.chkNoBold.Location = new System.Drawing.Point(3, 3);
            this.chkNoBold.Name = "chkNoBold";
            this.chkNoBold.Size = new System.Drawing.Size(158, 17);
            this.chkNoBold.TabIndex = 0;
            this.chkNoBold.Text = "Has title AWB will embolden";
            this.tooltip.SetToolTip(this.chkNoBold, "AWB \'\'\'emboldens\'\'\' the title when appropriate");
            // 
            // chkCiteTemplateDates
            // 
            this.chkCiteTemplateDates.AutoSize = true;
            this.chkCiteTemplateDates.Location = new System.Drawing.Point(167, 72);
            this.chkCiteTemplateDates.Name = "chkCiteTemplateDates";
            this.chkCiteTemplateDates.Size = new System.Drawing.Size(148, 17);
            this.chkCiteTemplateDates.TabIndex = 7;
            this.chkCiteTemplateDates.Text = "Citation dates AWB will fix";
            this.tooltip.SetToolTip(this.chkCiteTemplateDates, "AWB fixes various errors in dates within the citation family of templates");
            // 
            // chkPeopleCategories
            // 
            this.chkPeopleCategories.AutoSize = true;
            this.chkPeopleCategories.Location = new System.Drawing.Point(3, 118);
            this.chkPeopleCategories.Name = "chkPeopleCategories";
            this.chkPeopleCategories.Size = new System.Drawing.Size(138, 17);
            this.chkPeopleCategories.TabIndex = 10;
            this.chkPeopleCategories.Text = "Human birth/death cats";
            this.tooltip.SetToolTip(this.chkPeopleCategories, "AWB can add/correct birth & death categories for articles about people");
            // 
            // chkUnbalancedBrackets
            // 
            this.chkUnbalancedBrackets.AutoSize = true;
            this.chkUnbalancedBrackets.Location = new System.Drawing.Point(167, 95);
            this.chkUnbalancedBrackets.Name = "chkUnbalancedBrackets";
            this.chkUnbalancedBrackets.Size = new System.Drawing.Size(128, 17);
            this.chkUnbalancedBrackets.TabIndex = 9;
            this.chkUnbalancedBrackets.Text = "Unbalanced brackets";
            this.tooltip.SetToolTip(this.chkUnbalancedBrackets, "AWB identifies unbalanced brackets and is able to fix many of them automatically");
            // 
            // chkBadLinks
            // 
            this.chkBadLinks.AutoSize = true;
            this.chkBadLinks.Location = new System.Drawing.Point(3, 49);
            this.chkBadLinks.Name = "chkBadLinks";
            this.chkBadLinks.Size = new System.Drawing.Size(148, 17);
            this.chkBadLinks.TabIndex = 4;
            this.chkBadLinks.Text = "Has bad links AWB will fix";
            this.tooltip.SetToolTip(this.chkBadLinks, "AWB fixes bad links, such as wrong syntax and URL coding");
            // 
            // chkHasHTML
            // 
            this.chkHasHTML.AutoSize = true;
            this.chkHasHTML.Location = new System.Drawing.Point(3, 72);
            this.chkHasHTML.Name = "chkHasHTML";
            this.chkHasHTML.Size = new System.Drawing.Size(114, 17);
            this.chkHasHTML.TabIndex = 6;
            this.chkHasHTML.Text = "Has HTML entities";
            this.tooltip.SetToolTip(this.chkHasHTML, "AWB replaces HTML entities with unicode");
            // 
            // chkHeaderError
            // 
            this.chkHeaderError.AutoSize = true;
            this.chkHeaderError.Location = new System.Drawing.Point(167, 49);
            this.chkHeaderError.Name = "chkHeaderError";
            this.chkHeaderError.Size = new System.Drawing.Size(86, 17);
            this.chkHeaderError.TabIndex = 5;
            this.chkHeaderError.Text = "Section error";
            this.tooltip.SetToolTip(this.chkHeaderError, "AWB fixes common mis-capitalisation in headings");
            // 
            // chkUnbulletedLinks
            // 
            this.chkUnbulletedLinks.AutoSize = true;
            this.chkUnbulletedLinks.Location = new System.Drawing.Point(3, 95);
            this.chkUnbulletedLinks.Name = "chkUnbulletedLinks";
            this.chkUnbulletedLinks.Size = new System.Drawing.Size(101, 17);
            this.chkUnbulletedLinks.TabIndex = 8;
            this.chkUnbulletedLinks.Text = "Unbulleted links";
            this.tooltip.SetToolTip(this.chkUnbulletedLinks, "AWB bullets links in external links sections");
            // 
            // chkABCHeader
            // 
            this.chkABCHeader.AutoSize = true;
            this.chkABCHeader.Location = new System.Drawing.Point(3, 29);
            this.chkABCHeader.Name = "chkABCHeader";
            this.chkABCHeader.Size = new System.Drawing.Size(133, 17);
            this.chkABCHeader.TabIndex = 3;
            this.chkABCHeader.Text = "Alpha&betised headings";
            this.tooltip.SetToolTip(this.chkABCHeader, "Add alphabetised heading, list must be alphabetised first");
            this.chkABCHeader.CheckedChanged += new System.EventHandler(this.chkABCHeader_CheckedChanged);
            // 
            // txtStartFrom
            // 
            this.txtStartFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStartFrom.Location = new System.Drawing.Point(342, 48);
            this.txtStartFrom.Name = "txtStartFrom";
            this.txtStartFrom.Size = new System.Drawing.Size(172, 20);
            this.txtStartFrom.TabIndex = 3;
            this.tooltip.SetToolTip(this.txtStartFrom, "Page to start scanning from, leave blank to start at beginning");
            // 
            // chkSimpleLinks
            // 
            this.chkSimpleLinks.AutoSize = true;
            this.chkSimpleLinks.Location = new System.Drawing.Point(3, 26);
            this.chkSimpleLinks.Name = "chkSimpleLinks";
            this.chkSimpleLinks.Size = new System.Drawing.Size(150, 17);
            this.chkSimpleLinks.TabIndex = 2;
            this.chkSimpleLinks.Text = "Has links AWB will simplify";
            // 
            // ArticlesListBoxContextMenu
            // 
            this.ArticlesListBoxContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem,
            this.openRevisionInBowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.ArticlesListBoxContextMenu.Name = "contextMenuStrip1";
            this.ArticlesListBoxContextMenu.Size = new System.Drawing.Size(202, 98);
            this.ArticlesListBoxContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openInBrowserToolStripMenuItem.Text = "&Open in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openRevisionInBowserToolStripMenuItem
            // 
            this.openRevisionInBowserToolStripMenuItem.Enabled = false;
            this.openRevisionInBowserToolStripMenuItem.Name = "openRevisionInBowserToolStripMenuItem";
            this.openRevisionInBowserToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.openRevisionInBowserToolStripMenuItem.Text = "Open revision in bowser";
            this.openRevisionInBowserToolStripMenuItem.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // gbOutput
            // 
            this.gbOutput.Controls.Add(this.btnSaveArticleList);
            this.gbOutput.Controls.Add(this.lbArticles);
            this.gbOutput.Controls.Add(this.btnFilter);
            this.gbOutput.Controls.Add(this.btnClearList);
            this.gbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbOutput.Location = new System.Drawing.Point(0, 0);
            this.gbOutput.Name = "gbOutput";
            this.gbOutput.Size = new System.Drawing.Size(308, 307);
            this.gbOutput.TabIndex = 0;
            this.gbOutput.TabStop = false;
            this.gbOutput.Text = "Results";
            // 
            // btnSaveArticleList
            // 
            this.btnSaveArticleList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveArticleList.Location = new System.Drawing.Point(146, 278);
            this.btnSaveArticleList.Name = "btnSaveArticleList";
            this.btnSaveArticleList.Size = new System.Drawing.Size(75, 23);
            this.btnSaveArticleList.TabIndex = 2;
            this.btnSaveArticleList.Text = "Save";
            this.btnSaveArticleList.Click += new System.EventHandler(this.btnSaveArticleList_Click);
            // 
            // lbArticles
            // 
            this.lbArticles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbArticles.ContextMenuStrip = this.ArticlesListBoxContextMenu;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(6, 19);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(296, 251);
            this.lbArticles.TabIndex = 0;
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilter.Location = new System.Drawing.Point(6, 278);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 1;
            this.btnFilter.Text = "&Filter";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblStartFrom
            // 
            this.lblStartFrom.AutoSize = true;
            this.lblStartFrom.Location = new System.Drawing.Point(339, 29);
            this.lblStartFrom.Name = "lblStartFrom";
            this.lblStartFrom.Size = new System.Drawing.Size(82, 13);
            this.lblStartFrom.TabIndex = 2;
            this.lblStartFrom.Text = "Start from p&age:";
            // 
            // lblLimitResutls
            // 
            this.lblLimitResutls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLimitResutls.AutoSize = true;
            this.lblLimitResutls.Location = new System.Drawing.Point(339, 75);
            this.lblLimitResutls.Name = "lblLimitResutls";
            this.lblLimitResutls.Size = new System.Drawing.Size(76, 13);
            this.lblLimitResutls.TabIndex = 2;
            this.lblLimitResutls.Text = "Limit results to:";
            // 
            // nudLimitResults
            // 
            this.nudLimitResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLimitResults.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLimitResults.Location = new System.Drawing.Point(421, 71);
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
            this.nudLimitResults.Size = new System.Drawing.Size(93, 20);
            this.nudLimitResults.TabIndex = 3;
            this.nudLimitResults.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLimitResults.Value = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.nudLimitResults.ValueChanged += new System.EventHandler(this.nudLimitResults_ValueChanged);
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(549, 40);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSaveTxtList
            // 
            this.btnSaveTxtList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveTxtList.Location = new System.Drawing.Point(120, 246);
            this.btnSaveTxtList.Name = "btnSaveTxtList";
            this.btnSaveTxtList.Size = new System.Drawing.Size(75, 23);
            this.btnSaveTxtList.TabIndex = 9;
            this.btnSaveTxtList.Text = "Save";
            this.btnSaveTxtList.Click += new System.EventHandler(this.btnSaveTxtList_Click);
            // 
            // lblListType
            // 
            this.lblListType.AutoSize = true;
            this.lblListType.Location = new System.Drawing.Point(157, 30);
            this.lblListType.Name = "lblListType";
            this.lblListType.Size = new System.Drawing.Size(46, 13);
            this.lblListType.TabIndex = 4;
            this.lblListType.Text = "&List type";
            // 
            // rdoBullet
            // 
            this.rdoBullet.AutoSize = true;
            this.rdoBullet.Location = new System.Drawing.Point(247, 28);
            this.rdoBullet.Name = "rdoBullet";
            this.rdoBullet.Size = new System.Drawing.Size(29, 17);
            this.rdoBullet.TabIndex = 6;
            this.rdoBullet.Text = "*";
            // 
            // rdoHash
            // 
            this.rdoHash.AutoSize = true;
            this.rdoHash.Checked = true;
            this.rdoHash.Location = new System.Drawing.Point(209, 28);
            this.rdoHash.Name = "rdoHash";
            this.rdoHash.Size = new System.Drawing.Size(32, 17);
            this.rdoHash.TabIndex = 5;
            this.rdoHash.TabStop = true;
            this.rdoHash.Text = "#";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblPercentageComplete,
            this.lblCount,
            this.toolStripStatusLabel1,
            this.threadPriorityButton});
            this.statusStrip.Location = new System.Drawing.Point(0, 528);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(636, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "status";
            // 
            // progressBar
            // 
            this.progressBar.Maximum = 200;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblPercentageComplete
            // 
            this.lblPercentageComplete.Name = "lblPercentageComplete";
            this.lblPercentageComplete.Size = new System.Drawing.Size(23, 17);
            this.lblPercentageComplete.Text = "0%";
            // 
            // lblCount
            // 
            this.lblCount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(263, 17);
            this.lblCount.Spring = true;
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(88, 17);
            this.toolStripStatusLabel1.Text = "Thread priority:";
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
            this.threadPriorityButton.Size = new System.Drawing.Size(95, 20);
            this.threadPriorityButton.Text = "Below Normal";
            this.threadPriorityButton.ToolTipText = "Thread Priority";
            // 
            // highestToolStripMenuItem
            // 
            this.highestToolStripMenuItem.CheckOnClick = true;
            this.highestToolStripMenuItem.Name = "highestToolStripMenuItem";
            this.highestToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.highestToolStripMenuItem.Text = "&High";
            this.highestToolStripMenuItem.Click += new System.EventHandler(this.highestToolStripMenuItem_Click);
            // 
            // aboveNormalToolStripMenuItem
            // 
            this.aboveNormalToolStripMenuItem.CheckOnClick = true;
            this.aboveNormalToolStripMenuItem.Name = "aboveNormalToolStripMenuItem";
            this.aboveNormalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.aboveNormalToolStripMenuItem.Text = "&Above Normal";
            this.aboveNormalToolStripMenuItem.Click += new System.EventHandler(this.aboveNormalToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.CheckOnClick = true;
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.normalToolStripMenuItem.Text = "&Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // belowNormalToolStripMenuItem
            // 
            this.belowNormalToolStripMenuItem.Checked = true;
            this.belowNormalToolStripMenuItem.CheckOnClick = true;
            this.belowNormalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.belowNormalToolStripMenuItem.Name = "belowNormalToolStripMenuItem";
            this.belowNormalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.belowNormalToolStripMenuItem.Text = "&Below Normal";
            this.belowNormalToolStripMenuItem.Click += new System.EventHandler(this.belowNormalToolStripMenuItem_Click);
            // 
            // lowestToolStripMenuItem
            // 
            this.lowestToolStripMenuItem.CheckOnClick = true;
            this.lowestToolStripMenuItem.Name = "lowestToolStripMenuItem";
            this.lowestToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.lowestToolStripMenuItem.Text = "&Low";
            this.lowestToolStripMenuItem.Click += new System.EventHandler(this.lowestToolStripMenuItem_Click);
            // 
            // timerProgessUpdate
            // 
            this.timerProgessUpdate.Interval = 300;
            this.timerProgessUpdate.Tick += new System.EventHandler(this.timerProgessUpdate_Tick);
            // 
            // gbAWBSpecific
            // 
            this.gbAWBSpecific.Controls.Add(this.flwAWB);
            this.gbAWBSpecific.Location = new System.Drawing.Point(3, 3);
            this.gbAWBSpecific.Name = "gbAWBSpecific";
            this.gbAWBSpecific.Padding = new System.Windows.Forms.Padding(2);
            this.gbAWBSpecific.Size = new System.Drawing.Size(327, 159);
            this.gbAWBSpecific.TabIndex = 3;
            this.gbAWBSpecific.TabStop = false;
            this.gbAWBSpecific.Text = "A&WB specific";
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
            this.flwAWB.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwAWB.Location = new System.Drawing.Point(3, 13);
            this.flwAWB.Name = "flwAWB";
            this.flwAWB.Size = new System.Drawing.Size(321, 140);
            this.flwAWB.TabIndex = 5;
            // 
            // chkTypo
            // 
            this.chkTypo.AutoSize = true;
            this.chkTypo.Location = new System.Drawing.Point(167, 3);
            this.chkTypo.Name = "chkTypo";
            this.chkTypo.Size = new System.Drawing.Size(50, 17);
            this.chkTypo.TabIndex = 1;
            this.chkTypo.Text = "Typo";
            // 
            // chkDefaultSort
            // 
            this.chkDefaultSort.AutoSize = true;
            this.chkDefaultSort.Location = new System.Drawing.Point(167, 26);
            this.chkDefaultSort.Name = "chkDefaultSort";
            this.chkDefaultSort.Size = new System.Drawing.Size(129, 17);
            this.chkDefaultSort.TabIndex = 3;
            this.chkDefaultSort.Text = "Missing {{defaultsort}}";
            this.chkDefaultSort.UseVisualStyleBackColor = true;
            // 
            // tbParameters
            // 
            this.tbParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbParameters.Controls.Add(this.tabDump);
            this.tbParameters.Controls.Add(this.tabNamespace);
            this.tbParameters.Controls.Add(this.tabTitle);
            this.tbParameters.Controls.Add(this.tabRev);
            this.tbParameters.Controls.Add(this.tabText);
            this.tbParameters.Controls.Add(this.tabAWB);
            this.tbParameters.Controls.Add(this.tabRestrict);
            this.tbParameters.Location = new System.Drawing.Point(13, 12);
            this.tbParameters.Name = "tbParameters";
            this.tbParameters.SelectedIndex = 0;
            this.tbParameters.Size = new System.Drawing.Size(531, 191);
            this.tbParameters.TabIndex = 0;
            // 
            // tabDump
            // 
            this.tabDump.Controls.Add(this.flwHelpLinks);
            this.tabDump.Controls.Add(this.btnAbout);
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
            this.tabDump.Location = new System.Drawing.Point(4, 22);
            this.tabDump.Name = "tabDump";
            this.tabDump.Padding = new System.Windows.Forms.Padding(3);
            this.tabDump.Size = new System.Drawing.Size(523, 165);
            this.tabDump.TabIndex = 0;
            this.tabDump.Text = "Dump";
            this.tabDump.UseVisualStyleBackColor = true;
            // 
            // flwHelpLinks
            // 
            this.flwHelpLinks.Controls.Add(this.lblAlso);
            this.flwHelpLinks.Controls.Add(this.lnkWmfDumps);
            this.flwHelpLinks.Controls.Add(this.lnkWikiaDumps);
            this.flwHelpLinks.Controls.Add(this.lnkGenDump);
            this.flwHelpLinks.Location = new System.Drawing.Point(6, 138);
            this.flwHelpLinks.Name = "flwHelpLinks";
            this.flwHelpLinks.Size = new System.Drawing.Size(406, 19);
            this.flwHelpLinks.TabIndex = 16;
            // 
            // lblAlso
            // 
            this.lblAlso.AutoSize = true;
            this.lblAlso.Location = new System.Drawing.Point(3, 0);
            this.lblAlso.Name = "lblAlso";
            this.lblAlso.Size = new System.Drawing.Size(51, 13);
            this.lblAlso.TabIndex = 11;
            this.lblAlso.Text = "See also:";
            // 
            // lnkWmfDumps
            // 
            this.lnkWmfDumps.AutoSize = true;
            this.lnkWmfDumps.Location = new System.Drawing.Point(60, 0);
            this.lnkWmfDumps.Name = "lnkWmfDumps";
            this.lnkWmfDumps.Size = new System.Drawing.Size(67, 13);
            this.lnkWmfDumps.TabIndex = 12;
            this.lnkWmfDumps.TabStop = true;
            this.lnkWmfDumps.Text = "WMF dumps";
            this.lnkWmfDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWmfDumps_LinkClicked);
            // 
            // lnkWikiaDumps
            // 
            this.lnkWikiaDumps.AutoSize = true;
            this.lnkWikiaDumps.Location = new System.Drawing.Point(133, 0);
            this.lnkWikiaDumps.Name = "lnkWikiaDumps";
            this.lnkWikiaDumps.Size = new System.Drawing.Size(68, 13);
            this.lnkWikiaDumps.TabIndex = 13;
            this.lnkWikiaDumps.TabStop = true;
            this.lnkWikiaDumps.Text = "Wikia dumps";
            this.lnkWikiaDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWikiaDumps_LinkClicked);
            // 
            // lnkGenDump
            // 
            this.lnkGenDump.AutoSize = true;
            this.lnkGenDump.Location = new System.Drawing.Point(207, 0);
            this.lnkGenDump.Name = "lnkGenDump";
            this.lnkGenDump.Size = new System.Drawing.Size(142, 13);
            this.lnkGenDump.TabIndex = 14;
            this.lnkGenDump.TabStop = true;
            this.lnkGenDump.Text = "Generating Database dumps";
            this.lnkGenDump.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGenDump_LinkClicked);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.Location = new System.Drawing.Point(428, 136);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(75, 23);
            this.btnAbout.TabIndex = 15;
            this.btnAbout.Text = "About";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // txtCase
            // 
            this.txtCase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCase.Location = new System.Drawing.Point(89, 110);
            this.txtCase.Name = "txtCase";
            this.txtCase.ReadOnly = true;
            this.txtCase.Size = new System.Drawing.Size(414, 20);
            this.txtCase.TabIndex = 10;
            // 
            // lblCase
            // 
            this.lblCase.AutoSize = true;
            this.lblCase.Location = new System.Drawing.Point(6, 113);
            this.lblCase.Name = "lblCase";
            this.lblCase.Size = new System.Drawing.Size(34, 13);
            this.lblCase.TabIndex = 9;
            this.lblCase.Text = "&Case:";
            // 
            // txtGenerator
            // 
            this.txtGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenerator.Location = new System.Drawing.Point(89, 84);
            this.txtGenerator.Name = "txtGenerator";
            this.txtGenerator.ReadOnly = true;
            this.txtGenerator.Size = new System.Drawing.Size(414, 20);
            this.txtGenerator.TabIndex = 8;
            // 
            // lblGenerator
            // 
            this.lblGenerator.AutoSize = true;
            this.lblGenerator.Location = new System.Drawing.Point(6, 87);
            this.lblGenerator.Name = "lblGenerator";
            this.lblGenerator.Size = new System.Drawing.Size(57, 13);
            this.lblGenerator.TabIndex = 7;
            this.lblGenerator.Text = "&Generator:";
            // 
            // lnkBase
            // 
            this.lnkBase.AutoSize = true;
            this.lnkBase.Location = new System.Drawing.Point(88, 61);
            this.lnkBase.Name = "lnkBase";
            this.lnkBase.Size = new System.Drawing.Size(0, 13);
            this.lnkBase.TabIndex = 6;
            this.lnkBase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBase_LinkClicked);
            // 
            // lblBase
            // 
            this.lblBase.AutoSize = true;
            this.lblBase.Location = new System.Drawing.Point(6, 64);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(34, 13);
            this.lblBase.TabIndex = 5;
            this.lblBase.Text = "Base:";
            // 
            // txtSitename
            // 
            this.txtSitename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSitename.Location = new System.Drawing.Point(89, 32);
            this.txtSitename.Name = "txtSitename";
            this.txtSitename.ReadOnly = true;
            this.txtSitename.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtSitename.Size = new System.Drawing.Size(414, 20);
            this.txtSitename.TabIndex = 4;
            // 
            // lblSitename
            // 
            this.lblSitename.AutoSize = true;
            this.lblSitename.Location = new System.Drawing.Point(6, 35);
            this.lblSitename.Name = "lblSitename";
            this.lblSitename.Size = new System.Drawing.Size(57, 13);
            this.lblSitename.TabIndex = 3;
            this.lblSitename.Text = "Site &name:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(428, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Br&owse...";
            this.btnBrowse.Click += new System.EventHandler(this.btnOpen);
            // 
            // txtDumpLocation
            // 
            this.txtDumpLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDumpLocation.Location = new System.Drawing.Point(89, 6);
            this.txtDumpLocation.Name = "txtDumpLocation";
            this.txtDumpLocation.Size = new System.Drawing.Size(333, 20);
            this.txtDumpLocation.TabIndex = 1;
            // 
            // lblDBDump
            // 
            this.lblDBDump.AutoSize = true;
            this.lblDBDump.Location = new System.Drawing.Point(6, 9);
            this.lblDBDump.Name = "lblDBDump";
            this.lblDBDump.Size = new System.Drawing.Size(85, 13);
            this.lblDBDump.TabIndex = 0;
            this.lblDBDump.Text = "&Database dump:";
            // 
            // tabNamespace
            // 
            this.tabNamespace.Controls.Add(this.pageNamespaces);
            this.tabNamespace.Location = new System.Drawing.Point(4, 22);
            this.tabNamespace.Name = "tabNamespace";
            this.tabNamespace.Padding = new System.Windows.Forms.Padding(3);
            this.tabNamespace.Size = new System.Drawing.Size(523, 165);
            this.tabNamespace.TabIndex = 1;
            this.tabNamespace.Text = "Namespace";
            this.tabNamespace.UseVisualStyleBackColor = true;
            // 
            // pageNamespaces
            // 
            this.pageNamespaces.Location = new System.Drawing.Point(6, 3);
            this.pageNamespaces.Name = "pageNamespaces";
            this.pageNamespaces.Size = new System.Drawing.Size(297, 162);
            this.pageNamespaces.TabIndex = 0;
            // 
            // tabTitle
            // 
            this.tabTitle.Controls.Add(this.tableLayoutPanel2);
            this.tabTitle.Location = new System.Drawing.Point(4, 22);
            this.tabTitle.Name = "tabTitle";
            this.tabTitle.Padding = new System.Windows.Forms.Padding(3);
            this.tabTitle.Size = new System.Drawing.Size(523, 165);
            this.tabTitle.TabIndex = 6;
            this.tabTitle.Text = "Title";
            this.tabTitle.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.chkTitleCaseSensitive, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleContains, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleRegex, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleContains, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleDoesNotContain, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleNotContains, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(517, 159);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // chkTitleCaseSensitive
            // 
            this.chkTitleCaseSensitive.AutoSize = true;
            this.chkTitleCaseSensitive.Checked = true;
            this.chkTitleCaseSensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTitleCaseSensitive.Location = new System.Drawing.Point(161, 55);
            this.chkTitleCaseSensitive.Name = "chkTitleCaseSensitive";
            this.chkTitleCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkTitleCaseSensitive.TabIndex = 5;
            this.chkTitleCaseSensitive.Text = "Case &sensitive";
            // 
            // chkTitleContains
            // 
            this.chkTitleContains.AutoSize = true;
            this.chkTitleContains.Location = new System.Drawing.Point(3, 3);
            this.chkTitleContains.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkTitleContains.Name = "chkTitleContains";
            this.chkTitleContains.Size = new System.Drawing.Size(92, 17);
            this.chkTitleContains.TabIndex = 0;
            this.chkTitleContains.Text = "Title &contains:";
            this.chkTitleContains.CheckedChanged += new System.EventHandler(this.chkTitleContains_CheckedChanged);
            // 
            // chkTitleRegex
            // 
            this.chkTitleRegex.AutoSize = true;
            this.chkTitleRegex.Location = new System.Drawing.Point(98, 55);
            this.chkTitleRegex.Name = "chkTitleRegex";
            this.chkTitleRegex.Size = new System.Drawing.Size(57, 17);
            this.chkTitleRegex.TabIndex = 4;
            this.chkTitleRegex.Text = "&Regex";
            // 
            // txtTitleContains
            // 
            this.txtTitleContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleContains, 2);
            this.txtTitleContains.Enabled = false;
            this.txtTitleContains.Location = new System.Drawing.Point(98, 3);
            this.txtTitleContains.Name = "txtTitleContains";
            this.txtTitleContains.Size = new System.Drawing.Size(416, 20);
            this.txtTitleContains.TabIndex = 1;
            this.txtTitleContains.Leave += new System.EventHandler(this.txtTitleContains_Leave);
            // 
            // chkTitleDoesNotContain
            // 
            this.chkTitleDoesNotContain.AutoSize = true;
            this.chkTitleDoesNotContain.Location = new System.Drawing.Point(3, 29);
            this.chkTitleDoesNotContain.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkTitleDoesNotContain.Name = "chkTitleDoesNotContain";
            this.chkTitleDoesNotContain.Size = new System.Drawing.Size(89, 17);
            this.chkTitleDoesNotContain.TabIndex = 2;
            this.chkTitleDoesNotContain.Text = "&Not contains:";
            this.chkTitleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkTitleDoesNotContain_CheckedChanged);
            // 
            // txtTitleNotContains
            // 
            this.txtTitleNotContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleNotContains, 2);
            this.txtTitleNotContains.Enabled = false;
            this.txtTitleNotContains.Location = new System.Drawing.Point(98, 29);
            this.txtTitleNotContains.Name = "txtTitleNotContains";
            this.txtTitleNotContains.Size = new System.Drawing.Size(416, 20);
            this.txtTitleNotContains.TabIndex = 3;
            this.txtTitleNotContains.Leave += new System.EventHandler(this.txtTitleNotContains_Leave);
            // 
            // tabRev
            // 
            this.tabRev.Controls.Add(this.chkSearchDates);
            this.tabRev.Controls.Add(this.lblDate);
            this.tabRev.Controls.Add(this.dtpFrom);
            this.tabRev.Controls.Add(this.lblEndDate);
            this.tabRev.Controls.Add(this.dtpTo);
            this.tabRev.Location = new System.Drawing.Point(4, 22);
            this.tabRev.Name = "tabRev";
            this.tabRev.Padding = new System.Windows.Forms.Padding(3);
            this.tabRev.Size = new System.Drawing.Size(523, 165);
            this.tabRev.TabIndex = 4;
            this.tabRev.Text = "Revision";
            this.tabRev.UseVisualStyleBackColor = true;
            // 
            // chkSearchDates
            // 
            this.chkSearchDates.AutoSize = true;
            this.chkSearchDates.Location = new System.Drawing.Point(6, 10);
            this.chkSearchDates.Name = "chkSearchDates";
            this.chkSearchDates.Size = new System.Drawing.Size(84, 17);
            this.chkSearchDates.TabIndex = 9;
            this.chkSearchDates.Text = "&Search date";
            this.chkSearchDates.CheckedChanged += new System.EventHandler(this.chkSearchDates_CheckedChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(22, 45);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(33, 13);
            this.lblDate.TabIndex = 6;
            this.lblDate.Text = "&From:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Enabled = false;
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(58, 39);
            this.dtpFrom.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(88, 20);
            this.dtpFrom.TabIndex = 7;
            this.dtpFrom.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(22, 69);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(23, 13);
            this.lblEndDate.TabIndex = 8;
            this.lblEndDate.Text = "&To:";
            // 
            // dtpTo
            // 
            this.dtpTo.Checked = false;
            this.dtpTo.Enabled = false;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(58, 65);
            this.dtpTo.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(88, 20);
            this.dtpTo.TabIndex = 5;
            // 
            // tabText
            // 
            this.tabText.Controls.Add(this.gbProperties);
            this.tabText.Controls.Add(this.gbText);
            this.tabText.Location = new System.Drawing.Point(4, 22);
            this.tabText.Name = "tabText";
            this.tabText.Padding = new System.Windows.Forms.Padding(3);
            this.tabText.Size = new System.Drawing.Size(523, 165);
            this.tabText.TabIndex = 2;
            this.tabText.Text = "Text";
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
            this.gbProperties.Location = new System.Drawing.Point(289, 6);
            this.gbProperties.Name = "gbProperties";
            this.gbProperties.Size = new System.Drawing.Size(218, 141);
            this.gbProperties.TabIndex = 18;
            this.gbProperties.TabStop = false;
            this.gbProperties.Text = "Page Text Properties";
            // 
            // chkIgnoreRedirects
            // 
            this.chkIgnoreRedirects.AutoSize = true;
            this.chkIgnoreRedirects.Checked = true;
            this.chkIgnoreRedirects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreRedirects.Location = new System.Drawing.Point(69, 97);
            this.chkIgnoreRedirects.Name = "chkIgnoreRedirects";
            this.chkIgnoreRedirects.Size = new System.Drawing.Size(99, 17);
            this.chkIgnoreRedirects.TabIndex = 17;
            this.chkIgnoreRedirects.Text = "Ignore redirects";
            this.chkIgnoreRedirects.UseVisualStyleBackColor = true;
            // 
            // tabAWB
            // 
            this.tabAWB.Controls.Add(this.gbAWBSpecific);
            this.tabAWB.Controls.Add(this.nudLimitResults);
            this.tabAWB.Controls.Add(this.lblStartFrom);
            this.tabAWB.Controls.Add(this.txtStartFrom);
            this.tabAWB.Controls.Add(this.lblLimitResutls);
            this.tabAWB.Location = new System.Drawing.Point(4, 22);
            this.tabAWB.Name = "tabAWB";
            this.tabAWB.Padding = new System.Windows.Forms.Padding(3);
            this.tabAWB.Size = new System.Drawing.Size(523, 165);
            this.tabAWB.TabIndex = 3;
            this.tabAWB.Text = "Searching";
            this.tabAWB.UseVisualStyleBackColor = true;
            // 
            // tabRestrict
            // 
            this.tabRestrict.Controls.Add(this.chkProtection);
            this.tabRestrict.Controls.Add(this.MoveDelete);
            this.tabRestrict.Location = new System.Drawing.Point(4, 22);
            this.tabRestrict.Name = "tabRestrict";
            this.tabRestrict.Padding = new System.Windows.Forms.Padding(3);
            this.tabRestrict.Size = new System.Drawing.Size(523, 165);
            this.tabRestrict.TabIndex = 5;
            this.tabRestrict.Text = "Restriction";
            this.tabRestrict.UseVisualStyleBackColor = true;
            // 
            // chkProtection
            // 
            this.chkProtection.AutoSize = true;
            this.chkProtection.Location = new System.Drawing.Point(6, 10);
            this.chkProtection.Name = "chkProtection";
            this.chkProtection.Size = new System.Drawing.Size(110, 17);
            this.chkProtection.TabIndex = 1;
            this.chkProtection.Text = "Check Restriction";
            this.chkProtection.UseVisualStyleBackColor = true;
            this.chkProtection.CheckedChanged += new System.EventHandler(this.chkProtection_CheckedChanged);
            // 
            // MoveDelete
            // 
            this.MoveDelete.EditProtectionLevel = "";
            this.MoveDelete.Enabled = false;
            this.MoveDelete.Location = new System.Drawing.Point(6, 33);
            this.MoveDelete.MoveProtectionLevel = "";
            this.MoveDelete.Name = "MoveDelete";
            this.MoveDelete.Size = new System.Drawing.Size(311, 101);
            this.MoveDelete.TabIndex = 0;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(549, 98);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem2.Text = "&High";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem3.Text = "&AboveNormal";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem4.Text = "&Normal";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Checked = true;
            this.toolStripMenuItem5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem5.Text = "&BellowNormal";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(138, 22);
            this.toolStripMenuItem6.Text = "&Low";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConvert);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 307);
            this.tabControl1.TabIndex = 5;
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
            this.tabConvert.Location = new System.Drawing.Point(4, 22);
            this.tabConvert.Name = "tabConvert";
            this.tabConvert.Padding = new System.Windows.Forms.Padding(3);
            this.tabConvert.Size = new System.Drawing.Size(282, 281);
            this.tabConvert.TabIndex = 1;
            this.tabConvert.Text = "Convert";
            this.tabConvert.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(550, 69);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(74, 23);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "&Pause";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(12, 209);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbOutput);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(602, 307);
            this.splitContainer1.SplitterDistance = 308;
            this.splitContainer1.TabIndex = 5;
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt|CSV (Comma Separated Values" +
                ")|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // DatabaseScanner
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 550);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.tbParameters);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnReset);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(642, 575);
            this.Name = "DatabaseScanner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wiki Database Scanner";
            this.Load += new System.EventHandler(this.DatabaseScanner_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DatabaseScanner_FormClosing);
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
            this.flwHelpLinks.ResumeLayout(false);
            this.flwHelpLinks.PerformLayout();
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
            this.tabControl1.ResumeLayout(false);
            this.tabConvert.ResumeLayout(false);
            this.tabConvert.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
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
        private ToolTip tooltip;
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
        private WikiFunctions.Controls.Lists.ListBox2 lbArticles;
        private CheckBox chkUnbulletedLinks;
        private CheckBox chkHeaderError;
        private CheckBox chkHasHTML;
        private CheckBox chkBadLinks;
        private CheckBox chkSimpleLinks;
        private CheckBox chkNoBold;
        private CheckBox chkCiteTemplateDates;
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
        private ToolStripMenuItem openRevisionInBowserToolStripMenuItem;
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
        private Button btnAbout;
        private Button btnReset;
        private Label lblDBDump;
        private TextBox txtSitename;
        private ToolStripSeparator toolStripSeparator1;
        private GroupBox gbProperties;
        private TextBox txtGenerator;
        private LinkLabel lnkWikiaDumps;
        private LinkLabel lnkWmfDumps;
        private LinkLabel lnkGenDump;
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
        private SaveFileDialog saveListDialog;
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
    }
}