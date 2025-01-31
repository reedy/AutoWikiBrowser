namespace WikiFunctions.Disambiguation
{
    partial class DabForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DabForm));
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnUndoAll = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnArticle = new System.Windows.Forms.Button();
            this.contextMenuStripOther = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.watchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unwatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripOther.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            resources.ApplyResources(this.tableLayout, "tableLayout");
            this.tableLayout.Name = "tableLayout";
            // 
            // btnDone
            // 
            resources.ApplyResources(this.btnDone, "btnDone");
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDone.Name = "btnDone";
            this.btnDone.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnResetAll
            // 
            resources.ApplyResources(this.btnResetAll, "btnResetAll");
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnUndoAll
            // 
            resources.ApplyResources(this.btnUndoAll, "btnUndoAll");
            this.btnUndoAll.Name = "btnUndoAll";
            this.btnUndoAll.UseVisualStyleBackColor = true;
            this.btnUndoAll.Click += new System.EventHandler(this.btnUndoAll_Click);
            // 
            // btnAbort
            // 
            resources.ApplyResources(this.btnAbort, "btnAbort");
            this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // btnArticle
            // 
            resources.ApplyResources(this.btnArticle, "btnArticle");
            this.btnArticle.Name = "btnArticle";
            this.btnArticle.UseVisualStyleBackColor = true;
            this.btnArticle.Click += new System.EventHandler(this.btnArticle_Click);
            // 
            // contextMenuStripOther
            // 
            this.contextMenuStripOther.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.contextMenuStripOther.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem,
            this.editInBrowserToolStripMenuItem,
            this.toolStripSeparator1,
            this.watchToolStripMenuItem,
            this.unwatchToolStripMenuItem});
            this.contextMenuStripOther.Name = "contextMenuStripOther";
            resources.ApplyResources(this.contextMenuStripOther, "contextMenuStripOther");
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openInBrowserToolStripMenuItem, "openInBrowserToolStripMenuItem");
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // editInBrowserToolStripMenuItem
            // 
            this.editInBrowserToolStripMenuItem.Name = "editInBrowserToolStripMenuItem";
            resources.ApplyResources(this.editInBrowserToolStripMenuItem, "editInBrowserToolStripMenuItem");
            this.editInBrowserToolStripMenuItem.Click += new System.EventHandler(this.editInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // watchToolStripMenuItem
            // 
            this.watchToolStripMenuItem.Name = "watchToolStripMenuItem";
            resources.ApplyResources(this.watchToolStripMenuItem, "watchToolStripMenuItem");
            this.watchToolStripMenuItem.Click += new System.EventHandler(this.watchToolStripMenuItem_Click);
            // 
            // unwatchToolStripMenuItem
            // 
            this.unwatchToolStripMenuItem.Name = "unwatchToolStripMenuItem";
            resources.ApplyResources(this.unwatchToolStripMenuItem, "unwatchToolStripMenuItem");
            this.unwatchToolStripMenuItem.Click += new System.EventHandler(this.unwatchToolStripMenuItem_Click);
            // 
            // DabForm
            // 
            this.AcceptButton = this.btnDone;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnArticle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnUndoAll);
            this.Controls.Add(this.tableLayout);
            this.Controls.Add(this.btnResetAll);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "DabForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.DabForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DabForm_KeyPress);
            this.Move += new System.EventHandler(this.DabForm_Move);
            this.Resize += new System.EventHandler(this.DabForm_Resize);
            this.contextMenuStripOther.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.Button btnUndoAll;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnArticle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOther;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem watchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unwatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}