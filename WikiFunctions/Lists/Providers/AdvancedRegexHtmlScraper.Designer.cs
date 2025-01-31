namespace WikiFunctions.Lists.Providers
{
    partial class AdvancedRegexHtmlScraper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedRegexHtmlScraper));
            this.label1 = new System.Windows.Forms.Label();
            this.RegexTextBox = new System.Windows.Forms.TextBox();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToRegexTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.GroupNumber = new System.Windows.Forms.NumericUpDown();
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.SingleLineCheckBox = new System.Windows.Forms.CheckBox();
            this.MultiLineCheckBox = new System.Windows.Forms.CheckBox();
            this.CaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GroupNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // RegexTextBox
            // 
            this.RegexTextBox.ContextMenuStrip = this.menu;
            resources.ApplyResources(this.RegexTextBox, "RegexTextBox");
            this.RegexTextBox.Name = "RegexTextBox";
            this.toolTip1.SetToolTip(this.RegexTextBox, resources.GetString("RegexTextBox.ToolTip"));
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.copyToRegexTesterToolStripMenuItem});
            this.menu.Name = "menu";
            resources.ApplyResources(this.menu, "menu");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // copyToRegexTesterToolStripMenuItem
            // 
            this.copyToRegexTesterToolStripMenuItem.Name = "copyToRegexTesterToolStripMenuItem";
            resources.ApplyResources(this.copyToRegexTesterToolStripMenuItem, "copyToRegexTesterToolStripMenuItem");
            this.copyToRegexTesterToolStripMenuItem.Click += new System.EventHandler(this.copyToRegexTesterToolStripMenuItem_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // GroupNumber
            // 
            resources.ApplyResources(this.GroupNumber, "GroupNumber");
            this.GroupNumber.Name = "GroupNumber";
            this.toolTip1.SetToolTip(this.GroupNumber, resources.GetString("GroupNumber.ToolTip"));
            // 
            // OkBtn
            // 
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.OkBtn, "OkBtn");
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.UseVisualStyleBackColor = true;
            this.OkBtn.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SingleLineCheckBox
            // 
            resources.ApplyResources(this.SingleLineCheckBox, "SingleLineCheckBox");
            this.SingleLineCheckBox.Name = "SingleLineCheckBox";
            this.toolTip1.SetToolTip(this.SingleLineCheckBox, resources.GetString("SingleLineCheckBox.ToolTip"));
            this.SingleLineCheckBox.UseVisualStyleBackColor = true;
            // 
            // MultiLineCheckBox
            // 
            resources.ApplyResources(this.MultiLineCheckBox, "MultiLineCheckBox");
            this.MultiLineCheckBox.Name = "MultiLineCheckBox";
            this.toolTip1.SetToolTip(this.MultiLineCheckBox, resources.GetString("MultiLineCheckBox.ToolTip"));
            this.MultiLineCheckBox.UseVisualStyleBackColor = true;
            // 
            // CaseSensitiveCheckBox
            // 
            resources.ApplyResources(this.CaseSensitiveCheckBox, "CaseSensitiveCheckBox");
            this.CaseSensitiveCheckBox.Name = "CaseSensitiveCheckBox";
            this.toolTip1.SetToolTip(this.CaseSensitiveCheckBox, resources.GetString("CaseSensitiveCheckBox.ToolTip"));
            this.CaseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // AdvancedRegexHtmlScraper
            // 
            this.AcceptButton = this.OkBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CaseSensitiveCheckBox);
            this.Controls.Add(this.MultiLineCheckBox);
            this.Controls.Add(this.SingleLineCheckBox);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.Controls.Add(this.RegexTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GroupNumber);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AdvancedRegexHtmlScraper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdvancedRegexHtmlScraper_FormClosing);
            this.menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GroupNumber)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox RegexTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown GroupNumber;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToRegexTesterToolStripMenuItem;
        private System.Windows.Forms.CheckBox SingleLineCheckBox;
        private System.Windows.Forms.CheckBox MultiLineCheckBox;
        private System.Windows.Forms.CheckBox CaseSensitiveCheckBox;
        private WikiFunctions.Controls.AWBToolTip toolTip1;
    }
}