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
            this.chkEnhanceDiff = new System.Windows.Forms.CheckBox();
            this.chkScrollDown = new System.Windows.Forms.CheckBox();
            this.nudDiffFontSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnTextBoxFont = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudDiffFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // chkEnhanceDiff
            // 
            this.chkEnhanceDiff.AutoSize = true;
            this.chkEnhanceDiff.Location = new System.Drawing.Point(13, 14);
            this.chkEnhanceDiff.Name = "chkEnhanceDiff";
            this.chkEnhanceDiff.Size = new System.Drawing.Size(111, 17);
            this.chkEnhanceDiff.TabIndex = 0;
            this.chkEnhanceDiff.Text = "Enhance diff view";
            this.chkEnhanceDiff.UseVisualStyleBackColor = true;
            // 
            // chkScrollDown
            // 
            this.chkScrollDown.AutoSize = true;
            this.chkScrollDown.Location = new System.Drawing.Point(13, 38);
            this.chkScrollDown.Name = "chkScrollDown";
            this.chkScrollDown.Size = new System.Drawing.Size(138, 17);
            this.chkScrollDown.TabIndex = 1;
            this.chkScrollDown.Text = "Scroll Down on preview";
            this.chkScrollDown.UseVisualStyleBackColor = true;
            // 
            // nudDiffFontSize
            // 
            this.nudDiffFontSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudDiffFontSize.Location = new System.Drawing.Point(84, 61);
            this.nudDiffFontSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudDiffFontSize.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudDiffFontSize.Name = "nudDiffFontSize";
            this.nudDiffFontSize.Size = new System.Drawing.Size(49, 20);
            this.nudDiffFontSize.TabIndex = 2;
            this.nudDiffFontSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Diff font size";
            // 
            // btnTextBoxFont
            // 
            this.btnTextBoxFont.Location = new System.Drawing.Point(12, 87);
            this.btnTextBoxFont.Name = "btnTextBoxFont";
            this.btnTextBoxFont.Size = new System.Drawing.Size(82, 23);
            this.btnTextBoxFont.TabIndex = 4;
            this.btnTextBoxFont.Text = "Text box font";
            this.btnTextBoxFont.UseVisualStyleBackColor = true;
            this.btnTextBoxFont.Click += new System.EventHandler(this.btnTextBoxFont_Click);
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(246, 144);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(246, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MyPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 210);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnTextBoxFont);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudDiffFontSize);
            this.Controls.Add(this.chkScrollDown);
            this.Controls.Add(this.chkEnhanceDiff);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MyPreferences";
            this.ShowIcon = false;
            this.Text = "Preferences";
            ((System.ComponentModel.ISupportInitialize)(this.nudDiffFontSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnhanceDiff;
        private System.Windows.Forms.CheckBox chkScrollDown;
        private System.Windows.Forms.NumericUpDown nudDiffFontSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnTextBoxFont;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}