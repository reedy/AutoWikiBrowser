namespace WikiFunctions.Profiles
{
    partial class AWBProfileAdd
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
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkDefaultSettings = new System.Windows.Forms.CheckBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.openDefaultFile = new System.Windows.Forms.OpenFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkUseForUpload = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(76, 21);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(146, 20);
            this.txtUsername.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(74, 69);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(148, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // chkSavePassword
            // 
            this.chkSavePassword.AutoSize = true;
            this.chkSavePassword.Location = new System.Drawing.Point(15, 47);
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.Size = new System.Drawing.Size(106, 17);
            this.chkSavePassword.TabIndex = 1;
            this.chkSavePassword.Text = "Save Password?";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            this.chkSavePassword.CheckedChanged += new System.EventHandler(this.chkSavePassword_CheckedChanged);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(74, 180);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(148, 20);
            this.txtNotes.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 183);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Notes:";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Location = new System.Drawing.Point(28, 215);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(78, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Add";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkDefaultSettings
            // 
            this.chkDefaultSettings.AutoSize = true;
            this.chkDefaultSettings.Location = new System.Drawing.Point(15, 99);
            this.chkDefaultSettings.Name = "chkDefaultSettings";
            this.chkDefaultSettings.Size = new System.Drawing.Size(136, 17);
            this.chkDefaultSettings.TabIndex = 3;
            this.chkDefaultSettings.Text = "Select default settings?";
            this.chkDefaultSettings.UseVisualStyleBackColor = true;
            this.chkDefaultSettings.CheckedChanged += new System.EventHandler(this.chkDefaultSettings_CheckedChanged);
            // 
            // txtPath
            // 
            this.txtPath.Enabled = false;
            this.txtPath.Location = new System.Drawing.Point(74, 122);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(148, 20);
            this.txtPath.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Path:";
            // 
            // openDefaultFile
            // 
            this.openDefaultFile.DefaultExt = "xml";
            this.openDefaultFile.Filter = "XML files|*.xml";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(129, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkUseForUpload
            // 
            this.chkUseForUpload.AutoSize = true;
            this.chkUseForUpload.Location = new System.Drawing.Point(15, 152);
            this.chkUseForUpload.Name = "chkUseForUpload";
            this.chkUseForUpload.Size = new System.Drawing.Size(182, 17);
            this.chkUseForUpload.TabIndex = 12;
            this.chkUseForUpload.Text = "Use this profile for log uploading?";
            this.chkUseForUpload.UseVisualStyleBackColor = true;
            // 
            // AWBProfileAdd
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(234, 250);
            this.Controls.Add(this.chkUseForUpload);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.chkDefaultSettings);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.chkSavePassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUsername);
            this.Name = "AWBProfileAdd";
            this.ShowInTaskbar = false;
            this.Text = "Add new profile";
            this.Load += new System.EventHandler(this.AWBProfileAdd_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkSavePassword;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkDefaultSettings;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog openDefaultFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkUseForUpload;
    }
}