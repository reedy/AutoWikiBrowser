/*
    Autowikibrowser
    Copyright (C) 2006 Martin Richards

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

namespace AutoWikiBrowser
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listMaker1 = new WikiFunctions.Lists.ListMaker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tpEdit = new System.Windows.Forms.TabPage();
            this.txtEdit = new System.Windows.Forms.TextBox();
            this.mnuTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.wordWrapToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore1 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore2 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore3 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore4 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore5 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore6 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore7 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore8 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore9 = new System.Windows.Forms.ToolStripTextBox();
            this.PasteMore10 = new System.Windows.Forms.ToolStripTextBox();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.birthdeathCatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metadataTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.humanNameCategoryKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.humanNameDisambigTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speedyDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncategorisedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.convertListToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.unicodifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bypassAllRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllExcessWhitespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reparseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openPageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.openSelectionInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceTextWithLastEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tpLogs = new System.Windows.Forms.TabPage();
            this.lvIgnored = new WikiFunctions.NoFlickerListView();
            this.colIgnoreArticle = new System.Windows.Forms.ColumnHeader();
            this.colSkippedBy = new System.Windows.Forms.ColumnHeader();
            this.colSkipReason = new System.Windows.Forms.ColumnHeader();
            this.lvSaved = new WikiFunctions.NoFlickerListView();
            this.colSuccessSave = new System.Windows.Forms.ColumnHeader();
            this.btnAddToList = new System.Windows.Forms.Button();
            this.btnClearIgnored = new System.Windows.Forms.Button();
            this.btnSaveIgnored = new System.Windows.Forms.Button();
            this.btnClearSaved = new System.Windows.Forms.Button();
            this.btnSaveSaved = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDefaultSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterOutNonMainSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialFilterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertFromTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphabeticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToTextFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.launchDumpSearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchListComparerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.PreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.enableTheToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bypassRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotAutomaticallyDoAnythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboOnLoad = new System.Windows.Forms.ToolStripComboBox();
            this.ignoreNoBotsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.markAllAsMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAllToWatchlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.alphaSortInterwikiLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.addIgnoredToLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.summariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHidePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.makeModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testRegexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.runUpdaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpHTMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutDebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBotTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblIgnoredArticles = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEditCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEditsPerMin = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnDiff = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnFalsePositive = new System.Windows.Forms.Button();
            this.txtNewCategory = new System.Windows.Forms.TextBox();
            this.txtAppendMessage = new System.Windows.Forms.TextBox();
            this.chkAppend = new System.Windows.Forms.CheckBox();
            this.rdoAppend = new System.Windows.Forms.RadioButton();
            this.rdoPrepend = new System.Windows.Forms.RadioButton();
            this.chkSkipIfContains = new System.Windows.Forms.CheckBox();
            this.txtSkipIfNotContains = new System.Windows.Forms.TextBox();
            this.txtSkipIfContains = new System.Windows.Forms.TextBox();
            this.chkSkipIfNotContains = new System.Windows.Forms.CheckBox();
            this.chkSkipIsRegex = new System.Windows.Forms.CheckBox();
            this.chkSkipNoChanges = new System.Windows.Forms.CheckBox();
            this.chkGeneralFixes = new System.Windows.Forms.CheckBox();
            this.chkAutoTagger = new System.Windows.Forms.CheckBox();
            this.chkUnicodifyWhole = new System.Windows.Forms.CheckBox();
            this.chkFindandReplace = new System.Windows.Forms.CheckBox();
            this.chkQuickSave = new System.Windows.Forms.CheckBox();
            this.nudBotSpeed = new System.Windows.Forms.NumericUpDown();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveXML = new System.Windows.Forms.SaveFileDialog();
            this.openXML = new System.Windows.Forms.OpenFileDialog();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpSetOptions = new System.Windows.Forms.TabPage();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.chkSkipIfNoRegexTypo = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chkRegExTypo = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSubst = new System.Windows.Forms.Button();
            this.chkSkipWhenNoFAR = new System.Windows.Forms.CheckBox();
            this.btnFindAndReplaceAdvanced = new System.Windows.Forms.Button();
            this.btnMoreFindAndReplce = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnMoreSkip = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.chkSkipNonExistent = new System.Windows.Forms.CheckBox();
            this.chkSkipCaseSensitive = new System.Windows.Forms.CheckBox();
            this.tpMoreOptions = new System.Windows.Forms.TabPage();
            this.ImageGroupBox = new System.Windows.Forms.GroupBox();
            this.lblImageWith = new System.Windows.Forms.Label();
            this.chkSkipNoImgChange = new System.Windows.Forms.CheckBox();
            this.txtImageWith = new System.Windows.Forms.TextBox();
            this.txtImageReplace = new System.Windows.Forms.TextBox();
            this.cmboImages = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkSkipNoCatChange = new System.Windows.Forms.CheckBox();
            this.txtNewCategory2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboCategorise = new System.Windows.Forms.ComboBox();
            this.tpDab = new System.Windows.Forms.TabPage();
            this.panelDab = new System.Windows.Forms.Panel();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udContextChars = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chkSkipNoDab = new System.Windows.Forms.CheckBox();
            this.txtDabVariants = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.btnLoadLinks = new System.Windows.Forms.Button();
            this.txtDabLink = new System.Windows.Forms.TextBox();
            this.chkEnableDab = new System.Windows.Forms.CheckBox();
            this.tpBots = new System.Windows.Forms.TabPage();
            this.lblOnlyBots = new System.Windows.Forms.Label();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.chkNudgeSkip = new System.Windows.Forms.CheckBox();
            this.btnResetNudges = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblNudges = new System.Windows.Forms.Label();
            this.nudNudgeTime = new System.Windows.Forms.NumericUpDown();
            this.chkNudge = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSuppressTag = new System.Windows.Forms.CheckBox();
            this.chkAutoMode = new System.Windows.Forms.CheckBox();
            this.lblAutoDelay = new System.Windows.Forms.Label();
            this.tpStart = new System.Windows.Forms.TabPage();
            this.lblSummary = new System.Windows.Forms.Label();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.lblDone = new System.Windows.Forms.Label();
            this.chkFindCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkFindRegex = new System.Windows.Forms.CheckBox();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.cmboEditSummary = new System.Windows.Forms.ComboBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lbDuplicateWikilinks = new System.Windows.Forms.ListBox();
            this.lblDuplicateWikilinks = new System.Windows.Forms.Label();
            this.lblWarn = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblWords = new System.Windows.Forms.Label();
            this.lblInterLinks = new System.Windows.Forms.Label();
            this.lblCats = new System.Windows.Forms.Label();
            this.lblImages = new System.Windows.Forms.Label();
            this.lblLinks = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btntsShowHide = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsStart = new System.Windows.Forms.ToolStripButton();
            this.btntsStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsSave = new System.Windows.Forms.ToolStripButton();
            this.btntsIgnore = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsPreview = new System.Windows.Forms.ToolStripButton();
            this.btntsChanges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsFalsePositive = new System.Windows.Forms.ToolStripButton();
            this.lbltsNumberofItems = new System.Windows.Forms.ToolStripLabel();
            this.mnuWebBrowser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ntfyTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveTimer = new System.Windows.Forms.Timer(this.components);
            this.strListFile = new System.Windows.Forms.SaveFileDialog();
            this.webBrowserEdit = new WikiFunctions.Browser.WebControl();
            this.colSuccessTime = new System.Windows.Forms.ColumnHeader();
            this.colIgnoreTime = new System.Windows.Forms.ColumnHeader();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tpEdit.SuspendLayout();
            this.mnuTextBox.SuspendLayout();
            this.tpLogs.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpSetOptions.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tpMoreOptions.SuspendLayout();
            this.ImageGroupBox.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tpDab.SuspendLayout();
            this.panelDab.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udContextChars)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.tpBots.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNudgeTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.tpStart.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.mnuWebBrowser.SuspendLayout();
            this.mnuNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.listMaker1);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 372);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "(1) Make list";
            // 
            // listMaker1
            // 
            this.listMaker1.ListFile = "";
            this.listMaker1.Location = new System.Drawing.Point(3, 15);
            this.listMaker1.Name = "listMaker1";
            this.listMaker1.SelectedSource = WikiFunctions.Lists.SourceType.Category;
            this.listMaker1.Size = new System.Drawing.Size(201, 351);
            this.listMaker1.SourceText = "";
            this.listMaker1.TabIndex = 0;
            this.listMaker1.WikiStatus = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tabControl2);
            this.panel1.Location = new System.Drawing.Point(511, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(269, 372);
            this.panel1.TabIndex = 9;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tpEdit);
            this.tabControl2.Controls.Add(this.tpLogs);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(269, 372);
            this.tabControl2.TabIndex = 1;
            // 
            // tpEdit
            // 
            this.tpEdit.Controls.Add(this.txtEdit);
            this.tpEdit.Location = new System.Drawing.Point(4, 22);
            this.tpEdit.Name = "tpEdit";
            this.tpEdit.Padding = new System.Windows.Forms.Padding(3);
            this.tpEdit.Size = new System.Drawing.Size(261, 346);
            this.tpEdit.TabIndex = 0;
            this.tpEdit.Text = "Edit box";
            this.tpEdit.UseVisualStyleBackColor = true;
            // 
            // txtEdit
            // 
            this.txtEdit.AcceptsReturn = true;
            this.txtEdit.AcceptsTab = true;
            this.txtEdit.ContextMenuStrip = this.mnuTextBox;
            this.txtEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEdit.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEdit.Location = new System.Drawing.Point(3, 3);
            this.txtEdit.MaxLength = 0;
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEdit.Size = new System.Drawing.Size(255, 340);
            this.txtEdit.TabIndex = 0;
            this.txtEdit.TextChanged += new System.EventHandler(this.txtEdit_TextChanged);
            // 
            // mnuTextBox
            // 
            this.mnuTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapToolStripMenuItem1,
            this.toolStripSeparator1,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.pasteMoreToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator10,
            this.goToLineToolStripMenuItem,
            this.toolStripSeparator3,
            this.insertToolStripMenuItem,
            this.insertTagToolStripMenuItem,
            this.convertListToToolStripMenuItem,
            this.unicodifyToolStripMenuItem,
            this.bypassAllRedirectsToolStripMenuItem,
            this.removeAllExcessWhitespaceToolStripMenuItem,
            this.reparseToolStripMenuItem,
            this.toolStripSeparator4,
            this.openPageInBrowserToolStripMenuItem,
            this.openHistoryMenuItem,
            this.toolStripSeparator20,
            this.openSelectionInBrowserToolStripMenuItem,
            this.toolStripSeparator9,
            this.replaceTextWithLastEditToolStripMenuItem});
            this.mnuTextBox.Name = "contextMenuStrip1";
            this.mnuTextBox.Size = new System.Drawing.Size(233, 458);
            this.mnuTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // wordWrapToolStripMenuItem1
            // 
            this.wordWrapToolStripMenuItem1.Checked = true;
            this.wordWrapToolStripMenuItem1.CheckOnClick = true;
            this.wordWrapToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wordWrapToolStripMenuItem1.Name = "wordWrapToolStripMenuItem1";
            this.wordWrapToolStripMenuItem1.Size = new System.Drawing.Size(232, 22);
            this.wordWrapToolStripMenuItem1.Text = "Word wrap";
            this.wordWrapToolStripMenuItem1.Click += new System.EventHandler(this.wordWrapToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // pasteMoreToolStripMenuItem
            // 
            this.pasteMoreToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PasteMore1,
            this.PasteMore2,
            this.PasteMore3,
            this.PasteMore4,
            this.PasteMore5,
            this.PasteMore6,
            this.PasteMore7,
            this.PasteMore8,
            this.PasteMore9,
            this.PasteMore10});
            this.pasteMoreToolStripMenuItem.Name = "pasteMoreToolStripMenuItem";
            this.pasteMoreToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.pasteMoreToolStripMenuItem.Text = "Paste more";
            // 
            // PasteMore1
            // 
            this.PasteMore1.Name = "PasteMore1";
            this.PasteMore1.Size = new System.Drawing.Size(100, 21);
            this.PasteMore1.DoubleClick += new System.EventHandler(this.PasteMore1_DoubleClick);
            // 
            // PasteMore2
            // 
            this.PasteMore2.Name = "PasteMore2";
            this.PasteMore2.Size = new System.Drawing.Size(100, 21);
            this.PasteMore2.DoubleClick += new System.EventHandler(this.PasteMore2_DoubleClick);
            // 
            // PasteMore3
            // 
            this.PasteMore3.Name = "PasteMore3";
            this.PasteMore3.Size = new System.Drawing.Size(100, 21);
            this.PasteMore3.DoubleClick += new System.EventHandler(this.PasteMore3_DoubleClick);
            // 
            // PasteMore4
            // 
            this.PasteMore4.Name = "PasteMore4";
            this.PasteMore4.Size = new System.Drawing.Size(100, 21);
            this.PasteMore4.DoubleClick += new System.EventHandler(this.PasteMore4_DoubleClick);
            // 
            // PasteMore5
            // 
            this.PasteMore5.Name = "PasteMore5";
            this.PasteMore5.Size = new System.Drawing.Size(100, 21);
            this.PasteMore5.DoubleClick += new System.EventHandler(this.PasteMore5_DoubleClick);
            // 
            // PasteMore6
            // 
            this.PasteMore6.Name = "PasteMore6";
            this.PasteMore6.Size = new System.Drawing.Size(100, 21);
            this.PasteMore6.DoubleClick += new System.EventHandler(this.PasteMore6_DoubleClick);
            // 
            // PasteMore7
            // 
            this.PasteMore7.Name = "PasteMore7";
            this.PasteMore7.Size = new System.Drawing.Size(100, 21);
            this.PasteMore7.DoubleClick += new System.EventHandler(this.PasteMore7_DoubleClick);
            // 
            // PasteMore8
            // 
            this.PasteMore8.Name = "PasteMore8";
            this.PasteMore8.Size = new System.Drawing.Size(100, 21);
            this.PasteMore8.DoubleClick += new System.EventHandler(this.PasteMore8_DoubleClick);
            // 
            // PasteMore9
            // 
            this.PasteMore9.Name = "PasteMore9";
            this.PasteMore9.Size = new System.Drawing.Size(100, 21);
            this.PasteMore9.DoubleClick += new System.EventHandler(this.PasteMore9_DoubleClick);
            // 
            // PasteMore10
            // 
            this.PasteMore10.Name = "PasteMore10";
            this.PasteMore10.Size = new System.Drawing.Size(100, 21);
            this.PasteMore10.DoubleClick += new System.EventHandler(this.PasteMore10_DoubleClick);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(229, 6);
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.goToLineToolStripMenuItem.Text = "Go to line";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.MaxLength = 6;
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBox2.Text = "Enter line number";
            this.toolStripTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBox2_KeyPress);
            this.toolStripTextBox2.Click += new System.EventHandler(this.toolStripTextBox2_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(229, 6);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.birthdeathCatsToolStripMenuItem,
            this.metadataTemplateToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.insertToolStripMenuItem.Text = "Insert...";
            // 
            // birthdeathCatsToolStripMenuItem
            // 
            this.birthdeathCatsToolStripMenuItem.Name = "birthdeathCatsToolStripMenuItem";
            this.birthdeathCatsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.birthdeathCatsToolStripMenuItem.Text = "Guess birth/death cats";
            this.birthdeathCatsToolStripMenuItem.Click += new System.EventHandler(this.birthdeathCatsToolStripMenuItem_Click);
            // 
            // metadataTemplateToolStripMenuItem
            // 
            this.metadataTemplateToolStripMenuItem.Name = "metadataTemplateToolStripMenuItem";
            this.metadataTemplateToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.metadataTemplateToolStripMenuItem.Text = "Meta-data template";
            this.metadataTemplateToolStripMenuItem.Click += new System.EventHandler(this.metadataTemplateToolStripMenuItem_Click);
            // 
            // insertTagToolStripMenuItem
            // 
            this.insertTagToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.humanNameCategoryKeyToolStripMenuItem,
            this.humanNameDisambigTagToolStripMenuItem,
            this.wikifyToolStripMenuItem,
            this.cleanupToolStripMenuItem,
            this.expandToolStripMenuItem,
            this.speedyDeleteToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.uncategorisedToolStripMenuItem,
            this.stubToolStripMenuItem,
            this.toolStripTextBox1});
            this.insertTagToolStripMenuItem.Name = "insertTagToolStripMenuItem";
            this.insertTagToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.insertTagToolStripMenuItem.Text = "Insert tag";
            // 
            // humanNameCategoryKeyToolStripMenuItem
            // 
            this.humanNameCategoryKeyToolStripMenuItem.Name = "humanNameCategoryKeyToolStripMenuItem";
            this.humanNameCategoryKeyToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.humanNameCategoryKeyToolStripMenuItem.Text = "Human name DEFAULTSORT";
            this.humanNameCategoryKeyToolStripMenuItem.Click += new System.EventHandler(this.humanNameCategoryKeyToolStripMenuItem_Click);
            // 
            // humanNameDisambigTagToolStripMenuItem
            // 
            this.humanNameDisambigTagToolStripMenuItem.Name = "humanNameDisambigTagToolStripMenuItem";
            this.humanNameDisambigTagToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.humanNameDisambigTagToolStripMenuItem.Text = "Human name disambig tag";
            this.humanNameDisambigTagToolStripMenuItem.Click += new System.EventHandler(this.humanNameDisambigTagToolStripMenuItem_Click);
            // 
            // wikifyToolStripMenuItem
            // 
            this.wikifyToolStripMenuItem.Name = "wikifyToolStripMenuItem";
            this.wikifyToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.wikifyToolStripMenuItem.Text = "Wikify";
            this.wikifyToolStripMenuItem.Click += new System.EventHandler(this.wikifyToolStripMenuItem_Click);
            // 
            // cleanupToolStripMenuItem
            // 
            this.cleanupToolStripMenuItem.Name = "cleanupToolStripMenuItem";
            this.cleanupToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.cleanupToolStripMenuItem.Text = "Cleanup";
            this.cleanupToolStripMenuItem.Click += new System.EventHandler(this.cleanupToolStripMenuItem_Click);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // speedyDeleteToolStripMenuItem
            // 
            this.speedyDeleteToolStripMenuItem.Name = "speedyDeleteToolStripMenuItem";
            this.speedyDeleteToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.speedyDeleteToolStripMenuItem.Text = "Speedy delete";
            this.speedyDeleteToolStripMenuItem.Click += new System.EventHandler(this.speedyDeleteToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.clearToolStripMenuItem.Text = "{{clear}}";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // uncategorisedToolStripMenuItem
            // 
            this.uncategorisedToolStripMenuItem.Name = "uncategorisedToolStripMenuItem";
            this.uncategorisedToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.uncategorisedToolStripMenuItem.Text = "Uncategorised";
            this.uncategorisedToolStripMenuItem.Click += new System.EventHandler(this.uncategorisedToolStripMenuItem_Click);
            // 
            // stubToolStripMenuItem
            // 
            this.stubToolStripMenuItem.Name = "stubToolStripMenuItem";
            this.stubToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.stubToolStripMenuItem.Text = "Stub";
            this.stubToolStripMenuItem.Click += new System.EventHandler(this.stubToolStripMenuItem_Click);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBox1.Text = "{{stub}}";
            // 
            // convertListToToolStripMenuItem
            // 
            this.convertListToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listToolStripMenuItem,
            this.listToolStripMenuItem1});
            this.convertListToToolStripMenuItem.Name = "convertListToToolStripMenuItem";
            this.convertListToToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.convertListToToolStripMenuItem.Text = "Convert list to";
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.listToolStripMenuItem.Text = "* List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem1
            // 
            this.listToolStripMenuItem1.Name = "listToolStripMenuItem1";
            this.listToolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
            this.listToolStripMenuItem1.Text = "# List";
            this.listToolStripMenuItem1.Click += new System.EventHandler(this.listToolStripMenuItem1_Click);
            // 
            // unicodifyToolStripMenuItem
            // 
            this.unicodifyToolStripMenuItem.Name = "unicodifyToolStripMenuItem";
            this.unicodifyToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.unicodifyToolStripMenuItem.Text = "Unicodify selected";
            this.unicodifyToolStripMenuItem.Click += new System.EventHandler(this.unicodifyToolStripMenuItem_Click);
            // 
            // bypassAllRedirectsToolStripMenuItem
            // 
            this.bypassAllRedirectsToolStripMenuItem.Enabled = false;
            this.bypassAllRedirectsToolStripMenuItem.Name = "bypassAllRedirectsToolStripMenuItem";
            this.bypassAllRedirectsToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.bypassAllRedirectsToolStripMenuItem.Text = "Bypass all redirects";
            this.bypassAllRedirectsToolStripMenuItem.Click += new System.EventHandler(this.bypassAllRedirectsToolStripMenuItem_Click);
            // 
            // removeAllExcessWhitespaceToolStripMenuItem
            // 
            this.removeAllExcessWhitespaceToolStripMenuItem.Name = "removeAllExcessWhitespaceToolStripMenuItem";
            this.removeAllExcessWhitespaceToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.removeAllExcessWhitespaceToolStripMenuItem.Text = "Fix all excess whitespace";
            this.removeAllExcessWhitespaceToolStripMenuItem.Click += new System.EventHandler(this.removeAllExcessWhitespaceToolStripMenuItem_Click);
            // 
            // reparseToolStripMenuItem
            // 
            this.reparseToolStripMenuItem.Name = "reparseToolStripMenuItem";
            this.reparseToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.reparseToolStripMenuItem.Text = "Re-parse";
            this.reparseToolStripMenuItem.Click += new System.EventHandler(this.reparseToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(229, 6);
            // 
            // openPageInBrowserToolStripMenuItem
            // 
            this.openPageInBrowserToolStripMenuItem.Name = "openPageInBrowserToolStripMenuItem";
            this.openPageInBrowserToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.openPageInBrowserToolStripMenuItem.Text = "Open page in browser";
            this.openPageInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPageInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryMenuItem
            // 
            this.openHistoryMenuItem.Name = "openHistoryMenuItem";
            this.openHistoryMenuItem.Size = new System.Drawing.Size(232, 22);
            this.openHistoryMenuItem.Text = "Open page history in browser";
            this.openHistoryMenuItem.Click += new System.EventHandler(this.openHistoryMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(229, 6);
            // 
            // openSelectionInBrowserToolStripMenuItem
            // 
            this.openSelectionInBrowserToolStripMenuItem.Name = "openSelectionInBrowserToolStripMenuItem";
            this.openSelectionInBrowserToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.openSelectionInBrowserToolStripMenuItem.Text = "Open text selection in browser";
            this.openSelectionInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openSelectionInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(229, 6);
            // 
            // replaceTextWithLastEditToolStripMenuItem
            // 
            this.replaceTextWithLastEditToolStripMenuItem.Name = "replaceTextWithLastEditToolStripMenuItem";
            this.replaceTextWithLastEditToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.replaceTextWithLastEditToolStripMenuItem.Text = "Replace text with last edit";
            this.replaceTextWithLastEditToolStripMenuItem.Click += new System.EventHandler(this.replaceTextWithLastEditToolStripMenuItem_Click);
            // 
            // tpLogs
            // 
            this.tpLogs.Controls.Add(this.lvIgnored);
            this.tpLogs.Controls.Add(this.lvSaved);
            this.tpLogs.Controls.Add(this.btnAddToList);
            this.tpLogs.Controls.Add(this.btnClearIgnored);
            this.tpLogs.Controls.Add(this.btnSaveIgnored);
            this.tpLogs.Controls.Add(this.btnClearSaved);
            this.tpLogs.Controls.Add(this.btnSaveSaved);
            this.tpLogs.Controls.Add(this.label8);
            this.tpLogs.Controls.Add(this.label7);
            this.tpLogs.Location = new System.Drawing.Point(4, 22);
            this.tpLogs.Name = "tpLogs";
            this.tpLogs.Padding = new System.Windows.Forms.Padding(3);
            this.tpLogs.Size = new System.Drawing.Size(261, 346);
            this.tpLogs.TabIndex = 1;
            this.tpLogs.Text = "View log";
            this.tpLogs.UseVisualStyleBackColor = true;
            // 
            // lvIgnored
            // 
            this.lvIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvIgnored.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIgnoreArticle,
            this.colIgnoreTime,
            this.colSkippedBy,
            this.colSkipReason});
            this.lvIgnored.Location = new System.Drawing.Point(9, 202);
            this.lvIgnored.Name = "lvIgnored";
            this.lvIgnored.Size = new System.Drawing.Size(243, 108);
            this.lvIgnored.TabIndex = 12;
            this.lvIgnored.UseCompatibleStateImageBehavior = false;
            this.lvIgnored.View = System.Windows.Forms.View.Details;
            // 
            // colIgnoreArticle
            // 
            this.colIgnoreArticle.Text = "Article";
            this.colIgnoreArticle.Width = 48;
            // 
            // colSkippedBy
            // 
            this.colSkippedBy.Text = "Skipped By";
            this.colSkippedBy.Width = 70;
            // 
            // colSkipReason
            // 
            this.colSkipReason.Text = "Skip Reason";
            this.colSkipReason.Width = 81;
            // 
            // lvSaved
            // 
            this.lvSaved.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSaved.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSuccessSave,
            this.colSuccessTime});
            this.lvSaved.Location = new System.Drawing.Point(9, 22);
            this.lvSaved.Name = "lvSaved";
            this.lvSaved.Size = new System.Drawing.Size(243, 115);
            this.lvSaved.TabIndex = 11;
            this.lvSaved.UseCompatibleStateImageBehavior = false;
            this.lvSaved.View = System.Windows.Forms.View.Details;
            // 
            // colSuccessSave
            // 
            this.colSuccessSave.Text = "Article";
            this.colSuccessSave.Width = 171;
            // 
            // btnAddToList
            // 
            this.btnAddToList.Location = new System.Drawing.Point(143, 316);
            this.btnAddToList.Name = "btnAddToList";
            this.btnAddToList.Size = new System.Drawing.Size(112, 24);
            this.btnAddToList.TabIndex = 8;
            this.btnAddToList.Text = "Add to article list";
            this.btnAddToList.UseVisualStyleBackColor = true;
            this.btnAddToList.Click += new System.EventHandler(this.btnAddtoList_Click);
            // 
            // btnClearIgnored
            // 
            this.btnClearIgnored.Location = new System.Drawing.Point(87, 316);
            this.btnClearIgnored.Name = "btnClearIgnored";
            this.btnClearIgnored.Size = new System.Drawing.Size(50, 24);
            this.btnClearIgnored.TabIndex = 7;
            this.btnClearIgnored.Text = "Clear";
            this.btnClearIgnored.UseVisualStyleBackColor = true;
            this.btnClearIgnored.Click += new System.EventHandler(this.btnClearIgnored_Click);
            // 
            // btnSaveIgnored
            // 
            this.btnSaveIgnored.Location = new System.Drawing.Point(6, 316);
            this.btnSaveIgnored.Name = "btnSaveIgnored";
            this.btnSaveIgnored.Size = new System.Drawing.Size(75, 24);
            this.btnSaveIgnored.TabIndex = 6;
            this.btnSaveIgnored.Text = "Save log";
            this.btnSaveIgnored.UseVisualStyleBackColor = true;
            this.btnSaveIgnored.Click += new System.EventHandler(this.btnSaveIgnored_Click);
            // 
            // btnClearSaved
            // 
            this.btnClearSaved.Location = new System.Drawing.Point(87, 149);
            this.btnClearSaved.Name = "btnClearSaved";
            this.btnClearSaved.Size = new System.Drawing.Size(75, 24);
            this.btnClearSaved.TabIndex = 5;
            this.btnClearSaved.Text = "Clear";
            this.btnClearSaved.UseVisualStyleBackColor = true;
            this.btnClearSaved.Click += new System.EventHandler(this.btnClearSaved_Click);
            // 
            // btnSaveSaved
            // 
            this.btnSaveSaved.Location = new System.Drawing.Point(6, 149);
            this.btnSaveSaved.Name = "btnSaveSaved";
            this.btnSaveSaved.Size = new System.Drawing.Size(75, 24);
            this.btnSaveSaved.TabIndex = 4;
            this.btnSaveSaved.Text = "Save log";
            this.btnSaveSaved.UseVisualStyleBackColor = true;
            this.btnSaveSaved.Click += new System.EventHandler(this.btnSaveSaved_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 186);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Skipped:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Successfully saved:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generalToolStripMenuItem,
            this.ToolStripMenuGeneral,
            this.toolStripAdvanced,
            this.pluginsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(783, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDefaultSettingsToolStripMenuItem,
            this.loadSettingsToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveSettingsToolStripMenuItem,
            this.saveCurrentSettingsToolStripMenuItem,
            this.saveAsDefaultToolStripMenuItem,
            this.toolStripSeparator8,
            this.loginToolStripMenuItem,
            this.logOutToolStripMenuItem,
            this.toolStripSeparator17,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadDefaultSettingsToolStripMenuItem
            // 
            this.loadDefaultSettingsToolStripMenuItem.Name = "loadDefaultSettingsToolStripMenuItem";
            this.loadDefaultSettingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.loadDefaultSettingsToolStripMenuItem.Text = "New settings file";
            this.loadDefaultSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadDefaultSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.loadSettingsToolStripMenuItem.Text = "Open settings";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.recentToolStripMenuItem.Text = "Recent settings";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(198, 6);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveSettingsToolStripMenuItem.Text = "Save settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // saveCurrentSettingsToolStripMenuItem
            // 
            this.saveCurrentSettingsToolStripMenuItem.Name = "saveCurrentSettingsToolStripMenuItem";
            this.saveCurrentSettingsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveCurrentSettingsToolStripMenuItem.Text = "Save settings as...";
            this.saveCurrentSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveCurrentSettingsToolStripMenuItem_Click);
            // 
            // saveAsDefaultToolStripMenuItem
            // 
            this.saveAsDefaultToolStripMenuItem.Name = "saveAsDefaultToolStripMenuItem";
            this.saveAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.saveAsDefaultToolStripMenuItem.Text = "Save settings as default";
            this.saveAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.saveAsDefaultToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(198, 6);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.loginToolStripMenuItem.Text = "Log In";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            this.logOutToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.logOutToolStripMenuItem.Text = "Log Out";
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(198, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterOutNonMainSpaceToolStripMenuItem,
            this.specialFilterToolStripMenuItem1,
            this.convertToTalkPagesToolStripMenuItem,
            this.convertFromTalkPagesToolStripMenuItem,
            this.sortAlphabeticallyToolStripMenuItem,
            this.saveListToTextFileToolStripMenuItem,
            this.toolStripSeparator13,
            this.launchDumpSearcherToolStripMenuItem,
            this.launchListComparerToolStripMenuItem});
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.generalToolStripMenuItem.Text = "List";
            // 
            // filterOutNonMainSpaceToolStripMenuItem
            // 
            this.filterOutNonMainSpaceToolStripMenuItem.CheckOnClick = true;
            this.filterOutNonMainSpaceToolStripMenuItem.Name = "filterOutNonMainSpaceToolStripMenuItem";
            this.filterOutNonMainSpaceToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.filterOutNonMainSpaceToolStripMenuItem.Text = "Filter out non main space";
            this.filterOutNonMainSpaceToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceToolStripMenuItem_Click);
            // 
            // specialFilterToolStripMenuItem1
            // 
            this.specialFilterToolStripMenuItem1.Name = "specialFilterToolStripMenuItem1";
            this.specialFilterToolStripMenuItem1.Size = new System.Drawing.Size(210, 22);
            this.specialFilterToolStripMenuItem1.Text = "Filter";
            this.specialFilterToolStripMenuItem1.Click += new System.EventHandler(this.specialFilterToolStripMenuItem1_Click);
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            this.convertToTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.convertToTalkPagesToolStripMenuItem.Text = "Convert to talk pages";
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            this.convertFromTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.convertFromTalkPagesToolStripMenuItem.Text = "Convert from talk pages";
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // sortAlphabeticallyToolStripMenuItem
            // 
            this.sortAlphabeticallyToolStripMenuItem.CheckOnClick = true;
            this.sortAlphabeticallyToolStripMenuItem.Name = "sortAlphabeticallyToolStripMenuItem";
            this.sortAlphabeticallyToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.sortAlphabeticallyToolStripMenuItem.Text = "Sort alphabetically";
            this.sortAlphabeticallyToolStripMenuItem.Click += new System.EventHandler(this.sortAlphabeticallyToolStripMenuItem_Click);
            // 
            // saveListToTextFileToolStripMenuItem
            // 
            this.saveListToTextFileToolStripMenuItem.Name = "saveListToTextFileToolStripMenuItem";
            this.saveListToTextFileToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.saveListToTextFileToolStripMenuItem.Text = "Save list to text file";
            this.saveListToTextFileToolStripMenuItem.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(207, 6);
            // 
            // launchDumpSearcherToolStripMenuItem
            // 
            this.launchDumpSearcherToolStripMenuItem.Name = "launchDumpSearcherToolStripMenuItem";
            this.launchDumpSearcherToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.launchDumpSearcherToolStripMenuItem.Text = "Launch Database Scanner";
            this.launchDumpSearcherToolStripMenuItem.Click += new System.EventHandler(this.launchDumpSearcherToolStripMenuItem_Click);
            // 
            // launchListComparerToolStripMenuItem
            // 
            this.launchListComparerToolStripMenuItem.Name = "launchListComparerToolStripMenuItem";
            this.launchListComparerToolStripMenuItem.Size = new System.Drawing.Size(210, 22);
            this.launchListComparerToolStripMenuItem.Text = "Launch ListComparer";
            this.launchListComparerToolStripMenuItem.Click += new System.EventHandler(this.launchListComparerToolStripMenuItem_Click);
            // 
            // ToolStripMenuGeneral
            // 
            this.ToolStripMenuGeneral.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreferencesToolStripMenuItem,
            this.toolStripSeparator14,
            this.enableTheToolbarToolStripMenuItem,
            this.bypassRedirectsToolStripMenuItem,
            this.doNotAutomaticallyDoAnythingToolStripMenuItem,
            this.toolStripComboOnLoad,
            this.ignoreNoBotsToolStripMenuItem,
            this.toolStripSeparator6,
            this.markAllAsMinorToolStripMenuItem,
            this.addAllToWatchlistToolStripMenuItem,
            this.showTimerToolStripMenuItem,
            this.toolStripSeparator7,
            this.alphaSortInterwikiLinksToolStripMenuItem,
            this.toolStripSeparator11,
            this.addIgnoredToLogFileToolStripMenuItem,
            this.summariesToolStripMenuItem,
            this.toolStripSeparator19,
            this.reloadToolStripMenuItem,
            this.showHidePanelToolStripMenuItem});
            this.ToolStripMenuGeneral.Name = "ToolStripMenuGeneral";
            this.ToolStripMenuGeneral.Size = new System.Drawing.Size(56, 20);
            this.ToolStripMenuGeneral.Text = "General";
            // 
            // PreferencesToolStripMenuItem
            // 
            this.PreferencesToolStripMenuItem.Name = "PreferencesToolStripMenuItem";
            this.PreferencesToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.PreferencesToolStripMenuItem.Text = "User and project preferences";
            this.PreferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(252, 6);
            // 
            // enableTheToolbarToolStripMenuItem
            // 
            this.enableTheToolbarToolStripMenuItem.CheckOnClick = true;
            this.enableTheToolbarToolStripMenuItem.Name = "enableTheToolbarToolStripMenuItem";
            this.enableTheToolbarToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.enableTheToolbarToolStripMenuItem.Text = "Enable the toolbar";
            this.enableTheToolbarToolStripMenuItem.Click += new System.EventHandler(this.enableTheToolbarToolStripMenuItem_Click);
            // 
            // bypassRedirectsToolStripMenuItem
            // 
            this.bypassRedirectsToolStripMenuItem.Checked = true;
            this.bypassRedirectsToolStripMenuItem.CheckOnClick = true;
            this.bypassRedirectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bypassRedirectsToolStripMenuItem.Name = "bypassRedirectsToolStripMenuItem";
            this.bypassRedirectsToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.bypassRedirectsToolStripMenuItem.Text = "Bypass redirects";
            // 
            // doNotAutomaticallyDoAnythingToolStripMenuItem
            // 
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.CheckOnClick = true;
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Name = "doNotAutomaticallyDoAnythingToolStripMenuItem";
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Text = "Do not automatically apply changes";
            // 
            // toolStripComboOnLoad
            // 
            this.toolStripComboOnLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboOnLoad.Items.AddRange(new object[] {
            "On load: Show changes",
            "On load: Show preview",
            "On load: Show edit page"});
            this.toolStripComboOnLoad.Name = "toolStripComboOnLoad";
            this.toolStripComboOnLoad.Size = new System.Drawing.Size(160, 21);
            // 
            // ignoreNoBotsToolStripMenuItem
            // 
            this.ignoreNoBotsToolStripMenuItem.CheckOnClick = true;
            this.ignoreNoBotsToolStripMenuItem.Name = "ignoreNoBotsToolStripMenuItem";
            this.ignoreNoBotsToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.ignoreNoBotsToolStripMenuItem.Text = "Ignore {{bots}} and {{nobots}}";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(252, 6);
            // 
            // markAllAsMinorToolStripMenuItem
            // 
            this.markAllAsMinorToolStripMenuItem.CheckOnClick = true;
            this.markAllAsMinorToolStripMenuItem.Name = "markAllAsMinorToolStripMenuItem";
            this.markAllAsMinorToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.markAllAsMinorToolStripMenuItem.Text = "Mark all as minor";
            // 
            // addAllToWatchlistToolStripMenuItem
            // 
            this.addAllToWatchlistToolStripMenuItem.CheckOnClick = true;
            this.addAllToWatchlistToolStripMenuItem.Name = "addAllToWatchlistToolStripMenuItem";
            this.addAllToWatchlistToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.addAllToWatchlistToolStripMenuItem.Text = "Add all to watchlist";
            // 
            // showTimerToolStripMenuItem
            // 
            this.showTimerToolStripMenuItem.CheckOnClick = true;
            this.showTimerToolStripMenuItem.Name = "showTimerToolStripMenuItem";
            this.showTimerToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.showTimerToolStripMenuItem.Text = "Show timer";
            this.showTimerToolStripMenuItem.Click += new System.EventHandler(this.showTimerToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(252, 6);
            // 
            // alphaSortInterwikiLinksToolStripMenuItem
            // 
            this.alphaSortInterwikiLinksToolStripMenuItem.Checked = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckOnClick = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alphaSortInterwikiLinksToolStripMenuItem.Name = "alphaSortInterwikiLinksToolStripMenuItem";
            this.alphaSortInterwikiLinksToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.alphaSortInterwikiLinksToolStripMenuItem.Text = "Sort interwiki link order";
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.alphaSortInterwikiLinksToolStripMenuItem_CheckStateChanged);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(252, 6);
            // 
            // addIgnoredToLogFileToolStripMenuItem
            // 
            this.addIgnoredToLogFileToolStripMenuItem.CheckOnClick = true;
            this.addIgnoredToLogFileToolStripMenuItem.Name = "addIgnoredToLogFileToolStripMenuItem";
            this.addIgnoredToLogFileToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.addIgnoredToLogFileToolStripMenuItem.Text = "Enable button to log false positives";
            this.addIgnoredToLogFileToolStripMenuItem.Click += new System.EventHandler(this.addIgnoredToLogFileToolStripMenuItem_Click);
            // 
            // summariesToolStripMenuItem
            // 
            this.summariesToolStripMenuItem.Name = "summariesToolStripMenuItem";
            this.summariesToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.summariesToolStripMenuItem.Text = "Edit summaries";
            this.summariesToolStripMenuItem.Click += new System.EventHandler(this.summariesToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(252, 6);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.reloadToolStripMenuItem.Text = "Refresh Status/Typos";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // showHidePanelToolStripMenuItem
            // 
            this.showHidePanelToolStripMenuItem.Name = "showHidePanelToolStripMenuItem";
            this.showHidePanelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.showHidePanelToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.showHidePanelToolStripMenuItem.Text = "ShowHidePanel";
            this.showHidePanelToolStripMenuItem.Visible = false;
            this.showHidePanelToolStripMenuItem.Click += new System.EventHandler(this.showHidePanelToolStripMenuItem_Click);
            // 
            // toolStripAdvanced
            // 
            this.toolStripAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeModuleToolStripMenuItem,
            this.testRegexToolStripMenuItem,
            this.toolStripSeparator21,
            this.runUpdaterToolStripMenuItem});
            this.toolStripAdvanced.Name = "toolStripAdvanced";
            this.toolStripAdvanced.Size = new System.Drawing.Size(67, 20);
            this.toolStripAdvanced.Text = "Advanced";
            // 
            // makeModuleToolStripMenuItem
            // 
            this.makeModuleToolStripMenuItem.Name = "makeModuleToolStripMenuItem";
            this.makeModuleToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.makeModuleToolStripMenuItem.Text = "Make module";
            this.makeModuleToolStripMenuItem.Click += new System.EventHandler(this.makeModuleToolStripMenuItem_Click);
            // 
            // testRegexToolStripMenuItem
            // 
            this.testRegexToolStripMenuItem.Name = "testRegexToolStripMenuItem";
            this.testRegexToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.testRegexToolStripMenuItem.Text = "Test regex";
            this.testRegexToolStripMenuItem.Click += new System.EventHandler(this.testRegexToolStripMenuItem_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(144, 6);
            // 
            // runUpdaterToolStripMenuItem
            // 
            this.runUpdaterToolStripMenuItem.Name = "runUpdaterToolStripMenuItem";
            this.runUpdaterToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.runUpdaterToolStripMenuItem.Text = "Run Updater";
            this.runUpdaterToolStripMenuItem.Click += new System.EventHandler(this.runUpdaterToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.pluginsToolStripMenuItem.Text = "Plugins";
            this.pluginsToolStripMenuItem.Visible = false;
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpHTMLToolStripMenuItem,
            this.logOutDebugToolStripMenuItem,
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // dumpHTMLToolStripMenuItem
            // 
            this.dumpHTMLToolStripMenuItem.Name = "dumpHTMLToolStripMenuItem";
            this.dumpHTMLToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.dumpHTMLToolStripMenuItem.Text = "Dump HTML (Debug)";
            this.dumpHTMLToolStripMenuItem.Visible = false;
            this.dumpHTMLToolStripMenuItem.Click += new System.EventHandler(this.dumpHTMLToolStripMenuItem_Click);
            // 
            // logOutDebugToolStripMenuItem
            // 
            this.logOutDebugToolStripMenuItem.Name = "logOutDebugToolStripMenuItem";
            this.logOutDebugToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.logOutDebugToolStripMenuItem.Text = "Log out (Debug)";
            this.logOutDebugToolStripMenuItem.Visible = false;
            this.logOutDebugToolStripMenuItem.Click += new System.EventHandler(this.logOutDebugToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(183, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.lblStatusText,
            this.lblBotTimer,
            this.lblUserName,
            this.lblProject,
            this.lblIgnoredArticles,
            this.lblEditCount,
            this.lblEditsPerMin,
            this.lblTimer});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 599);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(783, 22);
            this.statusStrip1.TabIndex = 4;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblStatusText
            // 
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(0, 17);
            this.lblStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBotTimer
            // 
            this.lblBotTimer.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
            this.lblBotTimer.Name = "lblBotTimer";
            this.lblBotTimer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBotTimer.Size = new System.Drawing.Size(0, 17);
            this.lblBotTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.Color.Red;
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUserName.Size = new System.Drawing.Size(37, 17);
            this.lblUserName.Text = "User:";
            // 
            // lblProject
            // 
            this.lblProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblProject.Name = "lblProject";
            this.lblProject.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProject.Size = new System.Drawing.Size(56, 17);
            this.lblProject.Text = "Wikipedia";
            // 
            // lblIgnoredArticles
            // 
            this.lblIgnoredArticles.Name = "lblIgnoredArticles";
            this.lblIgnoredArticles.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIgnoredArticles.Size = new System.Drawing.Size(62, 17);
            this.lblIgnoredArticles.Text = "Ignored: 0";
            // 
            // lblEditCount
            // 
            this.lblEditCount.Name = "lblEditCount";
            this.lblEditCount.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEditCount.Size = new System.Drawing.Size(47, 17);
            this.lblEditCount.Text = "Edits: 0";
            // 
            // lblEditsPerMin
            // 
            this.lblEditsPerMin.Name = "lblEditsPerMin";
            this.lblEditsPerMin.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblEditsPerMin.Size = new System.Drawing.Size(67, 17);
            this.lblEditsPerMin.Text = "Edits/min: 0";
            // 
            // lblTimer
            // 
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTimer.Size = new System.Drawing.Size(50, 17);
            this.lblTimer.Text = "Timer: 0";
            this.lblTimer.Visible = false;
            // 
            // btnIgnore
            // 
            this.btnIgnore.Enabled = false;
            this.btnIgnore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIgnore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnIgnore.Location = new System.Drawing.Point(164, 179);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(102, 32);
            this.btnIgnore.TabIndex = 14;
            this.btnIgnore.Text = "Ignore";
            this.toolTip1.SetToolTip(this.btnIgnore, "Ignore the current article and move on to the next  (Shortcut ctrl + i)");
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnDiff
            // 
            this.btnDiff.Enabled = false;
            this.btnDiff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDiff.Location = new System.Drawing.Point(164, 119);
            this.btnDiff.Name = "btnDiff";
            this.btnDiff.Size = new System.Drawing.Size(102, 23);
            this.btnDiff.TabIndex = 16;
            this.btnDiff.Text = "Show changes";
            this.toolTip1.SetToolTip(this.btnDiff, "Show/refresh the diff (Shortcut ctrl + d)");
            this.btnDiff.UseVisualStyleBackColor = true;
            this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(164, 214);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 32);
            this.btnSave.TabIndex = 10;
            this.btnSave.Tag = "Apply all the changes";
            this.btnSave.Text = "Save";
            this.toolTip1.SetToolTip(this.btnSave, "Save the changes and move on  (Shortcut ctrl + s)");
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Enabled = false;
            this.btnPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreview.Location = new System.Drawing.Point(164, 93);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(102, 23);
            this.btnPreview.TabIndex = 15;
            this.btnPreview.Text = "Preview";
            this.toolTip1.SetToolTip(this.btnPreview, "Show/refresh the preview (Shortcut ctrl + e)");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            this.btnStart.Enabled = false;
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStart.Location = new System.Drawing.Point(164, 33);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(102, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Tag = "Start the process";
            this.btnStart.Text = "Start the process (Shortcut ctrl + s)";
            this.toolTip1.SetToolTip(this.btnStart, "Start the process!");
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStop.Location = new System.Drawing.Point(217, 59);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(49, 21);
            this.btnStop.TabIndex = 28;
            this.btnStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btnStop, "Stops everything  (Shortcut escape)");
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(55, 10);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(49, 20);
            this.btnFind.TabIndex = 25;
            this.btnFind.Text = "Find";
            this.toolTip1.SetToolTip(this.btnFind, "Finds occurances in the article text (Shortcut ctrl + f)");
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnFalsePositive
            // 
            this.btnFalsePositive.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFalsePositive.Location = new System.Drawing.Point(164, 59);
            this.btnFalsePositive.Name = "btnFalsePositive";
            this.btnFalsePositive.Size = new System.Drawing.Size(49, 21);
            this.btnFalsePositive.TabIndex = 29;
            this.btnFalsePositive.Text = "False";
            this.toolTip1.SetToolTip(this.btnFalsePositive, "Add to false positives file");
            this.btnFalsePositive.UseVisualStyleBackColor = true;
            this.btnFalsePositive.Visible = false;
            this.btnFalsePositive.Click += new System.EventHandler(this.btnFalsePositive_Click);
            // 
            // txtNewCategory
            // 
            this.txtNewCategory.Enabled = false;
            this.txtNewCategory.Location = new System.Drawing.Point(153, 16);
            this.txtNewCategory.Name = "txtNewCategory";
            this.txtNewCategory.Size = new System.Drawing.Size(107, 20);
            this.txtNewCategory.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtNewCategory, "The new category, the old one is specified when you make the list");
            this.txtNewCategory.DoubleClick += new System.EventHandler(this.txtNewCategory_DoubleClick);
            this.txtNewCategory.Leave += new System.EventHandler(this.txtNewCategory_Leave);
            // 
            // txtAppendMessage
            // 
            this.txtAppendMessage.Enabled = false;
            this.txtAppendMessage.Location = new System.Drawing.Point(6, 38);
            this.txtAppendMessage.Multiline = true;
            this.txtAppendMessage.Name = "txtAppendMessage";
            this.txtAppendMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAppendMessage.Size = new System.Drawing.Size(251, 97);
            this.txtAppendMessage.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtAppendMessage, "Message, including title in wiki format");
            // 
            // chkAppend
            // 
            this.chkAppend.AutoSize = true;
            this.chkAppend.Location = new System.Drawing.Point(7, 16);
            this.chkAppend.Name = "chkAppend";
            this.chkAppend.Size = new System.Drawing.Size(65, 17);
            this.chkAppend.TabIndex = 0;
            this.chkAppend.Text = "Enabled";
            this.toolTip1.SetToolTip(this.chkAppend, "For appending a message to a user talk page for example");
            this.chkAppend.UseVisualStyleBackColor = true;
            this.chkAppend.CheckedChanged += new System.EventHandler(this.chkAppend_CheckedChanged);
            // 
            // rdoAppend
            // 
            this.rdoAppend.AutoSize = true;
            this.rdoAppend.Checked = true;
            this.rdoAppend.Enabled = false;
            this.rdoAppend.Location = new System.Drawing.Point(124, 15);
            this.rdoAppend.Name = "rdoAppend";
            this.rdoAppend.Size = new System.Drawing.Size(62, 17);
            this.rdoAppend.TabIndex = 2;
            this.rdoAppend.TabStop = true;
            this.rdoAppend.Text = "Append";
            this.toolTip1.SetToolTip(this.rdoAppend, "Add text to end of talk page");
            this.rdoAppend.UseVisualStyleBackColor = true;
            // 
            // rdoPrepend
            // 
            this.rdoPrepend.AutoSize = true;
            this.rdoPrepend.Enabled = false;
            this.rdoPrepend.Location = new System.Drawing.Point(192, 15);
            this.rdoPrepend.Name = "rdoPrepend";
            this.rdoPrepend.Size = new System.Drawing.Size(65, 17);
            this.rdoPrepend.TabIndex = 3;
            this.rdoPrepend.Text = "Prepend";
            this.toolTip1.SetToolTip(this.rdoPrepend, "Add text to beginning of talk page");
            this.rdoPrepend.UseVisualStyleBackColor = true;
            // 
            // chkSkipIfContains
            // 
            this.chkSkipIfContains.AutoSize = true;
            this.chkSkipIfContains.Location = new System.Drawing.Point(6, 19);
            this.chkSkipIfContains.Name = "chkSkipIfContains";
            this.chkSkipIfContains.Size = new System.Drawing.Size(101, 17);
            this.chkSkipIfContains.TabIndex = 22;
            this.chkSkipIfContains.Text = "Skip if contains:";
            this.toolTip1.SetToolTip(this.chkSkipIfContains, "Skip articles that contain this text");
            this.chkSkipIfContains.UseVisualStyleBackColor = true;
            this.chkSkipIfContains.CheckedChanged += new System.EventHandler(this.chkIgnoreIfContains_CheckedChanged);
            // 
            // txtSkipIfNotContains
            // 
            this.txtSkipIfNotContains.Enabled = false;
            this.txtSkipIfNotContains.Location = new System.Drawing.Point(142, 41);
            this.txtSkipIfNotContains.Name = "txtSkipIfNotContains";
            this.txtSkipIfNotContains.Size = new System.Drawing.Size(117, 20);
            this.txtSkipIfNotContains.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtSkipIfNotContains, "Skip articles that do not contain this text");
            // 
            // txtSkipIfContains
            // 
            this.txtSkipIfContains.Enabled = false;
            this.txtSkipIfContains.Location = new System.Drawing.Point(142, 17);
            this.txtSkipIfContains.Name = "txtSkipIfContains";
            this.txtSkipIfContains.Size = new System.Drawing.Size(117, 20);
            this.txtSkipIfContains.TabIndex = 23;
            this.toolTip1.SetToolTip(this.txtSkipIfContains, "Skip articles that contain this text");
            // 
            // chkSkipIfNotContains
            // 
            this.chkSkipIfNotContains.AutoSize = true;
            this.chkSkipIfNotContains.Location = new System.Drawing.Point(6, 43);
            this.chkSkipIfNotContains.Name = "chkSkipIfNotContains";
            this.chkSkipIfNotContains.Size = new System.Drawing.Size(133, 17);
            this.chkSkipIfNotContains.TabIndex = 4;
            this.chkSkipIfNotContains.Text = "Skip if doesn\'t contain:";
            this.toolTip1.SetToolTip(this.chkSkipIfNotContains, "Skip articles that do not contain this text");
            this.chkSkipIfNotContains.UseVisualStyleBackColor = true;
            this.chkSkipIfNotContains.CheckedChanged += new System.EventHandler(this.chkOnlyIfContains_CheckedChanged);
            // 
            // chkSkipIsRegex
            // 
            this.chkSkipIsRegex.AutoSize = true;
            this.chkSkipIsRegex.Location = new System.Drawing.Point(177, 66);
            this.chkSkipIsRegex.Name = "chkSkipIsRegex";
            this.chkSkipIsRegex.Size = new System.Drawing.Size(82, 17);
            this.chkSkipIsRegex.TabIndex = 28;
            this.chkSkipIsRegex.Text = "Are regexes";
            this.toolTip1.SetToolTip(this.chkSkipIsRegex, "Enables regular expressions for the \"Skip if contains\".");
            this.chkSkipIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkSkipNoChanges
            // 
            this.chkSkipNoChanges.AutoSize = true;
            this.chkSkipNoChanges.Location = new System.Drawing.Point(5, 85);
            this.chkSkipNoChanges.Name = "chkSkipNoChanges";
            this.chkSkipNoChanges.Size = new System.Drawing.Size(195, 17);
            this.chkSkipNoChanges.TabIndex = 30;
            this.chkSkipNoChanges.Text = "Skip articles when no change made";
            this.toolTip1.SetToolTip(this.chkSkipNoChanges, "Automatically skips articles when no changes were automatically made");
            this.chkSkipNoChanges.UseVisualStyleBackColor = true;
            // 
            // chkGeneralFixes
            // 
            this.chkGeneralFixes.AutoSize = true;
            this.chkGeneralFixes.Checked = true;
            this.chkGeneralFixes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGeneralFixes.Location = new System.Drawing.Point(6, 16);
            this.chkGeneralFixes.Name = "chkGeneralFixes";
            this.chkGeneralFixes.Size = new System.Drawing.Size(114, 17);
            this.chkGeneralFixes.TabIndex = 11;
            this.chkGeneralFixes.Tag = "Apply general minor fixes";
            this.chkGeneralFixes.Text = "Apply general fixes";
            this.toolTip1.SetToolTip(this.chkGeneralFixes, "Apply general minor fixes");
            this.chkGeneralFixes.UseVisualStyleBackColor = true;
            this.chkGeneralFixes.CheckedChanged += new System.EventHandler(this.chkGeneralParse_CheckedChanged);
            // 
            // chkAutoTagger
            // 
            this.chkAutoTagger.AutoSize = true;
            this.chkAutoTagger.Checked = true;
            this.chkAutoTagger.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoTagger.Location = new System.Drawing.Point(152, 16);
            this.chkAutoTagger.Name = "chkAutoTagger";
            this.chkAutoTagger.Size = new System.Drawing.Size(66, 17);
            this.chkAutoTagger.TabIndex = 27;
            this.chkAutoTagger.Text = "Auto tag";
            this.toolTip1.SetToolTip(this.chkAutoTagger, "Automatically add {{wikify}} and {{stub}} tags when appropriate");
            this.chkAutoTagger.UseVisualStyleBackColor = true;
            // 
            // chkUnicodifyWhole
            // 
            this.chkUnicodifyWhole.AutoSize = true;
            this.chkUnicodifyWhole.Checked = true;
            this.chkUnicodifyWhole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnicodifyWhole.Location = new System.Drawing.Point(6, 39);
            this.chkUnicodifyWhole.Name = "chkUnicodifyWhole";
            this.chkUnicodifyWhole.Size = new System.Drawing.Size(132, 17);
            this.chkUnicodifyWhole.TabIndex = 27;
            this.chkUnicodifyWhole.Text = "Unicodify whole article";
            this.toolTip1.SetToolTip(this.chkUnicodifyWhole, "Converts all (with a couple of exceptions) HTML and URL encoded characters to uni" +
                    "code");
            this.chkUnicodifyWhole.UseVisualStyleBackColor = true;
            // 
            // chkFindandReplace
            // 
            this.chkFindandReplace.AutoSize = true;
            this.chkFindandReplace.Location = new System.Drawing.Point(6, 19);
            this.chkFindandReplace.Name = "chkFindandReplace";
            this.chkFindandReplace.Size = new System.Drawing.Size(65, 17);
            this.chkFindandReplace.TabIndex = 6;
            this.chkFindandReplace.Text = "Enabled";
            this.toolTip1.SetToolTip(this.chkFindandReplace, "Enable text find and replace.");
            this.chkFindandReplace.UseVisualStyleBackColor = true;
            this.chkFindandReplace.CheckedChanged += new System.EventHandler(this.chkFindandReplace_CheckedChanged);
            // 
            // chkQuickSave
            // 
            this.chkQuickSave.AutoSize = true;
            this.chkQuickSave.Enabled = false;
            this.chkQuickSave.Location = new System.Drawing.Point(106, 21);
            this.chkQuickSave.Name = "chkQuickSave";
            this.chkQuickSave.Size = new System.Drawing.Size(80, 17);
            this.chkQuickSave.TabIndex = 27;
            this.chkQuickSave.Text = "Quick save";
            this.toolTip1.SetToolTip(this.chkQuickSave, "Saves without loading diff first");
            this.chkQuickSave.UseVisualStyleBackColor = true;
            // 
            // nudBotSpeed
            // 
            this.nudBotSpeed.Enabled = false;
            this.nudBotSpeed.Location = new System.Drawing.Point(46, 45);
            this.nudBotSpeed.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudBotSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBotSpeed.Name = "nudBotSpeed";
            this.nudBotSpeed.Size = new System.Drawing.Size(51, 20);
            this.nudBotSpeed.TabIndex = 23;
            this.toolTip1.SetToolTip(this.nudBotSpeed, "Time in seconds between saves");
            this.nudBotSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file|.*txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // saveXML
            // 
            this.saveXML.FileName = "settings";
            this.saveXML.Filter = "XML files|*.xml";
            this.saveXML.SupportMultiDottedExtensions = true;
            // 
            // openXML
            // 
            this.openXML.Filter = "XML files|*.xml";
            this.openXML.SupportMultiDottedExtensions = true;
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 1000;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Location = new System.Drawing.Point(0, 221);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(783, 377);
            this.panel2.TabIndex = 668;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tpSetOptions);
            this.tabControl1.Controls.Add(this.tpMoreOptions);
            this.tabControl1.Controls.Add(this.tpDab);
            this.tabControl1.Controls.Add(this.tpBots);
            this.tabControl1.Controls.Add(this.tpStart);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(219, 5);
            this.tabControl1.MinimumSize = new System.Drawing.Size(276, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(286, 374);
            this.tabControl1.TabIndex = 666;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpSetOptions
            // 
            this.tpSetOptions.Controls.Add(this.groupBox13);
            this.tpSetOptions.Controls.Add(this.groupBox1);
            this.tpSetOptions.Controls.Add(this.groupBox6);
            this.tpSetOptions.Controls.Add(this.groupBox8);
            this.tpSetOptions.Location = new System.Drawing.Point(4, 22);
            this.tpSetOptions.Name = "tpSetOptions";
            this.tpSetOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpSetOptions.Size = new System.Drawing.Size(278, 348);
            this.tpSetOptions.TabIndex = 0;
            this.tpSetOptions.Text = "(2) Set options";
            this.tpSetOptions.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.chkSkipIfNoRegexTypo);
            this.groupBox13.Controls.Add(this.linkLabel1);
            this.groupBox13.Controls.Add(this.chkRegExTypo);
            this.groupBox13.Location = new System.Drawing.Point(6, 151);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(266, 58);
            this.groupBox13.TabIndex = 31;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "RegexTypoFix";
            // 
            // chkSkipIfNoRegexTypo
            // 
            this.chkSkipIfNoRegexTypo.AutoSize = true;
            this.chkSkipIfNoRegexTypo.Enabled = false;
            this.chkSkipIfNoRegexTypo.Location = new System.Drawing.Point(6, 36);
            this.chkSkipIfNoRegexTypo.Name = "chkSkipIfNoRegexTypo";
            this.chkSkipIfNoRegexTypo.Size = new System.Drawing.Size(170, 17);
            this.chkSkipIfNoRegexTypo.TabIndex = 30;
            this.chkSkipIfNoRegexTypo.Text = "Skip article when no typo fixed";
            this.chkSkipIfNoRegexTypo.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(23, 17);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(111, 13);
            this.linkLabel1.TabIndex = 29;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Enable RegexTypoFix";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // chkRegExTypo
            // 
            this.chkRegExTypo.AutoSize = true;
            this.chkRegExTypo.Location = new System.Drawing.Point(6, 17);
            this.chkRegExTypo.Name = "chkRegExTypo";
            this.chkRegExTypo.Size = new System.Drawing.Size(15, 14);
            this.chkRegExTypo.TabIndex = 28;
            this.chkRegExTypo.UseVisualStyleBackColor = true;
            this.chkRegExTypo.CheckedChanged += new System.EventHandler(this.chkRegExTypo_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSubst);
            this.groupBox1.Controls.Add(this.chkSkipWhenNoFAR);
            this.groupBox1.Controls.Add(this.btnFindAndReplaceAdvanced);
            this.groupBox1.Controls.Add(this.btnMoreFindAndReplce);
            this.groupBox1.Controls.Add(this.chkFindandReplace);
            this.groupBox1.Location = new System.Drawing.Point(6, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 68);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find and replace";
            // 
            // btnSubst
            // 
            this.btnSubst.Enabled = false;
            this.btnSubst.Location = new System.Drawing.Point(205, 15);
            this.btnSubst.Name = "btnSubst";
            this.btnSubst.Size = new System.Drawing.Size(55, 23);
            this.btnSubst.TabIndex = 11;
            this.btnSubst.Text = "subst:";
            this.btnSubst.UseVisualStyleBackColor = true;
            this.btnSubst.Click += new System.EventHandler(this.btnSubst_Click);
            // 
            // chkSkipWhenNoFAR
            // 
            this.chkSkipWhenNoFAR.AutoSize = true;
            this.chkSkipWhenNoFAR.Checked = true;
            this.chkSkipWhenNoFAR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkipWhenNoFAR.Enabled = false;
            this.chkSkipWhenNoFAR.Location = new System.Drawing.Point(6, 43);
            this.chkSkipWhenNoFAR.Name = "chkSkipWhenNoFAR";
            this.chkSkipWhenNoFAR.Size = new System.Drawing.Size(212, 17);
            this.chkSkipWhenNoFAR.TabIndex = 10;
            this.chkSkipWhenNoFAR.Text = "Skip article when no replacement made";
            this.chkSkipWhenNoFAR.UseVisualStyleBackColor = true;
            // 
            // btnFindAndReplaceAdvanced
            // 
            this.btnFindAndReplaceAdvanced.Enabled = false;
            this.btnFindAndReplaceAdvanced.Location = new System.Drawing.Point(132, 15);
            this.btnFindAndReplaceAdvanced.Name = "btnFindAndReplaceAdvanced";
            this.btnFindAndReplaceAdvanced.Size = new System.Drawing.Size(67, 23);
            this.btnFindAndReplaceAdvanced.TabIndex = 9;
            this.btnFindAndReplaceAdvanced.Text = "Advanced";
            this.btnFindAndReplaceAdvanced.UseVisualStyleBackColor = true;
            this.btnFindAndReplaceAdvanced.Click += new System.EventHandler(this.btnFindAndReplaceAdvanced_Click);
            // 
            // btnMoreFindAndReplce
            // 
            this.btnMoreFindAndReplce.Enabled = false;
            this.btnMoreFindAndReplce.Location = new System.Drawing.Point(69, 15);
            this.btnMoreFindAndReplce.Name = "btnMoreFindAndReplce";
            this.btnMoreFindAndReplce.Size = new System.Drawing.Size(57, 23);
            this.btnMoreFindAndReplce.TabIndex = 8;
            this.btnMoreFindAndReplce.Text = "Normal";
            this.btnMoreFindAndReplce.UseVisualStyleBackColor = true;
            this.btnMoreFindAndReplce.Click += new System.EventHandler(this.btnMoreFindAndReplce_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnMoreSkip);
            this.groupBox6.Controls.Add(this.chkUnicodifyWhole);
            this.groupBox6.Controls.Add(this.chkAutoTagger);
            this.groupBox6.Controls.Add(this.chkGeneralFixes);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(266, 67);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "General";
            // 
            // btnMoreSkip
            // 
            this.btnMoreSkip.Location = new System.Drawing.Point(152, 35);
            this.btnMoreSkip.Name = "btnMoreSkip";
            this.btnMoreSkip.Size = new System.Drawing.Size(82, 23);
            this.btnMoreSkip.TabIndex = 32;
            this.btnMoreSkip.Text = "Skip options";
            this.btnMoreSkip.UseVisualStyleBackColor = true;
            this.btnMoreSkip.Click += new System.EventHandler(this.btnMoreSkip_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.chkSkipNonExistent);
            this.groupBox8.Controls.Add(this.chkSkipNoChanges);
            this.groupBox8.Controls.Add(this.chkSkipCaseSensitive);
            this.groupBox8.Controls.Add(this.chkSkipIsRegex);
            this.groupBox8.Controls.Add(this.chkSkipIfNotContains);
            this.groupBox8.Controls.Add(this.txtSkipIfContains);
            this.groupBox8.Controls.Add(this.txtSkipIfNotContains);
            this.groupBox8.Controls.Add(this.chkSkipIfContains);
            this.groupBox8.Location = new System.Drawing.Point(6, 215);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(266, 130);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "General article skip options";
            // 
            // chkSkipNonExistent
            // 
            this.chkSkipNonExistent.AutoSize = true;
            this.chkSkipNonExistent.Checked = true;
            this.chkSkipNonExistent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSkipNonExistent.Location = new System.Drawing.Point(6, 107);
            this.chkSkipNonExistent.Name = "chkSkipNonExistent";
            this.chkSkipNonExistent.Size = new System.Drawing.Size(138, 17);
            this.chkSkipNonExistent.TabIndex = 31;
            this.chkSkipNonExistent.Text = "Skip non-existing pages";
            this.chkSkipNonExistent.UseVisualStyleBackColor = true;
            // 
            // chkSkipCaseSensitive
            // 
            this.chkSkipCaseSensitive.AutoSize = true;
            this.chkSkipCaseSensitive.Location = new System.Drawing.Point(77, 66);
            this.chkSkipCaseSensitive.Name = "chkSkipCaseSensitive";
            this.chkSkipCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkSkipCaseSensitive.TabIndex = 29;
            this.chkSkipCaseSensitive.Text = "Case sensitive";
            this.chkSkipCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // tpMoreOptions
            // 
            this.tpMoreOptions.Controls.Add(this.ImageGroupBox);
            this.tpMoreOptions.Controls.Add(this.groupBox4);
            this.tpMoreOptions.Controls.Add(this.groupBox5);
            this.tpMoreOptions.Location = new System.Drawing.Point(4, 22);
            this.tpMoreOptions.Name = "tpMoreOptions";
            this.tpMoreOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpMoreOptions.Size = new System.Drawing.Size(278, 348);
            this.tpMoreOptions.TabIndex = 1;
            this.tpMoreOptions.Text = "More...";
            this.tpMoreOptions.UseVisualStyleBackColor = true;
            // 
            // ImageGroupBox
            // 
            this.ImageGroupBox.Controls.Add(this.lblImageWith);
            this.ImageGroupBox.Controls.Add(this.chkSkipNoImgChange);
            this.ImageGroupBox.Controls.Add(this.txtImageWith);
            this.ImageGroupBox.Controls.Add(this.txtImageReplace);
            this.ImageGroupBox.Controls.Add(this.cmboImages);
            this.ImageGroupBox.Location = new System.Drawing.Point(6, 153);
            this.ImageGroupBox.Name = "ImageGroupBox";
            this.ImageGroupBox.Size = new System.Drawing.Size(266, 93);
            this.ImageGroupBox.TabIndex = 27;
            this.ImageGroupBox.TabStop = false;
            this.ImageGroupBox.Text = "Images";
            // 
            // lblImageWith
            // 
            this.lblImageWith.AutoSize = true;
            this.lblImageWith.Location = new System.Drawing.Point(81, 45);
            this.lblImageWith.Name = "lblImageWith";
            this.lblImageWith.Size = new System.Drawing.Size(63, 13);
            this.lblImageWith.TabIndex = 4;
            this.lblImageWith.Text = "With image:";
            this.lblImageWith.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkSkipNoImgChange
            // 
            this.chkSkipNoImgChange.AutoSize = true;
            this.chkSkipNoImgChange.Enabled = false;
            this.chkSkipNoImgChange.Location = new System.Drawing.Point(7, 68);
            this.chkSkipNoImgChange.Name = "chkSkipNoImgChange";
            this.chkSkipNoImgChange.Size = new System.Drawing.Size(198, 17);
            this.chkSkipNoImgChange.TabIndex = 3;
            this.chkSkipNoImgChange.Text = "Skip article when no image changed";
            this.chkSkipNoImgChange.UseVisualStyleBackColor = true;
            // 
            // txtImageWith
            // 
            this.txtImageWith.Enabled = false;
            this.txtImageWith.Location = new System.Drawing.Point(150, 42);
            this.txtImageWith.Name = "txtImageWith";
            this.txtImageWith.Size = new System.Drawing.Size(107, 20);
            this.txtImageWith.TabIndex = 2;
            this.txtImageWith.Leave += new System.EventHandler(this.txtImageWith_Leave);
            // 
            // txtImageReplace
            // 
            this.txtImageReplace.Location = new System.Drawing.Point(150, 16);
            this.txtImageReplace.Name = "txtImageReplace";
            this.txtImageReplace.Size = new System.Drawing.Size(107, 20);
            this.txtImageReplace.TabIndex = 1;
            this.txtImageReplace.Leave += new System.EventHandler(this.txtImageReplace_Leave);
            // 
            // cmboImages
            // 
            this.cmboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboImages.FormattingEnabled = true;
            this.cmboImages.Items.AddRange(new object[] {
            "Choose a task...",
            "Replace image:",
            "Remove image:",
            "Comment out image:"});
            this.cmboImages.Location = new System.Drawing.Point(6, 16);
            this.cmboImages.Name = "cmboImages";
            this.cmboImages.Size = new System.Drawing.Size(138, 21);
            this.cmboImages.TabIndex = 0;
            this.cmboImages.SelectedIndexChanged += new System.EventHandler(this.cmboImages_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdoPrepend);
            this.groupBox4.Controls.Add(this.rdoAppend);
            this.groupBox4.Controls.Add(this.chkAppend);
            this.groupBox4.Controls.Add(this.txtAppendMessage);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(266, 141);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Append/Prepend text";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkSkipNoCatChange);
            this.groupBox5.Controls.Add(this.txtNewCategory2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.cmboCategorise);
            this.groupBox5.Controls.Add(this.txtNewCategory);
            this.groupBox5.Location = new System.Drawing.Point(6, 252);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(266, 90);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Categories";
            // 
            // chkSkipNoCatChange
            // 
            this.chkSkipNoCatChange.AutoSize = true;
            this.chkSkipNoCatChange.Enabled = false;
            this.chkSkipNoCatChange.Location = new System.Drawing.Point(7, 68);
            this.chkSkipNoCatChange.Name = "chkSkipNoCatChange";
            this.chkSkipNoCatChange.Size = new System.Drawing.Size(211, 17);
            this.chkSkipNoCatChange.TabIndex = 3;
            this.chkSkipNoCatChange.Text = "Skip article when no category changed";
            this.chkSkipNoCatChange.UseVisualStyleBackColor = true;
            // 
            // txtNewCategory2
            // 
            this.txtNewCategory2.Enabled = false;
            this.txtNewCategory2.Location = new System.Drawing.Point(153, 42);
            this.txtNewCategory2.Name = "txtNewCategory2";
            this.txtNewCategory2.Size = new System.Drawing.Size(107, 20);
            this.txtNewCategory2.TabIndex = 2;
            this.txtNewCategory2.Leave += new System.EventHandler(this.txtNewCategory2_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 4;
            // 
            // cmboCategorise
            // 
            this.cmboCategorise.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboCategorise.FormattingEnabled = true;
            this.cmboCategorise.Items.AddRange(new object[] {
            "Choose a task...",
            "Replace category:",
            "Add category:",
            "Remove category:"});
            this.cmboCategorise.Location = new System.Drawing.Point(7, 16);
            this.cmboCategorise.Name = "cmboCategorise";
            this.cmboCategorise.Size = new System.Drawing.Size(140, 21);
            this.cmboCategorise.TabIndex = 0;
            this.cmboCategorise.SelectedIndexChanged += new System.EventHandler(this.cmboCategorise_SelectedIndexChanged);
            // 
            // tpDab
            // 
            this.tpDab.Controls.Add(this.panelDab);
            this.tpDab.Controls.Add(this.chkEnableDab);
            this.tpDab.Location = new System.Drawing.Point(4, 22);
            this.tpDab.Name = "tpDab";
            this.tpDab.Padding = new System.Windows.Forms.Padding(3);
            this.tpDab.Size = new System.Drawing.Size(278, 348);
            this.tpDab.TabIndex = 4;
            this.tpDab.Text = "Disambig";
            this.tpDab.UseVisualStyleBackColor = true;
            // 
            // panelDab
            // 
            this.panelDab.Controls.Add(this.groupBox12);
            this.panelDab.Controls.Add(this.groupBox11);
            this.panelDab.Enabled = false;
            this.panelDab.Location = new System.Drawing.Point(6, 24);
            this.panelDab.Name = "panelDab";
            this.panelDab.Size = new System.Drawing.Size(266, 313);
            this.panelDab.TabIndex = 1;
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label5);
            this.groupBox12.Controls.Add(this.udContextChars);
            this.groupBox12.Controls.Add(this.label4);
            this.groupBox12.Controls.Add(this.chkSkipNoDab);
            this.groupBox12.Controls.Add(this.txtDabVariants);
            this.groupBox12.Location = new System.Drawing.Point(3, 50);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(260, 263);
            this.groupBox12.TabIndex = 10;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Variants";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(98, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "characters for context";
            // 
            // udContextChars
            // 
            this.udContextChars.Location = new System.Drawing.Point(48, 240);
            this.udContextChars.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udContextChars.Name = "udContextChars";
            this.udContextChars.Size = new System.Drawing.Size(44, 20);
            this.udContextChars.TabIndex = 8;
            this.udContextChars.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 242);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Use ±";
            // 
            // chkSkipNoDab
            // 
            this.chkSkipNoDab.AutoSize = true;
            this.chkSkipNoDab.Location = new System.Drawing.Point(6, 217);
            this.chkSkipNoDab.Name = "chkSkipNoDab";
            this.chkSkipNoDab.Size = new System.Drawing.Size(229, 17);
            this.chkSkipNoDab.TabIndex = 6;
            this.chkSkipNoDab.Text = "Skip article when no disambiguations made";
            this.chkSkipNoDab.UseVisualStyleBackColor = true;
            // 
            // txtDabVariants
            // 
            this.txtDabVariants.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDabVariants.Location = new System.Drawing.Point(6, 19);
            this.txtDabVariants.Multiline = true;
            this.txtDabVariants.Name = "txtDabVariants";
            this.txtDabVariants.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDabVariants.Size = new System.Drawing.Size(248, 192);
            this.txtDabVariants.TabIndex = 5;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.btnLoadLinks);
            this.groupBox11.Controls.Add(this.txtDabLink);
            this.groupBox11.Location = new System.Drawing.Point(3, 3);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(260, 41);
            this.groupBox11.TabIndex = 9;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Link to disambiguate";
            // 
            // btnLoadLinks
            // 
            this.btnLoadLinks.Enabled = false;
            this.btnLoadLinks.Location = new System.Drawing.Point(179, 13);
            this.btnLoadLinks.Name = "btnLoadLinks";
            this.btnLoadLinks.Size = new System.Drawing.Size(75, 23);
            this.btnLoadLinks.TabIndex = 1;
            this.btnLoadLinks.Text = "Load links";
            this.btnLoadLinks.UseVisualStyleBackColor = true;
            this.btnLoadLinks.Click += new System.EventHandler(this.btnLoadLinks_Click);
            // 
            // txtDabLink
            // 
            this.txtDabLink.Location = new System.Drawing.Point(6, 15);
            this.txtDabLink.Name = "txtDabLink";
            this.txtDabLink.Size = new System.Drawing.Size(167, 20);
            this.txtDabLink.TabIndex = 0;
            this.txtDabLink.Enter += new System.EventHandler(this.txtDabLink_Enter);
            this.txtDabLink.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDabLink_KeyPress);
            this.txtDabLink.TextChanged += new System.EventHandler(this.txtDabLink_TextChanged);
            // 
            // chkEnableDab
            // 
            this.chkEnableDab.AutoSize = true;
            this.chkEnableDab.Location = new System.Drawing.Point(6, 6);
            this.chkEnableDab.Name = "chkEnableDab";
            this.chkEnableDab.Size = new System.Drawing.Size(132, 17);
            this.chkEnableDab.TabIndex = 0;
            this.chkEnableDab.Text = "Enable disambiguation";
            this.chkEnableDab.UseVisualStyleBackColor = true;
            this.chkEnableDab.CheckedChanged += new System.EventHandler(this.chkEnableDab_CheckedChanged);
            // 
            // tpBots
            // 
            this.tpBots.Controls.Add(this.lblOnlyBots);
            this.tpBots.Controls.Add(this.groupBox14);
            this.tpBots.Controls.Add(this.pictureBox1);
            this.tpBots.Controls.Add(this.groupBox7);
            this.tpBots.Location = new System.Drawing.Point(4, 22);
            this.tpBots.Name = "tpBots";
            this.tpBots.Padding = new System.Windows.Forms.Padding(3);
            this.tpBots.Size = new System.Drawing.Size(278, 348);
            this.tpBots.TabIndex = 5;
            this.tpBots.Text = "Bots";
            this.tpBots.UseVisualStyleBackColor = true;
            // 
            // lblOnlyBots
            // 
            this.lblOnlyBots.BackColor = System.Drawing.Color.Transparent;
            this.lblOnlyBots.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.lblOnlyBots.Location = new System.Drawing.Point(1, 1);
            this.lblOnlyBots.Name = "lblOnlyBots";
            this.lblOnlyBots.Size = new System.Drawing.Size(276, 345);
            this.lblOnlyBots.TabIndex = 35;
            this.lblOnlyBots.Text = "Sorry, these options are only available for approved bots";
            this.lblOnlyBots.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.chkNudgeSkip);
            this.groupBox14.Controls.Add(this.btnResetNudges);
            this.groupBox14.Controls.Add(this.label3);
            this.groupBox14.Controls.Add(this.lblNudges);
            this.groupBox14.Controls.Add(this.nudNudgeTime);
            this.groupBox14.Controls.Add(this.chkNudge);
            this.groupBox14.Location = new System.Drawing.Point(9, 112);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(263, 97);
            this.groupBox14.TabIndex = 34;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "\"Nudging\"";
            // 
            // chkNudgeSkip
            // 
            this.chkNudgeSkip.AutoSize = true;
            this.chkNudgeSkip.Enabled = false;
            this.chkNudgeSkip.Location = new System.Drawing.Point(9, 72);
            this.chkNudgeSkip.Name = "chkNudgeSkip";
            this.chkNudgeSkip.Size = new System.Drawing.Size(198, 17);
            this.chkNudgeSkip.TabIndex = 35;
            this.chkNudgeSkip.Text = "Skip article if first nudge doesn\'t help";
            this.chkNudgeSkip.UseVisualStyleBackColor = true;
            // 
            // btnResetNudges
            // 
            this.btnResetNudges.Location = new System.Drawing.Point(130, 44);
            this.btnResetNudges.Name = "btnResetNudges";
            this.btnResetNudges.Size = new System.Drawing.Size(104, 22);
            this.btnResetNudges.TabIndex = 34;
            this.btnResetNudges.Text = "Reset counter";
            this.btnResetNudges.UseVisualStyleBackColor = true;
            this.btnResetNudges.Click += new System.EventHandler(this.btnResetNudges_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(183, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "minutes if stuck";
            // 
            // lblNudges
            // 
            this.lblNudges.AutoSize = true;
            this.lblNudges.Location = new System.Drawing.Point(26, 49);
            this.lblNudges.Name = "lblNudges";
            this.lblNudges.Size = new System.Drawing.Size(81, 13);
            this.lblNudges.TabIndex = 33;
            this.lblNudges.Text = "Total nudges: 0";
            // 
            // nudNudgeTime
            // 
            this.nudNudgeTime.Enabled = false;
            this.nudNudgeTime.Location = new System.Drawing.Point(146, 18);
            this.nudNudgeTime.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudNudgeTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNudgeTime.Name = "nudNudgeTime";
            this.nudNudgeTime.Size = new System.Drawing.Size(35, 20);
            this.nudNudgeTime.TabIndex = 30;
            this.nudNudgeTime.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudNudgeTime.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // chkNudge
            // 
            this.chkNudge.AutoSize = true;
            this.chkNudge.Enabled = false;
            this.chkNudge.Location = new System.Drawing.Point(9, 19);
            this.chkNudge.Name = "chkNudge";
            this.chkNudge.Size = new System.Drawing.Size(136, 17);
            this.chkNudge.TabIndex = 29;
            this.chkNudge.Text = "Resave (\"nudge\") after";
            this.chkNudge.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AutoWikiBrowser.Properties.Resources.Bot_Image;
            this.pictureBox1.Location = new System.Drawing.Point(87, 212);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 130);
            this.pictureBox1.TabIndex = 36;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.chkSuppressTag);
            this.groupBox7.Controls.Add(this.chkQuickSave);
            this.groupBox7.Controls.Add(this.chkAutoMode);
            this.groupBox7.Controls.Add(this.nudBotSpeed);
            this.groupBox7.Controls.Add(this.lblAutoDelay);
            this.groupBox7.Location = new System.Drawing.Point(9, 6);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(263, 100);
            this.groupBox7.TabIndex = 27;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Auto save";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(103, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "seconds between edits:";
            // 
            // chkSuppressTag
            // 
            this.chkSuppressTag.AutoSize = true;
            this.chkSuppressTag.Enabled = false;
            this.chkSuppressTag.Location = new System.Drawing.Point(9, 74);
            this.chkSuppressTag.Name = "chkSuppressTag";
            this.chkSuppressTag.Size = new System.Drawing.Size(211, 17);
            this.chkSuppressTag.TabIndex = 28;
            this.chkSuppressTag.Text = "Suppress \"using AWB\" in edit summary";
            this.chkSuppressTag.UseVisualStyleBackColor = true;
            // 
            // chkAutoMode
            // 
            this.chkAutoMode.AutoSize = true;
            this.chkAutoMode.Location = new System.Drawing.Point(9, 21);
            this.chkAutoMode.Name = "chkAutoMode";
            this.chkAutoMode.Size = new System.Drawing.Size(74, 17);
            this.chkAutoMode.TabIndex = 26;
            this.chkAutoMode.Text = "Auto save";
            this.chkAutoMode.UseVisualStyleBackColor = true;
            this.chkAutoMode.CheckedChanged += new System.EventHandler(this.chkAutoMode_CheckedChanged);
            // 
            // lblAutoDelay
            // 
            this.lblAutoDelay.AutoSize = true;
            this.lblAutoDelay.Enabled = false;
            this.lblAutoDelay.Location = new System.Drawing.Point(6, 47);
            this.lblAutoDelay.Name = "lblAutoDelay";
            this.lblAutoDelay.Size = new System.Drawing.Size(34, 13);
            this.lblAutoDelay.TabIndex = 25;
            this.lblAutoDelay.Text = "Delay";
            // 
            // tpStart
            // 
            this.tpStart.Controls.Add(this.lblSummary);
            this.tpStart.Controls.Add(this.chkLock);
            this.tpStart.Controls.Add(this.btnMove);
            this.tpStart.Controls.Add(this.btnDelete);
            this.tpStart.Controls.Add(this.btnFalsePositive);
            this.tpStart.Controls.Add(this.groupBox10);
            this.tpStart.Controls.Add(this.btnStop);
            this.tpStart.Controls.Add(this.cmboEditSummary);
            this.tpStart.Controls.Add(this.groupBox9);
            this.tpStart.Controls.Add(this.label6);
            this.tpStart.Controls.Add(this.groupBox3);
            this.tpStart.Controls.Add(this.btnStart);
            this.tpStart.Controls.Add(this.btnPreview);
            this.tpStart.Controls.Add(this.btnSave);
            this.tpStart.Controls.Add(this.btnDiff);
            this.tpStart.Controls.Add(this.btnIgnore);
            this.tpStart.Location = new System.Drawing.Point(4, 22);
            this.tpStart.Name = "tpStart";
            this.tpStart.Padding = new System.Windows.Forms.Padding(3);
            this.tpStart.Size = new System.Drawing.Size(278, 348);
            this.tpStart.TabIndex = 3;
            this.tpStart.Text = "(3) Start";
            this.tpStart.UseVisualStyleBackColor = true;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoEllipsis = true;
            this.lblSummary.AutoSize = true;
            this.lblSummary.BackColor = System.Drawing.Color.Transparent;
            this.lblSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummary.Location = new System.Drawing.Point(56, 9);
            this.lblSummary.MaximumSize = new System.Drawing.Size(210, 13);
            this.lblSummary.MinimumSize = new System.Drawing.Size(210, 13);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(210, 13);
            this.lblSummary.TabIndex = 33;
            this.lblSummary.Visible = false;
            // 
            // chkLock
            // 
            this.chkLock.AutoSize = true;
            this.chkLock.Location = new System.Drawing.Point(55, 33);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(94, 17);
            this.chkLock.TabIndex = 32;
            this.chkLock.Text = "Lock summary";
            this.chkLock.UseVisualStyleBackColor = true;
            this.chkLock.CheckedChanged += new System.EventHandler(this.chkLock_CheckedChanged);
            // 
            // btnMove
            // 
            this.btnMove.Enabled = false;
            this.btnMove.Location = new System.Drawing.Point(164, 154);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(49, 23);
            this.btnMove.TabIndex = 31;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Location = new System.Drawing.Point(217, 154);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(49, 23);
            this.btnDelete.TabIndex = 30;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.lblDone);
            this.groupBox10.Controls.Add(this.chkFindCaseSensitive);
            this.groupBox10.Controls.Add(this.btnFind);
            this.groupBox10.Controls.Add(this.chkFindRegex);
            this.groupBox10.Controls.Add(this.txtFind);
            this.groupBox10.Location = new System.Drawing.Point(156, 249);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(110, 96);
            this.groupBox10.TabIndex = 16;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Find";
            // 
            // lblDone
            // 
            this.lblDone.AutoSize = true;
            this.lblDone.Location = new System.Drawing.Point(6, 14);
            this.lblDone.Name = "lblDone";
            this.lblDone.Size = new System.Drawing.Size(0, 13);
            this.lblDone.TabIndex = 31;
            // 
            // chkFindCaseSensitive
            // 
            this.chkFindCaseSensitive.AutoSize = true;
            this.chkFindCaseSensitive.Location = new System.Drawing.Point(6, 75);
            this.chkFindCaseSensitive.Name = "chkFindCaseSensitive";
            this.chkFindCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkFindCaseSensitive.TabIndex = 30;
            this.chkFindCaseSensitive.Text = "Case sensitive";
            this.chkFindCaseSensitive.UseVisualStyleBackColor = true;
            this.chkFindCaseSensitive.CheckedChanged += new System.EventHandler(this.chkFindCaseSensitive_CheckedChanged);
            // 
            // chkFindRegex
            // 
            this.chkFindRegex.AutoSize = true;
            this.chkFindRegex.Location = new System.Drawing.Point(6, 56);
            this.chkFindRegex.Name = "chkFindRegex";
            this.chkFindRegex.Size = new System.Drawing.Size(63, 17);
            this.chkFindRegex.TabIndex = 29;
            this.chkFindRegex.Text = "Is regex";
            this.chkFindRegex.UseVisualStyleBackColor = true;
            this.chkFindRegex.CheckedChanged += new System.EventHandler(this.chkFindRegex_CheckedChanged);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(6, 30);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(98, 20);
            this.txtFind.TabIndex = 26;
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFind_KeyPress);
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // cmboEditSummary
            // 
            this.cmboEditSummary.DropDownHeight = 198;
            this.cmboEditSummary.DropDownWidth = 400;
            this.cmboEditSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmboEditSummary.FormattingEnabled = true;
            this.cmboEditSummary.IntegralHeight = false;
            this.cmboEditSummary.Items.AddRange(new object[] {
            "clean up",
            "re-categorisation per [[WP:CFD|CFD]]",
            "clean up and  re-categorisation per [[WP:CFD|CFD]]",
            "removing category per [[WP:CFD|CFD]]",
            "[[Wikipedia:Template substitution|subst:\'ing]]",
            "[[Wikipedia:WikiProject Stub sorting|stub sorting]]",
            "[[WP:AWB/T|Typo fixing]]",
            "bad link repair",
            "Fixing [[Wikipedia:Disambiguation pages with links|links to disambiguation pages]" +
                "]",
            "Unicodifying"});
            this.cmboEditSummary.Location = new System.Drawing.Point(55, 6);
            this.cmboEditSummary.MaxLength = 155;
            this.cmboEditSummary.Name = "cmboEditSummary";
            this.cmboEditSummary.Size = new System.Drawing.Size(210, 21);
            this.cmboEditSummary.TabIndex = 7;
            this.cmboEditSummary.Text = "clean up";
            this.cmboEditSummary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cmboEditSummary_MouseMove);
            this.cmboEditSummary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbEditSummary_KeyDown);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.btnRemove);
            this.groupBox9.Controls.Add(this.lbDuplicateWikilinks);
            this.groupBox9.Controls.Add(this.lblDuplicateWikilinks);
            this.groupBox9.Controls.Add(this.lblWarn);
            this.groupBox9.Location = new System.Drawing.Point(10, 163);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(139, 182);
            this.groupBox9.TabIndex = 24;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Alerts";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(8, 160);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(125, 22);
            this.btnRemove.TabIndex = 29;
            this.btnRemove.Text = "Remove link";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Visible = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lbDuplicateWikilinks
            // 
            this.lbDuplicateWikilinks.FormattingEnabled = true;
            this.lbDuplicateWikilinks.Location = new System.Drawing.Point(7, 89);
            this.lbDuplicateWikilinks.Name = "lbDuplicateWikilinks";
            this.lbDuplicateWikilinks.Size = new System.Drawing.Size(127, 69);
            this.lbDuplicateWikilinks.TabIndex = 27;
            this.lbDuplicateWikilinks.Visible = false;
            this.lbDuplicateWikilinks.Click += new System.EventHandler(this.lbDuplicateWikilinks_Click);
            // 
            // lblDuplicateWikilinks
            // 
            this.lblDuplicateWikilinks.AutoSize = true;
            this.lblDuplicateWikilinks.ForeColor = System.Drawing.Color.Red;
            this.lblDuplicateWikilinks.Location = new System.Drawing.Point(5, 73);
            this.lblDuplicateWikilinks.Name = "lblDuplicateWikilinks";
            this.lblDuplicateWikilinks.Size = new System.Drawing.Size(91, 13);
            this.lblDuplicateWikilinks.TabIndex = 28;
            this.lblDuplicateWikilinks.Text = "Multiple wiki-links:";
            this.lblDuplicateWikilinks.Visible = false;
            // 
            // lblWarn
            // 
            this.lblWarn.ForeColor = System.Drawing.Color.Red;
            this.lblWarn.Location = new System.Drawing.Point(6, 14);
            this.lblWarn.Name = "lblWarn";
            this.lblWarn.Size = new System.Drawing.Size(127, 59);
            this.lblWarn.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 18);
            this.label6.TabIndex = 20;
            this.label6.Text = "Summary:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblWords);
            this.groupBox3.Controls.Add(this.lblInterLinks);
            this.groupBox3.Controls.Add(this.lblCats);
            this.groupBox3.Controls.Add(this.lblImages);
            this.groupBox3.Controls.Add(this.lblLinks);
            this.groupBox3.Location = new System.Drawing.Point(10, 56);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(139, 101);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Article statistics";
            // 
            // lblWords
            // 
            this.lblWords.AutoSize = true;
            this.lblWords.Location = new System.Drawing.Point(6, 18);
            this.lblWords.Name = "lblWords";
            this.lblWords.Size = new System.Drawing.Size(41, 13);
            this.lblWords.TabIndex = 17;
            this.lblWords.Text = "Words:";
            // 
            // lblInterLinks
            // 
            this.lblInterLinks.AutoSize = true;
            this.lblInterLinks.Location = new System.Drawing.Point(5, 86);
            this.lblInterLinks.Name = "lblInterLinks";
            this.lblInterLinks.Size = new System.Drawing.Size(73, 13);
            this.lblInterLinks.TabIndex = 22;
            this.lblInterLinks.Text = "Interwiki links:";
            // 
            // lblCats
            // 
            this.lblCats.AutoSize = true;
            this.lblCats.Location = new System.Drawing.Point(5, 69);
            this.lblCats.Name = "lblCats";
            this.lblCats.Size = new System.Drawing.Size(60, 13);
            this.lblCats.TabIndex = 18;
            this.lblCats.Text = "Categories:";
            // 
            // lblImages
            // 
            this.lblImages.AutoSize = true;
            this.lblImages.Location = new System.Drawing.Point(6, 52);
            this.lblImages.Name = "lblImages";
            this.lblImages.Size = new System.Drawing.Size(44, 13);
            this.lblImages.TabIndex = 19;
            this.lblImages.Text = "Images:";
            // 
            // lblLinks
            // 
            this.lblLinks.AutoSize = true;
            this.lblLinks.Location = new System.Drawing.Point(6, 35);
            this.lblLinks.Name = "lblLinks";
            this.lblLinks.Size = new System.Drawing.Size(35, 13);
            this.lblLinks.TabIndex = 20;
            this.lblLinks.Text = "Links:";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btntsShowHide,
            this.toolStripSeparator12,
            this.btntsStart,
            this.btntsStop,
            this.toolStripSeparator18,
            this.btntsSave,
            this.btntsIgnore,
            this.toolStripSeparator15,
            this.btntsPreview,
            this.btntsChanges,
            this.toolStripSeparator16,
            this.btntsFalsePositive,
            this.lbltsNumberofItems});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(783, 25);
            this.toolStrip.TabIndex = 669;
            this.toolStrip.Text = "toolStrip1";
            this.toolStrip.Visible = false;
            // 
            // btntsShowHide
            // 
            this.btntsShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsShowHide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsShowHide.Name = "btntsShowHide";
            this.btntsShowHide.Size = new System.Drawing.Size(23, 22);
            this.btntsShowHide.Text = "Show or hide the panel";
            this.btntsShowHide.Click += new System.EventHandler(this.btnShowHide_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // btntsStart
            // 
            this.btntsStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsStart.Enabled = false;
            this.btntsStart.Image = ((System.Drawing.Image)(resources.GetObject("btntsStart.Image")));
            this.btntsStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsStart.Name = "btntsStart";
            this.btntsStart.Size = new System.Drawing.Size(23, 22);
            this.btntsStart.Text = "toolStripButton1";
            this.btntsStart.ToolTipText = "Start";
            this.btntsStart.Click += new System.EventHandler(this.btntsStart_Click);
            // 
            // btntsStop
            // 
            this.btntsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsStop.Name = "btntsStop";
            this.btntsStop.Size = new System.Drawing.Size(23, 22);
            this.btntsStop.Text = "Stop";
            this.btntsStop.Click += new System.EventHandler(this.btntsStop_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(6, 25);
            // 
            // btntsSave
            // 
            this.btntsSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsSave.Enabled = false;
            this.btntsSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsSave.Name = "btntsSave";
            this.btntsSave.Size = new System.Drawing.Size(23, 22);
            this.btntsSave.Text = "Save";
            this.btntsSave.Click += new System.EventHandler(this.btntsSave_Click);
            // 
            // btntsIgnore
            // 
            this.btntsIgnore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsIgnore.Enabled = false;
            this.btntsIgnore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsIgnore.Name = "btntsIgnore";
            this.btntsIgnore.Size = new System.Drawing.Size(23, 22);
            this.btntsIgnore.Text = "Ignore";
            this.btntsIgnore.Click += new System.EventHandler(this.btntsIgnore_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 25);
            // 
            // btntsPreview
            // 
            this.btntsPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsPreview.Enabled = false;
            this.btntsPreview.Image = ((System.Drawing.Image)(resources.GetObject("btntsPreview.Image")));
            this.btntsPreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsPreview.Name = "btntsPreview";
            this.btntsPreview.Size = new System.Drawing.Size(23, 22);
            this.btntsPreview.Text = "Show preview";
            this.btntsPreview.Click += new System.EventHandler(this.btntsPreview_Click);
            // 
            // btntsChanges
            // 
            this.btntsChanges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsChanges.Enabled = false;
            this.btntsChanges.Image = ((System.Drawing.Image)(resources.GetObject("btntsChanges.Image")));
            this.btntsChanges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsChanges.Name = "btntsChanges";
            this.btntsChanges.Size = new System.Drawing.Size(23, 22);
            this.btntsChanges.Text = "Show changes";
            this.btntsChanges.Click += new System.EventHandler(this.btntsChanges_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 25);
            // 
            // btntsFalsePositive
            // 
            this.btntsFalsePositive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsFalsePositive.Image = ((System.Drawing.Image)(resources.GetObject("btntsFalsePositive.Image")));
            this.btntsFalsePositive.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsFalsePositive.Name = "btntsFalsePositive";
            this.btntsFalsePositive.Size = new System.Drawing.Size(23, 22);
            this.btntsFalsePositive.Text = "toolStripButton1";
            this.btntsFalsePositive.ToolTipText = "Add to false positives file";
            this.btntsFalsePositive.Visible = false;
            this.btntsFalsePositive.Click += new System.EventHandler(this.tsbuttonFalsePositive_Click);
            // 
            // lbltsNumberofItems
            // 
            this.lbltsNumberofItems.Name = "lbltsNumberofItems";
            this.lbltsNumberofItems.Size = new System.Drawing.Size(49, 22);
            this.lbltsNumberofItems.Text = "Articles: ";
            // 
            // mnuWebBrowser
            // 
            this.mnuWebBrowser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
            this.mnuWebBrowser.Name = "mnuWebBrowser";
            this.mnuWebBrowser.Size = new System.Drawing.Size(150, 26);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(149, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
            // 
            // ntfyTray
            // 
            this.ntfyTray.ContextMenuStrip = this.mnuNotify;
            this.ntfyTray.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfyTray.Icon")));
            this.ntfyTray.Text = "AutoWikiBrowser";
            this.ntfyTray.Visible = true;
            this.ntfyTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ntfyTray_MouseDoubleClick);
            // 
            // mnuNotify
            // 
            this.mnuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem1});
            this.mnuNotify.Name = "mnuNotify";
            this.mnuNotify.Size = new System.Drawing.Size(112, 76);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            this.hideToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.hideToolStripMenuItem.Text = "Hide";
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(108, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(111, 22);
            this.exitToolStripMenuItem1.Text = "Exit";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // SaveTimer
            // 
            this.SaveTimer.Interval = 120000;
            this.SaveTimer.Tick += new System.EventHandler(this.SaveTimer_Tick);
            // 
            // strListFile
            // 
            this.strListFile.DefaultExt = "txt";
            this.strListFile.Filter = "Text file with wiki markup|*.txt|Plaintext list|*.txt";
            this.strListFile.Title = "Save log";
            // 
            // webBrowserEdit
            // 
            this.webBrowserEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserEdit.ArticleText = "";
            this.webBrowserEdit.Busy = false;
            this.webBrowserEdit.ContextMenuStrip = this.mnuWebBrowser;
            this.webBrowserEdit.DiffFontSize = 120;
            this.webBrowserEdit.EnhanceDiffEnabled = true;
            this.webBrowserEdit.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserEdit.Location = new System.Drawing.Point(0, 25);
            this.webBrowserEdit.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserEdit.Name = "webBrowserEdit";
            this.webBrowserEdit.ProcessStage = WikiFunctions.Browser.enumProcessStage.none;
            this.webBrowserEdit.ScriptErrorsSuppressed = true;
            this.webBrowserEdit.ScrollDown = true;
            this.webBrowserEdit.Size = new System.Drawing.Size(780, 195);
            this.webBrowserEdit.TabIndex = 670;
            this.webBrowserEdit.TabStop = false;
            this.webBrowserEdit.TimeoutLimit = 30;
            this.webBrowserEdit.WebBrowserShortcutsEnabled = false;
            this.webBrowserEdit.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserEdit_Navigating);
            this.webBrowserEdit.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserEdit_DocumentCompleted);
            // 
            // colSuccessTime
            // 
            this.colSuccessTime.Text = "Time";
            // 
            // colIgnoreTime
            // 
            this.colIgnoreTime.Text = "Time";
            this.colIgnoreTime.Width = 42;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 621);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.webBrowserEdit);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(600, 482);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AutoWikiBrowser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tpEdit.ResumeLayout(false);
            this.tpEdit.PerformLayout();
            this.mnuTextBox.ResumeLayout(false);
            this.tpLogs.ResumeLayout(false);
            this.tpLogs.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).EndInit();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tpSetOptions.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tpMoreOptions.ResumeLayout(false);
            this.ImageGroupBox.ResumeLayout(false);
            this.ImageGroupBox.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tpDab.ResumeLayout(false);
            this.tpDab.PerformLayout();
            this.panelDab.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udContextChars)).EndInit();
            this.groupBox11.ResumeLayout(false);
            this.groupBox11.PerformLayout();
            this.tpBots.ResumeLayout(false);
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNudgeTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tpStart.ResumeLayout(false);
            this.tpStart.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mnuWebBrowser.ResumeLayout(false);
            this.mnuNotify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuGeneral;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip mnuTextBox;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem insertTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wikifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speedyDeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stubToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem openPageInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bypassRedirectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListToTextFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphabeticallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem markAllAsMinorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAllToWatchlistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem birthdeathCatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metadataTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncategorisedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alphaSortInterwikiLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem showTimerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem unicodifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.SaveFileDialog saveXML;
        private System.Windows.Forms.OpenFileDialog openXML;
        private System.Windows.Forms.ToolStripStatusLabel lblBotTimer;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reparseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doNotAutomaticallyDoAnythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceTextWithLastEditToolStripMenuItem;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem pasteMoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox PasteMore1;
        private System.Windows.Forms.ToolStripTextBox PasteMore2;
        private System.Windows.Forms.ToolStripTextBox PasteMore3;
        private System.Windows.Forms.ToolStripTextBox PasteMore4;
        private System.Windows.Forms.ToolStripTextBox PasteMore5;
        private System.Windows.Forms.ToolStripTextBox PasteMore6;
        private System.Windows.Forms.ToolStripTextBox PasteMore7;
        private System.Windows.Forms.ToolStripTextBox PasteMore8;
        private System.Windows.Forms.ToolStripTextBox PasteMore9;
        private System.Windows.Forms.ToolStripTextBox PasteMore10;
        private System.Windows.Forms.TextBox txtEdit;
        private System.Windows.Forms.ToolStripMenuItem addIgnoredToLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem convertListToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removeAllExcessWhitespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDefaultSettingsToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btntsShowHide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton btntsSave;
        private System.Windows.Forms.ToolStripButton btntsIgnore;
        private System.Windows.Forms.ToolStripButton btntsStop;
        private System.Windows.Forms.ToolStripMenuItem enableTheToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem launchDumpSearcherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchListComparerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton btntsPreview;
        private System.Windows.Forms.ToolStripButton btntsChanges;
        private WikiFunctions.Browser.WebControl webBrowserEdit;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem humanNameDisambigTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripButton btntsFalsePositive;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripLabel lbltsNumberofItems;
        private System.Windows.Forms.ToolStripStatusLabel lblProject;
        private System.Windows.Forms.ToolStripStatusLabel lblUserName;
        private System.Windows.Forms.ToolStripStatusLabel lblTimer;
        private System.Windows.Forms.ToolStripButton btntsStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem dumpHTMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private WikiFunctions.Lists.ListMaker listMaker1;
        private System.Windows.Forms.ToolStripStatusLabel lblEditCount;
        private System.Windows.Forms.ToolStripStatusLabel lblIgnoredArticles;
        private System.Windows.Forms.ToolStripStatusLabel lblEditsPerMin;
        private System.Windows.Forms.ToolStripMenuItem saveAsDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripAdvanced;
        private System.Windows.Forms.ToolStripMenuItem makeModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bypassAllRedirectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOutDebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHistoryMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripComboOnLoad;
        private System.Windows.Forms.ToolStripMenuItem summariesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testRegexToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuWebBrowser;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpSetOptions;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.CheckBox chkSkipIfNoRegexTypo;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.CheckBox chkRegExTypo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSubst;
        private System.Windows.Forms.CheckBox chkSkipWhenNoFAR;
        private System.Windows.Forms.Button btnFindAndReplaceAdvanced;
        private System.Windows.Forms.Button btnMoreFindAndReplce;
        private System.Windows.Forms.CheckBox chkFindandReplace;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkUnicodifyWhole;
        private System.Windows.Forms.CheckBox chkAutoTagger;
        private System.Windows.Forms.CheckBox chkGeneralFixes;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button btnMoreSkip;
        private System.Windows.Forms.CheckBox chkSkipNonExistent;
        private System.Windows.Forms.CheckBox chkSkipNoChanges;
        private System.Windows.Forms.CheckBox chkSkipCaseSensitive;
        private System.Windows.Forms.CheckBox chkSkipIsRegex;
        private System.Windows.Forms.CheckBox chkSkipIfNotContains;
        private System.Windows.Forms.TextBox txtSkipIfContains;
        private System.Windows.Forms.TextBox txtSkipIfNotContains;
        private System.Windows.Forms.CheckBox chkSkipIfContains;
        private System.Windows.Forms.TabPage tpMoreOptions;
        private System.Windows.Forms.CheckBox chkSkipNoCatChange;
        private System.Windows.Forms.CheckBox chkSkipNoImgChange;
        private System.Windows.Forms.GroupBox ImageGroupBox;
        private System.Windows.Forms.Label lblImageWith;
        private System.Windows.Forms.TextBox txtImageWith;
        private System.Windows.Forms.TextBox txtImageReplace;
        private System.Windows.Forms.ComboBox cmboImages;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdoPrepend;
        private System.Windows.Forms.RadioButton rdoAppend;
        private System.Windows.Forms.CheckBox chkAppend;
        private System.Windows.Forms.TextBox txtAppendMessage;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtNewCategory2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboCategorise;
        private System.Windows.Forms.TextBox txtNewCategory;
        private System.Windows.Forms.TabPage tpDab;
        private System.Windows.Forms.Panel panelDab;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.CheckBox chkSkipNoDab;
        private System.Windows.Forms.TextBox txtDabVariants;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button btnLoadLinks;
        private System.Windows.Forms.TextBox txtDabLink;
        private System.Windows.Forms.CheckBox chkEnableDab;
        private System.Windows.Forms.TabPage tpStart;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnFalsePositive;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label lblDone;
        private System.Windows.Forms.CheckBox chkFindCaseSensitive;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.CheckBox chkFindRegex;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cmboEditSummary;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.ListBox lbDuplicateWikilinks;
        private System.Windows.Forms.Label lblDuplicateWikilinks;
        private System.Windows.Forms.Label lblWarn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblWords;
        private System.Windows.Forms.Label lblInterLinks;
        private System.Windows.Forms.Label lblCats;
        private System.Windows.Forms.Label lblImages;
        private System.Windows.Forms.Label lblLinks;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDiff;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.NotifyIcon ntfyTray;
        private System.Windows.Forms.ContextMenuStrip mnuNotify;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showHidePanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem humanNameCategoryKeyToolStripMenuItem;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown udContextChars;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem saveCurrentSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem openSelectionInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreNoBotsToolStripMenuItem;
        private System.Windows.Forms.Timer SaveTimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.ToolStripMenuItem runUpdaterToolStripMenuItem;
        private System.Windows.Forms.TabPage tpBots;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.NumericUpDown nudNudgeTime;
        private System.Windows.Forms.CheckBox chkNudge;
        private System.Windows.Forms.CheckBox chkSuppressTag;
        private System.Windows.Forms.CheckBox chkQuickSave;
        private System.Windows.Forms.CheckBox chkAutoMode;
        private System.Windows.Forms.NumericUpDown nudBotSpeed;
        private System.Windows.Forms.Label lblAutoDelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.Button btnResetNudges;
        private System.Windows.Forms.Label lblNudges;
        private System.Windows.Forms.Label lblOnlyBots;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tpEdit;
        private System.Windows.Forms.TabPage tpLogs;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnClearIgnored;
        private System.Windows.Forms.Button btnSaveIgnored;
        private System.Windows.Forms.Button btnClearSaved;
        private System.Windows.Forms.Button btnSaveSaved;
        private System.Windows.Forms.SaveFileDialog strListFile;
        private System.Windows.Forms.CheckBox chkNudgeSkip;
        private System.Windows.Forms.Button btnAddToList;
        private WikiFunctions.NoFlickerListView lvSaved;
        private System.Windows.Forms.ColumnHeader colSuccessSave;
        private WikiFunctions.NoFlickerListView lvIgnored;
        private System.Windows.Forms.ColumnHeader colIgnoreArticle;
        private System.Windows.Forms.ColumnHeader colSkippedBy;
        private System.Windows.Forms.ColumnHeader colSkipReason;
        private System.Windows.Forms.ColumnHeader colIgnoreTime;
        private System.Windows.Forms.ColumnHeader colSuccessTime;


    }
}


