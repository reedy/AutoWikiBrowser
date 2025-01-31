namespace WikiFunctions.Controls
{
    partial class RegexTester
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegexTester));
            this.lblFind = new System.Windows.Forms.Label();
            this.lblReplace = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.ReplaceBtn = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.Captures = new System.Windows.Forms.TreeView();
            this.FindBtn = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.chkSingleline = new System.Windows.Forms.CheckBox();
            this.chkExplicitCapture = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.gbRegexOptions = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip1.SuspendLayout();
            this.gbRegexOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFind
            // 
            resources.ApplyResources(this.lblFind, "lblFind");
            this.lblFind.Name = "lblFind";
            this.toolTip1.SetToolTip(this.lblFind, resources.GetString("lblFind.ToolTip"));
            // 
            // lblReplace
            // 
            resources.ApplyResources(this.lblReplace, "lblReplace");
            this.lblReplace.Name = "lblReplace";
            this.toolTip1.SetToolTip(this.lblReplace, resources.GetString("lblReplace.ToolTip"));
            // 
            // txtFind
            // 
            resources.ApplyResources(this.txtFind, "txtFind");
            this.txtFind.Name = "txtFind";
            this.toolTip1.SetToolTip(this.txtFind, resources.GetString("txtFind.ToolTip"));
            this.txtFind.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // txtReplace
            // 
            resources.ApplyResources(this.txtReplace, "txtReplace");
            this.txtReplace.Name = "txtReplace";
            this.toolTip1.SetToolTip(this.txtReplace, resources.GetString("txtReplace.ToolTip"));
            this.txtReplace.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtReplace.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // ReplaceBtn
            // 
            resources.ApplyResources(this.ReplaceBtn, "ReplaceBtn");
            this.ReplaceBtn.Name = "ReplaceBtn";
            this.ReplaceBtn.UseVisualStyleBackColor = true;
            this.ReplaceBtn.Click += new System.EventHandler(this.Replace_Click);
            // 
            // txtInput
            // 
            resources.ApplyResources(this.txtInput, "txtInput");
            this.txtInput.Name = "txtInput";
            this.toolTip1.SetToolTip(this.txtInput, resources.GetString("txtInput.ToolTip"));
            this.txtInput.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // ResultText
            // 
            resources.ApplyResources(this.ResultText, "ResultText");
            this.ResultText.BackColor = System.Drawing.SystemColors.Window;
            this.ResultText.Name = "ResultText";
            this.ResultText.ReadOnly = true;
            // 
            // Captures
            // 
            resources.ApplyResources(this.Captures, "Captures");
            this.Captures.Name = "Captures";
            this.toolTip1.SetToolTip(this.Captures, resources.GetString("Captures.ToolTip"));
            // 
            // FindBtn
            // 
            resources.ApplyResources(this.FindBtn, "FindBtn");
            this.FindBtn.Name = "FindBtn";
            this.FindBtn.UseVisualStyleBackColor = true;
            this.FindBtn.Click += new System.EventHandler(this.FindBtn_Click);
            // 
            // Status
            // 
            this.Status.Name = "Status";
            resources.ApplyResources(this.Status, "Status");
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.Status});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            resources.ApplyResources(this.progressBar, "progressBar");
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, resources.GetString("label4.ToolTip"));
            // 
            // chkSingleline
            // 
            resources.ApplyResources(this.chkSingleline, "chkSingleline");
            this.chkSingleline.Name = "chkSingleline";
            this.toolTip1.SetToolTip(this.chkSingleline, resources.GetString("chkSingleline.ToolTip"));
            this.chkSingleline.UseVisualStyleBackColor = true;
            // 
            // chkExplicitCapture
            // 
            resources.ApplyResources(this.chkExplicitCapture, "chkExplicitCapture");
            this.chkExplicitCapture.Name = "chkExplicitCapture";
            this.toolTip1.SetToolTip(this.chkExplicitCapture, resources.GetString("chkExplicitCapture.ToolTip"));
            this.chkExplicitCapture.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreCase
            // 
            resources.ApplyResources(this.chkIgnoreCase, "chkIgnoreCase");
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.toolTip1.SetToolTip(this.chkIgnoreCase, resources.GetString("chkIgnoreCase.ToolTip"));
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // chkMultiline
            // 
            resources.ApplyResources(this.chkMultiline, "chkMultiline");
            this.chkMultiline.Name = "chkMultiline";
            this.toolTip1.SetToolTip(this.chkMultiline, resources.GetString("chkMultiline.ToolTip"));
            this.chkMultiline.UseVisualStyleBackColor = true;
            // 
            // gbRegexOptions
            // 
            resources.ApplyResources(this.gbRegexOptions, "gbRegexOptions");
            this.gbRegexOptions.Controls.Add(this.chkSingleline);
            this.gbRegexOptions.Controls.Add(this.chkExplicitCapture);
            this.gbRegexOptions.Controls.Add(this.chkIgnoreCase);
            this.gbRegexOptions.Controls.Add(this.chkMultiline);
            this.gbRegexOptions.Name = "gbRegexOptions";
            this.gbRegexOptions.TabStop = false;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtInput);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.Captures);
            this.splitContainer1.Panel2.Controls.Add(this.ResultText);
            // 
            // RegexTester
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.gbRegexOptions);
            this.Controls.Add(this.FindBtn);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ReplaceBtn);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.lblReplace);
            this.Controls.Add(this.lblFind);
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegexTester";
            this.ShowIcon = false;
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.RegexTester_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegexTester_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegexTester_KeyPress);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbRegexOptions.ResumeLayout(false);
            this.gbRegexOptions.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Button ReplaceBtn;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.TreeView Captures;
        private System.Windows.Forms.Button FindBtn;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private WikiFunctions.Controls.AWBToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbRegexOptions;
        private System.Windows.Forms.CheckBox chkSingleline;
        private System.Windows.Forms.CheckBox chkExplicitCapture;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.CheckBox chkMultiline;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

