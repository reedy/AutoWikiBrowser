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
            this.chkUnlock = new System.Windows.Forms.CheckBox();
            this.lbMove = new System.Windows.Forms.ListBox();
            this.lblMove = new System.Windows.Forms.Label();
            this.lbEdit = new System.Windows.Forms.ListBox();
            this.lblEdit = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkUnlock
            // 
            this.chkUnlock.AutoSize = true;
            this.chkUnlock.Location = new System.Drawing.Point(156, 65);
            this.chkUnlock.Name = "chkUnlock";
            this.chkUnlock.Size = new System.Drawing.Size(146, 17);
            this.chkUnlock.TabIndex = 17;
            this.chkUnlock.Text = "Unlock move permissions";
            this.chkUnlock.UseVisualStyleBackColor = true;
            this.chkUnlock.CheckedChanged += new System.EventHandler(this.chkUnlock_CheckedChanged);
            // 
            // lbMove
            // 
            this.lbMove.Enabled = false;
            this.lbMove.FormattingEnabled = true;
            this.lbMove.Items.AddRange(new object[] {
            "Unprotected",
            "Semi-protected",
            "Fully protected"});
            this.lbMove.Location = new System.Drawing.Point(157, 16);
            this.lbMove.Name = "lbMove";
            this.lbMove.Size = new System.Drawing.Size(145, 43);
            this.lbMove.TabIndex = 16;
            this.lbMove.SelectedIndexChanged += new System.EventHandler(this.BothListBox_SelectedIndexChanged);
            // 
            // lblMove
            // 
            this.lblMove.AutoSize = true;
            this.lblMove.Location = new System.Drawing.Point(154, 0);
            this.lblMove.Name = "lblMove";
            this.lblMove.Size = new System.Drawing.Size(34, 13);
            this.lblMove.TabIndex = 15;
            this.lblMove.Text = "Move";
            // 
            // lbEdit
            // 
            this.lbEdit.FormattingEnabled = true;
            this.lbEdit.Items.AddRange(new object[] {
            "Unprotected",
            "Semi-protected",
            "Fully protected"});
            this.lbEdit.Location = new System.Drawing.Point(6, 16);
            this.lbEdit.Name = "lbEdit";
            this.lbEdit.Size = new System.Drawing.Size(145, 43);
            this.lbEdit.TabIndex = 13;
            this.lbEdit.SelectedIndexChanged += new System.EventHandler(this.BothListBox_SelectedIndexChanged);
            this.lbEdit.SelectedIndexChanged += new System.EventHandler(this.lbEdit_SelectedIndexChanged);
            // 
            // lblEdit
            // 
            this.lblEdit.AutoSize = true;
            this.lblEdit.Location = new System.Drawing.Point(3, 0);
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(25, 13);
            this.lblEdit.TabIndex = 12;
            this.lblEdit.Text = "Edit";
            // 
            // MoveDeleteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUnlock);
            this.Controls.Add(this.lbMove);
            this.Controls.Add(this.lblMove);
            this.Controls.Add(this.lbEdit);
            this.Controls.Add(this.lblEdit);
            this.Name = "MoveDeleteControl";
            this.Size = new System.Drawing.Size(311, 88);
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
