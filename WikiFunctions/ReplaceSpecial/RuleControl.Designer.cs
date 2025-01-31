namespace WikiFunctions.ReplaceSpecial
{
  partial class RuleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RuleControl));
            this.RuleGroupBox = new System.Windows.Forms.GroupBox();
            this.RuleTabControl = new System.Windows.Forms.TabControl();
            this.ReplaceTabPage = new System.Windows.Forms.TabPage();
            this.ReplaceSplit = new System.Windows.Forms.SplitContainer();
            this.ReplaceTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.WithTextbox = new System.Windows.Forms.TextBox();
            this.ReplaceIsCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.ReplaceIsRegexCheckbox = new System.Windows.Forms.CheckBox();
            this.ReplaceIsMultilineCheckBox = new System.Windows.Forms.CheckBox();
            this.ReplaceIsSinglelineCheckbox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.NumberOfTimesUpDown = new System.Windows.Forms.NumericUpDown();
            this.TestFind = new System.Windows.Forms.Button();
            this.IfTabPage = new System.Windows.Forms.TabPage();
            this.IfSplit = new System.Windows.Forms.SplitContainer();
            this.HasMatchLabel = new System.Windows.Forms.Label();
            this.IfContainsTextBox = new System.Windows.Forms.TextBox();
            this.IfNotContainsLabel = new System.Windows.Forms.Label();
            this.IfNotContainsTextBox = new System.Windows.Forms.TextBox();
            this.IfIsSinglelineCheckBox = new System.Windows.Forms.CheckBox();
            this.IfIsMultilineCheckbox = new System.Windows.Forms.CheckBox();
            this.IfIsCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
            this.IfIsRegexCheckBox = new System.Windows.Forms.CheckBox();
            this.TestIf = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.NameTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.RuleTypeCombobox = new System.Windows.Forms.ComboBox();
            this.RuleEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.testIfContainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testIfNotContainsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RuleGroupBox.SuspendLayout();
            this.RuleTabControl.SuspendLayout();
            this.ReplaceTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceSplit)).BeginInit();
            this.ReplaceSplit.Panel1.SuspendLayout();
            this.ReplaceSplit.Panel2.SuspendLayout();
            this.ReplaceSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).BeginInit();
            this.IfTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IfSplit)).BeginInit();
            this.IfSplit.Panel1.SuspendLayout();
            this.IfSplit.Panel2.SuspendLayout();
            this.IfSplit.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // RuleGroupBox
            // 
            resources.ApplyResources(this.RuleGroupBox, "RuleGroupBox");
            this.RuleGroupBox.Controls.Add(this.RuleTabControl);
            this.RuleGroupBox.Controls.Add(this.label1);
            this.RuleGroupBox.Controls.Add(this.NameTextbox);
            this.RuleGroupBox.Controls.Add(this.label4);
            this.RuleGroupBox.Controls.Add(this.RuleTypeCombobox);
            this.RuleGroupBox.Controls.Add(this.RuleEnabledCheckBox);
            this.RuleGroupBox.Name = "RuleGroupBox";
            this.RuleGroupBox.TabStop = false;
            // 
            // RuleTabControl
            // 
            resources.ApplyResources(this.RuleTabControl, "RuleTabControl");
            this.RuleTabControl.Controls.Add(this.ReplaceTabPage);
            this.RuleTabControl.Controls.Add(this.IfTabPage);
            this.RuleTabControl.Name = "RuleTabControl";
            this.RuleTabControl.SelectedIndex = 0;
            // 
            // ReplaceTabPage
            // 
            this.ReplaceTabPage.BackColor = System.Drawing.Color.White;
            this.ReplaceTabPage.Controls.Add(this.ReplaceSplit);
            this.ReplaceTabPage.Controls.Add(this.ReplaceIsCaseSensitiveCheckBox);
            this.ReplaceTabPage.Controls.Add(this.ReplaceIsRegexCheckbox);
            this.ReplaceTabPage.Controls.Add(this.ReplaceIsMultilineCheckBox);
            this.ReplaceTabPage.Controls.Add(this.ReplaceIsSinglelineCheckbox);
            this.ReplaceTabPage.Controls.Add(this.label5);
            this.ReplaceTabPage.Controls.Add(this.NumberOfTimesUpDown);
            this.ReplaceTabPage.Controls.Add(this.TestFind);
            resources.ApplyResources(this.ReplaceTabPage, "ReplaceTabPage");
            this.ReplaceTabPage.Name = "ReplaceTabPage";
            // 
            // ReplaceSplit
            // 
            resources.ApplyResources(this.ReplaceSplit, "ReplaceSplit");
            this.ReplaceSplit.Name = "ReplaceSplit";
            // 
            // ReplaceSplit.Panel1
            // 
            this.ReplaceSplit.Panel1.Controls.Add(this.ReplaceTextbox);
            // 
            // ReplaceSplit.Panel2
            // 
            this.ReplaceSplit.Panel2.Controls.Add(this.label3);
            this.ReplaceSplit.Panel2.Controls.Add(this.WithTextbox);
            // 
            // ReplaceTextbox
            // 
            resources.ApplyResources(this.ReplaceTextbox, "ReplaceTextbox");
            this.ReplaceTextbox.Name = "ReplaceTextbox";
            this.ReplaceTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReplaceTextbox_KeyDown);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // WithTextbox
            // 
            resources.ApplyResources(this.WithTextbox, "WithTextbox");
            this.WithTextbox.Name = "WithTextbox";
            this.WithTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WithTextbox_KeyDown);
            // 
            // ReplaceIsCaseSensitiveCheckBox
            // 
            resources.ApplyResources(this.ReplaceIsCaseSensitiveCheckBox, "ReplaceIsCaseSensitiveCheckBox");
            this.ReplaceIsCaseSensitiveCheckBox.Name = "ReplaceIsCaseSensitiveCheckBox";
            this.ReplaceIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReplaceIsRegexCheckbox
            // 
            resources.ApplyResources(this.ReplaceIsRegexCheckbox, "ReplaceIsRegexCheckbox");
            this.ReplaceIsRegexCheckbox.Name = "ReplaceIsRegexCheckbox";
            this.ReplaceIsRegexCheckbox.CheckedChanged += new System.EventHandler(this.ReplaceIsRegexCheckbox_CheckedChanged);
            // 
            // ReplaceIsMultilineCheckBox
            // 
            resources.ApplyResources(this.ReplaceIsMultilineCheckBox, "ReplaceIsMultilineCheckBox");
            this.ReplaceIsMultilineCheckBox.Name = "ReplaceIsMultilineCheckBox";
            this.ReplaceIsMultilineCheckBox.UseVisualStyleBackColor = true;
            // 
            // ReplaceIsSinglelineCheckbox
            // 
            resources.ApplyResources(this.ReplaceIsSinglelineCheckbox, "ReplaceIsSinglelineCheckbox");
            this.ReplaceIsSinglelineCheckbox.Name = "ReplaceIsSinglelineCheckbox";
            this.ReplaceIsSinglelineCheckbox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // NumberOfTimesUpDown
            // 
            resources.ApplyResources(this.NumberOfTimesUpDown, "NumberOfTimesUpDown");
            this.NumberOfTimesUpDown.Name = "NumberOfTimesUpDown";
            this.NumberOfTimesUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TestFind
            // 
            resources.ApplyResources(this.TestFind, "TestFind");
            this.TestFind.BackColor = System.Drawing.SystemColors.Control;
            this.TestFind.Name = "TestFind";
            this.TestFind.UseVisualStyleBackColor = false;
            this.TestFind.Click += new System.EventHandler(this.TestFind_Click);
            // 
            // IfTabPage
            // 
            this.IfTabPage.BackColor = System.Drawing.Color.White;
            this.IfTabPage.Controls.Add(this.IfSplit);
            this.IfTabPage.Controls.Add(this.IfIsSinglelineCheckBox);
            this.IfTabPage.Controls.Add(this.IfIsMultilineCheckbox);
            this.IfTabPage.Controls.Add(this.IfIsCaseSensitiveCheckBox);
            this.IfTabPage.Controls.Add(this.IfIsRegexCheckBox);
            this.IfTabPage.Controls.Add(this.TestIf);
            resources.ApplyResources(this.IfTabPage, "IfTabPage");
            this.IfTabPage.Name = "IfTabPage";
            // 
            // IfSplit
            // 
            resources.ApplyResources(this.IfSplit, "IfSplit");
            this.IfSplit.Name = "IfSplit";
            // 
            // IfSplit.Panel1
            // 
            this.IfSplit.Panel1.Controls.Add(this.HasMatchLabel);
            this.IfSplit.Panel1.Controls.Add(this.IfContainsTextBox);
            // 
            // IfSplit.Panel2
            // 
            this.IfSplit.Panel2.Controls.Add(this.IfNotContainsLabel);
            this.IfSplit.Panel2.Controls.Add(this.IfNotContainsTextBox);
            // 
            // HasMatchLabel
            // 
            resources.ApplyResources(this.HasMatchLabel, "HasMatchLabel");
            this.HasMatchLabel.Name = "HasMatchLabel";
            // 
            // IfContainsTextBox
            // 
            resources.ApplyResources(this.IfContainsTextBox, "IfContainsTextBox");
            this.IfContainsTextBox.Name = "IfContainsTextBox";
            this.IfContainsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.IfContainsTextBox_KeyDown);
            // 
            // IfNotContainsLabel
            // 
            resources.ApplyResources(this.IfNotContainsLabel, "IfNotContainsLabel");
            this.IfNotContainsLabel.Name = "IfNotContainsLabel";
            // 
            // IfNotContainsTextBox
            // 
            resources.ApplyResources(this.IfNotContainsTextBox, "IfNotContainsTextBox");
            this.IfNotContainsTextBox.Name = "IfNotContainsTextBox";
            this.IfNotContainsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.IfNotContainsTextBox_KeyDown);
            // 
            // IfIsSinglelineCheckBox
            // 
            resources.ApplyResources(this.IfIsSinglelineCheckBox, "IfIsSinglelineCheckBox");
            this.IfIsSinglelineCheckBox.Name = "IfIsSinglelineCheckBox";
            this.IfIsSinglelineCheckBox.UseVisualStyleBackColor = true;
            // 
            // IfIsMultilineCheckbox
            // 
            resources.ApplyResources(this.IfIsMultilineCheckbox, "IfIsMultilineCheckbox");
            this.IfIsMultilineCheckbox.Name = "IfIsMultilineCheckbox";
            this.IfIsMultilineCheckbox.UseVisualStyleBackColor = true;
            // 
            // IfIsCaseSensitiveCheckBox
            // 
            resources.ApplyResources(this.IfIsCaseSensitiveCheckBox, "IfIsCaseSensitiveCheckBox");
            this.IfIsCaseSensitiveCheckBox.Name = "IfIsCaseSensitiveCheckBox";
            this.IfIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // IfIsRegexCheckBox
            // 
            resources.ApplyResources(this.IfIsRegexCheckBox, "IfIsRegexCheckBox");
            this.IfIsRegexCheckBox.Name = "IfIsRegexCheckBox";
            this.IfIsRegexCheckBox.UseVisualStyleBackColor = true;
            this.IfIsRegexCheckBox.CheckedChanged += new System.EventHandler(this.IfIsRegexCheckBox_CheckedChanged);
            // 
            // TestIf
            // 
            resources.ApplyResources(this.TestIf, "TestIf");
            this.TestIf.BackColor = System.Drawing.SystemColors.Control;
            this.TestIf.Name = "TestIf";
            this.TestIf.UseVisualStyleBackColor = false;
            this.TestIf.Click += new System.EventHandler(this.TestIf_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NameTextbox
            // 
            resources.ApplyResources(this.NameTextbox, "NameTextbox");
            this.NameTextbox.Name = "NameTextbox";
            this.NameTextbox.TextChanged += new System.EventHandler(this.NameTextbox_TextChanged);
            this.NameTextbox.DoubleClick += new System.EventHandler(this.NameTextbox_DoubleClick);
            this.NameTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NameTextbox_KeyDown);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // RuleTypeCombobox
            // 
            resources.ApplyResources(this.RuleTypeCombobox, "RuleTypeCombobox");
            this.RuleTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RuleTypeCombobox.FormattingEnabled = true;
            this.RuleTypeCombobox.Items.AddRange(new object[] {
            resources.GetString("RuleTypeCombobox.Items"),
            resources.GetString("RuleTypeCombobox.Items1")});
            this.RuleTypeCombobox.Name = "RuleTypeCombobox";
            // 
            // RuleEnabledCheckBox
            // 
            resources.ApplyResources(this.RuleEnabledCheckBox, "RuleEnabledCheckBox");
            this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
            this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testIfContainsToolStripMenuItem,
            this.testIfNotContainsToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            resources.ApplyResources(this.contextMenu, "contextMenu");
            // 
            // testIfContainsToolStripMenuItem
            // 
            this.testIfContainsToolStripMenuItem.Name = "testIfContainsToolStripMenuItem";
            resources.ApplyResources(this.testIfContainsToolStripMenuItem, "testIfContainsToolStripMenuItem");
            this.testIfContainsToolStripMenuItem.Click += new System.EventHandler(this.testIfContainsToolStripMenuItem_Click);
            // 
            // testIfNotContainsToolStripMenuItem
            // 
            this.testIfNotContainsToolStripMenuItem.Name = "testIfNotContainsToolStripMenuItem";
            resources.ApplyResources(this.testIfNotContainsToolStripMenuItem, "testIfNotContainsToolStripMenuItem");
            this.testIfNotContainsToolStripMenuItem.Click += new System.EventHandler(this.testIfNotContainsToolStripMenuItem_Click);
            // 
            // RuleControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RuleGroupBox);
            this.DoubleBuffered = true;
            this.Name = "RuleControl";
            this.RuleGroupBox.ResumeLayout(false);
            this.RuleGroupBox.PerformLayout();
            this.RuleTabControl.ResumeLayout(false);
            this.ReplaceTabPage.ResumeLayout(false);
            this.ReplaceTabPage.PerformLayout();
            this.ReplaceSplit.Panel1.ResumeLayout(false);
            this.ReplaceSplit.Panel1.PerformLayout();
            this.ReplaceSplit.Panel2.ResumeLayout(false);
            this.ReplaceSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ReplaceSplit)).EndInit();
            this.ReplaceSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).EndInit();
            this.IfTabPage.ResumeLayout(false);
            this.IfTabPage.PerformLayout();
            this.IfSplit.Panel1.ResumeLayout(false);
            this.IfSplit.Panel1.PerformLayout();
            this.IfSplit.Panel2.ResumeLayout(false);
            this.IfSplit.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IfSplit)).EndInit();
            this.IfSplit.ResumeLayout(false);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox RuleGroupBox;
    private System.Windows.Forms.TabControl RuleTabControl;
    private System.Windows.Forms.TabPage ReplaceTabPage;
    private System.Windows.Forms.NumericUpDown NumberOfTimesUpDown;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox ReplaceTextbox;
    private System.Windows.Forms.CheckBox ReplaceIsSinglelineCheckbox;
    private System.Windows.Forms.TextBox WithTextbox;
    private System.Windows.Forms.CheckBox ReplaceIsMultilineCheckBox;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox ReplaceIsCaseSensitiveCheckBox;
    private System.Windows.Forms.CheckBox ReplaceIsRegexCheckbox;
    private System.Windows.Forms.TabPage IfTabPage;
    private System.Windows.Forms.CheckBox IfIsSinglelineCheckBox;
    private System.Windows.Forms.CheckBox IfIsMultilineCheckbox;
    private System.Windows.Forms.CheckBox IfIsCaseSensitiveCheckBox;
    private System.Windows.Forms.CheckBox IfIsRegexCheckBox;
    private System.Windows.Forms.Label IfNotContainsLabel;
    private System.Windows.Forms.TextBox IfNotContainsTextBox;
    private System.Windows.Forms.Label HasMatchLabel;
    private System.Windows.Forms.TextBox IfContainsTextBox;
    private System.Windows.Forms.CheckBox RuleEnabledCheckBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox NameTextbox;
    private System.Windows.Forms.ComboBox RuleTypeCombobox;
      private System.Windows.Forms.Button TestFind;
      private System.Windows.Forms.Button TestIf;
      private System.Windows.Forms.ContextMenuStrip contextMenu;
      private System.Windows.Forms.ToolStripMenuItem testIfContainsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem testIfNotContainsToolStripMenuItem;
      private System.Windows.Forms.SplitContainer IfSplit;
      private System.Windows.Forms.SplitContainer ReplaceSplit;
  }
}
