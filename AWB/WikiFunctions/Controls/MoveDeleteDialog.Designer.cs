namespace WikiFunctions.Controls
{
    partial class MoveDeleteDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cmboSummary = new System.Windows.Forms.ComboBox();
            this.lbEdit = new System.Windows.Forms.ListBox();
            this.lbMove = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExpiry = new System.Windows.Forms.TextBox();
            this.chkUnlock = new System.Windows.Forms.CheckBox();
            this.chkAutoProtect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtNewTitle
            // 
            this.txtNewTitle.Location = new System.Drawing.Point(62, 12);
            this.txtNewTitle.Name = "txtNewTitle";
            this.txtNewTitle.Size = new System.Drawing.Size(339, 20);
            this.txtNewTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "New title";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Summary";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(132, 152);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(213, 152);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cmboSummary
            // 
            this.cmboSummary.FormattingEnabled = true;
            this.cmboSummary.Location = new System.Drawing.Point(62, 38);
            this.cmboSummary.Name = "cmboSummary";
            this.cmboSummary.Size = new System.Drawing.Size(339, 21);
            this.cmboSummary.TabIndex = 6;
            // 
            // lbEdit
            // 
            this.lbEdit.FormattingEnabled = true;
            this.lbEdit.Items.AddRange(new object[] {
            "Unprotected",
            "Semi-protected",
            "Fully protected"});
            this.lbEdit.Location = new System.Drawing.Point(62, 81);
            this.lbEdit.Name = "lbEdit";
            this.lbEdit.Size = new System.Drawing.Size(145, 43);
            this.lbEdit.TabIndex = 8;
            this.lbEdit.SelectedIndexChanged += new System.EventHandler(this.lbEdit_SelectedIndexChanged);
            // 
            // lbMove
            // 
            this.lbMove.Enabled = false;
            this.lbMove.FormattingEnabled = true;
            this.lbMove.Items.AddRange(new object[] {
            "Unprotected",
            "Semi-protected",
            "Fully protected"});
            this.lbMove.Location = new System.Drawing.Point(213, 81);
            this.lbMove.Name = "lbMove";
            this.lbMove.Size = new System.Drawing.Size(145, 43);
            this.lbMove.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Edit";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Move";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Expiry";
            // 
            // txtExpiry
            // 
            this.txtExpiry.Location = new System.Drawing.Point(62, 38);
            this.txtExpiry.Name = "txtExpiry";
            this.txtExpiry.Size = new System.Drawing.Size(339, 20);
            this.txtExpiry.TabIndex = 12;
            // 
            // chkUnlock
            // 
            this.chkUnlock.AutoSize = true;
            this.chkUnlock.Location = new System.Drawing.Point(212, 130);
            this.chkUnlock.Name = "chkUnlock";
            this.chkUnlock.Size = new System.Drawing.Size(146, 17);
            this.chkUnlock.TabIndex = 14;
            this.chkUnlock.Text = "Unlock move permissions";
            this.chkUnlock.UseVisualStyleBackColor = true;
            this.chkUnlock.CheckedChanged += new System.EventHandler(this.chkUnlock_CheckedChanged);
            // 
            // chkAutoProtect
            // 
            this.chkAutoProtect.AutoSize = true;
            this.chkAutoProtect.Location = new System.Drawing.Point(62, 130);
            this.chkAutoProtect.Name = "chkAutoProtect";
            this.chkAutoProtect.Size = new System.Drawing.Size(99, 17);
            this.chkAutoProtect.TabIndex = 15;
            this.chkAutoProtect.Text = "Auto Protect All";
            this.chkAutoProtect.UseVisualStyleBackColor = true;
            this.chkAutoProtect.Visible = false;
            // 
            // MoveDeleteDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 187);
            this.Controls.Add(this.chkAutoProtect);
            this.Controls.Add(this.chkUnlock);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtExpiry);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbMove);
            this.Controls.Add(this.lbEdit);
            this.Controls.Add(this.cmboSummary);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtNewTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MoveDeleteDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Enter details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtNewTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmboSummary;
        private System.Windows.Forms.ListBox lbEdit;
        private System.Windows.Forms.ListBox lbMove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtExpiry;
        private System.Windows.Forms.CheckBox chkUnlock;
        private System.Windows.Forms.CheckBox chkAutoProtect;
    }
}