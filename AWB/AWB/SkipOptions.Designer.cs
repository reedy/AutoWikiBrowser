namespace AutoWikiBrowser
{
    partial class SkipOptions
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
            this.chkUnicoder = new System.Windows.Forms.CheckBox();
            this.chkTagger = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkUnicoder
            // 
            this.chkUnicoder.AutoSize = true;
            this.chkUnicoder.Location = new System.Drawing.Point(13, 13);
            this.chkUnicoder.Name = "chkUnicoder";
            this.chkUnicoder.Size = new System.Drawing.Size(141, 17);
            this.chkUnicoder.TabIndex = 0;
            this.chkUnicoder.Text = "Skip if no unicodification";
            this.chkUnicoder.UseVisualStyleBackColor = true;
            // 
            // chkTagger
            // 
            this.chkTagger.AutoSize = true;
            this.chkTagger.Location = new System.Drawing.Point(13, 37);
            this.chkTagger.Name = "chkTagger";
            this.chkTagger.Size = new System.Drawing.Size(132, 17);
            this.chkTagger.TabIndex = 1;
            this.chkTagger.Text = "Skip if no tag changes";
            this.chkTagger.UseVisualStyleBackColor = true;
            // 
            // SkipOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 235);
            this.Controls.Add(this.chkTagger);
            this.Controls.Add(this.chkUnicoder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SkipOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUnicoder;
        private System.Windows.Forms.CheckBox chkTagger;
    }
}