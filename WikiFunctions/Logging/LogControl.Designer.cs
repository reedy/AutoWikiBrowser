﻿namespace WikiFunctions.Logging
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
            this.colIgnoreArticle = new System.Windows.Forms.ColumnHeader();
            this.colIgnoreTime = new System.Windows.Forms.ColumnHeader();
            this.colSkippedBy = new System.Windows.Forms.ColumnHeader();
            this.colSkipReason = new System.Windows.Forms.ColumnHeader();
            this.lvSaved = new WikiFunctions.Controls.NoFlickerExtendedListView();
            this.colSuccessSave = new System.Windows.Forms.ColumnHeader();
            this.colSuccessTime = new System.Windows.Forms.ColumnHeader();
            this.btnAddSuccessToList = new System.Windows.Forms.Button();
            this.mnuListView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSkippedToList
            // 
            this.btnAddSkippedToList.Location = new System.Drawing.Point(87, 310);
            this.btnAddSkippedToList.Name = "btnAddSkippedToList";
            this.btnAddSkippedToList.Size = new System.Drawing.Size(75, 24);
            this.btnAddSkippedToList.TabIndex = 19;
            this.btnAddSkippedToList.Text = "Add all to list";
            this.btnAddSkippedToList.UseVisualStyleBackColor = true;
            this.btnAddSkippedToList.Click += new System.EventHandler(this.btnAddToList_Click);
            this.btnAddSkippedToList.Enabled = false;
            // 
            // btnClearIgnored
            // 
            this.btnClearIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearIgnored.Location = new System.Drawing.Point(177, 310);
            this.btnClearIgnored.Name = "btnClearIgnored";
            this.btnClearIgnored.Size = new System.Drawing.Size(75, 24);
            this.btnClearIgnored.TabIndex = 18;
            this.btnClearIgnored.Text = "Clear";
            this.btnClearIgnored.UseVisualStyleBackColor = true;
            this.btnClearIgnored.Click += new System.EventHandler(this.btnClearIgnored_Click);
            this.btnClearIgnored.Enabled = false;
            // 
            // btnSaveIgnored
            // 
            this.btnSaveIgnored.Location = new System.Drawing.Point(6, 310);
            this.btnSaveIgnored.Name = "btnSaveIgnored";
            this.btnSaveIgnored.Size = new System.Drawing.Size(75, 24);
            this.btnSaveIgnored.TabIndex = 17;
            this.btnSaveIgnored.Text = "Save log";
            this.btnSaveIgnored.UseVisualStyleBackColor = true;
            this.btnSaveIgnored.Click += new System.EventHandler(this.btnSaveIgnored_Click);
            this.btnSaveIgnored.Enabled = false;
            // 
            // btnClearSaved
            // 
            this.btnClearSaved.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSaved.Location = new System.Drawing.Point(177, 137);
            this.btnClearSaved.Name = "btnClearSaved";
            this.btnClearSaved.Size = new System.Drawing.Size(75, 24);
            this.btnClearSaved.TabIndex = 16;
            this.btnClearSaved.Text = "Clear";
            this.btnClearSaved.UseVisualStyleBackColor = true;
            this.btnClearSaved.Click += new System.EventHandler(this.btnClearSaved_Click);
            this.btnClearSaved.Enabled = false;
            // 
            // btnSaveSaved
            // 
            this.btnSaveSaved.Location = new System.Drawing.Point(6, 137);
            this.btnSaveSaved.Name = "btnSaveSaved";
            this.btnSaveSaved.Size = new System.Drawing.Size(75, 24);
            this.btnSaveSaved.TabIndex = 15;
            this.btnSaveSaved.Text = "Save log";
            this.btnSaveSaved.UseVisualStyleBackColor = true;
            this.btnSaveSaved.Click += new System.EventHandler(this.btnSaveSaved_Click);
            this.btnSaveSaved.Enabled = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Skipped: 0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Successfully saved: 0";
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Plain text file|*.txt|Text file with wiki markup|*.txt|Text file with annotated w" +
                "iki markup|*.txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // mnuListView
            // 
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
            this.mnuListView.Size = new System.Drawing.Size(204, 304);
            this.mnuListView.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListView_Opening);
            // 
            // addSelectedToArticleListToolStripMenuItem
            // 
            this.addSelectedToArticleListToolStripMenuItem.Name = "addSelectedToArticleListToolStripMenuItem";
            this.addSelectedToArticleListToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.addSelectedToArticleListToolStripMenuItem.Text = "Add selected to page list";
            this.addSelectedToArticleListToolStripMenuItem.Click += new System.EventHandler(this.addSelectedToArticleListToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(200, 6);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            this.removeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(200, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.selectNoneToolStripMenuItem.Text = "Select none";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(200, 6);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open in browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            this.openInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                | System.Windows.Forms.Keys.P)));
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            this.openHistoryInBrowserToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openHistoryInBrowserToolStripMenuItem.Text = "Open history in browser";
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            this.openHistoryInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                | System.Windows.Forms.Keys.H)));
            // 
            // openDiffInBrowserToolStripMenuItem
            // 
            this.openDiffInBrowserToolStripMenuItem.Name = "openDiffInBrowserToolStripMenuItem";
            this.openDiffInBrowserToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openDiffInBrowserToolStripMenuItem.Text = "Open diff in browser";
            this.openDiffInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openDiffInBrowserToolStripMenuItem_Click);
            this.openDiffInBrowserToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                | System.Windows.Forms.Keys.D)));
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // filterShowOnlySelectedToolStripMenuItem
            // 
            this.filterShowOnlySelectedToolStripMenuItem.Name = "filterShowOnlySelectedToolStripMenuItem";
            this.filterShowOnlySelectedToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.filterShowOnlySelectedToolStripMenuItem.Text = "Filter: Show only";
            this.filterShowOnlySelectedToolStripMenuItem.Click += new System.EventHandler(this.filterShowOnlySelectedToolStripMenuItem_Click_1);
            // 
            // filterExcludeToolStripMenuItem
            // 
            this.filterExcludeToolStripMenuItem.Name = "filterExcludeToolStripMenuItem";
            this.filterExcludeToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.filterExcludeToolStripMenuItem.Text = "Filter: Exclude";
            this.filterExcludeToolStripMenuItem.Click += new System.EventHandler(this.filterExcludeToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(200, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // lvIgnored
            // 
            this.lvIgnored.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvIgnored.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIgnoreArticle,
            this.colIgnoreTime,
            this.colSkippedBy,
            this.colSkipReason});
            this.lvIgnored.ComparerFactory = this.lvIgnored;
            this.lvIgnored.ContextMenuStrip = this.mnuListView;
            this.lvIgnored.FullRowSelect = true;
            this.lvIgnored.Location = new System.Drawing.Point(3, 187);
            this.lvIgnored.Name = "lvIgnored";
            this.lvIgnored.ShowItemToolTips = true;
            this.lvIgnored.Size = new System.Drawing.Size(249, 115);
            this.lvIgnored.SortColumnsOnClick = true;
            this.lvIgnored.TabIndex = 21;
            this.lvIgnored.UseCompatibleStateImageBehavior = false;
            this.lvIgnored.View = System.Windows.Forms.View.Details;
            this.lvIgnored.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            this.lvIgnored.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LogLists_MouseMove);
            this.lvIgnored.MouseLeave += new System.EventHandler(this.LogLists_MouseLeave);
            // 
            // colIgnoreArticle
            // 
            this.colIgnoreArticle.Text = "Page";
            this.colIgnoreArticle.Width = 48;
            // 
            // colIgnoreTime
            // 
            this.colIgnoreTime.Text = "Time";
            this.colIgnoreTime.Width = 42;
            // 
            // colSkippedBy
            // 
            this.colSkippedBy.Text = "Skipped By";
            this.colSkippedBy.Width = 70;
            // 
            // colSkipReason
            // 
            this.colSkipReason.Text = "Skip Reason";
            this.colSkipReason.Width = 81;
            // 
            // lvSaved
            // 
            this.lvSaved.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSaved.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSuccessSave,
            this.colSuccessTime});
            this.lvSaved.ComparerFactory = this.lvSaved;
            this.lvSaved.ContextMenuStrip = this.mnuListView;
            this.lvSaved.FullRowSelect = true;
            this.lvSaved.Location = new System.Drawing.Point(3, 16);
            this.lvSaved.Name = "lvSaved";
            this.lvSaved.ShowItemToolTips = true;
            this.lvSaved.Size = new System.Drawing.Size(249, 115);
            this.lvSaved.SortColumnsOnClick = true;
            this.lvSaved.TabIndex = 20;
            this.lvSaved.UseCompatibleStateImageBehavior = false;
            this.lvSaved.View = System.Windows.Forms.View.Details;
            this.lvSaved.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            this.lvSaved.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LogLists_MouseMove);
            this.lvSaved.MouseLeave += new System.EventHandler(this.LogLists_MouseLeave);
            // 
            // colSuccessSave
            // 
            this.colSuccessSave.Text = "Page";
            this.colSuccessSave.Width = 171;
            // 
            // colSuccessTime
            // 
            this.colSuccessTime.Text = "Time";
            // 
            // btnAddSuccessToList
            // 
            this.btnAddSuccessToList.Location = new System.Drawing.Point(87, 137);
            this.btnAddSuccessToList.Name = "btnAddSuccessToList";
            this.btnAddSuccessToList.Size = new System.Drawing.Size(75, 24);
            this.btnAddSuccessToList.TabIndex = 22;
            this.btnAddSuccessToList.Text = "Add all to list";
            this.btnAddSuccessToList.UseVisualStyleBackColor = true;
            this.btnAddSuccessToList.Click += new System.EventHandler(this.btnAddSuccessToList_Click);
            this.btnAddSuccessToList.Enabled = false;
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
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
            this.Size = new System.Drawing.Size(257, 341);
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
