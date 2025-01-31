namespace WikiFunctions.Controls
{
    partial class NamespacesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NamespacesControl));
            this.chkContents = new System.Windows.Forms.CheckBox();
            this.checkedLBContent = new System.Windows.Forms.CheckedListBox();
            this.chkTalk = new System.Windows.Forms.CheckBox();
            this.checkedLBTalk = new System.Windows.Forms.CheckedListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkContents
            // 
            resources.ApplyResources(this.chkContents, "chkContents");
            this.chkContents.Name = "chkContents";
            this.chkContents.Tag = "1000";
            this.chkContents.CheckedChanged += new System.EventHandler(this.chkContents_CheckedChanged);
            // 
            // checkedLBContent
            // 
            resources.ApplyResources(this.checkedLBContent, "checkedLBContent");
            this.checkedLBContent.FormattingEnabled = true;
            this.checkedLBContent.Name = "checkedLBContent";
            // 
            // chkTalk
            // 
            resources.ApplyResources(this.chkTalk, "chkTalk");
            this.chkTalk.Name = "chkTalk";
            this.chkTalk.Tag = "1001";
            this.chkTalk.CheckedChanged += new System.EventHandler(this.chkTalk_CheckedChanged);
            // 
            // checkedLBTalk
            // 
            resources.ApplyResources(this.checkedLBTalk, "checkedLBTalk");
            this.checkedLBTalk.FormattingEnabled = true;
            this.checkedLBTalk.Name = "checkedLBTalk";
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkContents);
            this.splitContainer1.Panel1.Controls.Add(this.checkedLBContent);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkTalk);
            this.splitContainer1.Panel2.Controls.Add(this.checkedLBTalk);
            // 
            // NamespacesControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "NamespacesControl";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkContents;
        private System.Windows.Forms.CheckedListBox checkedLBContent;
        private System.Windows.Forms.CheckBox chkTalk;
        private System.Windows.Forms.CheckedListBox checkedLBTalk;
        private System.Windows.Forms.SplitContainer splitContainer1;

    }
}
