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
            this.lblPrompt = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.chkDontAskAgain = new System.Windows.Forms.CheckBox();
            this.lblTimeAndEdits = new System.Windows.Forms.Label();
            this.lblDidYouSave = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPrompt
            // 
            this.lblPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrompt.Location = new System.Drawing.Point(12, 9);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(246, 25);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = "Exit AutoWikiBrowser?";
            this.lblPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonOK.Location = new System.Drawing.Point(54, 97);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 1;
            this.ButtonOK.Text = "E&xit";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(135, 97);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Go back";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // chkDontAskAgain
            // 
            this.chkDontAskAgain.AutoSize = true;
            this.chkDontAskAgain.Location = new System.Drawing.Point(84, 74);
            this.chkDontAskAgain.Name = "chkDontAskAgain";
            this.chkDontAskAgain.Size = new System.Drawing.Size(100, 17);
            this.chkDontAskAgain.TabIndex = 3;
            this.chkDontAskAgain.Text = "&Don\'t ask again";
            this.chkDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // lblTimeAndEdits
            // 
            this.lblTimeAndEdits.Location = new System.Drawing.Point(12, 34);
            this.lblTimeAndEdits.Name = "lblTimeAndEdits";
            this.lblTimeAndEdits.Size = new System.Drawing.Size(246, 19);
            this.lblTimeAndEdits.TabIndex = 4;
            this.lblTimeAndEdits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDidYouSave
            // 
            this.lblDidYouSave.Location = new System.Drawing.Point(12, 39);
            this.lblDidYouSave.Name = "lblDidYouSave";
            this.lblDidYouSave.Size = new System.Drawing.Size(246, 23);
            this.lblDidYouSave.TabIndex = 5;
            this.lblDidYouSave.Text = "Have you remembered to save your settings?";
            this.lblDidYouSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ExitQuestion
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(270, 132);
            this.Controls.Add(this.lblDidYouSave);
            this.Controls.Add(this.lblTimeAndEdits);
            this.Controls.Add(this.chkDontAskAgain);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.lblPrompt);
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

        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label lblTimeAndEdits;
        private System.Windows.Forms.CheckBox chkDontAskAgain;
        private System.Windows.Forms.Label lblDidYouSave;
    }
}
