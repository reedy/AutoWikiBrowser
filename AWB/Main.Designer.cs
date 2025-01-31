/*
    Autowikibrowser
    Copyright (C) 2007 Martin Richards

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
            AutoWikiBrowser.Properties.Settings settings1 = new AutoWikiBrowser.Properties.Settings();
            this.mnuTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.wordWrapToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore1 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore2 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore3 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore4 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore5 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore6 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore7 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore8 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore9 = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMore10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.saveTextToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.birthdeathCatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.categoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.humanNameCategoryKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.humanNameDisambigTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wikifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speedyDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disambiguationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.commentSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openPageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTalkPageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHistoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.openSelectionInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceTextWithLastEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoAllChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDefaultSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.profilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableTheToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHidePanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enlargeEditAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHideEditToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.displayfalsePositivesButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphabeticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDuplicatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterOutNonMainSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
            this.convertToTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertFromTalkPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator30 = new System.Windows.Forms.ToolStripSeparator();
            this.specialFilterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToTextFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearCurrentListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.managePluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMenuGeneral = new System.Windows.Forms.ToolStripMenuItem();
            this.PreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.summariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preParseModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.followRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticallyDoAnythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator31 = new System.Windows.Forms.ToolStripSeparator();
            this.focusAtEndOfEditTextBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noSectionEditSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restrictDefaultsortChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restrictOrphanTaggingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noMOSComplianceFixesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syntaxHighlightEditBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highlightAllFindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scrollToAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.markAllAsMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToWatchList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.alphaSortInterwikiLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceReferenceTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.makeModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.externalProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator32 = new System.Windows.Forms.ToolStripSeparator();
            this.cEvalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.testRegexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchDumpSearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchListComparerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchListSplitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.resetEditSkippedCountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.submitStatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
            this.profileTyposToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invalidateCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.UsageStatsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runUpdaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusMain = new System.Windows.Forms.StatusStrip();
            this.MainFormProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBotTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserNotifications = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblNewArticles = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblIgnoredArticles = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEditCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblEditsPerMin = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPagesPerMin = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolTip = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnDiff = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnFalsePositive = new System.Windows.Forms.Button();
            this.nudBotSpeed = new System.Windows.Forms.NumericUpDown();
            this.txtNewCategory = new System.Windows.Forms.TextBox();
            this.txtAppendMessage = new WikiFunctions.Controls.RichTextBoxInvoke();
            this.chkAppend = new System.Windows.Forms.CheckBox();
            this.rdoAppend = new System.Windows.Forms.RadioButton();
            this.rdoPrepend = new System.Windows.Forms.RadioButton();
            this.chkGeneralFixes = new System.Windows.Forms.CheckBox();
            this.chkAutoTagger = new System.Windows.Forms.CheckBox();
            this.chkUnicodifyWhole = new System.Windows.Forms.CheckBox();
            this.chkFindandReplace = new System.Windows.Forms.CheckBox();
            this.chkNudge = new System.Windows.Forms.CheckBox();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.chkMinor = new System.Windows.Forms.CheckBox();
            this.chkShutdown = new System.Windows.Forms.CheckBox();
            this.btnProtect = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSubst = new System.Windows.Forms.Button();
            this.btnWatch = new System.Windows.Forms.Button();
            this.btnLoadLinks = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnFindAndReplaceAdvanced = new System.Windows.Forms.Button();
            this.btnMoreFindAndReplce = new System.Windows.Forms.Button();
            this.chkSkipNoChanges = new System.Windows.Forms.CheckBox();
            this.chkSkipSpamFilter = new System.Windows.Forms.CheckBox();
            this.chkSkipIfInuse = new System.Windows.Forms.CheckBox();
            this.chkSkipWhitespace = new System.Windows.Forms.CheckBox();
            this.chkSkipGeneralFixes = new System.Windows.Forms.CheckBox();
            this.chkSkipMinorGeneralFixes = new System.Windows.Forms.CheckBox();
            this.chkSkipCasing = new System.Windows.Forms.CheckBox();
            this.chkSkipIfRedirect = new System.Windows.Forms.CheckBox();
            this.chkSkipIfNoAlerts = new System.Windows.Forms.CheckBox();
            this.chkSkipNoPageLinks = new System.Windows.Forms.CheckBox();
            this.radSkipExistent = new System.Windows.Forms.RadioButton();
            this.radSkipNonExistent = new System.Windows.Forms.RadioButton();
            this.radSkipNone = new System.Windows.Forms.RadioButton();
            this.chkNudgeSkip = new System.Windows.Forms.CheckBox();
            this.btnResetNudges = new System.Windows.Forms.Button();
            this.lblNudges = new System.Windows.Forms.Label();
            this.chkSuppressTag = new System.Windows.Forms.CheckBox();
            this.chkAutoMode = new System.Windows.Forms.CheckBox();
            this.lblAutoDelay = new System.Windows.Forms.Label();
            this.lblbotEditsStop = new System.Windows.Forms.Label();
            this.chkFindCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkFindRegex = new System.Windows.Forms.CheckBox();
            this.txtFind = new System.Windows.Forms.RichTextBox();
            this.chkSkipNoCatChange = new System.Windows.Forms.CheckBox();
            this.chkRemoveSortKey = new System.Windows.Forms.CheckBox();
            this.chkSkipOnlyMinorFaR = new System.Windows.Forms.CheckBox();
            this.chkSkipWhenNoFAR = new System.Windows.Forms.CheckBox();
            this.AlertGroup = new System.Windows.Forms.GroupBox();
            this.lbAlerts = new System.Windows.Forms.ListBox();
            this.lbDuplicateWikilinks = new System.Windows.Forms.ListBox();
            this.lblDuplicateWikilinks = new System.Windows.Forms.Label();
            this.SummaryGroup = new System.Windows.Forms.GroupBox();
            this.lblWords = new System.Windows.Forms.Label();
            this.lblInterLinks = new System.Windows.Forms.Label();
            this.lblDates = new System.Windows.Forms.Label();
            this.lblCats = new System.Windows.Forms.Label();
            this.lblImages = new System.Windows.Forms.Label();
            this.lblLinks = new System.Windows.Forms.Label();
            this.chkSkipIfNoRegexTypo = new System.Windows.Forms.CheckBox();
            this.chkSkipNoImgChange = new System.Windows.Forms.CheckBox();
            this.chkSkipCosmetic = new System.Windows.Forms.CheckBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.txtReviewEditSummary = new System.Windows.Forms.RichTextBox();
            this.imgBold = new System.Windows.Forms.PictureBox();
            this.imgItalics = new System.Windows.Forms.PictureBox();
            this.imgLink = new System.Windows.Forms.PictureBox();
            this.imgExtlink = new System.Windows.Forms.PictureBox();
            this.imgMath = new System.Windows.Forms.PictureBox();
            this.imgNowiki = new System.Windows.Forms.PictureBox();
            this.imgHr = new System.Windows.Forms.PictureBox();
            this.imgRedirect = new System.Windows.Forms.PictureBox();
            this.imgStrike = new System.Windows.Forms.PictureBox();
            this.imgSup = new System.Windows.Forms.PictureBox();
            this.imgSub = new System.Windows.Forms.PictureBox();
            this.imgComment = new System.Windows.Forms.PictureBox();
            this.EnableRegexTypoFixLinkLabel = new System.Windows.Forms.LinkLabel();
            this.chkRegExTypo = new System.Windows.Forms.CheckBox();
            this.chkSkipNoDab = new System.Windows.Forms.CheckBox();
            this.chkAppendMetaDataSort = new System.Windows.Forms.CheckBox();
            this.botEditsStop = new System.Windows.Forms.NumericUpDown();
            this.mnuHistory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshHistoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveXML = new System.Windows.Forms.SaveFileDialog();
            this.openXML = new System.Windows.Forms.OpenFileDialog();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btntsShowHide = new System.Windows.Forms.ToolStripButton();
            this.btntsShowHideParameters = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsStart = new System.Windows.Forms.ToolStripButton();
            this.btntsStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsPreview = new System.Windows.Forms.ToolStripButton();
            this.btntsChanges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsSave = new System.Windows.Forms.ToolStripButton();
            this.btntsIgnore = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsFalsePositive = new System.Windows.Forms.ToolStripButton();
            this.lbltsNumberofItems = new System.Windows.Forms.ToolStripLabel();
            this.ntfyTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.EditBoxSaveTimer = new System.Windows.Forms.Timer(this.components);
            this.MainTab = new System.Windows.Forms.TabControl();
            this.tpOptions = new System.Windows.Forms.TabPage();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnMoreSkip = new System.Windows.Forms.Button();
            this.tpMoreOptions = new System.Windows.Forms.TabPage();
            this.ImageGroupBox = new System.Windows.Forms.GroupBox();
            this.lblImageWith = new System.Windows.Forms.Label();
            this.txtImageWith = new System.Windows.Forms.TextBox();
            this.txtImageReplace = new System.Windows.Forms.TextBox();
            this.cmboImages = new WikiFunctions.Controls.ComboBoxInvoke();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblNewlineCharacters = new System.Windows.Forms.Label();
            this.lblUse = new System.Windows.Forms.Label();
            this.udNewlineChars = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtNewCategory2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmboCategorise = new WikiFunctions.Controls.ComboBoxInvoke();
            this.tpDab = new System.Windows.Forms.TabPage();
            this.panelDab = new System.Windows.Forms.Panel();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.udContextChars = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDabVariants = new System.Windows.Forms.TextBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.txtDabLink = new System.Windows.Forms.TextBox();
            this.chkEnableDab = new System.Windows.Forms.CheckBox();
            this.tpSkip = new System.Windows.Forms.TabPage();
            this.gbRegexSkip = new System.Windows.Forms.GroupBox();
            this.skipIfNotContains = new WikiFunctions.Controls.PageNotContainsControl();
            this.skipIfContains = new WikiFunctions.Controls.PageContainsControl();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.gbPageExisting = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tpStart = new System.Windows.Forms.TabPage();
            this.SummaryLabel = new System.Windows.Forms.Label();
            this.cmboEditSummary = new System.Windows.Forms.ComboBox();
            this.findGroup = new System.Windows.Forms.GroupBox();
            this.tpBots = new System.Windows.Forms.TabPage();
            this.groupBox16 = new System.Windows.Forms.GroupBox();
            this.radStandby = new System.Windows.Forms.RadioButton();
            this.radHibernate = new System.Windows.Forms.RadioButton();
            this.radRestart = new System.Windows.Forms.RadioButton();
            this.radShutdown = new System.Windows.Forms.RadioButton();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BotImage = new System.Windows.Forms.PictureBox();
            this.lblOnlyBots = new System.Windows.Forms.Label();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.ShutdownTimer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.listMaker = new WikiFunctions.Controls.Lists.ListMaker();
            this.EditBoxTab = new System.Windows.Forms.TabControl();
            this.tpEdit = new System.Windows.Forms.TabPage();
            this.txtEdit = new WikiFunctions.Controls.ArticleTextBox();
            this.tpHistory = new System.Windows.Forms.TabPage();
            this.webBrowserHistory = new AutoWikiBrowser.AWBWebBrowser();
            this.tpLinks = new System.Windows.Forms.TabPage();
            this.webBrowserLinks = new AutoWikiBrowser.AWBWebBrowser();
            this.tpLogs = new System.Windows.Forms.TabPage();
            this.logControl = new WikiFunctions.Logging.LogControl();
            this.tpArticleActionLogs = new System.Windows.Forms.TabPage();
            this.articleActionLogControl1 = new WikiFunctions.Logging.ArticleActionLogControl();
            this.tpTypos = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.CurrentTypoStats = new WikiFunctions.Controls.TypoStatsControl();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.lblTypoRatio = new System.Windows.Forms.Label();
            this.OverallTypoStats = new WikiFunctions.Controls.TypoStatsControl();
            this.lblNoChange = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblOverallTypos = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.mnuMakeFromTextBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemMakeFromTextBoxUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemMakeFromTextBoxCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMakeFromTextBoxPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparatorMakeFromTextBox = new System.Windows.Forms.ToolStripSeparator();
            this.webBrowser = new AutoWikiBrowser.AWBWebBrowser();
            this.NudgeTimer = new AutoWikiBrowser.NudgeTimer(this.components);
            this.mnuTextBox.SuspendLayout();
            this.MnuMain.SuspendLayout();
            this.StatusMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).BeginInit();
            this.AlertGroup.SuspendLayout();
            this.SummaryGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgItalics)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgLink)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgExtlink)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNowiki)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRedirect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStrike)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSub)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgComment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.botEditsStop)).BeginInit();
            this.mnuHistory.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.mnuNotify.SuspendLayout();
            this.MainTab.SuspendLayout();
            this.tpOptions.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tpMoreOptions.SuspendLayout();
            this.ImageGroupBox.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udNewlineChars)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.tpDab.SuspendLayout();
            this.panelDab.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udContextChars)).BeginInit();
            this.groupBox11.SuspendLayout();
            this.tpSkip.SuspendLayout();
            this.gbRegexSkip.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.gbPageExisting.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tpStart.SuspendLayout();
            this.findGroup.SuspendLayout();
            this.tpBots.SuspendLayout();
            this.groupBox16.SuspendLayout();
            this.groupBox14.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BotImage)).BeginInit();
            this.panel1.SuspendLayout();
            this.EditBoxTab.SuspendLayout();
            this.tpEdit.SuspendLayout();
            this.tpHistory.SuspendLayout();
            this.tpLinks.SuspendLayout();
            this.tpLogs.SuspendLayout();
            this.tpArticleActionLogs.SuspendLayout();
            this.tpTypos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.mnuMakeFromTextBox.SuspendLayout();
            this.SuspendLayout();
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
            this.toolStripSeparator22,
            this.saveTextToFileToolStripMenuItem,
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
            this.commentSelectedToolStripMenuItem,
            this.toolStripSeparator4,
            this.openPageInBrowserToolStripMenuItem,
            this.openTalkPageInBrowserToolStripMenuItem,
            this.openHistoryMenuItem,
            this.toolStripSeparator20,
            this.openSelectionInBrowserToolStripMenuItem,
            this.toolStripSeparator9,
            this.replaceTextWithLastEditToolStripMenuItem,
            this.undoAllChangesToolStripMenuItem});
            this.mnuTextBox.Name = "contextMenuStrip1";
            resources.ApplyResources(this.mnuTextBox, "mnuTextBox");
            this.mnuTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.mnuTextBox_Opening);
            // 
            // wordWrapToolStripMenuItem1
            // 
            this.wordWrapToolStripMenuItem1.Checked = true;
            this.wordWrapToolStripMenuItem1.CheckOnClick = true;
            this.wordWrapToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wordWrapToolStripMenuItem1.Name = "wordWrapToolStripMenuItem1";
            resources.ApplyResources(this.wordWrapToolStripMenuItem1, "wordWrapToolStripMenuItem1");
            this.wordWrapToolStripMenuItem1.Click += new System.EventHandler(this.wordWrapToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
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
            this.PasteMore10,
            this.toolStripSeparator19,
            this.configureToolStripMenuItem});
            this.pasteMoreToolStripMenuItem.Name = "pasteMoreToolStripMenuItem";
            resources.ApplyResources(this.pasteMoreToolStripMenuItem, "pasteMoreToolStripMenuItem");
            // 
            // PasteMore1
            // 
            this.PasteMore1.Name = "PasteMore1";
            resources.ApplyResources(this.PasteMore1, "PasteMore1");
            this.PasteMore1.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore2
            // 
            this.PasteMore2.Name = "PasteMore2";
            resources.ApplyResources(this.PasteMore2, "PasteMore2");
            this.PasteMore2.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore3
            // 
            this.PasteMore3.Name = "PasteMore3";
            resources.ApplyResources(this.PasteMore3, "PasteMore3");
            this.PasteMore3.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore4
            // 
            this.PasteMore4.Name = "PasteMore4";
            resources.ApplyResources(this.PasteMore4, "PasteMore4");
            this.PasteMore4.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore5
            // 
            this.PasteMore5.Name = "PasteMore5";
            resources.ApplyResources(this.PasteMore5, "PasteMore5");
            this.PasteMore5.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore6
            // 
            this.PasteMore6.Name = "PasteMore6";
            resources.ApplyResources(this.PasteMore6, "PasteMore6");
            this.PasteMore6.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore7
            // 
            this.PasteMore7.Name = "PasteMore7";
            resources.ApplyResources(this.PasteMore7, "PasteMore7");
            this.PasteMore7.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore8
            // 
            this.PasteMore8.Name = "PasteMore8";
            resources.ApplyResources(this.PasteMore8, "PasteMore8");
            this.PasteMore8.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore9
            // 
            this.PasteMore9.Name = "PasteMore9";
            resources.ApplyResources(this.PasteMore9, "PasteMore9");
            this.PasteMore9.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // PasteMore10
            // 
            this.PasteMore10.Name = "PasteMore10";
            resources.ApplyResources(this.PasteMore10, "PasteMore10");
            this.PasteMore10.Click += new System.EventHandler(this.PasteMore_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            resources.ApplyResources(this.configureToolStripMenuItem, "configureToolStripMenuItem");
            this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // saveTextToFileToolStripMenuItem
            // 
            this.saveTextToFileToolStripMenuItem.Name = "saveTextToFileToolStripMenuItem";
            resources.ApplyResources(this.saveTextToFileToolStripMenuItem, "saveTextToFileToolStripMenuItem");
            this.saveTextToFileToolStripMenuItem.Click += new System.EventHandler(this.saveTextToFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            resources.ApplyResources(this.goToLineToolStripMenuItem, "goToLineToolStripMenuItem");
            // 
            // toolStripTextBox2
            // 
            resources.ApplyResources(this.toolStripTextBox2, "toolStripTextBox2");
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBox2_KeyPress);
            this.toolStripTextBox2.Click += new System.EventHandler(this.toolStripTextBox2_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.birthdeathCatsToolStripMenuItem,
            this.toolStripSeparator24,
            this.categoryToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            resources.ApplyResources(this.insertToolStripMenuItem, "insertToolStripMenuItem");
            // 
            // birthdeathCatsToolStripMenuItem
            // 
            this.birthdeathCatsToolStripMenuItem.Name = "birthdeathCatsToolStripMenuItem";
            resources.ApplyResources(this.birthdeathCatsToolStripMenuItem, "birthdeathCatsToolStripMenuItem");
            this.birthdeathCatsToolStripMenuItem.Click += new System.EventHandler(this.birthdeathCatsToolStripMenuItem_Click);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // categoryToolStripMenuItem
            // 
            this.categoryToolStripMenuItem.Name = "categoryToolStripMenuItem";
            resources.ApplyResources(this.categoryToolStripMenuItem, "categoryToolStripMenuItem");
            this.categoryToolStripMenuItem.Click += new System.EventHandler(this.categoryToolStripMenuItem_Click);
            // 
            // insertTagToolStripMenuItem
            // 
            this.insertTagToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.humanNameCategoryKeyToolStripMenuItem,
            this.humanNameDisambigTagToolStripMenuItem,
            this.wikifyToolStripMenuItem,
            this.cleanupToolStripMenuItem,
            this.speedyDeleteToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.disambiguationToolStripMenuItem,
            this.uncategorisedToolStripMenuItem,
            this.stubToolStripMenuItem,
            this.toolStripTextBox1});
            this.insertTagToolStripMenuItem.Name = "insertTagToolStripMenuItem";
            resources.ApplyResources(this.insertTagToolStripMenuItem, "insertTagToolStripMenuItem");
            // 
            // humanNameCategoryKeyToolStripMenuItem
            // 
            this.humanNameCategoryKeyToolStripMenuItem.Name = "humanNameCategoryKeyToolStripMenuItem";
            resources.ApplyResources(this.humanNameCategoryKeyToolStripMenuItem, "humanNameCategoryKeyToolStripMenuItem");
            this.humanNameCategoryKeyToolStripMenuItem.Click += new System.EventHandler(this.humanNameCategoryKeyToolStripMenuItem_Click);
            // 
            // humanNameDisambigTagToolStripMenuItem
            // 
            this.humanNameDisambigTagToolStripMenuItem.Name = "humanNameDisambigTagToolStripMenuItem";
            resources.ApplyResources(this.humanNameDisambigTagToolStripMenuItem, "humanNameDisambigTagToolStripMenuItem");
            this.humanNameDisambigTagToolStripMenuItem.Click += new System.EventHandler(this.humanNameDisambigTagToolStripMenuItem_Click);
            // 
            // wikifyToolStripMenuItem
            // 
            this.wikifyToolStripMenuItem.Name = "wikifyToolStripMenuItem";
            resources.ApplyResources(this.wikifyToolStripMenuItem, "wikifyToolStripMenuItem");
            this.wikifyToolStripMenuItem.Click += new System.EventHandler(this.wikifyToolStripMenuItem_Click);
            // 
            // cleanupToolStripMenuItem
            // 
            this.cleanupToolStripMenuItem.Name = "cleanupToolStripMenuItem";
            resources.ApplyResources(this.cleanupToolStripMenuItem, "cleanupToolStripMenuItem");
            this.cleanupToolStripMenuItem.Click += new System.EventHandler(this.cleanupToolStripMenuItem_Click);
            // 
            // speedyDeleteToolStripMenuItem
            // 
            this.speedyDeleteToolStripMenuItem.Name = "speedyDeleteToolStripMenuItem";
            resources.ApplyResources(this.speedyDeleteToolStripMenuItem, "speedyDeleteToolStripMenuItem");
            this.speedyDeleteToolStripMenuItem.Click += new System.EventHandler(this.speedyDeleteToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // disambiguationToolStripMenuItem
            // 
            this.disambiguationToolStripMenuItem.Name = "disambiguationToolStripMenuItem";
            resources.ApplyResources(this.disambiguationToolStripMenuItem, "disambiguationToolStripMenuItem");
            this.disambiguationToolStripMenuItem.Click += new System.EventHandler(this.disambiguationToolStripMenuItem_Click);
            // 
            // uncategorisedToolStripMenuItem
            // 
            this.uncategorisedToolStripMenuItem.Name = "uncategorisedToolStripMenuItem";
            resources.ApplyResources(this.uncategorisedToolStripMenuItem, "uncategorisedToolStripMenuItem");
            this.uncategorisedToolStripMenuItem.Click += new System.EventHandler(this.uncategorisedToolStripMenuItem_Click);
            // 
            // stubToolStripMenuItem
            // 
            this.stubToolStripMenuItem.Name = "stubToolStripMenuItem";
            resources.ApplyResources(this.stubToolStripMenuItem, "stubToolStripMenuItem");
            this.stubToolStripMenuItem.Click += new System.EventHandler(this.stubToolStripMenuItem_Click);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            resources.ApplyResources(this.toolStripTextBox1, "toolStripTextBox1");
            // 
            // convertListToToolStripMenuItem
            // 
            this.convertListToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listToolStripMenuItem,
            this.listToolStripMenuItem1});
            this.convertListToToolStripMenuItem.Name = "convertListToToolStripMenuItem";
            resources.ApplyResources(this.convertListToToolStripMenuItem, "convertListToToolStripMenuItem");
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            resources.ApplyResources(this.listToolStripMenuItem, "listToolStripMenuItem");
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem1
            // 
            this.listToolStripMenuItem1.Name = "listToolStripMenuItem1";
            resources.ApplyResources(this.listToolStripMenuItem1, "listToolStripMenuItem1");
            this.listToolStripMenuItem1.Click += new System.EventHandler(this.listToolStripMenuItem1_Click);
            // 
            // unicodifyToolStripMenuItem
            // 
            this.unicodifyToolStripMenuItem.Name = "unicodifyToolStripMenuItem";
            resources.ApplyResources(this.unicodifyToolStripMenuItem, "unicodifyToolStripMenuItem");
            this.unicodifyToolStripMenuItem.Click += new System.EventHandler(this.unicodifyToolStripMenuItem_Click);
            // 
            // bypassAllRedirectsToolStripMenuItem
            // 
            resources.ApplyResources(this.bypassAllRedirectsToolStripMenuItem, "bypassAllRedirectsToolStripMenuItem");
            this.bypassAllRedirectsToolStripMenuItem.Name = "bypassAllRedirectsToolStripMenuItem";
            this.bypassAllRedirectsToolStripMenuItem.Click += new System.EventHandler(this.bypassAllRedirectsToolStripMenuItem_Click);
            // 
            // removeAllExcessWhitespaceToolStripMenuItem
            // 
            this.removeAllExcessWhitespaceToolStripMenuItem.Name = "removeAllExcessWhitespaceToolStripMenuItem";
            resources.ApplyResources(this.removeAllExcessWhitespaceToolStripMenuItem, "removeAllExcessWhitespaceToolStripMenuItem");
            this.removeAllExcessWhitespaceToolStripMenuItem.Click += new System.EventHandler(this.removeAllExcessWhitespaceToolStripMenuItem_Click);
            // 
            // reparseToolStripMenuItem
            // 
            this.reparseToolStripMenuItem.Name = "reparseToolStripMenuItem";
            resources.ApplyResources(this.reparseToolStripMenuItem, "reparseToolStripMenuItem");
            this.reparseToolStripMenuItem.Click += new System.EventHandler(this.reparseToolStripMenuItem_Click);
            // 
            // commentSelectedToolStripMenuItem
            // 
            this.commentSelectedToolStripMenuItem.Name = "commentSelectedToolStripMenuItem";
            resources.ApplyResources(this.commentSelectedToolStripMenuItem, "commentSelectedToolStripMenuItem");
            this.commentSelectedToolStripMenuItem.Click += new System.EventHandler(this.imgComment_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // openPageInBrowserToolStripMenuItem
            // 
            this.openPageInBrowserToolStripMenuItem.Name = "openPageInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openPageInBrowserToolStripMenuItem, "openPageInBrowserToolStripMenuItem");
            this.openPageInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPageInBrowserToolStripMenuItem_Click);
            // 
            // openTalkPageInBrowserToolStripMenuItem
            // 
            this.openTalkPageInBrowserToolStripMenuItem.Name = "openTalkPageInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openTalkPageInBrowserToolStripMenuItem, "openTalkPageInBrowserToolStripMenuItem");
            this.openTalkPageInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openTalkPageInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryMenuItem
            // 
            this.openHistoryMenuItem.Name = "openHistoryMenuItem";
            resources.ApplyResources(this.openHistoryMenuItem, "openHistoryMenuItem");
            this.openHistoryMenuItem.Click += new System.EventHandler(this.openHistoryMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // openSelectionInBrowserToolStripMenuItem
            // 
            this.openSelectionInBrowserToolStripMenuItem.Name = "openSelectionInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openSelectionInBrowserToolStripMenuItem, "openSelectionInBrowserToolStripMenuItem");
            this.openSelectionInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openSelectionInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // replaceTextWithLastEditToolStripMenuItem
            // 
            this.replaceTextWithLastEditToolStripMenuItem.Name = "replaceTextWithLastEditToolStripMenuItem";
            resources.ApplyResources(this.replaceTextWithLastEditToolStripMenuItem, "replaceTextWithLastEditToolStripMenuItem");
            this.replaceTextWithLastEditToolStripMenuItem.Click += new System.EventHandler(this.replaceTextWithLastEditToolStripMenuItem_Click);
            // 
            // undoAllChangesToolStripMenuItem
            // 
            this.undoAllChangesToolStripMenuItem.Name = "undoAllChangesToolStripMenuItem";
            resources.ApplyResources(this.undoAllChangesToolStripMenuItem, "undoAllChangesToolStripMenuItem");
            this.undoAllChangesToolStripMenuItem.Click += new System.EventHandler(this.undoAllChangesToolStripMenuItem_Click);
            // 
            // MnuMain
            // 
            this.MnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.generalToolStripMenuItem,
            this.pluginsToolStripMenuItem,
            this.ToolStripMenuGeneral,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.MnuMain, "MnuMain");
            this.MnuMain.Name = "MnuMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadDefaultSettingsToolStripMenuItem,
            this.loadSettingsToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator2,
            this.saveSettingsToolStripMenuItem,
            this.saveSettingsAsToolStripMenuItem,
            this.saveAsDefaultToolStripMenuItem,
            this.toolStripSeparator8,
            this.profilesToolStripMenuItem,
            this.logOutToolStripMenuItem,
            this.reloadToolStripMenuItem,
            this.toolStripSeparator17,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // loadDefaultSettingsToolStripMenuItem
            // 
            this.loadDefaultSettingsToolStripMenuItem.Name = "loadDefaultSettingsToolStripMenuItem";
            resources.ApplyResources(this.loadDefaultSettingsToolStripMenuItem, "loadDefaultSettingsToolStripMenuItem");
            this.loadDefaultSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadDefaultSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            resources.ApplyResources(this.loadSettingsToolStripMenuItem, "loadSettingsToolStripMenuItem");
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            resources.ApplyResources(this.recentToolStripMenuItem, "recentToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            resources.ApplyResources(this.saveSettingsToolStripMenuItem, "saveSettingsToolStripMenuItem");
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // saveSettingsAsToolStripMenuItem
            // 
            this.saveSettingsAsToolStripMenuItem.Name = "saveSettingsAsToolStripMenuItem";
            resources.ApplyResources(this.saveSettingsAsToolStripMenuItem, "saveSettingsAsToolStripMenuItem");
            this.saveSettingsAsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsAsToolStripMenuItem_Click);
            // 
            // saveAsDefaultToolStripMenuItem
            // 
            this.saveAsDefaultToolStripMenuItem.Name = "saveAsDefaultToolStripMenuItem";
            resources.ApplyResources(this.saveAsDefaultToolStripMenuItem, "saveAsDefaultToolStripMenuItem");
            this.saveAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.saveAsDefaultToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // profilesToolStripMenuItem
            // 
            this.profilesToolStripMenuItem.Name = "profilesToolStripMenuItem";
            resources.ApplyResources(this.profilesToolStripMenuItem, "profilesToolStripMenuItem");
            this.profilesToolStripMenuItem.Click += new System.EventHandler(this.profilesToolStripMenuItem_Click);
            // 
            // logOutToolStripMenuItem
            // 
            this.logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
            resources.ApplyResources(this.logOutToolStripMenuItem, "logOutToolStripMenuItem");
            this.logOutToolStripMenuItem.Click += new System.EventHandler(this.logOutToolStripMenuItem_Click);
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            resources.ApplyResources(this.reloadToolStripMenuItem, "reloadToolStripMenuItem");
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableTheToolbarToolStripMenuItem,
            this.showHidePanelToolStripMenuItem,
            this.enlargeEditAreaToolStripMenuItem,
            this.showHideEditToolbarToolStripMenuItem,
            this.toolStripSeparator11,
            this.displayfalsePositivesButtonToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // enableTheToolbarToolStripMenuItem
            // 
            this.enableTheToolbarToolStripMenuItem.CheckOnClick = true;
            this.enableTheToolbarToolStripMenuItem.Name = "enableTheToolbarToolStripMenuItem";
            resources.ApplyResources(this.enableTheToolbarToolStripMenuItem, "enableTheToolbarToolStripMenuItem");
            this.enableTheToolbarToolStripMenuItem.Click += new System.EventHandler(this.enableTheToolbarToolStripMenuItem_Click);
            // 
            // showHidePanelToolStripMenuItem
            // 
            this.showHidePanelToolStripMenuItem.Checked = true;
            this.showHidePanelToolStripMenuItem.CheckOnClick = true;
            this.showHidePanelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showHidePanelToolStripMenuItem.Name = "showHidePanelToolStripMenuItem";
            resources.ApplyResources(this.showHidePanelToolStripMenuItem, "showHidePanelToolStripMenuItem");
            this.showHidePanelToolStripMenuItem.Click += new System.EventHandler(this.showHidePanelToolStripMenuItem_Click);
            // 
            // enlargeEditAreaToolStripMenuItem
            // 
            this.enlargeEditAreaToolStripMenuItem.Name = "enlargeEditAreaToolStripMenuItem";
            resources.ApplyResources(this.enlargeEditAreaToolStripMenuItem, "enlargeEditAreaToolStripMenuItem");
            this.enlargeEditAreaToolStripMenuItem.Click += new System.EventHandler(this.enlargeEditAreaToolStripMenuItem_Click);
            // 
            // showHideEditToolbarToolStripMenuItem
            // 
            this.showHideEditToolbarToolStripMenuItem.Checked = true;
            this.showHideEditToolbarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showHideEditToolbarToolStripMenuItem.Name = "showHideEditToolbarToolStripMenuItem";
            resources.ApplyResources(this.showHideEditToolbarToolStripMenuItem, "showHideEditToolbarToolStripMenuItem");
            this.showHideEditToolbarToolStripMenuItem.Click += new System.EventHandler(this.showHideEditToolbarToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // displayfalsePositivesButtonToolStripMenuItem
            // 
            this.displayfalsePositivesButtonToolStripMenuItem.CheckOnClick = true;
            this.displayfalsePositivesButtonToolStripMenuItem.Name = "displayfalsePositivesButtonToolStripMenuItem";
            resources.ApplyResources(this.displayfalsePositivesButtonToolStripMenuItem, "displayfalsePositivesButtonToolStripMenuItem");
            this.displayfalsePositivesButtonToolStripMenuItem.Click += new System.EventHandler(this.displayfalsePositivesButtonToolStripMenuItem_Click);
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortAlphabeticallyToolStripMenuItem,
            this.removeDuplicatesToolStripMenuItem,
            this.filterOutNonMainSpaceToolStripMenuItem,
            this.toolStripSeparator28,
            this.convertToTalkPagesToolStripMenuItem,
            this.convertFromTalkPagesToolStripMenuItem,
            this.toolStripSeparator30,
            this.specialFilterToolStripMenuItem1,
            this.saveListToTextFileToolStripMenuItem,
            this.clearCurrentListToolStripMenuItem});
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            resources.ApplyResources(this.generalToolStripMenuItem, "generalToolStripMenuItem");
            // 
            // sortAlphabeticallyToolStripMenuItem
            // 
            this.sortAlphabeticallyToolStripMenuItem.CheckOnClick = true;
            this.sortAlphabeticallyToolStripMenuItem.Name = "sortAlphabeticallyToolStripMenuItem";
            resources.ApplyResources(this.sortAlphabeticallyToolStripMenuItem, "sortAlphabeticallyToolStripMenuItem");
            this.sortAlphabeticallyToolStripMenuItem.Click += new System.EventHandler(this.sortAlphabeticallyToolStripMenuItem_Click);
            // 
            // removeDuplicatesToolStripMenuItem
            // 
            this.removeDuplicatesToolStripMenuItem.CheckOnClick = true;
            this.removeDuplicatesToolStripMenuItem.Name = "removeDuplicatesToolStripMenuItem";
            resources.ApplyResources(this.removeDuplicatesToolStripMenuItem, "removeDuplicatesToolStripMenuItem");
            this.removeDuplicatesToolStripMenuItem.Click += new System.EventHandler(this.removeDuplicatesToolStripMenuItem_Click);
            // 
            // filterOutNonMainSpaceToolStripMenuItem
            // 
            this.filterOutNonMainSpaceToolStripMenuItem.CheckOnClick = true;
            this.filterOutNonMainSpaceToolStripMenuItem.Name = "filterOutNonMainSpaceToolStripMenuItem";
            resources.ApplyResources(this.filterOutNonMainSpaceToolStripMenuItem, "filterOutNonMainSpaceToolStripMenuItem");
            this.filterOutNonMainSpaceToolStripMenuItem.CheckedChanged += new System.EventHandler(this.filterOutNonMainSpaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator28
            // 
            this.toolStripSeparator28.Name = "toolStripSeparator28";
            resources.ApplyResources(this.toolStripSeparator28, "toolStripSeparator28");
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            resources.ApplyResources(this.convertToTalkPagesToolStripMenuItem, "convertToTalkPagesToolStripMenuItem");
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            resources.ApplyResources(this.convertFromTalkPagesToolStripMenuItem, "convertFromTalkPagesToolStripMenuItem");
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator30
            // 
            this.toolStripSeparator30.Name = "toolStripSeparator30";
            resources.ApplyResources(this.toolStripSeparator30, "toolStripSeparator30");
            // 
            // specialFilterToolStripMenuItem1
            // 
            this.specialFilterToolStripMenuItem1.Name = "specialFilterToolStripMenuItem1";
            resources.ApplyResources(this.specialFilterToolStripMenuItem1, "specialFilterToolStripMenuItem1");
            this.specialFilterToolStripMenuItem1.Click += new System.EventHandler(this.specialFilterToolStripMenuItem_Click);
            // 
            // saveListToTextFileToolStripMenuItem
            // 
            this.saveListToTextFileToolStripMenuItem.Name = "saveListToTextFileToolStripMenuItem";
            resources.ApplyResources(this.saveListToTextFileToolStripMenuItem, "saveListToTextFileToolStripMenuItem");
            this.saveListToTextFileToolStripMenuItem.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem_Click);
            // 
            // clearCurrentListToolStripMenuItem
            // 
            this.clearCurrentListToolStripMenuItem.Name = "clearCurrentListToolStripMenuItem";
            resources.ApplyResources(this.clearCurrentListToolStripMenuItem, "clearCurrentListToolStripMenuItem");
            this.clearCurrentListToolStripMenuItem.Click += new System.EventHandler(this.clearCurrentListToolStripMenuItem_Click);
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPluginToolStripMenuItem,
            this.managePluginsToolStripMenuItem,
            this.toolStripSeparator26});
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            resources.ApplyResources(this.pluginsToolStripMenuItem, "pluginsToolStripMenuItem");
            // 
            // loadPluginToolStripMenuItem
            // 
            this.loadPluginToolStripMenuItem.Name = "loadPluginToolStripMenuItem";
            resources.ApplyResources(this.loadPluginToolStripMenuItem, "loadPluginToolStripMenuItem");
            this.loadPluginToolStripMenuItem.Click += new System.EventHandler(this.loadPluginToolStripMenuItem_Click);
            // 
            // managePluginsToolStripMenuItem
            // 
            this.managePluginsToolStripMenuItem.Name = "managePluginsToolStripMenuItem";
            resources.ApplyResources(this.managePluginsToolStripMenuItem, "managePluginsToolStripMenuItem");
            this.managePluginsToolStripMenuItem.Click += new System.EventHandler(this.managePluginsToolStripMenuItem_Click);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(this.toolStripSeparator26, "toolStripSeparator26");
            // 
            // ToolStripMenuGeneral
            // 
            this.ToolStripMenuGeneral.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PreferencesToolStripMenuItem,
            this.summariesToolStripMenuItem,
            this.autoSaveSettingsToolStripMenuItem,
            this.preParseModeToolStripMenuItem,
            this.toolStripSeparator14,
            this.followRedirectsToolStripMenuItem,
            this.automaticallyDoAnythingToolStripMenuItem,
            this.toolStripSeparator31,
            this.focusAtEndOfEditTextBoxToolStripMenuItem,
            this.noSectionEditSummaryToolStripMenuItem,
            this.restrictDefaultsortChangesToolStripMenuItem,
            this.restrictOrphanTaggingToolStripMenuItem,
            this.noMOSComplianceFixesToolStripMenuItem,
            this.syntaxHighlightEditBoxToolStripMenuItem,
            this.highlightAllFindToolStripMenuItem,
            this.scrollToAlertsToolStripMenuItem,
            this.toolStripSeparator6,
            this.markAllAsMinorToolStripMenuItem,
            this.addToWatchList,
            this.toolStripSeparator7,
            this.alphaSortInterwikiLinksToolStripMenuItem,
            this.replaceReferenceTagsToolStripMenuItem});
            this.ToolStripMenuGeneral.Name = "ToolStripMenuGeneral";
            resources.ApplyResources(this.ToolStripMenuGeneral, "ToolStripMenuGeneral");
            // 
            // PreferencesToolStripMenuItem
            // 
            this.PreferencesToolStripMenuItem.Name = "PreferencesToolStripMenuItem";
            resources.ApplyResources(this.PreferencesToolStripMenuItem, "PreferencesToolStripMenuItem");
            this.PreferencesToolStripMenuItem.Click += new System.EventHandler(this.PreferencesToolStripMenuItem_Click);
            // 
            // summariesToolStripMenuItem
            // 
            this.summariesToolStripMenuItem.Name = "summariesToolStripMenuItem";
            resources.ApplyResources(this.summariesToolStripMenuItem, "summariesToolStripMenuItem");
            this.summariesToolStripMenuItem.Click += new System.EventHandler(this.summariesToolStripMenuItem_Click);
            // 
            // autoSaveSettingsToolStripMenuItem
            // 
            this.autoSaveSettingsToolStripMenuItem.CheckOnClick = true;
            this.autoSaveSettingsToolStripMenuItem.Name = "autoSaveSettingsToolStripMenuItem";
            resources.ApplyResources(this.autoSaveSettingsToolStripMenuItem, "autoSaveSettingsToolStripMenuItem");
            // 
            // preParseModeToolStripMenuItem
            // 
            this.preParseModeToolStripMenuItem.CheckOnClick = true;
            this.preParseModeToolStripMenuItem.Name = "preParseModeToolStripMenuItem";
            resources.ApplyResources(this.preParseModeToolStripMenuItem, "preParseModeToolStripMenuItem");
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // followRedirectsToolStripMenuItem
            // 
            this.followRedirectsToolStripMenuItem.Checked = true;
            this.followRedirectsToolStripMenuItem.CheckOnClick = true;
            this.followRedirectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.followRedirectsToolStripMenuItem.Name = "followRedirectsToolStripMenuItem";
            resources.ApplyResources(this.followRedirectsToolStripMenuItem, "followRedirectsToolStripMenuItem");
            // 
            // automaticallyDoAnythingToolStripMenuItem
            // 
            this.automaticallyDoAnythingToolStripMenuItem.Checked = true;
            this.automaticallyDoAnythingToolStripMenuItem.CheckOnClick = true;
            this.automaticallyDoAnythingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.automaticallyDoAnythingToolStripMenuItem.Name = "automaticallyDoAnythingToolStripMenuItem";
            resources.ApplyResources(this.automaticallyDoAnythingToolStripMenuItem, "automaticallyDoAnythingToolStripMenuItem");
            // 
            // toolStripSeparator31
            // 
            this.toolStripSeparator31.Name = "toolStripSeparator31";
            resources.ApplyResources(this.toolStripSeparator31, "toolStripSeparator31");
            // 
            // focusAtEndOfEditTextBoxToolStripMenuItem
            // 
            this.focusAtEndOfEditTextBoxToolStripMenuItem.CheckOnClick = true;
            this.focusAtEndOfEditTextBoxToolStripMenuItem.Name = "focusAtEndOfEditTextBoxToolStripMenuItem";
            resources.ApplyResources(this.focusAtEndOfEditTextBoxToolStripMenuItem, "focusAtEndOfEditTextBoxToolStripMenuItem");
            // 
            // noSectionEditSummaryToolStripMenuItem
            // 
            this.noSectionEditSummaryToolStripMenuItem.CheckOnClick = true;
            this.noSectionEditSummaryToolStripMenuItem.Name = "noSectionEditSummaryToolStripMenuItem";
            resources.ApplyResources(this.noSectionEditSummaryToolStripMenuItem, "noSectionEditSummaryToolStripMenuItem");
            // 
            // restrictDefaultsortChangesToolStripMenuItem
            // 
            this.restrictDefaultsortChangesToolStripMenuItem.CheckOnClick = true;
            this.restrictDefaultsortChangesToolStripMenuItem.Name = "restrictDefaultsortChangesToolStripMenuItem";
            resources.ApplyResources(this.restrictDefaultsortChangesToolStripMenuItem, "restrictDefaultsortChangesToolStripMenuItem");
            // 
            // restrictOrphanTaggingToolStripMenuItem
            // 
            this.restrictOrphanTaggingToolStripMenuItem.CheckOnClick = true;
            this.restrictOrphanTaggingToolStripMenuItem.Name = "restrictOrphanTaggingToolStripMenuItem";
            resources.ApplyResources(this.restrictOrphanTaggingToolStripMenuItem, "restrictOrphanTaggingToolStripMenuItem");
            // 
            // noMOSComplianceFixesToolStripMenuItem
            // 
            this.noMOSComplianceFixesToolStripMenuItem.CheckOnClick = true;
            this.noMOSComplianceFixesToolStripMenuItem.Name = "noMOSComplianceFixesToolStripMenuItem";
            resources.ApplyResources(this.noMOSComplianceFixesToolStripMenuItem, "noMOSComplianceFixesToolStripMenuItem");
            // 
            // syntaxHighlightEditBoxToolStripMenuItem
            // 
            this.syntaxHighlightEditBoxToolStripMenuItem.CheckOnClick = true;
            this.syntaxHighlightEditBoxToolStripMenuItem.Name = "syntaxHighlightEditBoxToolStripMenuItem";
            resources.ApplyResources(this.syntaxHighlightEditBoxToolStripMenuItem, "syntaxHighlightEditBoxToolStripMenuItem");
            // 
            // highlightAllFindToolStripMenuItem
            // 
            this.highlightAllFindToolStripMenuItem.CheckOnClick = true;
            this.highlightAllFindToolStripMenuItem.Name = "highlightAllFindToolStripMenuItem";
            resources.ApplyResources(this.highlightAllFindToolStripMenuItem, "highlightAllFindToolStripMenuItem");
            // 
            // scrollToAlertsToolStripMenuItem
            // 
            this.scrollToAlertsToolStripMenuItem.CheckOnClick = true;
            this.scrollToAlertsToolStripMenuItem.Name = "scrollToAlertsToolStripMenuItem";
            resources.ApplyResources(this.scrollToAlertsToolStripMenuItem, "scrollToAlertsToolStripMenuItem");
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // markAllAsMinorToolStripMenuItem
            // 
            this.markAllAsMinorToolStripMenuItem.CheckOnClick = true;
            this.markAllAsMinorToolStripMenuItem.Name = "markAllAsMinorToolStripMenuItem";
            resources.ApplyResources(this.markAllAsMinorToolStripMenuItem, "markAllAsMinorToolStripMenuItem");
            this.markAllAsMinorToolStripMenuItem.Click += new System.EventHandler(this.markAllAsMinorToolStripMenuItem_Click);
            // 
            // addToWatchList
            // 
            this.addToWatchList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.addToWatchList.DropDownWidth = 160;
            this.addToWatchList.Items.AddRange(new object[] {
            resources.GetString("addToWatchList.Items"),
            resources.GetString("addToWatchList.Items1"),
            resources.GetString("addToWatchList.Items2"),
            resources.GetString("addToWatchList.Items3")});
            this.addToWatchList.Name = "addToWatchList";
            resources.ApplyResources(this.addToWatchList, "addToWatchList");
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // alphaSortInterwikiLinksToolStripMenuItem
            // 
            this.alphaSortInterwikiLinksToolStripMenuItem.Checked = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckOnClick = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alphaSortInterwikiLinksToolStripMenuItem.Name = "alphaSortInterwikiLinksToolStripMenuItem";
            resources.ApplyResources(this.alphaSortInterwikiLinksToolStripMenuItem, "alphaSortInterwikiLinksToolStripMenuItem");
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.alphaSortInterwikiLinksToolStripMenuItem_CheckStateChanged);
            // 
            // replaceReferenceTagsToolStripMenuItem
            // 
            this.replaceReferenceTagsToolStripMenuItem.Checked = true;
            this.replaceReferenceTagsToolStripMenuItem.CheckOnClick = true;
            this.replaceReferenceTagsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.replaceReferenceTagsToolStripMenuItem.Name = "replaceReferenceTagsToolStripMenuItem";
            resources.ApplyResources(this.replaceReferenceTagsToolStripMenuItem, "replaceReferenceTagsToolStripMenuItem");
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.makeModuleToolStripMenuItem,
            this.externalProcessingToolStripMenuItem,
            this.toolStripSeparator32,
            this.cEvalToolStripMenuItem,
            this.toolStripSeparator13,
            this.testRegexToolStripMenuItem,
            this.launchDumpSearcherToolStripMenuItem,
            this.launchListComparerToolStripMenuItem,
            this.launchListSplitterToolStripMenuItem,
            this.toolStripSeparator21,
            this.resetEditSkippedCountToolStripMenuItem,
            this.submitStatToolStripMenuItem,
            this.toolStripSeparator29,
            this.profileTyposToolStripMenuItem,
            this.invalidateCacheToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // makeModuleToolStripMenuItem
            // 
            this.makeModuleToolStripMenuItem.Name = "makeModuleToolStripMenuItem";
            resources.ApplyResources(this.makeModuleToolStripMenuItem, "makeModuleToolStripMenuItem");
            this.makeModuleToolStripMenuItem.Click += new System.EventHandler(this.makeModuleToolStripMenuItem_Click);
            // 
            // externalProcessingToolStripMenuItem
            // 
            this.externalProcessingToolStripMenuItem.Name = "externalProcessingToolStripMenuItem";
            resources.ApplyResources(this.externalProcessingToolStripMenuItem, "externalProcessingToolStripMenuItem");
            this.externalProcessingToolStripMenuItem.Click += new System.EventHandler(this.externalProcessingToolStripMenuItem_Click);
            // 
            // toolStripSeparator32
            // 
            this.toolStripSeparator32.Name = "toolStripSeparator32";
            resources.ApplyResources(this.toolStripSeparator32, "toolStripSeparator32");
            // 
            // cEvalToolStripMenuItem
            // 
            this.cEvalToolStripMenuItem.Name = "cEvalToolStripMenuItem";
            resources.ApplyResources(this.cEvalToolStripMenuItem, "cEvalToolStripMenuItem");
            this.cEvalToolStripMenuItem.Click += new System.EventHandler(this.cEvalToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // testRegexToolStripMenuItem
            // 
            this.testRegexToolStripMenuItem.Name = "testRegexToolStripMenuItem";
            resources.ApplyResources(this.testRegexToolStripMenuItem, "testRegexToolStripMenuItem");
            this.testRegexToolStripMenuItem.Click += new System.EventHandler(this.launchRegexTester);
            // 
            // launchDumpSearcherToolStripMenuItem
            // 
            this.launchDumpSearcherToolStripMenuItem.Name = "launchDumpSearcherToolStripMenuItem";
            resources.ApplyResources(this.launchDumpSearcherToolStripMenuItem, "launchDumpSearcherToolStripMenuItem");
            this.launchDumpSearcherToolStripMenuItem.Click += new System.EventHandler(this.launchDumpSearcherToolStripMenuItem_Click);
            // 
            // launchListComparerToolStripMenuItem
            // 
            this.launchListComparerToolStripMenuItem.Name = "launchListComparerToolStripMenuItem";
            resources.ApplyResources(this.launchListComparerToolStripMenuItem, "launchListComparerToolStripMenuItem");
            this.launchListComparerToolStripMenuItem.Click += new System.EventHandler(this.launchListComparerToolStripMenuItem_Click);
            // 
            // launchListSplitterToolStripMenuItem
            // 
            this.launchListSplitterToolStripMenuItem.Name = "launchListSplitterToolStripMenuItem";
            resources.ApplyResources(this.launchListSplitterToolStripMenuItem, "launchListSplitterToolStripMenuItem");
            this.launchListSplitterToolStripMenuItem.Click += new System.EventHandler(this.launchListSplitterToolStripMenuItem_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            // 
            // resetEditSkippedCountToolStripMenuItem
            // 
            this.resetEditSkippedCountToolStripMenuItem.Name = "resetEditSkippedCountToolStripMenuItem";
            resources.ApplyResources(this.resetEditSkippedCountToolStripMenuItem, "resetEditSkippedCountToolStripMenuItem");
            this.resetEditSkippedCountToolStripMenuItem.Click += new System.EventHandler(this.resetEditSkippedCountToolStripMenuItem_Click);
            // 
            // submitStatToolStripMenuItem
            // 
            this.submitStatToolStripMenuItem.Name = "submitStatToolStripMenuItem";
            resources.ApplyResources(this.submitStatToolStripMenuItem, "submitStatToolStripMenuItem");
            this.submitStatToolStripMenuItem.Click += new System.EventHandler(this.submitStatToolStripMenuItem_Click);
            // 
            // toolStripSeparator29
            // 
            this.toolStripSeparator29.Name = "toolStripSeparator29";
            resources.ApplyResources(this.toolStripSeparator29, "toolStripSeparator29");
            // 
            // profileTyposToolStripMenuItem
            // 
            this.profileTyposToolStripMenuItem.Name = "profileTyposToolStripMenuItem";
            resources.ApplyResources(this.profileTyposToolStripMenuItem, "profileTyposToolStripMenuItem");
            this.profileTyposToolStripMenuItem.Click += new System.EventHandler(this.profileTyposToolStripMenuItem_Click);
            // 
            // invalidateCacheToolStripMenuItem
            // 
            this.invalidateCacheToolStripMenuItem.Name = "invalidateCacheToolStripMenuItem";
            resources.ApplyResources(this.invalidateCacheToolStripMenuItem, "invalidateCacheToolStripMenuItem");
            this.invalidateCacheToolStripMenuItem.Click += new System.EventHandler(this.invalidateCacheToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.UsageStatsMenuItem,
            this.runUpdaterToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            resources.ApplyResources(this.helpToolStripMenuItem1, "helpToolStripMenuItem1");
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // UsageStatsMenuItem
            // 
            this.UsageStatsMenuItem.Name = "UsageStatsMenuItem";
            resources.ApplyResources(this.UsageStatsMenuItem, "UsageStatsMenuItem");
            this.UsageStatsMenuItem.Click += new System.EventHandler(this.UsageStatsMenuItem_Click);
            // 
            // runUpdaterToolStripMenuItem
            // 
            this.runUpdaterToolStripMenuItem.Name = "runUpdaterToolStripMenuItem";
            resources.ApplyResources(this.runUpdaterToolStripMenuItem, "runUpdaterToolStripMenuItem");
            this.runUpdaterToolStripMenuItem.Click += new System.EventHandler(this.runUpdaterToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // StatusMain
            // 
            this.StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainFormProgressBar,
            this.lblStatusText,
            this.lblBotTimer,
            this.lblUserNotifications,
            this.lblUserName,
            this.lblProject,
            this.lblNewArticles,
            this.lblIgnoredArticles,
            this.lblEditCount,
            this.lblEditsPerMin,
            this.lblPagesPerMin,
            this.lblTimer});
            this.StatusMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            resources.ApplyResources(this.StatusMain, "StatusMain");
            this.StatusMain.Name = "StatusMain";
            this.StatusMain.ShowItemToolTips = true;
            // 
            // MainFormProgressBar
            // 
            resources.ApplyResources(this.MainFormProgressBar, "MainFormProgressBar");
            this.MainFormProgressBar.Name = "MainFormProgressBar";
            this.MainFormProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblStatusText
            // 
            this.lblStatusText.Name = "lblStatusText";
            resources.ApplyResources(this.lblStatusText, "lblStatusText");
            // 
            // lblBotTimer
            // 
            this.lblBotTimer.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
            this.lblBotTimer.Name = "lblBotTimer";
            resources.ApplyResources(this.lblBotTimer, "lblBotTimer");
            // 
            // lblUserNotifications
            // 
            this.lblUserNotifications.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblUserNotifications.BackColor = System.Drawing.Color.Gray;
            this.lblUserNotifications.ForeColor = System.Drawing.Color.White;
            this.lblUserNotifications.Name = "lblUserNotifications";
            this.lblUserNotifications.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblUserNotifications, "lblUserNotifications");
            this.lblUserNotifications.Click += new System.EventHandler(this.lblUserNotifications_Click);
            this.lblUserNotifications.MouseHover += new System.EventHandler(this.statusBar_MouseHover);
            // 
            // lblUserName
            // 
            this.lblUserName.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblUserName.BackColor = System.Drawing.Color.Red;
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.Click += new System.EventHandler(this.lblUserName_Click);
            this.lblUserName.MouseHover += new System.EventHandler(this.statusBar_MouseHover);
            // 
            // lblProject
            // 
            this.lblProject.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblProject.Name = "lblProject";
            this.lblProject.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblProject, "lblProject");
            this.lblProject.Click += new System.EventHandler(this.lblProject_Click);
            this.lblProject.MouseHover += new System.EventHandler(this.statusBar_MouseHover);
            // 
            // lblNewArticles
            // 
            this.lblNewArticles.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblNewArticles.Name = "lblNewArticles";
            resources.ApplyResources(this.lblNewArticles, "lblNewArticles");
            // 
            // lblIgnoredArticles
            // 
            this.lblIgnoredArticles.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblIgnoredArticles.Name = "lblIgnoredArticles";
            this.lblIgnoredArticles.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblIgnoredArticles, "lblIgnoredArticles");
            // 
            // lblEditCount
            // 
            this.lblEditCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblEditCount.Name = "lblEditCount";
            this.lblEditCount.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblEditCount, "lblEditCount");
            // 
            // lblEditsPerMin
            // 
            this.lblEditsPerMin.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblEditsPerMin.Name = "lblEditsPerMin";
            this.lblEditsPerMin.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblEditsPerMin, "lblEditsPerMin");
            // 
            // lblPagesPerMin
            // 
            this.lblPagesPerMin.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblPagesPerMin.Name = "lblPagesPerMin";
            this.lblPagesPerMin.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblPagesPerMin, "lblPagesPerMin");
            // 
            // lblTimer
            // 
            this.lblTimer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            resources.ApplyResources(this.lblTimer, "lblTimer");
            // 
            // btnIgnore
            // 
            resources.ApplyResources(this.btnIgnore, "btnIgnore");
            this.btnIgnore.Name = "btnIgnore";
            this.ToolTip.SetToolTip(this.btnIgnore, resources.GetString("btnIgnore.ToolTip"));
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnDiff
            // 
            resources.ApplyResources(this.btnDiff, "btnDiff");
            this.btnDiff.Name = "btnDiff";
            this.ToolTip.SetToolTip(this.btnDiff, resources.GetString("btnDiff.ToolTip"));
            this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.ToolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPreview
            // 
            resources.ApplyResources(this.btnPreview, "btnPreview");
            this.btnPreview.Name = "btnPreview";
            this.ToolTip.SetToolTip(this.btnPreview, resources.GetString("btnPreview.ToolTip"));
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.ToolTip.SetToolTip(this.btnStart, resources.GetString("btnStart.ToolTip"));
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Transparent;
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Name = "btnStop";
            this.ToolTip.SetToolTip(this.btnStop, resources.GetString("btnStop.ToolTip"));
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnFind
            // 
            resources.ApplyResources(this.btnFind, "btnFind");
            this.btnFind.Name = "btnFind";
            this.ToolTip.SetToolTip(this.btnFind, resources.GetString("btnFind.ToolTip"));
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnFalsePositive
            // 
            resources.ApplyResources(this.btnFalsePositive, "btnFalsePositive");
            this.btnFalsePositive.Name = "btnFalsePositive";
            this.ToolTip.SetToolTip(this.btnFalsePositive, resources.GetString("btnFalsePositive.ToolTip"));
            this.btnFalsePositive.UseVisualStyleBackColor = true;
            this.btnFalsePositive.Click += new System.EventHandler(this.FalsePositiveClick);
            // 
            // nudBotSpeed
            // 
            resources.ApplyResources(this.nudBotSpeed, "nudBotSpeed");
            this.nudBotSpeed.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudBotSpeed.Name = "nudBotSpeed";
            this.ToolTip.SetToolTip(this.nudBotSpeed, resources.GetString("nudBotSpeed.ToolTip"));
            this.nudBotSpeed.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // txtNewCategory
            // 
            resources.ApplyResources(this.txtNewCategory, "txtNewCategory");
            this.txtNewCategory.Name = "txtNewCategory";
            this.ToolTip.SetToolTip(this.txtNewCategory, resources.GetString("txtNewCategory.ToolTip"));
            this.txtNewCategory.DoubleClick += new System.EventHandler(this.txtNewCategory_DoubleClick);
            this.txtNewCategory.Leave += new System.EventHandler(this.CategoryLeave);
            // 
            // txtAppendMessage
            // 
            this.txtAppendMessage.DetectUrls = false;
            resources.ApplyResources(this.txtAppendMessage, "txtAppendMessage");
            this.txtAppendMessage.Name = "txtAppendMessage";
            this.ToolTip.SetToolTip(this.txtAppendMessage, resources.GetString("txtAppendMessage.ToolTip"));
            // 
            // chkAppend
            // 
            resources.ApplyResources(this.chkAppend, "chkAppend");
            this.chkAppend.Name = "chkAppend";
            this.ToolTip.SetToolTip(this.chkAppend, resources.GetString("chkAppend.ToolTip"));
            this.chkAppend.UseVisualStyleBackColor = true;
            this.chkAppend.CheckedChanged += new System.EventHandler(this.chkAppend_CheckedChanged);
            // 
            // rdoAppend
            // 
            resources.ApplyResources(this.rdoAppend, "rdoAppend");
            this.rdoAppend.Checked = true;
            this.rdoAppend.Name = "rdoAppend";
            this.rdoAppend.TabStop = true;
            this.ToolTip.SetToolTip(this.rdoAppend, resources.GetString("rdoAppend.ToolTip"));
            // 
            // rdoPrepend
            // 
            resources.ApplyResources(this.rdoPrepend, "rdoPrepend");
            this.rdoPrepend.Name = "rdoPrepend";
            this.ToolTip.SetToolTip(this.rdoPrepend, resources.GetString("rdoPrepend.ToolTip"));
            // 
            // chkGeneralFixes
            // 
            resources.ApplyResources(this.chkGeneralFixes, "chkGeneralFixes");
            this.chkGeneralFixes.Checked = true;
            this.chkGeneralFixes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGeneralFixes.Name = "chkGeneralFixes";
            this.ToolTip.SetToolTip(this.chkGeneralFixes, resources.GetString("chkGeneralFixes.ToolTip"));
            this.chkGeneralFixes.CheckedChanged += new System.EventHandler(this.chkGeneralParse_CheckedChanged);
            // 
            // chkAutoTagger
            // 
            resources.ApplyResources(this.chkAutoTagger, "chkAutoTagger");
            this.chkAutoTagger.Checked = true;
            this.chkAutoTagger.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoTagger.Name = "chkAutoTagger";
            this.ToolTip.SetToolTip(this.chkAutoTagger, resources.GetString("chkAutoTagger.ToolTip"));
            // 
            // chkUnicodifyWhole
            // 
            resources.ApplyResources(this.chkUnicodifyWhole, "chkUnicodifyWhole");
            this.chkUnicodifyWhole.Checked = true;
            this.chkUnicodifyWhole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUnicodifyWhole.Name = "chkUnicodifyWhole";
            this.ToolTip.SetToolTip(this.chkUnicodifyWhole, resources.GetString("chkUnicodifyWhole.ToolTip"));
            // 
            // chkFindandReplace
            // 
            resources.ApplyResources(this.chkFindandReplace, "chkFindandReplace");
            this.chkFindandReplace.Name = "chkFindandReplace";
            this.ToolTip.SetToolTip(this.chkFindandReplace, resources.GetString("chkFindandReplace.ToolTip"));
            this.chkFindandReplace.CheckedChanged += new System.EventHandler(this.chkFindandReplace_CheckedChanged);
            // 
            // chkNudge
            // 
            resources.ApplyResources(this.chkNudge, "chkNudge");
            this.chkNudge.Name = "chkNudge";
            this.ToolTip.SetToolTip(this.chkNudge, resources.GetString("chkNudge.ToolTip"));
            // 
            // chkLock
            // 
            resources.ApplyResources(this.chkLock, "chkLock");
            this.chkLock.Name = "chkLock";
            this.ToolTip.SetToolTip(this.chkLock, resources.GetString("chkLock.ToolTip"));
            this.chkLock.CheckedChanged += new System.EventHandler(this.chkLock_CheckedChanged);
            // 
            // chkMinor
            // 
            resources.ApplyResources(this.chkMinor, "chkMinor");
            this.chkMinor.Name = "chkMinor";
            this.ToolTip.SetToolTip(this.chkMinor, resources.GetString("chkMinor.ToolTip"));
            this.chkMinor.CheckedChanged += new System.EventHandler(this.chkMinor_CheckedChanged);
            // 
            // chkShutdown
            // 
            resources.ApplyResources(this.chkShutdown, "chkShutdown");
            this.chkShutdown.Name = "chkShutdown";
            this.ToolTip.SetToolTip(this.chkShutdown, resources.GetString("chkShutdown.ToolTip"));
            this.chkShutdown.CheckedChanged += new System.EventHandler(this.chkShutdown_CheckedChanged);
            // 
            // btnProtect
            // 
            resources.ApplyResources(this.btnProtect, "btnProtect");
            this.btnProtect.Name = "btnProtect";
            this.ToolTip.SetToolTip(this.btnProtect, resources.GetString("btnProtect.ToolTip"));
            this.btnProtect.Click += new System.EventHandler(this.btnProtect_Click);
            // 
            // btnMove
            // 
            resources.ApplyResources(this.btnMove, "btnMove");
            this.btnMove.Name = "btnMove";
            this.ToolTip.SetToolTip(this.btnMove, resources.GetString("btnMove.ToolTip"));
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDelete.Name = "btnDelete";
            this.ToolTip.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSubst
            // 
            resources.ApplyResources(this.btnSubst, "btnSubst");
            this.btnSubst.Name = "btnSubst";
            this.ToolTip.SetToolTip(this.btnSubst, resources.GetString("btnSubst.ToolTip"));
            this.btnSubst.Click += new System.EventHandler(this.btnSubst_Click);
            // 
            // btnWatch
            // 
            resources.ApplyResources(this.btnWatch, "btnWatch");
            this.btnWatch.Name = "btnWatch";
            this.ToolTip.SetToolTip(this.btnWatch, resources.GetString("btnWatch.ToolTip"));
            this.btnWatch.Click += new System.EventHandler(this.btnWatch_Click);
            // 
            // btnLoadLinks
            // 
            resources.ApplyResources(this.btnLoadLinks, "btnLoadLinks");
            this.btnLoadLinks.Name = "btnLoadLinks";
            this.ToolTip.SetToolTip(this.btnLoadLinks, resources.GetString("btnLoadLinks.ToolTip"));
            this.btnLoadLinks.Click += new System.EventHandler(this.btnLoadLinks_Click);
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.ToolTip.SetToolTip(this.btnRemove, resources.GetString("btnRemove.ToolTip"));
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnFindAndReplaceAdvanced
            // 
            resources.ApplyResources(this.btnFindAndReplaceAdvanced, "btnFindAndReplaceAdvanced");
            this.btnFindAndReplaceAdvanced.Name = "btnFindAndReplaceAdvanced";
            this.ToolTip.SetToolTip(this.btnFindAndReplaceAdvanced, resources.GetString("btnFindAndReplaceAdvanced.ToolTip"));
            this.btnFindAndReplaceAdvanced.Click += new System.EventHandler(this.btnFindAndReplaceAdvanced_Click);
            // 
            // btnMoreFindAndReplce
            // 
            resources.ApplyResources(this.btnMoreFindAndReplce, "btnMoreFindAndReplce");
            this.btnMoreFindAndReplce.Name = "btnMoreFindAndReplce";
            this.ToolTip.SetToolTip(this.btnMoreFindAndReplce, resources.GetString("btnMoreFindAndReplce.ToolTip"));
            this.btnMoreFindAndReplce.Click += new System.EventHandler(this.btnMoreFindAndReplce_Click);
            // 
            // chkSkipNoChanges
            // 
            resources.ApplyResources(this.chkSkipNoChanges, "chkSkipNoChanges");
            this.chkSkipNoChanges.Name = "chkSkipNoChanges";
            this.ToolTip.SetToolTip(this.chkSkipNoChanges, resources.GetString("chkSkipNoChanges.ToolTip"));
            this.chkSkipNoChanges.UseVisualStyleBackColor = true;
            // 
            // chkSkipSpamFilter
            // 
            resources.ApplyResources(this.chkSkipSpamFilter, "chkSkipSpamFilter");
            this.chkSkipSpamFilter.Name = "chkSkipSpamFilter";
            this.ToolTip.SetToolTip(this.chkSkipSpamFilter, resources.GetString("chkSkipSpamFilter.ToolTip"));
            this.chkSkipSpamFilter.UseVisualStyleBackColor = true;
            // 
            // chkSkipIfInuse
            // 
            resources.ApplyResources(this.chkSkipIfInuse, "chkSkipIfInuse");
            this.chkSkipIfInuse.Name = "chkSkipIfInuse";
            this.ToolTip.SetToolTip(this.chkSkipIfInuse, resources.GetString("chkSkipIfInuse.ToolTip"));
            this.chkSkipIfInuse.UseVisualStyleBackColor = true;
            // 
            // chkSkipWhitespace
            // 
            resources.ApplyResources(this.chkSkipWhitespace, "chkSkipWhitespace");
            this.chkSkipWhitespace.Name = "chkSkipWhitespace";
            this.ToolTip.SetToolTip(this.chkSkipWhitespace, resources.GetString("chkSkipWhitespace.ToolTip"));
            this.chkSkipWhitespace.UseVisualStyleBackColor = true;
            // 
            // chkSkipGeneralFixes
            // 
            resources.ApplyResources(this.chkSkipGeneralFixes, "chkSkipGeneralFixes");
            this.chkSkipGeneralFixes.Name = "chkSkipGeneralFixes";
            this.ToolTip.SetToolTip(this.chkSkipGeneralFixes, resources.GetString("chkSkipGeneralFixes.ToolTip"));
            this.chkSkipGeneralFixes.UseVisualStyleBackColor = true;
            this.chkSkipGeneralFixes.CheckedChanged += new System.EventHandler(this.chkSkipGeneralFixes_CheckedChanged);
            // 
            // chkSkipMinorGeneralFixes
            // 
            resources.ApplyResources(this.chkSkipMinorGeneralFixes, "chkSkipMinorGeneralFixes");
            this.chkSkipMinorGeneralFixes.Name = "chkSkipMinorGeneralFixes";
            this.ToolTip.SetToolTip(this.chkSkipMinorGeneralFixes, resources.GetString("chkSkipMinorGeneralFixes.ToolTip"));
            this.chkSkipMinorGeneralFixes.UseVisualStyleBackColor = true;
            // 
            // chkSkipCasing
            // 
            resources.ApplyResources(this.chkSkipCasing, "chkSkipCasing");
            this.chkSkipCasing.Name = "chkSkipCasing";
            this.ToolTip.SetToolTip(this.chkSkipCasing, resources.GetString("chkSkipCasing.ToolTip"));
            this.chkSkipCasing.UseVisualStyleBackColor = true;
            // 
            // chkSkipIfRedirect
            // 
            resources.ApplyResources(this.chkSkipIfRedirect, "chkSkipIfRedirect");
            this.chkSkipIfRedirect.Name = "chkSkipIfRedirect";
            this.ToolTip.SetToolTip(this.chkSkipIfRedirect, resources.GetString("chkSkipIfRedirect.ToolTip"));
            this.chkSkipIfRedirect.UseVisualStyleBackColor = true;
            // 
            // chkSkipIfNoAlerts
            // 
            resources.ApplyResources(this.chkSkipIfNoAlerts, "chkSkipIfNoAlerts");
            this.chkSkipIfNoAlerts.Name = "chkSkipIfNoAlerts";
            this.ToolTip.SetToolTip(this.chkSkipIfNoAlerts, resources.GetString("chkSkipIfNoAlerts.ToolTip"));
            this.chkSkipIfNoAlerts.UseVisualStyleBackColor = true;
            // 
            // chkSkipNoPageLinks
            // 
            resources.ApplyResources(this.chkSkipNoPageLinks, "chkSkipNoPageLinks");
            this.chkSkipNoPageLinks.Name = "chkSkipNoPageLinks";
            this.ToolTip.SetToolTip(this.chkSkipNoPageLinks, resources.GetString("chkSkipNoPageLinks.ToolTip"));
            this.chkSkipNoPageLinks.UseVisualStyleBackColor = true;
            // 
            // radSkipExistent
            // 
            resources.ApplyResources(this.radSkipExistent, "radSkipExistent");
            this.radSkipExistent.Name = "radSkipExistent";
            this.ToolTip.SetToolTip(this.radSkipExistent, resources.GetString("radSkipExistent.ToolTip"));
            this.radSkipExistent.UseVisualStyleBackColor = true;
            // 
            // radSkipNonExistent
            // 
            resources.ApplyResources(this.radSkipNonExistent, "radSkipNonExistent");
            this.radSkipNonExistent.Checked = true;
            this.radSkipNonExistent.Name = "radSkipNonExistent";
            this.radSkipNonExistent.TabStop = true;
            this.ToolTip.SetToolTip(this.radSkipNonExistent, resources.GetString("radSkipNonExistent.ToolTip"));
            this.radSkipNonExistent.UseVisualStyleBackColor = true;
            // 
            // radSkipNone
            // 
            resources.ApplyResources(this.radSkipNone, "radSkipNone");
            this.radSkipNone.Name = "radSkipNone";
            this.ToolTip.SetToolTip(this.radSkipNone, resources.GetString("radSkipNone.ToolTip"));
            this.radSkipNone.UseVisualStyleBackColor = true;
            // 
            // chkNudgeSkip
            // 
            resources.ApplyResources(this.chkNudgeSkip, "chkNudgeSkip");
            this.chkNudgeSkip.Name = "chkNudgeSkip";
            this.ToolTip.SetToolTip(this.chkNudgeSkip, resources.GetString("chkNudgeSkip.ToolTip"));
            // 
            // btnResetNudges
            // 
            resources.ApplyResources(this.btnResetNudges, "btnResetNudges");
            this.btnResetNudges.Name = "btnResetNudges";
            this.ToolTip.SetToolTip(this.btnResetNudges, resources.GetString("btnResetNudges.ToolTip"));
            this.btnResetNudges.Click += new System.EventHandler(this.btnResetNudges_Click);
            // 
            // lblNudges
            // 
            resources.ApplyResources(this.lblNudges, "lblNudges");
            this.lblNudges.Name = "lblNudges";
            this.ToolTip.SetToolTip(this.lblNudges, resources.GetString("lblNudges.ToolTip"));
            // 
            // chkSuppressTag
            // 
            resources.ApplyResources(this.chkSuppressTag, "chkSuppressTag");
            this.chkSuppressTag.Name = "chkSuppressTag";
            this.ToolTip.SetToolTip(this.chkSuppressTag, resources.GetString("chkSuppressTag.ToolTip"));
            // 
            // chkAutoMode
            // 
            resources.ApplyResources(this.chkAutoMode, "chkAutoMode");
            this.chkAutoMode.Name = "chkAutoMode";
            this.ToolTip.SetToolTip(this.chkAutoMode, resources.GetString("chkAutoMode.ToolTip"));
            this.chkAutoMode.CheckedChanged += new System.EventHandler(this.chkAutoMode_CheckedChanged);
            // 
            // lblAutoDelay
            // 
            resources.ApplyResources(this.lblAutoDelay, "lblAutoDelay");
            this.lblAutoDelay.Name = "lblAutoDelay";
            this.ToolTip.SetToolTip(this.lblAutoDelay, resources.GetString("lblAutoDelay.ToolTip"));
            // 
            // lblbotEditsStop
            // 
            resources.ApplyResources(this.lblbotEditsStop, "lblbotEditsStop");
            this.lblbotEditsStop.Name = "lblbotEditsStop";
            this.ToolTip.SetToolTip(this.lblbotEditsStop, resources.GetString("lblbotEditsStop.ToolTip"));
            // 
            // chkFindCaseSensitive
            // 
            resources.ApplyResources(this.chkFindCaseSensitive, "chkFindCaseSensitive");
            this.chkFindCaseSensitive.Name = "chkFindCaseSensitive";
            this.ToolTip.SetToolTip(this.chkFindCaseSensitive, resources.GetString("chkFindCaseSensitive.ToolTip"));
            this.chkFindCaseSensitive.CheckedChanged += new System.EventHandler(this.ResetFind);
            // 
            // chkFindRegex
            // 
            resources.ApplyResources(this.chkFindRegex, "chkFindRegex");
            this.chkFindRegex.Name = "chkFindRegex";
            this.ToolTip.SetToolTip(this.chkFindRegex, resources.GetString("chkFindRegex.ToolTip"));
            this.chkFindRegex.CheckedChanged += new System.EventHandler(this.ResetFind);
            // 
            // txtFind
            // 
            this.txtFind.DetectUrls = false;
            resources.ApplyResources(this.txtFind, "txtFind");
            this.txtFind.Name = "txtFind";
            this.ToolTip.SetToolTip(this.txtFind, resources.GetString("txtFind.ToolTip"));
            this.txtFind.TextChanged += new System.EventHandler(this.ResetFind);
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFind_KeyPress);
            this.txtFind.MouseHover += new System.EventHandler(this.txtFind_MouseHover);
            // 
            // chkSkipNoCatChange
            // 
            resources.ApplyResources(this.chkSkipNoCatChange, "chkSkipNoCatChange");
            this.chkSkipNoCatChange.Name = "chkSkipNoCatChange";
            this.ToolTip.SetToolTip(this.chkSkipNoCatChange, resources.GetString("chkSkipNoCatChange.ToolTip"));
            this.chkSkipNoCatChange.UseVisualStyleBackColor = true;
            // 
            // chkRemoveSortKey
            // 
            resources.ApplyResources(this.chkRemoveSortKey, "chkRemoveSortKey");
            this.chkRemoveSortKey.Name = "chkRemoveSortKey";
            this.ToolTip.SetToolTip(this.chkRemoveSortKey, resources.GetString("chkRemoveSortKey.ToolTip"));
            this.chkRemoveSortKey.UseVisualStyleBackColor = true;
            // 
            // chkSkipOnlyMinorFaR
            // 
            resources.ApplyResources(this.chkSkipOnlyMinorFaR, "chkSkipOnlyMinorFaR");
            this.chkSkipOnlyMinorFaR.Name = "chkSkipOnlyMinorFaR";
            this.ToolTip.SetToolTip(this.chkSkipOnlyMinorFaR, resources.GetString("chkSkipOnlyMinorFaR.ToolTip"));
            // 
            // chkSkipWhenNoFAR
            // 
            resources.ApplyResources(this.chkSkipWhenNoFAR, "chkSkipWhenNoFAR");
            this.chkSkipWhenNoFAR.Name = "chkSkipWhenNoFAR";
            this.ToolTip.SetToolTip(this.chkSkipWhenNoFAR, resources.GetString("chkSkipWhenNoFAR.ToolTip"));
            // 
            // AlertGroup
            // 
            this.AlertGroup.Controls.Add(this.lbAlerts);
            this.AlertGroup.Controls.Add(this.btnRemove);
            this.AlertGroup.Controls.Add(this.lbDuplicateWikilinks);
            this.AlertGroup.Controls.Add(this.lblDuplicateWikilinks);
            resources.ApplyResources(this.AlertGroup, "AlertGroup");
            this.AlertGroup.Name = "AlertGroup";
            this.AlertGroup.TabStop = false;
            this.ToolTip.SetToolTip(this.AlertGroup, resources.GetString("AlertGroup.ToolTip"));
            // 
            // lbAlerts
            // 
            this.lbAlerts.FormattingEnabled = true;
            resources.ApplyResources(this.lbAlerts, "lbAlerts");
            this.lbAlerts.Name = "lbAlerts";
            this.lbAlerts.Click += new System.EventHandler(this.lbAlerts_Click);
            // 
            // lbDuplicateWikilinks
            // 
            this.lbDuplicateWikilinks.FormattingEnabled = true;
            resources.ApplyResources(this.lbDuplicateWikilinks, "lbDuplicateWikilinks");
            this.lbDuplicateWikilinks.Name = "lbDuplicateWikilinks";
            this.lbDuplicateWikilinks.Click += new System.EventHandler(this.lbDuplicateWikilinks_Click);
            // 
            // lblDuplicateWikilinks
            // 
            resources.ApplyResources(this.lblDuplicateWikilinks, "lblDuplicateWikilinks");
            this.lblDuplicateWikilinks.ForeColor = System.Drawing.Color.Red;
            this.lblDuplicateWikilinks.Name = "lblDuplicateWikilinks";
            this.ToolTip.SetToolTip(this.lblDuplicateWikilinks, resources.GetString("lblDuplicateWikilinks.ToolTip"));
            // 
            // SummaryGroup
            // 
            this.SummaryGroup.Controls.Add(this.lblWords);
            this.SummaryGroup.Controls.Add(this.lblInterLinks);
            this.SummaryGroup.Controls.Add(this.lblDates);
            this.SummaryGroup.Controls.Add(this.lblCats);
            this.SummaryGroup.Controls.Add(this.lblImages);
            this.SummaryGroup.Controls.Add(this.lblLinks);
            resources.ApplyResources(this.SummaryGroup, "SummaryGroup");
            this.SummaryGroup.Name = "SummaryGroup";
            this.SummaryGroup.TabStop = false;
            this.ToolTip.SetToolTip(this.SummaryGroup, resources.GetString("SummaryGroup.ToolTip"));
            // 
            // lblWords
            // 
            resources.ApplyResources(this.lblWords, "lblWords");
            this.lblWords.Name = "lblWords";
            // 
            // lblInterLinks
            // 
            resources.ApplyResources(this.lblInterLinks, "lblInterLinks");
            this.lblInterLinks.Name = "lblInterLinks";
            // 
            // lblDates
            // 
            resources.ApplyResources(this.lblDates, "lblDates");
            this.lblDates.Name = "lblDates";
            this.ToolTip.SetToolTip(this.lblDates, resources.GetString("lblDates.ToolTip"));
            // 
            // lblCats
            // 
            resources.ApplyResources(this.lblCats, "lblCats");
            this.lblCats.Name = "lblCats";
            // 
            // lblImages
            // 
            resources.ApplyResources(this.lblImages, "lblImages");
            this.lblImages.Name = "lblImages";
            // 
            // lblLinks
            // 
            resources.ApplyResources(this.lblLinks, "lblLinks");
            this.lblLinks.Name = "lblLinks";
            // 
            // chkSkipIfNoRegexTypo
            // 
            resources.ApplyResources(this.chkSkipIfNoRegexTypo, "chkSkipIfNoRegexTypo");
            this.chkSkipIfNoRegexTypo.Name = "chkSkipIfNoRegexTypo";
            this.ToolTip.SetToolTip(this.chkSkipIfNoRegexTypo, resources.GetString("chkSkipIfNoRegexTypo.ToolTip"));
            // 
            // chkSkipNoImgChange
            // 
            resources.ApplyResources(this.chkSkipNoImgChange, "chkSkipNoImgChange");
            this.chkSkipNoImgChange.Name = "chkSkipNoImgChange";
            this.ToolTip.SetToolTip(this.chkSkipNoImgChange, resources.GetString("chkSkipNoImgChange.ToolTip"));
            this.chkSkipNoImgChange.UseVisualStyleBackColor = true;
            // 
            // chkSkipCosmetic
            // 
            resources.ApplyResources(this.chkSkipCosmetic, "chkSkipCosmetic");
            this.chkSkipCosmetic.Name = "chkSkipCosmetic";
            this.ToolTip.SetToolTip(this.chkSkipCosmetic, resources.GetString("chkSkipCosmetic.ToolTip"));
            this.chkSkipCosmetic.UseVisualStyleBackColor = true;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoEllipsis = true;
            resources.ApplyResources(this.lblSummary, "lblSummary");
            this.lblSummary.Name = "lblSummary";
            this.ToolTip.SetToolTip(this.lblSummary, resources.GetString("lblSummary.ToolTip"));
            this.lblSummary.UseMnemonic = false;
            // 
            // txtReviewEditSummary
            // 
            resources.ApplyResources(this.txtReviewEditSummary, "txtReviewEditSummary");
            this.txtReviewEditSummary.BackColor = System.Drawing.SystemColors.Window;
            this.txtReviewEditSummary.DetectUrls = false;
            this.txtReviewEditSummary.Name = "txtReviewEditSummary";
            this.ToolTip.SetToolTip(this.txtReviewEditSummary, resources.GetString("txtReviewEditSummary.ToolTip"));
            // 
            // imgBold
            // 
            this.imgBold.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgBold, "imgBold");
            this.imgBold.Image = global::AutoWikiBrowser.Properties.Resources.button_bold;
            this.imgBold.Name = "imgBold";
            this.imgBold.TabStop = false;
            this.ToolTip.SetToolTip(this.imgBold, resources.GetString("imgBold.ToolTip"));
            this.imgBold.Click += new System.EventHandler(this.imgBold_Click);
            // 
            // imgItalics
            // 
            this.imgItalics.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgItalics, "imgItalics");
            this.imgItalics.Image = global::AutoWikiBrowser.Properties.Resources.button_italic;
            this.imgItalics.Name = "imgItalics";
            this.imgItalics.TabStop = false;
            this.ToolTip.SetToolTip(this.imgItalics, resources.GetString("imgItalics.ToolTip"));
            this.imgItalics.Click += new System.EventHandler(this.imgItalics_Click);
            // 
            // imgLink
            // 
            this.imgLink.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgLink, "imgLink");
            this.imgLink.Image = global::AutoWikiBrowser.Properties.Resources.button_link;
            this.imgLink.Name = "imgLink";
            this.imgLink.TabStop = false;
            this.ToolTip.SetToolTip(this.imgLink, resources.GetString("imgLink.ToolTip"));
            this.imgLink.Click += new System.EventHandler(this.imgLink_Click);
            // 
            // imgExtlink
            // 
            this.imgExtlink.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgExtlink, "imgExtlink");
            this.imgExtlink.Image = global::AutoWikiBrowser.Properties.Resources.button_extlink;
            this.imgExtlink.Name = "imgExtlink";
            this.imgExtlink.TabStop = false;
            this.ToolTip.SetToolTip(this.imgExtlink, resources.GetString("imgExtlink.ToolTip"));
            this.imgExtlink.Click += new System.EventHandler(this.imgExtlink_Click);
            // 
            // imgMath
            // 
            this.imgMath.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgMath, "imgMath");
            this.imgMath.Image = global::AutoWikiBrowser.Properties.Resources.button_math;
            this.imgMath.Name = "imgMath";
            this.imgMath.TabStop = false;
            this.ToolTip.SetToolTip(this.imgMath, resources.GetString("imgMath.ToolTip"));
            this.imgMath.Click += new System.EventHandler(this.imgMath_Click);
            // 
            // imgNowiki
            // 
            this.imgNowiki.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgNowiki, "imgNowiki");
            this.imgNowiki.Image = global::AutoWikiBrowser.Properties.Resources.button_nowiki;
            this.imgNowiki.Name = "imgNowiki";
            this.imgNowiki.TabStop = false;
            this.ToolTip.SetToolTip(this.imgNowiki, resources.GetString("imgNowiki.ToolTip"));
            this.imgNowiki.Click += new System.EventHandler(this.imgNowiki_Click);
            // 
            // imgHr
            // 
            this.imgHr.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgHr, "imgHr");
            this.imgHr.Image = global::AutoWikiBrowser.Properties.Resources.button_hr;
            this.imgHr.Name = "imgHr";
            this.imgHr.TabStop = false;
            this.ToolTip.SetToolTip(this.imgHr, resources.GetString("imgHr.ToolTip"));
            this.imgHr.Click += new System.EventHandler(this.imgHr_Click);
            // 
            // imgRedirect
            // 
            this.imgRedirect.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgRedirect, "imgRedirect");
            this.imgRedirect.Image = global::AutoWikiBrowser.Properties.Resources.Button_redirect;
            this.imgRedirect.Name = "imgRedirect";
            this.imgRedirect.TabStop = false;
            this.ToolTip.SetToolTip(this.imgRedirect, resources.GetString("imgRedirect.ToolTip"));
            this.imgRedirect.Click += new System.EventHandler(this.imgRedirect_Click);
            // 
            // imgStrike
            // 
            this.imgStrike.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgStrike, "imgStrike");
            this.imgStrike.Image = global::AutoWikiBrowser.Properties.Resources.Button_strike;
            this.imgStrike.Name = "imgStrike";
            this.imgStrike.TabStop = false;
            this.ToolTip.SetToolTip(this.imgStrike, resources.GetString("imgStrike.ToolTip"));
            this.imgStrike.Click += new System.EventHandler(this.imgStrike_Click);
            // 
            // imgSup
            // 
            this.imgSup.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgSup, "imgSup");
            this.imgSup.Image = global::AutoWikiBrowser.Properties.Resources.Button_upper_letter;
            this.imgSup.Name = "imgSup";
            this.imgSup.TabStop = false;
            this.ToolTip.SetToolTip(this.imgSup, resources.GetString("imgSup.ToolTip"));
            this.imgSup.Click += new System.EventHandler(this.imgSup_Click);
            // 
            // imgSub
            // 
            this.imgSub.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgSub, "imgSub");
            this.imgSub.Image = global::AutoWikiBrowser.Properties.Resources.Button_lower_letter;
            this.imgSub.Name = "imgSub";
            this.imgSub.TabStop = false;
            this.ToolTip.SetToolTip(this.imgSub, resources.GetString("imgSub.ToolTip"));
            this.imgSub.Click += new System.EventHandler(this.imgSub_Click);
            // 
            // imgComment
            // 
            this.imgComment.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.imgComment, "imgComment");
            this.imgComment.Image = global::AutoWikiBrowser.Properties.Resources.Button_hide_comment;
            this.imgComment.Name = "imgComment";
            this.imgComment.TabStop = false;
            this.ToolTip.SetToolTip(this.imgComment, resources.GetString("imgComment.ToolTip"));
            this.imgComment.Click += new System.EventHandler(this.imgComment_Click);
            // 
            // EnableRegexTypoFixLinkLabel
            // 
            resources.ApplyResources(this.EnableRegexTypoFixLinkLabel, "EnableRegexTypoFixLinkLabel");
            this.EnableRegexTypoFixLinkLabel.Name = "EnableRegexTypoFixLinkLabel";
            this.EnableRegexTypoFixLinkLabel.TabStop = true;
            this.ToolTip.SetToolTip(this.EnableRegexTypoFixLinkLabel, resources.GetString("EnableRegexTypoFixLinkLabel.ToolTip"));
            this.EnableRegexTypoFixLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ProfileToLoad_LinkClicked);
            // 
            // chkRegExTypo
            // 
            resources.ApplyResources(this.chkRegExTypo, "chkRegExTypo");
            this.chkRegExTypo.Name = "chkRegExTypo";
            this.ToolTip.SetToolTip(this.chkRegExTypo, resources.GetString("chkRegExTypo.ToolTip"));
            this.chkRegExTypo.CheckedChanged += new System.EventHandler(this.chkRegExTypo_CheckedChanged);
            // 
            // chkSkipNoDab
            // 
            resources.ApplyResources(this.chkSkipNoDab, "chkSkipNoDab");
            this.chkSkipNoDab.Name = "chkSkipNoDab";
            this.ToolTip.SetToolTip(this.chkSkipNoDab, resources.GetString("chkSkipNoDab.ToolTip"));
            this.chkSkipNoDab.UseVisualStyleBackColor = true;
            // 
            // chkAppendMetaDataSort
            // 
            resources.ApplyResources(this.chkAppendMetaDataSort, "chkAppendMetaDataSort");
            this.chkAppendMetaDataSort.Name = "chkAppendMetaDataSort";
            this.chkAppendMetaDataSort.UseVisualStyleBackColor = true;
            // 
            // botEditsStop
            // 
            resources.ApplyResources(this.botEditsStop, "botEditsStop");
            this.botEditsStop.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.botEditsStop.Name = "botEditsStop";
            // 
            // mnuHistory
            // 
            this.mnuHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem,
            this.refreshHistoryToolStripMenuItem});
            this.mnuHistory.Name = "mnuHistory";
            resources.ApplyResources(this.mnuHistory, "mnuHistory");
            this.mnuHistory.Opening += new System.ComponentModel.CancelEventHandler(this.mnuHistory_Opening);
            // 
            // openInBrowserToolStripMenuItem
            // 
            resources.ApplyResources(this.openInBrowserToolStripMenuItem, "openInBrowserToolStripMenuItem");
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // refreshHistoryToolStripMenuItem
            // 
            resources.ApplyResources(this.refreshHistoryToolStripMenuItem, "refreshHistoryToolStripMenuItem");
            this.refreshHistoryToolStripMenuItem.Name = "refreshHistoryToolStripMenuItem";
            this.refreshHistoryToolStripMenuItem.Click += new System.EventHandler(this.refreshHistoryToolStripMenuItem_Click);
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            resources.ApplyResources(this.saveListDialog, "saveListDialog");
            // 
            // saveXML
            // 
            this.saveXML.FileName = "settings";
            resources.ApplyResources(this.saveXML, "saveXML");
            this.saveXML.SupportMultiDottedExtensions = true;
            // 
            // openXML
            // 
            resources.ApplyResources(this.openXML, "openXML");
            this.openXML.SupportMultiDottedExtensions = true;
            this.openXML.FileOk += new System.ComponentModel.CancelEventHandler(this.openXML_FileOk);
            // 
            // Timer
            // 
            this.Timer.Enabled = true;
            this.Timer.Interval = 1000;
            this.Timer.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btntsShowHide,
            this.btntsShowHideParameters,
            this.toolStripSeparator12,
            this.btntsStart,
            this.btntsStop,
            this.toolStripSeparator15,
            this.btntsPreview,
            this.btntsChanges,
            this.toolStripSeparator18,
            this.btntsSave,
            this.btntsIgnore,
            this.toolStripSeparator16,
            this.btntsDelete,
            this.toolStripSeparator23,
            this.btntsFalsePositive,
            this.lbltsNumberofItems});
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            // 
            // btntsShowHide
            // 
            this.btntsShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsShowHide, "btntsShowHide");
            this.btntsShowHide.Name = "btntsShowHide";
            this.btntsShowHide.Click += new System.EventHandler(this.btnShowHide_Click);
            this.btntsShowHide.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // btntsShowHideParameters
            // 
            this.btntsShowHideParameters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsShowHideParameters, "btntsShowHideParameters");
            this.btntsShowHideParameters.Name = "btntsShowHideParameters";
            this.btntsShowHideParameters.Click += new System.EventHandler(this.btntsShowHideParameters_Click);
            this.btntsShowHideParameters.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // btntsStart
            // 
            this.btntsStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsStart, "btntsStart");
            this.btntsStart.Name = "btntsStart";
            this.btntsStart.Click += new System.EventHandler(this.btntsStart_Click);
            this.btntsStart.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // btntsStop
            // 
            this.btntsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsStop, "btntsStop");
            this.btntsStop.Name = "btntsStop";
            this.btntsStop.Click += new System.EventHandler(this.btntsStop_Click);
            this.btntsStop.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // btntsPreview
            // 
            this.btntsPreview.AutoToolTip = false;
            resources.ApplyResources(this.btntsPreview, "btntsPreview");
            this.btntsPreview.Name = "btntsPreview";
            this.btntsPreview.Click += new System.EventHandler(this.btntsPreview_Click);
            this.btntsPreview.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // btntsChanges
            // 
            this.btntsChanges.AutoToolTip = false;
            resources.ApplyResources(this.btntsChanges, "btntsChanges");
            this.btntsChanges.Name = "btntsChanges";
            this.btntsChanges.Click += new System.EventHandler(this.btntsChanges_Click);
            this.btntsChanges.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // btntsSave
            // 
            this.btntsSave.AutoToolTip = false;
            resources.ApplyResources(this.btntsSave, "btntsSave");
            this.btntsSave.Name = "btntsSave";
            this.btntsSave.Click += new System.EventHandler(this.btntsSave_Click);
            this.btntsSave.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // btntsIgnore
            // 
            this.btntsIgnore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsIgnore, "btntsIgnore");
            this.btntsIgnore.Name = "btntsIgnore";
            this.btntsIgnore.Click += new System.EventHandler(this.btntsIgnore_Click);
            this.btntsIgnore.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // btntsDelete
            // 
            this.btntsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsDelete, "btntsDelete");
            this.btntsDelete.Name = "btntsDelete";
            this.btntsDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btntsDelete.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // btntsFalsePositive
            // 
            this.btntsFalsePositive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btntsFalsePositive, "btntsFalsePositive");
            this.btntsFalsePositive.Name = "btntsFalsePositive";
            this.btntsFalsePositive.Click += new System.EventHandler(this.FalsePositiveClick);
            this.btntsFalsePositive.MouseHover += new System.EventHandler(this.editToolBar_MouseHover);
            // 
            // lbltsNumberofItems
            // 
            this.lbltsNumberofItems.Name = "lbltsNumberofItems";
            resources.ApplyResources(this.lbltsNumberofItems, "lbltsNumberofItems");
            // 
            // ntfyTray
            // 
            this.ntfyTray.ContextMenuStrip = this.mnuNotify;
            resources.ApplyResources(this.ntfyTray, "ntfyTray");
            this.ntfyTray.DoubleClick += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // mnuNotify
            // 
            this.mnuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.hideToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem1});
            this.mnuNotify.Name = "mnuNotify";
            resources.ApplyResources(this.mnuNotify, "mnuNotify");
            this.mnuNotify.Opening += new System.ComponentModel.CancelEventHandler(this.mnuNotify_Opening);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            resources.ApplyResources(this.showToolStripMenuItem, "showToolStripMenuItem");
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // hideToolStripMenuItem
            // 
            this.hideToolStripMenuItem.Name = "hideToolStripMenuItem";
            resources.ApplyResources(this.hideToolStripMenuItem, "hideToolStripMenuItem");
            this.hideToolStripMenuItem.Click += new System.EventHandler(this.hideToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            resources.ApplyResources(this.exitToolStripMenuItem1, "exitToolStripMenuItem1");
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // EditBoxSaveTimer
            // 
            this.EditBoxSaveTimer.Interval = 1000;
            this.EditBoxSaveTimer.Tick += new System.EventHandler(this.EditBoxSaveTimer_Tick);
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.tpOptions);
            this.MainTab.Controls.Add(this.tpMoreOptions);
            this.MainTab.Controls.Add(this.tpDab);
            this.MainTab.Controls.Add(this.tpSkip);
            this.MainTab.Controls.Add(this.tpStart);
            this.MainTab.HotTrack = true;
            resources.ApplyResources(this.MainTab, "MainTab");
            this.MainTab.Name = "MainTab";
            this.MainTab.SelectedIndex = 0;
            this.MainTab.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tpOptions
            // 
            this.tpOptions.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpOptions.Controls.Add(this.groupBox13);
            this.tpOptions.Controls.Add(this.groupBox1);
            this.tpOptions.Controls.Add(this.groupBox6);
            resources.ApplyResources(this.tpOptions, "tpOptions");
            this.tpOptions.Name = "tpOptions";
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.chkSkipIfNoRegexTypo);
            this.groupBox13.Controls.Add(this.chkRegExTypo);
            this.groupBox13.Controls.Add(this.EnableRegexTypoFixLinkLabel);
            resources.ApplyResources(this.groupBox13, "groupBox13");
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSkipOnlyMinorFaR);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.chkSkipWhenNoFAR);
            this.groupBox1.Controls.Add(this.btnSubst);
            this.groupBox1.Controls.Add(this.btnFindAndReplaceAdvanced);
            this.groupBox1.Controls.Add(this.btnMoreFindAndReplce);
            this.groupBox1.Controls.Add(this.chkFindandReplace);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnMoreSkip);
            this.groupBox6.Controls.Add(this.chkUnicodifyWhole);
            this.groupBox6.Controls.Add(this.chkGeneralFixes);
            this.groupBox6.Controls.Add(this.chkAutoTagger);
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // btnMoreSkip
            // 
            resources.ApplyResources(this.btnMoreSkip, "btnMoreSkip");
            this.btnMoreSkip.Name = "btnMoreSkip";
            this.btnMoreSkip.Click += new System.EventHandler(this.btnMoreSkip_Click);
            // 
            // tpMoreOptions
            // 
            resources.ApplyResources(this.tpMoreOptions, "tpMoreOptions");
            this.tpMoreOptions.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpMoreOptions.Controls.Add(this.ImageGroupBox);
            this.tpMoreOptions.Controls.Add(this.groupBox4);
            this.tpMoreOptions.Controls.Add(this.groupBox5);
            this.tpMoreOptions.Name = "tpMoreOptions";
            // 
            // ImageGroupBox
            // 
            this.ImageGroupBox.Controls.Add(this.lblImageWith);
            this.ImageGroupBox.Controls.Add(this.chkSkipNoImgChange);
            this.ImageGroupBox.Controls.Add(this.txtImageWith);
            this.ImageGroupBox.Controls.Add(this.txtImageReplace);
            this.ImageGroupBox.Controls.Add(this.cmboImages);
            resources.ApplyResources(this.ImageGroupBox, "ImageGroupBox");
            this.ImageGroupBox.Name = "ImageGroupBox";
            this.ImageGroupBox.TabStop = false;
            // 
            // lblImageWith
            // 
            resources.ApplyResources(this.lblImageWith, "lblImageWith");
            this.lblImageWith.Name = "lblImageWith";
            // 
            // txtImageWith
            // 
            resources.ApplyResources(this.txtImageWith, "txtImageWith");
            this.txtImageWith.Name = "txtImageWith";
            this.txtImageWith.Leave += new System.EventHandler(this.txtImageWith_Leave);
            // 
            // txtImageReplace
            // 
            resources.ApplyResources(this.txtImageReplace, "txtImageReplace");
            this.txtImageReplace.Name = "txtImageReplace";
            this.txtImageReplace.Leave += new System.EventHandler(this.txtImageReplace_Leave);
            // 
            // cmboImages
            // 
            this.cmboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboImages.FormattingEnabled = true;
            this.cmboImages.Items.AddRange(new object[] {
            resources.GetString("cmboImages.Items"),
            resources.GetString("cmboImages.Items1"),
            resources.GetString("cmboImages.Items2"),
            resources.GetString("cmboImages.Items3")});
            resources.ApplyResources(this.cmboImages, "cmboImages");
            this.cmboImages.Name = "cmboImages";
            this.cmboImages.SelectedIndexChanged += new System.EventHandler(this.cmboImages_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblNewlineCharacters);
            this.groupBox4.Controls.Add(this.lblUse);
            this.groupBox4.Controls.Add(this.udNewlineChars);
            this.groupBox4.Controls.Add(this.rdoPrepend);
            this.groupBox4.Controls.Add(this.rdoAppend);
            this.groupBox4.Controls.Add(this.chkAppend);
            this.groupBox4.Controls.Add(this.chkAppendMetaDataSort);
            this.groupBox4.Controls.Add(this.txtAppendMessage);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // lblNewlineCharacters
            // 
            resources.ApplyResources(this.lblNewlineCharacters, "lblNewlineCharacters");
            this.lblNewlineCharacters.Name = "lblNewlineCharacters";
            // 
            // lblUse
            // 
            resources.ApplyResources(this.lblUse, "lblUse");
            this.lblUse.Name = "lblUse";
            // 
            // udNewlineChars
            // 
            resources.ApplyResources(this.udNewlineChars, "udNewlineChars");
            this.udNewlineChars.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.udNewlineChars.Name = "udNewlineChars";
            this.udNewlineChars.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkSkipNoCatChange);
            this.groupBox5.Controls.Add(this.chkRemoveSortKey);
            this.groupBox5.Controls.Add(this.txtNewCategory2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.cmboCategorise);
            this.groupBox5.Controls.Add(this.txtNewCategory);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // txtNewCategory2
            // 
            resources.ApplyResources(this.txtNewCategory2, "txtNewCategory2");
            this.txtNewCategory2.Name = "txtNewCategory2";
            this.txtNewCategory2.DoubleClick += new System.EventHandler(this.txtNewCategory2_DoubleClick);
            this.txtNewCategory2.Leave += new System.EventHandler(this.CategoryLeave);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cmboCategorise
            // 
            this.cmboCategorise.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboCategorise.FormattingEnabled = true;
            this.cmboCategorise.Items.AddRange(new object[] {
            resources.GetString("cmboCategorise.Items"),
            resources.GetString("cmboCategorise.Items1"),
            resources.GetString("cmboCategorise.Items2"),
            resources.GetString("cmboCategorise.Items3")});
            resources.ApplyResources(this.cmboCategorise, "cmboCategorise");
            this.cmboCategorise.Name = "cmboCategorise";
            this.cmboCategorise.SelectedIndexChanged += new System.EventHandler(this.cmboCategorise_SelectedIndexChanged);
            // 
            // tpDab
            // 
            this.tpDab.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpDab.Controls.Add(this.panelDab);
            this.tpDab.Controls.Add(this.chkEnableDab);
            resources.ApplyResources(this.tpDab, "tpDab");
            this.tpDab.Name = "tpDab";
            // 
            // panelDab
            // 
            this.panelDab.Controls.Add(this.groupBox12);
            this.panelDab.Controls.Add(this.groupBox11);
            resources.ApplyResources(this.panelDab, "panelDab");
            this.panelDab.Name = "panelDab";
            // 
            // groupBox12
            // 
            this.groupBox12.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.groupBox12.Controls.Add(this.label5);
            this.groupBox12.Controls.Add(this.udContextChars);
            this.groupBox12.Controls.Add(this.label4);
            this.groupBox12.Controls.Add(this.chkSkipNoDab);
            this.groupBox12.Controls.Add(this.txtDabVariants);
            resources.ApplyResources(this.groupBox12, "groupBox12");
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // udContextChars
            // 
            resources.ApplyResources(this.udContextChars, "udContextChars");
            this.udContextChars.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udContextChars.Name = "udContextChars";
            this.udContextChars.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtDabVariants
            // 
            resources.ApplyResources(this.txtDabVariants, "txtDabVariants");
            this.txtDabVariants.Name = "txtDabVariants";
            // 
            // groupBox11
            // 
            this.groupBox11.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.groupBox11.Controls.Add(this.btnLoadLinks);
            this.groupBox11.Controls.Add(this.txtDabLink);
            resources.ApplyResources(this.groupBox11, "groupBox11");
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.TabStop = false;
            // 
            // txtDabLink
            // 
            resources.ApplyResources(this.txtDabLink, "txtDabLink");
            this.txtDabLink.Name = "txtDabLink";
            this.txtDabLink.TextChanged += new System.EventHandler(this.txtDabLink_TextChanged);
            this.txtDabLink.Enter += new System.EventHandler(this.txtDabLink_Enter);
            this.txtDabLink.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDabLink_KeyPress);
            // 
            // chkEnableDab
            // 
            resources.ApplyResources(this.chkEnableDab, "chkEnableDab");
            this.chkEnableDab.Name = "chkEnableDab";
            this.chkEnableDab.CheckedChanged += new System.EventHandler(this.chkEnableDab_CheckedChanged);
            // 
            // tpSkip
            // 
            this.tpSkip.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpSkip.Controls.Add(this.gbRegexSkip);
            this.tpSkip.Controls.Add(this.groupBox8);
            this.tpSkip.Controls.Add(this.gbPageExisting);
            resources.ApplyResources(this.tpSkip, "tpSkip");
            this.tpSkip.Name = "tpSkip";
            // 
            // gbRegexSkip
            // 
            this.gbRegexSkip.Controls.Add(this.skipIfNotContains);
            this.gbRegexSkip.Controls.Add(this.skipIfContains);
            resources.ApplyResources(this.gbRegexSkip, "gbRegexSkip");
            this.gbRegexSkip.Name = "gbRegexSkip";
            this.gbRegexSkip.TabStop = false;
            // 
            // skipIfNotContains
            // 
            this.skipIfNotContains.After = false;
            this.skipIfNotContains.CheckEnabled = false;
            this.skipIfNotContains.CheckText = "";
            this.skipIfNotContains.IsCaseSensitive = false;
            this.skipIfNotContains.IsRegex = false;
            resources.ApplyResources(this.skipIfNotContains, "skipIfNotContains");
            this.skipIfNotContains.Name = "skipIfNotContains";
            // 
            // skipIfContains
            // 
            this.skipIfContains.After = false;
            this.skipIfContains.CheckEnabled = false;
            this.skipIfContains.CheckText = "";
            this.skipIfContains.IsCaseSensitive = false;
            this.skipIfContains.IsRegex = false;
            resources.ApplyResources(this.skipIfContains, "skipIfContains");
            this.skipIfContains.Name = "skipIfContains";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.chkSkipCosmetic);
            this.groupBox8.Controls.Add(this.chkSkipIfNoAlerts);
            this.groupBox8.Controls.Add(this.chkSkipMinorGeneralFixes);
            this.groupBox8.Controls.Add(this.chkSkipIfRedirect);
            this.groupBox8.Controls.Add(this.chkSkipCasing);
            this.groupBox8.Controls.Add(this.chkSkipGeneralFixes);
            this.groupBox8.Controls.Add(this.chkSkipNoPageLinks);
            this.groupBox8.Controls.Add(this.chkSkipWhitespace);
            this.groupBox8.Controls.Add(this.chkSkipIfInuse);
            this.groupBox8.Controls.Add(this.chkSkipSpamFilter);
            this.groupBox8.Controls.Add(this.chkSkipNoChanges);
            resources.ApplyResources(this.groupBox8, "groupBox8");
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.TabStop = false;
            // 
            // gbPageExisting
            // 
            this.gbPageExisting.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.gbPageExisting, "gbPageExisting");
            this.gbPageExisting.Name = "gbPageExisting";
            this.gbPageExisting.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.radSkipExistent);
            this.flowLayoutPanel1.Controls.Add(this.radSkipNonExistent);
            this.flowLayoutPanel1.Controls.Add(this.radSkipNone);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // tpStart
            // 
            this.tpStart.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpStart.Controls.Add(this.btnStart);
            this.tpStart.Controls.Add(this.btnStop);
            this.tpStart.Controls.Add(this.btnFalsePositive);
            this.tpStart.Controls.Add(this.btnPreview);
            this.tpStart.Controls.Add(this.btnDiff);
            this.tpStart.Controls.Add(this.btnWatch);
            this.tpStart.Controls.Add(this.btnMove);
            this.tpStart.Controls.Add(this.btnProtect);
            this.tpStart.Controls.Add(this.btnDelete);
            this.tpStart.Controls.Add(this.btnIgnore);
            this.tpStart.Controls.Add(this.btnSave);
            this.tpStart.Controls.Add(this.SummaryLabel);
            this.tpStart.Controls.Add(this.lblSummary);
            this.tpStart.Controls.Add(this.cmboEditSummary);
            this.tpStart.Controls.Add(this.chkLock);
            this.tpStart.Controls.Add(this.chkMinor);
            this.tpStart.Controls.Add(this.findGroup);
            this.tpStart.Controls.Add(this.AlertGroup);
            this.tpStart.Controls.Add(this.SummaryGroup);
            resources.ApplyResources(this.tpStart, "tpStart");
            this.tpStart.Name = "tpStart";
            // 
            // SummaryLabel
            // 
            resources.ApplyResources(this.SummaryLabel, "SummaryLabel");
            this.SummaryLabel.Name = "SummaryLabel";
            // 
            // cmboEditSummary
            // 
            this.cmboEditSummary.DropDownHeight = 198;
            this.cmboEditSummary.DropDownWidth = 400;
            resources.ApplyResources(this.cmboEditSummary, "cmboEditSummary");
            this.cmboEditSummary.FormattingEnabled = true;
            this.cmboEditSummary.Name = "cmboEditSummary";
            this.cmboEditSummary.TextChanged += new System.EventHandler(this.cmboEditSummary_TextChanged);
            this.cmboEditSummary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbEditSummary_KeyDown);
            this.cmboEditSummary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cmboEditSummary_MouseMove);
            // 
            // findGroup
            // 
            this.findGroup.Controls.Add(this.chkFindCaseSensitive);
            this.findGroup.Controls.Add(this.btnFind);
            this.findGroup.Controls.Add(this.chkFindRegex);
            this.findGroup.Controls.Add(this.txtFind);
            resources.ApplyResources(this.findGroup, "findGroup");
            this.findGroup.Name = "findGroup";
            this.findGroup.TabStop = false;
            // 
            // tpBots
            // 
            this.tpBots.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tpBots.Controls.Add(this.groupBox16);
            this.tpBots.Controls.Add(this.groupBox14);
            this.tpBots.Controls.Add(this.groupBox7);
            this.tpBots.Controls.Add(this.BotImage);
            this.tpBots.Controls.Add(this.lblOnlyBots);
            resources.ApplyResources(this.tpBots, "tpBots");
            this.tpBots.Name = "tpBots";
            // 
            // groupBox16
            // 
            this.groupBox16.Controls.Add(this.radStandby);
            this.groupBox16.Controls.Add(this.radHibernate);
            this.groupBox16.Controls.Add(this.radRestart);
            this.groupBox16.Controls.Add(this.radShutdown);
            this.groupBox16.Controls.Add(this.chkShutdown);
            resources.ApplyResources(this.groupBox16, "groupBox16");
            this.groupBox16.Name = "groupBox16";
            this.groupBox16.TabStop = false;
            // 
            // radStandby
            // 
            resources.ApplyResources(this.radStandby, "radStandby");
            this.radStandby.Name = "radStandby";
            this.radStandby.TabStop = true;
            // 
            // radHibernate
            // 
            resources.ApplyResources(this.radHibernate, "radHibernate");
            this.radHibernate.Name = "radHibernate";
            this.radHibernate.TabStop = true;
            // 
            // radRestart
            // 
            resources.ApplyResources(this.radRestart, "radRestart");
            this.radRestart.Name = "radRestart";
            this.radRestart.TabStop = true;
            // 
            // radShutdown
            // 
            resources.ApplyResources(this.radShutdown, "radShutdown");
            this.radShutdown.Name = "radShutdown";
            this.radShutdown.TabStop = true;
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.chkNudgeSkip);
            this.groupBox14.Controls.Add(this.btnResetNudges);
            this.groupBox14.Controls.Add(this.lblNudges);
            this.groupBox14.Controls.Add(this.chkNudge);
            resources.ApplyResources(this.groupBox14, "groupBox14");
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.chkSuppressTag);
            this.groupBox7.Controls.Add(this.chkAutoMode);
            this.groupBox7.Controls.Add(this.nudBotSpeed);
            this.groupBox7.Controls.Add(this.botEditsStop);
            this.groupBox7.Controls.Add(this.lblbotEditsStop);
            this.groupBox7.Controls.Add(this.lblAutoDelay);
            resources.ApplyResources(this.groupBox7, "groupBox7");
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // BotImage
            // 
            resources.ApplyResources(this.BotImage, "BotImage");
            this.BotImage.Image = global::AutoWikiBrowser.Properties.Resources.BotImage;
            this.BotImage.Name = "BotImage";
            this.BotImage.TabStop = false;
            this.BotImage.Click += new System.EventHandler(this.BotImage_Click);
            // 
            // lblOnlyBots
            // 
            this.lblOnlyBots.BackColor = System.Drawing.SystemColors.ButtonFace;
            resources.ApplyResources(this.lblOnlyBots, "lblOnlyBots");
            this.lblOnlyBots.Name = "lblOnlyBots";
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // ShutdownTimer
            // 
            this.ShutdownTimer.Interval = 30000;
            this.ShutdownTimer.Tick += new System.EventHandler(this.ShutdownTimer_Tick);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.listMaker);
            this.panel1.Controls.Add(this.MainTab);
            this.panel1.Controls.Add(this.EditBoxTab);
            this.panel1.Name = "panel1";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.Name = "label8";
            // 
            // listMaker
            // 
            resources.ApplyResources(this.listMaker, "listMaker");
            this.listMaker.Name = "listMaker";
            this.listMaker.SelectedProvider = "CategoryListProvider";
            this.listMaker.SourceText = "";
            this.listMaker.SpecialFilterSettings = ((WikiFunctions.AWBSettings.SpecialFilterPrefs)(resources.GetObject("listMaker.SpecialFilterSettings")));
            // 
            // EditBoxTab
            // 
            resources.ApplyResources(this.EditBoxTab, "EditBoxTab");
            this.EditBoxTab.Controls.Add(this.tpEdit);
            this.EditBoxTab.Controls.Add(this.tpHistory);
            this.EditBoxTab.Controls.Add(this.tpLinks);
            this.EditBoxTab.Controls.Add(this.tpLogs);
            this.EditBoxTab.Controls.Add(this.tpArticleActionLogs);
            this.EditBoxTab.Controls.Add(this.tpTypos);
            this.EditBoxTab.HotTrack = true;
            this.EditBoxTab.Name = "EditBoxTab";
            this.EditBoxTab.SelectedIndex = 0;
            this.EditBoxTab.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.EditBoxTab.SelectedIndexChanged += new System.EventHandler(this.tabControl2_SelectedIndexChanged);
            // 
            // tpEdit
            // 
            this.tpEdit.BackColor = System.Drawing.Color.Transparent;
            this.tpEdit.Controls.Add(this.imgComment);
            this.tpEdit.Controls.Add(this.imgSub);
            this.tpEdit.Controls.Add(this.imgSup);
            this.tpEdit.Controls.Add(this.imgStrike);
            this.tpEdit.Controls.Add(this.imgRedirect);
            this.tpEdit.Controls.Add(this.imgHr);
            this.tpEdit.Controls.Add(this.imgNowiki);
            this.tpEdit.Controls.Add(this.imgMath);
            this.tpEdit.Controls.Add(this.imgExtlink);
            this.tpEdit.Controls.Add(this.imgLink);
            this.tpEdit.Controls.Add(this.imgItalics);
            this.tpEdit.Controls.Add(this.imgBold);
            this.tpEdit.Controls.Add(this.txtEdit);
            this.tpEdit.Controls.Add(this.txtReviewEditSummary);
            resources.ApplyResources(this.tpEdit, "tpEdit");
            this.tpEdit.Name = "tpEdit";
            // 
            // txtEdit
            // 
            this.txtEdit.AcceptsTab = true;
            resources.ApplyResources(this.txtEdit, "txtEdit");
            this.txtEdit.ContextMenuStrip = this.mnuTextBox;
            this.txtEdit.DetectUrls = false;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.TextChanged += new System.EventHandler(this.txtEdit_TextChanged);
            // 
            // tpHistory
            // 
            this.tpHistory.ContextMenuStrip = this.mnuHistory;
            this.tpHistory.Controls.Add(this.webBrowserHistory);
            resources.ApplyResources(this.tpHistory, "tpHistory");
            this.tpHistory.Name = "tpHistory";
            this.tpHistory.UseVisualStyleBackColor = true;
            // 
            // webBrowserHistory
            // 
            this.webBrowserHistory.ContextMenuStrip = this.mnuHistory;
            resources.ApplyResources(this.webBrowserHistory, "webBrowserHistory");
            this.webBrowserHistory.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserHistory.Name = "webBrowserHistory";
            this.webBrowserHistory.ScriptErrorsSuppressed = true;
            this.webBrowserHistory.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserHistory_DocumentCompleted);
            // 
            // tpLinks
            // 
            this.tpLinks.Controls.Add(this.webBrowserLinks);
            resources.ApplyResources(this.tpLinks, "tpLinks");
            this.tpLinks.Name = "tpLinks";
            this.tpLinks.UseVisualStyleBackColor = true;
            // 
            // webBrowserLinks
            // 
            resources.ApplyResources(this.webBrowserLinks, "webBrowserLinks");
            this.webBrowserLinks.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserLinks.Name = "webBrowserLinks";
            this.webBrowserLinks.ScriptErrorsSuppressed = true;
            this.webBrowserLinks.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserLinks_DocumentCompleted);
            // 
            // tpLogs
            // 
            this.tpLogs.Controls.Add(this.logControl);
            resources.ApplyResources(this.tpLogs, "tpLogs");
            this.tpLogs.Name = "tpLogs";
            this.tpLogs.UseVisualStyleBackColor = true;
            // 
            // logControl
            // 
            resources.ApplyResources(this.logControl, "logControl");
            this.logControl.Name = "logControl";
            // 
            // tpArticleActionLogs
            // 
            this.tpArticleActionLogs.Controls.Add(this.articleActionLogControl1);
            resources.ApplyResources(this.tpArticleActionLogs, "tpArticleActionLogs");
            this.tpArticleActionLogs.Name = "tpArticleActionLogs";
            this.tpArticleActionLogs.UseVisualStyleBackColor = true;
            // 
            // articleActionLogControl1
            // 
            resources.ApplyResources(this.articleActionLogControl1, "articleActionLogControl1");
            this.articleActionLogControl1.Name = "articleActionLogControl1";
            // 
            // tpTypos
            // 
            this.tpTypos.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.tpTypos, "tpTypos");
            this.tpTypos.Name = "tpTypos";
            this.tpTypos.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox9);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox10);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.CurrentTypoStats);
            resources.ApplyResources(this.groupBox9, "groupBox9");
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.TabStop = false;
            // 
            // CurrentTypoStats
            // 
            this.CurrentTypoStats.ComparerFactory = this.CurrentTypoStats;
            resources.ApplyResources(this.CurrentTypoStats, "CurrentTypoStats");
            this.CurrentTypoStats.MultiSelect = false;
            this.CurrentTypoStats.Name = "CurrentTypoStats";
            this.CurrentTypoStats.SortColumnsOnClick = true;
            this.CurrentTypoStats.UseCompatibleStateImageBehavior = false;
            this.CurrentTypoStats.View = System.Windows.Forms.View.Details;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.lblTypoRatio);
            this.groupBox10.Controls.Add(this.OverallTypoStats);
            this.groupBox10.Controls.Add(this.lblNoChange);
            this.groupBox10.Controls.Add(this.label3);
            this.groupBox10.Controls.Add(this.label6);
            this.groupBox10.Controls.Add(this.lblOverallTypos);
            this.groupBox10.Controls.Add(this.label7);
            resources.ApplyResources(this.groupBox10, "groupBox10");
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.TabStop = false;
            // 
            // lblTypoRatio
            // 
            resources.ApplyResources(this.lblTypoRatio, "lblTypoRatio");
            this.lblTypoRatio.Name = "lblTypoRatio";
            // 
            // OverallTypoStats
            // 
            resources.ApplyResources(this.OverallTypoStats, "OverallTypoStats");
            this.OverallTypoStats.ComparerFactory = this.OverallTypoStats;
            this.OverallTypoStats.IsOverallStats = true;
            this.OverallTypoStats.MultiSelect = false;
            this.OverallTypoStats.Name = "OverallTypoStats";
            this.OverallTypoStats.SortColumnsOnClick = true;
            this.OverallTypoStats.UseCompatibleStateImageBehavior = false;
            this.OverallTypoStats.View = System.Windows.Forms.View.Details;
            // 
            // lblNoChange
            // 
            resources.ApplyResources(this.lblNoChange, "lblNoChange");
            this.lblNoChange.Name = "lblNoChange";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // lblOverallTypos
            // 
            resources.ApplyResources(this.lblOverallTypos, "lblOverallTypos");
            this.lblOverallTypos.Name = "lblOverallTypos";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // mnuMakeFromTextBox
            // 
            this.mnuMakeFromTextBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemMakeFromTextBoxUndo,
            this.toolStripSeparator27,
            this.menuitemMakeFromTextBoxCut,
            this.menuitemMakeFromTextBoxCopy,
            this.menuitemMakeFromTextBoxPaste,
            this.toolStripSeparatorMakeFromTextBox});
            this.mnuMakeFromTextBox.Name = "mnuMakeFromTextBox";
            resources.ApplyResources(this.mnuMakeFromTextBox, "mnuMakeFromTextBox");
            // 
            // menuitemMakeFromTextBoxUndo
            // 
            this.menuitemMakeFromTextBoxUndo.Name = "menuitemMakeFromTextBoxUndo";
            resources.ApplyResources(this.menuitemMakeFromTextBoxUndo, "menuitemMakeFromTextBoxUndo");
            this.menuitemMakeFromTextBoxUndo.Click += new System.EventHandler(this.menuitemMakeFromTextBoxUndo_Click);
            // 
            // toolStripSeparator27
            // 
            this.toolStripSeparator27.Name = "toolStripSeparator27";
            resources.ApplyResources(this.toolStripSeparator27, "toolStripSeparator27");
            // 
            // menuitemMakeFromTextBoxCut
            // 
            this.menuitemMakeFromTextBoxCut.Name = "menuitemMakeFromTextBoxCut";
            resources.ApplyResources(this.menuitemMakeFromTextBoxCut, "menuitemMakeFromTextBoxCut");
            this.menuitemMakeFromTextBoxCut.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCut_Click);
            // 
            // menuitemMakeFromTextBoxCopy
            // 
            this.menuitemMakeFromTextBoxCopy.Name = "menuitemMakeFromTextBoxCopy";
            resources.ApplyResources(this.menuitemMakeFromTextBoxCopy, "menuitemMakeFromTextBoxCopy");
            this.menuitemMakeFromTextBoxCopy.Click += new System.EventHandler(this.menuitemMakeFromTextBoxCopy_Click);
            // 
            // menuitemMakeFromTextBoxPaste
            // 
            this.menuitemMakeFromTextBoxPaste.Name = "menuitemMakeFromTextBoxPaste";
            resources.ApplyResources(this.menuitemMakeFromTextBoxPaste, "menuitemMakeFromTextBoxPaste");
            this.menuitemMakeFromTextBoxPaste.Click += new System.EventHandler(this.menuitemMakeFromTextBoxPaste_Click);
            // 
            // toolStripSeparatorMakeFromTextBox
            // 
            this.toolStripSeparatorMakeFromTextBox.Name = "toolStripSeparatorMakeFromTextBox";
            resources.ApplyResources(this.toolStripSeparatorMakeFromTextBox, "toolStripSeparatorMakeFromTextBox");
            // 
            // webBrowser
            // 
            this.webBrowser.AllowNavigation = false;
            this.webBrowser.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowser, "webBrowser");
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // NudgeTimer
            // 
            this.NudgeTimer.Interval = 120000;
            this.NudgeTimer.Tick += new AutoWikiBrowser.NudgeTimer.TickEventHandler(this.NudgeTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.MnuMain);
            this.Controls.Add(this.StatusMain);
            this.Controls.Add(this.toolStrip);
            settings1.AskForTerminate = true;
            settings1.CustomWikis = "";
            settings1.Privacy = false;
            settings1.SettingsKey = "";
            settings1.WindowLocation = new System.Drawing.Point(0, 0);
            settings1.WindowSize = new System.Drawing.Size(925, 717);
            settings1.WindowState = System.Windows.Forms.FormWindowState.Normal;
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", settings1, "WindowLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.mnuTextBox.ResumeLayout(false);
            this.MnuMain.ResumeLayout(false);
            this.MnuMain.PerformLayout();
            this.StatusMain.ResumeLayout(false);
            this.StatusMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).EndInit();
            this.AlertGroup.ResumeLayout(false);
            this.AlertGroup.PerformLayout();
            this.SummaryGroup.ResumeLayout(false);
            this.SummaryGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgItalics)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgLink)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgExtlink)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgMath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgNowiki)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgRedirect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgStrike)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgSub)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgComment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.botEditsStop)).EndInit();
            this.mnuHistory.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mnuNotify.ResumeLayout(false);
            this.MainTab.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tpMoreOptions.ResumeLayout(false);
            this.ImageGroupBox.ResumeLayout(false);
            this.ImageGroupBox.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udNewlineChars)).EndInit();
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
            this.tpSkip.ResumeLayout(false);
            this.gbRegexSkip.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.gbPageExisting.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tpStart.ResumeLayout(false);
            this.tpStart.PerformLayout();
            this.findGroup.ResumeLayout(false);
            this.findGroup.PerformLayout();
            this.tpBots.ResumeLayout(false);
            this.groupBox16.ResumeLayout(false);
            this.groupBox16.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BotImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.EditBoxTab.ResumeLayout(false);
            this.tpEdit.ResumeLayout(false);
            this.tpEdit.PerformLayout();
            this.tpHistory.ResumeLayout(false);
            this.tpLinks.ResumeLayout(false);
            this.tpLogs.ResumeLayout(false);
            this.tpArticleActionLogs.ResumeLayout(false);
            this.tpTypos.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.mnuMakeFromTextBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.MenuStrip MnuMain;
        private System.Windows.Forms.StatusStrip StatusMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuGeneral;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private WikiFunctions.Controls.AWBToolTip ToolTip;
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
        private System.Windows.Forms.ToolStripMenuItem speedyDeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stubToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem openPageInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disambiguationToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem followRedirectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSaveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preParseModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListToTextFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphabeticallyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem markAllAsMinorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem birthdeathCatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncategorisedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alphaSortInterwikiLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem unicodifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar MainFormProgressBar;
        private System.Windows.Forms.SaveFileDialog saveXML;
        private System.Windows.Forms.OpenFileDialog openXML;
        private System.Windows.Forms.ToolStripStatusLabel lblBotTimer;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reparseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automaticallyDoAnythingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceTextWithLastEditToolStripMenuItem;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem pasteMoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem convertListToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem removeAllExcessWhitespaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDefaultSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btntsShowHide;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton btntsSave;
        private System.Windows.Forms.ToolStripButton btntsIgnore;
        private System.Windows.Forms.ToolStripButton btntsStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton btntsPreview;
        private System.Windows.Forms.ToolStripButton btntsChanges;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem humanNameDisambigTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripButton btntsFalsePositive;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripLabel lbltsNumberofItems;
        private System.Windows.Forms.ToolStripStatusLabel lblProject;
        private System.Windows.Forms.ToolStripStatusLabel lblUserName;
        private System.Windows.Forms.ToolStripStatusLabel lblUserNotifications;
        private System.Windows.Forms.ToolStripStatusLabel lblTimer;
        private System.Windows.Forms.ToolStripButton btntsStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblEditCount;
        private System.Windows.Forms.ToolStripStatusLabel lblIgnoredArticles;
        private System.Windows.Forms.ToolStripStatusLabel lblEditsPerMin;
        private System.Windows.Forms.ToolStripMenuItem saveAsDefaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bypassAllRedirectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHistoryMenuItem;
        private System.Windows.Forms.ToolStripMenuItem summariesToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon ntfyTray;
        private System.Windows.Forms.ContextMenuStrip mnuNotify;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem humanNameCategoryKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem openSelectionInBrowserToolStripMenuItem;
        private NudgeTimer NudgeTimer;
        private System.Windows.Forms.Timer EditBoxSaveTimer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
        private System.Windows.Forms.ToolStripMenuItem saveTextToFileToolStripMenuItem;
        private WikiFunctions.Controls.Lists.ListMaker listMaker;
        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage tpOptions;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.CheckBox chkSkipIfNoRegexTypo;
        private System.Windows.Forms.LinkLabel EnableRegexTypoFixLinkLabel;
        private System.Windows.Forms.CheckBox chkRegExTypo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSubst;
        private System.Windows.Forms.CheckBox chkSkipWhenNoFAR;
        private System.Windows.Forms.Button btnFindAndReplaceAdvanced;
        private System.Windows.Forms.Button btnMoreFindAndReplce;
        private System.Windows.Forms.CheckBox chkFindandReplace;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btnMoreSkip;
        private System.Windows.Forms.CheckBox chkUnicodifyWhole;
        private System.Windows.Forms.CheckBox chkAutoTagger;
        private System.Windows.Forms.CheckBox chkGeneralFixes;
        private System.Windows.Forms.TabPage tpMoreOptions;
        private System.Windows.Forms.GroupBox ImageGroupBox;
        private System.Windows.Forms.Label lblImageWith;
        private System.Windows.Forms.CheckBox chkSkipNoImgChange;
        private System.Windows.Forms.TextBox txtImageWith;
        private System.Windows.Forms.TextBox txtImageReplace;
        private WikiFunctions.Controls.ComboBoxInvoke cmboImages;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdoPrepend;
        private System.Windows.Forms.RadioButton rdoAppend;
        private System.Windows.Forms.CheckBox chkAppend;
        private System.Windows.Forms.CheckBox chkAppendMetaDataSort;
        private WikiFunctions.Controls.RichTextBoxInvoke txtAppendMessage;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox chkSkipNoCatChange;
        private System.Windows.Forms.CheckBox chkRemoveSortKey;
        private System.Windows.Forms.TextBox txtNewCategory2;
        private System.Windows.Forms.Label label1;
        private WikiFunctions.Controls.ComboBoxInvoke cmboCategorise;
        private System.Windows.Forms.TextBox txtNewCategory;
        private System.Windows.Forms.TabPage tpDab;
        private System.Windows.Forms.Panel panelDab;
        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown udContextChars;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkSkipNoDab;
        private System.Windows.Forms.TextBox txtDabVariants;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.Button btnLoadLinks;
        private System.Windows.Forms.TextBox txtDabLink;
        private System.Windows.Forms.CheckBox chkEnableDab;
        private System.Windows.Forms.TabPage tpBots;
        private System.Windows.Forms.Label lblOnlyBots;
        private System.Windows.Forms.GroupBox groupBox14;
        private System.Windows.Forms.CheckBox chkNudgeSkip;
        private System.Windows.Forms.Button btnResetNudges;
        private System.Windows.Forms.Label lblNudges;
        private System.Windows.Forms.CheckBox chkNudge;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSuppressTag;
        private System.Windows.Forms.CheckBox chkAutoMode;
        private System.Windows.Forms.NumericUpDown nudBotSpeed;
        private System.Windows.Forms.NumericUpDown botEditsStop;
        private System.Windows.Forms.Label lblAutoDelay;
        private System.Windows.Forms.Label lblbotEditsStop;
        private System.Windows.Forms.TabPage tpStart;
        private System.Windows.Forms.CheckBox chkLock;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnFalsePositive;
        private System.Windows.Forms.GroupBox findGroup;
        private System.Windows.Forms.CheckBox chkFindCaseSensitive;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.CheckBox chkFindRegex;
        private System.Windows.Forms.RichTextBox txtFind;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cmboEditSummary;
        private System.Windows.Forms.GroupBox AlertGroup;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.ListBox lbDuplicateWikilinks;
        private System.Windows.Forms.Label lblDuplicateWikilinks;
        private System.Windows.Forms.Label SummaryLabel;
        private System.Windows.Forms.GroupBox SummaryGroup;
        private System.Windows.Forms.Label lblWords;
        private System.Windows.Forms.Label lblInterLinks;
        private System.Windows.Forms.Label lblDates;
        private System.Windows.Forms.Label lblCats;
        private System.Windows.Forms.Label lblImages;
        private System.Windows.Forms.Label lblLinks;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDiff;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.ToolStripButton btntsShowHideParameters;
        private AWBWebBrowser webBrowser;
        private System.Windows.Forms.ToolStripMenuItem undoAllChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTalkPageInBrowserToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuHistory;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshHistoryToolStripMenuItem;
        private System.Windows.Forms.Button btnProtect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.ToolStripMenuItem profilesToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkMinor;
        private System.Windows.Forms.GroupBox groupBox16;
        private System.Windows.Forms.RadioButton radStandby;
        private System.Windows.Forms.RadioButton radHibernate;
        private System.Windows.Forms.RadioButton radRestart;
        private System.Windows.Forms.RadioButton radShutdown;
        private System.Windows.Forms.CheckBox chkShutdown;
        private System.Windows.Forms.Timer ShutdownTimer;
        private System.Windows.Forms.PictureBox BotImage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblUse;
        private System.Windows.Forms.NumericUpDown udNewlineChars;
        private System.Windows.Forms.Label lblNewlineCharacters;
        private System.Windows.Forms.Button btnWatch;
        private System.Windows.Forms.ToolStripButton btntsDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
        private System.Windows.Forms.ToolStripMenuItem loadPluginToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
        private System.Windows.Forms.ToolStripMenuItem managePluginsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceReferenceTagsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuMakeFromTextBox;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxUndo;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxCut;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxCopy;
        private System.Windows.Forms.ToolStripMenuItem menuitemMakeFromTextBoxPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorMakeFromTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testRegexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchDumpSearcherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchListComparerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchListSplitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem externalProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem runUpdaterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetEditSkippedCountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
        private System.Windows.Forms.TabPage tpSkip;
        private System.Windows.Forms.ToolStripMenuItem UsageStatsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeDuplicatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
        private System.Windows.Forms.ToolStripMenuItem focusAtEndOfEditTextBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noSectionEditSummaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restrictDefaultsortChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restrictOrphanTaggingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noMOSComplianceFixesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syntaxHighlightEditBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highlightAllFindToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scrollToAlertsToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox gbPageExisting;
        private System.Windows.Forms.RadioButton radSkipNone;
        private System.Windows.Forms.RadioButton radSkipExistent;
        private System.Windows.Forms.RadioButton radSkipNonExistent;
        private System.Windows.Forms.CheckBox chkSkipWhitespace;
        private System.Windows.Forms.CheckBox chkSkipIfInuse;
        private System.Windows.Forms.CheckBox chkSkipSpamFilter;
        private System.Windows.Forms.CheckBox chkSkipNoChanges;
        private System.Windows.Forms.CheckBox chkSkipNoPageLinks;
        private System.Windows.Forms.CheckBox chkSkipGeneralFixes;
        private System.Windows.Forms.CheckBox chkSkipMinorGeneralFixes;
        private System.Windows.Forms.CheckBox chkSkipCasing;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator30;
        private System.Windows.Forms.ToolStripMenuItem commentSelectedToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSkipIfRedirect;
        private System.Windows.Forms.CheckBox chkSkipIfNoAlerts;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableTheToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHidePanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enlargeEditAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHideEditToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem displayfalsePositivesButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PasteMore1;
        private System.Windows.Forms.ToolStripMenuItem PasteMore2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PasteMore3;
        private System.Windows.Forms.ToolStripMenuItem PasteMore4;
        private System.Windows.Forms.ToolStripMenuItem PasteMore5;
        private System.Windows.Forms.ToolStripMenuItem PasteMore6;
        private System.Windows.Forms.ToolStripMenuItem PasteMore7;
        private System.Windows.Forms.ToolStripMenuItem PasteMore8;
        private System.Windows.Forms.ToolStripMenuItem PasteMore9;
        private System.Windows.Forms.ToolStripMenuItem PasteMore10;
        private System.Windows.Forms.ToolStripStatusLabel lblPagesPerMin;
        private System.Windows.Forms.ToolStripMenuItem profileTyposToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox addToWatchList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator31;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
        private System.Windows.Forms.ToolStripMenuItem invalidateCacheToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearCurrentListToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblNewArticles;
        private System.Windows.Forms.CheckBox chkSkipOnlyMinorFaR;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem submitStatToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.ListBox lbAlerts;
        private System.Windows.Forms.ToolStripMenuItem logOutToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSkipCosmetic;
        private System.Windows.Forms.TabControl EditBoxTab;
        private System.Windows.Forms.TabPage tpEdit;
        private System.Windows.Forms.PictureBox imgComment;
        private System.Windows.Forms.PictureBox imgSub;
        private System.Windows.Forms.PictureBox imgSup;
        private System.Windows.Forms.PictureBox imgStrike;
        private System.Windows.Forms.PictureBox imgRedirect;
        private System.Windows.Forms.PictureBox imgHr;
        private System.Windows.Forms.PictureBox imgNowiki;
        private System.Windows.Forms.PictureBox imgMath;
        private System.Windows.Forms.PictureBox imgExtlink;
        private System.Windows.Forms.PictureBox imgLink;
        private System.Windows.Forms.PictureBox imgItalics;
        private System.Windows.Forms.PictureBox imgBold;
        private WikiFunctions.Controls.ArticleTextBox txtEdit;
        private System.Windows.Forms.RichTextBox txtReviewEditSummary;
        private System.Windows.Forms.TabPage tpHistory;
        private AWBWebBrowser webBrowserHistory;
        private System.Windows.Forms.TabPage tpLinks;
        private AWBWebBrowser webBrowserLinks;
        private System.Windows.Forms.TabPage tpLogs;
        private WikiFunctions.Logging.LogControl logControl;
        private System.Windows.Forms.TabPage tpArticleActionLogs;
        private WikiFunctions.Logging.ArticleActionLogControl articleActionLogControl1;
        private System.Windows.Forms.TabPage tpTypos;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox9;
        private WikiFunctions.Controls.TypoStatsControl CurrentTypoStats;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label lblTypoRatio;
        private WikiFunctions.Controls.TypoStatsControl OverallTypoStats;
        private System.Windows.Forms.Label lblNoChange;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblOverallTypos;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbRegexSkip;
        private WikiFunctions.Controls.PageNotContainsControl skipIfNotContains;
        private WikiFunctions.Controls.PageContainsControl skipIfContains;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator32;
        private System.Windows.Forms.ToolStripMenuItem cEvalToolStripMenuItem;
    }
}
