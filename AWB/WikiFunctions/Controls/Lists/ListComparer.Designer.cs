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
            this.lbBoth = new WikiFunctions.Controls.Lists.ListBoxString();
            this.mnuList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferToListMaker1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transferToListMaker2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGo = new System.Windows.Forms.Button();
            this.lblNoBoth = new System.Windows.Forms.Label();
            this.btnSaveBoth = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblBoth = new System.Windows.Forms.Label();
            this.gbList1 = new System.Windows.Forms.GroupBox();
            this.listMaker1 = new WikiFunctions.Controls.Lists.ListMaker();
            this.gbList2 = new System.Windows.Forms.GroupBox();
            this.listMaker2 = new WikiFunctions.Controls.Lists.ListMaker();
            this.gbResults = new System.Windows.Forms.GroupBox();
            this.flwDiff2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblOnly2 = new System.Windows.Forms.Label();
            this.lblNo2 = new System.Windows.Forms.Label();
            this.lbNo2 = new WikiFunctions.Controls.Lists.ListBoxString();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveOnly2 = new System.Windows.Forms.Button();
            this.btnMoveOnly2 = new System.Windows.Forms.Button();
            this.flwBoth = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMoveCommon = new System.Windows.Forms.Button();
            this.flwDiff1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblOnly1 = new System.Windows.Forms.Label();
            this.lblNo1 = new System.Windows.Forms.Label();
            this.lbNo1 = new WikiFunctions.Controls.Lists.ListBoxString();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSaveOnly1 = new System.Windows.Forms.Button();
            this.btnMoveOnly1 = new System.Windows.Forms.Button();
            this.mnuList.SuspendLayout();
            this.gbList1.SuspendLayout();
            this.gbList2.SuspendLayout();
            this.gbResults.SuspendLayout();
            this.flwDiff2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flwBoth.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flwDiff1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbBoth
            // 
            this.lbBoth.ContextMenuStrip = this.mnuList;
            this.lbBoth.FormattingEnabled = true;
            this.lbBoth.Location = new System.Drawing.Point(3, 29);
            this.lbBoth.Name = "lbBoth";
            this.lbBoth.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbBoth.Size = new System.Drawing.Size(133, 264);
            this.lbBoth.TabIndex = 2;
            // 
            // mnuList
            // 
            this.mnuList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferToListMaker1ToolStripMenuItem,
            this.transferToListMaker2ToolStripMenuItem,
            this.toolStripSeparator1,
            this.openInBrowserToolStripMenuItem,
            this.toolStripSeparator2,
            this.copyToolStripMenuItem,
            this.removeSelectedToolStripMenuItem});
            this.mnuList.Name = "mnuDuplicates";
            this.mnuList.Size = new System.Drawing.Size(195, 148);
            // 
            // transferToListMaker1ToolStripMenuItem
            // 
            this.transferToListMaker1ToolStripMenuItem.Name = "transferToListMaker1ToolStripMenuItem";
            this.transferToListMaker1ToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.transferToListMaker1ToolStripMenuItem.Text = "Transfer to ListMaker 1";
            this.transferToListMaker1ToolStripMenuItem.Click += new System.EventHandler(this.transferDuplicatesToList1ToolStripMenuItem_Click);
            // 
            // transferToListMaker2ToolStripMenuItem
            // 
            this.transferToListMaker2ToolStripMenuItem.Name = "transferToListMaker2ToolStripMenuItem";
            this.transferToListMaker2ToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.transferToListMaker2ToolStripMenuItem.Text = "Transfer to ListMaker 2";
            this.transferToListMaker2ToolStripMenuItem.Click += new System.EventHandler(this.transferToListMaker2ToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(191, 6);
            // 
            // openInBrowserToolStripMenuItem
            // 
            this.openInBrowserToolStripMenuItem.Name = "openInBrowserToolStripMenuItem";
            this.openInBrowserToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.openInBrowserToolStripMenuItem.Text = "Open in Browser";
            this.openInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openInBrowserToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(191, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.copyToolStripMenuItem.Text = "Copy selected";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // removeSelectedToolStripMenuItem
            // 
            this.removeSelectedToolStripMenuItem.Name = "removeSelectedToolStripMenuItem";
            this.removeSelectedToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.removeSelectedToolStripMenuItem.Text = "Remove selected";
            this.removeSelectedToolStripMenuItem.Click += new System.EventHandler(this.removeSelectedToolStripMenuItem_Click);
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(460, 155);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(57, 56);
            this.btnGo.TabIndex = 5;
            this.btnGo.Text = "Compare";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // lblNoBoth
            // 
            this.lblNoBoth.AutoSize = true;
            this.lblNoBoth.Location = new System.Drawing.Point(3, 13);
            this.lblNoBoth.Name = "lblNoBoth";
            this.lblNoBoth.Size = new System.Drawing.Size(13, 13);
            this.lblNoBoth.TabIndex = 8;
            this.lblNoBoth.Text = "0";
            // 
            // btnSaveBoth
            // 
            this.btnSaveBoth.Location = new System.Drawing.Point(3, 3);
            this.btnSaveBoth.Name = "btnSaveBoth";
            this.btnSaveBoth.Size = new System.Drawing.Size(60, 27);
            this.btnSaveBoth.TabIndex = 9;
            this.btnSaveBoth.Text = "Save list";
            this.btnSaveBoth.UseVisualStyleBackColor = true;
            this.btnSaveBoth.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(184, 351);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear all";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblBoth
            // 
            this.lblBoth.AutoSize = true;
            this.lblBoth.Location = new System.Drawing.Point(3, 0);
            this.lblBoth.Name = "lblBoth";
            this.lblBoth.Size = new System.Drawing.Size(51, 13);
            this.lblBoth.TabIndex = 13;
            this.lblBoth.Text = "Common:";
            // 
            // gbList1
            // 
            this.gbList1.Controls.Add(this.listMaker1);
            this.gbList1.Location = new System.Drawing.Point(12, 12);
            this.gbList1.Name = "gbList1";
            this.gbList1.Size = new System.Drawing.Size(218, 377);
            this.gbList1.TabIndex = 16;
            this.gbList1.TabStop = false;
            this.gbList1.Text = "List 1";
            // 
            // listMaker1
            // 
            this.listMaker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMaker1.Location = new System.Drawing.Point(3, 16);
            this.listMaker1.Name = "listMaker1";
            this.listMaker1.SelectedProvider = "CategoryListProvider";
            this.listMaker1.Size = new System.Drawing.Size(212, 358);
            this.listMaker1.SourceText = "";
            this.listMaker1.SpecialFilterSettings = ((WikiFunctions.AWBSettings.SpecialFilterPrefs)(resources.GetObject("listMaker1.SpecialFilterSettings")));
            this.listMaker1.TabIndex = 14;
            // 
            // gbList2
            // 
            this.gbList2.Controls.Add(this.listMaker2);
            this.gbList2.Location = new System.Drawing.Point(236, 12);
            this.gbList2.Name = "gbList2";
            this.gbList2.Size = new System.Drawing.Size(218, 377);
            this.gbList2.TabIndex = 17;
            this.gbList2.TabStop = false;
            this.gbList2.Text = "List 2";
            // 
            // listMaker2
            // 
            this.listMaker2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMaker2.Location = new System.Drawing.Point(3, 16);
            this.listMaker2.Name = "listMaker2";
            this.listMaker2.SelectedProvider = "CategoryListProvider";
            this.listMaker2.Size = new System.Drawing.Size(212, 358);
            this.listMaker2.SourceText = "";
            this.listMaker2.SpecialFilterSettings = ((WikiFunctions.AWBSettings.SpecialFilterPrefs)(resources.GetObject("listMaker2.SpecialFilterSettings")));
            this.listMaker2.TabIndex = 14;
            // 
            // gbResults
            // 
            this.gbResults.Controls.Add(this.flwDiff2);
            this.gbResults.Controls.Add(this.flwBoth);
            this.gbResults.Controls.Add(this.flwDiff1);
            this.gbResults.Controls.Add(this.btnClear);
            this.gbResults.Location = new System.Drawing.Point(523, 12);
            this.gbResults.Name = "gbResults";
            this.gbResults.Size = new System.Drawing.Size(431, 377);
            this.gbResults.TabIndex = 18;
            this.gbResults.TabStop = false;
            this.gbResults.Text = "Results";
            // 
            // flwDiff2
            // 
            this.flwDiff2.Controls.Add(this.lblOnly2);
            this.flwDiff2.Controls.Add(this.lblNo2);
            this.flwDiff2.Controls.Add(this.lbNo2);
            this.flwDiff2.Controls.Add(this.flowLayoutPanel3);
            this.flwDiff2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwDiff2.Location = new System.Drawing.Point(284, 16);
            this.flwDiff2.Name = "flwDiff2";
            this.flwDiff2.Size = new System.Drawing.Size(140, 336);
            this.flwDiff2.TabIndex = 24;
            // 
            // lblOnly2
            // 
            this.lblOnly2.AutoSize = true;
            this.lblOnly2.Location = new System.Drawing.Point(3, 0);
            this.lblOnly2.Name = "lblOnly2";
            this.lblOnly2.Size = new System.Drawing.Size(83, 13);
            this.lblOnly2.TabIndex = 21;
            this.lblOnly2.Text = "Unqiue in List 2:";
            // 
            // lblNo2
            // 
            this.lblNo2.AutoSize = true;
            this.lblNo2.Location = new System.Drawing.Point(3, 13);
            this.lblNo2.Name = "lblNo2";
            this.lblNo2.Size = new System.Drawing.Size(13, 13);
            this.lblNo2.TabIndex = 19;
            this.lblNo2.Text = "0";
            // 
            // lbNo2
            // 
            this.lbNo2.ContextMenuStrip = this.mnuList;
            this.lbNo2.FormattingEnabled = true;
            this.lbNo2.Location = new System.Drawing.Point(3, 29);
            this.lbNo2.Name = "lbNo2";
            this.lbNo2.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbNo2.Size = new System.Drawing.Size(133, 264);
            this.lbNo2.TabIndex = 18;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.btnSaveOnly2);
            this.flowLayoutPanel3.Controls.Add(this.btnMoveOnly2);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 299);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(133, 33);
            this.flowLayoutPanel3.TabIndex = 27;
            // 
            // btnSaveOnly2
            // 
            this.btnSaveOnly2.Location = new System.Drawing.Point(3, 3);
            this.btnSaveOnly2.Name = "btnSaveOnly2";
            this.btnSaveOnly2.Size = new System.Drawing.Size(60, 27);
            this.btnSaveOnly2.TabIndex = 20;
            this.btnSaveOnly2.Text = "Save list";
            this.btnSaveOnly2.UseVisualStyleBackColor = true;
            this.btnSaveOnly2.Click += new System.EventHandler(this.btnSaveOnly2_Click);
            // 
            // btnMoveOnly2
            // 
            this.btnMoveOnly2.Enabled = false;
            this.btnMoveOnly2.Location = new System.Drawing.Point(69, 3);
            this.btnMoveOnly2.Name = "btnMoveOnly2";
            this.btnMoveOnly2.Size = new System.Drawing.Size(60, 27);
            this.btnMoveOnly2.TabIndex = 17;
            this.btnMoveOnly2.Text = "Use list";
            this.btnMoveOnly2.UseVisualStyleBackColor = true;
            this.btnMoveOnly2.Click += new System.EventHandler(this.btnMoveOnly2_Click);
            // 
            // flwBoth
            // 
            this.flwBoth.Controls.Add(this.lblBoth);
            this.flwBoth.Controls.Add(this.lblNoBoth);
            this.flwBoth.Controls.Add(this.lbBoth);
            this.flwBoth.Controls.Add(this.flowLayoutPanel2);
            this.flwBoth.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwBoth.Location = new System.Drawing.Point(145, 16);
            this.flwBoth.Name = "flwBoth";
            this.flwBoth.Size = new System.Drawing.Size(140, 336);
            this.flwBoth.TabIndex = 23;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.btnSaveBoth);
            this.flowLayoutPanel2.Controls.Add(this.btnMoveCommon);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 299);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(133, 33);
            this.flowLayoutPanel2.TabIndex = 26;
            // 
            // btnMoveCommon
            // 
            this.btnMoveCommon.Enabled = false;
            this.btnMoveCommon.Location = new System.Drawing.Point(69, 3);
            this.btnMoveCommon.Name = "btnMoveCommon";
            this.btnMoveCommon.Size = new System.Drawing.Size(60, 27);
            this.btnMoveCommon.TabIndex = 17;
            this.btnMoveCommon.Text = "Use list";
            this.btnMoveCommon.UseVisualStyleBackColor = true;
            this.btnMoveCommon.Click += new System.EventHandler(this.btnMoveCommon_Click);
            // 
            // flwDiff1
            // 
            this.flwDiff1.Controls.Add(this.lblOnly1);
            this.flwDiff1.Controls.Add(this.lblNo1);
            this.flwDiff1.Controls.Add(this.lbNo1);
            this.flwDiff1.Controls.Add(this.flowLayoutPanel1);
            this.flwDiff1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flwDiff1.Location = new System.Drawing.Point(6, 16);
            this.flwDiff1.Name = "flwDiff1";
            this.flwDiff1.Size = new System.Drawing.Size(140, 336);
            this.flwDiff1.TabIndex = 22;
            // 
            // lblOnly1
            // 
            this.lblOnly1.AutoSize = true;
            this.lblOnly1.Location = new System.Drawing.Point(3, 0);
            this.lblOnly1.Name = "lblOnly1";
            this.lblOnly1.Size = new System.Drawing.Size(83, 13);
            this.lblOnly1.TabIndex = 17;
            this.lblOnly1.Text = "Unique in List 1:";
            // 
            // lblNo1
            // 
            this.lblNo1.AutoSize = true;
            this.lblNo1.Location = new System.Drawing.Point(3, 13);
            this.lblNo1.Name = "lblNo1";
            this.lblNo1.Size = new System.Drawing.Size(13, 13);
            this.lblNo1.TabIndex = 15;
            this.lblNo1.Text = "0";
            // 
            // lbNo1
            // 
            this.lbNo1.ContextMenuStrip = this.mnuList;
            this.lbNo1.FormattingEnabled = true;
            this.lbNo1.Location = new System.Drawing.Point(3, 29);
            this.lbNo1.Name = "lbNo1";
            this.lbNo1.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbNo1.Size = new System.Drawing.Size(133, 264);
            this.lbNo1.TabIndex = 14;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnSaveOnly1);
            this.flowLayoutPanel1.Controls.Add(this.btnMoveOnly1);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 299);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(133, 33);
            this.flowLayoutPanel1.TabIndex = 25;
            // 
            // btnSaveOnly1
            // 
            this.btnSaveOnly1.Location = new System.Drawing.Point(3, 3);
            this.btnSaveOnly1.Name = "btnSaveOnly1";
            this.btnSaveOnly1.Size = new System.Drawing.Size(60, 27);
            this.btnSaveOnly1.TabIndex = 16;
            this.btnSaveOnly1.Text = "Save list";
            this.btnSaveOnly1.UseVisualStyleBackColor = true;
            this.btnSaveOnly1.Click += new System.EventHandler(this.btnSaveOnly1_Click);
            // 
            // btnMoveOnly1
            // 
            this.btnMoveOnly1.Enabled = false;
            this.btnMoveOnly1.Location = new System.Drawing.Point(69, 3);
            this.btnMoveOnly1.Name = "btnMoveOnly1";
            this.btnMoveOnly1.Size = new System.Drawing.Size(60, 27);
            this.btnMoveOnly1.TabIndex = 17;
            this.btnMoveOnly1.Text = "Use list";
            this.btnMoveOnly1.UseVisualStyleBackColor = true;
            this.btnMoveOnly1.Click += new System.EventHandler(this.btnMoveOnly1_Click);
            // 
            // ListComparer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 401);
            this.Controls.Add(this.gbList1);
            this.Controls.Add(this.gbList2);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.gbResults);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ListComparer";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "List comparer";
            this.mnuList.ResumeLayout(false);
            this.gbList1.ResumeLayout(false);
            this.gbList2.ResumeLayout(false);
            this.gbResults.ResumeLayout(false);
            this.flwDiff2.ResumeLayout(false);
            this.flwDiff2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flwBoth.ResumeLayout(false);
            this.flwBoth.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flwDiff1.ResumeLayout(false);
            this.flwDiff1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ListBoxString lbBoth;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Label lblNoBoth;
        private System.Windows.Forms.Button btnSaveBoth;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblBoth;
        private ListMaker listMaker1;
        private System.Windows.Forms.GroupBox gbList1;
        private System.Windows.Forms.GroupBox gbList2;
        private ListMaker listMaker2;
        private System.Windows.Forms.GroupBox gbResults;
        private System.Windows.Forms.Label lblOnly2;
        private System.Windows.Forms.Button btnSaveOnly2;
        private System.Windows.Forms.Label lblNo2;
        private ListBoxString lbNo2;
        private System.Windows.Forms.Label lblOnly1;
        private System.Windows.Forms.Button btnSaveOnly1;
        private System.Windows.Forms.Label lblNo1;
        private ListBoxString lbNo1;
        private System.Windows.Forms.ContextMenuStrip mnuList;
        private System.Windows.Forms.ToolStripMenuItem transferToListMaker1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.FlowLayoutPanel flwDiff2;
        private System.Windows.Forms.FlowLayoutPanel flwBoth;
        private System.Windows.Forms.FlowLayoutPanel flwDiff1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnMoveOnly1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btnMoveOnly2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnMoveCommon;
        private System.Windows.Forms.ToolStripMenuItem removeSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transferToListMaker2ToolStripMenuItem;
    }
}

