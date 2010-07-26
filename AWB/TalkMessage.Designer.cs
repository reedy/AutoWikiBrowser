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
            this.btnDefault = new System.Windows.Forms.Button();
            this.lblNewMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDefault.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnDefault.Location = new System.Drawing.Point(37, 34);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(97, 23);
            this.btnDefault.TabIndex = 1;
            this.btnDefault.Text = "&View";
            this.btnDefault.UseVisualStyleBackColor = true;
            // 
            // lblNewMsg
            // 
            this.lblNewMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewMsg.Location = new System.Drawing.Point(20, 9);
            this.lblNewMsg.Name = "lblNewMsg";
            this.lblNewMsg.Size = new System.Drawing.Size(131, 16);
            this.lblNewMsg.TabIndex = 2;
            this.lblNewMsg.Text = "You have new messages.";
            // 
            // TalkMessage
            // 
            this.AcceptButton = this.btnDefault;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(170, 69);
            this.ControlBox = false;
            this.Controls.Add(this.lblNewMsg);
            this.Controls.Add(this.btnDefault);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TalkMessage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New messages";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Label lblNewMsg;
        
    }
}
