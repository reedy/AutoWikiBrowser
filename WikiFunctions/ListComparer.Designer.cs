namespace WikiFunctions.Controls.Lists
{
    partial class ListComparer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListComparer));
            this.lbBoth = new System.Windows.Forms.ListBox();
            this.mnuDuplicates = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferDuplicatesToList1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGo = new System.Windows.Forms.Button();
            this.openListDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblBoth = new System.Windows.Forms.Label();
            this.saveListDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSaveBoth = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listMaker1 = new WikiFunctions.Controls.Lists.ListMaker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listMaker2 = new WikiFunctions.Controls.Lists.ListMaker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSaveOnly2 = new System.Windows.Forms.Button();
            this.lblOnly2 = new System.Windows.Forms.Label();
            this.lbOnly2 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveOnly1 = new System.Windows.Forms.Button();
            this.lblOnly1 = new System.Windows.Forms.Label();
            this.lbOnly1 = new System.Windows.Forms.ListBox();
            this.mnuList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openInBrowserToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.copySelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDuplicates.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.mnuList.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbBoth
            // 
            this.lbBoth.ContextMenuStrip = this.mnuDuplicates;
            this.lbBoth.FormattingEnabled = true;
            this.lbBoth.Location = new System.Drawing.Point(9, 34);
            this.lbBoth.Name = "lbBoth";
            this.lbBoth.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbBoth.Size = new System.Drawing.Size(130, 277);
            this.lbBoth.TabIndex = 2;
            // 
            // mnuDuplicates
            // 
            this.mnuDuplicates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferDuplicatesToList1ToolStripMenuItem,
            this.toolStripSeparator1,
            this.openInBrowserToolStripMenuItem,
            this.toolStripSeparator2,
            this.copyToolStripMenuItem});
            this.mnuDuplicates.Name = "mnuDuplicates";
            this.mnuDuplicates.Size = new System.Drawing.Size(220, 82);
            // 
            // transferDuplicatesToList1ToolStripMenuItem
            // 
            this.transferDuplicatesToList1ToolStripMenuItem.Name = "transferDuplicatesToList1ToolStripMenuItem";
            this.transferDuplicatesToList1ToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.transferDuplicatesToList1ToolStripMenuItem.Text = "Transfer Duplicates to List 1";
            this.transferDuplicatesToList1ToolStripMenuItem.Click += new System.EventHandler(this.transferDuplicatesToList1ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(216, 6);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open in Browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(216, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.copyToolStripMenuItem.Text = "Copy Selected";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(486, 156);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(57, 56);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "Compare";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // openListDialog
            // 
            this.openListDialog.Filter = "Text files|*.txt";
            this.openListDialog.SupportMultiDottedExtensions = true;
            // 
            // lblBoth
            // 
            this.lblBoth.AutoSize = true;
            this.lblBoth.Location = new System.Drawing.Point(72, 18);
            this.lblBoth.Name = "lblBoth";
            this.lblBoth.Size = new System.Drawing.Size(13, 13);
            this.lblBoth.TabIndex = 8;
            this.lblBoth.Text = "0";
            // 
            // saveListDialog
            // 
            this.saveListDialog.Filter = "Text files|*.txt";
            this.saveListDialog.SupportMultiDottedExtensions = true;
            // 
            // btnSaveBoth
            // 
            this.btnSaveBoth.Location = new System.Drawing.Point(37, 317);
            this.btnSaveBoth.Name = "btnSaveBoth";
            this.btnSaveBoth.Size = new System.Drawing.Size(75, 23);
            this.btnSaveBoth.TabIndex = 9;
            this.btnSaveBoth.Text = "Save list";
            this.btnSaveBoth.UseVisualStyleBackColor = true;
            this.btnSaveBoth.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(168, 346);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear all";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Duplicates:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listMaker1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 377);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "List 1";
            // 
            // listMaker1
            // 
            this.listMaker1.ListFile = "";
            this.listMaker1.Location = new System.Drawing.Point(7, 18);
            this.listMaker1.Name = "listMaker1";
            this.listMaker1.SelectedSource = WikiFunctions.Lists.SourceType.Category;
            this.listMaker1.Size = new System.Drawing.Size(201, 351);
            this.listMaker1.SourceText = "";
            this.listMaker1.TabIndex = 14;
            this.listMaker1.WikiStatus = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listMaker2);
            this.groupBox2.Location = new System.Drawing.Point(236, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(218, 377);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "List 2";
            // 
            // listMaker2
            // 
            this.listMaker2.ListFile = "";
            this.listMaker2.Location = new System.Drawing.Point(7, 18);
            this.listMaker2.Name = "listMaker2";
            this.listMaker2.SelectedSource = WikiFunctions.Lists.SourceType.Category;
            this.listMaker2.Size = new System.Drawing.Size(201, 351);
            this.listMaker2.SourceText = "";
            this.listMaker2.TabIndex = 14;
            this.listMaker2.WikiStatus = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.btnSaveOnly2);
            this.groupBox3.Controls.Add(this.lblOnly2);
            this.groupBox3.Controls.Add(this.lbOnly2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnSaveOnly1);
            this.groupBox3.Controls.Add(this.lblOnly1);
            this.groupBox3.Controls.Add(this.lbOnly1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnClear);
            this.groupBox3.Controls.Add(this.btnSaveBoth);
            this.groupBox3.Controls.Add(this.lblBoth);
            this.groupBox3.Controls.Add(this.lbBoth);
            this.groupBox3.Location = new System.Drawing.Point(569, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(420, 377);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Results";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(278, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Only in List 2:";
            // 
            // btnSaveOnly2
            // 
            this.btnSaveOnly2.Location = new System.Drawing.Point(307, 317);
            this.btnSaveOnly2.Name = "btnSaveOnly2";
            this.btnSaveOnly2.Size = new System.Drawing.Size(75, 23);
            this.btnSaveOnly2.TabIndex = 20;
            this.btnSaveOnly2.Text = "Save list";
            this.btnSaveOnly2.UseVisualStyleBackColor = true;
            this.btnSaveOnly2.Click += new System.EventHandler(this.btnSaveOnly2_Click);
            // 
            // lblOnly2
            // 
            this.lblOnly2.AutoSize = true;
            this.lblOnly2.Location = new System.Drawing.Point(354, 18);
            this.lblOnly2.Name = "lblOnly2";
            this.lblOnly2.Size = new System.Drawing.Size(13, 13);
            this.lblOnly2.TabIndex = 19;
            this.lblOnly2.Text = "0";
            // 
            // lbOnly2
            // 
            this.lbOnly2.ContextMenuStrip = this.mnuList;
            this.lbOnly2.FormattingEnabled = true;
            this.lbOnly2.Location = new System.Drawing.Point(281, 34);
            this.lbOnly2.Name = "lbOnly2";
            this.lbOnly2.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbOnly2.Size = new System.Drawing.Size(130, 277);
            this.lbOnly2.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(142, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Only in List 1:";
            // 
            // btnSaveOnly1
            // 
            this.btnSaveOnly1.Location = new System.Drawing.Point(168, 317);
            this.btnSaveOnly1.Name = "btnSaveOnly1";
            this.btnSaveOnly1.Size = new System.Drawing.Size(75, 23);
            this.btnSaveOnly1.TabIndex = 16;
            this.btnSaveOnly1.Text = "Save list";
            this.btnSaveOnly1.UseVisualStyleBackColor = true;
            this.btnSaveOnly1.Click += new System.EventHandler(this.btnSaveOnly1_Click);
            // 
            // lblOnly1
            // 
            this.lblOnly1.AutoSize = true;
            this.lblOnly1.Location = new System.Drawing.Point(218, 18);
            this.lblOnly1.Name = "lblOnly1";
            this.lblOnly1.Size = new System.Drawing.Size(13, 13);
            this.lblOnly1.TabIndex = 15;
            this.lblOnly1.Text = "0";
            // 
            // lbOnly1
            // 
            this.lbOnly1.ContextMenuStrip = this.mnuList;
            this.lbOnly1.FormattingEnabled = true;
            this.lbOnly1.Location = new System.Drawing.Point(145, 34);
            this.lbOnly1.Name = "lbOnly1";
            this.lbOnly1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbOnly1.Size = new System.Drawing.Size(130, 277);
            this.lbOnly1.TabIndex = 14;
            // 
            // mnuList
            // 
            this.mnuList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openInBrowserToolStripMenuItem1,
            this.toolStripSeparator3,
            this.copySelectedToolStripMenuItem});
            this.mnuList.Name = "mnuList1";
            this.mnuList.Size = new System.Drawing.Size(165, 76);
            // 
            // openInBrowserToolStripMenuItem1
            // 
            this.openInBrowserToolStripMenuItem1.Name = "openInBrowserToolStripMenuItem1";
            this.openInBrowserToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.openInBrowserToolStripMenuItem1.Text = "Open in Browser";
            this.openInBrowserToolStripMenuItem1.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(161, 6);
            // 
            // copySelectedToolStripMenuItem
            // 
            this.copySelectedToolStripMenuItem.Name = "copySelectedToolStripMenuItem";
            this.copySelectedToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.copySelectedToolStripMenuItem.Text = "Copy Selected";
            this.copySelectedToolStripMenuItem.Click += new System.EventHandler(this.copySelectedToolStripMenuItem_Click);
            // 
            // ListComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 401);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnGo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ListComparer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "List comparer";
            this.mnuDuplicates.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.mnuList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbBoth;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.OpenFileDialog openListDialog;
        private System.Windows.Forms.Label lblBoth;
        private System.Windows.Forms.SaveFileDialog saveListDialog;
        private System.Windows.Forms.Button btnSaveBoth;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label3;
        private ListMaker listMaker1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private ListMaker listMaker2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveOnly2;
        private System.Windows.Forms.Label lblOnly2;
        private System.Windows.Forms.ListBox lbOnly2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveOnly1;
        private System.Windows.Forms.Label lblOnly1;
        private System.Windows.Forms.ListBox lbOnly1;
        private System.Windows.Forms.ContextMenuStrip mnuDuplicates;
        private System.Windows.Forms.ToolStripMenuItem transferDuplicatesToList1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip mnuList;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem copySelectedToolStripMenuItem;
    }
}

