namespace WikiFunctions.Profiles
{
    partial class AWBProfilesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        //private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            this.chkSaveProfile = new System.Windows.Forms.CheckBox();
            this.btnQuickLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvAccounts
            // 
            this.lvAccounts.Size = new System.Drawing.Size(496, 117);
            // 
            // colNotes
            // 
            this.colNotes.Width = 118;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(93, 137);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(255, 137);
            // 
            // colID
            // 
            this.colID.Width = 26;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(433, 270);
            this.btnClose.TabIndex = 6;
            // 
            // colUsedForUpload
            // 
            this.colUsedForUpload.Width = 98;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(12, 137);
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(174, 137);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkSavePassword);
            this.groupBox1.Controls.Add(this.chkSaveProfile);
            this.groupBox1.Controls.Add(this.btnQuickLogin);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 164);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 92);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quick login";
            // 
            // chkSavePassword
            // 
            this.chkSavePassword.AutoSize = true;
            this.chkSavePassword.Enabled = false;
            this.chkSavePassword.Location = new System.Drawing.Point(177, 65);
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.Size = new System.Drawing.Size(99, 17);
            this.chkSavePassword.TabIndex = 6;
            this.chkSavePassword.Text = "Sa&ve password";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            // 
            // chkSaveProfile
            // 
            this.chkSaveProfile.AutoSize = true;
            this.chkSaveProfile.Location = new System.Drawing.Point(9, 65);
            this.chkSaveProfile.Name = "chkSaveProfile";
            this.chkSaveProfile.Size = new System.Drawing.Size(112, 17);
            this.chkSaveProfile.TabIndex = 5;
            this.chkSaveProfile.Text = "&Save this account";
            this.chkSaveProfile.UseVisualStyleBackColor = true;
            this.chkSaveProfile.CheckedChanged += new System.EventHandler(this.chkSaveProfile_CheckedChanged);
            // 
            // btnQuickLogin
            // 
            this.btnQuickLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuickLogin.Location = new System.Drawing.Point(415, 27);
            this.btnQuickLogin.Name = "btnQuickLogin";
            this.btnQuickLogin.Size = new System.Drawing.Size(75, 23);
            this.btnQuickLogin.TabIndex = 4;
            this.btnQuickLogin.Text = "L&ogin";
            this.btnQuickLogin.UseVisualStyleBackColor = true;
            this.btnQuickLogin.Click += new System.EventHandler(this.btnQuickLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(70, 39);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(339, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.TextChanged += new System.EventHandler(this.UsernameOrPasswordChanged);
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Location = new System.Drawing.Point(70, 13);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(339, 20);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.TextChanged += new System.EventHandler(this.UsernameOrPasswordChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Username:";
            // 
            // AWBProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(520, 305);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(450, 272);
            this.Name = "AWBProfilesForm";
            this.Load += new System.EventHandler(this.AWBProfilesForm_Load);
            this.Controls.SetChildIndex(this.btnDelete, 0);
            this.Controls.SetChildIndex(this.BtnEdit, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnLogin, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.lvAccounts, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSavePassword;
        private System.Windows.Forms.CheckBox chkSaveProfile;
        private System.Windows.Forms.Button btnQuickLogin;
    }
}
