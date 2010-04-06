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
        this.RuleGroupBox = new System.Windows.Forms.GroupBox();
        this.label3 = new System.Windows.Forms.Label();
        this.ChangeNameToTextBox = new System.Windows.Forms.TextBox();
        this.label2 = new System.Windows.Forms.Label();
        this.ParamNameTextBox = new System.Windows.Forms.TextBox();
        this.RuleEnabledCheckBox = new System.Windows.Forms.CheckBox();
        this.label1 = new System.Windows.Forms.Label();
        this.NameTextbox = new System.Windows.Forms.TextBox();
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
        this.RuleGroupBox.Controls.Add(this.ParamNameTextBox);
        this.RuleGroupBox.Controls.Add(this.label3);
        this.RuleGroupBox.Controls.Add(this.ChangeNameToTextBox);
        this.RuleGroupBox.Controls.Add(this.RuleEnabledCheckBox);
        this.RuleGroupBox.Location = new System.Drawing.Point(3, 3);
        this.RuleGroupBox.Name = "RuleGroupBox";
        this.RuleGroupBox.Size = new System.Drawing.Size(426, 405);
        this.RuleGroupBox.TabIndex = 12;
        this.RuleGroupBox.TabStop = false;
        this.RuleGroupBox.Text = "Template parameter";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(20, 100);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(88, 13);
        this.label3.TabIndex = 4;
        this.label3.Text = "&Change name to:";
        // 
        // ChangeNameToTextBox
        // 
        this.ChangeNameToTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ChangeNameToTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ChangeNameToTextBox.Location = new System.Drawing.Point(23, 116);
        this.ChangeNameToTextBox.Name = "ChangeNameToTextBox";
        this.ChangeNameToTextBox.Size = new System.Drawing.Size(397, 21);
        this.ChangeNameToTextBox.TabIndex = 5;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(20, 60);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(101, 13);
        this.label2.TabIndex = 2;
        this.label2.Text = "&Name of Parameter:";
        // 
        // ParamNameTextBox
        // 
        this.ParamNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.ParamNameTextBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.ParamNameTextBox.Location = new System.Drawing.Point(23, 76);
        this.ParamNameTextBox.Name = "ParamNameTextBox";
        this.ParamNameTextBox.Size = new System.Drawing.Size(397, 21);
        this.ParamNameTextBox.TabIndex = 3;
        // 
        // RuleEnabledCheckBox
        // 
        this.RuleEnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.RuleEnabledCheckBox.AutoSize = true;
        this.RuleEnabledCheckBox.Location = new System.Drawing.Point(355, 21);
        this.RuleEnabledCheckBox.Name = "RuleEnabledCheckBox";
        this.RuleEnabledCheckBox.Size = new System.Drawing.Size(65, 17);
        this.RuleEnabledCheckBox.TabIndex = 6;
        this.RuleEnabledCheckBox.Text = "&Enabled";
        this.RuleEnabledCheckBox.UseVisualStyleBackColor = true;
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
        this.NameTextbox.TextChanged += new System.EventHandler(this.NameTextbox_TextChanged);
        // 
        // TemplateParamRuleControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.RuleGroupBox);
        this.Name = "TemplateParamRuleControl";
        this.Size = new System.Drawing.Size(434, 414);
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
