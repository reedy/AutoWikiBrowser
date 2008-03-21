namespace WikiFunctions.MWB
{
  partial class InTemplateRuleControl
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
        this.label1 = new System.Windows.Forms.Label();
        this.NameTextbox = new System.Windows.Forms.TextBox();
        this.label2 = new System.Windows.Forms.Label();
        this.AliasesListBox = new System.Windows.Forms.ListBox();
        this.AddButton = new System.Windows.Forms.Button();
        this.AliasTextBox = new System.Windows.Forms.TextBox();
        this.DeleteButton = new System.Windows.Forms.Button();
        this.ReplaceCheckBox = new System.Windows.Forms.CheckBox();
        this.ReplaceWithTextBox = new System.Windows.Forms.TextBox();
        this.RuleEnabledCheckBox = new System.Windows.Forms.CheckBox();
        this.RuleGroupBox.SuspendLayout();
        this.SuspendLayout();
        // 
        // RuleGroupBox
        // 
        this.RuleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.RuleGroupBox.Controls.Add(this.label1);
        this.RuleGroupBox.Controls.Add(this.NameTextbox);
        this.RuleGroupBox.Controls.Add(this.label2);
        this.RuleGroupBox.Controls.Add(this.AliasTextBox);
        this.RuleGroupBox.Controls.Add(this.AddButton);
        this.RuleGroupBox.Controls.Add(this.AliasesListBox);
        this.RuleGroupBox.Controls.Add(this.DeleteButton);
        this.RuleGroupBox.Controls.Add(this.ReplaceCheckBox);
        this.RuleGroupBox.Controls.Add(this.ReplaceWithTextBox);
        this.RuleGroupBox.Controls.Add(this.RuleEnabledCheckBox);
        this.RuleGroupBox.Location = new System.Drawing.Point(3, 3);
        this.RuleGroupBox.Name = "RuleGroupBox";
        this.RuleGroupBox.Size = new System.Drawing.Size(426, 405);
        this.RuleGroupBox.TabIndex = 11;
        this.RuleGroupBox.TabStop = false;
        this.RuleGroupBox.Text = "In template call";
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(6, 22);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(35, 13);
        this.label1.TabIndex = 0;
        this.label1.Text = "Name";
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
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(20, 60);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(59, 13);
        this.label2.TabIndex = 2;
        this.label2.Text = "&Templates:";
        // 
        // AliasesListBox
        // 
        this.AliasesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.AliasesListBox.FormattingEnabled = true;
        this.AliasesListBox.Location = new System.Drawing.Point(23, 102);
        this.AliasesListBox.Name = "AliasesListBox";
        this.AliasesListBox.Size = new System.Drawing.Size(326, 238);
        this.AliasesListBox.Sorted = true;
        this.AliasesListBox.TabIndex = 5;
        this.AliasesListBox.SelectedIndexChanged += new System.EventHandler(this.AliasesListBox_SelectedIndexChanged);
        // 
        // AddButton
        // 
        this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.AddButton.Location = new System.Drawing.Point(355, 74);
        this.AddButton.Name = "AddButton";
        this.AddButton.Size = new System.Drawing.Size(65, 23);
        this.AddButton.TabIndex = 4;
        this.AddButton.Text = "&Add";
        this.AddButton.UseVisualStyleBackColor = true;
        this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
        // 
        // AliasTextBox
        // 
        this.AliasTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.AliasTextBox.Location = new System.Drawing.Point(23, 76);
        this.AliasTextBox.Name = "AliasTextBox";
        this.AliasTextBox.Size = new System.Drawing.Size(326, 20);
        this.AliasTextBox.TabIndex = 3;
        // 
        // DeleteButton
        // 
        this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.DeleteButton.Location = new System.Drawing.Point(355, 103);
        this.DeleteButton.Name = "DeleteButton";
        this.DeleteButton.Size = new System.Drawing.Size(65, 23);
        this.DeleteButton.TabIndex = 6;
        this.DeleteButton.Text = "&Remove";
        this.DeleteButton.UseVisualStyleBackColor = true;
        this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
        // 
        // ReplaceCheckBox
        // 
        this.ReplaceCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.ReplaceCheckBox.AutoSize = true;
        this.ReplaceCheckBox.Location = new System.Drawing.Point(23, 346);
        this.ReplaceCheckBox.Name = "ReplaceCheckBox";
        this.ReplaceCheckBox.Size = new System.Drawing.Size(91, 17);
        this.ReplaceCheckBox.TabIndex = 7;
        this.ReplaceCheckBox.Text = "Replace &with:";
        this.ReplaceCheckBox.UseVisualStyleBackColor = true;
        this.ReplaceCheckBox.CheckedChanged += new System.EventHandler(this.ReplaceCheckBox_CheckedChanged);
        // 
        // ReplaceWithTextBox
        // 
        this.ReplaceWithTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ReplaceWithTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ReplaceWithTextBox.Location = new System.Drawing.Point(23, 369);
        this.ReplaceWithTextBox.Name = "ReplaceWithTextBox";
        this.ReplaceWithTextBox.Size = new System.Drawing.Size(326, 21);
        this.ReplaceWithTextBox.TabIndex = 8;
        // 
        // RuleEnabledCheckBox
        // 
        this.RuleEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleEnabledCheckBox.AutoSize = true;
        this.RuleEnabledCheckBox.Location = new System.Drawing.Point(355, 21);
        this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
        this.RuleEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
        this.RuleEnabledCheckBox.TabIndex = 9;
        this.RuleEnabledCheckBox.Text = "&Enabled";
        this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
        // 
        // InTemplateRuleControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.RuleGroupBox);
        this.Name = "InTemplateRuleControl";
        this.Size = new System.Drawing.Size(434, 414);
        this.RuleGroupBox.ResumeLayout(false);
        this.RuleGroupBox.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox RuleGroupBox;
    private System.Windows.Forms.CheckBox RuleEnabledCheckBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox NameTextbox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox ReplaceCheckBox;
    private System.Windows.Forms.TextBox ReplaceWithTextBox;
    private System.Windows.Forms.ListBox AliasesListBox;
    private System.Windows.Forms.TextBox AliasTextBox;
    private System.Windows.Forms.Button AddButton;
    private System.Windows.Forms.Button DeleteButton;
  }
}
