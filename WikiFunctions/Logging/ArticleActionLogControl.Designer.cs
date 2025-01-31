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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArticleActionLogControl));
            this.btnAddFailedToList = new System.Windows.Forms.Button();
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
            this.openLogInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lvSuccessful = new WikiFunctions.Logging.ArticleActionListView();
            this.lvFailed = new WikiFunctions.Logging.ArticleActionListView();
            this.btnAddSucessfulToList = new System.Windows.Forms.Button();
            this.mnuListView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddFailedToList
            // 
            resources.ApplyResources(this.btnAddFailedToList, "btnAddFailedToList");
            this.btnAddFailedToList.Name = "btnAddFailedToList";
            this.btnAddFailedToList.UseVisualStyleBackColor = true;
            this.btnAddFailedToList.Click += new System.EventHandler(this.btnAddToList_Click);
            // 
            // btnClearFailed
            // 
            resources.ApplyResources(this.btnClearFailed, "btnClearFailed");
            this.btnClearFailed.Name = "btnClearFailed";
            this.btnClearFailed.UseVisualStyleBackColor = true;
            this.btnClearFailed.Click += new System.EventHandler(this.btnClearIgnored_Click);
            // 
            // btnSaveFailed
            // 
            resources.ApplyResources(this.btnSaveFailed, "btnSaveFailed");
            this.btnSaveFailed.Name = "btnSaveFailed";
            this.btnSaveFailed.UseVisualStyleBackColor = true;
            this.btnSaveFailed.Click += new System.EventHandler(this.btnSaveIgnored_Click);
            // 
            // btnClearSuccessful
            // 
            resources.ApplyResources(this.btnClearSuccessful, "btnClearSuccessful");
            this.btnClearSuccessful.Name = "btnClearSuccessful";
            this.btnClearSuccessful.UseVisualStyleBackColor = true;
            this.btnClearSuccessful.Click += new System.EventHandler(this.btnClearSaved_Click);
            // 
            // btnSaveSuccessful
            // 
            resources.ApplyResources(this.btnSaveSuccessful, "btnSaveSuccessful");
            this.btnSaveSuccessful.Name = "btnSaveSuccessful";
            this.btnSaveSuccessful.UseVisualStyleBackColor = true;
            this.btnSaveSuccessful.Click += new System.EventHandler(this.btnSaveSaved_Click);
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
            this.openLogInBrowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.clearToolStripMenuItem});
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
            // openLogInBrowserToolStripMenuItem
            // 
            this.openLogInBrowserToolStripMenuItem.Name = "openLogInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openLogInBrowserToolStripMenuItem, "openLogInBrowserToolStripMenuItem");
            this.openLogInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openLogInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            resources.ApplyResources(this.clearToolStripMenuItem, "clearToolStripMenuItem");
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // lvSuccessful
            // 
            resources.ApplyResources(this.lvSuccessful, "lvSuccessful");
            this.lvSuccessful.ComparerFactory = this.lvSuccessful;
            this.lvSuccessful.ContextMenuStrip = this.mnuListView;
            this.lvSuccessful.FullRowSelect = true;
            this.lvSuccessful.HideSelection = false;
            this.lvSuccessful.Name = "lvSuccessful";
            this.lvSuccessful.ShowItemToolTips = true;
            this.lvSuccessful.SortColumnsOnClick = true;
            this.lvSuccessful.UseCompatibleStateImageBehavior = false;
            this.lvSuccessful.View = System.Windows.Forms.View.Details;
            this.lvSuccessful.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            // 
            // lvFailed
            // 
            resources.ApplyResources(this.lvFailed, "lvFailed");
            this.lvFailed.ComparerFactory = this.lvFailed;
            this.lvFailed.ContextMenuStrip = this.mnuListView;
            this.lvFailed.FullRowSelect = true;
            this.lvFailed.HideSelection = false;
            this.lvFailed.Name = "lvFailed";
            this.lvFailed.ShowItemToolTips = true;
            this.lvFailed.SortColumnsOnClick = true;
            this.lvFailed.UseCompatibleStateImageBehavior = false;
            this.lvFailed.View = System.Windows.Forms.View.Details;
            this.lvFailed.DoubleClick += new System.EventHandler(this.LogLists_DoubleClick);
            // 
            // btnAddSucessfulToList
            // 
            resources.ApplyResources(this.btnAddSucessfulToList, "btnAddSucessfulToList");
            this.btnAddSucessfulToList.Name = "btnAddSucessfulToList";
            this.btnAddSucessfulToList.UseVisualStyleBackColor = true;
            this.btnAddSucessfulToList.Click += new System.EventHandler(this.btnAddSucessfulToList_Click);
            // 
            // ArticleActionLogControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnAddSucessfulToList);
            this.Controls.Add(this.lvFailed);
            this.Controls.Add(this.lvSuccessful);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnSaveFailed);
            this.Controls.Add(this.btnSaveSuccessful);
            this.Controls.Add(this.btnClearFailed);
            this.Controls.Add(this.btnClearSuccessful);
            this.Controls.Add(this.btnAddFailedToList);
            this.Name = "ArticleActionLogControl";
            this.mnuListView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddFailedToList;
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
        private System.Windows.Forms.Button btnAddSucessfulToList;
    }
}
