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
            this.lblTimeoutPost = new System.Windows.Forms.Label();
            this.chkAlwaysConfirmExit = new System.Windows.Forms.CheckBox();
            this.chkSupressAWB = new System.Windows.Forms.CheckBox();
            this.chkSaveArticleList = new System.Windows.Forms.CheckBox();
            this.chkMinimize = new System.Windows.Forms.CheckBox();
            this.lblTimeoutPre = new System.Windows.Forms.Label();
            this.chkLowPriority = new System.Windows.Forms.CheckBox();
            this.nudTimeOutLimit = new System.Windows.Forms.NumericUpDown();
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
            this.tabSite = new System.Windows.Forms.TabPage();
            this.chkPHP5Ext = new System.Windows.Forms.CheckBox();
            this.chkIgnoreNoBots = new System.Windows.Forms.CheckBox();
            this.tabEditing = new System.Windows.Forms.TabPage();
            this.chkShowTimer = new System.Windows.Forms.CheckBox();
            this.tabPrivacy = new System.Windows.Forms.TabPage();
            this.lblSaveAsDefaultFile = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeOutLimit)).BeginInit();
            this.AutoSaveEditBoxGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEditBoxAutosave)).BeginInit();
            this.tbPrefs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabSite.SuspendLayout();
            this.tabEditing.SuspendLayout();
            this.tabPrivacy.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownHeight = 212;
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.IntegralHeight = false;
            this.cmboLang.Location = new System.Drawing.Point(70, 33);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(121, 21);
            this.cmboLang.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(246, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cmboProject
            // 
            this.cmboProject.DropDownHeight = 206;
            this.cmboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProject.FormattingEnabled = true;
            this.cmboProject.IntegralHeight = false;
            this.cmboProject.Location = new System.Drawing.Point(70, 6);
            this.cmboProject.Name = "cmboProject";
            this.cmboProject.Size = new System.Drawing.Size(121, 21);
            this.cmboProject.TabIndex = 1;
            this.cmboProject.SelectedIndexChanged += new System.EventHandler(this.cmboProject_SelectedIndexChanged);
            // 
            // lblLang
            // 
            this.lblLang.Location = new System.Drawing.Point(6, 36);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(58, 13);
            this.lblLang.TabIndex = 2;
            this.lblLang.Text = "&Language:";
            this.lblLang.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblProject
            // 
            this.lblProject.AutoSize = true;
            this.lblProject.Location = new System.Drawing.Point(21, 9);
            this.lblProject.Name = "lblProject";
            this.lblProject.Size = new System.Drawing.Size(43, 13);
            this.lblProject.TabIndex = 0;
            this.lblProject.Text = "&Project:";
            // 
            // lblNonEnNotice
            // 
            this.lblNonEnNotice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNonEnNotice.Location = new System.Drawing.Point(6, 80);
            this.lblNonEnNotice.Name = "lblNonEnNotice";
            this.lblNonEnNotice.Size = new System.Drawing.Size(370, 26);
            this.lblNonEnNotice.TabIndex = 6;
            this.lblNonEnNotice.Text = "Wikis not related to Wikimedia are not guaranteed to function properly.";
            // 
            // btnTextBoxFont
            // 
            this.btnTextBoxFont.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTextBoxFont.Location = new System.Drawing.Point(267, 152);
            this.btnTextBoxFont.Name = "btnTextBoxFont";
            this.btnTextBoxFont.Size = new System.Drawing.Size(112, 23);
            this.btnTextBoxFont.TabIndex = 5;
            this.btnTextBoxFont.Text = "Set edit box &font";
            this.btnTextBoxFont.UseVisualStyleBackColor = true;
            this.btnTextBoxFont.Click += new System.EventHandler(this.btnTextBoxFont_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(327, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            // 
            // lblPostfix
            // 
            this.lblPostfix.AutoSize = true;
            this.lblPostfix.Location = new System.Drawing.Point(197, 36);
            this.lblPostfix.Name = "lblPostfix";
            this.lblPostfix.Size = new System.Drawing.Size(48, 13);
            this.lblPostfix.TabIndex = 4;
            this.lblPostfix.Text = "lblPostfix";
            // 
            // cmboCustomProject
            // 
            this.cmboCustomProject.FormattingEnabled = true;
            this.cmboCustomProject.Location = new System.Drawing.Point(70, 33);
            this.cmboCustomProject.Name = "cmboCustomProject";
            this.cmboCustomProject.Size = new System.Drawing.Size(121, 21);
            this.cmboCustomProject.TabIndex = 5;
            this.cmboCustomProject.SelectedIndexChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            this.cmboCustomProject.Leave += new System.EventHandler(this.txtCustomProject_Leave);
            this.cmboCustomProject.TextChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            // 
            // chkAddUsingAWBToActionSummaries
            // 
            this.chkAddUsingAWBToActionSummaries.AutoSize = true;
            this.chkAddUsingAWBToActionSummaries.Location = new System.Drawing.Point(6, 105);
            this.chkAddUsingAWBToActionSummaries.Name = "chkAddUsingAWBToActionSummaries";
            this.chkAddUsingAWBToActionSummaries.Size = new System.Drawing.Size(286, 17);
            this.chkAddUsingAWBToActionSummaries.TabIndex = 1;
            this.chkAddUsingAWBToActionSummaries.Text = "Add \"using AWB\" to when deleting or protecting pages";
            this.chkAddUsingAWBToActionSummaries.UseVisualStyleBackColor = true;
            // 
            // lblTimeoutPost
            // 
            this.lblTimeoutPost.AutoSize = true;
            this.lblTimeoutPost.Location = new System.Drawing.Point(98, 111);
            this.lblTimeoutPost.Name = "lblTimeoutPost";
            this.lblTimeoutPost.Size = new System.Drawing.Size(183, 13);
            this.lblTimeoutPost.TabIndex = 7;
            this.lblTimeoutPost.Text = "seconds before web control &times out";
            // 
            // chkAlwaysConfirmExit
            // 
            this.chkAlwaysConfirmExit.AutoSize = true;
            this.chkAlwaysConfirmExit.Checked = true;
            this.chkAlwaysConfirmExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlwaysConfirmExit.Location = new System.Drawing.Point(6, 29);
            this.chkAlwaysConfirmExit.Name = "chkAlwaysConfirmExit";
            this.chkAlwaysConfirmExit.Size = new System.Drawing.Size(86, 17);
            this.chkAlwaysConfirmExit.TabIndex = 2;
            this.chkAlwaysConfirmExit.Text = "&Warn on exit";
            this.chkAlwaysConfirmExit.UseVisualStyleBackColor = true;
            // 
            // chkSupressAWB
            // 
            this.chkSupressAWB.AutoSize = true;
            this.chkSupressAWB.Enabled = false;
            this.chkSupressAWB.Location = new System.Drawing.Point(70, 60);
            this.chkSupressAWB.Name = "chkSupressAWB";
            this.chkSupressAWB.Size = new System.Drawing.Size(138, 17);
            this.chkSupressAWB.TabIndex = 5;
            this.chkSupressAWB.Text = "&Suppress \"Using AWB\"";
            this.chkSupressAWB.UseVisualStyleBackColor = true;
            // 
            // chkSaveArticleList
            // 
            this.chkSaveArticleList.AutoSize = true;
            this.chkSaveArticleList.Checked = true;
            this.chkSaveArticleList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveArticleList.Location = new System.Drawing.Point(6, 52);
            this.chkSaveArticleList.Name = "chkSaveArticleList";
            this.chkSaveArticleList.Size = new System.Drawing.Size(154, 17);
            this.chkSaveArticleList.TabIndex = 3;
            this.chkSaveArticleList.Text = "Save page &list with settings";
            this.chkSaveArticleList.UseVisualStyleBackColor = true;
            // 
            // chkMinimize
            // 
            this.chkMinimize.AutoSize = true;
            this.chkMinimize.Location = new System.Drawing.Point(6, 6);
            this.chkMinimize.Name = "chkMinimize";
            this.chkMinimize.Size = new System.Drawing.Size(197, 17);
            this.chkMinimize.TabIndex = 1;
            this.chkMinimize.Text = "&Minimize to notification area (systray)";
            this.chkMinimize.UseVisualStyleBackColor = true;
            // 
            // lblTimeoutPre
            // 
            this.lblTimeoutPre.AutoSize = true;
            this.lblTimeoutPre.Location = new System.Drawing.Point(5, 111);
            this.lblTimeoutPre.Name = "lblTimeoutPre";
            this.lblTimeoutPre.Size = new System.Drawing.Size(29, 13);
            this.lblTimeoutPre.TabIndex = 9;
            this.lblTimeoutPre.Text = "Wait";
            // 
            // chkLowPriority
            // 
            this.chkLowPriority.AutoSize = true;
            this.chkLowPriority.Location = new System.Drawing.Point(6, 75);
            this.chkLowPriority.Name = "chkLowPriority";
            this.chkLowPriority.Size = new System.Drawing.Size(250, 17);
            this.chkLowPriority.TabIndex = 4;
            this.chkLowPriority.Text = "Low &thread priority (works better in background)";
            this.chkLowPriority.UseVisualStyleBackColor = true;
            // 
            // nudTimeOutLimit
            // 
            this.nudTimeOutLimit.Location = new System.Drawing.Point(37, 109);
            this.nudTimeOutLimit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.nudTimeOutLimit.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.nudTimeOutLimit.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudTimeOutLimit.Name = "nudTimeOutLimit";
            this.nudTimeOutLimit.Size = new System.Drawing.Size(58, 20);
            this.nudTimeOutLimit.TabIndex = 8;
            this.nudTimeOutLimit.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // chkBeep
            // 
            this.chkBeep.AutoSize = true;
            this.chkBeep.Checked = true;
            this.chkBeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBeep.Location = new System.Drawing.Point(178, 128);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(51, 17);
            this.chkBeep.TabIndex = 4;
            this.chkBeep.Text = "&Beep";
            this.chkBeep.UseVisualStyleBackColor = true;
            // 
            // chkFlash
            // 
            this.chkFlash.AutoSize = true;
            this.chkFlash.Checked = true;
            this.chkFlash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFlash.Location = new System.Drawing.Point(121, 128);
            this.chkFlash.Name = "chkFlash";
            this.chkFlash.Size = new System.Drawing.Size(51, 17);
            this.chkFlash.TabIndex = 3;
            this.chkFlash.Text = "&Flash";
            this.chkFlash.UseVisualStyleBackColor = true;
            // 
            // lblDoneDo
            // 
            this.lblDoneDo.AutoSize = true;
            this.lblDoneDo.Location = new System.Drawing.Point(9, 129);
            this.lblDoneDo.Name = "lblDoneDo";
            this.lblDoneDo.Size = new System.Drawing.Size(106, 13);
            this.lblDoneDo.TabIndex = 2;
            this.lblDoneDo.Text = "When ready to save:";
            // 
            // chkAutoSaveEdit
            // 
            this.chkAutoSaveEdit.AutoSize = true;
            this.chkAutoSaveEdit.Location = new System.Drawing.Point(6, 19);
            this.chkAutoSaveEdit.Name = "chkAutoSaveEdit";
            this.chkAutoSaveEdit.Size = new System.Drawing.Size(183, 17);
            this.chkAutoSaveEdit.TabIndex = 0;
            this.chkAutoSaveEdit.Text = "A&utomatically save edit box every";
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
            this.AutoSaveEditBoxGroup.Location = new System.Drawing.Point(6, 6);
            this.AutoSaveEditBoxGroup.Name = "AutoSaveEditBoxGroup";
            this.AutoSaveEditBoxGroup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.AutoSaveEditBoxGroup.Size = new System.Drawing.Size(370, 70);
            this.AutoSaveEditBoxGroup.TabIndex = 0;
            this.AutoSaveEditBoxGroup.TabStop = false;
            this.AutoSaveEditBoxGroup.Text = "Auto save edit box";
            // 
            // btnSetFile
            // 
            this.btnSetFile.Enabled = false;
            this.btnSetFile.Location = new System.Drawing.Point(289, 40);
            this.btnSetFile.Name = "btnSetFile";
            this.btnSetFile.Size = new System.Drawing.Size(75, 23);
            this.btnSetFile.TabIndex = 5;
            this.btnSetFile.Text = "&Browse";
            this.btnSetFile.UseVisualStyleBackColor = true;
            this.btnSetFile.Click += new System.EventHandler(this.btnSetFile_Click);
            // 
            // txtAutosave
            // 
            this.txtAutosave.Location = new System.Drawing.Point(38, 42);
            this.txtAutosave.Name = "txtAutosave";
            this.txtAutosave.ReadOnly = true;
            this.txtAutosave.Size = new System.Drawing.Size(245, 20);
            this.txtAutosave.TabIndex = 4;
            // 
            // lblAutosaveFile
            // 
            this.lblAutosaveFile.AutoSize = true;
            this.lblAutosaveFile.Location = new System.Drawing.Point(6, 45);
            this.lblAutosaveFile.Name = "lblAutosaveFile";
            this.lblAutosaveFile.Size = new System.Drawing.Size(26, 13);
            this.lblAutosaveFile.TabIndex = 3;
            this.lblAutosaveFile.Text = "File:";
            // 
            // AutoSaveEditCont
            // 
            this.AutoSaveEditCont.AutoSize = true;
            this.AutoSaveEditCont.Location = new System.Drawing.Point(248, 20);
            this.AutoSaveEditCont.Name = "AutoSaveEditCont";
            this.AutoSaveEditCont.Size = new System.Drawing.Size(47, 13);
            this.AutoSaveEditCont.TabIndex = 2;
            this.AutoSaveEditCont.Text = "seconds";
            // 
            // nudEditBoxAutosave
            // 
            this.nudEditBoxAutosave.Location = new System.Drawing.Point(189, 18);
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
            this.nudEditBoxAutosave.Size = new System.Drawing.Size(58, 20);
            this.nudEditBoxAutosave.TabIndex = 1;
            this.nudEditBoxAutosave.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // saveFile
            // 
            this.saveFile.Filter = ".txt Files|*.txt";
            // 
            // chkPrivacy
            // 
            this.chkPrivacy.AutoSize = true;
            this.chkPrivacy.Checked = true;
            this.chkPrivacy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPrivacy.Location = new System.Drawing.Point(6, 6);
            this.chkPrivacy.Name = "chkPrivacy";
            this.chkPrivacy.Size = new System.Drawing.Size(209, 17);
            this.chkPrivacy.TabIndex = 0;
            this.chkPrivacy.Text = "Include username to im&prove accuracy";
            // 
            // lblPrivacy
            // 
            this.lblPrivacy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrivacy.Location = new System.Drawing.Point(6, 26);
            this.lblPrivacy.Name = "lblPrivacy";
            this.lblPrivacy.Size = new System.Drawing.Size(370, 129);
            this.lblPrivacy.TabIndex = 1;
            this.lblPrivacy.Text = resources.GetString("lblPrivacy.Text");
            // 
            // tbPrefs
            // 
            this.tbPrefs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPrefs.Controls.Add(this.tabGeneral);
            this.tbPrefs.Controls.Add(this.tabSite);
            this.tbPrefs.Controls.Add(this.tabEditing);
            this.tbPrefs.Controls.Add(this.tabPrivacy);
            this.tbPrefs.Location = new System.Drawing.Point(12, 12);
            this.tbPrefs.Name = "tbPrefs";
            this.tbPrefs.SelectedIndex = 0;
            this.tbPrefs.Size = new System.Drawing.Size(390, 207);
            this.tbPrefs.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chkMinimize);
            this.tabGeneral.Controls.Add(this.chkLowPriority);
            this.tabGeneral.Controls.Add(this.chkSaveArticleList);
            this.tabGeneral.Controls.Add(this.chkAlwaysConfirmExit);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(382, 181);
            this.tabGeneral.TabIndex = 4;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // tabSite
            // 
            this.tabSite.Controls.Add(this.chkPHP5Ext);
            this.tabSite.Controls.Add(this.chkSupressAWB);
            this.tabSite.Controls.Add(this.chkIgnoreNoBots);
            this.tabSite.Controls.Add(this.lblTimeoutPost);
            this.tabSite.Controls.Add(this.nudTimeOutLimit);
            this.tabSite.Controls.Add(this.lblTimeoutPre);
            this.tabSite.Controls.Add(this.lblPostfix);
            this.tabSite.Controls.Add(this.cmboLang);
            this.tabSite.Controls.Add(this.lblLang);
            this.tabSite.Controls.Add(this.cmboProject);
            this.tabSite.Controls.Add(this.lblProject);
            this.tabSite.Controls.Add(this.cmboCustomProject);
            this.tabSite.Controls.Add(this.lblNonEnNotice);
            this.tabSite.Location = new System.Drawing.Point(4, 22);
            this.tabSite.Name = "tabSite";
            this.tabSite.Padding = new System.Windows.Forms.Padding(3);
            this.tabSite.Size = new System.Drawing.Size(382, 181);
            this.tabSite.TabIndex = 0;
            this.tabSite.Text = "Site";
            this.tabSite.UseVisualStyleBackColor = true;
            // 
            // chkPHP5Ext
            // 
            this.chkPHP5Ext.AutoSize = true;
            this.chkPHP5Ext.Enabled = false;
            this.chkPHP5Ext.Location = new System.Drawing.Point(214, 60);
            this.chkPHP5Ext.Name = "chkPHP5Ext";
            this.chkPHP5Ext.Size = new System.Drawing.Size(123, 17);
            this.chkPHP5Ext.TabIndex = 11;
            this.chkPHP5Ext.Text = "Use .php5 extension";
            this.chkPHP5Ext.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreNoBots
            // 
            this.chkIgnoreNoBots.AutoSize = true;
            this.chkIgnoreNoBots.Location = new System.Drawing.Point(8, 135);
            this.chkIgnoreNoBots.Name = "chkIgnoreNoBots";
            this.chkIgnoreNoBots.Size = new System.Drawing.Size(167, 17);
            this.chkIgnoreNoBots.TabIndex = 10;
            this.chkIgnoreNoBots.Text = "Ignore {{bots}} and {{no&bots}}";
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
            this.tabEditing.Location = new System.Drawing.Point(4, 22);
            this.tabEditing.Name = "tabEditing";
            this.tabEditing.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditing.Size = new System.Drawing.Size(382, 181);
            this.tabEditing.TabIndex = 3;
            this.tabEditing.Text = "Editing and saving";
            this.tabEditing.UseVisualStyleBackColor = true;
            // 
            // chkShowTimer
            // 
            this.chkShowTimer.AutoSize = true;
            this.chkShowTimer.Enabled = false;
            this.chkShowTimer.Location = new System.Drawing.Point(6, 82);
            this.chkShowTimer.Name = "chkShowTimer";
            this.chkShowTimer.Size = new System.Drawing.Size(164, 17);
            this.chkShowTimer.TabIndex = 0;
            this.chkShowTimer.Text = "Display moving average &timer";
            this.chkShowTimer.UseVisualStyleBackColor = true;
            // 
            // tabPrivacy
            // 
            this.tabPrivacy.Controls.Add(this.lblPrivacy);
            this.tabPrivacy.Controls.Add(this.chkPrivacy);
            this.tabPrivacy.Location = new System.Drawing.Point(4, 22);
            this.tabPrivacy.Name = "tabPrivacy";
            this.tabPrivacy.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrivacy.Size = new System.Drawing.Size(382, 181);
            this.tabPrivacy.TabIndex = 1;
            this.tabPrivacy.Text = "Privacy";
            this.tabPrivacy.UseVisualStyleBackColor = true;
            // 
            // lblSaveAsDefaultFile
            // 
            this.lblSaveAsDefaultFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSaveAsDefaultFile.AutoSize = true;
            this.lblSaveAsDefaultFile.Location = new System.Drawing.Point(13, 233);
            this.lblSaveAsDefaultFile.Name = "lblSaveAsDefaultFile";
            this.lblSaveAsDefaultFile.Size = new System.Drawing.Size(205, 13);
            this.lblSaveAsDefaultFile.TabIndex = 1;
            this.lblSaveAsDefaultFile.Text = "\"Save settings as default\" to retain values";
            // 
            // MyPreferences
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(414, 263);
            this.Controls.Add(this.tbPrefs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblSaveAsDefaultFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeOutLimit)).EndInit();
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
            this.tabPrivacy.ResumeLayout(false);
            this.tabPrivacy.PerformLayout();
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
        private System.Windows.Forms.NumericUpDown nudTimeOutLimit;
        private System.Windows.Forms.Label lblTimeoutPre;
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
        private System.Windows.Forms.Label lblTimeoutPost;
        private System.Windows.Forms.CheckBox chkAddUsingAWBToActionSummaries;
        private System.Windows.Forms.TabControl tbPrefs;
        private System.Windows.Forms.TabPage tabSite;
        private System.Windows.Forms.TabPage tabPrivacy;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabEditing;
        private System.Windows.Forms.CheckBox chkIgnoreNoBots;
        private System.Windows.Forms.CheckBox chkShowTimer;
        private System.Windows.Forms.Label lblSaveAsDefaultFile;
        private System.Windows.Forms.CheckBox chkPHP5Ext;
    }
}
