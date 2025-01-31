namespace WikiFunctions.Controls
{
    partial class ArticleActionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArticleActionDialog));
            this.txtNewTitle = new System.Windows.Forms.TextBox();
            this.lblNewTitle = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmboSummary = new System.Windows.Forms.ComboBox();
            this.lblExpiry = new System.Windows.Forms.Label();
            this.txtExpiry = new System.Windows.Forms.TextBox();
            this.chkAutoProtect = new System.Windows.Forms.CheckBox();
            this.chkCascadingProtection = new System.Windows.Forms.CheckBox();
            this.chkNoRedirect = new System.Windows.Forms.CheckBox();
            this.toolTip = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.chkWatch = new System.Windows.Forms.CheckBox();
            this.MoveDelete = new WikiFunctions.Controls.EditProtectControl();
            this.chkDealWithAssoc = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtNewTitle
            // 
            resources.ApplyResources(this.txtNewTitle, "txtNewTitle");
            this.txtNewTitle.Name = "txtNewTitle";
            // 
            // lblNewTitle
            // 
            resources.ApplyResources(this.lblNewTitle, "lblNewTitle");
            this.lblNewTitle.Name = "lblNewTitle";
            // 
            // lblSummary
            // 
            resources.ApplyResources(this.lblSummary, "lblSummary");
            this.lblSummary.Name = "lblSummary";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmboSummary
            // 
            this.cmboSummary.FormattingEnabled = true;
            resources.ApplyResources(this.cmboSummary, "cmboSummary");
            this.cmboSummary.Name = "cmboSummary";
            // 
            // lblExpiry
            // 
            resources.ApplyResources(this.lblExpiry, "lblExpiry");
            this.lblExpiry.Name = "lblExpiry";
            // 
            // txtExpiry
            // 
            resources.ApplyResources(this.txtExpiry, "txtExpiry");
            this.txtExpiry.Name = "txtExpiry";
            // 
            // chkAutoProtect
            // 
            resources.ApplyResources(this.chkAutoProtect, "chkAutoProtect");
            this.chkAutoProtect.Name = "chkAutoProtect";
            this.chkAutoProtect.UseVisualStyleBackColor = true;
            // 
            // chkCascadingProtection
            // 
            resources.ApplyResources(this.chkCascadingProtection, "chkCascadingProtection");
            this.chkCascadingProtection.Name = "chkCascadingProtection";
            this.chkCascadingProtection.UseVisualStyleBackColor = true;
            // 
            // chkNoRedirect
            // 
            resources.ApplyResources(this.chkNoRedirect, "chkNoRedirect");
            this.chkNoRedirect.Name = "chkNoRedirect";
            this.chkNoRedirect.UseVisualStyleBackColor = true;
            // 
            // chkWatch
            // 
            resources.ApplyResources(this.chkWatch, "chkWatch");
            this.chkWatch.Name = "chkWatch";
            this.chkWatch.UseVisualStyleBackColor = true;
            // 
            // MoveDelete
            // 
            this.MoveDelete.EditProtectionLevel = "";
            resources.ApplyResources(this.MoveDelete, "MoveDelete");
            this.MoveDelete.MoveProtectionLevel = "";
            this.MoveDelete.Name = "MoveDelete";
            this.MoveDelete.TextBoxIndexChanged += new System.EventHandler(this.MoveDelete_TextBoxIndexChanged);
            // 
            // chkDealWithAssoc
            // 
            resources.ApplyResources(this.chkDealWithAssoc, "chkDealWithAssoc");
            this.chkDealWithAssoc.Name = "chkDealWithAssoc";
            this.chkDealWithAssoc.UseVisualStyleBackColor = true;
            // 
            // ArticleActionDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.chkDealWithAssoc);
            this.Controls.Add(this.chkWatch);
            this.Controls.Add(this.chkNoRedirect);
            this.Controls.Add(this.chkAutoProtect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chkCascadingProtection);
            this.Controls.Add(this.txtExpiry);
            this.Controls.Add(this.lblExpiry);
            this.Controls.Add(this.cmboSummary);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.txtNewTitle);
            this.Controls.Add(this.lblNewTitle);
            this.Controls.Add(this.MoveDelete);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ArticleActionDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArticleActionDialog_FormClosing);
            this.Load += new System.EventHandler(this.ArticleActionDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNewTitle;
        private System.Windows.Forms.Label lblNewTitle;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmboSummary;
        private System.Windows.Forms.Label lblExpiry;
        private System.Windows.Forms.TextBox txtExpiry;
        private System.Windows.Forms.CheckBox chkAutoProtect;
        private System.Windows.Forms.CheckBox chkCascadingProtection;
        private EditProtectControl MoveDelete;
        private System.Windows.Forms.CheckBox chkNoRedirect;
        private WikiFunctions.Controls.AWBToolTip toolTip;
        private System.Windows.Forms.CheckBox chkWatch;
        private System.Windows.Forms.CheckBox chkDealWithAssoc;
    }
}
