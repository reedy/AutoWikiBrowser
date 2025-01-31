namespace WikiFunctions.ReplaceSpecial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InTemplateRuleControl));
            this.RuleGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NameTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AliasTextBox = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.AliasesListBox = new System.Windows.Forms.ListBox();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ReplaceCheckBox = new System.Windows.Forms.CheckBox();
            this.ReplaceWithTextBox = new System.Windows.Forms.TextBox();
            this.RuleEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.RuleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // RuleGroupBox
            // 
            resources.ApplyResources(this.RuleGroupBox, "RuleGroupBox");
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
            this.RuleGroupBox.Name = "RuleGroupBox";
            this.RuleGroupBox.TabStop = false;
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
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // AliasTextBox
            // 
            resources.ApplyResources(this.AliasTextBox, "AliasTextBox");
            this.AliasTextBox.Name = "AliasTextBox";
            // 
            // AddButton
            // 
            resources.ApplyResources(this.AddButton, "AddButton");
            this.AddButton.Name = "AddButton";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // AliasesListBox
            // 
            resources.ApplyResources(this.AliasesListBox, "AliasesListBox");
            this.AliasesListBox.FormattingEnabled = true;
            this.AliasesListBox.Name = "AliasesListBox";
            this.AliasesListBox.Sorted = true;
            this.AliasesListBox.SelectedIndexChanged += new System.EventHandler(this.AliasesListBox_SelectedIndexChanged);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ReplaceCheckBox
            // 
            resources.ApplyResources(this.ReplaceCheckBox, "ReplaceCheckBox");
            this.ReplaceCheckBox.Name = "ReplaceCheckBox";
            this.ReplaceCheckBox.UseVisualStyleBackColor = true;
            this.ReplaceCheckBox.CheckedChanged += new System.EventHandler(this.ReplaceCheckBox_CheckedChanged);
            // 
            // ReplaceWithTextBox
            // 
            resources.ApplyResources(this.ReplaceWithTextBox, "ReplaceWithTextBox");
            this.ReplaceWithTextBox.Name = "ReplaceWithTextBox";
            // 
            // RuleEnabledCheckBox
            // 
            resources.ApplyResources(this.RuleEnabledCheckBox, "RuleEnabledCheckBox");
            this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
            this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // InTemplateRuleControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RuleGroupBox);
            this.Name = "InTemplateRuleControl";
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
