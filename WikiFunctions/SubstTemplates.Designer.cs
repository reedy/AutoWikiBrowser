namespace WikiFunctions
{
    partial class SubstTemplates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubstTemplates));
            this.lblTemplates = new System.Windows.Forms.Label();
            this.textBoxTemplates = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.chkIgnoreUnformatted = new System.Windows.Forms.CheckBox();
            this.chkUseExpandTemplates = new System.Windows.Forms.CheckBox();
            this.chkIncludeComment = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblTemplates
            // 
            resources.ApplyResources(this.lblTemplates, "lblTemplates");
            this.lblTemplates.Name = "lblTemplates";
            // 
            // textBoxTemplates
            // 
            this.textBoxTemplates.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxTemplates, "textBoxTemplates");
            this.textBoxTemplates.Name = "textBoxTemplates";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // chkIgnoreUnformatted
            // 
            resources.ApplyResources(this.chkIgnoreUnformatted, "chkIgnoreUnformatted");
            this.chkIgnoreUnformatted.Name = "chkIgnoreUnformatted";
            // 
            // chkUseExpandTemplates
            // 
            resources.ApplyResources(this.chkUseExpandTemplates, "chkUseExpandTemplates");
            this.chkUseExpandTemplates.Checked = true;
            this.chkUseExpandTemplates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseExpandTemplates.Name = "chkUseExpandTemplates";
            this.chkUseExpandTemplates.CheckedChanged += new System.EventHandler(this.chkUseExpandTemplates_CheckedChanged);
            // 
            // chkIncludeComment
            // 
            resources.ApplyResources(this.chkIncludeComment, "chkIncludeComment");
            this.chkIncludeComment.Name = "chkIncludeComment";
            // 
            // SubstTemplates
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.chkIncludeComment);
            this.Controls.Add(this.chkUseExpandTemplates);
            this.Controls.Add(this.chkIgnoreUnformatted);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxTemplates);
            this.Controls.Add(this.lblTemplates);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SubstTemplates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTemplates;
        private System.Windows.Forms.TextBox textBoxTemplates;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkIgnoreUnformatted;
        private System.Windows.Forms.CheckBox chkUseExpandTemplates;
        private System.Windows.Forms.CheckBox chkIncludeComment;
    }
}
