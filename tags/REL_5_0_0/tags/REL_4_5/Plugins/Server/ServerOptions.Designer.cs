namespace AutoWikiBrowser.Plugins.Server
{
    partial class ServerOptions
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
            this.components = new System.ComponentModel.Container();
            this.EnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.AuthenticateCheckBox = new System.Windows.Forms.CheckBox();
            this.EncryptCheckBox = new System.Windows.Forms.CheckBox();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.PortLabel = new System.Windows.Forms.Label();
            this.LoginIDLabel = new System.Windows.Forms.Label();
            this.LoginIDTextBox = new System.Windows.Forms.TextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // EnabledCheckBox
            // 
            this.EnabledCheckBox.AutoSize = true;
            this.EnabledCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EnabledCheckBox.Location = new System.Drawing.Point(39, 9);
            this.EnabledCheckBox.Name = "EnabledCheckBox";
            this.EnabledCheckBox.Size = new System.Drawing.Size(121, 17);
            this.EnabledCheckBox.TabIndex = 0;
            this.EnabledCheckBox.Text = "Accept connections";
            this.EnabledCheckBox.UseVisualStyleBackColor = true;
            this.EnabledCheckBox.CheckedChanged += new System.EventHandler(this.EnabledCheckBox_CheckedChanged);
            // 
            // AuthenticateCheckBox
            // 
            this.AuthenticateCheckBox.AutoSize = true;
            this.AuthenticateCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AuthenticateCheckBox.Checked = true;
            this.AuthenticateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AuthenticateCheckBox.Enabled = false;
            this.AuthenticateCheckBox.Location = new System.Drawing.Point(72, 32);
            this.AuthenticateCheckBox.Name = "AuthenticateCheckBox";
            this.AuthenticateCheckBox.Size = new System.Drawing.Size(88, 17);
            this.AuthenticateCheckBox.TabIndex = 1;
            this.AuthenticateCheckBox.Text = "Require login";
            this.AuthenticateCheckBox.UseVisualStyleBackColor = true;
            this.AuthenticateCheckBox.CheckedChanged += new System.EventHandler(this.AuthenticateCheckBox_CheckedChanged);
            // 
            // EncryptCheckBox
            // 
            this.EncryptCheckBox.AutoSize = true;
            this.EncryptCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EncryptCheckBox.Enabled = false;
            this.EncryptCheckBox.Location = new System.Drawing.Point(84, 55);
            this.EncryptCheckBox.Name = "EncryptCheckBox";
            this.EncryptCheckBox.Size = new System.Drawing.Size(76, 17);
            this.EncryptCheckBox.TabIndex = 2;
            this.EncryptCheckBox.Text = "Encryption";
            this.toolTip1.SetToolTip(this.EncryptCheckBox, "Use RSA public key encryption\r\nto secure your username and\r\npassword.");
            this.EncryptCheckBox.UseVisualStyleBackColor = true;
            // 
            // PortTextBox
            // 
            this.PortTextBox.Enabled = false;
            this.PortTextBox.Location = new System.Drawing.Point(60, 99);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(100, 20);
            this.PortTextBox.TabIndex = 3;
            this.PortTextBox.Text = "49155";
            this.toolTip1.SetToolTip(this.PortTextBox, "The TCP port the server should listen on");
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(57, 83);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(26, 13);
            this.PortLabel.TabIndex = 4;
            this.PortLabel.Text = "Port";
            this.PortLabel.Click += new System.EventHandler(this.PortLabel_Click);
            // 
            // LoginIDLabel
            // 
            this.LoginIDLabel.AutoSize = true;
            this.LoginIDLabel.Location = new System.Drawing.Point(57, 133);
            this.LoginIDLabel.Name = "LoginIDLabel";
            this.LoginIDLabel.Size = new System.Drawing.Size(33, 13);
            this.LoginIDLabel.TabIndex = 6;
            this.LoginIDLabel.Text = "Login";
            // 
            // LoginIDTextBox
            // 
            this.LoginIDTextBox.Enabled = false;
            this.LoginIDTextBox.Location = new System.Drawing.Point(60, 149);
            this.LoginIDTextBox.Name = "LoginIDTextBox";
            this.LoginIDTextBox.Size = new System.Drawing.Size(100, 20);
            this.LoginIDTextBox.TabIndex = 5;
            this.toolTip1.SetToolTip(this.LoginIDTextBox, "The username you want\r\nto log into the server with.\r\nStored encrypted in the\r\nsys" +
                    "tem registry.");
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(57, 182);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.PasswordLabel.TabIndex = 8;
            this.PasswordLabel.Text = "Password";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Enabled = false;
            this.PasswordTextBox.Location = new System.Drawing.Point(60, 198);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(100, 20);
            this.PasswordTextBox.TabIndex = 7;
            this.toolTip1.SetToolTip(this.PasswordTextBox, "The password you want\r\nto log into the server with.\r\nStored encrypted in the\r\nsys" +
                    "tem registry.");
            // 
            // OKBtn
            // 
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(60, 235);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(46, 23);
            this.OKBtn.TabIndex = 9;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(112, 235);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(48, 23);
            this.CancelBtn.TabIndex = 10;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // ServerOptions
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 264);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.LoginIDLabel);
            this.Controls.Add(this.LoginIDTextBox);
            this.Controls.Add(this.PortLabel);
            this.Controls.Add(this.PortTextBox);
            this.Controls.Add(this.EncryptCheckBox);
            this.Controls.Add(this.AuthenticateCheckBox);
            this.Controls.Add(this.EnabledCheckBox);
            this.HelpButton = true;
            this.Name = "ServerOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AWB Server Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox EnabledCheckBox;
        private System.Windows.Forms.CheckBox AuthenticateCheckBox;
        private System.Windows.Forms.CheckBox EncryptCheckBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Label LoginIDLabel;
        private System.Windows.Forms.TextBox LoginIDTextBox;
        private System.Windows.Forms.Label PasswordLabel;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.ToolTip toolTip1;


    }
}