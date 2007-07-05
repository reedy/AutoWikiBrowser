namespace WikiFunctions.AWBProfiles
{
    partial class AWBLogUploadProfilesForm
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
            this.lvAccounts = new System.Windows.Forms.ListView();
            this.colID = new System.Windows.Forms.ColumnHeader();
            this.colAccountName = new System.Windows.Forms.ColumnHeader();
            this.colPasswordSaved = new System.Windows.Forms.ColumnHeader();
            this.colProfileSettings = new System.Windows.Forms.ColumnHeader();
            this.colUsedForUpload = new System.Windows.Forms.ColumnHeader();
            this.colNotes = new System.Windows.Forms.ColumnHeader();
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.mnuAccounts.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvAccounts
            // 
            this.lvAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colAccountName,
            this.colPasswordSaved,
            this.colProfileSettings,
            this.colUsedForUpload,
            this.colNotes});
            this.lvAccounts.ContextMenuStrip = this.mnuAccounts;
            this.lvAccounts.FullRowSelect = true;
            this.lvAccounts.Location = new System.Drawing.Point(12, 12);
            this.lvAccounts.Name = "lvAccounts";
            this.lvAccounts.Size = new System.Drawing.Size(494, 179);
            this.lvAccounts.TabIndex = 0;
            this.lvAccounts.UseCompatibleStateImageBehavior = false;
            this.lvAccounts.View = System.Windows.Forms.View.Details;
            this.lvAccounts.DoubleClick += new System.EventHandler(this.lvAccounts_DoubleClick);
            this.lvAccounts.SelectedIndexChanged += new System.EventHandler(this.lvAccounts_SelectedIndexChanged);
            // 
            // colID
            // 
            this.colID.Text = "ID";
            this.colID.Width = 32;
            // 
            // colAccountName
            // 
            this.colAccountName.Text = "Username";
            this.colAccountName.Width = 89;
            // 
            // colPasswordSaved
            // 
            this.colPasswordSaved.Text = "Password saved?";
            this.colPasswordSaved.Width = 107;
            // 
            // colProfileSettings
            // 
            this.colProfileSettings.Text = "Profile default settings";
            this.colProfileSettings.Width = 125;
            // 
            // colUsedForUpload
            // 
            this.colUsedForUpload.Text = "Used for Upload?";
            this.colUsedForUpload.Width = 98;
            // 
            // colNotes
            // 
            this.colNotes.Text = "Notes";
            this.colNotes.Width = 63;
            // 
            // mnuAccounts
            // 
            this.mnuAccounts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginAsThisAccountToolStripMenuItem,
            this.editThisAccountToolStripMenuItem,
            this.changePasswordToolStripMenuItem,
            this.toolStripSeparator1,
            this.addNewAccountToolStripMenuItem,
            this.toolStripSeparator2,
            this.deleteThisAccountToolStripMenuItem});
            this.mnuAccounts.Name = "mnuAccounts";
            this.mnuAccounts.Size = new System.Drawing.Size(192, 126);
            // 
            // loginAsThisAccountToolStripMenuItem
            // 
            this.loginAsThisAccountToolStripMenuItem.Name = "loginAsThisAccountToolStripMenuItem";
            this.loginAsThisAccountToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loginAsThisAccountToolStripMenuItem.Text = "Log-in as this account";
            this.loginAsThisAccountToolStripMenuItem.Visible = false;
            // 
            // editThisAccountToolStripMenuItem
            // 
            this.editThisAccountToolStripMenuItem.Name = "editThisAccountToolStripMenuItem";
            this.editThisAccountToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.editThisAccountToolStripMenuItem.Text = "Edit this account";
            this.editThisAccountToolStripMenuItem.Click += new System.EventHandler(this.editThisAccountToolStripMenuItem_Click);
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.changePasswordToolStripMenuItem.Text = "Change password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(188, 6);
            // 
            // addNewAccountToolStripMenuItem
            // 
            this.addNewAccountToolStripMenuItem.Name = "addNewAccountToolStripMenuItem";
            this.addNewAccountToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.addNewAccountToolStripMenuItem.Text = "Add new account";
            this.addNewAccountToolStripMenuItem.Click += new System.EventHandler(this.addNewAccountToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(188, 6);
            // 
            // deleteThisAccountToolStripMenuItem
            // 
            this.deleteThisAccountToolStripMenuItem.Name = "deleteThisAccountToolStripMenuItem";
            this.deleteThisAccountToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.deleteThisAccountToolStripMenuItem.Text = "Delete this account";
            this.deleteThisAccountToolStripMenuItem.Click += new System.EventHandler(this.deleteThisSavedAccountToolStripMenuItem_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(109, 200);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(91, 36);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add new account";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(206, 200);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(91, 36);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(415, 200);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(91, 36);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Enabled = false;
            this.btnLogin.Location = new System.Drawing.Point(12, 200);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(91, 36);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Login as selected";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Visible = false;
            // 
            // AWBLogUploadProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(518, 248);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lvAccounts);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnDelete);
            this.Name = "AWBLogUploadProfilesForm";
            this.ShowInTaskbar = false;
            this.Text = "Profiles";
            this.Load += new System.EventHandler(this.AWBProfiles_Load);
            this.mnuAccounts.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.ListView lvAccounts;
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
        protected System.Windows.Forms.Button btnExit;
        protected System.Windows.Forms.ColumnHeader colUsedForUpload;
        protected System.Windows.Forms.ToolStripMenuItem loginAsThisAccountToolStripMenuItem;
        protected System.Windows.Forms.Button btnLogin;

    }
}