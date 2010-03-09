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
            this.chkEmptyOnProjectChange = new System.Windows.Forms.CheckBox();
            this.chkPHP5Ext = new System.Windows.Forms.CheckBox();
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
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblSaveAsDefaultFile = new System.Windows.Forms.Label();
            this.AutoSaveEditBoxGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudEditBoxAutosave)).BeginInit();
            this.tbPrefs.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabSite.SuspendLayout();
            this.tabEditing.SuspendLayout();
            this.tabTools.SuspendLayout();
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
            this.btnOK.Click += new System.EventHandler(this.btnOk_Click);
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
            this.lblNonEnNotice.Size = new System.Drawing.Size(370, 19);
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
            this.chkAddUsingAWBToActionSummaries.Text = "&Add \"using AWB\" to when deleting or protecting pages";
            this.chkAddUsingAWBToActionSummaries.UseVisualStyleBackColor = true;
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
            this.chkSaveArticleList.Text = "&Save page list with settings";
            this.ToolTip.SetToolTip(this.chkSaveArticleList, "Allows you to save the current page list every time the settings are saved");
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
            // chkLowPriority
            // 
            this.chkLowPriority.AutoSize = true;
            this.chkLowPriority.Location = new System.Drawing.Point(6, 75);
            this.chkLowPriority.Name = "chkLowPriority";
            this.chkLowPriority.Size = new System.Drawing.Size(250, 17);
            this.chkLowPriority.TabIndex = 4;
            this.chkLowPriority.Text = "&Low thread priority (works better in background)";
            this.ToolTip.SetToolTip(this.chkLowPriority, "Sets CPU thread priority to low");
            this.chkLowPriority.UseVisualStyleBackColor = true;
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
            this.chkBeep.Text = "B&eep";
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
            this.chkAutoSaveEdit.Text = "Automatically &save edit box every";
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
            this.lblAutosaveFile.Text = "&File:";
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
            this.chkPrivacy.Text = "&Include username to improve accuracy";
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
            this.tbPrefs.Controls.Add(this.tabTools);
            this.tbPrefs.Controls.Add(this.tabPrivacy);
            this.tbPrefs.Location = new System.Drawing.Point(12, 12);
            this.tbPrefs.Name = "tbPrefs";
            this.tbPrefs.SelectedIndex = 0;
            this.tbPrefs.Size = new System.Drawing.Size(390, 207);
            this.tbPrefs.TabIndex = 0;
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
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(382, 181);
            this.tabGeneral.TabIndex = 4;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // chkEnableLogging
            // 
            this.chkEnableLogging.AutoSize = true;
            this.chkEnableLogging.Checked = true;
            this.chkEnableLogging.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableLogging.Location = new System.Drawing.Point(8, 148);
            this.chkEnableLogging.Name = "chkEnableLogging";
            this.chkEnableLogging.Size = new System.Drawing.Size(100, 17);
            this.chkEnableLogging.TabIndex = 8;
            this.chkEnableLogging.Text = "Enable Logging";
            this.ToolTip.SetToolTip(this.chkEnableLogging, "Disactivate it to stop logging of saved/skipped pages in the Log tab");
            this.chkEnableLogging.UseVisualStyleBackColor = true;
            // 
            // chkDiffInBotMode
            // 
            this.chkDiffInBotMode.AutoSize = true;
            this.chkDiffInBotMode.Location = new System.Drawing.Point(8, 125);
            this.chkDiffInBotMode.Name = "chkDiffInBotMode";
            this.chkDiffInBotMode.Size = new System.Drawing.Size(161, 17);
            this.chkDiffInBotMode.TabIndex = 7;
            this.chkDiffInBotMode.Text = "Preview the Diff in Bot Mode";
            this.chkDiffInBotMode.UseVisualStyleBackColor = true;
            // 
            // cmboOnLoad
            // 
            this.cmboOnLoad.FormattingEnabled = true;
            this.cmboOnLoad.Items.AddRange(new object[] {
        	        	        	"Show changes",
        	        	        	"Show preview",
        	        	        	"Show edit page"});
            this.cmboOnLoad.Location = new System.Drawing.Point(58, 98);
            this.cmboOnLoad.Name = "cmboOnLoad";
            this.cmboOnLoad.Size = new System.Drawing.Size(121, 21);
            this.cmboOnLoad.TabIndex = 6;
            this.cmboOnLoad.SelectedIndexChanged += new System.EventHandler(this.cmboOnLoad_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "On load:";
            // 
            // tabSite
            // 
            this.tabSite.Controls.Add(this.chkEmptyOnProjectChange);
            this.tabSite.Controls.Add(this.chkPHP5Ext);
            this.tabSite.Controls.Add(this.chkSupressAWB);
            this.tabSite.Controls.Add(this.chkIgnoreNoBots);
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
            // chkEmptyOnProjectChange
            // 
            this.chkEmptyOnProjectChange.AutoSize = true;
            this.chkEmptyOnProjectChange.Location = new System.Drawing.Point(9, 125);
            this.chkEmptyOnProjectChange.Name = "chkEmptyOnProjectChange";
            this.chkEmptyOnProjectChange.Size = new System.Drawing.Size(186, 17);
            this.chkEmptyOnProjectChange.TabIndex = 12;
            this.chkEmptyOnProjectChange.Text = "Empty page list on project change";
            this.ToolTip.SetToolTip(this.chkEmptyOnProjectChange, "Automatically empties the page list on project change");
            this.chkEmptyOnProjectChange.UseVisualStyleBackColor = true;
            // 
            // chkPHP5Ext
            // 
            this.chkPHP5Ext.AutoSize = true;
            this.chkPHP5Ext.Enabled = false;
            this.chkPHP5Ext.Location = new System.Drawing.Point(214, 60);
            this.chkPHP5Ext.Name = "chkPHP5Ext";
            this.chkPHP5Ext.Size = new System.Drawing.Size(123, 17);
            this.chkPHP5Ext.TabIndex = 11;
            this.chkPHP5Ext.Text = "&Use .php5 extension";
            this.chkPHP5Ext.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreNoBots
            // 
            this.chkIgnoreNoBots.AutoSize = true;
            this.chkIgnoreNoBots.Location = new System.Drawing.Point(9, 102);
            this.chkIgnoreNoBots.Name = "chkIgnoreNoBots";
            this.chkIgnoreNoBots.Size = new System.Drawing.Size(167, 17);
            this.chkIgnoreNoBots.TabIndex = 10;
            this.chkIgnoreNoBots.Text = "&Ignore {{bots}} and {{nobots}}";
            this.ToolTip.SetToolTip(this.chkIgnoreNoBots, "Checking this box will cause you to edit pages with the {{bots}} and {{nobots}} t" +
                        "ags on them. By default these pages are skipped.");
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
            this.chkShowTimer.Location = new System.Drawing.Point(6, 82);
            this.chkShowTimer.Name = "chkShowTimer";
            this.chkShowTimer.Size = new System.Drawing.Size(164, 17);
            this.chkShowTimer.TabIndex = 0;
            this.chkShowTimer.Text = "&Display moving average timer";
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
            this.tabTools.Location = new System.Drawing.Point(4, 22);
            this.tabTools.Name = "tabTools";
            this.tabTools.Padding = new System.Windows.Forms.Padding(3);
            this.tabTools.Size = new System.Drawing.Size(382, 181);
            this.tabTools.TabIndex = 5;
            this.tabTools.Text = "Tools";
            this.tabTools.UseVisualStyleBackColor = true;
            // 
            // cmboDBScanner
            // 
            this.cmboDBScanner.FormattingEnabled = true;
            this.cmboDBScanner.Items.AddRange(new object[] {
        	        	        	"Ask",
        	        	        	"Always",
        	        	        	"Never"});
            this.cmboDBScanner.Location = new System.Drawing.Point(227, 66);
            this.cmboDBScanner.Name = "cmboDBScanner";
            this.cmboDBScanner.Size = new System.Drawing.Size(121, 21);
            this.cmboDBScanner.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(214, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Add Database Scanner results to current list";
            // 
            // cmboListSplitter
            // 
            this.cmboListSplitter.FormattingEnabled = true;
            this.cmboListSplitter.Items.AddRange(new object[] {
        	        	        	"Ask",
        	        	        	"Always",
        	        	        	"Never"});
            this.cmboListSplitter.Location = new System.Drawing.Point(227, 39);
            this.cmboListSplitter.Name = "cmboListSplitter";
            this.cmboListSplitter.Size = new System.Drawing.Size(121, 21);
            this.cmboListSplitter.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Add current article list to List Splitter:";
            // 
            // cmboListComparer
            // 
            this.cmboListComparer.FormattingEnabled = true;
            this.cmboListComparer.Items.AddRange(new object[] {
        	        	        	"Ask",
        	        	        	"Always",
        	        	        	"Never"});
            this.cmboListComparer.Location = new System.Drawing.Point(227, 12);
            this.cmboListComparer.Name = "cmboListComparer";
            this.cmboListComparer.Size = new System.Drawing.Size(121, 21);
            this.cmboListComparer.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Add current article list to List Comparer:";
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
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
        private System.Windows.Forms.CheckBox chkPHP5Ext;
        private System.Windows.Forms.ToolTip ToolTip;
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
    }
}