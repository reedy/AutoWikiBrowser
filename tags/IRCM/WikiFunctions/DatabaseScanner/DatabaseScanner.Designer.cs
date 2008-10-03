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
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
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
            this.txtTitleNotContains = new System.Windows.Forms.TextBox();
            this.chkTitleDoesNotContain = new System.Windows.Forms.CheckBox();
            this.txtTitleContains = new System.Windows.Forms.TextBox();
            this.chkTitleContains = new System.Windows.Forms.CheckBox();
            this.gbTitle = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkTitleCase = new System.Windows.Forms.CheckBox();
            this.chkTitleRegex = new System.Windows.Forms.CheckBox();
            this.nudHeadingSpace = new System.Windows.Forms.NumericUpDown();
            this.chkHeading = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.chkNoBold = new System.Windows.Forms.CheckBox();
            this.chkBadLinks = new System.Windows.Forms.CheckBox();
            this.chkHasHTML = new System.Windows.Forms.CheckBox();
            this.chkHeaderError = new System.Windows.Forms.CheckBox();
            this.chkUnbulletedLinks = new System.Windows.Forms.CheckBox();
            this.chkABCHeader = new System.Windows.Forms.CheckBox();
            this.txtStartFrom = new System.Windows.Forms.TextBox();
            this.chkProjectNamespace = new System.Windows.Forms.CheckBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSaveTxtList = new System.Windows.Forms.Button();
            this.lblListType = new System.Windows.Forms.Label();
            this.rdoBullet = new System.Windows.Forms.RadioButton();
            this.rdoHash = new System.Windows.Forms.RadioButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
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
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.gbDate = new System.Windows.Forms.GroupBox();
            this.chkSearchDates = new System.Windows.Forms.CheckBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
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
            this.tabProps = new System.Windows.Forms.TabPage();
            this.gbNamespace = new System.Windows.Forms.GroupBox();
            this.flwNamespace = new System.Windows.Forms.FlowLayoutPanel();
            this.chkMainNamespace = new System.Windows.Forms.CheckBox();
            this.chkTemplateNamespace = new System.Windows.Forms.CheckBox();
            this.chkCategoryNamespace = new System.Windows.Forms.CheckBox();
            this.chkImageNamespace = new System.Windows.Forms.CheckBox();
            this.tabRev = new System.Windows.Forms.TabPage();
            this.tabText = new System.Windows.Forms.TabPage();
            this.gbProperties = new System.Windows.Forms.GroupBox();
            this.chkIgnoreRedirects = new System.Windows.Forms.CheckBox();
            this.tabAWB = new System.Windows.Forms.TabPage();
            this.tabRestrict = new System.Windows.Forms.TabPage();
            this.grpEditMove = new System.Windows.Forms.GroupBox();
            this.chkProtection = new System.Windows.Forms.CheckBox();
            this.MoveDelete = new WikiFunctions.Controls.EditProtectControl();
            this.btnReset = new System.Windows.Forms.Button();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbConvert = new System.Windows.Forms.TabPage();
            this.btnPause = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.gbText.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).BeginInit();
            this.gbTitle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).BeginInit();
            this.ArticlesListBoxContextMenu.SuspendLayout();
            this.gbOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.gbAWBSpecific.SuspendLayout();
            this.flwAWB.SuspendLayout();
            this.gbDate.SuspendLayout();
            this.tbParameters.SuspendLayout();
            this.tabDump.SuspendLayout();
            this.flwHelpLinks.SuspendLayout();
            this.tabProps.SuspendLayout();
            this.gbNamespace.SuspendLayout();
            this.flwNamespace.SuspendLayout();
            this.tabRev.SuspendLayout();
            this.tabText.SuspendLayout();
            this.gbProperties.SuspendLayout();
            this.tabAWB.SuspendLayout();
            this.tabRestrict.SuspendLayout();
            this.grpEditMove.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tbConvert.SuspendLayout();
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
            this.txtList.Location = new System.Drawing.Point(6, 68);
            this.txtList.MaxLength = 0;
            this.txtList.Multiline = true;
            this.txtList.Name = "txtList";
            this.txtList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtList.Size = new System.Drawing.Size(264, 172);
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
            this.tableLayoutPanel3.Controls.Add(this.chkRegex, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.chkCaseSensitive, 1, 3);
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
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(98, 55);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(57, 17);
            this.chkRegex.TabIndex = 4;
            this.chkRegex.Text = "&Regex";
            this.chkRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // chkCaseSensitive
            // 
            this.chkCaseSensitive.AutoSize = true;
            this.chkCaseSensitive.Location = new System.Drawing.Point(98, 78);
            this.chkCaseSensitive.Name = "chkCaseSensitive";
            this.chkCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkCaseSensitive.TabIndex = 5;
            this.chkCaseSensitive.Text = "Case sens&itive";
            this.tooltip.SetToolTip(this.chkCaseSensitive, "Changes case sensitivity");
            // 
            // chkSingle
            // 
            this.chkSingle.AutoSize = true;
            this.chkSingle.Enabled = false;
            this.chkSingle.Location = new System.Drawing.Point(198, 55);
            this.chkSingle.Name = "chkSingle";
            this.chkSingle.Size = new System.Drawing.Size(71, 17);
            this.chkSingle.TabIndex = 6;
            this.chkSingle.Text = "&Singleline";
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
            this.lblLength.Location = new System.Drawing.Point(5, 20);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(58, 13);
            this.lblLength.TabIndex = 8;
            this.lblLength.Text = "Characters";
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
            this.lblLinks.Location = new System.Drawing.Point(31, 46);
            this.lblLinks.Name = "lblLinks";
            this.lblLinks.Size = new System.Drawing.Size(32, 13);
            this.lblLinks.TabIndex = 11;
            this.lblLinks.Text = "Links";
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
            this.lblWords.Location = new System.Drawing.Point(25, 72);
            this.lblWords.Name = "lblWords";
            this.lblWords.Size = new System.Drawing.Size(38, 13);
            this.lblWords.TabIndex = 14;
            this.lblWords.Text = "Words";
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
            // txtTitleNotContains
            // 
            this.txtTitleNotContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleNotContains, 2);
            this.txtTitleNotContains.Enabled = false;
            this.txtTitleNotContains.Location = new System.Drawing.Point(98, 29);
            this.txtTitleNotContains.Name = "txtTitleNotContains";
            this.txtTitleNotContains.Size = new System.Drawing.Size(233, 20);
            this.txtTitleNotContains.TabIndex = 3;
            this.txtTitleNotContains.Leave += new System.EventHandler(this.txtTitleNotContains_Leave);
            // 
            // chkTitleDoesNotContain
            // 
            this.chkTitleDoesNotContain.AutoSize = true;
            this.chkTitleDoesNotContain.Location = new System.Drawing.Point(3, 29);
            this.chkTitleDoesNotContain.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkTitleDoesNotContain.Name = "chkTitleDoesNotContain";
            this.chkTitleDoesNotContain.Size = new System.Drawing.Size(89, 17);
            this.chkTitleDoesNotContain.TabIndex = 2;
            this.chkTitleDoesNotContain.Text = "Not contains:";
            this.chkTitleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkCheckNotInTitle_CheckedChanged);
            // 
            // txtTitleContains
            // 
            this.txtTitleContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.SetColumnSpan(this.txtTitleContains, 2);
            this.txtTitleContains.Enabled = false;
            this.txtTitleContains.Location = new System.Drawing.Point(98, 3);
            this.txtTitleContains.Name = "txtTitleContains";
            this.txtTitleContains.Size = new System.Drawing.Size(233, 20);
            this.txtTitleContains.TabIndex = 1;
            this.txtTitleContains.Leave += new System.EventHandler(this.txtTitleContains_Leave);
            // 
            // chkTitleContains
            // 
            this.chkTitleContains.AutoSize = true;
            this.chkTitleContains.Location = new System.Drawing.Point(3, 3);
            this.chkTitleContains.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.chkTitleContains.Name = "chkTitleContains";
            this.chkTitleContains.Size = new System.Drawing.Size(92, 17);
            this.chkTitleContains.TabIndex = 0;
            this.chkTitleContains.Text = "Title contains:";
            this.chkTitleContains.CheckedChanged += new System.EventHandler(this.chkCheckTitle_CheckedChanged);
            // 
            // gbTitle
            // 
            this.gbTitle.Controls.Add(this.tableLayoutPanel2);
            this.gbTitle.Location = new System.Drawing.Point(101, 6);
            this.gbTitle.Name = "gbTitle";
            this.gbTitle.Size = new System.Drawing.Size(340, 106);
            this.gbTitle.TabIndex = 0;
            this.gbTitle.TabStop = false;
            this.gbTitle.Text = "&Title matching";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.chkTitleCase, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleContains, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleRegex, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleContains, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkTitleDoesNotContain, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.txtTitleNotContains, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(334, 87);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // chkTitleCase
            // 
            this.chkTitleCase.AutoSize = true;
            this.chkTitleCase.Checked = true;
            this.chkTitleCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTitleCase.Location = new System.Drawing.Point(161, 55);
            this.chkTitleCase.Name = "chkTitleCase";
            this.chkTitleCase.Size = new System.Drawing.Size(94, 17);
            this.chkTitleCase.TabIndex = 5;
            this.chkTitleCase.Text = "Case sensitive";
            // 
            // chkTitleRegex
            // 
            this.chkTitleRegex.AutoSize = true;
            this.chkTitleRegex.Location = new System.Drawing.Point(98, 55);
            this.chkTitleRegex.Name = "chkTitleRegex";
            this.chkTitleRegex.Size = new System.Drawing.Size(57, 17);
            this.chkTitleRegex.TabIndex = 4;
            this.chkTitleRegex.Text = "Regex";
            // 
            // nudHeadingSpace
            // 
            this.nudHeadingSpace.Enabled = false;
            this.nudHeadingSpace.Location = new System.Drawing.Point(124, 18);
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
            this.chkHeading.Location = new System.Drawing.Point(3, 19);
            this.chkHeading.Name = "chkHeading";
            this.chkHeading.Size = new System.Drawing.Size(115, 17);
            this.chkHeading.TabIndex = 0;
            this.chkHeading.Text = "Add heading every";
            this.tooltip.SetToolTip(this.chkHeading, "Add numbered heading");
            this.chkHeading.CheckedChanged += new System.EventHandler(this.chkHeading_CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(195, 246);
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
            this.btnCopy.Location = new System.Drawing.Point(3, 246);
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
            this.btnTransfer.Location = new System.Drawing.Point(195, 15);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(75, 23);
            this.btnTransfer.TabIndex = 6;
            this.btnTransfer.Text = "Make";
            this.tooltip.SetToolTip(this.btnTransfer, "Turn list into wiki formatted text, which can be saved or copied.");
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnClearList
            // 
            this.btnClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearList.Location = new System.Drawing.Point(227, 278);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(75, 23);
            this.btnClearList.TabIndex = 8;
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
            // chkBadLinks
            // 
            this.chkBadLinks.AutoSize = true;
            this.chkBadLinks.Location = new System.Drawing.Point(3, 49);
            this.chkBadLinks.Name = "chkBadLinks";
            this.chkBadLinks.Size = new System.Drawing.Size(148, 17);
            this.chkBadLinks.TabIndex = 2;
            this.chkBadLinks.Text = "Has bad links AWB will fix";
            this.tooltip.SetToolTip(this.chkBadLinks, "AWB fixes bad links, such as wrong syntax and URL coding");
            // 
            // chkHasHTML
            // 
            this.chkHasHTML.AutoSize = true;
            this.chkHasHTML.Location = new System.Drawing.Point(3, 72);
            this.chkHasHTML.Name = "chkHasHTML";
            this.chkHasHTML.Size = new System.Drawing.Size(114, 17);
            this.chkHasHTML.TabIndex = 3;
            this.chkHasHTML.Text = "Has HTML entities";
            this.tooltip.SetToolTip(this.chkHasHTML, "AWB replaces HTML entities with unicode");
            // 
            // chkHeaderError
            // 
            this.chkHeaderError.AutoSize = true;
            this.chkHeaderError.Location = new System.Drawing.Point(167, 3);
            this.chkHeaderError.Name = "chkHeaderError";
            this.chkHeaderError.Size = new System.Drawing.Size(86, 17);
            this.chkHeaderError.TabIndex = 4;
            this.chkHeaderError.Text = "Section error";
            this.tooltip.SetToolTip(this.chkHeaderError, "AWB fixes common mis-capitalisation in headings");
            // 
            // chkUnbulletedLinks
            // 
            this.chkUnbulletedLinks.AutoSize = true;
            this.chkUnbulletedLinks.Location = new System.Drawing.Point(167, 26);
            this.chkUnbulletedLinks.Name = "chkUnbulletedLinks";
            this.chkUnbulletedLinks.Size = new System.Drawing.Size(101, 17);
            this.chkUnbulletedLinks.TabIndex = 5;
            this.chkUnbulletedLinks.Text = "Unbulleted links";
            this.tooltip.SetToolTip(this.chkUnbulletedLinks, "AWB bullets links in external links sections");
            // 
            // chkABCHeader
            // 
            this.chkABCHeader.AutoSize = true;
            this.chkABCHeader.Location = new System.Drawing.Point(3, 43);
            this.chkABCHeader.Name = "chkABCHeader";
            this.chkABCHeader.Size = new System.Drawing.Size(133, 17);
            this.chkABCHeader.TabIndex = 2;
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
            this.txtStartFrom.Size = new System.Drawing.Size(161, 20);
            this.txtStartFrom.TabIndex = 3;
            this.tooltip.SetToolTip(this.txtStartFrom, "Page to start scanning from, leave blank to start at beginning");
            // 
            // chkProjectNamespace
            // 
            this.chkProjectNamespace.AutoSize = true;
            this.chkProjectNamespace.Location = new System.Drawing.Point(3, 95);
            this.chkProjectNamespace.Name = "chkProjectNamespace";
            this.chkProjectNamespace.Size = new System.Drawing.Size(59, 17);
            this.chkProjectNamespace.TabIndex = 4;
            this.chkProjectNamespace.Text = "Project";
            this.tooltip.SetToolTip(this.chkProjectNamespace, "Typically the name of the wiki, so Wikipedia, Wikibooks, etc...");
            // 
            // chkSimpleLinks
            // 
            this.chkSimpleLinks.AutoSize = true;
            this.chkSimpleLinks.Location = new System.Drawing.Point(3, 26);
            this.chkSimpleLinks.Name = "chkSimpleLinks";
            this.chkSimpleLinks.Size = new System.Drawing.Size(150, 17);
            this.chkSimpleLinks.TabIndex = 1;
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
            this.ArticlesListBoxContextMenu.Size = new System.Drawing.Size(201, 98);
            this.ArticlesListBoxContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.openInBrowserToolStripMenuItem.Text = "&Open in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openRevisionInBowserToolStripMenuItem
            // 
            this.openRevisionInBowserToolStripMenuItem.Enabled = false;
            this.openRevisionInBowserToolStripMenuItem.Name = "openRevisionInBowserToolStripMenuItem";
            this.openRevisionInBowserToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.openRevisionInBowserToolStripMenuItem.Text = "Open revision in bowser";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
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
            this.btnSaveArticleList.TabIndex = 12;
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
            this.lbArticles.TabIndex = 5;
            // 
            // btnFilter
            // 
            this.btnFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFilter.Location = new System.Drawing.Point(6, 278);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "Filter";
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
            this.lblLimitResutls.Location = new System.Drawing.Point(349, 9);
            this.lblLimitResutls.Name = "lblLimitResutls";
            this.lblLimitResutls.Size = new System.Drawing.Size(73, 13);
            this.lblLimitResutls.TabIndex = 2;
            this.lblLimitResutls.Text = "Limit results to";
            // 
            // nudLimitResults
            // 
            this.nudLimitResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLimitResults.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLimitResults.Location = new System.Drawing.Point(428, 6);
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
            this.nudLimitResults.Size = new System.Drawing.Size(75, 20);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSaveTxtList);
            this.groupBox4.Controls.Add(this.lblListType);
            this.groupBox4.Controls.Add(this.chkABCHeader);
            this.groupBox4.Controls.Add(this.rdoBullet);
            this.groupBox4.Controls.Add(this.rdoHash);
            this.groupBox4.Controls.Add(this.txtList);
            this.groupBox4.Controls.Add(this.chkHeading);
            this.groupBox4.Controls.Add(this.nudHeadingSpace);
            this.groupBox4.Controls.Add(this.btnClear);
            this.groupBox4.Controls.Add(this.btnTransfer);
            this.groupBox4.Controls.Add(this.btnCopy);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(276, 275);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Convert results into &list";
            // 
            // btnSaveTxtList
            // 
            this.btnSaveTxtList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveTxtList.Location = new System.Drawing.Point(114, 246);
            this.btnSaveTxtList.Name = "btnSaveTxtList";
            this.btnSaveTxtList.Size = new System.Drawing.Size(75, 23);
            this.btnSaveTxtList.TabIndex = 11;
            this.btnSaveTxtList.Text = "Save";
            this.btnSaveTxtList.Click += new System.EventHandler(this.btnSaveTxtList_Click);
            // 
            // lblListType
            // 
            this.lblListType.AutoSize = true;
            this.lblListType.Location = new System.Drawing.Point(151, 45);
            this.lblListType.Name = "lblListType";
            this.lblListType.Size = new System.Drawing.Size(46, 13);
            this.lblListType.TabIndex = 3;
            this.lblListType.Text = "List type";
            // 
            // rdoBullet
            // 
            this.rdoBullet.AutoSize = true;
            this.rdoBullet.Location = new System.Drawing.Point(241, 43);
            this.rdoBullet.Name = "rdoBullet";
            this.rdoBullet.Size = new System.Drawing.Size(29, 17);
            this.rdoBullet.TabIndex = 5;
            this.rdoBullet.Text = "*";
            // 
            // rdoHash
            // 
            this.rdoHash.AutoSize = true;
            this.rdoHash.Checked = true;
            this.rdoHash.Location = new System.Drawing.Point(203, 43);
            this.rdoHash.Name = "rdoHash";
            this.rdoHash.Size = new System.Drawing.Size(32, 17);
            this.rdoHash.TabIndex = 4;
            this.rdoHash.TabStop = true;
            this.rdoHash.Text = "#";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblCount,
            this.toolStripStatusLabel1,
            this.threadPriorityButton});
            this.statusStrip.Location = new System.Drawing.Point(0, 527);
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
            // lblCount
            // 
            this.lblCount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(272, 17);
            this.lblCount.Spring = true;
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(82, 17);
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
            this.threadPriorityButton.Size = new System.Drawing.Size(84, 20);
            this.threadPriorityButton.Text = "Below Normal";
            this.threadPriorityButton.ToolTipText = "Thread Priority";
            // 
            // highestToolStripMenuItem
            // 
            this.highestToolStripMenuItem.CheckOnClick = true;
            this.highestToolStripMenuItem.Name = "highestToolStripMenuItem";
            this.highestToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.highestToolStripMenuItem.Text = "&High";
            this.highestToolStripMenuItem.Click += new System.EventHandler(this.highestToolStripMenuItem_Click);
            // 
            // aboveNormalToolStripMenuItem
            // 
            this.aboveNormalToolStripMenuItem.CheckOnClick = true;
            this.aboveNormalToolStripMenuItem.Name = "aboveNormalToolStripMenuItem";
            this.aboveNormalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboveNormalToolStripMenuItem.Text = "&Above Normal";
            this.aboveNormalToolStripMenuItem.Click += new System.EventHandler(this.aboveNormalToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.CheckOnClick = true;
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.normalToolStripMenuItem.Text = "&Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // belowNormalToolStripMenuItem
            // 
            this.belowNormalToolStripMenuItem.Checked = true;
            this.belowNormalToolStripMenuItem.CheckOnClick = true;
            this.belowNormalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.belowNormalToolStripMenuItem.Name = "belowNormalToolStripMenuItem";
            this.belowNormalToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.belowNormalToolStripMenuItem.Text = "&Below Normal";
            this.belowNormalToolStripMenuItem.Click += new System.EventHandler(this.belowNormalToolStripMenuItem_Click);
            // 
            // lowestToolStripMenuItem
            // 
            this.lowestToolStripMenuItem.CheckOnClick = true;
            this.lowestToolStripMenuItem.Name = "lowestToolStripMenuItem";
            this.lowestToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            this.gbAWBSpecific.Location = new System.Drawing.Point(6, 6);
            this.gbAWBSpecific.Name = "gbAWBSpecific";
            this.gbAWBSpecific.Size = new System.Drawing.Size(321, 116);
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
            this.flwAWB.Controls.Add(this.chkHeaderError);
            this.flwAWB.Controls.Add(this.chkUnbulletedLinks);
            this.flwAWB.Controls.Add(this.chkTypo);
            this.flwAWB.Controls.Add(this.chkDefaultSort);
            this.flwAWB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwAWB.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwAWB.Location = new System.Drawing.Point(3, 16);
            this.flwAWB.Name = "flwAWB";
            this.flwAWB.Size = new System.Drawing.Size(315, 97);
            this.flwAWB.TabIndex = 5;
            // 
            // chkTypo
            // 
            this.chkTypo.AutoSize = true;
            this.chkTypo.Location = new System.Drawing.Point(167, 49);
            this.chkTypo.Name = "chkTypo";
            this.chkTypo.Size = new System.Drawing.Size(50, 17);
            this.chkTypo.TabIndex = 7;
            this.chkTypo.Text = "Typo";
            // 
            // chkDefaultSort
            // 
            this.chkDefaultSort.AutoSize = true;
            this.chkDefaultSort.Location = new System.Drawing.Point(167, 72);
            this.chkDefaultSort.Name = "chkDefaultSort";
            this.chkDefaultSort.Size = new System.Drawing.Size(129, 17);
            this.chkDefaultSort.TabIndex = 8;
            this.chkDefaultSort.Text = "Missing {{defaultsort}}";
            this.chkDefaultSort.UseVisualStyleBackColor = true;
            // 
            // dtpFrom
            // 
            this.dtpFrom.Enabled = false;
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(61, 48);
            this.dtpFrom.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(88, 20);
            this.dtpFrom.TabIndex = 2;
            this.dtpFrom.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            // 
            // gbDate
            // 
            this.gbDate.Controls.Add(this.chkSearchDates);
            this.gbDate.Controls.Add(this.lblDate);
            this.gbDate.Controls.Add(this.dtpFrom);
            this.gbDate.Controls.Add(this.lblEndDate);
            this.gbDate.Controls.Add(this.dtpTo);
            this.gbDate.Location = new System.Drawing.Point(6, 6);
            this.gbDate.Name = "gbDate";
            this.gbDate.Size = new System.Drawing.Size(286, 116);
            this.gbDate.TabIndex = 1;
            this.gbDate.TabStop = false;
            this.gbDate.Text = "Last edited date";
            // 
            // chkSearchDates
            // 
            this.chkSearchDates.AutoSize = true;
            this.chkSearchDates.Location = new System.Drawing.Point(9, 19);
            this.chkSearchDates.Name = "chkSearchDates";
            this.chkSearchDates.Size = new System.Drawing.Size(86, 17);
            this.chkSearchDates.TabIndex = 4;
            this.chkSearchDates.Text = "Search Date";
            this.chkSearchDates.CheckedChanged += new System.EventHandler(this.chkSearchDates_CheckedChanged);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(6, 52);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(49, 13);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Between";
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(39, 78);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(16, 13);
            this.lblEndDate.TabIndex = 3;
            this.lblEndDate.Text = "to";
            // 
            // dtpTo
            // 
            this.dtpTo.Checked = false;
            this.dtpTo.Enabled = false;
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(61, 74);
            this.dtpTo.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(88, 20);
            this.dtpTo.TabIndex = 0;
            // 
            // tbParameters
            // 
            this.tbParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbParameters.Controls.Add(this.tabDump);
            this.tbParameters.Controls.Add(this.tabProps);
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
            this.lblCase.Text = "Case:";
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
            this.lblGenerator.Text = "Generator:";
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
            this.lblBase.Location = new System.Drawing.Point(6, 61);
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
            this.lblSitename.Text = "Site name:";
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
            this.lblDBDump.Location = new System.Drawing.Point(3, 9);
            this.lblDBDump.Name = "lblDBDump";
            this.lblDBDump.Size = new System.Drawing.Size(85, 13);
            this.lblDBDump.TabIndex = 0;
            this.lblDBDump.Text = "Datebase dump:";
            // 
            // tabProps
            // 
            this.tabProps.Controls.Add(this.gbNamespace);
            this.tabProps.Controls.Add(this.gbTitle);
            this.tabProps.Location = new System.Drawing.Point(4, 22);
            this.tabProps.Name = "tabProps";
            this.tabProps.Padding = new System.Windows.Forms.Padding(3);
            this.tabProps.Size = new System.Drawing.Size(523, 165);
            this.tabProps.TabIndex = 1;
            this.tabProps.Text = "Title";
            this.tabProps.UseVisualStyleBackColor = true;
            // 
            // gbNamespace
            // 
            this.gbNamespace.Controls.Add(this.flwNamespace);
            this.gbNamespace.Location = new System.Drawing.Point(6, 6);
            this.gbNamespace.Name = "gbNamespace";
            this.gbNamespace.Size = new System.Drawing.Size(89, 141);
            this.gbNamespace.TabIndex = 3;
            this.gbNamespace.TabStop = false;
            this.gbNamespace.Text = "Namespaces";
            // 
            // flwNamespace
            // 
            this.flwNamespace.Controls.Add(this.chkMainNamespace);
            this.flwNamespace.Controls.Add(this.chkTemplateNamespace);
            this.flwNamespace.Controls.Add(this.chkCategoryNamespace);
            this.flwNamespace.Controls.Add(this.chkImageNamespace);
            this.flwNamespace.Controls.Add(this.chkProjectNamespace);
            this.flwNamespace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwNamespace.Location = new System.Drawing.Point(3, 16);
            this.flwNamespace.Name = "flwNamespace";
            this.flwNamespace.Size = new System.Drawing.Size(83, 122);
            this.flwNamespace.TabIndex = 4;
            // 
            // chkMainNamespace
            // 
            this.chkMainNamespace.AutoSize = true;
            this.chkMainNamespace.Checked = true;
            this.chkMainNamespace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMainNamespace.Location = new System.Drawing.Point(3, 3);
            this.chkMainNamespace.Name = "chkMainNamespace";
            this.chkMainNamespace.Size = new System.Drawing.Size(54, 17);
            this.chkMainNamespace.TabIndex = 0;
            this.chkMainNamespace.Text = "(m&ain)";
            // 
            // chkTemplateNamespace
            // 
            this.chkTemplateNamespace.AutoSize = true;
            this.chkTemplateNamespace.Location = new System.Drawing.Point(3, 26);
            this.chkTemplateNamespace.Name = "chkTemplateNamespace";
            this.chkTemplateNamespace.Size = new System.Drawing.Size(70, 17);
            this.chkTemplateNamespace.TabIndex = 1;
            this.chkTemplateNamespace.Text = "&Template";
            // 
            // chkCategoryNamespace
            // 
            this.chkCategoryNamespace.AutoSize = true;
            this.chkCategoryNamespace.Location = new System.Drawing.Point(3, 49);
            this.chkCategoryNamespace.Name = "chkCategoryNamespace";
            this.chkCategoryNamespace.Size = new System.Drawing.Size(68, 17);
            this.chkCategoryNamespace.TabIndex = 3;
            this.chkCategoryNamespace.Text = "&Category";
            // 
            // chkImageNamespace
            // 
            this.chkImageNamespace.AutoSize = true;
            this.chkImageNamespace.Location = new System.Drawing.Point(3, 72);
            this.chkImageNamespace.Name = "chkImageNamespace";
            this.chkImageNamespace.Size = new System.Drawing.Size(55, 17);
            this.chkImageNamespace.TabIndex = 2;
            this.chkImageNamespace.Text = "&Image";
            // 
            // tabRev
            // 
            this.tabRev.Controls.Add(this.gbDate);
            this.tabRev.Location = new System.Drawing.Point(4, 22);
            this.tabRev.Name = "tabRev";
            this.tabRev.Padding = new System.Windows.Forms.Padding(3);
            this.tabRev.Size = new System.Drawing.Size(523, 165);
            this.tabRev.TabIndex = 4;
            this.tabRev.Text = "Revision";
            this.tabRev.UseVisualStyleBackColor = true;
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
            this.tabAWB.Controls.Add(this.lblLimitResutls);
            this.tabAWB.Controls.Add(this.nudLimitResults);
            this.tabAWB.Controls.Add(this.lblStartFrom);
            this.tabAWB.Controls.Add(this.txtStartFrom);
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
            this.tabRestrict.Controls.Add(this.grpEditMove);
            this.tabRestrict.Location = new System.Drawing.Point(4, 22);
            this.tabRestrict.Name = "tabRestrict";
            this.tabRestrict.Padding = new System.Windows.Forms.Padding(3);
            this.tabRestrict.Size = new System.Drawing.Size(523, 165);
            this.tabRestrict.TabIndex = 5;
            this.tabRestrict.Text = "Restriction";
            this.tabRestrict.UseVisualStyleBackColor = true;
            // 
            // grpEditMove
            // 
            this.grpEditMove.Controls.Add(this.chkProtection);
            this.grpEditMove.Controls.Add(this.MoveDelete);
            this.grpEditMove.Enabled = false;
            this.grpEditMove.Location = new System.Drawing.Point(6, 6);
            this.grpEditMove.Name = "grpEditMove";
            this.grpEditMove.Size = new System.Drawing.Size(330, 127);
            this.grpEditMove.TabIndex = 2;
            this.grpEditMove.TabStop = false;
            // 
            // chkProtection
            // 
            this.chkProtection.AutoSize = true;
            this.chkProtection.Location = new System.Drawing.Point(6, 16);
            this.chkProtection.Name = "chkProtection";
            this.chkProtection.Size = new System.Drawing.Size(108, 17);
            this.chkProtection.TabIndex = 1;
            this.chkProtection.Text = "Check Protection";
            this.chkProtection.UseVisualStyleBackColor = true;
            this.chkProtection.CheckedChanged += new System.EventHandler(this.chkProtection_CheckedChanged);
            // 
            // MoveDelete
            // 
            this.MoveDelete.Enabled = false;
            this.MoveDelete.Location = new System.Drawing.Point(6, 39);
            this.MoveDelete.Name = "MoveDelete";
            this.MoveDelete.Size = new System.Drawing.Size(311, 88);
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
            this.tabControl1.Controls.Add(this.tbConvert);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 307);
            this.tabControl1.TabIndex = 5;
            // 
            // tbConvert
            // 
            this.tbConvert.Controls.Add(this.groupBox4);
            this.tbConvert.Location = new System.Drawing.Point(4, 22);
            this.tbConvert.Name = "tbConvert";
            this.tbConvert.Padding = new System.Windows.Forms.Padding(3);
            this.tbConvert.Size = new System.Drawing.Size(282, 281);
            this.tbConvert.TabIndex = 1;
            this.tbConvert.Text = "Convert";
            this.tbConvert.UseVisualStyleBackColor = true;
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
            this.saveListDialog.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt|CSV (Comma Seperated Values" +
                ")|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // DatabaseScanner
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 549);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.tbParameters);
            this.Controls.Add(this.splitContainer1);
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
            this.gbTitle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).EndInit();
            this.ArticlesListBoxContextMenu.ResumeLayout(false);
            this.gbOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gbAWBSpecific.ResumeLayout(false);
            this.flwAWB.ResumeLayout(false);
            this.flwAWB.PerformLayout();
            this.gbDate.ResumeLayout(false);
            this.gbDate.PerformLayout();
            this.tbParameters.ResumeLayout(false);
            this.tabDump.ResumeLayout(false);
            this.tabDump.PerformLayout();
            this.flwHelpLinks.ResumeLayout(false);
            this.flwHelpLinks.PerformLayout();
            this.tabProps.ResumeLayout(false);
            this.gbNamespace.ResumeLayout(false);
            this.flwNamespace.ResumeLayout(false);
            this.flwNamespace.PerformLayout();
            this.tabRev.ResumeLayout(false);
            this.tabText.ResumeLayout(false);
            this.gbProperties.ResumeLayout(false);
            this.gbProperties.PerformLayout();
            this.tabAWB.ResumeLayout(false);
            this.tabAWB.PerformLayout();
            this.tabRestrict.ResumeLayout(false);
            this.grpEditMove.ResumeLayout(false);
            this.grpEditMove.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tbConvert.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox gbTitle;
        private Button btnClear;
        private Button btnCopy;
        private ToolTip tooltip;
        private CheckBox chkHeading;
        private NumericUpDown nudHeadingSpace;
        private TextBox txtTitleContains;
        private CheckBox chkTitleContains;
        private Button btnTransfer;
        private Button btnClearList;
        private ContextMenuStrip ArticlesListBoxContextMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private GroupBox gbOutput;
        private GroupBox groupBox4;
        private RadioButton rdoBullet;
        private RadioButton rdoHash;
        private ToolStripProgressBar progressBar;
        private Timer timerProgessUpdate;
        private ToolStripStatusLabel lblCount;
        private ToolStripMenuItem removeToolStripMenuItem;
        private TextBox txtTitleNotContains;
        private CheckBox chkTitleDoesNotContain;
        private CheckBox chkTitleRegex;
        private CheckBox chkTitleCase;
        private GroupBox gbAWBSpecific;
        private CheckBox chkCaseSensitive;
        private CheckBox chkRegex;
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
        private CheckBox chkTypo;
        private CheckBox chkABCHeader;
        private Label lblStartFrom;
        private TextBox txtStartFrom;
        private Label lblListType;
        private Button btnStart;
        private NumericUpDown nudLimitResults;
        private Label lblLimitResutls;
        private DateTimePicker dtpFrom;
        private GroupBox gbDate;
        private TabControl tbParameters;
        private TabPage tabDump;
        private TabPage tabProps;
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
        private CheckBox chkImageNamespace;
        private CheckBox chkCategoryNamespace;
        private CheckBox chkMainNamespace;
        private CheckBox chkProjectNamespace;
        private CheckBox chkTemplateNamespace;
        private GroupBox grpEditMove;
        private LinkLabel lnkBase;
        private ToolStripMenuItem openRevisionInBowserToolStripMenuItem;
        private GroupBox gbNamespace;
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
        private DateTimePicker dtpTo;
        private Label lblEndDate;
        private Label lblDate;
        private CheckBox chkDefaultSort;
        private TabPage tabRev;
        private TabPage tabRestrict;
        private Label lblAlso;
        private CheckBox chkSearchDates;
        private TextBox txtCase;
        private TabControl tabControl1;
        private TabPage tbConvert;
        private FlowLayoutPanel flwAWB;
        private FlowLayoutPanel flwNamespace;
        private TableLayoutPanel tableLayoutPanel2;
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
    }
}