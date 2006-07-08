namespace WikiFunctions.MWB
{
  partial class ReplaceSpecial
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplaceSpecial));
      this.UpButton = new System.Windows.Forms.Button();
      this.DownButton = new System.Windows.Forms.Button();
      this.DeleteButton = new System.Windows.Forms.Button();
      this.NewRuleButton = new System.Windows.Forms.Button();
      this.OkButton = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.RulesTreeView = new System.Windows.Forms.TreeView();
      this.TreeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.NewRuleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.CutContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.CopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.PasteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.DeleteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.RuleMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.NewMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.NewRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewInTemplateRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewTemplateParameterRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleInTemplateCallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleTemplateParameterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
      this.CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.NewSubruleButton = new System.Windows.Forms.Button();
      this.NoRuleSelectedLabel = new System.Windows.Forms.Label();
      this.UndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.RedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.TreeViewContextMenu.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // UpButton
      // 
      this.UpButton.Image = ((System.Drawing.Image)(resources.GetObject("UpButton.Image")));
      this.UpButton.Location = new System.Drawing.Point(237, 28);
      this.UpButton.Name = "UpButton";
      this.UpButton.Size = new System.Drawing.Size(24, 23);
      this.UpButton.TabIndex = 10;
      this.UpButton.UseVisualStyleBackColor = true;
      this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
      // 
      // DownButton
      // 
      this.DownButton.Image = ((System.Drawing.Image)(resources.GetObject("DownButton.Image")));
      this.DownButton.Location = new System.Drawing.Point(237, 57);
      this.DownButton.Name = "DownButton";
      this.DownButton.Size = new System.Drawing.Size(24, 23);
      this.DownButton.TabIndex = 11;
      this.DownButton.UseVisualStyleBackColor = true;
      this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
      // 
      // DeleteButton
      // 
      this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.DeleteButton.Location = new System.Drawing.Point(179, 439);
      this.DeleteButton.Name = "DeleteButton";
      this.DeleteButton.Size = new System.Drawing.Size(55, 23);
      this.DeleteButton.TabIndex = 12;
      this.DeleteButton.Text = "Delete";
      this.DeleteButton.UseVisualStyleBackColor = true;
      this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
      // 
      // NewRuleButton
      // 
      this.NewRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.NewRuleButton.Location = new System.Drawing.Point(7, 439);
      this.NewRuleButton.Name = "NewRuleButton";
      this.NewRuleButton.Size = new System.Drawing.Size(80, 23);
      this.NewRuleButton.TabIndex = 13;
      this.NewRuleButton.Text = "New Rule";
      this.NewRuleButton.UseVisualStyleBackColor = true;
      this.NewRuleButton.Click += new System.EventHandler(this.NewRuleButton_Click);
      // 
      // OkButton
      // 
      this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.OkButton.Location = new System.Drawing.Point(605, 439);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 14;
      this.OkButton.Text = "OK";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // RulesTreeView
      // 
      this.RulesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.RulesTreeView.ContextMenuStrip = this.TreeViewContextMenu;
      this.RulesTreeView.HideSelection = false;
      this.RulesTreeView.Location = new System.Drawing.Point(7, 27);
      this.RulesTreeView.Name = "RulesTreeView";
      this.RulesTreeView.Size = new System.Drawing.Size(227, 406);
      this.RulesTreeView.TabIndex = 15;
      this.RulesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RulesTreeView_AfterSelect);
      // 
      // TreeViewContextMenu
      // 
      this.TreeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewRuleContextMenuItem,
            this.NewSubruleContextMenuItem,
            this.toolStripSeparator1,
            this.CutContextMenuItem,
            this.CopyContextMenuItem,
            this.PasteContextMenuItem,
            this.DeleteContextMenuItem});
      this.TreeViewContextMenu.Name = "TreeViewContextMenu";
      this.TreeViewContextMenu.ShowImageMargin = false;
      this.TreeViewContextMenu.Size = new System.Drawing.Size(126, 142);
      // 
      // NewRuleContextMenuItem
      // 
      this.NewRuleContextMenuItem.Name = "NewRuleContextMenuItem";
      this.NewRuleContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.NewRuleContextMenuItem.Text = "New Rule";
      // 
      // NewSubruleContextMenuItem
      // 
      this.NewSubruleContextMenuItem.Name = "NewSubruleContextMenuItem";
      this.NewSubruleContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.NewSubruleContextMenuItem.Text = "New Subrule";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
      // 
      // CutContextMenuItem
      // 
      this.CutContextMenuItem.Name = "CutContextMenuItem";
      this.CutContextMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
      this.CutContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.CutContextMenuItem.Text = "Cut";
      this.CutContextMenuItem.Click += new System.EventHandler(this.CutMenuItem_Click);
      // 
      // CopyContextMenuItem
      // 
      this.CopyContextMenuItem.Name = "CopyContextMenuItem";
      this.CopyContextMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
      this.CopyContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.CopyContextMenuItem.Text = "Copy";
      this.CopyContextMenuItem.Click += new System.EventHandler(this.CopyMenuItem_Click);
      // 
      // PasteContextMenuItem
      // 
      this.PasteContextMenuItem.Name = "PasteContextMenuItem";
      this.PasteContextMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
      this.PasteContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.PasteContextMenuItem.Text = "Paste";
      this.PasteContextMenuItem.Click += new System.EventHandler(this.PasteMenuItem_Click);
      // 
      // DeleteContextMenuItem
      // 
      this.DeleteContextMenuItem.Name = "DeleteContextMenuItem";
      this.DeleteContextMenuItem.ShortcutKeyDisplayString = "Del";
      this.DeleteContextMenuItem.Size = new System.Drawing.Size(125, 22);
      this.DeleteContextMenuItem.Text = "Delete";
      this.DeleteContextMenuItem.Click += new System.EventHandler(this.DeleteMenuItem_Click);
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RuleMenu,
            this.EditMenu});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(705, 24);
      this.menuStrip1.TabIndex = 16;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // RuleMenu
      // 
      this.RuleMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.RuleMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewMenu,
            this.NewSubruleMenu});
      this.RuleMenu.Name = "RuleMenu";
      this.RuleMenu.Size = new System.Drawing.Size(40, 20);
      this.RuleMenu.Text = "Rule";
      // 
      // NewMenu
      // 
      this.NewMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.NewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewRuleMenuItem,
            this.NewInTemplateRuleMenuItem,
            this.NewTemplateParameterRuleMenuItem});
      this.NewMenu.Name = "NewMenu";
      this.NewMenu.Size = new System.Drawing.Size(145, 22);
      this.NewMenu.Text = "New";
      // 
      // NewRuleMenuItem
      // 
      this.NewRuleMenuItem.Name = "NewRuleMenuItem";
      this.NewRuleMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewRuleMenuItem.Text = "Rule";
      this.NewRuleMenuItem.Click += new System.EventHandler(this.NewRuleMenuItem_Click);
      // 
      // NewInTemplateRuleMenuItem
      // 
      this.NewInTemplateRuleMenuItem.Name = "NewInTemplateRuleMenuItem";
      this.NewInTemplateRuleMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewInTemplateRuleMenuItem.Text = "In Template Call Rule";
      this.NewInTemplateRuleMenuItem.Click += new System.EventHandler(this.NewInTemplateRuleMenuItem_Click);
      // 
      // NewTemplateParameterRuleMenuItem
      // 
      this.NewTemplateParameterRuleMenuItem.Name = "NewTemplateParameterRuleMenuItem";
      this.NewTemplateParameterRuleMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewTemplateParameterRuleMenuItem.Text = "Template Parameter Rule";
      this.NewTemplateParameterRuleMenuItem.Click += new System.EventHandler(this.NewTemplateParameterRuleMenuItem_Click);
      // 
      // NewSubruleMenu
      // 
      this.NewSubruleMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.NewSubruleMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewSubruleMenuItem,
            this.NewSubruleInTemplateCallMenuItem,
            this.NewSubruleTemplateParameterMenuItem});
      this.NewSubruleMenu.Name = "NewSubruleMenu";
      this.NewSubruleMenu.Size = new System.Drawing.Size(145, 22);
      this.NewSubruleMenu.Text = "New Subrule";
      // 
      // NewSubruleMenuItem
      // 
      this.NewSubruleMenuItem.Name = "NewSubruleMenuItem";
      this.NewSubruleMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewSubruleMenuItem.Text = "Rule";
      this.NewSubruleMenuItem.Click += new System.EventHandler(this.NewSubruleMenuItem_Click);
      // 
      // NewSubruleInTemplateCallMenuItem
      // 
      this.NewSubruleInTemplateCallMenuItem.Name = "NewSubruleInTemplateCallMenuItem";
      this.NewSubruleInTemplateCallMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewSubruleInTemplateCallMenuItem.Text = "In Template Call Rule";
      this.NewSubruleInTemplateCallMenuItem.Click += new System.EventHandler(this.NewSubruleInTemplateCallMenuItem_Click);
      // 
      // NewSubruleTemplateParameterMenuItem
      // 
      this.NewSubruleTemplateParameterMenuItem.Name = "NewSubruleTemplateParameterMenuItem";
      this.NewSubruleTemplateParameterMenuItem.Size = new System.Drawing.Size(206, 22);
      this.NewSubruleTemplateParameterMenuItem.Text = "Template Parameter Rule";
      this.NewSubruleTemplateParameterMenuItem.Click += new System.EventHandler(this.NewSubruleTemplateParameterMenuItem_Click);
      // 
      // EditMenu
      // 
      this.EditMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UndoMenuItem,
            this.RedoMenuItem,
            this.toolStripSeparator2,
            this.CutMenuItem,
            this.CopyMenuItem,
            this.PasteMenuItem,
            this.DeleteMenuItem});
      this.EditMenu.Name = "EditMenu";
      this.EditMenu.Size = new System.Drawing.Size(37, 20);
      this.EditMenu.Text = "Edit";
      // 
      // CutMenuItem
      // 
      this.CutMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.CutMenuItem.Name = "CutMenuItem";
      this.CutMenuItem.ShortcutKeyDisplayString = "Ctrl+X";
      this.CutMenuItem.Size = new System.Drawing.Size(152, 22);
      this.CutMenuItem.Text = "Cut";
      this.CutMenuItem.Click += new System.EventHandler(this.CutMenuItem_Click);
      // 
      // CopyMenuItem
      // 
      this.CopyMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.CopyMenuItem.Name = "CopyMenuItem";
      this.CopyMenuItem.ShortcutKeyDisplayString = "Ctrl+C";
      this.CopyMenuItem.Size = new System.Drawing.Size(152, 22);
      this.CopyMenuItem.Text = "Copy";
      this.CopyMenuItem.Click += new System.EventHandler(this.CopyMenuItem_Click);
      // 
      // PasteMenuItem
      // 
      this.PasteMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.PasteMenuItem.Name = "PasteMenuItem";
      this.PasteMenuItem.ShortcutKeyDisplayString = "Ctrl+V";
      this.PasteMenuItem.Size = new System.Drawing.Size(152, 22);
      this.PasteMenuItem.Text = "Paste";
      this.PasteMenuItem.Click += new System.EventHandler(this.PasteMenuItem_Click);
      // 
      // DeleteMenuItem
      // 
      this.DeleteMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.DeleteMenuItem.Name = "DeleteMenuItem";
      this.DeleteMenuItem.ShortcutKeyDisplayString = "Del";
      this.DeleteMenuItem.Size = new System.Drawing.Size(152, 22);
      this.DeleteMenuItem.Text = "Delete";
      this.DeleteMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
      this.DeleteMenuItem.Click += new System.EventHandler(this.DeleteMenuItem_Click);
      // 
      // NewSubruleButton
      // 
      this.NewSubruleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.NewSubruleButton.Location = new System.Drawing.Point(93, 439);
      this.NewSubruleButton.Name = "NewSubruleButton";
      this.NewSubruleButton.Size = new System.Drawing.Size(80, 23);
      this.NewSubruleButton.TabIndex = 17;
      this.NewSubruleButton.Text = "New Subrule";
      this.NewSubruleButton.UseVisualStyleBackColor = true;
      this.NewSubruleButton.Click += new System.EventHandler(this.NewSubruleButton_Click);
      // 
      // NoRuleSelectedLabel
      // 
      this.NoRuleSelectedLabel.AutoSize = true;
      this.NoRuleSelectedLabel.Location = new System.Drawing.Point(423, 224);
      this.NoRuleSelectedLabel.Name = "NoRuleSelectedLabel";
      this.NoRuleSelectedLabel.Size = new System.Drawing.Size(89, 13);
      this.NoRuleSelectedLabel.TabIndex = 18;
      this.NoRuleSelectedLabel.Text = "No Rule selected";
      // 
      // UndoMenuItem
      // 
      this.UndoMenuItem.Name = "UndoMenuItem";
      this.UndoMenuItem.ShortcutKeyDisplayString = "Ctr+Z";
      this.UndoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
      this.UndoMenuItem.Size = new System.Drawing.Size(152, 22);
      this.UndoMenuItem.Text = "Undo";
      this.UndoMenuItem.Click += new System.EventHandler(this.UndoMenuItem_Click);
      // 
      // RedoMenuItem
      // 
      this.RedoMenuItem.Name = "RedoMenuItem";
      this.RedoMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
      this.RedoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
      this.RedoMenuItem.Size = new System.Drawing.Size(152, 22);
      this.RedoMenuItem.Text = "Redo";
      this.RedoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
      // 
      // ReplaceSpecial
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(705, 467);
      this.Controls.Add(this.NoRuleSelectedLabel);
      this.Controls.Add(this.NewSubruleButton);
      this.Controls.Add(this.menuStrip1);
      this.Controls.Add(this.RulesTreeView);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.NewRuleButton);
      this.Controls.Add(this.DeleteButton);
      this.Controls.Add(this.DownButton);
      this.Controls.Add(this.UpButton);
      this.DoubleBuffered = true;
      this.KeyPreview = true;
      this.Location = new System.Drawing.Point(634, 1000);
      this.MainMenuStrip = this.menuStrip1;
      this.MaximizeBox = false;
      this.MinimumSize = new System.Drawing.Size(713, 501);
      this.Name = "ReplaceSpecial";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Replace Special";
      this.Deactivate += new System.EventHandler(this.ReplaceSpecial_Deactivate);
      this.VisibleChanged += new System.EventHandler(this.ReplaceSpecial_VisibleChanged);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReplaceSpecial_FormClosing);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReplaceSpecial_KeyDown);
      this.Leave += new System.EventHandler(this.ReplaceSpecial_Leave);
      this.TreeViewContextMenu.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button UpButton;
    private System.Windows.Forms.Button DownButton;
    private System.Windows.Forms.Button DeleteButton;
    private System.Windows.Forms.Button NewRuleButton;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.TreeView RulesTreeView;
    private System.Windows.Forms.ContextMenuStrip TreeViewContextMenu;
    private System.Windows.Forms.ToolStripMenuItem NewSubruleContextMenuItem;
    private System.Windows.Forms.ToolStripMenuItem DeleteContextMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewRuleContextMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.Button NewSubruleButton;
    private System.Windows.Forms.ToolStripMenuItem RuleMenu;
    private System.Windows.Forms.ToolStripMenuItem NewMenu;
    private System.Windows.Forms.ToolStripMenuItem NewSubruleMenu;
    private System.Windows.Forms.ToolStripMenuItem EditMenu;
    private System.Windows.Forms.ToolStripMenuItem CutMenuItem;
    private System.Windows.Forms.ToolStripMenuItem CopyMenuItem;
    private System.Windows.Forms.ToolStripMenuItem PasteMenuItem;
    private System.Windows.Forms.ToolStripMenuItem DeleteMenuItem;
    private System.Windows.Forms.ToolStripMenuItem CutContextMenuItem;
    private System.Windows.Forms.ToolStripMenuItem CopyContextMenuItem;
    private System.Windows.Forms.ToolStripMenuItem PasteContextMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewRuleMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewInTemplateRuleMenuItem;
    private System.Windows.Forms.Label NoRuleSelectedLabel;
    private System.Windows.Forms.ToolStripMenuItem NewTemplateParameterRuleMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewSubruleMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewSubruleInTemplateCallMenuItem;
    private System.Windows.Forms.ToolStripMenuItem NewSubruleTemplateParameterMenuItem;
    private System.Windows.Forms.ToolStripMenuItem UndoMenuItem;
    private System.Windows.Forms.ToolStripMenuItem RedoMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
  }
}