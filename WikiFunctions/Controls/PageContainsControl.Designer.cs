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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageContainsControl));
            this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkAfterProcessing = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.RichTextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.SuspendLayout();
            // 
            // chkCaseSensitive
            // 
            resources.ApplyResources(this.chkCaseSensitive, "chkCaseSensitive");
            this.chkCaseSensitive.Name = "chkCaseSensitive";
            this.toolTip1.SetToolTip(this.chkCaseSensitive, resources.GetString("chkCaseSensitive.ToolTip"));
            this.chkCaseSensitive.UseVisualStyleBackColor = true;
            this.chkCaseSensitive.CheckedChanged += new System.EventHandler(this.InvalidateComparer);
            // 
            // chkAfterProcessing
            // 
            resources.ApplyResources(this.chkAfterProcessing, "chkAfterProcessing");
            this.chkAfterProcessing.Name = "chkAfterProcessing";
            this.toolTip1.SetToolTip(this.chkAfterProcessing, resources.GetString("chkAfterProcessing.ToolTip"));
            this.chkAfterProcessing.UseVisualStyleBackColor = true;
            // 
            // chkIsRegex
            // 
            resources.ApplyResources(this.chkIsRegex, "chkIsRegex");
            this.chkIsRegex.Name = "chkIsRegex";
            this.toolTip1.SetToolTip(this.chkIsRegex, resources.GetString("chkIsRegex.ToolTip"));
            this.chkIsRegex.UseVisualStyleBackColor = true;
            this.chkIsRegex.CheckedChanged += new System.EventHandler(this.InvalidateComparer);
            // 
            // txtContains
            // 
            this.txtContains.DetectUrls = false;
            resources.ApplyResources(this.txtContains, "txtContains");
            this.txtContains.Name = "txtContains";
            this.toolTip1.SetToolTip(this.txtContains, resources.GetString("txtContains.ToolTip"));
            this.txtContains.TextChanged += new System.EventHandler(this.InvalidateComparer);
            // 
            // chkContains
            // 
            resources.ApplyResources(this.chkContains, "chkContains");
            this.chkContains.Name = "chkContains";
            this.toolTip1.SetToolTip(this.chkContains, resources.GetString("chkContains.ToolTip"));
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkSkipIfContains_CheckedChanged);
            // 
            // PageContainsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkCaseSensitive);
            this.Controls.Add(this.chkAfterProcessing);
            this.Controls.Add(this.chkIsRegex);
            this.Controls.Add(this.txtContains);
            this.Controls.Add(this.chkContains);
            this.Name = "PageContainsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.CheckBox chkCaseSensitive;
        protected System.Windows.Forms.CheckBox chkAfterProcessing;
        protected System.Windows.Forms.CheckBox chkIsRegex;
        protected System.Windows.Forms.RichTextBox txtContains;
        protected System.Windows.Forms.CheckBox chkContains;
        protected WikiFunctions.Controls.AWBToolTip toolTip1;
    }
}
