namespace WikiFunctions.Controls
{
    partial class PageContainsControl
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
            this.chkSkipContainsCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkSkipContainsAfterProcessing = new System.Windows.Forms.CheckBox();
            this.chkSkipContainsIsRegex = new System.Windows.Forms.CheckBox();
            this.txtSkipIfContains = new System.Windows.Forms.RichTextBox();
            this.chkSkipIfContains = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkSkipContainsCaseSensitive
            // 
            this.chkSkipContainsCaseSensitive.AutoSize = true;
            this.chkSkipContainsCaseSensitive.Location = new System.Drawing.Point(66, 26);
            this.chkSkipContainsCaseSensitive.Name = "chkSkipContainsCaseSensitive";
            this.chkSkipContainsCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkSkipContainsCaseSensitive.TabIndex = 10;
            this.chkSkipContainsCaseSensitive.Text = "Case sens&itive";
            this.chkSkipContainsCaseSensitive.UseVisualStyleBackColor = true;
            this.chkSkipContainsCaseSensitive.CheckedChanged += new System.EventHandler(this.Invalidate_CheckedChanged);
            // 
            // chkSkipContainsAfterProcessing
            // 
            this.chkSkipContainsAfterProcessing.AutoSize = true;
            this.chkSkipContainsAfterProcessing.Location = new System.Drawing.Point(169, 26);
            this.chkSkipContainsAfterProcessing.Name = "chkSkipContainsAfterProcessing";
            this.chkSkipContainsAfterProcessing.Size = new System.Drawing.Size(81, 17);
            this.chkSkipContainsAfterProcessing.TabIndex = 11;
            this.chkSkipContainsAfterProcessing.Text = "Check after";
            this.chkSkipContainsAfterProcessing.UseVisualStyleBackColor = true;
            // 
            // chkSkipContainsIsRegex
            // 
            this.chkSkipContainsIsRegex.AutoSize = true;
            this.chkSkipContainsIsRegex.Location = new System.Drawing.Point(0, 26);
            this.chkSkipContainsIsRegex.Name = "chkSkipContainsIsRegex";
            this.chkSkipContainsIsRegex.Size = new System.Drawing.Size(57, 17);
            this.chkSkipContainsIsRegex.TabIndex = 9;
            this.chkSkipContainsIsRegex.Text = "Regex";
            this.chkSkipContainsIsRegex.UseVisualStyleBackColor = true;
            this.chkSkipContainsIsRegex.CheckedChanged += new System.EventHandler(this.Invalidate_CheckedChanged);
            // 
            // txtSkipIfContains
            // 
            this.txtSkipIfContains.DetectUrls = false;
            this.txtSkipIfContains.Enabled = false;
            this.txtSkipIfContains.Location = new System.Drawing.Point(100, 1);
            this.txtSkipIfContains.Multiline = false;
            this.txtSkipIfContains.Name = "txtSkipIfContains";
            this.txtSkipIfContains.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtSkipIfContains.Size = new System.Drawing.Size(154, 20);
            this.txtSkipIfContains.TabIndex = 8;
            this.txtSkipIfContains.Text = "";
            // 
            // chkSkipIfContains
            // 
            this.chkSkipIfContains.Location = new System.Drawing.Point(0, 3);
            this.chkSkipIfContains.Name = "chkSkipIfContains";
            this.chkSkipIfContains.Size = new System.Drawing.Size(89, 17);
            this.chkSkipIfContains.TabIndex = 7;
            this.chkSkipIfContains.Text = "&Contains:";
            this.chkSkipIfContains.UseVisualStyleBackColor = true;
            this.chkSkipIfContains.CheckedChanged += new System.EventHandler(this.chkSkipIfContains_CheckedChanged);
            // 
            // PageContainsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkSkipContainsCaseSensitive);
            this.Controls.Add(this.chkSkipContainsAfterProcessing);
            this.Controls.Add(this.chkSkipContainsIsRegex);
            this.Controls.Add(this.txtSkipIfContains);
            this.Controls.Add(this.chkSkipIfContains);
            this.Name = "PageContainsControl";
            this.Size = new System.Drawing.Size(260, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSkipContainsCaseSensitive;
        private System.Windows.Forms.CheckBox chkSkipContainsAfterProcessing;
        private System.Windows.Forms.CheckBox chkSkipContainsIsRegex;
        private System.Windows.Forms.RichTextBox txtSkipIfContains;
        private System.Windows.Forms.CheckBox chkSkipIfContains;
    }
}
