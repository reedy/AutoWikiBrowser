//$Header: /cvsroot/autowikibrowser/src/Main.Designer.cs,v 1.107 2006/07/06 14:06:54 wikibluemoose Exp $

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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIgnoreWhenNoFAR = new System.Windows.Forms.CheckBox();
            this.btnFindAndReplaceAdvanced = new System.Windows.Forms.Button();
            this.btnMoreFindAndReplce = new System.Windows.Forms.Button();
            this.chkFindandReplace = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkUnicodifyWhole = new System.Windows.Forms.CheckBox();
            this.chkAutoTagger = new System.Windows.Forms.CheckBox();
            this.chkGeneralParse = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.chkSkipNoChanges = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkIgnoreIsRegex = new System.Windows.Forms.CheckBox();
            this.chkOnlyIfContains = new System.Windows.Forms.CheckBox();
            this.txtIgnoreIfContains = new System.Windows.Forms.TextBox();
            this.txtOnlyIfContains = new System.Windows.Forms.TextBox();
            this.chkIgnoreIfContains = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cmboCategorise = new System.Windows.Forms.ComboBox();
            this.txtNewCategory = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ImageGroupBox = new System.Windows.Forms.GroupBox();
            this.lblImageWith = new System.Windows.Forms.Label();
            this.lblImageReplace = new System.Windows.Forms.Label();
            this.txtImageWith = new System.Windows.Forms.TextBox();
            this.txtImageReplace = new System.Windows.Forms.TextBox();
            this.cmboImages = new System.Windows.Forms.ComboBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.chkSuppressTag = new System.Windows.Forms.CheckBox();
            this.chkQuickSave = new System.Windows.Forms.CheckBox();
            this.chkAutoMode = new System.Windows.Forms.CheckBox();
            this.nudBotSpeed = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoPrepend = new System.Windows.Forms.RadioButton();
            this.rdoAppend = new System.Windows.Forms.RadioButton();
            this.chkAppend = new System.Windows.Forms.CheckBox();
            this.txtAppendMessage = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnFalsePositive = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.chkFindCaseSensitive = new System.Windows.Forms.CheckBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.chkFindRegex = new System.Windows.Forms.CheckBox();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblTimer = new System.Windows.Forms.Label();
            this.cmboEditSummary = new System.Windows.Forms.ComboBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.lbDuplicateWikilinks = new System.Windows.Forms.ListBox();
            this.lblDuplicateWikilinks = new System.Windows.Forms.Label();
            this.lblWarn = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblLength = new System.Windows.Forms.Label();
            this.lblInterLinks = new System.Windows.Forms.Label();
            this.lblCats = new System.Windows.Forms.Label();
            this.lblImages = new System.Windows.Forms.Label();
            this.lblLinks = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnDiff = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
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
            this.fromWhatlinkshereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromLinksOnPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromImageLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chkWLHRedirects = new System.Windows.Forms.CheckBox();
            this.txtSelectSource = new System.Windows.Forms.TextBox();
            this.lblNumberOfArticles = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemoveArticle = new System.Windows.Forms.Button();
            this.btnArticlesListClear = new System.Windows.Forms.Button();
            this.btnArticlesListSave = new System.Windows.Forms.Button();
            this.txtNewArticle = new System.Windows.Forms.TextBox();
            this.lblSourceSelect = new System.Windows.Forms.Label();
            this.btnMakeList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbArticles = new System.Windows.Forms.ListBox();
            this.cmboSourceSelect = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
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
            this.removeAllExcessWhitespaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reparseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openPageInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceTextWithLastEditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadDefaultSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.selectProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterOutNonMainSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.specialFilterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sortAlphabeticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListToTextFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.launchDumpSearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchListComparerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableTheToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bypassRedirectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreNonexistentPagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doNotAutomaticallyDoAnythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewInsteadOfDiffToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.markAllAsMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAllToWatchlistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.alphaSortInterwikiLinksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.addIgnoredToLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchIRCMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblBotTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveXML = new System.Windows.Forms.SaveFileDialog();
            this.openXML = new System.Windows.Forms.OpenFileDialog();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.lblProject = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btntsShowHide = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsSave = new System.Windows.Forms.ToolStripButton();
            this.btntsIgnore = new System.Windows.Forms.ToolStripButton();
            this.btntsStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.btntsPreview = new System.Windows.Forms.ToolStripButton();
            this.btntsChanges = new System.Windows.Forms.ToolStripButton();
            this.webBrowserEdit = new WikiFunctions.WebControl();
            this.btnFilter = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.ImageGroupBox.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.mnuListBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.mnuTextBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(219, 5);
            this.tabControl1.MinimumSize = new System.Drawing.Size(276, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(276, 368);
            this.tabControl1.TabIndex = 666;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.groupBox6);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(268, 342);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "(2) Set options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkIgnoreWhenNoFAR);
            this.groupBox1.Controls.Add(this.btnFindAndReplaceAdvanced);
            this.groupBox1.Controls.Add(this.btnMoreFindAndReplce);
            this.groupBox1.Controls.Add(this.chkFindandReplace);
            this.groupBox1.Location = new System.Drawing.Point(6, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 68);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Find and replace";
            // 
            // chkIgnoreWhenNoFAR
            // 
            this.chkIgnoreWhenNoFAR.AutoSize = true;
            this.chkIgnoreWhenNoFAR.Checked = true;
            this.chkIgnoreWhenNoFAR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreWhenNoFAR.Enabled = false;
            this.chkIgnoreWhenNoFAR.Location = new System.Drawing.Point(6, 43);
            this.chkIgnoreWhenNoFAR.Name = "chkIgnoreWhenNoFAR";
            this.chkIgnoreWhenNoFAR.Size = new System.Drawing.Size(221, 17);
            this.chkIgnoreWhenNoFAR.TabIndex = 10;
            this.chkIgnoreWhenNoFAR.Text = "Ignore article when no replacement made";
            this.chkIgnoreWhenNoFAR.UseVisualStyleBackColor = true;
            // 
            // btnFindAndReplaceAdvanced
            // 
            this.btnFindAndReplaceAdvanced.Enabled = false;
            this.btnFindAndReplaceAdvanced.Location = new System.Drawing.Point(169, 15);
            this.btnFindAndReplaceAdvanced.Name = "btnFindAndReplaceAdvanced";
            this.btnFindAndReplaceAdvanced.Size = new System.Drawing.Size(82, 23);
            this.btnFindAndReplaceAdvanced.TabIndex = 9;
            this.btnFindAndReplaceAdvanced.Text = "Advanced";
            this.btnFindAndReplaceAdvanced.UseVisualStyleBackColor = true;
            this.btnFindAndReplaceAdvanced.Click += new System.EventHandler(this.btnFindAndReplaceAdvanced_Click);
            // 
            // btnMoreFindAndReplce
            // 
            this.btnMoreFindAndReplce.Enabled = false;
            this.btnMoreFindAndReplce.Location = new System.Drawing.Point(77, 15);
            this.btnMoreFindAndReplce.Name = "btnMoreFindAndReplce";
            this.btnMoreFindAndReplce.Size = new System.Drawing.Size(82, 23);
            this.btnMoreFindAndReplce.TabIndex = 8;
            this.btnMoreFindAndReplce.Text = "Normal";
            this.btnMoreFindAndReplce.UseVisualStyleBackColor = true;
            this.btnMoreFindAndReplce.Click += new System.EventHandler(this.btnMoreFindAndReplce_Click);
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
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkUnicodifyWhole);
            this.groupBox6.Controls.Add(this.chkAutoTagger);
            this.groupBox6.Controls.Add(this.chkGeneralParse);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(259, 65);
            this.groupBox6.TabIndex = 17;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "General";
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
            // chkAutoTagger
            // 
            this.chkAutoTagger.AutoSize = true;
            this.chkAutoTagger.Checked = true;
            this.chkAutoTagger.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoTagger.Location = new System.Drawing.Point(119, 16);
            this.chkAutoTagger.Name = "chkAutoTagger";
            this.chkAutoTagger.Size = new System.Drawing.Size(66, 17);
            this.chkAutoTagger.TabIndex = 27;
            this.chkAutoTagger.Text = "Auto tag";
            this.toolTip1.SetToolTip(this.chkAutoTagger, "Automatically add {{wikify}} and {{stub}} tags when appropriate");
            this.chkAutoTagger.UseVisualStyleBackColor = true;
            // 
            // chkGeneralParse
            // 
            this.chkGeneralParse.AutoSize = true;
            this.chkGeneralParse.Checked = true;
            this.chkGeneralParse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGeneralParse.Location = new System.Drawing.Point(6, 16);
            this.chkGeneralParse.Name = "chkGeneralParse";
            this.chkGeneralParse.Size = new System.Drawing.Size(114, 17);
            this.chkGeneralParse.TabIndex = 11;
            this.chkGeneralParse.Tag = "Apply general minor fixes";
            this.chkGeneralParse.Text = "Apply general fixes";
            this.toolTip1.SetToolTip(this.chkGeneralParse, "Apply general minor fixes");
            this.chkGeneralParse.UseVisualStyleBackColor = true;
            this.chkGeneralParse.CheckedChanged += new System.EventHandler(this.chkGeneralParse_CheckedChanged);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.chkSkipNoChanges);
            this.groupBox8.Controls.Add(this.chkIgnoreCaseSensitive);
            this.groupBox8.Controls.Add(this.chkIgnoreIsRegex);
            this.groupBox8.Controls.Add(this.chkOnlyIfContains);
            this.groupBox8.Controls.Add(this.txtIgnoreIfContains);
            this.groupBox8.Controls.Add(this.txtOnlyIfContains);
            this.groupBox8.Controls.Add(this.chkIgnoreIfContains);
            this.groupBox8.Location = new System.Drawing.Point(6, 203);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(259, 133);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Skip articles";
            // 
            // chkSkipNoChanges
            // 
            this.chkSkipNoChanges.AutoSize = true;
            this.chkSkipNoChanges.Location = new System.Drawing.Point(4, 110);
            this.chkSkipNoChanges.Name = "chkSkipNoChanges";
            this.chkSkipNoChanges.Size = new System.Drawing.Size(195, 17);
            this.chkSkipNoChanges.TabIndex = 30;
            this.chkSkipNoChanges.Text = "Skip articles when no change made";
            this.toolTip1.SetToolTip(this.chkSkipNoChanges, "Automatically skips articles when no changes were automatically made");
            this.chkSkipNoChanges.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreCaseSensitive
            // 
            this.chkIgnoreCaseSensitive.AutoSize = true;
            this.chkIgnoreCaseSensitive.Location = new System.Drawing.Point(69, 13);
            this.chkIgnoreCaseSensitive.Name = "chkIgnoreCaseSensitive";
            this.chkIgnoreCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkIgnoreCaseSensitive.TabIndex = 29;
            this.chkIgnoreCaseSensitive.Text = "Case sensitive";
            this.chkIgnoreCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreIsRegex
            // 
            this.chkIgnoreIsRegex.AutoSize = true;
            this.chkIgnoreIsRegex.Location = new System.Drawing.Point(169, 13);
            this.chkIgnoreIsRegex.Name = "chkIgnoreIsRegex";
            this.chkIgnoreIsRegex.Size = new System.Drawing.Size(82, 17);
            this.chkIgnoreIsRegex.TabIndex = 28;
            this.chkIgnoreIsRegex.Text = "Are regexes";
            this.toolTip1.SetToolTip(this.chkIgnoreIsRegex, "Enables regular expressions for the \"Ignore if contains\".");
            this.chkIgnoreIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkOnlyIfContains
            // 
            this.chkOnlyIfContains.AutoSize = true;
            this.chkOnlyIfContains.Location = new System.Drawing.Point(4, 64);
            this.chkOnlyIfContains.Name = "chkOnlyIfContains";
            this.chkOnlyIfContains.Size = new System.Drawing.Size(130, 17);
            this.chkOnlyIfContains.TabIndex = 4;
            this.chkOnlyIfContains.Text = "Skip if doesn\'t contain";
            this.chkOnlyIfContains.UseVisualStyleBackColor = true;
            this.chkOnlyIfContains.CheckedChanged += new System.EventHandler(this.chkOnlyIfContains_CheckedChanged);
            // 
            // txtIgnoreIfContains
            // 
            this.txtIgnoreIfContains.Enabled = false;
            this.txtIgnoreIfContains.Location = new System.Drawing.Point(134, 36);
            this.txtIgnoreIfContains.Name = "txtIgnoreIfContains";
            this.txtIgnoreIfContains.Size = new System.Drawing.Size(117, 20);
            this.txtIgnoreIfContains.TabIndex = 23;
            // 
            // txtOnlyIfContains
            // 
            this.txtOnlyIfContains.Enabled = false;
            this.txtOnlyIfContains.Location = new System.Drawing.Point(134, 62);
            this.txtOnlyIfContains.Name = "txtOnlyIfContains";
            this.txtOnlyIfContains.Size = new System.Drawing.Size(117, 20);
            this.txtOnlyIfContains.TabIndex = 6;
            // 
            // chkIgnoreIfContains
            // 
            this.chkIgnoreIfContains.AutoSize = true;
            this.chkIgnoreIfContains.Location = new System.Drawing.Point(4, 38);
            this.chkIgnoreIfContains.Name = "chkIgnoreIfContains";
            this.chkIgnoreIfContains.Size = new System.Drawing.Size(101, 17);
            this.chkIgnoreIfContains.TabIndex = 22;
            this.chkIgnoreIfContains.Text = "Skip if contains:";
            this.toolTip1.SetToolTip(this.chkIgnoreIfContains, "Wont make changes if article contains this text");
            this.chkIgnoreIfContains.UseVisualStyleBackColor = true;
            this.chkIgnoreIfContains.CheckedChanged += new System.EventHandler(this.chkIgnoreIfContains_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cmboCategorise);
            this.groupBox5.Controls.Add(this.txtNewCategory);
            this.groupBox5.Location = new System.Drawing.Point(6, 151);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(259, 46);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Categorisation";
            // 
            // cmboCategorise
            // 
            this.cmboCategorise.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboCategorise.FormattingEnabled = true;
            this.cmboCategorise.Items.AddRange(new object[] {
            "None",
            "Replace category with:",
            "Add new category:",
            "Remove category:"});
            this.cmboCategorise.Location = new System.Drawing.Point(7, 16);
            this.cmboCategorise.Name = "cmboCategorise";
            this.cmboCategorise.Size = new System.Drawing.Size(140, 21);
            this.cmboCategorise.TabIndex = 3;
            this.cmboCategorise.SelectedIndexChanged += new System.EventHandler(this.cmboCategorise_SelectedIndexChanged);
            // 
            // txtNewCategory
            // 
            this.txtNewCategory.Enabled = false;
            this.txtNewCategory.Location = new System.Drawing.Point(153, 16);
            this.txtNewCategory.Name = "txtNewCategory";
            this.txtNewCategory.Size = new System.Drawing.Size(100, 20);
            this.txtNewCategory.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtNewCategory, "The new category, the old one is specified when you make the list");
            this.txtNewCategory.DoubleClick += new System.EventHandler(this.txtNewCategory_DoubleClick);
            this.txtNewCategory.Leave += new System.EventHandler(this.txtNewCategory_Leave);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ImageGroupBox);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(268, 342);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "More options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ImageGroupBox
            // 
            this.ImageGroupBox.Controls.Add(this.lblImageWith);
            this.ImageGroupBox.Controls.Add(this.lblImageReplace);
            this.ImageGroupBox.Controls.Add(this.txtImageWith);
            this.ImageGroupBox.Controls.Add(this.txtImageReplace);
            this.ImageGroupBox.Controls.Add(this.cmboImages);
            this.ImageGroupBox.Location = new System.Drawing.Point(6, 206);
            this.ImageGroupBox.Name = "ImageGroupBox";
            this.ImageGroupBox.Size = new System.Drawing.Size(256, 100);
            this.ImageGroupBox.TabIndex = 27;
            this.ImageGroupBox.TabStop = false;
            this.ImageGroupBox.Text = "Images";
            // 
            // lblImageWith
            // 
            this.lblImageWith.AutoSize = true;
            this.lblImageWith.Location = new System.Drawing.Point(18, 77);
            this.lblImageWith.Name = "lblImageWith";
            this.lblImageWith.Size = new System.Drawing.Size(63, 13);
            this.lblImageWith.TabIndex = 4;
            this.lblImageWith.Text = "With image:";
            // 
            // lblImageReplace
            // 
            this.lblImageReplace.AutoSize = true;
            this.lblImageReplace.Location = new System.Drawing.Point(4, 52);
            this.lblImageReplace.Name = "lblImageReplace";
            this.lblImageReplace.Size = new System.Drawing.Size(81, 13);
            this.lblImageReplace.TabIndex = 3;
            this.lblImageReplace.Text = "Replace image:";
            // 
            // txtImageWith
            // 
            this.txtImageWith.Location = new System.Drawing.Point(87, 74);
            this.txtImageWith.Name = "txtImageWith";
            this.txtImageWith.Size = new System.Drawing.Size(163, 20);
            this.txtImageWith.TabIndex = 2;
            this.txtImageWith.Leave += new System.EventHandler(this.txtImageWith_Leave);
            // 
            // txtImageReplace
            // 
            this.txtImageReplace.Location = new System.Drawing.Point(87, 49);
            this.txtImageReplace.Name = "txtImageReplace";
            this.txtImageReplace.Size = new System.Drawing.Size(163, 20);
            this.txtImageReplace.TabIndex = 1;
            this.txtImageReplace.Leave += new System.EventHandler(this.txtImageReplace_Leave);
            // 
            // cmboImages
            // 
            this.cmboImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboImages.FormattingEnabled = true;
            this.cmboImages.Items.AddRange(new object[] {
            "None",
            "Replace"});
            this.cmboImages.Location = new System.Drawing.Point(87, 20);
            this.cmboImages.Name = "cmboImages";
            this.cmboImages.Size = new System.Drawing.Size(163, 21);
            this.cmboImages.TabIndex = 0;
            this.cmboImages.SelectedIndexChanged += new System.EventHandler(this.cmboImages_SelectedIndexChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.chkSuppressTag);
            this.groupBox7.Controls.Add(this.chkQuickSave);
            this.groupBox7.Controls.Add(this.chkAutoMode);
            this.groupBox7.Controls.Add(this.nudBotSpeed);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Location = new System.Drawing.Point(5, 139);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(257, 64);
            this.groupBox7.TabIndex = 26;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Auto mode";
            // 
            // chkSuppressTag
            // 
            this.chkSuppressTag.AutoSize = true;
            this.chkSuppressTag.Enabled = false;
            this.chkSuppressTag.Location = new System.Drawing.Point(96, 40);
            this.chkSuppressTag.Name = "chkSuppressTag";
            this.chkSuppressTag.Size = new System.Drawing.Size(136, 17);
            this.chkSuppressTag.TabIndex = 28;
            this.chkSuppressTag.Text = "Suppress \"using AWB\"";
            this.chkSuppressTag.UseVisualStyleBackColor = true;
            // 
            // chkQuickSave
            // 
            this.chkQuickSave.AutoSize = true;
            this.chkQuickSave.Enabled = false;
            this.chkQuickSave.Location = new System.Drawing.Point(9, 40);
            this.chkQuickSave.Name = "chkQuickSave";
            this.chkQuickSave.Size = new System.Drawing.Size(80, 17);
            this.chkQuickSave.TabIndex = 27;
            this.chkQuickSave.Text = "Quick save";
            this.toolTip1.SetToolTip(this.chkQuickSave, "Saves without loading diff first");
            this.chkQuickSave.UseVisualStyleBackColor = true;
            // 
            // chkAutoMode
            // 
            this.chkAutoMode.AutoSize = true;
            this.chkAutoMode.Enabled = false;
            this.chkAutoMode.Location = new System.Drawing.Point(9, 16);
            this.chkAutoMode.Name = "chkAutoMode";
            this.chkAutoMode.Size = new System.Drawing.Size(77, 17);
            this.chkAutoMode.TabIndex = 26;
            this.chkAutoMode.Text = "Auto mode";
            this.chkAutoMode.UseVisualStyleBackColor = true;
            this.chkAutoMode.CheckedChanged += new System.EventHandler(this.chkAutoMode_CheckedChanged);
            // 
            // nudBotSpeed
            // 
            this.nudBotSpeed.Enabled = false;
            this.nudBotSpeed.Location = new System.Drawing.Point(180, 15);
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
            15,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(91, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Delay (seconds)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdoPrepend);
            this.groupBox4.Controls.Add(this.rdoAppend);
            this.groupBox4.Controls.Add(this.chkAppend);
            this.groupBox4.Controls.Add(this.txtAppendMessage);
            this.groupBox4.Location = new System.Drawing.Point(5, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(257, 124);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Messaging";
            // 
            // rdoPrepend
            // 
            this.rdoPrepend.AutoSize = true;
            this.rdoPrepend.Enabled = false;
            this.rdoPrepend.Location = new System.Drawing.Point(74, 39);
            this.rdoPrepend.Name = "rdoPrepend";
            this.rdoPrepend.Size = new System.Drawing.Size(65, 17);
            this.rdoPrepend.TabIndex = 3;
            this.rdoPrepend.Text = "Prepend";
            this.toolTip1.SetToolTip(this.rdoPrepend, "Add text to beginning of talk page");
            this.rdoPrepend.UseVisualStyleBackColor = true;
            // 
            // rdoAppend
            // 
            this.rdoAppend.AutoSize = true;
            this.rdoAppend.Checked = true;
            this.rdoAppend.Enabled = false;
            this.rdoAppend.Location = new System.Drawing.Point(6, 39);
            this.rdoAppend.Name = "rdoAppend";
            this.rdoAppend.Size = new System.Drawing.Size(62, 17);
            this.rdoAppend.TabIndex = 2;
            this.rdoAppend.TabStop = true;
            this.rdoAppend.Text = "Append";
            this.toolTip1.SetToolTip(this.rdoAppend, "Add text to end of talk page");
            this.rdoAppend.UseVisualStyleBackColor = true;
            // 
            // chkAppend
            // 
            this.chkAppend.AutoSize = true;
            this.chkAppend.Location = new System.Drawing.Point(6, 18);
            this.chkAppend.Name = "chkAppend";
            this.chkAppend.Size = new System.Drawing.Size(191, 17);
            this.chkAppend.TabIndex = 0;
            this.chkAppend.Text = "Append message  (talk pages only)";
            this.toolTip1.SetToolTip(this.chkAppend, "For appending a message to a user talk page for example");
            this.chkAppend.UseVisualStyleBackColor = true;
            this.chkAppend.CheckedChanged += new System.EventHandler(this.chkAppend_CheckedChanged);
            // 
            // txtAppendMessage
            // 
            this.txtAppendMessage.Enabled = false;
            this.txtAppendMessage.Location = new System.Drawing.Point(6, 62);
            this.txtAppendMessage.Multiline = true;
            this.txtAppendMessage.Name = "txtAppendMessage";
            this.txtAppendMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAppendMessage.Size = new System.Drawing.Size(245, 56);
            this.txtAppendMessage.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtAppendMessage, "Message, including title in wiki format");
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnFalsePositive);
            this.tabPage4.Controls.Add(this.groupBox10);
            this.tabPage4.Controls.Add(this.btnStop);
            this.tabPage4.Controls.Add(this.lblTimer);
            this.tabPage4.Controls.Add(this.cmboEditSummary);
            this.tabPage4.Controls.Add(this.groupBox9);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.groupBox3);
            this.tabPage4.Controls.Add(this.btnStart);
            this.tabPage4.Controls.Add(this.btnPreview);
            this.tabPage4.Controls.Add(this.btnApply);
            this.tabPage4.Controls.Add(this.btnDiff);
            this.tabPage4.Controls.Add(this.btnIgnore);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(268, 342);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "(3) Start";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnFalsePositive
            // 
            this.btnFalsePositive.Location = new System.Drawing.Point(160, 60);
            this.btnFalsePositive.Name = "btnFalsePositive";
            this.btnFalsePositive.Size = new System.Drawing.Size(49, 20);
            this.btnFalsePositive.TabIndex = 29;
            this.btnFalsePositive.Text = "False";
            this.btnFalsePositive.UseVisualStyleBackColor = true;
            this.btnFalsePositive.Visible = false;
            this.btnFalsePositive.Click += new System.EventHandler(this.btnFalsePositive_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.chkFindCaseSensitive);
            this.groupBox10.Controls.Add(this.btnFind);
            this.groupBox10.Controls.Add(this.chkFindRegex);
            this.groupBox10.Controls.Add(this.txtFind);
            this.groupBox10.Location = new System.Drawing.Point(152, 240);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(110, 96);
            this.groupBox10.TabIndex = 16;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Find";
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
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // btnStop
            // 
            this.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(213, 60);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(49, 20);
            this.btnStop.TabIndex = 28;
            this.btnStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btnStop, "Stops everything  (Shortcut escape)");
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblTimer
            // 
            this.lblTimer.Location = new System.Drawing.Point(222, 229);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(37, 13);
            this.lblTimer.TabIndex = 27;
            // 
            // cmboEditSummary
            // 
            this.cmboEditSummary.FormattingEnabled = true;
            this.cmboEditSummary.Items.AddRange(new object[] {
            "clean up",
            "re-categorisation per [[WP:CFD|CFD]]",
            "clean up and  re-categorisation per [[WP:CFD|CFD]]",
            "removing category per [[WP:CFD|CFD]]",
            "[[Wikipedia:Template substitution|subst:\'ing]]",
            "stub sorting",
            "clean up + typo fix",
            "bad link repair.  [[Wikipedia:Bad links|You can help!]]",
            "Fixing [[Wikipedia:Disambiguation pages with links|links to disambiguation pages]" +
                "]",
            "Unicodifying"});
            this.cmboEditSummary.Location = new System.Drawing.Point(55, 6);
            this.cmboEditSummary.MaxLength = 155;
            this.cmboEditSummary.Name = "cmboEditSummary";
            this.cmboEditSummary.Size = new System.Drawing.Size(210, 21);
            this.cmboEditSummary.TabIndex = 7;
            this.cmboEditSummary.Text = "clean up";
            this.toolTip1.SetToolTip(this.cmboEditSummary, "Write or select an edit summary");
            this.cmboEditSummary.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbEditSummary_KeyDown);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.lbDuplicateWikilinks);
            this.groupBox9.Controls.Add(this.lblDuplicateWikilinks);
            this.groupBox9.Controls.Add(this.lblWarn);
            this.groupBox9.Location = new System.Drawing.Point(6, 131);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(139, 205);
            this.groupBox9.TabIndex = 24;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Alerts";
            // 
            // lbDuplicateWikilinks
            // 
            this.lbDuplicateWikilinks.FormattingEnabled = true;
            this.lbDuplicateWikilinks.Location = new System.Drawing.Point(6, 130);
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
            this.lblDuplicateWikilinks.Location = new System.Drawing.Point(6, 114);
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
            this.lblWarn.Size = new System.Drawing.Size(127, 90);
            this.lblWarn.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 17);
            this.label6.TabIndex = 20;
            this.label6.Text = "Summary";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblLength);
            this.groupBox3.Controls.Add(this.lblInterLinks);
            this.groupBox3.Controls.Add(this.lblCats);
            this.groupBox3.Controls.Add(this.lblImages);
            this.groupBox3.Controls.Add(this.lblLinks);
            this.groupBox3.Location = new System.Drawing.Point(6, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(139, 101);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Article statistics";
            // 
            // lblLength
            // 
            this.lblLength.AutoSize = true;
            this.lblLength.Location = new System.Drawing.Point(6, 18);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(61, 13);
            this.lblLength.TabIndex = 17;
            this.lblLength.Text = "Characters:";
            // 
            // lblInterLinks
            // 
            this.lblInterLinks.AutoSize = true;
            this.lblInterLinks.Location = new System.Drawing.Point(5, 86);
            this.lblInterLinks.Name = "lblInterLinks";
            this.lblInterLinks.Size = new System.Drawing.Size(55, 13);
            this.lblInterLinks.TabIndex = 22;
            this.lblInterLinks.Text = "Inter links:";
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
            // btnStart
            // 
            this.btnStart.Enabled = false;
            this.btnStart.Location = new System.Drawing.Point(160, 33);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(102, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Tag = "Start the process";
            this.btnStart.Text = "Start the process (Shortcut ctrl + s)";
            this.toolTip1.SetToolTip(this.btnStart, "Start the process!");
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Enabled = false;
            this.btnPreview.Location = new System.Drawing.Point(160, 98);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(102, 23);
            this.btnPreview.TabIndex = 15;
            this.btnPreview.Text = "Preview";
            this.toolTip1.SetToolTip(this.btnPreview, "Show/refresh the preview");
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApply.Location = new System.Drawing.Point(161, 197);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(102, 32);
            this.btnApply.TabIndex = 10;
            this.btnApply.Tag = "Apply all the changes";
            this.btnApply.Text = "Save";
            this.toolTip1.SetToolTip(this.btnApply, "Save the changes and move on  (Shortcut ctrl + s)");
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnDiff
            // 
            this.btnDiff.Enabled = false;
            this.btnDiff.Location = new System.Drawing.Point(160, 125);
            this.btnDiff.Name = "btnDiff";
            this.btnDiff.Size = new System.Drawing.Size(102, 23);
            this.btnDiff.TabIndex = 16;
            this.btnDiff.Text = "Show changes";
            this.toolTip1.SetToolTip(this.btnDiff, "Show/refresh the diff");
            this.btnDiff.UseVisualStyleBackColor = true;
            this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Enabled = false;
            this.btnIgnore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIgnore.Location = new System.Drawing.Point(161, 162);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(102, 32);
            this.btnIgnore.TabIndex = 14;
            this.btnIgnore.Text = "Ignore";
            this.toolTip1.SetToolTip(this.btnIgnore, "Ignore the current article and move on to the next  (Shortcut ctrl + i)");
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.ContextMenuStrip = this.mnuListBox;
            this.groupBox2.Controls.Add(this.btnFilter);
            this.groupBox2.Controls.Add(this.chkWLHRedirects);
            this.groupBox2.Controls.Add(this.txtSelectSource);
            this.groupBox2.Controls.Add(this.lblNumberOfArticles);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnRemoveArticle);
            this.groupBox2.Controls.Add(this.btnArticlesListClear);
            this.groupBox2.Controls.Add(this.btnArticlesListSave);
            this.groupBox2.Controls.Add(this.txtNewArticle);
            this.groupBox2.Controls.Add(this.lblSourceSelect);
            this.groupBox2.Controls.Add(this.btnMakeList);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lbArticles);
            this.groupBox2.Controls.Add(this.cmboSourceSelect);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 366);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "(1) Make list";
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
            this.clearToolStripMenuItem1});
            this.mnuListBox.Name = "contextMenuStrip2";
            this.mnuListBox.Size = new System.Drawing.Size(195, 214);
            this.mnuListBox.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListBox_Opening);
            // 
            // filterOutNonMainSpaceArticlesToolStripMenuItem
            // 
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Name = "filterOutNonMainSpaceArticlesToolStripMenuItem";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Text = "Filter out non main space";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.ToolTipText = "Filter out articles that are not in the main namespace";
            this.filterOutNonMainSpaceArticlesToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceArticlesToolStripMenuItem_Click);
            // 
            // specialFilterToolStripMenuItem
            // 
            this.specialFilterToolStripMenuItem.Name = "specialFilterToolStripMenuItem";
            this.specialFilterToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.specialFilterToolStripMenuItem.Text = "Filter";
            this.specialFilterToolStripMenuItem.ToolTipText = "Filter articles by namespace";
            this.specialFilterToolStripMenuItem.Click += new System.EventHandler(this.specialFilterToolStripMenuItem_Click);
            // 
            // convertToTalkPagesToolStripMenuItem
            // 
            this.convertToTalkPagesToolStripMenuItem.Name = "convertToTalkPagesToolStripMenuItem";
            this.convertToTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.convertToTalkPagesToolStripMenuItem.Text = "Convert to talk pages";
            this.convertToTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace to talk";
            this.convertToTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertToTalkPagesToolStripMenuItem_Click);
            // 
            // convertFromTalkPagesToolStripMenuItem
            // 
            this.convertFromTalkPagesToolStripMenuItem.Name = "convertFromTalkPagesToolStripMenuItem";
            this.convertFromTalkPagesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.convertFromTalkPagesToolStripMenuItem.Text = "Convert from talk pages";
            this.convertFromTalkPagesToolStripMenuItem.ToolTipText = "Convert namespace of talk pages to associated article";
            this.convertFromTalkPagesToolStripMenuItem.Click += new System.EventHandler(this.convertFromTalkPagesToolStripMenuItem_Click);
            // 
            // sortAlphebeticallyMenuItem
            // 
            this.sortAlphebeticallyMenuItem.Name = "sortAlphebeticallyMenuItem";
            this.sortAlphebeticallyMenuItem.Size = new System.Drawing.Size(194, 22);
            this.sortAlphebeticallyMenuItem.Text = "Sort alphabetically";
            this.sortAlphebeticallyMenuItem.ToolTipText = "Sorts the list alphabetically";
            this.sortAlphebeticallyMenuItem.Click += new System.EventHandler(this.sortAlphebeticallyToolStripMenuItem_Click);
            // 
            // saveListToTextFileToolStripMenuItem1
            // 
            this.saveListToTextFileToolStripMenuItem1.Name = "saveListToTextFileToolStripMenuItem1";
            this.saveListToTextFileToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
            this.saveListToTextFileToolStripMenuItem1.Text = "Save list to file";
            this.saveListToTextFileToolStripMenuItem1.ToolTipText = "Saves list to a text file";
            this.saveListToTextFileToolStripMenuItem1.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem1_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(191, 6);
            // 
            // addSelectedToListToolStripMenuItem
            // 
            this.addSelectedToListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromCategoryToolStripMenuItem,
            this.fromWhatlinkshereToolStripMenuItem,
            this.fromLinksOnPageToolStripMenuItem,
            this.fromImageLinksToolStripMenuItem});
            this.addSelectedToListToolStripMenuItem.Name = "addSelectedToListToolStripMenuItem";
            this.addSelectedToListToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.addSelectedToListToolStripMenuItem.Text = "Add selected to list...";
            // 
            // fromCategoryToolStripMenuItem
            // 
            this.fromCategoryToolStripMenuItem.Name = "fromCategoryToolStripMenuItem";
            this.fromCategoryToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.fromCategoryToolStripMenuItem.Text = "From category";
            this.fromCategoryToolStripMenuItem.Click += new System.EventHandler(this.fromCategoryToolStripMenuItem_Click);
            // 
            // fromWhatlinkshereToolStripMenuItem
            // 
            this.fromWhatlinkshereToolStripMenuItem.Name = "fromWhatlinkshereToolStripMenuItem";
            this.fromWhatlinkshereToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.fromWhatlinkshereToolStripMenuItem.Text = "From whatlinkshere";
            this.fromWhatlinkshereToolStripMenuItem.Click += new System.EventHandler(this.fromWhatlinkshereToolStripMenuItem_Click);
            // 
            // fromLinksOnPageToolStripMenuItem
            // 
            this.fromLinksOnPageToolStripMenuItem.Name = "fromLinksOnPageToolStripMenuItem";
            this.fromLinksOnPageToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.fromLinksOnPageToolStripMenuItem.Text = "From links on page";
            this.fromLinksOnPageToolStripMenuItem.Click += new System.EventHandler(this.fromLinksOnPageToolStripMenuItem_Click);
            // 
            // fromImageLinksToolStripMenuItem
            // 
            this.fromImageLinksToolStripMenuItem.Name = "fromImageLinksToolStripMenuItem";
            this.fromImageLinksToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.fromImageLinksToolStripMenuItem.Text = "From image links";
            this.fromImageLinksToolStripMenuItem.Click += new System.EventHandler(this.fromImageLinksToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(191, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.ToolTipText = "Remove the selected articles";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem1
            // 
            this.clearToolStripMenuItem1.Name = "clearToolStripMenuItem1";
            this.clearToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
            this.clearToolStripMenuItem1.Text = "Clear";
            this.clearToolStripMenuItem1.ToolTipText = "Clear the list";
            this.clearToolStripMenuItem1.Click += new System.EventHandler(this.clearToolStripMenuItem1_Click);
            // 
            // chkWLHRedirects
            // 
            this.chkWLHRedirects.AutoSize = true;
            this.chkWLHRedirects.Location = new System.Drawing.Point(6, 76);
            this.chkWLHRedirects.Name = "chkWLHRedirects";
            this.chkWLHRedirects.Size = new System.Drawing.Size(71, 17);
            this.chkWLHRedirects.TabIndex = 15;
            this.chkWLHRedirects.Text = "Red. only";
            this.toolTip1.SetToolTip(this.chkWLHRedirects, "Only find redirects");
            this.chkWLHRedirects.UseVisualStyleBackColor = true;
            this.chkWLHRedirects.Visible = false;
            // 
            // txtSelectSource
            // 
            this.txtSelectSource.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtSelectSource.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtSelectSource.Location = new System.Drawing.Point(80, 48);
            this.txtSelectSource.Name = "txtSelectSource";
            this.txtSelectSource.Size = new System.Drawing.Size(127, 20);
            this.txtSelectSource.TabIndex = 0;
            this.toolTip1.SetToolTip(this.txtSelectSource, "Select the source of the list");
            this.txtSelectSource.DoubleClick += new System.EventHandler(this.txtSelectSource_DoubleClick);
            this.txtSelectSource.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSelectSource_KeyPress);
            // 
            // lblNumberOfArticles
            // 
            this.lblNumberOfArticles.Location = new System.Drawing.Point(145, 76);
            this.lblNumberOfArticles.Name = "lblNumberOfArticles";
            this.lblNumberOfArticles.Size = new System.Drawing.Size(59, 17);
            this.lblNumberOfArticles.TabIndex = 14;
            this.lblNumberOfArticles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 211);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(58, 23);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "Add";
            this.toolTip1.SetToolTip(this.btnAdd, "Add the new article");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemoveArticle
            // 
            this.btnRemoveArticle.Location = new System.Drawing.Point(6, 240);
            this.btnRemoveArticle.Name = "btnRemoveArticle";
            this.btnRemoveArticle.Size = new System.Drawing.Size(58, 23);
            this.btnRemoveArticle.TabIndex = 10;
            this.btnRemoveArticle.Text = "Remove";
            this.toolTip1.SetToolTip(this.btnRemoveArticle, "Remove the selected article  (Shortcut delete)");
            this.btnRemoveArticle.UseVisualStyleBackColor = true;
            this.btnRemoveArticle.Click += new System.EventHandler(this.btnRemoveArticle_Click);
            // 
            // btnArticlesListClear
            // 
            this.btnArticlesListClear.Location = new System.Drawing.Point(6, 269);
            this.btnArticlesListClear.Name = "btnArticlesListClear";
            this.btnArticlesListClear.Size = new System.Drawing.Size(58, 23);
            this.btnArticlesListClear.TabIndex = 12;
            this.btnArticlesListClear.Text = "Clear";
            this.toolTip1.SetToolTip(this.btnArticlesListClear, "Clear out the list");
            this.btnArticlesListClear.UseVisualStyleBackColor = true;
            this.btnArticlesListClear.Click += new System.EventHandler(this.btnArticlesListClear_Click);
            // 
            // btnArticlesListSave
            // 
            this.btnArticlesListSave.Location = new System.Drawing.Point(6, 327);
            this.btnArticlesListSave.Name = "btnArticlesListSave";
            this.btnArticlesListSave.Size = new System.Drawing.Size(58, 23);
            this.btnArticlesListSave.TabIndex = 16;
            this.btnArticlesListSave.Text = "Save...";
            this.toolTip1.SetToolTip(this.btnArticlesListSave, "Save the list to a file");
            this.btnArticlesListSave.UseVisualStyleBackColor = true;
            this.btnArticlesListSave.Click += new System.EventHandler(this.btnArticlesListSave_Click);
            // 
            // txtNewArticle
            // 
            this.txtNewArticle.Location = new System.Drawing.Point(6, 181);
            this.txtNewArticle.Name = "txtNewArticle";
            this.txtNewArticle.Size = new System.Drawing.Size(58, 20);
            this.txtNewArticle.TabIndex = 8;
            this.toolTip1.SetToolTip(this.txtNewArticle, "Enter the name of a new article");
            this.txtNewArticle.DoubleClick += new System.EventHandler(this.txtNewArticle_DoubleClick);
            this.txtNewArticle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewArticle_KeyPress);
            // 
            // lblSourceSelect
            // 
            this.lblSourceSelect.Location = new System.Drawing.Point(6, 47);
            this.lblSourceSelect.Name = "lblSourceSelect";
            this.lblSourceSelect.Size = new System.Drawing.Size(73, 21);
            this.lblSourceSelect.TabIndex = 5;
            this.lblSourceSelect.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMakeList
            // 
            this.btnMakeList.Enabled = false;
            this.btnMakeList.Location = new System.Drawing.Point(80, 72);
            this.btnMakeList.Name = "btnMakeList";
            this.btnMakeList.Size = new System.Drawing.Size(62, 23);
            this.btnMakeList.TabIndex = 3;
            this.btnMakeList.Tag = "Get all pages needing editting";
            this.btnMakeList.Text = "Make list";
            this.toolTip1.SetToolTip(this.btnMakeList, "Make the List!");
            this.btnMakeList.UseVisualStyleBackColor = true;
            this.btnMakeList.Click += new System.EventHandler(this.btnMakeList_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Make from";
            // 
            // lbArticles
            // 
            this.lbArticles.ContextMenuStrip = this.mnuListBox;
            this.lbArticles.FormattingEnabled = true;
            this.lbArticles.Location = new System.Drawing.Point(70, 99);
            this.lbArticles.Name = "lbArticles";
            this.lbArticles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbArticles.Size = new System.Drawing.Size(137, 264);
            this.lbArticles.TabIndex = 4;
            this.lbArticles.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbArticles_MouseMove);
            this.lbArticles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbArticles_KeyDown);
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.Items.AddRange(new object[] {
            "Category",
            "What links here",
            "What transcludes here",
            "Links on page",
            "Text file",
            "Google Wikipedia",
            "User contribs",
            "Special page",
            "Image file links",
            "Database dump"});
            this.cmboSourceSelect.Location = new System.Drawing.Point(80, 20);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(127, 21);
            this.cmboSourceSelect.TabIndex = 1;
            this.cmboSourceSelect.Tag = "Source of pages to edit";
            this.toolTip1.SetToolTip(this.cmboSourceSelect, "Select the way you want to make the list");
            this.cmboSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSourceSelect_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.txtEdit);
            this.panel1.Location = new System.Drawing.Point(497, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(283, 366);
            this.panel1.TabIndex = 9;
            // 
            // txtEdit
            // 
            this.txtEdit.AcceptsReturn = true;
            this.txtEdit.AcceptsTab = true;
            this.txtEdit.ContextMenuStrip = this.mnuTextBox;
            this.txtEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEdit.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEdit.Location = new System.Drawing.Point(0, 0);
            this.txtEdit.MaxLength = 0;
            this.txtEdit.Multiline = true;
            this.txtEdit.Name = "txtEdit";
            this.txtEdit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEdit.Size = new System.Drawing.Size(283, 366);
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
            this.removeAllExcessWhitespaceToolStripMenuItem,
            this.reparseToolStripMenuItem,
            this.toolStripSeparator4,
            this.openPageInBrowserToolStripMenuItem,
            this.toolStripSeparator9,
            this.replaceTextWithLastEditToolStripMenuItem});
            this.mnuTextBox.Name = "contextMenuStrip1";
            this.mnuTextBox.Size = new System.Drawing.Size(220, 386);
            this.mnuTextBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // wordWrapToolStripMenuItem1
            // 
            this.wordWrapToolStripMenuItem1.Checked = true;
            this.wordWrapToolStripMenuItem1.CheckOnClick = true;
            this.wordWrapToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wordWrapToolStripMenuItem1.Name = "wordWrapToolStripMenuItem1";
            this.wordWrapToolStripMenuItem1.Size = new System.Drawing.Size(219, 22);
            this.wordWrapToolStripMenuItem1.Text = "Word wrap";
            this.wordWrapToolStripMenuItem1.Click += new System.EventHandler(this.wordWrapToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(216, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
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
            this.pasteMoreToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
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
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(216, 6);
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2});
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.goToLineToolStripMenuItem.Text = "Go to line";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.Size = new System.Drawing.Size(100, 21);
            this.toolStripTextBox2.Text = "Enter line number";
            this.toolStripTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.toolStripTextBox2_KeyPress);
            this.toolStripTextBox2.Click += new System.EventHandler(this.toolStripTextBox2_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(216, 6);
            // 
            // insertToolStripMenuItem
            // 
            this.insertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.birthdeathCatsToolStripMenuItem,
            this.metadataTemplateToolStripMenuItem});
            this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            this.insertToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.insertToolStripMenuItem.Text = "Insert...";
            // 
            // birthdeathCatsToolStripMenuItem
            // 
            this.birthdeathCatsToolStripMenuItem.Name = "birthdeathCatsToolStripMenuItem";
            this.birthdeathCatsToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.birthdeathCatsToolStripMenuItem.Text = "Guess birth/death cats";
            this.birthdeathCatsToolStripMenuItem.Click += new System.EventHandler(this.birthdeathCatsToolStripMenuItem_Click);
            // 
            // metadataTemplateToolStripMenuItem
            // 
            this.metadataTemplateToolStripMenuItem.Name = "metadataTemplateToolStripMenuItem";
            this.metadataTemplateToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.metadataTemplateToolStripMenuItem.Text = "Meta-data template";
            this.metadataTemplateToolStripMenuItem.Click += new System.EventHandler(this.metadataTemplateToolStripMenuItem_Click);
            // 
            // insertTagToolStripMenuItem
            // 
            this.insertTagToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wikifyToolStripMenuItem,
            this.cleanupToolStripMenuItem,
            this.expandToolStripMenuItem,
            this.speedyDeleteToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.uncategorisedToolStripMenuItem,
            this.stubToolStripMenuItem,
            this.toolStripTextBox1});
            this.insertTagToolStripMenuItem.Name = "insertTagToolStripMenuItem";
            this.insertTagToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.insertTagToolStripMenuItem.Text = "Insert tag";
            // 
            // wikifyToolStripMenuItem
            // 
            this.wikifyToolStripMenuItem.Name = "wikifyToolStripMenuItem";
            this.wikifyToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.wikifyToolStripMenuItem.Text = "Wikify";
            this.wikifyToolStripMenuItem.Click += new System.EventHandler(this.wikifyToolStripMenuItem_Click);
            // 
            // cleanupToolStripMenuItem
            // 
            this.cleanupToolStripMenuItem.Name = "cleanupToolStripMenuItem";
            this.cleanupToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.cleanupToolStripMenuItem.Text = "Cleanup";
            this.cleanupToolStripMenuItem.Click += new System.EventHandler(this.cleanupToolStripMenuItem_Click);
            // 
            // expandToolStripMenuItem
            // 
            this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
            this.expandToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.expandToolStripMenuItem.Text = "Expand";
            this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
            // 
            // speedyDeleteToolStripMenuItem
            // 
            this.speedyDeleteToolStripMenuItem.Name = "speedyDeleteToolStripMenuItem";
            this.speedyDeleteToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.speedyDeleteToolStripMenuItem.Text = "Speedy delete";
            this.speedyDeleteToolStripMenuItem.Click += new System.EventHandler(this.speedyDeleteToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.clearToolStripMenuItem.Text = "{{clear}}";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // uncategorisedToolStripMenuItem
            // 
            this.uncategorisedToolStripMenuItem.Name = "uncategorisedToolStripMenuItem";
            this.uncategorisedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.uncategorisedToolStripMenuItem.Text = "Uncategorised";
            this.uncategorisedToolStripMenuItem.Click += new System.EventHandler(this.uncategorisedToolStripMenuItem_Click);
            // 
            // stubToolStripMenuItem
            // 
            this.stubToolStripMenuItem.Name = "stubToolStripMenuItem";
            this.stubToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
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
            this.convertListToToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.convertListToToolStripMenuItem.Text = "Convert list to";
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.listToolStripMenuItem.Text = "* List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem1
            // 
            this.listToolStripMenuItem1.Name = "listToolStripMenuItem1";
            this.listToolStripMenuItem1.Size = new System.Drawing.Size(101, 22);
            this.listToolStripMenuItem1.Text = "# List";
            this.listToolStripMenuItem1.Click += new System.EventHandler(this.listToolStripMenuItem1_Click);
            // 
            // unicodifyToolStripMenuItem
            // 
            this.unicodifyToolStripMenuItem.Name = "unicodifyToolStripMenuItem";
            this.unicodifyToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.unicodifyToolStripMenuItem.Text = "Unicodify selected";
            this.unicodifyToolStripMenuItem.Click += new System.EventHandler(this.unicodifyToolStripMenuItem_Click);
            // 
            // removeAllExcessWhitespaceToolStripMenuItem
            // 
            this.removeAllExcessWhitespaceToolStripMenuItem.Name = "removeAllExcessWhitespaceToolStripMenuItem";
            this.removeAllExcessWhitespaceToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.removeAllExcessWhitespaceToolStripMenuItem.Text = "Remove all excess whitespace";
            this.removeAllExcessWhitespaceToolStripMenuItem.Click += new System.EventHandler(this.removeAllExcessWhitespaceToolStripMenuItem_Click);
            // 
            // reparseToolStripMenuItem
            // 
            this.reparseToolStripMenuItem.Name = "reparseToolStripMenuItem";
            this.reparseToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.reparseToolStripMenuItem.Text = "Re-parse";
            this.reparseToolStripMenuItem.Click += new System.EventHandler(this.reparseToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(216, 6);
            // 
            // openPageInBrowserToolStripMenuItem
            // 
            this.openPageInBrowserToolStripMenuItem.Name = "openPageInBrowserToolStripMenuItem";
            this.openPageInBrowserToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.openPageInBrowserToolStripMenuItem.Text = "Open page in browser";
            this.openPageInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openPageInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(216, 6);
            // 
            // replaceTextWithLastEditToolStripMenuItem
            // 
            this.replaceTextWithLastEditToolStripMenuItem.Name = "replaceTextWithLastEditToolStripMenuItem";
            this.replaceTextWithLastEditToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.replaceTextWithLastEditToolStripMenuItem.Text = "Replace text with last edit";
            this.replaceTextWithLastEditToolStripMenuItem.Click += new System.EventHandler(this.replaceTextWithLastEditToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generalToolStripMenuItem,
            this.toolsToolStripMenuItem,
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
            this.saveSettingsToolStripMenuItem,
            this.loadSettingsToolStripMenuItem,
            this.loadDefaultSettingsToolStripMenuItem,
            this.toolStripSeparator8,
            this.selectProjectToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.saveSettingsToolStripMenuItem.Text = "Save settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToolStripMenuItem_Click);
            // 
            // loadSettingsToolStripMenuItem
            // 
            this.loadSettingsToolStripMenuItem.Name = "loadSettingsToolStripMenuItem";
            this.loadSettingsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.loadSettingsToolStripMenuItem.Text = "Load settings";
            this.loadSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadSettingsToolStripMenuItem_Click);
            // 
            // loadDefaultSettingsToolStripMenuItem
            // 
            this.loadDefaultSettingsToolStripMenuItem.Name = "loadDefaultSettingsToolStripMenuItem";
            this.loadDefaultSettingsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.loadDefaultSettingsToolStripMenuItem.Text = "Reset settings";
            this.loadDefaultSettingsToolStripMenuItem.Click += new System.EventHandler(this.loadDefaultSettingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(205, 6);
            // 
            // selectProjectToolStripMenuItem
            // 
            this.selectProjectToolStripMenuItem.Name = "selectProjectToolStripMenuItem";
            this.selectProjectToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.selectProjectToolStripMenuItem.Text = "Select language and project";
            this.selectProjectToolStripMenuItem.Click += new System.EventHandler(this.selectProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(205, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterOutNonMainSpaceToolStripMenuItem,
            this.specialFilterToolStripMenuItem1,
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
            this.filterOutNonMainSpaceToolStripMenuItem.Name = "filterOutNonMainSpaceToolStripMenuItem";
            this.filterOutNonMainSpaceToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.filterOutNonMainSpaceToolStripMenuItem.Text = "Filter out non main space";
            this.filterOutNonMainSpaceToolStripMenuItem.Click += new System.EventHandler(this.filterOutNonMainSpaceToolStripMenuItem_Click);
            // 
            // specialFilterToolStripMenuItem1
            // 
            this.specialFilterToolStripMenuItem1.Name = "specialFilterToolStripMenuItem1";
            this.specialFilterToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
            this.specialFilterToolStripMenuItem1.Text = "Filter";
            this.specialFilterToolStripMenuItem1.Click += new System.EventHandler(this.specialFilterToolStripMenuItem1_Click);
            // 
            // sortAlphabeticallyToolStripMenuItem
            // 
            this.sortAlphabeticallyToolStripMenuItem.Name = "sortAlphabeticallyToolStripMenuItem";
            this.sortAlphabeticallyToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.sortAlphabeticallyToolStripMenuItem.Text = "Sort alphabetically";
            this.sortAlphabeticallyToolStripMenuItem.Click += new System.EventHandler(this.sortAlphabeticallyToolStripMenuItem_Click);
            // 
            // saveListToTextFileToolStripMenuItem
            // 
            this.saveListToTextFileToolStripMenuItem.Name = "saveListToTextFileToolStripMenuItem";
            this.saveListToTextFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.saveListToTextFileToolStripMenuItem.Text = "Save list to text file";
            this.saveListToTextFileToolStripMenuItem.Click += new System.EventHandler(this.saveListToTextFileToolStripMenuItem_Click_1);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(191, 6);
            // 
            // launchDumpSearcherToolStripMenuItem
            // 
            this.launchDumpSearcherToolStripMenuItem.Name = "launchDumpSearcherToolStripMenuItem";
            this.launchDumpSearcherToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.launchDumpSearcherToolStripMenuItem.Text = "Launch DumpSearcher";
            this.launchDumpSearcherToolStripMenuItem.Click += new System.EventHandler(this.launchDumpSearcherToolStripMenuItem_Click);
            // 
            // launchListComparerToolStripMenuItem
            // 
            this.launchListComparerToolStripMenuItem.Name = "launchListComparerToolStripMenuItem";
            this.launchListComparerToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.launchListComparerToolStripMenuItem.Text = "Launch ListComparer";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableTheToolbarToolStripMenuItem,
            this.bypassRedirectsToolStripMenuItem,
            this.ignoreNonexistentPagesToolStripMenuItem,
            this.doNotAutomaticallyDoAnythingToolStripMenuItem,
            this.previewInsteadOfDiffToolStripMenuItem,
            this.toolStripSeparator6,
            this.markAllAsMinorToolStripMenuItem,
            this.addAllToWatchlistToolStripMenuItem,
            this.showTimerToolStripMenuItem,
            this.toolStripSeparator7,
            this.alphaSortInterwikiLinksToolStripMenuItem,
            this.toolStripSeparator11,
            this.addIgnoredToLogFileToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.toolsToolStripMenuItem.Text = "General";
            // 
            // enableTheToolbarToolStripMenuItem
            // 
            this.enableTheToolbarToolStripMenuItem.CheckOnClick = true;
            this.enableTheToolbarToolStripMenuItem.Name = "enableTheToolbarToolStripMenuItem";
            this.enableTheToolbarToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.enableTheToolbarToolStripMenuItem.Text = "Enable the toolbar";
            this.enableTheToolbarToolStripMenuItem.Click += new System.EventHandler(this.enableTheToolbarToolStripMenuItem_Click);
            // 
            // bypassRedirectsToolStripMenuItem
            // 
            this.bypassRedirectsToolStripMenuItem.Checked = true;
            this.bypassRedirectsToolStripMenuItem.CheckOnClick = true;
            this.bypassRedirectsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bypassRedirectsToolStripMenuItem.Name = "bypassRedirectsToolStripMenuItem";
            this.bypassRedirectsToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.bypassRedirectsToolStripMenuItem.Text = "Bypass redirects";
            // 
            // ignoreNonexistentPagesToolStripMenuItem
            // 
            this.ignoreNonexistentPagesToolStripMenuItem.Checked = true;
            this.ignoreNonexistentPagesToolStripMenuItem.CheckOnClick = true;
            this.ignoreNonexistentPagesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ignoreNonexistentPagesToolStripMenuItem.Name = "ignoreNonexistentPagesToolStripMenuItem";
            this.ignoreNonexistentPagesToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.ignoreNonexistentPagesToolStripMenuItem.Text = "Ignore non-existent pages";
            // 
            // doNotAutomaticallyDoAnythingToolStripMenuItem
            // 
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.CheckOnClick = true;
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Name = "doNotAutomaticallyDoAnythingToolStripMenuItem";
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.doNotAutomaticallyDoAnythingToolStripMenuItem.Text = "Do not automatically apply changes";
            // 
            // previewInsteadOfDiffToolStripMenuItem
            // 
            this.previewInsteadOfDiffToolStripMenuItem.CheckOnClick = true;
            this.previewInsteadOfDiffToolStripMenuItem.Name = "previewInsteadOfDiffToolStripMenuItem";
            this.previewInsteadOfDiffToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.previewInsteadOfDiffToolStripMenuItem.Text = "Preview instead of diff";
            this.previewInsteadOfDiffToolStripMenuItem.Click += new System.EventHandler(this.previewInsteadOfDiffToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(241, 6);
            // 
            // markAllAsMinorToolStripMenuItem
            // 
            this.markAllAsMinorToolStripMenuItem.CheckOnClick = true;
            this.markAllAsMinorToolStripMenuItem.Name = "markAllAsMinorToolStripMenuItem";
            this.markAllAsMinorToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.markAllAsMinorToolStripMenuItem.Text = "Mark all as minor";
            // 
            // addAllToWatchlistToolStripMenuItem
            // 
            this.addAllToWatchlistToolStripMenuItem.CheckOnClick = true;
            this.addAllToWatchlistToolStripMenuItem.Name = "addAllToWatchlistToolStripMenuItem";
            this.addAllToWatchlistToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.addAllToWatchlistToolStripMenuItem.Text = "Add all to watchlist";
            // 
            // showTimerToolStripMenuItem
            // 
            this.showTimerToolStripMenuItem.CheckOnClick = true;
            this.showTimerToolStripMenuItem.Name = "showTimerToolStripMenuItem";
            this.showTimerToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.showTimerToolStripMenuItem.Text = "Show timer";
            this.showTimerToolStripMenuItem.Click += new System.EventHandler(this.showTimerToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(241, 6);
            // 
            // alphaSortInterwikiLinksToolStripMenuItem
            // 
            this.alphaSortInterwikiLinksToolStripMenuItem.Checked = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckOnClick = true;
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alphaSortInterwikiLinksToolStripMenuItem.Name = "alphaSortInterwikiLinksToolStripMenuItem";
            this.alphaSortInterwikiLinksToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.alphaSortInterwikiLinksToolStripMenuItem.Text = "Sort interwiki link order";
            this.alphaSortInterwikiLinksToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.alphaSortInterwikiLinksToolStripMenuItem_CheckStateChanged);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(241, 6);
            // 
            // addIgnoredToLogFileToolStripMenuItem
            // 
            this.addIgnoredToLogFileToolStripMenuItem.CheckOnClick = true;
            this.addIgnoredToLogFileToolStripMenuItem.Name = "addIgnoredToLogFileToolStripMenuItem";
            this.addIgnoredToLogFileToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.addIgnoredToLogFileToolStripMenuItem.Text = "Enable button to log false positives";
            this.addIgnoredToLogFileToolStripMenuItem.Click += new System.EventHandler(this.addIgnoredToLogFileToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchIRCMonitorToolStripMenuItem,
            this.helpToolStripMenuItem1,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // launchIRCMonitorToolStripMenuItem
            // 
            this.launchIRCMonitorToolStripMenuItem.Name = "launchIRCMonitorToolStripMenuItem";
            this.launchIRCMonitorToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.launchIRCMonitorToolStripMenuItem.Text = "Launch IRC Monitor";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.lblStatusText,
            this.lblBotTimer});
            this.statusStrip1.Location = new System.Drawing.Point(0, 599);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(557, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = false;
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(300, 17);
            this.lblStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBotTimer
            // 
            this.lblBotTimer.AutoSize = false;
            this.lblBotTimer.Margin = new System.Windows.Forms.Padding(2, 3, 0, 2);
            this.lblBotTimer.Name = "lblBotTimer";
            this.lblBotTimer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBotTimer.Size = new System.Drawing.Size(85, 17);
            this.lblBotTimer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file|*.txt";
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
            // statusStrip2
            // 
            this.statusStrip2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip2.AutoSize = false;
            this.statusStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblProject,
            this.lblUserName});
            this.statusStrip2.Location = new System.Drawing.Point(555, 599);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(228, 22);
            this.statusStrip2.TabIndex = 667;
            // 
            // lblProject
            // 
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(52, 17);
            this.lblProject.Text = "Wikipedia";
            // 
            // lblUserName
            // 
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(0, 17);
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
            this.panel2.Size = new System.Drawing.Size(783, 371);
            this.panel2.TabIndex = 668;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btntsShowHide,
            this.toolStripSeparator12,
            this.btntsSave,
            this.btntsIgnore,
            this.btntsStop,
            this.toolStripSeparator15,
            this.btntsPreview,
            this.btntsChanges});
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
            // btntsStop
            // 
            this.btntsStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btntsStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btntsStop.Name = "btntsStop";
            this.btntsStop.Size = new System.Drawing.Size(23, 22);
            this.btntsStop.Text = "Stop";
            this.btntsStop.Click += new System.EventHandler(this.btntsStop_Click);
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
            // webBrowserEdit
            // 
            this.webBrowserEdit.AllowNavigation = false;
            this.webBrowserEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserEdit.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserEdit.Location = new System.Drawing.Point(0, 27);
            this.webBrowserEdit.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserEdit.Name = "webBrowserEdit";
            this.webBrowserEdit.ScriptErrorsSuppressed = true;
            this.webBrowserEdit.Size = new System.Drawing.Size(780, 193);
            this.webBrowserEdit.TabIndex = 670;
            this.webBrowserEdit.TabStop = false;
            this.webBrowserEdit.WebBrowserShortcutsEnabled = false;
            this.webBrowserEdit.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserEdit_Navigating);
            this.webBrowserEdit.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserEdit_DocumentCompleted);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(6, 298);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(58, 23);
            this.btnFilter.TabIndex = 17;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 621);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.webBrowserEdit);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ImageGroupBox.ResumeLayout(false);
            this.ImageGroupBox.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBotSpeed)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.mnuListBox.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.mnuTextBox.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnArticlesListClear;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtNewArticle;
        private System.Windows.Forms.Button btnRemoveArticle;
        private System.Windows.Forms.Label lblSourceSelect;
        private System.Windows.Forms.Button btnMakeList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboSourceSelect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.Label lblNumberOfArticles;
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
        private System.Windows.Forms.ComboBox cmboEditSummary;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkFindandReplace;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtIgnoreIfContains;
        private System.Windows.Forms.CheckBox chkIgnoreIfContains;
        private System.Windows.Forms.CheckBox chkGeneralParse;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtNewCategory;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox chkAutoMode;
        private System.Windows.Forms.NumericUpDown nudBotSpeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkAppend;
        private System.Windows.Forms.TextBox txtAppendMessage;
        private System.Windows.Forms.TextBox txtOnlyIfContains;
        private System.Windows.Forms.CheckBox chkOnlyIfContains;
        private System.Windows.Forms.CheckBox chkAutoTagger;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkIgnoreIsRegex;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ContextMenuStrip mnuListBox;
        private System.Windows.Forms.ToolStripMenuItem filterOutNonMainSpaceArticlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortAlphebeticallyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListToTextFileToolStripMenuItem1;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnDiff;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bypassRedirectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewInsteadOfDiffToolStripMenuItem;
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
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label lblLinks;
        private System.Windows.Forms.Label lblImages;
        private System.Windows.Forms.Label lblCats;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.Label lblInterLinks;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblWarn;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.ToolStripMenuItem metadataTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncategorisedToolStripMenuItem;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblDuplicateWikilinks;
        private System.Windows.Forms.ListBox lbDuplicateWikilinks;
        private System.Windows.Forms.ToolStripMenuItem alphaSortInterwikiLinksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ComboBox cmboCategorise;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.ToolStripMenuItem showTimerToolStripMenuItem;
        private System.Windows.Forms.Button btnMoreFindAndReplce;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem unicodifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.SaveFileDialog saveXML;
        private System.Windows.Forms.OpenFileDialog openXML;
        private System.Windows.Forms.ToolStripStatusLabel lblBotTimer;
        private System.Windows.Forms.CheckBox chkIgnoreCaseSensitive;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtSelectSource;
        private System.Windows.Forms.CheckBox chkWLHRedirects;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem reparseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doNotAutomaticallyDoAnythingToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkFindCaseSensitive;
        private System.Windows.Forms.CheckBox chkFindRegex;
        private System.Windows.Forms.ToolStripMenuItem replaceTextWithLastEditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.CheckBox chkUnicodifyWhole;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ListBox lbArticles;
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
        private System.Windows.Forms.ToolStripMenuItem ignoreNonexistentPagesToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.TextBox txtEdit;
        private System.Windows.Forms.ToolStripMenuItem addIgnoredToLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.Button btnFalsePositive;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem specialFilterToolStripMenuItem1;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel lblUserName;
        private System.Windows.Forms.ToolStripStatusLabel lblProject;
        private System.Windows.Forms.ToolStripMenuItem convertListToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem1;
        private System.Windows.Forms.CheckBox chkQuickSave;
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
        private System.Windows.Forms.ToolStripMenuItem convertToTalkPagesToolStripMenuItem;
        private System.Windows.Forms.Button btnArticlesListSave;
        private System.Windows.Forms.RadioButton rdoPrepend;
        private System.Windows.Forms.RadioButton rdoAppend;
        private System.Windows.Forms.ToolStripMenuItem convertFromTalkPagesToolStripMenuItem;
        private System.Windows.Forms.GroupBox ImageGroupBox;
        private System.Windows.Forms.Label lblImageWith;
        private System.Windows.Forms.Label lblImageReplace;
        private System.Windows.Forms.TextBox txtImageWith;
        private System.Windows.Forms.TextBox txtImageReplace;
        private System.Windows.Forms.ComboBox cmboImages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem launchDumpSearcherToolStripMenuItem;
        private System.Windows.Forms.Button btnFindAndReplaceAdvanced;
        private System.Windows.Forms.ToolStripMenuItem launchIRCMonitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchListComparerToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkIgnoreWhenNoFAR;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton btntsPreview;
        private System.Windows.Forms.ToolStripButton btntsChanges;
        private System.Windows.Forms.CheckBox chkSuppressTag;
        private WikiFunctions.WebControl webBrowserEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.CheckBox chkSkipNoChanges;
        private System.Windows.Forms.ToolStripMenuItem wordWrapToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromCategoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromWhatlinkshereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromLinksOnPageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromImageLinksToolStripMenuItem;
        private System.Windows.Forms.Button btnFilter;


    }
}

