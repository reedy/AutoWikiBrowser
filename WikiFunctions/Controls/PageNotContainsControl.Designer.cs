namespace WikiFunctions.Controls
{
    partial class PageNotContainsControl
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
            this.SuspendLayout();
            // 
            // chkCaseSensitive
            // 
            this.toolTip1.SetToolTip(this.chkCaseSensitive, "Makes \"Doesn\'t contain\" matches case-sensitive");
            // 
            // chkAfterProcessing
            // 
            this.toolTip1.SetToolTip(this.chkAfterProcessing, "Apply the \"Doesn\'t contain\" check after processing the page");
            // 
            // chkIsRegex
            // 
            this.toolTip1.SetToolTip(this.chkIsRegex, "Enables regular expressions for \"Doesn\'t contain\"");
            // 
            // txtContains
            // 
            this.toolTip1.SetToolTip(this.txtContains, "Skip pages that don\'t contain this text");
            // 
            // chkContains
            // 
            this.chkContains.Size = new System.Drawing.Size(107, 17);
            this.chkContains.Text = "Doesn\'t contain:";
            this.toolTip1.SetToolTip(this.chkContains, "Skip pages that don\'t contain this text");
            // 
            // PageNotContainsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PageNotContainsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
