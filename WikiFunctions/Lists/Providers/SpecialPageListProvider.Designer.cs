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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecialPageListProvider));
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
            // cboNamespace
            // 
            resources.ApplyResources(this.cboNamespace, "cboNamespace");
            this.cboNamespace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNamespace.FormattingEnabled = true;
            this.cboNamespace.Name = "cboNamespace";
            // 
            // lblNamespace
            // 
            resources.ApplyResources(this.lblNamespace, "lblNamespace");
            this.lblNamespace.Name = "lblNamespace";
            // 
            // lblSource
            // 
            resources.ApplyResources(this.lblSource, "lblSource");
            this.lblSource.Name = "lblSource";
            // 
            // txtPages
            // 
            resources.ApplyResources(this.txtPages, "txtPages");
            this.txtPages.Name = "txtPages";
            // 
            // cmboSourceSelect
            // 
            resources.ApplyResources(this.cmboSourceSelect, "cmboSourceSelect");
            this.cmboSourceSelect.DropDownHeight = 250;
            this.cmboSourceSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSourceSelect.FormattingEnabled = true;
            this.cmboSourceSelect.Name = "cmboSourceSelect";
            this.cmboSourceSelect.Sorted = true;
            this.cmboSourceSelect.SelectedIndexChanged += new System.EventHandler(this.cmboSourceSelect_SelectedIndexChanged);
            // 
            // lblPages
            // 
            resources.ApplyResources(this.lblPages, "lblPages");
            this.lblPages.Name = "lblPages";
            // 
            // SpecialPageListProvider
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblPages);
            this.Controls.Add(this.cmboSourceSelect);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboNamespace);
            this.Controls.Add(this.lblNamespace);
            this.Controls.Add(this.txtPages);
            this.Controls.Add(this.lblSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SpecialPageListProvider";
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


