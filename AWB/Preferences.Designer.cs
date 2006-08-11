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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkSetAsDefault = new System.Windows.Forms.CheckBox();
            this.btnTextBoxFont = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nudDiffFontSize = new System.Windows.Forms.NumericUpDown();
            this.chkScrollDown = new System.Windows.Forms.CheckBox();
            this.chkEnhanceDiff = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiffFontSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmboLang
            // 
            this.cmboLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.Location = new System.Drawing.Point(77, 90);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(121, 21);
            this.cmboLang.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(163, 294);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // cmboProject
            // 
            this.cmboProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboProject.FormattingEnabled = true;
            this.cmboProject.Location = new System.Drawing.Point(77, 63);
            this.cmboProject.Name = "cmboProject";
            this.cmboProject.Size = new System.Drawing.Size(121, 21);
            this.cmboProject.TabIndex = 2;
            this.cmboProject.SelectedIndexChanged += new System.EventHandler(this.cmboProject_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Language:";
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
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(284, 26);
            this.label3.TabIndex = 5;
            this.label3.Text = "Warning! for languages and projects other than the English\r\n wikipedia this softw" +
                "are may not function properly.";
            // 
            // chkSetAsDefault
            // 
            this.chkSetAsDefault.AutoSize = true;
            this.chkSetAsDefault.Location = new System.Drawing.Point(77, 117);
            this.chkSetAsDefault.Name = "chkSetAsDefault";
            this.chkSetAsDefault.Size = new System.Drawing.Size(91, 17);
            this.chkSetAsDefault.TabIndex = 6;
            this.chkSetAsDefault.Text = "Set as default";
            this.chkSetAsDefault.UseVisualStyleBackColor = true;
            // 
            // btnTextBoxFont
            // 
            this.btnTextBoxFont.Location = new System.Drawing.Point(5, 92);
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
            this.btnCancel.Location = new System.Drawing.Point(244, 294);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmboLang);
            this.groupBox1.Controls.Add(this.cmboProject);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkSetAsDefault);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(307, 141);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.chkEnhanceDiff);
            this.groupBox2.Controls.Add(this.chkScrollDown);
            this.groupBox2.Controls.Add(this.nudDiffFontSize);
            this.groupBox2.Controls.Add(this.btnTextBoxFont);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(13, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 128);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Other";
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
            // MyPreferences
            // 
            this.ClientSize = new System.Drawing.Size(331, 325);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ComboBox cmboProject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkSetAsDefault;
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
    }
}