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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.radAllPages = new System.Windows.Forms.RadioButton();
            this.radPrefixIndex = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.cboNamespaces = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Maintenance reports";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "List of pages";
            // 
            // radAllPages
            // 
            this.radAllPages.AutoSize = true;
            this.radAllPages.Checked = true;
            this.radAllPages.Location = new System.Drawing.Point(15, 83);
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
            this.radPrefixIndex.Location = new System.Drawing.Point(15, 106);
            this.radPrefixIndex.Name = "radPrefixIndex";
            this.radPrefixIndex.Size = new System.Drawing.Size(178, 17);
            this.radPrefixIndex.TabIndex = 7;
            this.radPrefixIndex.Text = "All pages with prefix (Prefixindex)";
            this.radPrefixIndex.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 150);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "Recent changes and logs";
            // 
            // cboNamespaces
            // 
            this.cboNamespaces.FormattingEnabled = true;
            this.cboNamespaces.Location = new System.Drawing.Point(82, 236);
            this.cboNamespaces.Name = "cboNamespaces";
            this.cboNamespaces.Size = new System.Drawing.Size(135, 21);
            this.cboNamespaces.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Namespace";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Pages:";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(82, 211);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(135, 20);
            this.txtSource.TabIndex = 12;
            // 
            // SpecialPageListMakerProvider
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 332);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cboNamespaces);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.radPrefixIndex);
            this.Controls.Add(this.radAllPages);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SpecialPageListMakerProvider";
            this.Text = "Special Pages";
            this.Load += new System.EventHandler(this.SpecialPageListMakerProvider_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radAllPages;
        private System.Windows.Forms.RadioButton radPrefixIndex;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboNamespaces;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSource;
    }
}