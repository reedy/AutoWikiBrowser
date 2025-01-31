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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkipOptions));
            this.btnClose = new System.Windows.Forms.Button();
            this.CheckAll = new System.Windows.Forms.CheckBox();
            this.CheckNone = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.skipListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CheckAll
            // 
            resources.ApplyResources(this.CheckAll, "CheckAll");
            this.CheckAll.Name = "CheckAll";
            this.toolTip1.SetToolTip(this.CheckAll, resources.GetString("CheckAll.ToolTip"));
            this.CheckAll.UseVisualStyleBackColor = true;
            this.CheckAll.CheckedChanged += new System.EventHandler(this.CheckAll_CheckedChanged);
            // 
            // CheckNone
            // 
            resources.ApplyResources(this.CheckNone, "CheckNone");
            this.CheckNone.Name = "CheckNone";
            this.toolTip1.SetToolTip(this.CheckNone, resources.GetString("CheckNone.ToolTip"));
            this.CheckNone.UseVisualStyleBackColor = true;
            this.CheckNone.CheckedChanged += new System.EventHandler(this.CheckNone_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // skipListBox
            // 
            resources.ApplyResources(this.skipListBox, "skipListBox");
            this.skipListBox.FormattingEnabled = true;
            this.skipListBox.Name = "skipListBox";
            // 
            // SkipOptions
            // 
            this.AcceptButton = this.btnClose;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.Controls.Add(this.skipListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CheckNone);
            this.Controls.Add(this.CheckAll);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox CheckAll;
        private System.Windows.Forms.CheckBox CheckNone;
        private System.Windows.Forms.Label label1;
        private WikiFunctions.Controls.AWBToolTip toolTip1;
        private System.Windows.Forms.CheckedListBox skipListBox;
    }
}