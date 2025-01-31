namespace AutoWikiBrowser
{
    partial class PluginManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginManager));
            this.lvPlugin = new WikiFunctions.Controls.NoFlickerExtendedListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.loadPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadNewPluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPluginCount = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvPlugin
            // 
            resources.ApplyResources(this.lvPlugin, "lvPlugin");
            this.lvPlugin.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvPlugin.ComparerFactory = this.lvPlugin;
            this.lvPlugin.ContextMenuStrip = this.contextMenuStrip1;
            this.lvPlugin.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups1"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups2"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups3"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups4"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups5"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups6"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("lvPlugin.Groups7")))});
            this.lvPlugin.Name = "lvPlugin";
            this.lvPlugin.UseCompatibleStateImageBehavior = false;
            this.lvPlugin.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            resources.ApplyResources(this.colName, "colName");
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPluginToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // loadPluginToolStripMenuItem
            // 
            this.loadPluginToolStripMenuItem.Name = "loadPluginToolStripMenuItem";
            resources.ApplyResources(this.loadPluginToolStripMenuItem, "loadPluginToolStripMenuItem");
            this.loadPluginToolStripMenuItem.Click += new System.EventHandler(this.loadPluginToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // pluginToolStripMenuItem
            // 
            this.pluginToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadNewPluginsToolStripMenuItem});
            this.pluginToolStripMenuItem.Name = "pluginToolStripMenuItem";
            resources.ApplyResources(this.pluginToolStripMenuItem, "pluginToolStripMenuItem");
            // 
            // loadNewPluginsToolStripMenuItem
            // 
            this.loadNewPluginsToolStripMenuItem.Name = "loadNewPluginsToolStripMenuItem";
            resources.ApplyResources(this.loadNewPluginsToolStripMenuItem, "loadNewPluginsToolStripMenuItem");
            this.loadNewPluginsToolStripMenuItem.Click += new System.EventHandler(this.loadNewPluginsToolStripMenuItem_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblPluginCount
            // 
            resources.ApplyResources(this.lblPluginCount, "lblPluginCount");
            this.lblPluginCount.Name = "lblPluginCount";
            // 
            // PluginManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lvPlugin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPluginCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PluginManager";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.PluginManager_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WikiFunctions.Controls.NoFlickerExtendedListView lvPlugin;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem pluginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadNewPluginsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadPluginToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPluginCount;
    }
}