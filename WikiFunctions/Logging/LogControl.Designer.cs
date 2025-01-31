namespace WikiFunctions.Logging
{
    partial class LogControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogControl));
            this.btnAddSkippedToList = new System.Windows.Forms.Button();
            this.btnClearIgnored = new System.Windows.Forms.Button();
            this.btnSaveIgnored = new System.Windows.Forms.Button();
            this.btnClearSaved = new System.Windows.Forms.Button();
            this.btnSaveSaved = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.mnuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSelectedToArticleListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHistoryInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDiffInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.filterShowOnlySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filterExcludeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvIgnored = new WikiFunctions.Controls.NoFlickerExtendedListView();
            this.colIgnoreArticle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIgnoreTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkippedBy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSkipReason = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvSaved = new WikiFunctions.Controls.NoFlickerExtendedListView();
            this.colSuccessSave = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSuccessTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddSuccessToList = new System.Windows.Forms.Button();
            this.mnuListView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSkippedToList
            // 
            resources.ApplyResources(this.btnAddSkippedToList, "btnAddSkippedToList");
            this.btnAddSkippedToList.Name = "btnAddSkippedToList";
            this.btnAddSkippedToList.UseVisualStyleBackColor = true;
            this.btnAddSkippedToList.Click += new System.EventHandler(this.btnAddToList_Click);
            // 
            // btnClearIgnored
            // 
            resources.ApplyResources(this.btnClearIgnored, "btnClearIgnored");
            this.btnClearIgnored.Name = "btnClearIgnored";
            this.btnClearIgnored.UseVisualStyleBackColor = true;
            this.btnClearIgnored.Click += new System.EventHandler(this.btnClearIgnored_Click);
            // 
            // btnSaveIgnored
            // 
            resources.ApplyResources(this.btnSaveIgnored, "btnSaveIgnored");
            this.btnSaveIgnored.Name = "btnSaveIgnored";
            this.btnSaveIgnored.UseVisualStyleBackColor = true;
            this.btnSaveIgnored.Click += new System.EventHandler(this.btnSaveIgnored_Click);
            // 
            // btnClearSaved
            // 
            resources.ApplyResources(this.btnClearSaved, "btnClearSaved");
            this.btnClearSaved.Name = "btnClearSaved";
            this.btnClearSaved.UseVisualStyleBackColor = true;
            this.btnClearSaved.Click += new System.EventHandler(this.btnClearSaved_Click);
            // 
            // btnSaveSaved
            // 
            resources.ApplyResources(this.btnSaveSaved, "btnSaveSaved");
            this.btnSaveSaved.Name = "btnSaveSaved";
            this.btnSaveSaved.UseVisualStyleBackColor = true;
            this.btnSaveSaved.Click += new System.EventHandler(this.btnSaveSaved_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            resources.ApplyResources(this.saveListDialog, "saveListDialog");
            // 
            // mnuListView
            // 
            this.mnuListView.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.mnuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSelectedToArticleListToolStripMenuItem,
            this.toolStripSeparator2,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator5,
            this.removeToolStripMenuItem,
            this.toolStripSeparator3,
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.toolStripSeparator4,
            this.openInBrowserToolStripMenuItem,
            this.openHistoryInBrowserToolStripMenuItem,
            this.openDiffInBrowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.filterShowOnlySelectedToolStripMenuItem,
            this.filterExcludeToolStripMenuItem,
            this.toolStripSeparator6,
            this.clearToolStripMenuItem,
            this.resetToolStripMenuItem});
            this.mnuListView.Name = "mnuListView";
            resources.ApplyResources(this.mnuListView, "mnuListView");
            this.mnuListView.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListView_Opening);
            // 
            // addSelectedToArticleListToolStripMenuItem
            // 
            this.addSelectedToArticleListToolStripMenuItem.Name = "addSelectedToArticleListToolStripMenuItem";
            resources.ApplyResources(this.addSelectedToArticleListToolStripMenuItem, "addSelectedToArticleListToolStripMenuItem");
            this.addSelectedToArticleListToolStripMenuItem.Click += new System.EventHandler(this.addSelectedToArticleListToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            resources.ApplyResources(this.selectAllToolStripMenuItem, "selectAllToolStripMenuItem");
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            resources.ApplyResources(this.selectNoneToolStripMenuItem, "selectNoneToolStripMenuItem");
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openInBrowserToolStripMenuItem, "openInBrowserToolStripMenuItem");
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openHistoryInBrowserToolStripMenuItem, "openHistoryInBrowserToolStripMenuItem");
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            // 
            // openDiffInBrowserToolStripMenuItem
            // 
            this.openDiffInBrowserToolStripMenuItem.Name = "openDiffInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openDiffInBrowserToolStripMenuItem, "openDiffInBrowserToolStripMenuItem");
            this.openDiffInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openDiffInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // filterShowOnlySelectedToolStripMenuItem
            // 
            this.filterShowOnlySelectedToolStripMenuItem.Name = "filterShowOnlySelectedToolStripMenuItem";
            resources.ApplyResources(this.filterShowOnlySelectedToolStripMenuItem, "filterShowOnlySelectedToolStripMenuItem");
            this.filterShowOnlySelectedToolStripMenuItem.Click += new System.EventHandler(this.filterShowOnlySelectedToolStripMenuItem_Click_1);
            // 
            // filterExcludeToolStripMenuItem
            // 
            this.filterExcludeToolStripMenuItem.Name = "filterExcludeToolStripMenuItem";
            resources.ApplyResources(this.filterExcludeToolStripMenuItem, "filterExcludeToolStripMenuItem");
            this.filterExcludeToolStripMenuItem.Click += new System.EventHandler(this.filterExcludeToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resources.ApplyResources(this.resetToolStripMenuItem, "resetToolStripMenuItem");
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // lvIgnored
            // 
            resources.ApplyResources(this.lvIgnored, "lvIgnored");
            this.lvIgnored.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIgnoreArticle,
            this.colIgnoreTime,
            this.colSkippedBy,
            this.colSkipReason});
            this.lvIgnored.ComparerFactory = this.lvIgnored;
            this.lvIgnored.ContextMenuStrip = this.mnuListView;
            this.lvIgnored.FullRowSelect = true;
            this.lvIgnored.HideSelection = false;
            this.lvIgnored.Name = "lvIgnored";
            this.lvIgnored.ShowItemToolTips = true;
            this.lvIgnored.SortColumnsOnClick = true;
            this.lvIgnored.UseCompatibleStateImageBehavior = false;
            this.lvIgnored.View = System.Windows.Forms.View.Details;
            this.lvIgnored.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            this.lvIgnored.MouseLeave += new System.EventHandler(this.LogLists_MouseLeave);
            this.lvIgnored.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LogLists_MouseMove);
            // 
            // colIgnoreArticle
            // 
            resources.ApplyResources(this.colIgnoreArticle, "colIgnoreArticle");
            // 
            // colIgnoreTime
            // 
            resources.ApplyResources(this.colIgnoreTime, "colIgnoreTime");
            // 
            // colSkippedBy
            // 
            resources.ApplyResources(this.colSkippedBy, "colSkippedBy");
            // 
            // colSkipReason
            // 
            resources.ApplyResources(this.colSkipReason, "colSkipReason");
            // 
            // lvSaved
            // 
            resources.ApplyResources(this.lvSaved, "lvSaved");
            this.lvSaved.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSuccessSave,
            this.colSuccessTime});
            this.lvSaved.ComparerFactory = this.lvSaved;
            this.lvSaved.ContextMenuStrip = this.mnuListView;
            this.lvSaved.FullRowSelect = true;
            this.lvSaved.HideSelection = false;
            this.lvSaved.Name = "lvSaved";
            this.lvSaved.ShowItemToolTips = true;
            this.lvSaved.SortColumnsOnClick = true;
            this.lvSaved.UseCompatibleStateImageBehavior = false;
            this.lvSaved.View = System.Windows.Forms.View.Details;
            this.lvSaved.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            this.lvSaved.MouseLeave += new System.EventHandler(this.LogLists_MouseLeave);
            this.lvSaved.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LogLists_MouseMove);
            // 
            // colSuccessSave
            // 
            resources.ApplyResources(this.colSuccessSave, "colSuccessSave");
            // 
            // colSuccessTime
            // 
            resources.ApplyResources(this.colSuccessTime, "colSuccessTime");
            // 
            // btnAddSuccessToList
            // 
            resources.ApplyResources(this.btnAddSuccessToList, "btnAddSuccessToList");
            this.btnAddSuccessToList.Name = "btnAddSuccessToList";
            this.btnAddSuccessToList.UseVisualStyleBackColor = true;
            this.btnAddSuccessToList.Click += new System.EventHandler(this.btnAddSuccessToList_Click);
            // 
            // LogControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddSuccessToList);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lvIgnored);
            this.Controls.Add(this.btnSaveIgnored);
            this.Controls.Add(this.lvSaved);
            this.Controls.Add(this.btnSaveSaved);
            this.Controls.Add(this.btnClearIgnored);
            this.Controls.Add(this.btnClearSaved);
            this.Controls.Add(this.btnAddSkippedToList);
            this.Name = "LogControl";
            this.mnuListView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.NoFlickerExtendedListView lvIgnored;
        private System.Windows.Forms.ColumnHeader colIgnoreArticle;
        private System.Windows.Forms.ColumnHeader colIgnoreTime;
        private System.Windows.Forms.ColumnHeader colSkippedBy;
        private System.Windows.Forms.ColumnHeader colSkipReason;
        private Controls.NoFlickerExtendedListView lvSaved;
        private System.Windows.Forms.ColumnHeader colSuccessSave;
        private System.Windows.Forms.ColumnHeader colSuccessTime;
        private System.Windows.Forms.Button btnAddSkippedToList;
        private System.Windows.Forms.Button btnClearIgnored;
        private System.Windows.Forms.Button btnSaveIgnored;
        private System.Windows.Forms.Button btnClearSaved;
        private System.Windows.Forms.Button btnSaveSaved;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ContextMenuStrip mnuListView;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToArticleListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem filterShowOnlySelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHistoryInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDiffInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filterExcludeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.Button btnAddSuccessToList;
    }
}
