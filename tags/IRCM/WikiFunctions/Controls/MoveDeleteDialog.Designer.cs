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
            this.lblNewTitle.Size = new System.Drawing.Size(48, 13);
            this.lblNewTitle.TabIndex = 0;
            this.lblNewTitle.Text = "New title";
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
            this.btnOk.Location = new System.Drawing.Point(137, 171);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(218, 171);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
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
            this.lblExpiry.Size = new System.Drawing.Size(35, 13);
            this.lblExpiry.TabIndex = 2;
            this.lblExpiry.Text = "Expiry";
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
            this.chkAutoProtect.Location = new System.Drawing.Point(69, 128);
            this.chkAutoProtect.Name = "chkAutoProtect";
            this.chkAutoProtect.Size = new System.Drawing.Size(99, 17);
            this.chkAutoProtect.TabIndex = 8;
            this.chkAutoProtect.Text = "Auto Protect All";
            this.chkAutoProtect.UseVisualStyleBackColor = true;
            this.chkAutoProtect.Visible = false;
            // 
            // chkCascadingProtection
            // 
            this.chkCascadingProtection.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.chkCascadingProtection.AutoSize = true;
            this.chkCascadingProtection.Enabled = false;
            this.chkCascadingProtection.Location = new System.Drawing.Point(12, 151);
            this.chkCascadingProtection.Name = "chkCascadingProtection";
            this.chkCascadingProtection.Size = new System.Drawing.Size(399, 17);
            this.chkCascadingProtection.TabIndex = 12;
            this.chkCascadingProtection.Text = "Cascading protection (automatically protect any pages transcluded in this page)";
            this.chkCascadingProtection.UseVisualStyleBackColor = true;
            // 
            // MoveDelete
            // 
            this.MoveDelete.Location = new System.Drawing.Point(62, 64);
            this.MoveDelete.Name = "MoveDelete";
            this.MoveDelete.Size = new System.Drawing.Size(313, 84);
            this.MoveDelete.TabIndex = 15;
            // 
            // ArticleActionDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(423, 208);
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
    }
}
