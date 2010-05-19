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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Loaded AWBPlugins", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Previously Loaded AWB Plugins", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Loaded ListMaker Plugins", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Previously Loaded ListMaker Plugins", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Loaded Base Plugins", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Previously Loaded Base Plugins", System.Windows.Forms.HorizontalAlignment.Left);
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
            this.lvPlugin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvPlugin.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvPlugin.ComparerFactory = this.lvPlugin;
            this.lvPlugin.ContextMenuStrip = this.contextMenuStrip1;
            listViewGroup1.Header = "Loaded AWBPlugins";
            listViewGroup1.Name = "groupAWBLoaded";
            listViewGroup2.Header = "Previously Loaded AWB Plugins";
            listViewGroup2.Name = "groupAWBPrevious";
            listViewGroup3.Header = "Loaded ListMaker Plugins";
            listViewGroup3.Name = "groupLMLoaded";
            listViewGroup4.Header = "Previously Loaded ListMaker Plugins";
            listViewGroup4.Name = "groupLMPrevious";
            listViewGroup5.Header = "Loaded Base Plugins";
            listViewGroup5.Name = "groupBaseLoaded";
            listViewGroup6.Header = "Previously Loaded Base Plugins";
            listViewGroup6.Name = "groupBasePrevious";
            this.lvPlugin.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3,
            listViewGroup4,
            listViewGroup5,
            listViewGroup6});
            this.lvPlugin.Location = new System.Drawing.Point(12, 47);
            this.lvPlugin.Name = "lvPlugin";
            this.lvPlugin.Size = new System.Drawing.Size(384, 231);
            this.lvPlugin.TabIndex = 0;
            this.lvPlugin.UseCompatibleStateImageBehavior = false;
            this.lvPlugin.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Plugin Name";
            this.colName.Width = 357;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadPluginToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(185, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // loadPluginToolStripMenuItem
            // 
            this.loadPluginToolStripMenuItem.Name = "loadPluginToolStripMenuItem";
            this.loadPluginToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.loadPluginToolStripMenuItem.Text = "Load Selected Plugin";
            this.loadPluginToolStripMenuItem.Click += new System.EventHandler(this.loadPluginToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(408, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pluginToolStripMenuItem
            // 
            this.pluginToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadNewPluginsToolStripMenuItem});
            this.pluginToolStripMenuItem.Name = "pluginToolStripMenuItem";
            this.pluginToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.pluginToolStripMenuItem.Text = "&Plugin";
            // 
            // loadNewPluginsToolStripMenuItem
            // 
            this.loadNewPluginsToolStripMenuItem.Name = "loadNewPluginsToolStripMenuItem";
            this.loadNewPluginsToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.loadNewPluginsToolStripMenuItem.Text = "&Load new Plugin(s)";
            this.loadNewPluginsToolStripMenuItem.Click += new System.EventHandler(this.loadNewPluginsToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "No of Plugins:";
            // 
            // lblPluginCount
            // 
            this.lblPluginCount.AutoSize = true;
            this.lblPluginCount.Location = new System.Drawing.Point(91, 31);
            this.lblPluginCount.Name = "lblPluginCount";
            this.lblPluginCount.Size = new System.Drawing.Size(13, 13);
            this.lblPluginCount.TabIndex = 3;
            this.lblPluginCount.Text = "0";
            // 
            // PluginManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 290);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lvPlugin);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPluginCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "PluginManager";
            this.ShowIcon = false;
            this.Text = "Plugin Manager";
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