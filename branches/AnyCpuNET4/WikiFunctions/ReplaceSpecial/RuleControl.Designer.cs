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
        this.ReplaceSplit.Panel1.SuspendLayout();
        this.ReplaceSplit.Panel2.SuspendLayout();
        this.ReplaceSplit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).BeginInit();
        this.IfTabPage.SuspendLayout();
        this.IfSplit.Panel1.SuspendLayout();
        this.IfSplit.Panel2.SuspendLayout();
        this.IfSplit.SuspendLayout();
        this.contextMenu.SuspendLayout();
        this.SuspendLayout();
        // 
        // RuleGroupBox
        // 
        this.RuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.RuleGroupBox.Controls.Add(this.RuleTabControl);
        this.RuleGroupBox.Controls.Add(this.label1);
        this.RuleGroupBox.Controls.Add(this.NameTextbox);
        this.RuleGroupBox.Controls.Add(this.label4);
        this.RuleGroupBox.Controls.Add(this.RuleTypeCombobox);
        this.RuleGroupBox.Controls.Add(this.RuleEnabledCheckBox);
        this.RuleGroupBox.Location = new System.Drawing.Point(3, 3);
        this.RuleGroupBox.Name = "RuleGroupBox";
        this.RuleGroupBox.Size = new System.Drawing.Size(426, 405);
        this.RuleGroupBox.TabIndex = 10;
        this.RuleGroupBox.TabStop = false;
        this.RuleGroupBox.Text = "Find and replace";
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
        this.RuleTabControl.TabIndex = 4;
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
        this.ReplaceTabPage.Location = new System.Drawing.Point(4, 22);
        this.ReplaceTabPage.Name = "ReplaceTabPage";
        this.ReplaceTabPage.Padding = new System.Windows.Forms.Padding(3);
        this.ReplaceTabPage.Size = new System.Drawing.Size(401, 301);
        this.ReplaceTabPage.TabIndex = 0;
        this.ReplaceTabPage.Text = "Find";
        // 
        // ReplaceSplit
        // 
        this.ReplaceSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ReplaceSplit.Location = new System.Drawing.Point(0, 0);
        this.ReplaceSplit.Name = "ReplaceSplit";
        this.ReplaceSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
        // 
        // ReplaceSplit.Panel1
        // 
        this.ReplaceSplit.Panel1.Controls.Add(this.ReplaceTextbox);
        // 
        // ReplaceSplit.Panel2
        // 
        this.ReplaceSplit.Panel2.Controls.Add(this.label3);
        this.ReplaceSplit.Panel2.Controls.Add(this.WithTextbox);
        this.ReplaceSplit.Size = new System.Drawing.Size(401, 249);
        this.ReplaceSplit.SplitterDistance = 126;
        this.ReplaceSplit.SplitterWidth = 3;
        this.ReplaceSplit.TabIndex = 0;
        // 
        // ReplaceTextbox
        // 
        this.ReplaceTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ReplaceTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ReplaceTextbox.Location = new System.Drawing.Point(3, 6);
        this.ReplaceTextbox.Multiline = true;
        this.ReplaceTextbox.Name = "ReplaceTextbox";
        this.ReplaceTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.ReplaceTextbox.Size = new System.Drawing.Size(392, 120);
        this.ReplaceTextbox.TabIndex = 0;
        this.ReplaceTextbox.WordWrap = false;
        // 
        // label3
        // 
        this.label3.Location = new System.Drawing.Point(6, 0);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(100, 13);
        this.label3.TabIndex = 0;
        this.label3.Text = "Replace &with:";
        // 
        // WithTextbox
        // 
        this.WithTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.WithTextbox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.WithTextbox.Location = new System.Drawing.Point(3, 15);
        this.WithTextbox.Multiline = true;
        this.WithTextbox.Name = "WithTextbox";
        this.WithTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.WithTextbox.Size = new System.Drawing.Size(393, 105);
        this.WithTextbox.TabIndex = 1;
        this.WithTextbox.WordWrap = false;
        // 
        // ReplaceIsCaseSensitiveCheckBox
        // 
        this.ReplaceIsCaseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsCaseSensitiveCheckBox.AutoSize = true;
        this.ReplaceIsCaseSensitiveCheckBox.Location = new System.Drawing.Point(3, 278);
        this.ReplaceIsCaseSensitiveCheckBox.Name = "ReplaceIsCaseSensitiveCheckBox";
        this.ReplaceIsCaseSensitiveCheckBox.Size = new System.Drawing.Size(94, 17);
        this.ReplaceIsCaseSensitiveCheckBox.TabIndex = 2;
        this.ReplaceIsCaseSensitiveCheckBox.Text = "Case sens&itive";
        this.ReplaceIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
        // 
        // ReplaceIsRegexCheckbox
        // 
        this.ReplaceIsRegexCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsRegexCheckbox.AutoSize = true;
        this.ReplaceIsRegexCheckbox.Location = new System.Drawing.Point(3, 255);
        this.ReplaceIsRegexCheckbox.Name = "ReplaceIsRegexCheckbox";
        this.ReplaceIsRegexCheckbox.Size = new System.Drawing.Size(116, 17);
        this.ReplaceIsRegexCheckbox.TabIndex = 1;
        this.ReplaceIsRegexCheckbox.Text = "&Regular expression";
        this.ReplaceIsRegexCheckbox.CheckedChanged += new System.EventHandler(this.ReplaceIsRegexCheckbox_CheckedChanged);
        // 
        // ReplaceIsMultilineCheckBox
        // 
        this.ReplaceIsMultilineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsMultilineCheckBox.AutoSize = true;
        this.ReplaceIsMultilineCheckBox.Location = new System.Drawing.Point(125, 255);
        this.ReplaceIsMultilineCheckBox.Name = "ReplaceIsMultilineCheckBox";
        this.ReplaceIsMultilineCheckBox.Size = new System.Drawing.Size(64, 17);
        this.ReplaceIsMultilineCheckBox.TabIndex = 3;
        this.ReplaceIsMultilineCheckBox.Text = "&Multiline";
        this.ReplaceIsMultilineCheckBox.UseVisualStyleBackColor = true;
        // 
        // ReplaceIsSinglelineCheckbox
        // 
        this.ReplaceIsSinglelineCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceIsSinglelineCheckbox.AutoSize = true;
        this.ReplaceIsSinglelineCheckbox.Location = new System.Drawing.Point(125, 278);
        this.ReplaceIsSinglelineCheckbox.Name = "ReplaceIsSinglelineCheckbox";
        this.ReplaceIsSinglelineCheckbox.Size = new System.Drawing.Size(71, 17);
        this.ReplaceIsSinglelineCheckbox.TabIndex = 4;
        this.ReplaceIsSinglelineCheckbox.Text = "&Singleline";
        this.ReplaceIsSinglelineCheckbox.UseVisualStyleBackColor = true;
        // 
        // label5
        // 
        this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(209, 256);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(92, 13);
        this.label5.TabIndex = 5;
        this.label5.Text = "Apply No. of times";
        // 
        // NumberOfTimesUpDown
        // 
        this.NumberOfTimesUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.NumberOfTimesUpDown.Location = new System.Drawing.Point(212, 275);
        this.NumberOfTimesUpDown.Name = "NumberOfTimesUpDown";
        this.NumberOfTimesUpDown.Size = new System.Drawing.Size(79, 20);
        this.NumberOfTimesUpDown.TabIndex = 6;
        this.NumberOfTimesUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
        // 
        // TestFind
        // 
        this.TestFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.TestFind.BackColor = System.Drawing.SystemColors.Control;
        this.TestFind.Location = new System.Drawing.Point(320, 272);
        this.TestFind.Name = "TestFind";
        this.TestFind.Size = new System.Drawing.Size(75, 23);
        this.TestFind.TabIndex = 7;
        this.TestFind.Text = "&Test...";
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
        this.IfTabPage.Location = new System.Drawing.Point(4, 22);
        this.IfTabPage.Name = "IfTabPage";
        this.IfTabPage.Padding = new System.Windows.Forms.Padding(3);
        this.IfTabPage.Size = new System.Drawing.Size(401, 301);
        this.IfTabPage.TabIndex = 1;
        this.IfTabPage.Text = "If";
        // 
        // IfSplit
        // 
        this.IfSplit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.IfSplit.Location = new System.Drawing.Point(0, 0);
        this.IfSplit.Name = "IfSplit";
        this.IfSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
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
        this.IfSplit.Size = new System.Drawing.Size(401, 249);
        this.IfSplit.SplitterDistance = 126;
        this.IfSplit.SplitterWidth = 3;
        this.IfSplit.TabIndex = 0;
        // 
        // HasMatchLabel
        // 
        this.HasMatchLabel.AutoSize = true;
        this.HasMatchLabel.Location = new System.Drawing.Point(6, 3);
        this.HasMatchLabel.Name = "HasMatchLabel";
        this.HasMatchLabel.Size = new System.Drawing.Size(48, 13);
        this.HasMatchLabel.TabIndex = 0;
        this.HasMatchLabel.Text = "&Contains";
        // 
        // IfContainsTextBox
        // 
        this.IfContainsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.IfContainsTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.IfContainsTextBox.Location = new System.Drawing.Point(3, 19);
        this.IfContainsTextBox.Multiline = true;
        this.IfContainsTextBox.Name = "IfContainsTextBox";
        this.IfContainsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.IfContainsTextBox.Size = new System.Drawing.Size(392, 107);
        this.IfContainsTextBox.TabIndex = 1;
        this.IfContainsTextBox.WordWrap = false;
        // 
        // IfNotContainsLabel
        // 
        this.IfNotContainsLabel.Location = new System.Drawing.Point(6, 0);
        this.IfNotContainsLabel.Name = "IfNotContainsLabel";
        this.IfNotContainsLabel.Size = new System.Drawing.Size(100, 13);
        this.IfNotContainsLabel.TabIndex = 0;
        this.IfNotContainsLabel.Text = "&Not contains";
        // 
        // IfNotContainsTextBox
        // 
        this.IfNotContainsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.IfNotContainsTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.IfNotContainsTextBox.Location = new System.Drawing.Point(3, 15);
        this.IfNotContainsTextBox.Multiline = true;
        this.IfNotContainsTextBox.Name = "IfNotContainsTextBox";
        this.IfNotContainsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        this.IfNotContainsTextBox.Size = new System.Drawing.Size(393, 105);
        this.IfNotContainsTextBox.TabIndex = 1;
        this.IfNotContainsTextBox.WordWrap = false;
        // 
        // IfIsSinglelineCheckBox
        // 
        this.IfIsSinglelineCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsSinglelineCheckBox.AutoSize = true;
        this.IfIsSinglelineCheckBox.Location = new System.Drawing.Point(125, 278);
        this.IfIsSinglelineCheckBox.Name = "IfIsSinglelineCheckBox";
        this.IfIsSinglelineCheckBox.Size = new System.Drawing.Size(71, 17);
        this.IfIsSinglelineCheckBox.TabIndex = 4;
        this.IfIsSinglelineCheckBox.Text = "&Singleline";
        this.IfIsSinglelineCheckBox.UseVisualStyleBackColor = true;
        // 
        // IfIsMultilineCheckbox
        // 
        this.IfIsMultilineCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsMultilineCheckbox.AutoSize = true;
        this.IfIsMultilineCheckbox.Location = new System.Drawing.Point(125, 255);
        this.IfIsMultilineCheckbox.Name = "IfIsMultilineCheckbox";
        this.IfIsMultilineCheckbox.Size = new System.Drawing.Size(64, 17);
        this.IfIsMultilineCheckbox.TabIndex = 3;
        this.IfIsMultilineCheckbox.Text = "&Multiline";
        this.IfIsMultilineCheckbox.UseVisualStyleBackColor = true;
        // 
        // IfIsCaseSensitiveCheckBox
        // 
        this.IfIsCaseSensitiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsCaseSensitiveCheckBox.AutoSize = true;
        this.IfIsCaseSensitiveCheckBox.Location = new System.Drawing.Point(3, 278);
        this.IfIsCaseSensitiveCheckBox.Name = "IfIsCaseSensitiveCheckBox";
        this.IfIsCaseSensitiveCheckBox.Size = new System.Drawing.Size(94, 17);
        this.IfIsCaseSensitiveCheckBox.TabIndex = 2;
        this.IfIsCaseSensitiveCheckBox.Text = "Case sens&itive";
        this.IfIsCaseSensitiveCheckBox.UseVisualStyleBackColor = true;
        // 
        // IfIsRegexCheckBox
        // 
        this.IfIsRegexCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.IfIsRegexCheckBox.AutoSize = true;
        this.IfIsRegexCheckBox.Location = new System.Drawing.Point(3, 255);
        this.IfIsRegexCheckBox.Name = "IfIsRegexCheckBox";
        this.IfIsRegexCheckBox.Size = new System.Drawing.Size(116, 17);
        this.IfIsRegexCheckBox.TabIndex = 1;
        this.IfIsRegexCheckBox.Text = "&Regular expression";
        this.IfIsRegexCheckBox.UseVisualStyleBackColor = true;
        this.IfIsRegexCheckBox.CheckedChanged += new System.EventHandler(this.IfIsRegexCheckBox_CheckedChanged);
        // 
        // TestIf
        // 
        this.TestIf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.TestIf.BackColor = System.Drawing.SystemColors.Control;
        this.TestIf.Location = new System.Drawing.Point(320, 272);
        this.TestIf.Name = "TestIf";
        this.TestIf.Size = new System.Drawing.Size(75, 23);
        this.TestIf.TabIndex = 5;
        this.TestIf.Text = "&Test...";
        this.TestIf.UseVisualStyleBackColor = false;
        this.TestIf.Click += new System.EventHandler(this.TestIf_Click);
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(6, 22);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(35, 13);
        this.label1.TabIndex = 0;
        this.label1.Text = "Name";
        this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // NameTextbox
        // 
        this.NameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.NameTextbox.Location = new System.Drawing.Point(47, 19);
        this.NameTextbox.Name = "NameTextbox";
        this.NameTextbox.Size = new System.Drawing.Size(302, 20);
        this.NameTextbox.TabIndex = 1;
        this.NameTextbox.DoubleClick += new System.EventHandler(this.NameTextbox_DoubleClick);
        this.NameTextbox.TextChanged += new System.EventHandler(this.NameTextbox_TextChanged);
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(10, 48);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(31, 13);
        this.label4.TabIndex = 2;
        this.label4.Text = "T&ype";
        this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
        // 
        // RuleTypeCombobox
        // 
        this.RuleTypeCombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleTypeCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.RuleTypeCombobox.FormattingEnabled = true;
        this.RuleTypeCombobox.Items.AddRange(new object[] {
            "Entire text",
            "Inside templates {{..}}"});
        this.RuleTypeCombobox.Location = new System.Drawing.Point(47, 45);
        this.RuleTypeCombobox.Name = "RuleTypeCombobox";
        this.RuleTypeCombobox.Size = new System.Drawing.Size(302, 21);
        this.RuleTypeCombobox.TabIndex = 3;
        // 
        // RuleEnabledCheckBox
        // 
        this.RuleEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleEnabledCheckBox.AutoSize = true;
        this.RuleEnabledCheckBox.Location = new System.Drawing.Point(355, 21);
        this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
        this.RuleEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
        this.RuleEnabledCheckBox.TabIndex = 5;
        this.RuleEnabledCheckBox.Text = "&Enabled";
        this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
        // 
        // contextMenu
        // 
        this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testIfContainsToolStripMenuItem,
            this.testIfNotContainsToolStripMenuItem});
        this.contextMenu.Name = "contextMenu";
        this.contextMenu.Size = new System.Drawing.Size(182, 48);
        // 
        // testIfContainsToolStripMenuItem
        // 
        this.testIfContainsToolStripMenuItem.Name = "testIfContainsToolStripMenuItem";
        this.testIfContainsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
        this.testIfContainsToolStripMenuItem.Text = "Test if contains...";
        this.testIfContainsToolStripMenuItem.Click += new System.EventHandler(this.testIfContainsToolStripMenuItem_Click);
        // 
        // testIfNotContainsToolStripMenuItem
        // 
        this.testIfNotContainsToolStripMenuItem.Name = "testIfNotContainsToolStripMenuItem";
        this.testIfNotContainsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
        this.testIfNotContainsToolStripMenuItem.Text = "Test if not contains..";
        this.testIfNotContainsToolStripMenuItem.Click += new System.EventHandler(this.testIfNotContainsToolStripMenuItem_Click);
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
        this.ReplaceSplit.Panel1.ResumeLayout(false);
        this.ReplaceSplit.Panel1.PerformLayout();
        this.ReplaceSplit.Panel2.ResumeLayout(false);
        this.ReplaceSplit.Panel2.PerformLayout();
        this.ReplaceSplit.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.NumberOfTimesUpDown)).EndInit();
        this.IfTabPage.ResumeLayout(false);
        this.IfTabPage.PerformLayout();
        this.IfSplit.Panel1.ResumeLayout(false);
        this.IfSplit.Panel1.PerformLayout();
        this.IfSplit.Panel2.ResumeLayout(false);
        this.IfSplit.Panel2.PerformLayout();
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
