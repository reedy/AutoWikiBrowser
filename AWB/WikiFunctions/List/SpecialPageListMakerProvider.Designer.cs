namespace WikiFunctions.Lists
{
    partial class SpecialPageListMakerProvider
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.radAllPages = new System.Windows.Forms.RadioButton();
            this.radPrefixIndex = new System.Windows.Forms.RadioButton();
            this.lblRecent = new System.Windows.Forms.Label();
            this.cboNamespace = new System.Windows.Forms.ComboBox();
            this.lblNamespace = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.flwPages = new System.Windows.Forms.FlowLayoutPanel();
            this.gbPages = new System.Windows.Forms.GroupBox();
            this.flwPages.SuspendLayout();
            this.gbPages.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(176, 297);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(257, 297);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(130, 16);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Maintenance reports";
            // 
            // radAllPages
            // 
            this.radAllPages.AutoSize = true;
            this.radAllPages.Checked = true;
            this.radAllPages.Location = new System.Drawing.Point(3, 3);
            this.radAllPages.Name = "radAllPages";
            this.radAllPages.Size = new System.Drawing.Size(68, 17);
            this.radAllPages.TabIndex = 6;
            this.radAllPages.TabStop = true;
            this.radAllPages.Text = "All pages";
            this.radAllPages.UseVisualStyleBackColor = true;
            // 
            // radPrefixIndex
            // 
            this.radPrefixIndex.AutoSize = true;
            this.radPrefixIndex.Location = new System.Drawing.Point(3, 26);
            this.radPrefixIndex.Name = "radPrefixIndex";
            this.radPrefixIndex.Size = new System.Drawing.Size(178, 17);
            this.radPrefixIndex.TabIndex = 7;
            this.radPrefixIndex.Text = "All pages with prefix (Prefixindex)";
            this.radPrefixIndex.UseVisualStyleBackColor = true;
            // 
            // lblRecent
            // 
            this.lblRecent.AutoSize = true;
            this.lblRecent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecent.Location = new System.Drawing.Point(12, 150);
            this.lblRecent.Name = "lblRecent";
            this.lblRecent.Size = new System.Drawing.Size(161, 16);
            this.lblRecent.TabIndex = 8;
            this.lblRecent.Text = "Recent changes and logs";
            // 
            // cboNamespace
            // 
            this.cboNamespace.FormattingEnabled = true;
            this.cboNamespace.Location = new System.Drawing.Point(82, 236);
            this.cboNamespace.Name = "cboNamespace";
            this.cboNamespace.Size = new System.Drawing.Size(135, 21);
            this.cboNamespace.TabIndex = 9;
            // 
            // lblNamespace
            // 
            this.lblNamespace.AutoSize = true;
            this.lblNamespace.Location = new System.Drawing.Point(12, 239);
            this.lblNamespace.Name = "lblNamespace";
            this.lblNamespace.Size = new System.Drawing.Size(64, 13);
            this.lblNamespace.TabIndex = 10;
            this.lblNamespace.Text = "Namespace";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(31, 214);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(40, 13);
            this.lblSource.TabIndex = 11;
            this.lblSource.Text = "Pages:";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(82, 211);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(135, 20);
            this.txtSource.TabIndex = 12;
            // 
            // flwPages
            // 
            this.flwPages.Controls.Add(this.radAllPages);
            this.flwPages.Controls.Add(this.radPrefixIndex);
            this.flwPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwPages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwPages.Location = new System.Drawing.Point(3, 16);
            this.flwPages.Name = "flwPages";
            this.flwPages.Size = new System.Drawing.Size(202, 52);
            this.flwPages.TabIndex = 0;
            // 
            // gbPages
            // 
            this.gbPages.Controls.Add(this.flwPages);
            this.gbPages.Location = new System.Drawing.Point(9, 56);
            this.gbPages.Name = "gbPages";
            this.gbPages.Size = new System.Drawing.Size(208, 71);
            this.gbPages.TabIndex = 13;
            this.gbPages.TabStop = false;
            this.gbPages.Text = "List of pages";
            // 
            // SpecialPageListMakerProvider
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 332);
            this.Controls.Add(this.gbPages);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboNamespace);
            this.Controls.Add(this.lblNamespace);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.lblRecent);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SpecialPageListMakerProvider";
            this.Text = "Special Pages";
            this.Load += new System.EventHandler(this.SpecialPageListMakerProvider_Load);
            this.flwPages.ResumeLayout(false);
            this.flwPages.PerformLayout();
            this.gbPages.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.RadioButton radAllPages;
        private System.Windows.Forms.RadioButton radPrefixIndex;
        private System.Windows.Forms.Label lblRecent;
        private System.Windows.Forms.ComboBox cboNamespace;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.FlowLayoutPanel flwPages;
        private System.Windows.Forms.GroupBox gbPages;
    }
}
