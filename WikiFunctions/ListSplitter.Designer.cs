namespace WikiFunctions.Lists
{
    partial class ListSplitter
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
            this.btnSplitList = new System.Windows.Forms.Button();
            this.numSplitAmount = new System.Windows.Forms.NumericUpDown();
            this.lvSplit = new System.Windows.Forms.ListView();
            this.colArticleName = new System.Windows.Forms.ColumnHeader();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.listMaker1 = new WikiFunctions.Lists.ListMaker();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numSplitAmount)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSplitList
            // 
            this.btnSplitList.Location = new System.Drawing.Point(241, 198);
            this.btnSplitList.Name = "btnSplitList";
            this.btnSplitList.Size = new System.Drawing.Size(72, 35);
            this.btnSplitList.TabIndex = 1;
            this.btnSplitList.Text = "Split!";
            this.btnSplitList.UseVisualStyleBackColor = true;
            this.btnSplitList.Click += new System.EventHandler(this.btnSplitList_Click);
            // 
            // numSplitAmount
            // 
            this.numSplitAmount.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSplitAmount.Location = new System.Drawing.Point(219, 172);
            this.numSplitAmount.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numSplitAmount.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSplitAmount.Name = "numSplitAmount";
            this.numSplitAmount.Size = new System.Drawing.Size(120, 20);
            this.numSplitAmount.TabIndex = 2;
            this.numSplitAmount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lvSplit
            // 
            this.lvSplit.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colArticleName});
            this.lvSplit.ContextMenuStrip = this.contextMenuStrip;
            this.lvSplit.Location = new System.Drawing.Point(345, 12);
            this.lvSplit.Name = "lvSplit";
            this.lvSplit.Size = new System.Drawing.Size(244, 323);
            this.lvSplit.TabIndex = 3;
            this.lvSplit.UseCompatibleStateImageBehavior = false;
            this.lvSplit.View = System.Windows.Forms.View.Details;
            // 
            // colArticleName
            // 
            this.colArticleName.Text = "Article Name";
            this.colArticleName.Width = 228;
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(384, 341);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(100, 20);
            this.txtFile.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(352, 344);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "File:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(490, 341);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // listMaker1
            // 
            this.listMaker1.ListFile = "";
            this.listMaker1.Location = new System.Drawing.Point(12, 12);
            this.listMaker1.Name = "listMaker1";
            this.listMaker1.SelectedSource = WikiFunctions.Lists.SourceType.None;
            this.listMaker1.Size = new System.Drawing.Size(201, 351);
            this.listMaker1.SourceText = "";
            this.listMaker1.TabIndex = 0;
            this.listMaker1.WikiStatus = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(111, 26);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Split Size:";
            // 
            // ListSplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 371);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFile);
            this.Controls.Add(this.lvSplit);
            this.Controls.Add(this.numSplitAmount);
            this.Controls.Add(this.btnSplitList);
            this.Controls.Add(this.listMaker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ListSplitter";
            this.Text = "List Splitter";
            this.Load += new System.EventHandler(this.ListSplitter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numSplitAmount)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListMaker listMaker1;
        private System.Windows.Forms.Button btnSplitList;
        private System.Windows.Forms.NumericUpDown numSplitAmount;
        private System.Windows.Forms.ListView lvSplit;
        private System.Windows.Forms.ColumnHeader colArticleName;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.Label label2;
    }
}