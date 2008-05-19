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
                if (f != null) f.Dispose();

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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTextBoxFont = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPostfix = new System.Windows.Forms.Label();
            this.cmboCustomProject = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkAlwaysConfirmExit = new System.Windows.Forms.CheckBox();
            this.chkSupressAWB = new System.Windows.Forms.CheckBox();
            this.chkSaveArticleList = new System.Windows.Forms.CheckBox();
            this.chkMinimize = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkLowPriority = new System.Windows.Forms.CheckBox();
            this.numTimeOutLimit = new System.Windows.Forms.NumericUpDown();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.chkFlash = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAutoSaveEdit = new System.Windows.Forms.CheckBox();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSetFile = new System.Windows.Forms.Button();
            this.txtAutosave = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numEditBoxAutosave = new System.Windows.Forms.NumericUpDown();
            this.saveFile = new System.Windows.Forms.SaveFileDialog();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.PrivacyCheckBox = new System.Windows.Forms.CheckBox();
            this.PrivacyLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeOutLimit)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEditBoxAutosave)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownHeight = 212;
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.IntegralHeight = false;
            this.cmboLang.Location = new System.Drawing.Point(70, 46);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(121, 21);
            this.cmboLang.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(476, 399);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // cmboProject
            // 
            this.cmboProject.DropDownHeight = 206;
            this.cmboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProject.FormattingEnabled = true;
            this.cmboProject.IntegralHeight = false;
            this.cmboProject.Location = new System.Drawing.Point(70, 19);
            this.cmboProject.Name = "cmboProject";
            this.cmboProject.Size = new System.Drawing.Size(121, 21);
            this.cmboProject.TabIndex = 1;
            this.cmboProject.SelectedIndexChanged += new System.EventHandler(this.cmboProject_SelectedIndexChanged);
            // 
            // lblLang
            // 
            this.lblLang.Location = new System.Drawing.Point(6, 49);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(58, 13);
            this.lblLang.TabIndex = 2;
            this.lblLang.Text = "&Language:";
            this.lblLang.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Project:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(294, 26);
            this.label3.TabIndex = 0;
            this.label3.Text = "Languages and projects other than the English Wikipedia may not function properly" +
                ".";
            // 
            // btnTextBoxFont
            // 
            this.btnTextBoxFont.Location = new System.Drawing.Point(188, 19);
            this.btnTextBoxFont.Name = "btnTextBoxFont";
            this.btnTextBoxFont.Size = new System.Drawing.Size(112, 23);
            this.btnTextBoxFont.TabIndex = 10;
            this.btnTextBoxFont.Text = "Set edit box &font";
            this.btnTextBoxFont.UseVisualStyleBackColor = true;
            this.btnTextBoxFont.Click += new System.EventHandler(this.btnTextBoxFont_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(557, 399);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPostfix);
            this.groupBox1.Controls.Add(this.cmboLang);
            this.groupBox1.Controls.Add(this.cmboProject);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblLang);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmboCustomProject);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 102);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project";
            // 
            // lblPostfix
            // 
            this.lblPostfix.AutoSize = true;
            this.lblPostfix.Location = new System.Drawing.Point(197, 49);
            this.lblPostfix.Name = "lblPostfix";
            this.lblPostfix.Size = new System.Drawing.Size(48, 13);
            this.lblPostfix.TabIndex = 6;
            this.lblPostfix.Text = "lblPostfix";
            // 
            // cmboCustomProject
            // 
            this.cmboCustomProject.FormattingEnabled = true;
            this.cmboCustomProject.Location = new System.Drawing.Point(70, 46);
            this.cmboCustomProject.Name = "cmboCustomProject";
            this.cmboCustomProject.Size = new System.Drawing.Size(121, 21);
            this.cmboCustomProject.TabIndex = 5;
            this.cmboCustomProject.SelectedIndexChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            this.cmboCustomProject.TextChanged += new System.EventHandler(this.cmboCustomProjectChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(418, 383);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(214, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Save settings as default to remember details";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.chkAlwaysConfirmExit);
            this.groupBox2.Controls.Add(this.chkSupressAWB);
            this.groupBox2.Controls.Add(this.chkSaveArticleList);
            this.groupBox2.Controls.Add(this.chkMinimize);
            this.groupBox2.Controls.Add(this.btnTextBoxFont);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.chkLowPriority);
            this.groupBox2.Controls.Add(this.numTimeOutLimit);
            this.groupBox2.Controls.Add(this.chkBeep);
            this.groupBox2.Controls.Add(this.chkFlash);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 228);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 194);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Misc";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(95, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "seconds before &timing out";
            // 
            // chkAlwaysConfirmExit
            // 
            this.chkAlwaysConfirmExit.AutoSize = true;
            this.chkAlwaysConfirmExit.Checked = true;
            this.chkAlwaysConfirmExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlwaysConfirmExit.Location = new System.Drawing.Point(6, 88);
            this.chkAlwaysConfirmExit.Name = "chkAlwaysConfirmExit";
            this.chkAlwaysConfirmExit.Size = new System.Drawing.Size(86, 17);
            this.chkAlwaysConfirmExit.TabIndex = 9;
            this.chkAlwaysConfirmExit.Text = "&Warn on exit";
            this.chkAlwaysConfirmExit.UseVisualStyleBackColor = true;
            // 
            // chkSupressAWB
            // 
            this.chkSupressAWB.AutoSize = true;
            this.chkSupressAWB.Enabled = false;
            this.chkSupressAWB.Location = new System.Drawing.Point(6, 19);
            this.chkSupressAWB.Name = "chkSupressAWB";
            this.chkSupressAWB.Size = new System.Drawing.Size(138, 17);
            this.chkSupressAWB.TabIndex = 0;
            this.chkSupressAWB.Text = "&Suppress \"Using AWB\"";
            this.chkSupressAWB.UseVisualStyleBackColor = true;
            // 
            // chkSaveArticleList
            // 
            this.chkSaveArticleList.AutoSize = true;
            this.chkSaveArticleList.Checked = true;
            this.chkSaveArticleList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveArticleList.Location = new System.Drawing.Point(6, 111);
            this.chkSaveArticleList.Name = "chkSaveArticleList";
            this.chkSaveArticleList.Size = new System.Drawing.Size(159, 17);
            this.chkSaveArticleList.TabIndex = 3;
            this.chkSaveArticleList.Text = "Save pages list with settings";
            this.chkSaveArticleList.UseVisualStyleBackColor = true;
            // 
            // chkMinimize
            // 
            this.chkMinimize.AutoSize = true;
            this.chkMinimize.Location = new System.Drawing.Point(6, 65);
            this.chkMinimize.Name = "chkMinimize";
            this.chkMinimize.Size = new System.Drawing.Size(197, 17);
            this.chkMinimize.TabIndex = 2;
            this.chkMinimize.Text = "&Minimize to notification area (systray)";
            this.chkMinimize.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Wait";
            // 
            // chkLowPriority
            // 
            this.chkLowPriority.AutoSize = true;
            this.chkLowPriority.Location = new System.Drawing.Point(6, 42);
            this.chkLowPriority.Name = "chkLowPriority";
            this.chkLowPriority.Size = new System.Drawing.Size(250, 17);
            this.chkLowPriority.TabIndex = 1;
            this.chkLowPriority.Text = "Low &thread priority (works better in background)";
            this.chkLowPriority.UseVisualStyleBackColor = true;
            // 
            // numTimeOutLimit
            // 
            this.numTimeOutLimit.Location = new System.Drawing.Point(38, 151);
            this.numTimeOutLimit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.numTimeOutLimit.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numTimeOutLimit.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numTimeOutLimit.Name = "numTimeOutLimit";
            this.numTimeOutLimit.Size = new System.Drawing.Size(58, 20);
            this.numTimeOutLimit.TabIndex = 8;
            this.numTimeOutLimit.Value = new decimal(new int[] {
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
            this.chkBeep.Location = new System.Drawing.Point(171, 134);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(51, 17);
            this.chkBeep.TabIndex = 6;
            this.chkBeep.Text = "&Beep";
            this.chkBeep.UseVisualStyleBackColor = true;
            // 
            // chkFlash
            // 
            this.chkFlash.AutoSize = true;
            this.chkFlash.Checked = true;
            this.chkFlash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFlash.Location = new System.Drawing.Point(114, 134);
            this.chkFlash.Name = "chkFlash";
            this.chkFlash.Size = new System.Drawing.Size(51, 17);
            this.chkFlash.TabIndex = 5;
            this.chkFlash.Text = "&Flash";
            this.chkFlash.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 135);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "When ready to save:";
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSetFile);
            this.groupBox3.Controls.Add(this.chkAutoSaveEdit);
            this.groupBox3.Controls.Add(this.txtAutosave);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numEditBoxAutosave);
            this.groupBox3.Location = new System.Drawing.Point(12, 120);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(307, 102);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Auto save edit box";
            // 
            // btnSetFile
            // 
            this.btnSetFile.Enabled = false;
            this.btnSetFile.Location = new System.Drawing.Point(226, 73);
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
            this.txtAutosave.Size = new System.Drawing.Size(263, 20);
            this.txtAutosave.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 45);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "&File:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(253, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "seconds";
            // 
            // numEditBoxAutosave
            // 
            this.numEditBoxAutosave.Location = new System.Drawing.Point(189, 18);
            this.numEditBoxAutosave.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numEditBoxAutosave.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numEditBoxAutosave.Name = "numEditBoxAutosave";
            this.numEditBoxAutosave.Size = new System.Drawing.Size(58, 20);
            this.numEditBoxAutosave.TabIndex = 1;
            this.numEditBoxAutosave.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // saveFile
            // 
            this.saveFile.Filter = ".txt Files|*.txt";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.PrivacyCheckBox);
            this.groupBox4.Controls.Add(this.PrivacyLabel);
            this.groupBox4.Location = new System.Drawing.Point(325, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(307, 210);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Statistics";
            // 
            // PrivacyCheckBox
            // 
            this.PrivacyCheckBox.AutoSize = true;
            this.PrivacyCheckBox.Location = new System.Drawing.Point(6, 19);
            this.PrivacyCheckBox.Name = "PrivacyCheckBox";
            this.PrivacyCheckBox.Size = new System.Drawing.Size(186, 17);
            this.PrivacyCheckBox.TabIndex = 0;
            this.PrivacyCheckBox.Text = "Allow AWB to re&port my username";
            this.PrivacyCheckBox.UseVisualStyleBackColor = true;
            // 
            // PrivacyLabel
            // 
            this.PrivacyLabel.Location = new System.Drawing.Point(6, 39);
            this.PrivacyLabel.Name = "PrivacyLabel";
            this.PrivacyLabel.Size = new System.Drawing.Size(295, 160);
            this.PrivacyLabel.TabIndex = 1;
            this.PrivacyLabel.Text = resources.GetString("PrivacyLabel.Text");
            // 
            // MyPreferences
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(644, 434);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeOutLimit)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEditBoxAutosave)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmboProject;
        private System.Windows.Forms.Label lblLang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTextBoxFont;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.CheckBox chkLowPriority;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox chkFlash;
        private System.Windows.Forms.CheckBox chkMinimize;
        private System.Windows.Forms.CheckBox chkSaveArticleList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTimeOutLimit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkAutoSaveEdit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numEditBoxAutosave;
        private System.Windows.Forms.TextBox txtAutosave;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmboCustomProject;
        private System.Windows.Forms.Button btnSetFile;
        private System.Windows.Forms.SaveFileDialog saveFile;
        private System.Windows.Forms.Label lblPostfix;
        private System.Windows.Forms.CheckBox chkSupressAWB;
        private System.Windows.Forms.CheckBox chkAlwaysConfirmExit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label PrivacyLabel;
        private System.Windows.Forms.CheckBox PrivacyCheckBox;
        private System.Windows.Forms.Label label4;
    }
}