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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageNotContainsControl));
            this.SuspendLayout();
            // 
            // chkCaseSensitive
            // 
            this.toolTip1.SetToolTip(this.chkCaseSensitive, resources.GetString("chkCaseSensitive.ToolTip"));
            // 
            // chkAfterProcessing
            // 
            this.toolTip1.SetToolTip(this.chkAfterProcessing, resources.GetString("chkAfterProcessing.ToolTip"));
            // 
            // chkIsRegex
            // 
            this.toolTip1.SetToolTip(this.chkIsRegex, resources.GetString("chkIsRegex.ToolTip"));
            // 
            // txtContains
            // 
            resources.ApplyResources(this.txtContains, "txtContains");
            this.toolTip1.SetToolTip(this.txtContains, resources.GetString("txtContains.ToolTip"));
            // 
            // chkContains
            // 
            resources.ApplyResources(this.chkContains, "chkContains");
            this.toolTip1.SetToolTip(this.chkContains, resources.GetString("chkContains.ToolTip"));
            // 
            // PageNotContainsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "PageNotContainsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
