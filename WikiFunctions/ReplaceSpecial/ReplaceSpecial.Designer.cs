namespace WikiFunctions.ReplaceSpecial
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
            this.RulesTreeView = new System.Windows.Forms.TreeView();
            this.TreeViewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewRuleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewSubruleContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CutContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.UndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshColoursToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewSubruleButton = new System.Windows.Forms.Button();
            this.NoRuleSelectedLabel = new System.Windows.Forms.Label();
            this.RuleControlSpace = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.TreeViewContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.RuleControlSpace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpButton
            // 
            resources.ApplyResources(this.UpButton, "UpButton");
            this.UpButton.Name = "UpButton";
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // DownButton
            // 
            resources.ApplyResources(this.DownButton, "DownButton");
            this.DownButton.Name = "DownButton";
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // DeleteButton
            // 
            resources.ApplyResources(this.DeleteButton, "DeleteButton");
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // NewRuleButton
            // 
            resources.ApplyResources(this.NewRuleButton, "NewRuleButton");
            this.NewRuleButton.Name = "NewRuleButton";
            this.NewRuleButton.Click += new System.EventHandler(this.NewRuleButton_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OkButton.Name = "OkButton";
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // RulesTreeView
            // 
            this.RulesTreeView.AllowDrop = true;
            resources.ApplyResources(this.RulesTreeView, "RulesTreeView");
            this.RulesTreeView.ContextMenuStrip = this.TreeViewContextMenu;
            this.RulesTreeView.HideSelection = false;
            this.RulesTreeView.HotTracking = true;
            this.RulesTreeView.Name = "RulesTreeView";
            this.RulesTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.RulesTreeView_ItemDrag);
            this.RulesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RulesTreeView_AfterSelect);
            this.RulesTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.RulesTreeView_DragDrop);
            this.RulesTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.RulesTreeView_DragEnter);
            this.RulesTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.RulesTreeView_DragOver);
            // 
            // TreeViewContextMenu
            // 
            this.TreeViewContextMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.TreeViewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewRuleContextMenuItem,
            this.NewSubruleContextMenuItem,
            this.toolStripSeparator1,
            this.CutContextMenuItem,
            this.CopyContextMenuItem,
            this.PasteContextMenuItem,
            this.DeleteContextMenuItem,
            this.toolStripSeparator4,
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
            this.TreeViewContextMenu.Name = "TreeViewContextMenu";
            this.TreeViewContextMenu.ShowImageMargin = false;
            resources.ApplyResources(this.TreeViewContextMenu, "TreeViewContextMenu");
            // 
            // NewRuleContextMenuItem
            // 
            this.NewRuleContextMenuItem.Name = "NewRuleContextMenuItem";
            resources.ApplyResources(this.NewRuleContextMenuItem, "NewRuleContextMenuItem");
            this.NewRuleContextMenuItem.Click += new System.EventHandler(this.NewRuleContextMenuItem_Click);
            // 
            // NewSubruleContextMenuItem
            // 
            this.NewSubruleContextMenuItem.Name = "NewSubruleContextMenuItem";
            resources.ApplyResources(this.NewSubruleContextMenuItem, "NewSubruleContextMenuItem");
            this.NewSubruleContextMenuItem.Click += new System.EventHandler(this.NewSubruleContextMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // CutContextMenuItem
            // 
            this.CutContextMenuItem.Name = "CutContextMenuItem";
            resources.ApplyResources(this.CutContextMenuItem, "CutContextMenuItem");
            this.CutContextMenuItem.Click += new System.EventHandler(this.CutMenuItem_Click);
            // 
            // CopyContextMenuItem
            // 
            this.CopyContextMenuItem.Name = "CopyContextMenuItem";
            resources.ApplyResources(this.CopyContextMenuItem, "CopyContextMenuItem");
            this.CopyContextMenuItem.Click += new System.EventHandler(this.CopyMenuItem_Click);
            // 
            // PasteContextMenuItem
            // 
            this.PasteContextMenuItem.Name = "PasteContextMenuItem";
            resources.ApplyResources(this.PasteContextMenuItem, "PasteContextMenuItem");
            this.PasteContextMenuItem.Click += new System.EventHandler(this.PasteMenuItem_Click);
            // 
            // DeleteContextMenuItem
            // 
            this.DeleteContextMenuItem.Name = "DeleteContextMenuItem";
            resources.ApplyResources(this.DeleteContextMenuItem, "DeleteContextMenuItem");
            this.DeleteContextMenuItem.Click += new System.EventHandler(this.DeleteMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            resources.ApplyResources(this.expandAllToolStripMenuItem, "expandAllToolStripMenuItem");
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            resources.ApplyResources(this.collapseAllToolStripMenuItem, "collapseAllToolStripMenuItem");
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RuleMenu,
            this.EditMenu});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // RuleMenu
            // 
            this.RuleMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.RuleMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewMenu,
            this.NewSubruleMenu});
            this.RuleMenu.Name = "RuleMenu";
            resources.ApplyResources(this.RuleMenu, "RuleMenu");
            // 
            // NewMenu
            // 
            this.NewMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.NewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewRuleMenuItem,
            this.NewInTemplateRuleMenuItem,
            this.NewTemplateParameterRuleMenuItem});
            this.NewMenu.Name = "NewMenu";
            resources.ApplyResources(this.NewMenu, "NewMenu");
            // 
            // NewRuleMenuItem
            // 
            this.NewRuleMenuItem.Name = "NewRuleMenuItem";
            resources.ApplyResources(this.NewRuleMenuItem, "NewRuleMenuItem");
            this.NewRuleMenuItem.Click += new System.EventHandler(this.NewRuleMenuItem_Click);
            // 
            // NewInTemplateRuleMenuItem
            // 
            this.NewInTemplateRuleMenuItem.Name = "NewInTemplateRuleMenuItem";
            resources.ApplyResources(this.NewInTemplateRuleMenuItem, "NewInTemplateRuleMenuItem");
            this.NewInTemplateRuleMenuItem.Click += new System.EventHandler(this.NewInTemplateRuleMenuItem_Click);
            // 
            // NewTemplateParameterRuleMenuItem
            // 
            this.NewTemplateParameterRuleMenuItem.Name = "NewTemplateParameterRuleMenuItem";
            resources.ApplyResources(this.NewTemplateParameterRuleMenuItem, "NewTemplateParameterRuleMenuItem");
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
            resources.ApplyResources(this.NewSubruleMenu, "NewSubruleMenu");
            // 
            // NewSubruleMenuItem
            // 
            this.NewSubruleMenuItem.Name = "NewSubruleMenuItem";
            resources.ApplyResources(this.NewSubruleMenuItem, "NewSubruleMenuItem");
            this.NewSubruleMenuItem.Click += new System.EventHandler(this.NewSubruleMenuItem_Click);
            // 
            // NewSubruleInTemplateCallMenuItem
            // 
            this.NewSubruleInTemplateCallMenuItem.Name = "NewSubruleInTemplateCallMenuItem";
            resources.ApplyResources(this.NewSubruleInTemplateCallMenuItem, "NewSubruleInTemplateCallMenuItem");
            this.NewSubruleInTemplateCallMenuItem.Click += new System.EventHandler(this.NewSubruleInTemplateCallMenuItem_Click);
            // 
            // NewSubruleTemplateParameterMenuItem
            // 
            this.NewSubruleTemplateParameterMenuItem.Name = "NewSubruleTemplateParameterMenuItem";
            resources.ApplyResources(this.NewSubruleTemplateParameterMenuItem, "NewSubruleTemplateParameterMenuItem");
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
            this.toolStripSeparator5,
            this.DeleteMenuItem,
            this.toolStripSeparator3,
            this.refreshColoursToolStripMenuItem});
            this.EditMenu.Name = "EditMenu";
            resources.ApplyResources(this.EditMenu, "EditMenu");
            // 
            // UndoMenuItem
            // 
            this.UndoMenuItem.Name = "UndoMenuItem";
            resources.ApplyResources(this.UndoMenuItem, "UndoMenuItem");
            this.UndoMenuItem.Click += new System.EventHandler(this.UndoMenuItem_Click);
            // 
            // RedoMenuItem
            // 
            this.RedoMenuItem.Name = "RedoMenuItem";
            resources.ApplyResources(this.RedoMenuItem, "RedoMenuItem");
            this.RedoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // CutMenuItem
            // 
            this.CutMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CutMenuItem.Name = "CutMenuItem";
            resources.ApplyResources(this.CutMenuItem, "CutMenuItem");
            this.CutMenuItem.Click += new System.EventHandler(this.CutMenuItem_Click);
            // 
            // CopyMenuItem
            // 
            this.CopyMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CopyMenuItem.Name = "CopyMenuItem";
            resources.ApplyResources(this.CopyMenuItem, "CopyMenuItem");
            this.CopyMenuItem.Click += new System.EventHandler(this.CopyMenuItem_Click);
            // 
            // PasteMenuItem
            // 
            this.PasteMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.PasteMenuItem.Name = "PasteMenuItem";
            resources.ApplyResources(this.PasteMenuItem, "PasteMenuItem");
            this.PasteMenuItem.Click += new System.EventHandler(this.PasteMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // DeleteMenuItem
            // 
            this.DeleteMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.DeleteMenuItem.Name = "DeleteMenuItem";
            resources.ApplyResources(this.DeleteMenuItem, "DeleteMenuItem");
            this.DeleteMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.DeleteMenuItem.Click += new System.EventHandler(this.DeleteMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // refreshColoursToolStripMenuItem
            // 
            this.refreshColoursToolStripMenuItem.Name = "refreshColoursToolStripMenuItem";
            resources.ApplyResources(this.refreshColoursToolStripMenuItem, "refreshColoursToolStripMenuItem");
            this.refreshColoursToolStripMenuItem.Click += new System.EventHandler(this.refreshColoursToolStripMenuItem_Click);
            // 
            // NewSubruleButton
            // 
            resources.ApplyResources(this.NewSubruleButton, "NewSubruleButton");
            this.NewSubruleButton.Name = "NewSubruleButton";
            this.NewSubruleButton.Click += new System.EventHandler(this.NewSubruleButton_Click);
            // 
            // NoRuleSelectedLabel
            // 
            resources.ApplyResources(this.NoRuleSelectedLabel, "NoRuleSelectedLabel");
            this.NoRuleSelectedLabel.Name = "NoRuleSelectedLabel";
            // 
            // RuleControlSpace
            // 
            resources.ApplyResources(this.RuleControlSpace, "RuleControlSpace");
            this.RuleControlSpace.Controls.Add(this.NoRuleSelectedLabel);
            this.RuleControlSpace.Name = "RuleControlSpace";
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.RulesTreeView);
            this.splitContainer.Panel1.Controls.Add(this.UpButton);
            this.splitContainer.Panel1.Controls.Add(this.DownButton);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.RuleControlSpace);
            // 
            // ReplaceSpecial
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.NewRuleButton);
            this.Controls.Add(this.NewSubruleButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.OkButton);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ReplaceSpecial";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Deactivate += new System.EventHandler(this.ReplaceSpecial_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReplaceSpecial_FormClosing);
            this.Load += new System.EventHandler(this.ReplaceSpecial_Load);
            this.VisibleChanged += new System.EventHandler(this.ReplaceSpecial_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ReplaceSpecial_KeyDown);
            this.Leave += new System.EventHandler(this.ReplaceSpecial_Leave);
            this.TreeViewContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.RuleControlSpace.ResumeLayout(false);
            this.RuleControlSpace.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button UpButton;
    private System.Windows.Forms.Button DownButton;
    private System.Windows.Forms.Button DeleteButton;
    private System.Windows.Forms.Button NewRuleButton;
      private System.Windows.Forms.Button OkButton;
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
    private System.Windows.Forms.Panel RuleControlSpace;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem refreshColoursToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
      private System.Windows.Forms.SplitContainer splitContainer;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
  }
}
