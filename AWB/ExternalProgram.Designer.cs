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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalProgram));
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
            resources.ApplyResources(this.chkEnabled, "chkEnabled");
            this.chkEnabled.Name = "chkEnabled";
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnSelectIO
            // 
            resources.ApplyResources(this.btnSelectIO, "btnSelectIO");
            this.btnSelectIO.Name = "btnSelectIO";
            this.btnSelectIO.UseVisualStyleBackColor = true;
            this.btnSelectIO.Click += new System.EventHandler(this.btnSelectIO_Click);
            // 
            // txtFile
            // 
            resources.ApplyResources(this.txtFile, "txtFile");
            this.txtFile.Name = "txtFile";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnSelectProgram
            // 
            resources.ApplyResources(this.btnSelectProgram, "btnSelectProgram");
            this.btnSelectProgram.Name = "btnSelectProgram";
            this.btnSelectProgram.UseVisualStyleBackColor = true;
            this.btnSelectProgram.Click += new System.EventHandler(this.btnSelectProgram_Click);
            // 
            // radFile
            // 
            resources.ApplyResources(this.radFile, "radFile");
            this.radFile.Checked = true;
            this.radFile.Name = "radFile";
            this.radFile.TabStop = true;
            this.radFile.UseVisualStyleBackColor = true;
            this.radFile.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // radParameter
            // 
            resources.ApplyResources(this.radParameter, "radParameter");
            this.radParameter.Name = "radParameter";
            this.radParameter.UseVisualStyleBackColor = true;
            this.radParameter.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtParameters
            // 
            resources.ApplyResources(this.txtParameters, "txtParameters");
            this.txtParameters.Name = "txtParameters";
            // 
            // txtProgram
            // 
            resources.ApplyResources(this.txtProgram, "txtProgram");
            this.txtProgram.Name = "txtProgram";
            // 
            // chkSkip
            // 
            resources.ApplyResources(this.chkSkip, "chkSkip");
            this.chkSkip.Name = "chkSkip";
            this.chkSkip.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // openProgram
            // 
            this.openProgram.DefaultExt = "exe";
            resources.ApplyResources(this.openProgram, "openProgram");
            // 
            // openIO
            // 
            this.openIO.CheckFileExists = false;
            this.openIO.DefaultExt = "exe";
            resources.ApplyResources(this.openIO, "openIO");
            // 
            // ExternalProgram
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOk;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.chkSkip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExternalProgram";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExternalProgram_FormClosing);
            this.Load += new System.EventHandler(this.ExternalProgram_Load);
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