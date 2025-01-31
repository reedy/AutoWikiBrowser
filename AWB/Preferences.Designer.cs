//$Header: /cvsroot/autowikibrowser/src/Project\040select.Designer.cs,v 1.15 2006/06/15 10:14:49 wikibluemoose Exp $

namespace AutoWikiBrowser
{
    partial class MyPreferences
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
                if (TextBoxFont != null) TextBoxFont.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyPreferences));
            this.cmboLang = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmboProject = new System.Windows.Forms.ComboBox();
            this.lblLang = new System.Windows.Forms.Label();
            this.lblProject = new System.Windows.Forms.Label();
            this.lblNonEnNotice = new System.Windows.Forms.Label();
            this.btnTextBoxFont = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPostfix = new System.Windows.Forms.Label();
            this.cmboCustomProject = new System.Windows.Forms.ComboBox();
            this.chkAddUsingAWBToActionSummaries = new System.Windows.Forms.CheckBox();
            this.chkAlwaysConfirmExit = new System.Windows.Forms.CheckBox();
            this.chkSupressAWB = new System.Windows.Forms.CheckBox();
            this.chkSaveArticleList = new System.Windows.Forms.CheckBox();
            this.chkMinimize = new System.Windows.Forms.CheckBox();
            this.chkLowPriority = new System.Windows.Forms.CheckBox();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.chkFlash = new System.Windows.Forms.CheckBox();
            this.lblDoneDo = new System.Windows.Forms.Label();
            this.chkAutoSaveEdit = new System.Windows.Forms.CheckBox();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.AutoSaveEditBoxGroup = new System.Windows.Forms.GroupBox();
            this.btnSetFile = new System.Windows.Forms.Button();
            this.txtAutosave = new System.Windows.Forms.TextBox();
            this.lblAutosaveFile = new System.Windows.Forms.Label();
            this.AutoSaveEditCont = new System.Windows.Forms.Label();
            this.nudEditBoxAutosave = new System.Windows.Forms.NumericUpDown();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.chkPrivacy = new System.Windows.Forms.CheckBox();
            this.lblPrivacy = new System.Windows.Forms.Label();
            this.tbPrefs = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chkEnableLogging = new System.Windows.Forms.CheckBox();
            this.chkDiffInBotMode = new System.Windows.Forms.CheckBox();
            this.cmboOnLoad = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabSite = new System.Windows.Forms.TabPage();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.chkDomain = new System.Windows.Forms.CheckBox();
            this.cmboProtocol = new System.Windows.Forms.ComboBox();
            this.chkEmptyOnProjectChange = new System.Windows.Forms.CheckBox();
            this.chkIgnoreNoBots = new System.Windows.Forms.CheckBox();
            this.tabEditing = new System.Windows.Forms.TabPage();
            this.chkShowTimer = new System.Windows.Forms.CheckBox();
            this.tabTools = new System.Windows.Forms.TabPage();
            this.cmboDBScanner = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmboListSplitter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmboListComparer = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPrivacy = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.alertListBox = new System.Windows.Forms.CheckedListBox();
            this.ToolTip = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.lblSaveAsDefaultFile = new System.Windows.Forms.Label();
            this.AutoSaveEditBoxGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEditBoxAutosave)).BeginInit();
            this.tbPrefs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabSite.SuspendLayout();
            this.tabEditing.SuspendLayout();
            this.tabTools.SuspendLayout();
            this.tabPrivacy.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownHeight = 212;
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            resources.ApplyResources(this.cmboLang, "cmboLang");
            this.cmboLang.Name = "cmboLang";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmboProject
            // 
            this.cmboProject.DropDownHeight = 206;
            this.cmboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProject.FormattingEnabled = true;
            resources.ApplyResources(this.cmboProject, "cmboProject");
            this.cmboProject.Name = "cmboProject";
            this.cmboProject.SelectedIndexChanged += new System.EventHandler(this.cmboProject_SelectedIndexChanged);
            // 
            // lblLang
            // 
            resources.ApplyResources(this.lblLang, "lblLang");
            this.lblLang.Name = "lblLang";
            // 
            // lblProject
            // 
            resources.ApplyResources(this.lblProject, "lblProject");
            this.lblProject.Name = "lblProject";
            // 
            // lblNonEnNotice
            // 
            resources.ApplyResources(this.lblNonEnNotice, "lblNonEnNotice");
            this.lblNonEnNotice.Name = "lblNonEnNotice";
            // 
            // btnTextBoxFont
            // 
            resources.ApplyResources(this.btnTextBoxFont, "btnTextBoxFont");
            this.btnTextBoxFont.Name = "btnTextBoxFont";
            this.btnTextBoxFont.UseVisualStyleBackColor = true;
            this.btnTextBoxFont.Click += new System.EventHandler(this.btnTextBoxFont_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            // 
            // lblPostfix
            // 
            resources.ApplyResources(this.lblPostfix, "lblPostfix");
            this.lblPostfix.Name = "lblPostfix";
            // 
            // cmboCustomProject
            // 
            this.cmboCustomProject.FormattingEnabled = true;
            resources.ApplyResources(this.cmboCustomProject, "cmboCustomProject");
            this.cmboCustomProject.Name = "cmboCustomProject";
            this.cmboCustomProject.SelectedIndexChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            this.cmboCustomProject.TextChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            this.cmboCustomProject.Leave += new System.EventHandler(this.txtCustomProject_Leave);
            // 
            // chkAddUsingAWBToActionSummaries
            // 
            resources.ApplyResources(this.chkAddUsingAWBToActionSummaries, "chkAddUsingAWBToActionSummaries");
            this.chkAddUsingAWBToActionSummaries.Name = "chkAddUsingAWBToActionSummaries";
            this.chkAddUsingAWBToActionSummaries.UseVisualStyleBackColor = true;
            // 
            // chkAlwaysConfirmExit
            // 
            resources.ApplyResources(this.chkAlwaysConfirmExit, "chkAlwaysConfirmExit");
            this.chkAlwaysConfirmExit.Checked = true;
            this.chkAlwaysConfirmExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlwaysConfirmExit.Name = "chkAlwaysConfirmExit";
            this.chkAlwaysConfirmExit.UseVisualStyleBackColor = true;
            // 
            // chkSupressAWB
            // 
            resources.ApplyResources(this.chkSupressAWB, "chkSupressAWB");
            this.chkSupressAWB.Name = "chkSupressAWB";
            this.chkSupressAWB.UseVisualStyleBackColor = true;
            // 
            // chkSaveArticleList
            // 
            resources.ApplyResources(this.chkSaveArticleList, "chkSaveArticleList");
            this.chkSaveArticleList.Checked = true;
            this.chkSaveArticleList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveArticleList.Name = "chkSaveArticleList";
            this.ToolTip.SetToolTip(this.chkSaveArticleList, resources.GetString("chkSaveArticleList.ToolTip"));
            this.chkSaveArticleList.UseVisualStyleBackColor = true;
            // 
            // chkMinimize
            // 
            resources.ApplyResources(this.chkMinimize, "chkMinimize");
            this.chkMinimize.Name = "chkMinimize";
            this.chkMinimize.UseVisualStyleBackColor = true;
            // 
            // chkLowPriority
            // 
            resources.ApplyResources(this.chkLowPriority, "chkLowPriority");
            this.chkLowPriority.Name = "chkLowPriority";
            this.ToolTip.SetToolTip(this.chkLowPriority, resources.GetString("chkLowPriority.ToolTip"));
            this.chkLowPriority.UseVisualStyleBackColor = true;
            // 
            // chkBeep
            // 
            resources.ApplyResources(this.chkBeep, "chkBeep");
            this.chkBeep.Checked = true;
            this.chkBeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.UseVisualStyleBackColor = true;
            // 
            // chkFlash
            // 
            resources.ApplyResources(this.chkFlash, "chkFlash");
            this.chkFlash.Checked = true;
            this.chkFlash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFlash.Name = "chkFlash";
            this.chkFlash.UseVisualStyleBackColor = true;
            // 
            // lblDoneDo
            // 
            resources.ApplyResources(this.lblDoneDo, "lblDoneDo");
            this.lblDoneDo.Name = "lblDoneDo";
            // 
            // chkAutoSaveEdit
            // 
            resources.ApplyResources(this.chkAutoSaveEdit, "chkAutoSaveEdit");
            this.chkAutoSaveEdit.Name = "chkAutoSaveEdit";
            this.chkAutoSaveEdit.UseVisualStyleBackColor = true;
            this.chkAutoSaveEdit.CheckedChanged += new System.EventHandler(this.chkAutoSaveEdit_CheckedChanged);
            // 
            // AutoSaveEditBoxGroup
            // 
            this.AutoSaveEditBoxGroup.Controls.Add(this.btnSetFile);
            this.AutoSaveEditBoxGroup.Controls.Add(this.txtAutosave);
            this.AutoSaveEditBoxGroup.Controls.Add(this.lblAutosaveFile);
            this.AutoSaveEditBoxGroup.Controls.Add(this.AutoSaveEditCont);
            this.AutoSaveEditBoxGroup.Controls.Add(this.nudEditBoxAutosave);
            this.AutoSaveEditBoxGroup.Controls.Add(this.chkAutoSaveEdit);
            resources.ApplyResources(this.AutoSaveEditBoxGroup, "AutoSaveEditBoxGroup");
            this.AutoSaveEditBoxGroup.Name = "AutoSaveEditBoxGroup";
            this.AutoSaveEditBoxGroup.TabStop = false;
            // 
            // btnSetFile
            // 
            resources.ApplyResources(this.btnSetFile, "btnSetFile");
            this.btnSetFile.Name = "btnSetFile";
            this.btnSetFile.UseVisualStyleBackColor = true;
            this.btnSetFile.Click += new System.EventHandler(this.btnSetFile_Click);
            // 
            // txtAutosave
            // 
            resources.ApplyResources(this.txtAutosave, "txtAutosave");
            this.txtAutosave.Name = "txtAutosave";
            this.txtAutosave.ReadOnly = true;
            // 
            // lblAutosaveFile
            // 
            resources.ApplyResources(this.lblAutosaveFile, "lblAutosaveFile");
            this.lblAutosaveFile.Name = "lblAutosaveFile";
            // 
            // AutoSaveEditCont
            // 
            resources.ApplyResources(this.AutoSaveEditCont, "AutoSaveEditCont");
            this.AutoSaveEditCont.Name = "AutoSaveEditCont";
            // 
            // nudEditBoxAutosave
            // 
            resources.ApplyResources(this.nudEditBoxAutosave, "nudEditBoxAutosave");
            this.nudEditBoxAutosave.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudEditBoxAutosave.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudEditBoxAutosave.Name = "nudEditBoxAutosave";
            this.nudEditBoxAutosave.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // saveFile
            // 
            resources.ApplyResources(this.saveFile, "saveFile");
            // 
            // chkPrivacy
            // 
            resources.ApplyResources(this.chkPrivacy, "chkPrivacy");
            this.chkPrivacy.Checked = true;
            this.chkPrivacy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrivacy.Name = "chkPrivacy";
            // 
            // lblPrivacy
            // 
            resources.ApplyResources(this.lblPrivacy, "lblPrivacy");
            this.lblPrivacy.Name = "lblPrivacy";
            // 
            // tbPrefs
            // 
            resources.ApplyResources(this.tbPrefs, "tbPrefs");
            this.tbPrefs.Controls.Add(this.tabGeneral);
            this.tbPrefs.Controls.Add(this.tabSite);
            this.tbPrefs.Controls.Add(this.tabEditing);
            this.tbPrefs.Controls.Add(this.tabTools);
            this.tbPrefs.Controls.Add(this.tabPrivacy);
            this.tbPrefs.Controls.Add(this.tabPage1);
            this.tbPrefs.Name = "tbPrefs";
            this.tbPrefs.SelectedIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chkEnableLogging);
            this.tabGeneral.Controls.Add(this.chkDiffInBotMode);
            this.tabGeneral.Controls.Add(this.cmboOnLoad);
            this.tabGeneral.Controls.Add(this.label4);
            this.tabGeneral.Controls.Add(this.chkMinimize);
            this.tabGeneral.Controls.Add(this.chkLowPriority);
            this.tabGeneral.Controls.Add(this.chkSaveArticleList);
            this.tabGeneral.Controls.Add(this.chkAlwaysConfirmExit);
            resources.ApplyResources(this.tabGeneral, "tabGeneral");
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // chkEnableLogging
            // 
            resources.ApplyResources(this.chkEnableLogging, "chkEnableLogging");
            this.chkEnableLogging.Checked = true;
            this.chkEnableLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableLogging.Name = "chkEnableLogging";
            this.ToolTip.SetToolTip(this.chkEnableLogging, resources.GetString("chkEnableLogging.ToolTip"));
            this.chkEnableLogging.UseVisualStyleBackColor = true;
            // 
            // chkDiffInBotMode
            // 
            resources.ApplyResources(this.chkDiffInBotMode, "chkDiffInBotMode");
            this.chkDiffInBotMode.Name = "chkDiffInBotMode";
            this.chkDiffInBotMode.UseVisualStyleBackColor = true;
            // 
            // cmboOnLoad
            // 
            this.cmboOnLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboOnLoad.FormattingEnabled = true;
            this.cmboOnLoad.Items.AddRange(new object[] {
            resources.GetString("cmboOnLoad.Items"),
            resources.GetString("cmboOnLoad.Items1")});
            resources.ApplyResources(this.cmboOnLoad, "cmboOnLoad");
            this.cmboOnLoad.Name = "cmboOnLoad";
            this.cmboOnLoad.SelectedIndexChanged += new System.EventHandler(this.cmboOnLoad_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tabSite
            // 
            this.tabSite.Controls.Add(this.txtDomain);
            this.tabSite.Controls.Add(this.chkDomain);
            this.tabSite.Controls.Add(this.cmboProtocol);
            this.tabSite.Controls.Add(this.chkEmptyOnProjectChange);
            this.tabSite.Controls.Add(this.chkSupressAWB);
            this.tabSite.Controls.Add(this.chkIgnoreNoBots);
            this.tabSite.Controls.Add(this.lblPostfix);
            this.tabSite.Controls.Add(this.cmboLang);
            this.tabSite.Controls.Add(this.lblLang);
            this.tabSite.Controls.Add(this.cmboProject);
            this.tabSite.Controls.Add(this.lblProject);
            this.tabSite.Controls.Add(this.cmboCustomProject);
            this.tabSite.Controls.Add(this.lblNonEnNotice);
            resources.ApplyResources(this.tabSite, "tabSite");
            this.tabSite.Name = "tabSite";
            this.tabSite.UseVisualStyleBackColor = true;
            // 
            // txtDomain
            // 
            resources.ApplyResources(this.txtDomain, "txtDomain");
            this.txtDomain.Name = "txtDomain";
            // 
            // chkDomain
            // 
            resources.ApplyResources(this.chkDomain, "chkDomain");
            this.chkDomain.Name = "chkDomain";
            this.chkDomain.UseVisualStyleBackColor = true;
            this.chkDomain.CheckedChanged += new System.EventHandler(this.chkDomain_CheckedChanged);
            // 
            // cmboProtocol
            // 
            this.cmboProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProtocol.FormattingEnabled = true;
            this.cmboProtocol.Items.AddRange(new object[] {
            resources.GetString("cmboProtocol.Items"),
            resources.GetString("cmboProtocol.Items1")});
            resources.ApplyResources(this.cmboProtocol, "cmboProtocol");
            this.cmboProtocol.Name = "cmboProtocol";
            // 
            // chkEmptyOnProjectChange
            // 
            resources.ApplyResources(this.chkEmptyOnProjectChange, "chkEmptyOnProjectChange");
            this.chkEmptyOnProjectChange.Name = "chkEmptyOnProjectChange";
            this.ToolTip.SetToolTip(this.chkEmptyOnProjectChange, resources.GetString("chkEmptyOnProjectChange.ToolTip"));
            this.chkEmptyOnProjectChange.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreNoBots
            // 
            resources.ApplyResources(this.chkIgnoreNoBots, "chkIgnoreNoBots");
            this.chkIgnoreNoBots.Name = "chkIgnoreNoBots";
            this.ToolTip.SetToolTip(this.chkIgnoreNoBots, resources.GetString("chkIgnoreNoBots.ToolTip"));
            this.chkIgnoreNoBots.UseVisualStyleBackColor = true;
            // 
            // tabEditing
            // 
            this.tabEditing.Controls.Add(this.AutoSaveEditBoxGroup);
            this.tabEditing.Controls.Add(this.chkShowTimer);
            this.tabEditing.Controls.Add(this.chkAddUsingAWBToActionSummaries);
            this.tabEditing.Controls.Add(this.lblDoneDo);
            this.tabEditing.Controls.Add(this.chkFlash);
            this.tabEditing.Controls.Add(this.chkBeep);
            this.tabEditing.Controls.Add(this.btnTextBoxFont);
            resources.ApplyResources(this.tabEditing, "tabEditing");
            this.tabEditing.Name = "tabEditing";
            this.tabEditing.UseVisualStyleBackColor = true;
            // 
            // chkShowTimer
            // 
            resources.ApplyResources(this.chkShowTimer, "chkShowTimer");
            this.chkShowTimer.Name = "chkShowTimer";
            this.chkShowTimer.UseVisualStyleBackColor = true;
            // 
            // tabTools
            // 
            this.tabTools.Controls.Add(this.cmboDBScanner);
            this.tabTools.Controls.Add(this.label3);
            this.tabTools.Controls.Add(this.cmboListSplitter);
            this.tabTools.Controls.Add(this.label2);
            this.tabTools.Controls.Add(this.cmboListComparer);
            this.tabTools.Controls.Add(this.label1);
            resources.ApplyResources(this.tabTools, "tabTools");
            this.tabTools.Name = "tabTools";
            this.tabTools.UseVisualStyleBackColor = true;
            // 
            // cmboDBScanner
            // 
            this.cmboDBScanner.FormattingEnabled = true;
            this.cmboDBScanner.Items.AddRange(new object[] {
            resources.GetString("cmboDBScanner.Items"),
            resources.GetString("cmboDBScanner.Items1"),
            resources.GetString("cmboDBScanner.Items2")});
            resources.ApplyResources(this.cmboDBScanner, "cmboDBScanner");
            this.cmboDBScanner.Name = "cmboDBScanner";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmboListSplitter
            // 
            this.cmboListSplitter.FormattingEnabled = true;
            this.cmboListSplitter.Items.AddRange(new object[] {
            resources.GetString("cmboListSplitter.Items"),
            resources.GetString("cmboListSplitter.Items1"),
            resources.GetString("cmboListSplitter.Items2")});
            resources.ApplyResources(this.cmboListSplitter, "cmboListSplitter");
            this.cmboListSplitter.Name = "cmboListSplitter";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmboListComparer
            // 
            this.cmboListComparer.FormattingEnabled = true;
            this.cmboListComparer.Items.AddRange(new object[] {
            resources.GetString("cmboListComparer.Items"),
            resources.GetString("cmboListComparer.Items1"),
            resources.GetString("cmboListComparer.Items2")});
            resources.ApplyResources(this.cmboListComparer, "cmboListComparer");
            this.cmboListComparer.Name = "cmboListComparer";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPrivacy
            // 
            this.tabPrivacy.Controls.Add(this.lblPrivacy);
            this.tabPrivacy.Controls.Add(this.chkPrivacy);
            resources.ApplyResources(this.tabPrivacy, "tabPrivacy");
            this.tabPrivacy.Name = "tabPrivacy";
            this.tabPrivacy.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.alertListBox);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // alertListBox
            // 
            this.alertListBox.FormattingEnabled = true;
            resources.ApplyResources(this.alertListBox, "alertListBox");
            this.alertListBox.Name = "alertListBox";
            // 
            // lblSaveAsDefaultFile
            // 
            resources.ApplyResources(this.lblSaveAsDefaultFile, "lblSaveAsDefaultFile");
            this.lblSaveAsDefaultFile.Name = "lblSaveAsDefaultFile";
            // 
            // MyPreferences
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tbPrefs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblSaveAsDefaultFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.AutoSaveEditBoxGroup.ResumeLayout(false);
            this.AutoSaveEditBoxGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEditBoxAutosave)).EndInit();
            this.tbPrefs.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabSite.ResumeLayout(false);
            this.tabSite.PerformLayout();
            this.tabEditing.ResumeLayout(false);
            this.tabEditing.PerformLayout();
            this.tabTools.ResumeLayout(false);
            this.tabTools.PerformLayout();
            this.tabPrivacy.ResumeLayout(false);
            this.tabPrivacy.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmboProject;
        private System.Windows.Forms.Label lblLang;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Label lblNonEnNotice;
        private System.Windows.Forms.Button btnTextBoxFont;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.CheckBox chkLowPriority;
        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox chkFlash;
        private System.Windows.Forms.CheckBox chkMinimize;
        private System.Windows.Forms.CheckBox chkSaveArticleList;
        private System.Windows.Forms.Label lblDoneDo;
        private System.Windows.Forms.CheckBox chkAutoSaveEdit;
        private System.Windows.Forms.GroupBox AutoSaveEditBoxGroup;
        private System.Windows.Forms.Label AutoSaveEditCont;
        private System.Windows.Forms.NumericUpDown nudEditBoxAutosave;
        private System.Windows.Forms.TextBox txtAutosave;
        private System.Windows.Forms.Label lblAutosaveFile;
        private System.Windows.Forms.ComboBox cmboCustomProject;
        private System.Windows.Forms.Button btnSetFile;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.Label lblPostfix;
        private System.Windows.Forms.CheckBox chkSupressAWB;
        private System.Windows.Forms.CheckBox chkAlwaysConfirmExit;
        private System.Windows.Forms.Label lblPrivacy;
        private System.Windows.Forms.CheckBox chkPrivacy;
        private System.Windows.Forms.CheckBox chkAddUsingAWBToActionSummaries;
        private System.Windows.Forms.TabControl tbPrefs;
        private System.Windows.Forms.TabPage tabSite;
        private System.Windows.Forms.TabPage tabPrivacy;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabEditing;
        private System.Windows.Forms.CheckBox chkIgnoreNoBots;
        private System.Windows.Forms.CheckBox chkShowTimer;
        private System.Windows.Forms.Label lblSaveAsDefaultFile;
        private WikiFunctions.Controls.AWBToolTip ToolTip;
        private System.Windows.Forms.TabPage tabTools;
        private System.Windows.Forms.ComboBox cmboListComparer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmboListSplitter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmboDBScanner;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmboOnLoad;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkDiffInBotMode;
        private System.Windows.Forms.CheckBox chkEmptyOnProjectChange;
        private System.Windows.Forms.CheckBox chkEnableLogging;
        private System.Windows.Forms.ComboBox cmboProtocol;
        private System.Windows.Forms.CheckBox chkDomain;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckedListBox alertListBox;
    }
}