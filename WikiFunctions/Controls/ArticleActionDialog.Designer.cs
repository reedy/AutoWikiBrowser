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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.chkWatch = new System.Windows.Forms.CheckBox();
            this.MoveDelete = new WikiFunctions.Controls.EditProtectControl();
            this.SuspendLayout();
            // 
            // txtNewTitle
            // 
            this.txtNewTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNewTitle.Location = new System.Drawing.Point(62, 12);
            this.txtNewTitle.Name = "txtNewTitle";
            this.txtNewTitle.Size = new System.Drawing.Size(349, 20);
            this.txtNewTitle.TabIndex = 1;
            // 
            // lblNewTitle
            // 
            this.lblNewTitle.AutoSize = true;
            this.lblNewTitle.Location = new System.Drawing.Point(8, 15);
            this.lblNewTitle.Name = "lblNewTitle";
            this.lblNewTitle.Size = new System.Drawing.Size(51, 13);
            this.lblNewTitle.TabIndex = 0;
            this.lblNewTitle.Text = "&New title:";
            // 
            // lblSummary
            // 
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(9, 41);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(50, 13);
            this.lblSummary.TabIndex = 4;
            this.lblSummary.Text = "Summary";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(137, 182);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(218, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmboSummary
            // 
            this.cmboSummary.FormattingEnabled = true;
            this.cmboSummary.Location = new System.Drawing.Point(62, 38);
            this.cmboSummary.Name = "cmboSummary";
            this.cmboSummary.Size = new System.Drawing.Size(349, 21);
            this.cmboSummary.TabIndex = 5;
            // 
            // lblExpiry
            // 
            this.lblExpiry.AutoSize = true;
            this.lblExpiry.Location = new System.Drawing.Point(8, 41);
            this.lblExpiry.Name = "lblExpiry";
            this.lblExpiry.Size = new System.Drawing.Size(38, 13);
            this.lblExpiry.TabIndex = 2;
            this.lblExpiry.Text = "&Expiry:";
            // 
            // txtExpiry
            // 
            this.txtExpiry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExpiry.Location = new System.Drawing.Point(62, 38);
            this.txtExpiry.Name = "txtExpiry";
            this.txtExpiry.Size = new System.Drawing.Size(349, 20);
            this.txtExpiry.TabIndex = 3;
            // 
            // chkAutoProtect
            // 
            this.chkAutoProtect.AutoSize = true;
            this.chkAutoProtect.Location = new System.Drawing.Point(148, 162);
            this.chkAutoProtect.Name = "chkAutoProtect";
            this.chkAutoProtect.Size = new System.Drawing.Size(99, 17);
            this.chkAutoProtect.TabIndex = 8;
            this.chkAutoProtect.Text = "Auto &Protect All";
            this.chkAutoProtect.UseVisualStyleBackColor = true;
            this.chkAutoProtect.Visible = false;
            // 
            // chkCascadingProtection
            // 
            this.chkCascadingProtection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.chkCascadingProtection.AutoSize = true;
            this.chkCascadingProtection.Enabled = false;
            this.chkCascadingProtection.Location = new System.Drawing.Point(23, 162);
            this.chkCascadingProtection.Name = "chkCascadingProtection";
            this.chkCascadingProtection.Size = new System.Drawing.Size(126, 17);
            this.chkCascadingProtection.TabIndex = 7;
            this.chkCascadingProtection.Text = "&Cascading protection";
            this.chkCascadingProtection.UseVisualStyleBackColor = true;
            // 
            // chkNoRedirect
            // 
            this.chkNoRedirect.AutoSize = true;
            this.chkNoRedirect.Location = new System.Drawing.Point(253, 162);
            this.chkNoRedirect.Name = "chkNoRedirect";
            this.chkNoRedirect.Size = new System.Drawing.Size(83, 17);
            this.chkNoRedirect.TabIndex = 9;
            this.chkNoRedirect.Text = "&No Redirect";
            this.chkNoRedirect.UseVisualStyleBackColor = true;
            this.chkNoRedirect.Visible = false;
            // 
            // chkWatch
            // 
            this.chkWatch.AutoSize = true;
            this.chkWatch.Location = new System.Drawing.Point(342, 162);
            this.chkWatch.Name = "chkWatch";
            this.chkWatch.Size = new System.Drawing.Size(58, 17);
            this.chkWatch.TabIndex = 10;
            this.chkWatch.Text = "&Watch";
            this.chkWatch.UseVisualStyleBackColor = true;
            this.chkWatch.Visible = false;
            // 
            // MoveDelete
            // 
            this.MoveDelete.EditProtectionLevel = "";
            this.MoveDelete.Location = new System.Drawing.Point(62, 64);
            this.MoveDelete.MoveProtectionLevel = "";
            this.MoveDelete.Name = "MoveDelete";
            this.MoveDelete.Size = new System.Drawing.Size(313, 101);
            this.MoveDelete.TabIndex = 6;
            this.MoveDelete.TextBoxIndexChanged += new System.EventHandler(this.MoveDelete_TextBoxIndexChanged);
            // 
            // ArticleActionDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(423, 219);
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
            this.MaximumSize = new System.Drawing.Size(429, 243);
            this.MinimumSize = new System.Drawing.Size(429, 100);
            this.Name = "ArticleActionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter details";
            this.Load += new System.EventHandler(this.ArticleActionDialog_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArticleActionDialog_FormClosing);
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
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox chkWatch;
    }
}
