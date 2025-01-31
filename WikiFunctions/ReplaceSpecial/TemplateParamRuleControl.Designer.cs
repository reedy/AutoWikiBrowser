namespace WikiFunctions.ReplaceSpecial
{
  partial class TemplateParamRuleControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateParamRuleControl));
            this.RuleGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.NameTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ParamNameTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChangeNameToTextBox = new System.Windows.Forms.TextBox();
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
            this.RuleGroupBox.Controls.Add(this.ParamNameTextBox);
            this.RuleGroupBox.Controls.Add(this.label3);
            this.RuleGroupBox.Controls.Add(this.ChangeNameToTextBox);
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
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ParamNameTextBox
            // 
            resources.ApplyResources(this.ParamNameTextBox, "ParamNameTextBox");
            this.ParamNameTextBox.Name = "ParamNameTextBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // ChangeNameToTextBox
            // 
            resources.ApplyResources(this.ChangeNameToTextBox, "ChangeNameToTextBox");
            this.ChangeNameToTextBox.Name = "ChangeNameToTextBox";
            // 
            // RuleEnabledCheckBox
            // 
            resources.ApplyResources(this.RuleEnabledCheckBox, "RuleEnabledCheckBox");
            this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
            this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // TemplateParamRuleControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RuleGroupBox);
            this.Name = "TemplateParamRuleControl";
            this.RuleGroupBox.ResumeLayout(false);
            this.RuleGroupBox.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox RuleGroupBox;
    private System.Windows.Forms.TextBox ChangeNameToTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox ParamNameTextBox;
    private System.Windows.Forms.CheckBox RuleEnabledCheckBox;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox NameTextbox;
    private System.Windows.Forms.Label label3;
  }
}
