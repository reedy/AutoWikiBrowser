namespace WikiFunctions.Profiles
{
    partial class AWBProfilesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AWBProfilesForm));
            this.mnuAccounts = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loginAsThisAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editThisAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addNewAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteThisAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.BtnEdit = new System.Windows.Forms.Button();
            this.lvAccounts = new WikiFunctions.Controls.NoFlickerExtendedListView();
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAccountName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPasswordSaved = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProfileSettings = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNotes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            this.chkSaveProfile = new System.Windows.Forms.CheckBox();
            this.btnQuickLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mnuAccounts.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuAccounts
            // 
            this.mnuAccounts.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.mnuAccounts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginAsThisAccountToolStripMenuItem,
            this.editThisAccountToolStripMenuItem,
            this.changePasswordToolStripMenuItem,
            this.toolStripSeparator1,
            this.addNewAccountToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteThisAccountToolStripMenuItem});
            this.mnuAccounts.Name = "mnuAccounts";
            resources.ApplyResources(this.mnuAccounts, "mnuAccounts");
            // 
            // loginAsThisAccountToolStripMenuItem
            // 
            this.loginAsThisAccountToolStripMenuItem.Name = "loginAsThisAccountToolStripMenuItem";
            resources.ApplyResources(this.loginAsThisAccountToolStripMenuItem, "loginAsThisAccountToolStripMenuItem");
            // 
            // editThisAccountToolStripMenuItem
            // 
            this.editThisAccountToolStripMenuItem.Name = "editThisAccountToolStripMenuItem";
            resources.ApplyResources(this.editThisAccountToolStripMenuItem, "editThisAccountToolStripMenuItem");
            this.editThisAccountToolStripMenuItem.Click += new System.EventHandler(this.editThisAccountToolStripMenuItem_Click);
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            resources.ApplyResources(this.changePasswordToolStripMenuItem, "changePasswordToolStripMenuItem");
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // addNewAccountToolStripMenuItem
            // 
            this.addNewAccountToolStripMenuItem.Name = "addNewAccountToolStripMenuItem";
            resources.ApplyResources(this.addNewAccountToolStripMenuItem, "addNewAccountToolStripMenuItem");
            this.addNewAccountToolStripMenuItem.Click += new System.EventHandler(this.addNewAccountToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // deleteThisAccountToolStripMenuItem
            // 
            this.deleteThisAccountToolStripMenuItem.Name = "deleteThisAccountToolStripMenuItem";
            resources.ApplyResources(this.deleteThisAccountToolStripMenuItem, "deleteThisAccountToolStripMenuItem");
            this.deleteThisAccountToolStripMenuItem.Click += new System.EventHandler(this.deleteThisSavedAccountToolStripMenuItem_Click);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // BtnEdit
            // 
            resources.ApplyResources(this.BtnEdit, "BtnEdit");
            this.BtnEdit.Name = "BtnEdit";
            this.BtnEdit.UseVisualStyleBackColor = true;
            this.BtnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // lvAccounts
            // 
            resources.ApplyResources(this.lvAccounts, "lvAccounts");
            this.lvAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colAccountName,
            this.colPasswordSaved,
            this.colProfileSettings,
            this.colNotes});
            this.lvAccounts.ComparerFactory = this.lvAccounts;
            this.lvAccounts.ContextMenuStrip = this.mnuAccounts;
            this.lvAccounts.FullRowSelect = true;
            this.lvAccounts.HideSelection = false;
            this.lvAccounts.Name = "lvAccounts";
            this.lvAccounts.ResizeColumnsOnControlResize = true;
            this.lvAccounts.SortColumnsOnClick = true;
            this.lvAccounts.UseCompatibleStateImageBehavior = false;
            this.lvAccounts.View = System.Windows.Forms.View.Details;
            this.lvAccounts.SelectedIndexChanged += new System.EventHandler(this.lvAccounts_SelectedIndexChanged);
            this.lvAccounts.DoubleClick += new System.EventHandler(this.lvAccounts_DoubleClick);
            // 
            // colID
            // 
            resources.ApplyResources(this.colID, "colID");
            // 
            // colAccountName
            // 
            resources.ApplyResources(this.colAccountName, "colAccountName");
            // 
            // colPasswordSaved
            // 
            resources.ApplyResources(this.colPasswordSaved, "colPasswordSaved");
            // 
            // colProfileSettings
            // 
            resources.ApplyResources(this.colProfileSettings, "colProfileSettings");
            // 
            // colNotes
            // 
            resources.ApplyResources(this.colNotes, "colNotes");
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkSavePassword);
            this.groupBox1.Controls.Add(this.chkSaveProfile);
            this.groupBox1.Controls.Add(this.btnQuickLogin);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chkSavePassword
            // 
            resources.ApplyResources(this.chkSavePassword, "chkSavePassword");
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            // 
            // chkSaveProfile
            // 
            resources.ApplyResources(this.chkSaveProfile, "chkSaveProfile");
            this.chkSaveProfile.Name = "chkSaveProfile";
            this.chkSaveProfile.UseVisualStyleBackColor = true;
            this.chkSaveProfile.CheckedChanged += new System.EventHandler(this.chkSaveProfile_CheckedChanged);
            // 
            // btnQuickLogin
            // 
            resources.ApplyResources(this.btnQuickLogin, "btnQuickLogin");
            this.btnQuickLogin.Name = "btnQuickLogin";
            this.btnQuickLogin.UseVisualStyleBackColor = true;
            this.btnQuickLogin.Click += new System.EventHandler(this.btnQuickLogin_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.TextChanged += new System.EventHandler(this.UsernameOrPasswordChanged);
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            // 
            // txtUsername
            // 
            resources.ApplyResources(this.txtUsername, "txtUsername");
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.TextChanged += new System.EventHandler(this.UsernameOrPasswordChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // AWBProfilesForm
            // 
            this.AcceptButton = this.btnLogin;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BtnEdit);
            this.Controls.Add(this.lvAccounts);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnLogin);
            this.Name = "AWBProfilesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.AWBProfiles_Load);
            this.mnuAccounts.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected WikiFunctions.Controls.NoFlickerExtendedListView lvAccounts;
        protected System.Windows.Forms.ColumnHeader colAccountName;
        protected System.Windows.Forms.ColumnHeader colPasswordSaved;
        protected System.Windows.Forms.ColumnHeader colProfileSettings;
        protected System.Windows.Forms.ColumnHeader colNotes;
        protected System.Windows.Forms.ContextMenuStrip mnuAccounts;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        protected System.Windows.Forms.ToolStripMenuItem addNewAccountToolStripMenuItem;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        protected System.Windows.Forms.ToolStripMenuItem deleteThisAccountToolStripMenuItem;
        protected System.Windows.Forms.Button btnAdd;
        protected System.Windows.Forms.Button btnDelete;
        protected System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        protected System.Windows.Forms.ColumnHeader colID;
        protected System.Windows.Forms.ToolStripMenuItem editThisAccountToolStripMenuItem;
        protected System.Windows.Forms.Button btnClose;
        protected System.Windows.Forms.ToolStripMenuItem loginAsThisAccountToolStripMenuItem;
        protected System.Windows.Forms.Button btnLogin;
        protected System.Windows.Forms.Button BtnEdit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSavePassword;
        private System.Windows.Forms.CheckBox chkSaveProfile;
        private System.Windows.Forms.Button btnQuickLogin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label1;

    }
}