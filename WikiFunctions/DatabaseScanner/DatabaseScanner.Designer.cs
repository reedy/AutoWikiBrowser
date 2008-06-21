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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkTitleCase = new System.Windows.Forms.CheckBox();
            this.chkTitleRegex = new System.Windows.Forms.CheckBox();
            this.nudHeadingSpace = new System.Windows.Forms.NumericUpDown();
            this.chkHeading = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnAlphaList = new System.Windows.Forms.Button();
            this.btnClearList = new System.Windows.Forms.Button();
            this.rdoNoBold = new System.Windows.Forms.RadioButton();
            this.rdoSimpleLinks = new System.Windows.Forms.RadioButton();
            this.rdoBadLinks = new System.Windows.Forms.RadioButton();
            this.rdoHasHTML = new System.Windows.Forms.RadioButton();
            this.rdoHeaderError = new System.Windows.Forms.RadioButton();
            this.rdoUnbulletedLinks = new System.Windows.Forms.RadioButton();
            this.chkABCHeader = new System.Windows.Forms.CheckBox();
            this.txtStartFrom = new System.Windows.Forms.TextBox();
            this.ArticlesListBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRevisionInBowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblStartFrom = new System.Windows.Forms.Label();
            this.lblLimitResutls = new System.Windows.Forms.Label();
            this.nudLimitResults = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblListType = new System.Windows.Forms.Label();
            this.rdoBullet = new System.Windows.Forms.RadioButton();
            this.rdoHash = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.threadPriorityButton = new System.Windows.Forms.ToolStripSplitButton();
            this.highestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboveNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belowNormalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lowestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerProgessUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdoTypo = new System.Windows.Forms.RadioButton();
            this.rdoNone = new System.Windows.Forms.RadioButton();
            this.beginDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabDump = new System.Windows.Forms.TabPage();
            this.lnkGenDump = new System.Windows.Forms.LinkLabel();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.lnkWikiaDumps = new System.Windows.Forms.LinkLabel();
            this.lnkWmfDumps = new System.Windows.Forms.LinkLabel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtDumpLocation = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblBase = new System.Windows.Forms.Label();
            this.lblGenerator = new System.Windows.Forms.Label();
            this.lblCase = new System.Windows.Forms.Label();
            this.txtSitename = new System.Windows.Forms.TextBox();
            this.lnkBase = new System.Windows.Forms.LinkLabel();
            this.txtGenerator = new System.Windows.Forms.TextBox();
            this.txtCase = new System.Windows.Forms.TextBox();
            this.lblSitename = new System.Windows.Forms.Label();
            this.lblDBDump = new System.Windows.Forms.Label();
            this.tabProps = new System.Windows.Forms.TabPage();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.chkProjectNamespace = new System.Windows.Forms.CheckBox();
            this.chkCategoryNamespace = new System.Windows.Forms.CheckBox();
            this.chkImageNamespace = new System.Windows.Forms.CheckBox();
            this.chkTemplateNamespace = new System.Windows.Forms.CheckBox();
            this.chkMainNamespace = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.tabText = new System.Windows.Forms.TabPage();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.chkIgnoreRedirects = new System.Windows.Forms.CheckBox();
            this.tabAWB = new System.Windows.Forms.TabPage();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.lbArticles = new WikiFunctions.Controls.Lists.ListBox2();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).BeginInit();
            this.ArticlesListBoxContextMenu.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabDump.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabProps.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tabText.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.tabAWB.SuspendLayout();
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
            this.txtList.Size = new System.Drawing.Size(274, 234);
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkArticleDoesContain);
            this.groupBox1.Controls.Add(this.txtArticleDoesContain);
            this.groupBox1.Controls.Add(this.chkArticleDoesNotContain);
            this.groupBox1.Controls.Add(this.txtArticleDoesNotContain);
            this.groupBox1.Controls.Add(this.chkRegex);
            this.groupBox1.Controls.Add(this.chkCaseSensitive);
            this.groupBox1.Controls.Add(this.chkSingle);
            this.groupBox1.Controls.Add(this.chkMulti);
            this.groupBox1.Controls.Add(this.chkIgnoreComments);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 149);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Te&xt Searching";
            // 
            // chkArticleDoesContain
            // 
            this.chkArticleDoesContain.AutoSize = true;
            this.chkArticleDoesContain.Location = new System.Drawing.Point(6, 19);
            this.chkArticleDoesContain.Name = "chkArticleDoesContain";
            this.chkArticleDoesContain.Size = new System.Drawing.Size(97, 17);
            this.chkArticleDoesContain.TabIndex = 0;
            this.chkArticleDoesContain.Text = "Page &contains:";
            this.chkArticleDoesContain.CheckedChanged += new System.EventHandler(this.chkDoesContain_CheckedChanged);
            // 
            // txtArticleDoesContain
            // 
            this.txtArticleDoesContain.Enabled = false;
            this.txtArticleDoesContain.Location = new System.Drawing.Point(113, 17);
            this.txtArticleDoesContain.Name = "txtArticleDoesContain";
            this.txtArticleDoesContain.Size = new System.Drawing.Size(185, 20);
            this.txtArticleDoesContain.TabIndex = 1;
            // 
            // chkArticleDoesNotContain
            // 
            this.chkArticleDoesNotContain.AutoSize = true;
            this.chkArticleDoesNotContain.Location = new System.Drawing.Point(6, 45);
            this.chkArticleDoesNotContain.Name = "chkArticleDoesNotContain";
            this.chkArticleDoesNotContain.Size = new System.Drawing.Size(89, 17);
            this.chkArticleDoesNotContain.TabIndex = 2;
            this.chkArticleDoesNotContain.Text = "&Not contains:";
            this.chkArticleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkDoesNotContain_CheckedChanged);
            // 
            // txtArticleDoesNotContain
            // 
            this.txtArticleDoesNotContain.Enabled = false;
            this.txtArticleDoesNotContain.Location = new System.Drawing.Point(113, 43);
            this.txtArticleDoesNotContain.Name = "txtArticleDoesNotContain";
            this.txtArticleDoesNotContain.Size = new System.Drawing.Size(185, 20);
            this.txtArticleDoesNotContain.TabIndex = 3;
            // 
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(113, 69);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(57, 17);
            this.chkRegex.TabIndex = 4;
            this.chkRegex.Text = "&Regex";
            this.chkRegex.CheckedChanged += new System.EventHandler(this.chkRegex_CheckedChanged);
            // 
            // chkCaseSensitive
            // 
            this.chkCaseSensitive.AutoSize = true;
            this.chkCaseSensitive.Location = new System.Drawing.Point(113, 89);
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
            this.chkSingle.Location = new System.Drawing.Point(227, 69);
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
            this.chkMulti.Location = new System.Drawing.Point(227, 89);
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
            this.chkIgnoreComments.Location = new System.Drawing.Point(113, 108);
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
            10,
            0,
            0,
            0});
            this.nudLength.Location = new System.Drawing.Point(175, 18);
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
            this.nudLength.Size = new System.Drawing.Size(73, 20);
            this.nudLength.TabIndex = 10;
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
            "at most",
            "at least"});
            this.cmboLength.Location = new System.Drawing.Point(69, 17);
            this.cmboLength.Name = "cmboLength";
            this.cmboLength.Size = new System.Drawing.Size(104, 21);
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
            "at most",
            "at least"});
            this.cmboLinks.Location = new System.Drawing.Point(69, 43);
            this.cmboLinks.Name = "cmboLinks";
            this.cmboLinks.Size = new System.Drawing.Size(104, 21);
            this.cmboLinks.TabIndex = 12;
            this.cmboLinks.SelectedIndexChanged += new System.EventHandler(this.cmboLinks_SelectedIndexChanged);
            // 
            // nudLinks
            // 
            this.nudLinks.Enabled = false;
            this.nudLinks.Location = new System.Drawing.Point(176, 44);
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
            this.nudLinks.Size = new System.Drawing.Size(72, 20);
            this.nudLinks.TabIndex = 13;
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
            "at most",
            "at least"});
            this.cmboWords.Location = new System.Drawing.Point(69, 70);
            this.cmboWords.Name = "cmboWords";
            this.cmboWords.Size = new System.Drawing.Size(104, 21);
            this.cmboWords.TabIndex = 15;
            this.cmboWords.SelectedIndexChanged += new System.EventHandler(this.cmboWords_SelectedIndexChanged);
            // 
            // nudWords
            // 
            this.nudWords.Enabled = false;
            this.nudWords.Location = new System.Drawing.Point(175, 70);
            this.nudWords.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudWords.Name = "nudWords";
            this.nudWords.Size = new System.Drawing.Size(73, 20);
            this.nudWords.TabIndex = 16;
            this.nudWords.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // txtTitleNotContains
            // 
            this.txtTitleNotContains.Enabled = false;
            this.txtTitleNotContains.Location = new System.Drawing.Point(105, 43);
            this.txtTitleNotContains.Name = "txtTitleNotContains";
            this.txtTitleNotContains.Size = new System.Drawing.Size(175, 20);
            this.txtTitleNotContains.TabIndex = 3;
            this.txtTitleNotContains.Leave += new System.EventHandler(this.txtTitleNotContains_Leave);
            // 
            // chkTitleDoesNotContain
            // 
            this.chkTitleDoesNotContain.AutoSize = true;
            this.chkTitleDoesNotContain.Location = new System.Drawing.Point(6, 45);
            this.chkTitleDoesNotContain.Name = "chkTitleDoesNotContain";
            this.chkTitleDoesNotContain.Size = new System.Drawing.Size(89, 17);
            this.chkTitleDoesNotContain.TabIndex = 2;
            this.chkTitleDoesNotContain.Text = "Not contains:";
            this.chkTitleDoesNotContain.CheckedChanged += new System.EventHandler(this.chkCheckNotInTitle_CheckedChanged);
            // 
            // txtTitleContains
            // 
            this.txtTitleContains.Enabled = false;
            this.txtTitleContains.Location = new System.Drawing.Point(104, 17);
            this.txtTitleContains.Name = "txtTitleContains";
            this.txtTitleContains.Size = new System.Drawing.Size(176, 20);
            this.txtTitleContains.TabIndex = 1;
            this.txtTitleContains.Leave += new System.EventHandler(this.txtTitleContains_Leave);
            // 
            // chkTitleContains
            // 
            this.chkTitleContains.AutoSize = true;
            this.chkTitleContains.Location = new System.Drawing.Point(6, 19);
            this.chkTitleContains.Name = "chkTitleContains";
            this.chkTitleContains.Size = new System.Drawing.Size(92, 17);
            this.chkTitleContains.TabIndex = 0;
            this.chkTitleContains.Text = "Title contains:";
            this.chkTitleContains.CheckedChanged += new System.EventHandler(this.chkCheckTitle_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkTitleCase);
            this.groupBox2.Controls.Add(this.chkTitleRegex);
            this.groupBox2.Controls.Add(this.txtTitleNotContains);
            this.groupBox2.Controls.Add(this.chkTitleDoesNotContain);
            this.groupBox2.Controls.Add(this.txtTitleContains);
            this.groupBox2.Controls.Add(this.chkTitleContains);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(286, 93);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&Title matching";
            // 
            // chkTitleCase
            // 
            this.chkTitleCase.AutoSize = true;
            this.chkTitleCase.Checked = true;
            this.chkTitleCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTitleCase.Location = new System.Drawing.Point(186, 69);
            this.chkTitleCase.Name = "chkTitleCase";
            this.chkTitleCase.Size = new System.Drawing.Size(94, 17);
            this.chkTitleCase.TabIndex = 5;
            this.chkTitleCase.Text = "Case sensitive";
            // 
            // chkTitleRegex
            // 
            this.chkTitleRegex.AutoSize = true;
            this.chkTitleRegex.Location = new System.Drawing.Point(105, 69);
            this.chkTitleRegex.Name = "chkTitleRegex";
            this.chkTitleRegex.Size = new System.Drawing.Size(57, 17);
            this.chkTitleRegex.TabIndex = 4;
            this.chkTitleRegex.Text = "Regex";
            // 
            // nudHeadingSpace
            // 
            this.nudHeadingSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudHeadingSpace.Enabled = false;
            this.nudHeadingSpace.Location = new System.Drawing.Point(127, 18);
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
            this.chkHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkHeading.AutoSize = true;
            this.chkHeading.Location = new System.Drawing.Point(6, 19);
            this.chkHeading.Name = "chkHeading";
            this.chkHeading.Size = new System.Drawing.Size(115, 17);
            this.chkHeading.TabIndex = 0;
            this.chkHeading.Text = "Add heading every";
            this.tooltip.SetToolTip(this.chkHeading, "Add numbered heading");
            this.chkHeading.CheckedChanged += new System.EventHandler(this.chkHeading_CheckedChanged);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(235, 308);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(45, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.tooltip.SetToolTip(this.btnClear, "Clear");
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(6, 308);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 8;
            this.btnCopy.Text = "Copy";
            this.tooltip.SetToolTip(this.btnCopy, "Copy to clipboard");
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Location = new System.Drawing.Point(179, 15);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(100, 23);
            this.btnTransfer.TabIndex = 6;
            this.btnTransfer.Text = "Make";
            this.tooltip.SetToolTip(this.btnTransfer, "Turn list into wiki formatted text, which can be saved or copied.");
            this.btnTransfer.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAlphaList
            // 
            this.btnAlphaList.Location = new System.Drawing.Point(65, 308);
            this.btnAlphaList.Name = "btnAlphaList";
            this.btnAlphaList.Size = new System.Drawing.Size(47, 23);
            this.btnAlphaList.TabIndex = 7;
            this.btnAlphaList.Text = "Sort";
            this.tooltip.SetToolTip(this.btnAlphaList, "Alphabetize the list");
            this.btnAlphaList.Click += new System.EventHandler(this.AlphaList_Click);
            // 
            // btnClearList
            // 
            this.btnClearList.Location = new System.Drawing.Point(222, 308);
            this.btnClearList.Name = "btnClearList";
            this.btnClearList.Size = new System.Drawing.Size(75, 23);
            this.btnClearList.TabIndex = 8;
            this.btnClearList.Text = "Clear";
            this.tooltip.SetToolTip(this.btnClearList, "Clear the list");
            this.btnClearList.Click += new System.EventHandler(this.lbClear_Click);
            // 
            // rdoNoBold
            // 
            this.rdoNoBold.AutoSize = true;
            this.rdoNoBold.Location = new System.Drawing.Point(6, 30);
            this.rdoNoBold.Name = "rdoNoBold";
            this.rdoNoBold.Size = new System.Drawing.Size(157, 17);
            this.rdoNoBold.TabIndex = 0;
            this.rdoNoBold.Text = "Has title AWB will embolden";
            this.tooltip.SetToolTip(this.rdoNoBold, "AWB \'\'\'emboldens\'\'\' the title when appropriate");
            // 
            // rdoSimpleLinks
            // 
            this.rdoSimpleLinks.AutoSize = true;
            this.rdoSimpleLinks.Location = new System.Drawing.Point(6, 47);
            this.rdoSimpleLinks.Name = "rdoSimpleLinks";
            this.rdoSimpleLinks.Size = new System.Drawing.Size(149, 17);
            this.rdoSimpleLinks.TabIndex = 1;
            this.rdoSimpleLinks.Text = "Has links AWB will simplify";
            this.tooltip.SetToolTip(this.rdoSimpleLinks, "AWB simplifies some links");
            // 
            // rdoBadLinks
            // 
            this.rdoBadLinks.AutoSize = true;
            this.rdoBadLinks.Location = new System.Drawing.Point(6, 64);
            this.rdoBadLinks.Name = "rdoBadLinks";
            this.rdoBadLinks.Size = new System.Drawing.Size(147, 17);
            this.rdoBadLinks.TabIndex = 2;
            this.rdoBadLinks.Text = "Has bad links AWB will fix";
            this.tooltip.SetToolTip(this.rdoBadLinks, "AWB fixes bad links, such as wrong syntax and URL coding");
            // 
            // rdoHasHTML
            // 
            this.rdoHasHTML.AutoSize = true;
            this.rdoHasHTML.Location = new System.Drawing.Point(166, 13);
            this.rdoHasHTML.Name = "rdoHasHTML";
            this.rdoHasHTML.Size = new System.Drawing.Size(113, 17);
            this.rdoHasHTML.TabIndex = 3;
            this.rdoHasHTML.Text = "Has HTML entities";
            this.tooltip.SetToolTip(this.rdoHasHTML, "AWB replaces HTML entities with unicode");
            // 
            // rdoHeaderError
            // 
            this.rdoHeaderError.AutoSize = true;
            this.rdoHeaderError.Location = new System.Drawing.Point(166, 30);
            this.rdoHeaderError.Name = "rdoHeaderError";
            this.rdoHeaderError.Size = new System.Drawing.Size(85, 17);
            this.rdoHeaderError.TabIndex = 4;
            this.rdoHeaderError.Text = "Section error";
            this.tooltip.SetToolTip(this.rdoHeaderError, "AWB fixes common mis-capitalisation in headings");
            // 
            // rdoUnbulletedLinks
            // 
            this.rdoUnbulletedLinks.AutoSize = true;
            this.rdoUnbulletedLinks.Location = new System.Drawing.Point(166, 47);
            this.rdoUnbulletedLinks.Name = "rdoUnbulletedLinks";
            this.rdoUnbulletedLinks.Size = new System.Drawing.Size(100, 17);
            this.rdoUnbulletedLinks.TabIndex = 5;
            this.rdoUnbulletedLinks.Text = "Unbulleted links";
            this.tooltip.SetToolTip(this.rdoUnbulletedLinks, "AWB bullets links in external links sections");
            // 
            // chkABCHeader
            // 
            this.chkABCHeader.AutoSize = true;
            this.chkABCHeader.Location = new System.Drawing.Point(6, 44);
            this.chkABCHeader.Name = "chkABCHeader";
            this.chkABCHeader.Size = new System.Drawing.Size(133, 17);
            this.chkABCHeader.TabIndex = 2;
            this.chkABCHeader.Text = "Alpha&betised headings";
            this.tooltip.SetToolTip(this.chkABCHeader, "Add alphabetised heading, list must be alphabetised first");
            this.chkABCHeader.CheckedChanged += new System.EventHandler(this.chkABCHeader_CheckedChanged);
            // 
            // txtStartFrom
            // 
            this.txtStartFrom.Location = new System.Drawing.Point(105, 17);
            this.txtStartFrom.Name = "txtStartFrom";
            this.txtStartFrom.Size = new System.Drawing.Size(192, 20);
            this.txtStartFrom.TabIndex = 3;
            this.tooltip.SetToolTip(this.txtStartFrom, "Page to start scanning from, leave blank to start at beginning");
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
            this.ArticlesListBoxContextMenu.Size = new System.Drawing.Size(202, 120);
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
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(198, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblStartFrom);
            this.groupBox3.Controls.Add(this.txtStartFrom);
            this.groupBox3.Controls.Add(this.lblLimitResutls);
            this.groupBox3.Controls.Add(this.nudLimitResults);
            this.groupBox3.Controls.Add(this.btnStart);
            this.groupBox3.Controls.Add(this.lbArticles);
            this.groupBox3.Controls.Add(this.btnFilter);
            this.groupBox3.Controls.Add(this.btnClearList);
            this.groupBox3.Controls.Add(this.btnAlphaList);
            this.groupBox3.Location = new System.Drawing.Point(13, 226);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(303, 337);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Generate results";
            // 
            // lblStartFrom
            // 
            this.lblStartFrom.AutoSize = true;
            this.lblStartFrom.Location = new System.Drawing.Point(13, 20);
            this.lblStartFrom.Name = "lblStartFrom";
            this.lblStartFrom.Size = new System.Drawing.Size(86, 13);
            this.lblStartFrom.TabIndex = 2;
            this.lblStartFrom.Text = "Start from &article:";
            // 
            // lblLimitResutls
            // 
            this.lblLimitResutls.AutoSize = true;
            this.lblLimitResutls.Location = new System.Drawing.Point(26, 45);
            this.lblLimitResutls.Name = "lblLimitResutls";
            this.lblLimitResutls.Size = new System.Drawing.Size(73, 13);
            this.lblLimitResutls.TabIndex = 0;
            this.lblLimitResutls.Text = "Limit results to";
            // 
            // nudLimitResults
            // 
            this.nudLimitResults.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudLimitResults.Location = new System.Drawing.Point(105, 43);
            this.nudLimitResults.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.nudLimitResults.Name = "nudLimitResults";
            this.nudLimitResults.Size = new System.Drawing.Size(84, 20);
            this.nudLimitResults.TabIndex = 1;
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
            this.btnStart.Location = new System.Drawing.Point(197, 43);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(6, 308);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(53, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "Filter";
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblListType);
            this.groupBox4.Controls.Add(this.chkABCHeader);
            this.groupBox4.Controls.Add(this.rdoBullet);
            this.groupBox4.Controls.Add(this.rdoHash);
            this.groupBox4.Controls.Add(this.btnSave);
            this.groupBox4.Controls.Add(this.txtList);
            this.groupBox4.Controls.Add(this.chkHeading);
            this.groupBox4.Controls.Add(this.nudHeadingSpace);
            this.groupBox4.Controls.Add(this.btnClear);
            this.groupBox4.Controls.Add(this.btnTransfer);
            this.groupBox4.Controls.Add(this.btnCopy);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(322, 226);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(286, 337);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Convert results into &list";
            // 
            // lblListType
            // 
            this.lblListType.AutoSize = true;
            this.lblListType.Location = new System.Drawing.Point(160, 45);
            this.lblListType.Name = "lblListType";
            this.lblListType.Size = new System.Drawing.Size(46, 13);
            this.lblListType.TabIndex = 3;
            this.lblListType.Text = "List type";
            // 
            // rdoBullet
            // 
            this.rdoBullet.AutoSize = true;
            this.rdoBullet.Location = new System.Drawing.Point(250, 43);
            this.rdoBullet.Name = "rdoBullet";
            this.rdoBullet.Size = new System.Drawing.Size(29, 17);
            this.rdoBullet.TabIndex = 5;
            this.rdoBullet.Text = "*";
            // 
            // rdoHash
            // 
            this.rdoHash.AutoSize = true;
            this.rdoHash.Checked = true;
            this.rdoHash.Location = new System.Drawing.Point(212, 43);
            this.rdoHash.Name = "rdoHash";
            this.rdoHash.Size = new System.Drawing.Size(32, 17);
            this.rdoHash.TabIndex = 4;
            this.rdoHash.TabStop = true;
            this.rdoHash.Text = "#";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(182, 308);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(47, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.lblCount,
            this.threadPriorityButton});
            this.statusStrip.Location = new System.Drawing.Point(0, 566);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(614, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "status";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblCount
            // 
            this.lblCount.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.lblCount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(349, 17);
            this.lblCount.Spring = true;
            // 
            // threadPriorityButton
            // 
            this.threadPriorityButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.threadPriorityButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highestToolStripMenuItem,
            this.aboveNormalToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.belowNormalToolStripMenuItem,
            this.lowestToolStripMenuItem});
            this.threadPriorityButton.Name = "threadPriorityButton";
            this.threadPriorityButton.Size = new System.Drawing.Size(98, 20);
            this.threadPriorityButton.Text = "BellowNormal";
            // 
            // highestToolStripMenuItem
            // 
            this.highestToolStripMenuItem.Name = "highestToolStripMenuItem";
            this.highestToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.highestToolStripMenuItem.Text = "&High";
            this.highestToolStripMenuItem.Click += new System.EventHandler(this.highestToolStripMenuItem_Click);
            // 
            // aboveNormalToolStripMenuItem
            // 
            this.aboveNormalToolStripMenuItem.Name = "aboveNormalToolStripMenuItem";
            this.aboveNormalToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.aboveNormalToolStripMenuItem.Text = "&AboveNormal";
            this.aboveNormalToolStripMenuItem.Click += new System.EventHandler(this.aboveNormalToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.normalToolStripMenuItem.Text = "&Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // belowNormalToolStripMenuItem
            // 
            this.belowNormalToolStripMenuItem.Checked = true;
            this.belowNormalToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.belowNormalToolStripMenuItem.Name = "belowNormalToolStripMenuItem";
            this.belowNormalToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.belowNormalToolStripMenuItem.Text = "&BelowNormal";
            this.belowNormalToolStripMenuItem.Click += new System.EventHandler(this.belowNormalToolStripMenuItem_Click);
            // 
            // lowestToolStripMenuItem
            // 
            this.lowestToolStripMenuItem.Name = "lowestToolStripMenuItem";
            this.lowestToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.lowestToolStripMenuItem.Text = "&Low";
            this.lowestToolStripMenuItem.Click += new System.EventHandler(this.lowestToolStripMenuItem_Click);
            // 
            // timerProgessUpdate
            // 
            this.timerProgessUpdate.Interval = 500;
            this.timerProgessUpdate.Tick += new System.EventHandler(this.timerProgessUpdate_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdoTypo);
            this.groupBox5.Controls.Add(this.rdoUnbulletedLinks);
            this.groupBox5.Controls.Add(this.rdoHeaderError);
            this.groupBox5.Controls.Add(this.rdoHasHTML);
            this.groupBox5.Controls.Add(this.rdoBadLinks);
            this.groupBox5.Controls.Add(this.rdoSimpleLinks);
            this.groupBox5.Controls.Add(this.rdoNoBold);
            this.groupBox5.Controls.Add(this.rdoNone);
            this.groupBox5.Location = new System.Drawing.Point(6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(286, 84);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "A&WB specific";
            // 
            // rdoTypo
            // 
            this.rdoTypo.AutoSize = true;
            this.rdoTypo.Location = new System.Drawing.Point(166, 64);
            this.rdoTypo.Name = "rdoTypo";
            this.rdoTypo.Size = new System.Drawing.Size(49, 17);
            this.rdoTypo.TabIndex = 7;
            this.rdoTypo.TabStop = true;
            this.rdoTypo.Text = "Typo";
            // 
            // rdoNone
            // 
            this.rdoNone.AutoSize = true;
            this.rdoNone.Checked = true;
            this.rdoNone.Location = new System.Drawing.Point(6, 13);
            this.rdoNone.Name = "rdoNone";
            this.rdoNone.Size = new System.Drawing.Size(51, 17);
            this.rdoNone.TabIndex = 6;
            this.rdoNone.TabStop = true;
            this.rdoNone.Text = "None";
            // 
            // beginDate
            // 
            this.beginDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.beginDate.Location = new System.Drawing.Point(61, 19);
            this.beginDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.beginDate.Name = "beginDate";
            this.beginDate.Size = new System.Drawing.Size(88, 20);
            this.beginDate.TabIndex = 2;
            this.beginDate.Value = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblDate);
            this.groupBox6.Controls.Add(this.beginDate);
            this.groupBox6.Controls.Add(this.lblEndDate);
            this.groupBox6.Controls.Add(this.endDate);
            this.groupBox6.Location = new System.Drawing.Point(6, 105);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(286, 48);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Last Modified";
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(6, 23);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(49, 13);
            this.lblDate.TabIndex = 1;
            this.lblDate.Text = "Between";
            // 
            // lblEndDate
            // 
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new System.Drawing.Point(155, 23);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(16, 13);
            this.lblEndDate.TabIndex = 3;
            this.lblEndDate.Text = "to";
            // 
            // endDate
            // 
            this.endDate.Checked = false;
            this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.endDate.Location = new System.Drawing.Point(177, 19);
            this.endDate.MinDate = new System.DateTime(2000, 1, 1, 0, 0, 0, 0);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(88, 20);
            this.endDate.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabDump);
            this.tabControl1.Controls.Add(this.tabProps);
            this.tabControl1.Controls.Add(this.tabText);
            this.tabControl1.Controls.Add(this.tabAWB);
            this.tabControl1.Location = new System.Drawing.Point(13, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(595, 213);
            this.tabControl1.TabIndex = 0;
            // 
            // tabDump
            // 
            this.tabDump.Controls.Add(this.lnkGenDump);
            this.tabDump.Controls.Add(this.btnReset);
            this.tabDump.Controls.Add(this.btnAbout);
            this.tabDump.Controls.Add(this.lnkWikiaDumps);
            this.tabDump.Controls.Add(this.lnkWmfDumps);
            this.tabDump.Controls.Add(this.btnBrowse);
            this.tabDump.Controls.Add(this.txtDumpLocation);
            this.tabDump.Controls.Add(this.tableLayoutPanel1);
            this.tabDump.Controls.Add(this.lblDBDump);
            this.tabDump.Location = new System.Drawing.Point(4, 22);
            this.tabDump.Name = "tabDump";
            this.tabDump.Padding = new System.Windows.Forms.Padding(3);
            this.tabDump.Size = new System.Drawing.Size(587, 187);
            this.tabDump.TabIndex = 0;
            this.tabDump.Text = "Dump";
            this.tabDump.UseVisualStyleBackColor = true;
            // 
            // lnkGenDump
            // 
            this.lnkGenDump.AutoSize = true;
            this.lnkGenDump.Location = new System.Drawing.Point(439, 165);
            this.lnkGenDump.Name = "lnkGenDump";
            this.lnkGenDump.Size = new System.Drawing.Size(142, 13);
            this.lnkGenDump.TabIndex = 9;
            this.lnkGenDump.TabStop = true;
            this.lnkGenDump.Text = "Generating Database dumps";
            this.lnkGenDump.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkGenDump_LinkClicked);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(62, 161);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(54, 20);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(6, 161);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(50, 20);
            this.btnAbout.TabIndex = 5;
            this.btnAbout.Text = "About";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // lnkWikiaDumps
            // 
            this.lnkWikiaDumps.AutoSize = true;
            this.lnkWikiaDumps.Location = new System.Drawing.Point(365, 165);
            this.lnkWikiaDumps.Name = "lnkWikiaDumps";
            this.lnkWikiaDumps.Size = new System.Drawing.Size(68, 13);
            this.lnkWikiaDumps.TabIndex = 8;
            this.lnkWikiaDumps.TabStop = true;
            this.lnkWikiaDumps.Text = "Wikia dumps";
            this.lnkWikiaDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWikiaDumps_LinkClicked);
            // 
            // lnkWmfDumps
            // 
            this.lnkWmfDumps.AutoSize = true;
            this.lnkWmfDumps.Location = new System.Drawing.Point(292, 165);
            this.lnkWmfDumps.Name = "lnkWmfDumps";
            this.lnkWmfDumps.Size = new System.Drawing.Size(67, 13);
            this.lnkWmfDumps.TabIndex = 8;
            this.lnkWmfDumps.TabStop = true;
            this.lnkWmfDumps.Text = "WMF dumps";
            this.lnkWmfDumps.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWmfDumps_LinkClicked);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(506, 6);
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
            this.txtDumpLocation.Location = new System.Drawing.Point(92, 6);
            this.txtDumpLocation.Name = "txtDumpLocation";
            this.txtDumpLocation.Size = new System.Drawing.Size(408, 20);
            this.txtDumpLocation.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblBase, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblGenerator, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblCase, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lnkBase, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtGenerator, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblSitename, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtSitename, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtCase, 1, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 32);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(575, 88);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // lblBase
            // 
            this.lblBase.AutoSize = true;
            this.lblBase.Location = new System.Drawing.Point(3, 20);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(32, 13);
            this.lblBase.TabIndex = 2;
            this.lblBase.Text = "URL:";
            // 
            // lblGenerator
            // 
            this.lblGenerator.AutoSize = true;
            this.lblGenerator.Location = new System.Drawing.Point(3, 40);
            this.lblGenerator.Name = "lblGenerator";
            this.lblGenerator.Size = new System.Drawing.Size(57, 13);
            this.lblGenerator.TabIndex = 4;
            this.lblGenerator.Text = "Generator:";
            // 
            // lblCase
            // 
            this.lblCase.AutoSize = true;
            this.lblCase.Location = new System.Drawing.Point(3, 60);
            this.lblCase.Name = "lblCase";
            this.lblCase.Size = new System.Drawing.Size(57, 13);
            this.lblCase.TabIndex = 6;
            this.lblCase.Text = "Case type:";
            // 
            // txtSitename
            // 
            this.txtSitename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSitename.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSitename.Location = new System.Drawing.Point(83, 3);
            this.txtSitename.Name = "txtSitename";
            this.txtSitename.ReadOnly = true;
            this.txtSitename.Size = new System.Drawing.Size(489, 13);
            this.txtSitename.TabIndex = 7;
            // 
            // lnkBase
            // 
            this.lnkBase.AutoSize = true;
            this.lnkBase.Location = new System.Drawing.Point(83, 20);
            this.lnkBase.Name = "lnkBase";
            this.lnkBase.Size = new System.Drawing.Size(55, 13);
            this.lnkBase.TabIndex = 3;
            this.lnkBase.TabStop = true;
            this.lnkBase.Text = "linkLabel1";
            this.lnkBase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBase_LinkClicked);
            // 
            // txtGenerator
            // 
            this.txtGenerator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGenerator.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtGenerator.Location = new System.Drawing.Point(83, 43);
            this.txtGenerator.Name = "txtGenerator";
            this.txtGenerator.ReadOnly = true;
            this.txtGenerator.Size = new System.Drawing.Size(489, 13);
            this.txtGenerator.TabIndex = 5;
            // 
            // txtCase
            // 
            this.txtCase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCase.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCase.Location = new System.Drawing.Point(83, 63);
            this.txtCase.Name = "txtCase";
            this.txtCase.ReadOnly = true;
            this.txtCase.Size = new System.Drawing.Size(489, 13);
            this.txtCase.TabIndex = 1;
            // 
            // lblSitename
            // 
            this.lblSitename.AutoSize = true;
            this.lblSitename.Location = new System.Drawing.Point(3, 0);
            this.lblSitename.Name = "lblSitename";
            this.lblSitename.Size = new System.Drawing.Size(38, 13);
            this.lblSitename.TabIndex = 0;
            this.lblSitename.Text = "Name:";
            // 
            // lblDBDump
            // 
            this.lblDBDump.AutoSize = true;
            this.lblDBDump.Location = new System.Drawing.Point(1, 9);
            this.lblDBDump.Name = "lblDBDump";
            this.lblDBDump.Size = new System.Drawing.Size(85, 13);
            this.lblDBDump.TabIndex = 0;
            this.lblDBDump.Text = "Datebase dump:";
            // 
            // tabProps
            // 
            this.tabProps.Controls.Add(this.groupBox8);
            this.tabProps.Controls.Add(this.groupBox7);
            this.tabProps.Controls.Add(this.groupBox2);
            this.tabProps.Controls.Add(this.groupBox6);
            this.tabProps.Location = new System.Drawing.Point(4, 22);
            this.tabProps.Name = "tabProps";
            this.tabProps.Padding = new System.Windows.Forms.Padding(3);
            this.tabProps.Size = new System.Drawing.Size(587, 187);
            this.tabProps.TabIndex = 1;
            this.tabProps.Text = "Properties";
            this.tabProps.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.chkProjectNamespace);
            this.groupBox8.Controls.Add(this.chkCategoryNamespace);
            this.groupBox8.Controls.Add(this.chkImageNamespace);
            this.groupBox8.Controls.Add(this.chkTemplateNamespace);
            this.groupBox8.Controls.Add(this.chkMainNamespace);
            this.groupBox8.Location = new System.Drawing.Point(492, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(89, 130);
            this.groupBox8.TabIndex = 3;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Namespaces";
            // 
            // chkProjectNamespace
            // 
            this.chkProjectNamespace.AutoSize = true;
            this.chkProjectNamespace.Location = new System.Drawing.Point(6, 111);
            this.chkProjectNamespace.Name = "chkProjectNamespace";
            this.chkProjectNamespace.Size = new System.Drawing.Size(73, 17);
            this.chkProjectNamespace.TabIndex = 4;
            this.chkProjectNamespace.Text = "&Wikipedia";
            // 
            // chkCategoryNamespace
            // 
            this.chkCategoryNamespace.AutoSize = true;
            this.chkCategoryNamespace.Location = new System.Drawing.Point(6, 88);
            this.chkCategoryNamespace.Name = "chkCategoryNamespace";
            this.chkCategoryNamespace.Size = new System.Drawing.Size(68, 17);
            this.chkCategoryNamespace.TabIndex = 3;
            this.chkCategoryNamespace.Text = "&Category";
            // 
            // chkImageNamespace
            // 
            this.chkImageNamespace.AutoSize = true;
            this.chkImageNamespace.Location = new System.Drawing.Point(6, 65);
            this.chkImageNamespace.Name = "chkImageNamespace";
            this.chkImageNamespace.Size = new System.Drawing.Size(55, 17);
            this.chkImageNamespace.TabIndex = 2;
            this.chkImageNamespace.Text = "&Image";
            // 
            // chkTemplateNamespace
            // 
            this.chkTemplateNamespace.AutoSize = true;
            this.chkTemplateNamespace.Location = new System.Drawing.Point(6, 42);
            this.chkTemplateNamespace.Name = "chkTemplateNamespace";
            this.chkTemplateNamespace.Size = new System.Drawing.Size(70, 17);
            this.chkTemplateNamespace.TabIndex = 1;
            this.chkTemplateNamespace.Text = "&Template";
            // 
            // chkMainNamespace
            // 
            this.chkMainNamespace.AutoSize = true;
            this.chkMainNamespace.Checked = true;
            this.chkMainNamespace.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMainNamespace.Location = new System.Drawing.Point(6, 19);
            this.chkMainNamespace.Name = "chkMainNamespace";
            this.chkMainNamespace.Size = new System.Drawing.Size(54, 17);
            this.chkMainNamespace.TabIndex = 0;
            this.chkMainNamespace.Text = "&article";
            // 
            // groupBox7
            // 
            this.groupBox7.Location = new System.Drawing.Point(298, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(177, 147);
            this.groupBox7.TabIndex = 2;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Restrictions";
            // 
            // tabText
            // 
            this.tabText.Controls.Add(this.groupBox9);
            this.tabText.Controls.Add(this.groupBox1);
            this.tabText.Location = new System.Drawing.Point(4, 22);
            this.tabText.Name = "tabText";
            this.tabText.Padding = new System.Windows.Forms.Padding(3);
            this.tabText.Size = new System.Drawing.Size(587, 187);
            this.tabText.TabIndex = 2;
            this.tabText.Text = "Text";
            this.tabText.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.chkIgnoreRedirects);
            this.groupBox9.Controls.Add(this.lblWords);
            this.groupBox9.Controls.Add(this.cmboWords);
            this.groupBox9.Controls.Add(this.nudWords);
            this.groupBox9.Controls.Add(this.lblLinks);
            this.groupBox9.Controls.Add(this.cmboLinks);
            this.groupBox9.Controls.Add(this.nudLinks);
            this.groupBox9.Controls.Add(this.lblLength);
            this.groupBox9.Controls.Add(this.cmboLength);
            this.groupBox9.Controls.Add(this.nudLength);
            this.groupBox9.Location = new System.Drawing.Point(311, 6);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(261, 149);
            this.groupBox9.TabIndex = 18;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Page Text Properties";
            // 
            // chkIgnoreRedirects
            // 
            this.chkIgnoreRedirects.AutoSize = true;
            this.chkIgnoreRedirects.Location = new System.Drawing.Point(69, 97);
            this.chkIgnoreRedirects.Name = "chkIgnoreRedirects";
            this.chkIgnoreRedirects.Size = new System.Drawing.Size(104, 17);
            this.chkIgnoreRedirects.TabIndex = 17;
            this.chkIgnoreRedirects.Text = "Ignore Redirects";
            this.chkIgnoreRedirects.UseVisualStyleBackColor = true;
            // 
            // tabAWB
            // 
            this.tabAWB.Controls.Add(this.groupBox5);
            this.tabAWB.Location = new System.Drawing.Point(4, 22);
            this.tabAWB.Name = "tabAWB";
            this.tabAWB.Padding = new System.Windows.Forms.Padding(3);
            this.tabAWB.Size = new System.Drawing.Size(587, 187);
            this.tabAWB.TabIndex = 3;
            this.tabAWB.Text = "AWB";
            this.tabAWB.UseVisualStyleBackColor = true;
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
            // lbArticles
            // 
            this.lbArticles.ContextMenuStrip = this.ArticlesListBoxContextMenu;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(6, 69);
            this.lbArticles.MultiColumn = true;
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(291, 238);
            this.lbArticles.TabIndex = 5;
            // 
            // DatabaseScanner
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 588);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DatabaseScanner";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wiki Database Scanner";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeadingSpace)).EndInit();
            this.ArticlesListBoxContextMenu.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLimitResults)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabDump.ResumeLayout(false);
            this.tabDump.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabProps.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tabText.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.tabAWB.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtList;
        private System.Windows.Forms.OpenFileDialog openXMLDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Button btnClear;
        private Button btnCopy;
        private ToolTip tooltip;
        private CheckBox chkHeading;
        private NumericUpDown nudHeadingSpace;
        private TextBox txtTitleContains;
        private CheckBox chkTitleContains;
        private Button btnTransfer;
        private Button btnAlphaList;
        private Button btnClearList;
        private ContextMenuStrip ArticlesListBoxContextMenu;
        private ToolStripMenuItem copyToolStripMenuItem;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Button btnSave;
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
        private GroupBox groupBox5;
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
        private RadioButton rdoUnbulletedLinks;
        private RadioButton rdoHeaderError;
        private RadioButton rdoHasHTML;
        private RadioButton rdoBadLinks;
        private RadioButton rdoSimpleLinks;
        private RadioButton rdoNoBold;
        private RadioButton rdoNone;
        private RadioButton rdoTypo;
        private CheckBox chkABCHeader;
        private Label lblStartFrom;
        private TextBox txtStartFrom;
        private Label lblListType;
        private Button btnStart;
        private NumericUpDown nudLimitResults;
        private Label lblLimitResutls;
        private DateTimePicker beginDate;
        private GroupBox groupBox6;
        private TabControl tabControl1;
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
        private ToolStripSplitButton threadPriorityButton;
        private CheckBox chkIgnoreComments;
        private CheckBox chkIgnoreRedirects;
        private CheckBox chkImageNamespace;
        private CheckBox chkCategoryNamespace;
        private CheckBox chkMainNamespace;
        private CheckBox chkProjectNamespace;
        private CheckBox chkTemplateNamespace;
        private GroupBox groupBox7;
        private TableLayoutPanel tableLayoutPanel1;
        private LinkLabel lnkBase;
        private ToolStripMenuItem openRevisionInBowserToolStripMenuItem;
        private GroupBox groupBox8;
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
        private GroupBox groupBox9;
        private TextBox txtGenerator;
        private TextBox txtCase;
        private LinkLabel lnkWikiaDumps;
        private LinkLabel lnkWmfDumps;
        private LinkLabel lnkGenDump;
        private StatusStrip statusStrip;
        private DateTimePicker endDate;
        private Label lblEndDate;
        private Label lblDate;
    }
}



