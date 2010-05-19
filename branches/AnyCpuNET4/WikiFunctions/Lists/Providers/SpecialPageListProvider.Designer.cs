namespace WikiFunctions.Lists.Providers
{
    partial class SpecialPageListProvider
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cboNamespace = new System.Windows.Forms.ComboBox();
            this.lblNamespace = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtPages = new System.Windows.Forms.TextBox();
            this.cmboSourceSelect = new System.Windows.Forms.ComboBox();
            this.lblPages = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(108, 94);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(189, 94);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cboNamespace
            // 
            this.cboNamespace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNamespace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNamespace.FormattingEnabled = true;
            this.cboNamespace.Location = new System.Drawing.Point(98, 65);
            this.cboNamespace.Name = "cboNamespace";
            this.cboNamespace.Size = new System.Drawing.Size(166, 21);
            this.cboNamespace.TabIndex = 9;
            // 
            // lblNamespace
            // 
            this.lblNamespace.AutoSize = true;
            this.lblNamespace.Location = new System.Drawing.Point(12, 68);
            this.lblNamespace.Name = "lblNamespace";
            this.lblNamespace.Size = new System.Drawing.Size(67, 13);
            this.lblNamespace.TabIndex = 10;
            this.lblNamespace.Text = "&Namespace:";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(12, 15);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(44, 13);
            this.lblSource.TabIndex = 11;
            this.lblSource.Text = "&Source:";
            // 
            // txtPages
            // 
            this.txtPages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPages.Location = new System.Drawing.Point(98, 39);
            this.txtPages.Name = "txtPages";
            this.txtPages.Size = new System.Drawing.Size(166, 20);
            this.txtPages.TabIndex = 12;
            // 
            // cmboSourceSelect
            // 
            this.cmboSourceSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmboSourceSelect.DropDownHeight = 250;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.IntegralHeight = false;
            this.cmboSourceSelect.Location = new System.Drawing.Point(98, 12);
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Size = new System.Drawing.Size(166, 21);
            this.cmboSourceSelect.Sorted = true;
            this.cmboSourceSelect.TabIndex = 14;
            this.cmboSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmboSourceSelect_SelectedIndexChanged);
            // 
            // lblPages
            // 
            this.lblPages.AutoSize = true;
            this.lblPages.Location = new System.Drawing.Point(12, 42);
            this.lblPages.Name = "lblPages";
            this.lblPages.Size = new System.Drawing.Size(40, 13);
            this.lblPages.TabIndex = 15;
            this.lblPages.Text = "&Pages:";
            // 
            // SpecialPageListProvider
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(276, 129);
            this.Controls.Add(this.lblPages);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboNamespace);
            this.Controls.Add(this.lblNamespace);
            this.Controls.Add(this.txtPages);
            this.Controls.Add(this.lblSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SpecialPageListProvider";
            this.Text = "Special Pages";
            this.Load += new System.EventHandler(this.SpecialPageListProvider_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cboNamespace;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtPages;
        public System.Windows.Forms.ComboBox cmboSourceSelect;
        private System.Windows.Forms.Label lblPages;
    }
}


