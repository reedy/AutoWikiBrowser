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
            this.btnIE = new System.Windows.Forms.Button();
            this.btnDefault = new System.Windows.Forms.Button();
            this.lblNewMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnIE
            // 
            this.btnIE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIE.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnIE.Location = new System.Drawing.Point(115, 58);
            this.btnIE.Name = "btnIE";
            this.btnIE.Size = new System.Drawing.Size(97, 23);
            this.btnIE.TabIndex = 0;
            this.btnIE.Text = "&Internet Explorer";
            this.btnIE.UseVisualStyleBackColor = true;
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDefault.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnDefault.Location = new System.Drawing.Point(12, 59);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(97, 23);
            this.btnDefault.TabIndex = 1;
            this.btnDefault.Text = "&Default browser";
            this.btnDefault.UseVisualStyleBackColor = true;
            // 
            // lblNewMsg
            // 
            this.lblNewMsg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewMsg.Location = new System.Drawing.Point(12, 9);
            this.lblNewMsg.Name = "lblNewMsg";
            this.lblNewMsg.Size = new System.Drawing.Size(200, 43);
            this.lblNewMsg.TabIndex = 2;
            this.lblNewMsg.Text = "You have new messages.\r\n\r\nOpen in:";
            // 
            // TalkMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 94);
            this.ControlBox = false;
            this.Controls.Add(this.lblNewMsg);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.btnIE);
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

        private System.Windows.Forms.Button btnIE;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Label lblNewMsg;
    }
}
