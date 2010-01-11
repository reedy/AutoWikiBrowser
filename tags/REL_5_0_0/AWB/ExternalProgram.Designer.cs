namespace AutoWikiBrowser
{
    partial class ExternalProgram
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
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectIO = new System.Windows.Forms.Button();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSelectProgram = new System.Windows.Forms.Button();
            this.radFile = new System.Windows.Forms.RadioButton();
            this.radParameter = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.txtProgram = new System.Windows.Forms.TextBox();
            this.chkSkip = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.openProgram = new System.Windows.Forms.OpenFileDialog();
            this.openIO = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(12, 18);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkEnabled.TabIndex = 0;
            this.chkEnabled.Text = "&Enabled";
            this.chkEnabled.UseVisualStyleBackColor = true;
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSelectIO);
            this.groupBox1.Controls.Add(this.txtFile);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnSelectProgram);
            this.groupBox1.Controls.Add(this.radFile);
            this.groupBox1.Controls.Add(this.radParameter);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtParameters);
            this.groupBox1.Controls.Add(this.txtProgram);
            this.groupBox1.Location = new System.Drawing.Point(12, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(322, 187);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // btnSelectIO
            // 
            this.btnSelectIO.Location = new System.Drawing.Point(91, 100);
            this.btnSelectIO.Name = "btnSelectIO";
            this.btnSelectIO.Size = new System.Drawing.Size(140, 23);
            this.btnSelectIO.TabIndex = 11;
            this.btnSelectIO.Text = "Select I/O File";
            this.btnSelectIO.UseVisualStyleBackColor = true;
            this.btnSelectIO.Click += new System.EventHandler(this.btnSelectIO_Click);
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(130, 129);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(186, 20);
            this.txtFile.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "&Input/Output file:";
            // 
            // btnSelectProgram
            // 
            this.btnSelectProgram.Location = new System.Drawing.Point(91, 19);
            this.btnSelectProgram.Name = "btnSelectProgram";
            this.btnSelectProgram.Size = new System.Drawing.Size(140, 23);
            this.btnSelectProgram.TabIndex = 0;
            this.btnSelectProgram.Text = "&Select program/script";
            this.btnSelectProgram.UseVisualStyleBackColor = true;
            this.btnSelectProgram.Click += new System.EventHandler(this.btnSelectProgram_Click);
            // 
            // radFile
            // 
            this.radFile.AutoSize = true;
            this.radFile.Checked = true;
            this.radFile.Location = new System.Drawing.Point(15, 164);
            this.radFile.Name = "radFile";
            this.radFile.Size = new System.Drawing.Size(129, 17);
            this.radFile.TabIndex = 9;
            this.radFile.TabStop = true;
            this.radFile.Text = "Pass article text as &file";
            this.radFile.UseVisualStyleBackColor = true;
            // 
            // radParameter
            // 
            this.radParameter.AutoSize = true;
            this.radParameter.Location = new System.Drawing.Point(153, 164);
            this.radParameter.Name = "radParameter";
            this.radParameter.Size = new System.Drawing.Size(163, 17);
            this.radParameter.TabIndex = 10;
            this.radParameter.Text = "Pass article &text as parameter";
            this.radParameter.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "&Arguments/Parameters:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Program or script:";
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(130, 74);
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(186, 20);
            this.txtParameters.TabIndex = 6;
            this.txtParameters.Text = "%%file%%";
            // 
            // txtProgram
            // 
            this.txtProgram.Location = new System.Drawing.Point(130, 48);
            this.txtProgram.Name = "txtProgram";
            this.txtProgram.Size = new System.Drawing.Size(186, 20);
            this.txtProgram.TabIndex = 4;
            // 
            // chkSkip
            // 
            this.chkSkip.AutoSize = true;
            this.chkSkip.Location = new System.Drawing.Point(83, 18);
            this.chkSkip.Name = "chkSkip";
            this.chkSkip.Size = new System.Drawing.Size(109, 17);
            this.chkSkip.TabIndex = 1;
            this.chkSkip.Text = "Skip if &no change";
            this.chkSkip.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Location = new System.Drawing.Point(259, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Close";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // openProgram
            // 
            this.openProgram.DefaultExt = "exe";
            this.openProgram.Filter = "Executable files (*.exe)|*.exe|Scripts (*.bat; *.pl; *py; *.vbs; *.php)|*.bat;*.p" +
                "l;*py;*.vbs;*.php";
            // 
            // openIO
            // 
            this.openIO.CheckFileExists = false;
            this.openIO.DefaultExt = "exe";
            this.openIO.Filter = "Text files (*.txt)|*.txt|All files (*.*)|(*.*)";
            // 
            // ExternalProgram
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(346, 240);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.chkSkip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExternalProgram";
            this.ShowIcon = false;
            this.Text = "External Program Processing";
            this.Load += new System.EventHandler(this.ExternalProgram_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExternalProgram_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSkip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.TextBox txtProgram;
        private System.Windows.Forms.RadioButton radFile;
        private System.Windows.Forms.RadioButton radParameter;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnSelectProgram;
        private System.Windows.Forms.OpenFileDialog openProgram;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelectIO;
        private System.Windows.Forms.OpenFileDialog openIO;
    }
}