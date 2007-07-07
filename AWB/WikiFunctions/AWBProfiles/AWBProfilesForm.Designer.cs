namespace WikiFunctions.AWBProfiles
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
            // colID
            // 
            this.colID.Width = 27;
            // 
            // colUsedForUpload
            // 
            this.colUsedForUpload.Width = 95;
            // 
            // btnLogin
            // 
            this.btnLogin.Visible = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // AWBProfilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(518, 248);
            this.Name = "AWBProfilesForm";
            this.Controls.SetChildIndex(this.btnDelete, 0);
            this.Controls.SetChildIndex(this.btnExit, 0);
            this.Controls.SetChildIndex(this.btnAdd, 0);
            this.Controls.SetChildIndex(this.lvAccounts, 0);
            this.Controls.SetChildIndex(this.btnLogin, 0);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
