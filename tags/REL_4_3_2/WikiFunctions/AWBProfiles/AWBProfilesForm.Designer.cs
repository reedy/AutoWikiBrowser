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
            this.SuspendLayout();
            // 
            // lvAccounts
            // 
            this.lvAccounts.Size = new System.Drawing.Size(494, 170);
            this.lvAccounts.DoubleClick += new System.EventHandler(this.lvAccounts_DoubleClick);
            // 
            // colAccountName
            // 
            this.colAccountName.Width = 60;
            // 
            // colPasswordSaved
            // 
            this.colPasswordSaved.Width = 96;
            // 
            // colProfileSettings
            // 
            this.colProfileSettings.Width = 161;
            // 
            // colNotes
            // 
            this.colNotes.Width = 51;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(93, 188);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(255, 188);
            // 
            // colID
            // 
            this.colID.Width = 27;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(431, 188);
            // 
            // colUsedForUpload
            // 
            this.colUsedForUpload.Width = 95;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(12, 188);
            this.btnLogin.Visible = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // BtnEdit
            // 
            this.BtnEdit.Location = new System.Drawing.Point(174, 188);
            // 
            // AWBProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(518, 223);
            this.Name = "AWBProfilesForm";
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.btnDelete, 0);
            this.Controls.SetChildIndex(this.BtnEdit, 0);
            this.Controls.SetChildIndex(this.btnLogin, 0);
            this.Controls.SetChildIndex(this.lvAccounts, 0);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
