namespace WikiFunctions.Controls
{
    partial class EditProtectControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditProtectControl));
            this.chkUnlock = new System.Windows.Forms.CheckBox();
            this.lbMove = new System.Windows.Forms.ListBox();
            this.lblMove = new System.Windows.Forms.Label();
            this.lbEdit = new System.Windows.Forms.ListBox();
            this.lblEdit = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkUnlock
            // 
            resources.ApplyResources(this.chkUnlock, "chkUnlock");
            this.chkUnlock.Name = "chkUnlock";
            this.chkUnlock.UseVisualStyleBackColor = true;
            this.chkUnlock.CheckedChanged += new System.EventHandler(this.chkUnlock_CheckedChanged);
            // 
            // lbMove
            // 
            resources.ApplyResources(this.lbMove, "lbMove");
            this.lbMove.FormattingEnabled = true;
            this.lbMove.Name = "lbMove";
            this.lbMove.SelectedIndexChanged += new System.EventHandler(this.BothListBox_SelectedIndexChanged);
            // 
            // lblMove
            // 
            resources.ApplyResources(this.lblMove, "lblMove");
            this.lblMove.Name = "lblMove";
            // 
            // lbEdit
            // 
            this.lbEdit.FormattingEnabled = true;
            resources.ApplyResources(this.lbEdit, "lbEdit");
            this.lbEdit.Name = "lbEdit";
            this.lbEdit.SelectedIndexChanged += new System.EventHandler(this.lbEdit_SelectedIndexChanged);
            // 
            // lblEdit
            // 
            resources.ApplyResources(this.lblEdit, "lblEdit");
            this.lblEdit.Name = "lblEdit";
            // 
            // EditProtectControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUnlock);
            this.Controls.Add(this.lbMove);
            this.Controls.Add(this.lblMove);
            this.Controls.Add(this.lbEdit);
            this.Controls.Add(this.lblEdit);
            this.Name = "EditProtectControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUnlock;
        private System.Windows.Forms.ListBox lbMove;
        private System.Windows.Forms.Label lblMove;
        private System.Windows.Forms.ListBox lbEdit;
        private System.Windows.Forms.Label lblEdit;
    }
}
