namespace WikiFunctions.Logging
{
    partial class ArticleActionLogControl
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
            this.btnClearFailed = new System.Windows.Forms.Button();
            this.btnSaveFailed = new System.Windows.Forms.Button();
            this.btnClearSuccessful = new System.Windows.Forms.Button();
            this.btnSaveSuccessful = new System.Windows.Forms.Button();
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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvSuccessful = new WikiFunctions.Logging.ArticleActionListView();
            this.lvFailed = new WikiFunctions.Logging.ArticleActionListView();
            this.openLogInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuListView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddToList
            // 
            this.btnAddToList.Location = new System.Drawing.Point(87, 310);
            this.btnAddToList.Name = "btnAddToList";
            this.btnAddToList.Size = new System.Drawing.Size(78, 24);
            this.btnAddToList.TabIndex = 19;
            this.btnAddToList.Text = "Add all to list";
            this.btnAddToList.UseVisualStyleBackColor = true;
            this.btnAddToList.Click += new System.EventHandler(this.btnAddToList_Click);
            // 
            // btnClearFailed
            // 
            this.btnClearFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearFailed.Location = new System.Drawing.Point(177, 310);
            this.btnClearFailed.Name = "btnClearFailed";
            this.btnClearFailed.Size = new System.Drawing.Size(75, 24);
            this.btnClearFailed.TabIndex = 18;
            this.btnClearFailed.Text = "Clear";
            this.btnClearFailed.UseVisualStyleBackColor = true;
            this.btnClearFailed.Click += new System.EventHandler(this.btnClearIgnored_Click);
            // 
            // btnSaveFailed
            // 
            this.btnSaveFailed.Location = new System.Drawing.Point(6, 310);
            this.btnSaveFailed.Name = "btnSaveFailed";
            this.btnSaveFailed.Size = new System.Drawing.Size(75, 24);
            this.btnSaveFailed.TabIndex = 17;
            this.btnSaveFailed.Text = "Save log";
            this.btnSaveFailed.UseVisualStyleBackColor = true;
            this.btnSaveFailed.Click += new System.EventHandler(this.btnSaveIgnored_Click);
            // 
            // btnClearSuccessful
            // 
            this.btnClearSuccessful.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSuccessful.Location = new System.Drawing.Point(177, 137);
            this.btnClearSuccessful.Name = "btnClearSuccessful";
            this.btnClearSuccessful.Size = new System.Drawing.Size(75, 24);
            this.btnClearSuccessful.TabIndex = 16;
            this.btnClearSuccessful.Text = "Clear";
            this.btnClearSuccessful.UseVisualStyleBackColor = true;
            this.btnClearSuccessful.Click += new System.EventHandler(this.btnClearSaved_Click);
            // 
            // btnSaveSuccessful
            // 
            this.btnSaveSuccessful.Location = new System.Drawing.Point(6, 137);
            this.btnSaveSuccessful.Name = "btnSaveSuccessful";
            this.btnSaveSuccessful.Size = new System.Drawing.Size(75, 24);
            this.btnSaveSuccessful.TabIndex = 15;
            this.btnSaveSuccessful.Text = "Save log";
            this.btnSaveSuccessful.UseVisualStyleBackColor = true;
            this.btnSaveSuccessful.Click += new System.EventHandler(this.btnSaveSaved_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Failed:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Successful:";
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
            this.openLogInBrowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearToolStripMenuItem});
            this.mnuListView.Name = "mnuListView";
            this.mnuListView.Size = new System.Drawing.Size(204, 276);
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
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
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
            // 
            // openHistoryInBrowserToolStripMenuItem
            // 
            this.openHistoryInBrowserToolStripMenuItem.Name = "openHistoryInBrowserToolStripMenuItem";
            this.openHistoryInBrowserToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openHistoryInBrowserToolStripMenuItem.Text = "Open history in browser";
            this.openHistoryInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openHistoryInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // lvSuccessful
            // 
            this.lvSuccessful.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSuccessful.ComparerFactory = this.lvSuccessful;
            this.lvSuccessful.ContextMenuStrip = this.mnuListView;
            this.lvSuccessful.FullRowSelect = true;
            this.lvSuccessful.Location = new System.Drawing.Point(3, 16);
            this.lvSuccessful.Name = "lvSuccessful";
            this.lvSuccessful.ShowItemToolTips = true;
            this.lvSuccessful.Size = new System.Drawing.Size(249, 115);
            this.lvSuccessful.SortColumnsOnClick = true;
            this.lvSuccessful.TabIndex = 20;
            this.lvSuccessful.UseCompatibleStateImageBehavior = false;
            this.lvSuccessful.View = System.Windows.Forms.View.Details;
            this.lvSuccessful.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            // 
            // lvFailed
            // 
            this.lvFailed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFailed.ComparerFactory = this.lvFailed;
            this.lvFailed.ContextMenuStrip = this.mnuListView;
            this.lvFailed.FullRowSelect = true;
            this.lvFailed.Location = new System.Drawing.Point(3, 187);
            this.lvFailed.Name = "lvFailed";
            this.lvFailed.ShowItemToolTips = true;
            this.lvFailed.Size = new System.Drawing.Size(249, 115);
            this.lvFailed.SortColumnsOnClick = true;
            this.lvFailed.TabIndex = 20;
            this.lvFailed.UseCompatibleStateImageBehavior = false;
            this.lvFailed.View = System.Windows.Forms.View.Details;
            this.lvFailed.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            // 
            // openLogInBrowserToolStripMenuItem
            // 
            this.openLogInBrowserToolStripMenuItem.Name = "openLogInBrowserToolStripMenuItem";
            this.openLogInBrowserToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.openLogInBrowserToolStripMenuItem.Text = "Open log in browser";
            this.openLogInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openLogInBrowserToolStripMenuItem_Click);
            // 
            // ArticleActionLogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvFailed);
            this.Controls.Add(this.lvSuccessful);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnSaveFailed);
            this.Controls.Add(this.btnSaveSuccessful);
            this.Controls.Add(this.btnClearFailed);
            this.Controls.Add(this.btnClearSuccessful);
            this.Controls.Add(this.btnAddToList);
            this.Name = "ArticleActionLogControl";
            this.Size = new System.Drawing.Size(257, 341);
            this.mnuListView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddToList;
        private System.Windows.Forms.Button btnClearFailed;
        private System.Windows.Forms.Button btnSaveFailed;
        private System.Windows.Forms.Button btnClearSuccessful;
        private System.Windows.Forms.Button btnSaveSuccessful;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.ContextMenuStrip mnuListView;
        private System.Windows.Forms.ToolStripMenuItem addSelectedToArticleListToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHistoryInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private ArticleActionListView lvSuccessful;
        private ArticleActionListView lvFailed;
        private System.Windows.Forms.ToolStripMenuItem openLogInBrowserToolStripMenuItem;
    }
}
