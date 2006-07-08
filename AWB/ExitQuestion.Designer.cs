namespace AutoWikiBrowser
{
    partial class ExitQuestion
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.chkDontAskAgain = new System.Windows.Forms.CheckBox();
            this.lblTimeAndEdits = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(9, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(246, 51);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Ok to exit AutoWikiBrowser?";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(47, 63);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 1;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(128, 63);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // chkDontAskAgain
            // 
            this.chkDontAskAgain.AutoSize = true;
            this.chkDontAskAgain.Location = new System.Drawing.Point(9, 119);
            this.chkDontAskAgain.Name = "chkDontAskAgain";
            this.chkDontAskAgain.Size = new System.Drawing.Size(100, 17);
            this.chkDontAskAgain.TabIndex = 3;
            this.chkDontAskAgain.Text = "Don\'t ask again";
            this.chkDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // lblTimeAndEdits
            // 
            this.lblTimeAndEdits.AutoSize = true;
            this.lblTimeAndEdits.Location = new System.Drawing.Point(9, 94);
            this.lblTimeAndEdits.Name = "lblTimeAndEdits";
            this.lblTimeAndEdits.Size = new System.Drawing.Size(0, 13);
            this.lblTimeAndEdits.TabIndex = 4;
            // 
            // ExitQuestion
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(267, 142);
            this.Controls.Add(this.lblTimeAndEdits);
            this.Controls.Add(this.chkDontAskAgain);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExitQuestion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exit?";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label lblTimeAndEdits;
        private System.Windows.Forms.CheckBox chkDontAskAgain;
    }
}