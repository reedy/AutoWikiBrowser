namespace AutoWikiBrowser
{
    partial class TalkMessage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TalkMessage));
            this.btnDefault = new System.Windows.Forms.Button();
            this.lblNewMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDefault
            // 
            resources.ApplyResources(this.btnDefault, "btnDefault");
            this.btnDefault.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.UseVisualStyleBackColor = true;
            // 
            // lblNewMsg
            // 
            resources.ApplyResources(this.lblNewMsg, "lblNewMsg");
            this.lblNewMsg.Name = "lblNewMsg";
            // 
            // TalkMessage
            // 
            this.AcceptButton = this.btnDefault;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.lblNewMsg);
            this.Controls.Add(this.btnDefault);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TalkMessage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Label lblNewMsg;
        
    }
}
