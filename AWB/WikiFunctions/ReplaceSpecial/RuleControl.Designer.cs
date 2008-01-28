namespace WikiFunctions.MWB
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
        this.RuleGroupBox = new System.Windows.Forms.GroupBox();
        this.RuleTabControl = new System.Windows.Forms.TabControl();
        this.ReplaceTabPage = new System.Windows.Forms.TabPage();
        this.NumberOfTimesUpDown = new System.Windows.Forms.NumericUpDown();
        this.label5 = new System.Windows.Forms.Label();
        this.ReplaceTextbox = new System.Windows.Forms.TextBox();
        this.ReplaceIsSinglelineCheckbox = new System.Windows.Forms.CheckBox();
        this.WithTextbox = new System.Windows.Forms.TextBox();
        this.ReplaceIsMultilineCheckBox = new System.Windows.Forms.CheckBox();
        this.label3 = new System.Windows.Forms.Label();
        this.ReplaceIsCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
        this.ReplaceIsRegexCheckbox = new System.Windows.Forms.CheckBox();
        this.IfTabPage = new System.Windows.Forms.TabPage();
        this.IfIsSinglelineCheckBox = new System.Windows.Forms.CheckBox();
        this.IfIsMultilineCheckbox = new System.Windows.Forms.CheckBox();
        this.IfIsCaseSensitiveCheckBox = new System.Windows.Forms.CheckBox();
        this.IfIsRegexCheckBox = new System.Windows.Forms.CheckBox();
        this.IfNotContainsLabel = new System.Windows.Forms.Label();
        this.IfNotContainsTextBox = new System.Windows.Forms.TextBox();
        this.HasMatchLabel = new System.Windows.Forms.Label();
        this.IfContainsTextBox = new System.Windows.Forms.TextBox();
        this.RuleEnabledCheckBox = new System.Windows.Forms.CheckBox();
        this.label4 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.NameTextbox = new System.Windows.Forms.TextBox();
        this.RuleTypeCombobox = new System.Windows.Forms.ComboBox();
        this.RuleGroupBox.SuspendLayout();
        this.RuleTabControl.SuspendLayout();
        this.ReplaceTabPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).BeginInit();
        this.IfTabPage.SuspendLayout();
        this.SuspendLayout();
        // 
        // RuleGroupBox
        // 
        this.RuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.RuleGroupBox.Controls.Add(this.RuleTabControl);
        this.RuleGroupBox.Controls.Add(this.RuleEnabledCheckBox);
        this.RuleGroupBox.Controls.Add(this.label4);
        this.RuleGroupBox.Controls.Add(this.label1);
        this.RuleGroupBox.Controls.Add(this.NameTextbox);
        this.RuleGroupBox.Controls.Add(this.RuleTypeCombobox);
        this.RuleGroupBox.Location = new System.Drawing.Point(3, 3);
        this.RuleGroupBox.Name = "RuleGroupBox";
        this.RuleGroupBox.Size = new System.Drawing.Size(426, 405);
        this.RuleGroupBox.TabIndex = 10;
        this.RuleGroupBox.TabStop = false;
        this.RuleGroupBox.Text = "Rule";
        // 
        // RuleTabControl
        // 
        this.RuleTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleTabControl.Controls.Add(this.ReplaceTabPage);
        this.RuleTabControl.Controls.Add(this.IfTabPage);
        this.RuleTabControl.Location = new System.Drawing.Point(10, 72);
        this.RuleTabControl.Name = "RuleTabControl";
        this.RuleTabControl.SelectedIndex = 0;
        this.RuleTabControl.Size = new System.Drawing.Size(409, 327);
        this.RuleTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
        this.RuleTabControl.TabIndex = 17;
        // 
        // ReplaceTabPage
        // 
        this.ReplaceTabPage.Controls.Add(this.NumberOfTimesUpDown);
        this.ReplaceTabPage.Controls.Add(this.label5);
        this.ReplaceTabPage.Controls.Add(this.ReplaceTextbox);
        this.ReplaceTabPage.Controls.Add(this.ReplaceIsSinglelineCheckbox);
        this.ReplaceTabPage.Controls.Add(this.WithTextbox);
        this.ReplaceTabPage.Controls.Add(this.ReplaceIsMultilineCheckBox);
        this.ReplaceTabPage.Controls.Add(this.label3);
        this.ReplaceTabPage.Controls.Add(this.ReplaceIsCaseSensitiveCheckBox);
        this.ReplaceTabPage.Controls.Add(this.ReplaceIsRegexCheckbox);
        this.ReplaceTabPage.Location = new System.Drawing.Point(4, 22);
        this.ReplaceTabPage.Name = "ReplaceTabPage";
        this.ReplaceTabPage.Padding = new System.Windows.Forms.Padding(3);
        this.ReplaceTabPage.Size = new System.Drawing.Size(401, 301);
        this.ReplaceTabPage.TabIndex = 0;
        this.ReplaceTabPage.Text = "Find";
        this.ReplaceTabPage.UseVisualStyleBackColor = true;
        // 
        // NumberOfTimesUpDown
        // 
        this.NumberOfTimesUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.NumberOfTimesUpDown.Location = new System.Drawing.Point(247, 267);
        this.NumberOfTimesUpDown.Name = "NumberOfTimesUpDown";
        this.NumberOfTimesUpDown.Size = new System.Drawing.Size(79, 20);
        this.NumberOfTimesUpDown.TabIndex = 17;
        this.NumberOfTimesUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
        // 
        // label5
        // 
        this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(244, 245);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(82, 13);
        this.label5.TabIndex = 16;
        this.label5.Text = "Apply # of times";
        // 
        // ReplaceTextbox
        // 
        this.ReplaceTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ReplaceTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ReplaceTextbox.Location = new System.Drawing.Point(6, 6);
        this.ReplaceTextbox.Multiline = true;
        this.ReplaceTextbox.Name = "ReplaceTextbox";
        this.ReplaceTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.ReplaceTextbox.Size = new System.Drawing.Size(392, 109);
        this.ReplaceTextbox.TabIndex = 6;
        this.ReplaceTextbox.WordWrap = false;
        // 
        // ReplaceIsSinglelineCheckbox
        // 
        this.ReplaceIsSinglelineCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsSinglelineCheckbox.AutoSize = true;
        this.ReplaceIsSinglelineCheckbox.Location = new System.Drawing.Point(128, 267);
        this.ReplaceIsSinglelineCheckbox.Name = "ReplaceIsSinglelineCheckbox";
        this.ReplaceIsSinglelineCheckbox.Size = new System.Drawing.Size(71, 17);
        this.ReplaceIsSinglelineCheckbox.TabIndex = 14;
        this.ReplaceIsSinglelineCheckbox.Text = "Singleline";
        this.ReplaceIsSinglelineCheckbox.UseVisualStyleBackColor = true;
        // 
        // WithTextbox
        // 
        this.WithTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.WithTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WithTextbox.Location = new System.Drawing.Point(5, 134);
        this.WithTextbox.Multiline = true;
        this.WithTextbox.Name = "WithTextbox";
        this.WithTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.WithTextbox.Size = new System.Drawing.Size(393, 104);
        this.WithTextbox.TabIndex = 6;
        this.WithTextbox.WordWrap = false;
        // 
        // ReplaceIsMultilineCheckBox
        // 
        this.ReplaceIsMultilineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsMultilineCheckBox.AutoSize = true;
        this.ReplaceIsMultilineCheckBox.Location = new System.Drawing.Point(128, 244);
        this.ReplaceIsMultilineCheckBox.Name = "ReplaceIsMultilineCheckBox";
        this.ReplaceIsMultilineCheckBox.Size = new System.Drawing.Size(64, 17);
        this.ReplaceIsMultilineCheckBox.TabIndex = 13;
        this.ReplaceIsMultilineCheckBox.Text = "Multiline";
        this.ReplaceIsMultilineCheckBox.UseVisualStyleBackColor = true;
        // 
        // label3
        // 
        this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label3.Location = new System.Drawing.Point(6, 118);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(78, 13);
        this.label3.TabIndex = 8;
        this.label3.Text = "Replace with:";
        // 
        // ReplaceIsCaseSensitiveCheckBox
        // 
        this.ReplaceIsCaseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsCaseSensitiveCheckBox.AutoSize = true;
        this.ReplaceIsCaseSensitiveCheckBox.Location = new System.Drawing.Point(6, 267);
        this.ReplaceIsCaseSensitiveCheckBox.Name = "ReplaceIsCaseSensitiveCheckBox";
        this.ReplaceIsCaseSensitiveCheckBox.Size = new System.Drawing.Size(94, 17);
        this.ReplaceIsCaseSensitiveCheckBox.TabIndex = 12;
        this.ReplaceIsCaseSensitiveCheckBox.Text = "Case sensitive";
        this.ReplaceIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
        // 
        // ReplaceIsRegexCheckbox
        // 
        this.ReplaceIsRegexCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsRegexCheckbox.AutoSize = true;
        this.ReplaceIsRegexCheckbox.Location = new System.Drawing.Point(6, 244);
        this.ReplaceIsRegexCheckbox.Name = "ReplaceIsRegexCheckbox";
        this.ReplaceIsRegexCheckbox.Size = new System.Drawing.Size(116, 17);
        this.ReplaceIsRegexCheckbox.TabIndex = 11;
        this.ReplaceIsRegexCheckbox.Text = "Regular expression";
        this.ReplaceIsRegexCheckbox.UseVisualStyleBackColor = true;
        this.ReplaceIsRegexCheckbox.CheckedChanged += new System.EventHandler(this.ReplaceIsRegexCheckbox_CheckedChanged);
        // 
        // IfTabPage
        // 
        this.IfTabPage.Controls.Add(this.IfIsSinglelineCheckBox);
        this.IfTabPage.Controls.Add(this.IfIsMultilineCheckbox);
        this.IfTabPage.Controls.Add(this.IfIsCaseSensitiveCheckBox);
        this.IfTabPage.Controls.Add(this.IfIsRegexCheckBox);
        this.IfTabPage.Controls.Add(this.IfNotContainsLabel);
        this.IfTabPage.Controls.Add(this.IfNotContainsTextBox);
        this.IfTabPage.Controls.Add(this.HasMatchLabel);
        this.IfTabPage.Controls.Add(this.IfContainsTextBox);
        this.IfTabPage.Location = new System.Drawing.Point(4, 22);
        this.IfTabPage.Name = "IfTabPage";
        this.IfTabPage.Padding = new System.Windows.Forms.Padding(3);
        this.IfTabPage.Size = new System.Drawing.Size(401, 301);
        this.IfTabPage.TabIndex = 1;
        this.IfTabPage.Text = "If";
        this.IfTabPage.UseVisualStyleBackColor = true;
        // 
        // IfIsSinglelineCheckBox
        // 
        this.IfIsSinglelineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsSinglelineCheckBox.AutoSize = true;
        this.IfIsSinglelineCheckBox.Location = new System.Drawing.Point(128, 278);
        this.IfIsSinglelineCheckBox.Name = "IfIsSinglelineCheckBox";
        this.IfIsSinglelineCheckBox.Size = new System.Drawing.Size(71, 17);
        this.IfIsSinglelineCheckBox.TabIndex = 15;
        this.IfIsSinglelineCheckBox.Text = "Singleline";
        this.IfIsSinglelineCheckBox.UseVisualStyleBackColor = true;
        // 
        // IfIsMultilineCheckbox
        // 
        this.IfIsMultilineCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsMultilineCheckbox.AutoSize = true;
        this.IfIsMultilineCheckbox.Location = new System.Drawing.Point(128, 255);
        this.IfIsMultilineCheckbox.Name = "IfIsMultilineCheckbox";
        this.IfIsMultilineCheckbox.Size = new System.Drawing.Size(64, 17);
        this.IfIsMultilineCheckbox.TabIndex = 14;
        this.IfIsMultilineCheckbox.Text = "Multiline";
        this.IfIsMultilineCheckbox.UseVisualStyleBackColor = true;
        // 
        // IfIsCaseSensitiveCheckBox
        // 
        this.IfIsCaseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsCaseSensitiveCheckBox.AutoSize = true;
        this.IfIsCaseSensitiveCheckBox.Location = new System.Drawing.Point(6, 278);
        this.IfIsCaseSensitiveCheckBox.Name = "IfIsCaseSensitiveCheckBox";
        this.IfIsCaseSensitiveCheckBox.Size = new System.Drawing.Size(94, 17);
        this.IfIsCaseSensitiveCheckBox.TabIndex = 13;
        this.IfIsCaseSensitiveCheckBox.Text = "Case sensitive";
        this.IfIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
        // 
        // IfIsRegexCheckBox
        // 
        this.IfIsRegexCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsRegexCheckBox.AutoSize = true;
        this.IfIsRegexCheckBox.Location = new System.Drawing.Point(6, 255);
        this.IfIsRegexCheckBox.Name = "IfIsRegexCheckBox";
        this.IfIsRegexCheckBox.Size = new System.Drawing.Size(116, 17);
        this.IfIsRegexCheckBox.TabIndex = 12;
        this.IfIsRegexCheckBox.Text = "Regular expression";
        this.IfIsRegexCheckBox.UseVisualStyleBackColor = true;
        this.IfIsRegexCheckBox.CheckedChanged += new System.EventHandler(this.IfIsRegexCheckBox_CheckedChanged);
        // 
        // IfNotContainsLabel
        // 
        this.IfNotContainsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfNotContainsLabel.Location = new System.Drawing.Point(6, 139);
        this.IfNotContainsLabel.Name = "IfNotContainsLabel";
        this.IfNotContainsLabel.Size = new System.Drawing.Size(48, 31);
        this.IfNotContainsLabel.TabIndex = 3;
        this.IfNotContainsLabel.Text = "Not contains";
        // 
        // IfNotContainsTextBox
        // 
        this.IfNotContainsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.IfNotContainsTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.IfNotContainsTextBox.Location = new System.Drawing.Point(60, 139);
        this.IfNotContainsTextBox.Multiline = true;
        this.IfNotContainsTextBox.Name = "IfNotContainsTextBox";
        this.IfNotContainsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.IfNotContainsTextBox.Size = new System.Drawing.Size(335, 110);
        this.IfNotContainsTextBox.TabIndex = 2;
        this.IfNotContainsTextBox.WordWrap = false;
        // 
        // HasMatchLabel
        // 
        this.HasMatchLabel.AutoSize = true;
        this.HasMatchLabel.Location = new System.Drawing.Point(6, 12);
        this.HasMatchLabel.Name = "HasMatchLabel";
        this.HasMatchLabel.Size = new System.Drawing.Size(48, 13);
        this.HasMatchLabel.TabIndex = 1;
        this.HasMatchLabel.Text = "Contains";
        // 
        // IfContainsTextBox
        // 
        this.IfContainsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.IfContainsTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.IfContainsTextBox.Location = new System.Drawing.Point(60, 12);
        this.IfContainsTextBox.Multiline = true;
        this.IfContainsTextBox.Name = "IfContainsTextBox";
        this.IfContainsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.IfContainsTextBox.Size = new System.Drawing.Size(335, 121);
        this.IfContainsTextBox.TabIndex = 0;
        this.IfContainsTextBox.WordWrap = false;
        // 
        // RuleEnabledCheckBox
        // 
        this.RuleEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleEnabledCheckBox.AutoSize = true;
        this.RuleEnabledCheckBox.Location = new System.Drawing.Point(355, 22);
        this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
        this.RuleEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
        this.RuleEnabledCheckBox.TabIndex = 10;
        this.RuleEnabledCheckBox.Text = "Enabled";
        this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(7, 49);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(31, 13);
        this.label4.TabIndex = 9;
        this.label4.Text = "Type";
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(3, 22);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(35, 13);
        this.label1.TabIndex = 2;
        this.label1.Text = "Name";
        // 
        // NameTextbox
        // 
        this.NameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.NameTextbox.Location = new System.Drawing.Point(47, 19);
        this.NameTextbox.Name = "NameTextbox";
        this.NameTextbox.Size = new System.Drawing.Size(302, 20);
        this.NameTextbox.TabIndex = 3;
        this.NameTextbox.DoubleClick += new System.EventHandler(this.NameTextbox_DoubleClick);
        this.NameTextbox.TextChanged += new System.EventHandler(this.NameTextbox_TextChanged);
        // 
        // RuleTypeCombobox
        // 
        this.RuleTypeCombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.RuleTypeCombobox.FormattingEnabled = true;
        this.RuleTypeCombobox.Items.AddRange(new object[] {
            "Entire text",
            "Inside template calls {{..}}"});
        this.RuleTypeCombobox.Location = new System.Drawing.Point(47, 45);
        this.RuleTypeCombobox.Name = "RuleTypeCombobox";
        this.RuleTypeCombobox.Size = new System.Drawing.Size(302, 21);
        this.RuleTypeCombobox.TabIndex = 5;
        // 
        // RuleControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.Controls.Add(this.RuleGroupBox);
        this.DoubleBuffered = true;
        this.Name = "RuleControl";
        this.Size = new System.Drawing.Size(435, 415);
        this.RuleGroupBox.ResumeLayout(false);
        this.RuleGroupBox.PerformLayout();
        this.RuleTabControl.ResumeLayout(false);
        this.ReplaceTabPage.ResumeLayout(false);
        this.ReplaceTabPage.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).EndInit();
        this.IfTabPage.ResumeLayout(false);
        this.IfTabPage.PerformLayout();
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
  }
}
