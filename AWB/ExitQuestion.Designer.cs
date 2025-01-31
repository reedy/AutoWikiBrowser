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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExitQuestion));
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
            resources.ApplyResources(this.lblPrompt, "lblPrompt");
            this.lblPrompt.Name = "lblPrompt";
            // 
            // ButtonOK
            // 
            this.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.ButtonOK, "ButtonOK");
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // chkDontAskAgain
            // 
            resources.ApplyResources(this.chkDontAskAgain, "chkDontAskAgain");
            this.chkDontAskAgain.Name = "chkDontAskAgain";
            this.chkDontAskAgain.UseVisualStyleBackColor = true;
            // 
            // lblTimeAndEdits
            // 
            resources.ApplyResources(this.lblTimeAndEdits, "lblTimeAndEdits");
            this.lblTimeAndEdits.Name = "lblTimeAndEdits";
            // 
            // lblDidYouSave
            // 
            resources.ApplyResources(this.lblDidYouSave, "lblDidYouSave");
            this.lblDidYouSave.Name = "lblDidYouSave";
            // 
            // ExitQuestion
            // 
            this.AcceptButton = this.ButtonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
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
