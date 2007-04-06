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
            this.cmboLang = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.cmboProject = new System.Windows.Forms.ComboBox();
            this.lblLang = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTextBoxFont = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nudDiffFontSize = new System.Windows.Forms.NumericUpDown();
            this.chkScrollDown = new System.Windows.Forms.CheckBox();
            this.chkEnhanceDiff = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCustomProject = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAutoSaveEdit = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numTimeOutLimit = new System.Windows.Forms.NumericUpDown();
            this.chkOverrideWatchlist = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSaveArticleList = new System.Windows.Forms.CheckBox();
            this.chkMinimize = new System.Windows.Forms.CheckBox();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.chkFlash = new System.Windows.Forms.CheckBox();
            this.chkLowPriority = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtAutosave = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.numEditBoxAutosave = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiffFontSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeOutLimit)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEditBoxAutosave)).BeginInit();
            this.SuspendLayout();
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownHeight = 212;
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.IntegralHeight = false;
            this.cmboLang.Location = new System.Drawing.Point(77, 90);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(121, 21);
            this.cmboLang.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(163, 499);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // cmboProject
            // 
            this.cmboProject.DropDownHeight = 206;
            this.cmboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProject.FormattingEnabled = true;
            this.cmboProject.IntegralHeight = false;
            this.cmboProject.Location = new System.Drawing.Point(77, 63);
            this.cmboProject.Name = "cmboProject";
            this.cmboProject.Size = new System.Drawing.Size(121, 21);
            this.cmboProject.TabIndex = 2;
            this.cmboProject.SelectedIndexChanged += new System.EventHandler(this.cmboProject_SelectedIndexChanged);
            // 
            // lblLang
            // 
            this.lblLang.Location = new System.Drawing.Point(13, 93);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(58, 13);
            this.lblLang.TabIndex = 3;
            this.lblLang.Text = "Language:";
            this.lblLang.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Project:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(290, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "Warning!  For languages and projects other than the English\r\n wikipedia this soft" +
                "ware may not function properly.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnTextBoxFont
            // 
            this.btnTextBoxFont.Location = new System.Drawing.Point(177, 19);
            this.btnTextBoxFont.Name = "btnTextBoxFont";
            this.btnTextBoxFont.Size = new System.Drawing.Size(112, 23);
            this.btnTextBoxFont.TabIndex = 11;
            this.btnTextBoxFont.Text = "Set text box font";
            this.btnTextBoxFont.UseVisualStyleBackColor = true;
            this.btnTextBoxFont.Click += new System.EventHandler(this.btnTextBoxFont_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Diff font size";
            // 
            // nudDiffFontSize
            // 
            this.nudDiffFontSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudDiffFontSize.Location = new System.Drawing.Point(77, 66);
            this.nudDiffFontSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDiffFontSize.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudDiffFontSize.Name = "nudDiffFontSize";
            this.nudDiffFontSize.Size = new System.Drawing.Size(49, 20);
            this.nudDiffFontSize.TabIndex = 9;
            this.nudDiffFontSize.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            // 
            // chkScrollDown
            // 
            this.chkScrollDown.AutoSize = true;
            this.chkScrollDown.Checked = true;
            this.chkScrollDown.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkScrollDown.Location = new System.Drawing.Point(6, 43);
            this.chkScrollDown.Name = "chkScrollDown";
            this.chkScrollDown.Size = new System.Drawing.Size(138, 17);
            this.chkScrollDown.TabIndex = 8;
            this.chkScrollDown.Text = "Scroll Down on preview";
            this.chkScrollDown.UseVisualStyleBackColor = true;
            // 
            // chkEnhanceDiff
            // 
            this.chkEnhanceDiff.AutoSize = true;
            this.chkEnhanceDiff.Checked = true;
            this.chkEnhanceDiff.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnhanceDiff.Location = new System.Drawing.Point(6, 19);
            this.chkEnhanceDiff.Name = "chkEnhanceDiff";
            this.chkEnhanceDiff.Size = new System.Drawing.Size(111, 17);
            this.chkEnhanceDiff.TabIndex = 7;
            this.chkEnhanceDiff.Text = "Enhance diff view";
            this.chkEnhanceDiff.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(244, 499);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmboLang);
            this.groupBox1.Controls.Add(this.cmboProject);
            this.groupBox1.Controls.Add(this.lblLang);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtCustomProject);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 141);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(214, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Save settings as default to remember details";
            // 
            // txtCustomProject
            // 
            this.txtCustomProject.Location = new System.Drawing.Point(78, 91);
            this.txtCustomProject.Name = "txtCustomProject";
            this.txtCustomProject.Size = new System.Drawing.Size(212, 20);
            this.txtCustomProject.TabIndex = 7;
            this.txtCustomProject.Visible = false;
            this.txtCustomProject.Leave += new System.EventHandler(this.txtCustomProject_Leave);
            this.txtCustomProject.TextChanged += new System.EventHandler(this.edtCustomProject_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkAutoSaveEdit);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numTimeOutLimit);
            this.groupBox2.Controls.Add(this.chkOverrideWatchlist);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.chkSaveArticleList);
            this.groupBox2.Controls.Add(this.chkMinimize);
            this.groupBox2.Controls.Add(this.chkBeep);
            this.groupBox2.Controls.Add(this.chkFlash);
            this.groupBox2.Controls.Add(this.chkLowPriority);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.chkEnhanceDiff);
            this.groupBox2.Controls.Add(this.chkScrollDown);
            this.groupBox2.Controls.Add(this.nudDiffFontSize);
            this.groupBox2.Controls.Add(this.btnTextBoxFont);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(13, 159);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 250);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other";
            // 
            // chkAutoSaveEdit
            // 
            this.chkAutoSaveEdit.AutoSize = true;
            this.chkAutoSaveEdit.Location = new System.Drawing.Point(11, 220);
            this.chkAutoSaveEdit.Name = "chkAutoSaveEdit";
            this.chkAutoSaveEdit.Size = new System.Drawing.Size(124, 17);
            this.chkAutoSaveEdit.TabIndex = 23;
            this.chkAutoSaveEdit.Text = "Auto Save Edit Box?";
            this.chkAutoSaveEdit.UseVisualStyleBackColor = true;
            this.chkAutoSaveEdit.CheckedChanged += new System.EventHandler(this.chkAutoSaveEdit_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Timeout Limit (seconds):";
            // 
            // numTimeOutLimit
            // 
            this.numTimeOutLimit.Location = new System.Drawing.Point(135, 194);
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
            this.numTimeOutLimit.TabIndex = 21;
            this.numTimeOutLimit.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // chkOverrideWatchlist
            // 
            this.chkOverrideWatchlist.AutoSize = true;
            this.chkOverrideWatchlist.Location = new System.Drawing.Point(6, 125);
            this.chkOverrideWatchlist.Name = "chkOverrideWatchlist";
            this.chkOverrideWatchlist.Size = new System.Drawing.Size(215, 17);
            this.chkOverrideWatchlist.TabIndex = 20;
            this.chkOverrideWatchlist.Text = "Allow AWB to override watchlist settings";
            this.chkOverrideWatchlist.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "When ready to save:";
            // 
            // chkSaveArticleList
            // 
            this.chkSaveArticleList.AutoSize = true;
            this.chkSaveArticleList.Checked = true;
            this.chkSaveArticleList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveArticleList.Location = new System.Drawing.Point(6, 148);
            this.chkSaveArticleList.Name = "chkSaveArticleList";
            this.chkSaveArticleList.Size = new System.Drawing.Size(185, 17);
            this.chkSaveArticleList.TabIndex = 18;
            this.chkSaveArticleList.Text = "Save article list on saving settings";
            this.chkSaveArticleList.UseVisualStyleBackColor = true;
            // 
            // chkMinimize
            // 
            this.chkMinimize.AutoSize = true;
            this.chkMinimize.Location = new System.Drawing.Point(123, 102);
            this.chkMinimize.Name = "chkMinimize";
            this.chkMinimize.Size = new System.Drawing.Size(133, 17);
            this.chkMinimize.TabIndex = 17;
            this.chkMinimize.Text = "Minimize to system tray";
            this.chkMinimize.UseVisualStyleBackColor = true;
            // 
            // chkBeep
            // 
            this.chkBeep.AutoSize = true;
            this.chkBeep.Checked = true;
            this.chkBeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBeep.Location = new System.Drawing.Point(170, 171);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(51, 17);
            this.chkBeep.TabIndex = 16;
            this.chkBeep.Text = "Beep";
            this.chkBeep.UseVisualStyleBackColor = true;
            // 
            // chkFlash
            // 
            this.chkFlash.AutoSize = true;
            this.chkFlash.Checked = true;
            this.chkFlash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFlash.Location = new System.Drawing.Point(118, 171);
            this.chkFlash.Name = "chkFlash";
            this.chkFlash.Size = new System.Drawing.Size(51, 17);
            this.chkFlash.TabIndex = 15;
            this.chkFlash.Text = "Flash";
            this.chkFlash.UseVisualStyleBackColor = true;
            // 
            // chkLowPriority
            // 
            this.chkLowPriority.AutoSize = true;
            this.chkLowPriority.Location = new System.Drawing.Point(6, 102);
            this.chkLowPriority.Name = "chkLowPriority";
            this.chkLowPriority.Size = new System.Drawing.Size(112, 17);
            this.chkLowPriority.TabIndex = 13;
            this.chkLowPriority.Text = "Low thread priority";
            this.chkLowPriority.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(129, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "%";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtAutosave);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.numEditBoxAutosave);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(13, 415);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(306, 78);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "AutoSave Edit Box";
            // 
            // txtAutosave
            // 
            this.txtAutosave.Location = new System.Drawing.Point(77, 40);
            this.txtAutosave.Name = "txtAutosave";
            this.txtAutosave.Size = new System.Drawing.Size(179, 20);
            this.txtAutosave.TabIndex = 30;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(27, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "To file:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(203, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "(seconds)";
            // 
            // numEditBoxAutosave
            // 
            this.numEditBoxAutosave.Location = new System.Drawing.Point(139, 14);
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
            this.numEditBoxAutosave.TabIndex = 27;
            this.numEditBoxAutosave.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(126, 13);
            this.label8.TabIndex = 26;
            this.label8.Text = "Autosave Edit Box every:";
            // 
            // MyPreferences
            // 
            this.ClientSize = new System.Drawing.Size(331, 534);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preferences";
            ((System.ComponentModel.ISupportInitialize)(this.nudDiffFontSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeOutLimit)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEditBoxAutosave)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ComboBox cmboProject;
        private System.Windows.Forms.Label lblLang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnTextBoxFont;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudDiffFontSize;
        private System.Windows.Forms.CheckBox chkScrollDown;
        private System.Windows.Forms.CheckBox chkEnhanceDiff;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkLowPriority;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCustomProject;
        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox chkFlash;
        private System.Windows.Forms.CheckBox chkMinimize;
        private System.Windows.Forms.CheckBox chkSaveArticleList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOverrideWatchlist;
        private System.Windows.Forms.NumericUpDown numTimeOutLimit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkAutoSaveEdit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown numEditBoxAutosave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAutosave;
        private System.Windows.Forms.Label label10;
    }
}