namespace WikiFunctions
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
            this.btnAddToList = new System.Windows.Forms.Button();
            this.btnClearIgnored = new System.Windows.Forms.Button();
            this.btnSaveIgnored = new System.Windows.Forms.Button();
            this.btnClearSaved = new System.Windows.Forms.Button();
            this.btnSaveSaved = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.mnuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSelectedToArticleListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.filterByReasonOfSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvIgnored = new WikiFunctions.NoFlickerListView();
            this.colIgnoreArticle = new System.Windows.Forms.ColumnHeader();
            this.colIgnoreTime = new System.Windows.Forms.ColumnHeader();
            this.colSkippedBy = new System.Windows.Forms.ColumnHeader();
            this.colSkipReason = new System.Windows.Forms.ColumnHeader();
            this.lvSaved = new WikiFunctions.NoFlickerListView();
            this.colSuccessSave = new System.Windows.Forms.ColumnHeader();
            this.colSuccessTime = new System.Windows.Forms.ColumnHeader();
            this.mnuListView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddToList
            // 
            this.btnAddToList.Location = new System.Drawing.Point(140, 310);
            this.btnAddToList.Name = "btnAddToList";
            this.btnAddToList.Size = new System.Drawing.Size(112, 24);
            this.btnAddToList.TabIndex = 19;
            this.btnAddToList.Text = "Add to article list";
            this.btnAddToList.UseVisualStyleBackColor = true;
            this.btnAddToList.Click += new System.EventHandler(this.btnAddToList_Click);
            // 
            // btnClearIgnored
            // 
            this.btnClearIgnored.Location = new System.Drawing.Point(87, 310);
            this.btnClearIgnored.Name = "btnClearIgnored";
            this.btnClearIgnored.Size = new System.Drawing.Size(50, 24);
            this.btnClearIgnored.TabIndex = 18;
            this.btnClearIgnored.Text = "Clear";
            this.btnClearIgnored.UseVisualStyleBackColor = true;
            this.btnClearIgnored.Click += new System.EventHandler(this.btnClearIgnored_Click);
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
            // 
            // btnClearSaved
            // 
            this.btnClearSaved.Location = new System.Drawing.Point(87, 137);
            this.btnClearSaved.Name = "btnClearSaved";
            this.btnClearSaved.Size = new System.Drawing.Size(75, 24);
            this.btnClearSaved.TabIndex = 16;
            this.btnClearSaved.Text = "Clear";
            this.btnClearSaved.UseVisualStyleBackColor = true;
            this.btnClearSaved.Click += new System.EventHandler(this.btnClearSaved_Click);
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
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Skipped:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Successfully saved:";
            // 
            // saveListDialog
            // 
            this.saveListDialog.DefaultExt = "txt";
            this.saveListDialog.Filter = "Text file|.*txt";
            this.saveListDialog.Title = "Save article list";
            // 
            // mnuListView
            // 
            this.mnuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSelectedToArticleListToolStripMenuItem,
            this.toolStripSeparator1,
            this.filterByReasonOfSelectedToolStripMenuItem});
            this.mnuListView.Name = "mnuListView";
            this.mnuListView.Size = new System.Drawing.Size(217, 54);
            this.mnuListView.Opening += new System.ComponentModel.CancelEventHandler(this.mnuListView_Opening);
            // 
            // addSelectedToArticleListToolStripMenuItem
            // 
            this.addSelectedToArticleListToolStripMenuItem.Name = "addSelectedToArticleListToolStripMenuItem";
            this.addSelectedToArticleListToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.addSelectedToArticleListToolStripMenuItem.Text = "Add selected to article list";
            this.addSelectedToArticleListToolStripMenuItem.Click += new System.EventHandler(this.addSelectedToArticleListToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // filterByReasonOfSelectedToolStripMenuItem
            // 
            this.filterByReasonOfSelectedToolStripMenuItem.Name = "filterByReasonOfSelectedToolStripMenuItem";
            this.filterByReasonOfSelectedToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.filterByReasonOfSelectedToolStripMenuItem.Text = "Filter by reason of selected";
            this.filterByReasonOfSelectedToolStripMenuItem.Click += new System.EventHandler(this.filterByReasonOfSelectedToolStripMenuItem_Click_1);
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
            this.lvIgnored.ContextMenuStrip = this.mnuListView;
            this.lvIgnored.FullRowSelect = true;
            this.lvIgnored.Location = new System.Drawing.Point(6, 187);
            this.lvIgnored.Name = "lvIgnored";
            this.lvIgnored.ShowItemToolTips = true;
            this.lvIgnored.Size = new System.Drawing.Size(246, 117);
            this.lvIgnored.TabIndex = 21;
            this.lvIgnored.UseCompatibleStateImageBehavior = false;
            this.lvIgnored.View = System.Windows.Forms.View.Details;
            this.lvIgnored.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvIgnoredColumnSort);
            // 
            // colIgnoreArticle
            // 
            this.colIgnoreArticle.Text = "Article";
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
            this.lvSaved.ContextMenuStrip = this.mnuListView;
            this.lvSaved.FullRowSelect = true;
            this.lvSaved.Location = new System.Drawing.Point(6, 16);
            this.lvSaved.Name = "lvSaved";
            this.lvSaved.ShowItemToolTips = true;
            this.lvSaved.Size = new System.Drawing.Size(246, 115);
            this.lvSaved.TabIndex = 20;
            this.lvSaved.UseCompatibleStateImageBehavior = false;
            this.lvSaved.View = System.Windows.Forms.View.Details;
            this.lvSaved.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvSavedColumnSort);
            // 
            // colSuccessSave
            // 
            this.colSuccessSave.Text = "Article";
            this.colSuccessSave.Width = 171;
            // 
            // colSuccessTime
            // 
            this.colSuccessTime.Text = "Time";
            // 
            // LogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lvIgnored);
            this.Controls.Add(this.lvSaved);
            this.Controls.Add(this.btnAddToList);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnClearIgnored);
            this.Controls.Add(this.btnSaveIgnored);
            this.Controls.Add(this.btnClearSaved);
            this.Controls.Add(this.btnSaveSaved);
            this.Name = "LogControl";
            this.Size = new System.Drawing.Size(257, 341);
            this.mnuListView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NoFlickerListView lvIgnored;
        private System.Windows.Forms.ColumnHeader colIgnoreArticle;
        private System.Windows.Forms.ColumnHeader colIgnoreTime;
        private System.Windows.Forms.ColumnHeader colSkippedBy;
        private System.Windows.Forms.ColumnHeader colSkipReason;
        private NoFlickerListView lvSaved;
        private System.Windows.Forms.ColumnHeader colSuccessSave;
        private System.Windows.Forms.ColumnHeader colSuccessTime;
        private System.Windows.Forms.Button btnAddToList;
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
        private System.Windows.Forms.ToolStripMenuItem filterByReasonOfSelectedToolStripMenuItem;
    }
}
