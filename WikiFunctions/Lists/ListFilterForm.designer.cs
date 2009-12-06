namespace WikiFunctions.Lists
{
    partial class ListFilterForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.chkNotContains = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.gbNamespaces = new System.Windows.Forms.GroupBox();
            this.pageNamespaces = new WikiFunctions.Controls.NamespacesControl();
            this.gbSearch = new System.Windows.Forms.GroupBox();
            this.gbSets = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnGetList = new System.Windows.Forms.Button();
            this.lbRemove = new WikiFunctions.Controls.Lists.ListBoxArticle();
            this.cbOpType = new System.Windows.Forms.ComboBox();
            this.gbMisc = new System.Windows.Forms.GroupBox();
            this.flwOther = new System.Windows.Forms.FlowLayoutPanel();
            this.chkSortAZ = new System.Windows.Forms.CheckBox();
            this.chkRemoveDups = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gbNamespaces.SuspendLayout();
            this.gbSearch.SuspendLayout();
            this.gbSets.SuspendLayout();
            this.gbMisc.SuspendLayout();
            this.flwOther.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(655, 12);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Apply";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(655, 41);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtContains
            // 
            this.txtContains.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(6, 41);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(180, 20);
            this.txtContains.TabIndex = 1;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(6, 19);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(145, 17);
            this.chkContains.TabIndex = 0;
            this.chkContains.Text = "Remove titles &containing:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            this.chkIsRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Enabled = false;
            this.chkIsRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.chkIsRegex.Location = new System.Drawing.Point(65, 113);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(121, 17);
            this.chkIsRegex.TabIndex = 4;
            this.chkIsRegex.Text = "&Regular expressions";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkNotContains
            // 
            this.chkNotContains.AutoSize = true;
            this.chkNotContains.Location = new System.Drawing.Point(6, 67);
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.Size = new System.Drawing.Size(130, 17);
            this.chkNotContains.TabIndex = 2;
            this.chkNotContains.Text = "Keep titles co&ntaining:";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(6, 87);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(180, 20);
            this.txtDoesNotContain.TabIndex = 3;
            // 
            // gbNamespaces
            // 
            this.gbNamespaces.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbNamespaces.Controls.Add(this.pageNamespaces);
            this.gbNamespaces.Location = new System.Drawing.Point(3, 0);
            this.gbNamespaces.Name = "gbNamespaces";
            this.gbNamespaces.Size = new System.Drawing.Size(232, 267);
            this.gbNamespaces.TabIndex = 0;
            this.gbNamespaces.TabStop = false;
            this.gbNamespaces.Text = "Namespaces to keep";
            // 
            // pageNamespaces
            // 
            this.pageNamespaces.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pageNamespaces.Location = new System.Drawing.Point(3, 16);
            this.pageNamespaces.Name = "pageNamespaces";
            this.pageNamespaces.Size = new System.Drawing.Size(224, 242);
            this.pageNamespaces.TabIndex = 0;
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.chkIsRegex);
            this.gbSearch.Controls.Add(this.txtDoesNotContain);
            this.gbSearch.Controls.Add(this.chkNotContains);
            this.gbSearch.Controls.Add(this.txtContains);
            this.gbSearch.Controls.Add(this.chkContains);
            this.gbSearch.Location = new System.Drawing.Point(3, 3);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(192, 141);
            this.gbSearch.TabIndex = 1;
            this.gbSearch.TabStop = false;
            this.gbSearch.Text = "Matches";
            // 
            // gbSets
            // 
            this.gbSets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSets.Controls.Add(this.btnClear);
            this.gbSets.Controls.Add(this.btnGetList);
            this.gbSets.Controls.Add(this.lbRemove);
            this.gbSets.Controls.Add(this.cbOpType);
            this.gbSets.Location = new System.Drawing.Point(201, 3);
            this.gbSets.Name = "gbSets";
            this.gbSets.Size = new System.Drawing.Size(206, 264);
            this.gbSets.TabIndex = 3;
            this.gbSets.TabStop = false;
            this.gbSets.Text = "Set operations";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(117, 235);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(83, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetList
            // 
            this.btnGetList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetList.Location = new System.Drawing.Point(6, 235);
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.Size = new System.Drawing.Size(83, 23);
            this.btnGetList.TabIndex = 2;
            this.btnGetList.Text = "&Open file";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
            // 
            // lbRemove
            // 
            this.lbRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRemove.FormattingEnabled = true;
            this.lbRemove.Location = new System.Drawing.Point(6, 44);
            this.lbRemove.Name = "lbRemove";
            this.lbRemove.Size = new System.Drawing.Size(194, 173);
            this.lbRemove.TabIndex = 1;
            // 
            // cbOpType
            // 
            this.cbOpType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbOpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpType.FormattingEnabled = true;
            this.cbOpType.Items.AddRange(new object[] {
            "Symmetric difference",
            "Intersection"});
            this.cbOpType.Location = new System.Drawing.Point(6, 19);
            this.cbOpType.Name = "cbOpType";
            this.cbOpType.Size = new System.Drawing.Size(194, 21);
            this.cbOpType.TabIndex = 0;
            // 
            // gbMisc
            // 
            this.gbMisc.Controls.Add(this.flwOther);
            this.gbMisc.Location = new System.Drawing.Point(3, 150);
            this.gbMisc.Name = "gbMisc";
            this.gbMisc.Size = new System.Drawing.Size(192, 63);
            this.gbMisc.TabIndex = 2;
            this.gbMisc.TabStop = false;
            this.gbMisc.Text = "Other";
            // 
            // flwOther
            // 
            this.flwOther.Controls.Add(this.chkSortAZ);
            this.flwOther.Controls.Add(this.chkRemoveDups);
            this.flwOther.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwOther.Location = new System.Drawing.Point(3, 16);
            this.flwOther.Name = "flwOther";
            this.flwOther.Size = new System.Drawing.Size(186, 44);
            this.flwOther.TabIndex = 0;
            // 
            // chkSortAZ
            // 
            this.chkSortAZ.AutoSize = true;
            this.chkSortAZ.Checked = true;
            this.chkSortAZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortAZ.Location = new System.Drawing.Point(3, 3);
            this.chkSortAZ.Name = "chkSortAZ";
            this.chkSortAZ.Size = new System.Drawing.Size(112, 17);
            this.chkSortAZ.TabIndex = 0;
            this.chkSortAZ.Text = "Sort alpha&betically";
            this.chkSortAZ.UseVisualStyleBackColor = true;
            // 
            // chkRemoveDups
            // 
            this.chkRemoveDups.AutoSize = true;
            this.chkRemoveDups.Checked = true;
            this.chkRemoveDups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveDups.Location = new System.Drawing.Point(3, 26);
            this.chkRemoveDups.Name = "chkRemoveDups";
            this.chkRemoveDups.Size = new System.Drawing.Size(117, 17);
            this.chkRemoveDups.TabIndex = 1;
            this.chkRemoveDups.Text = "Remove &duplicates";
            this.chkRemoveDups.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(1, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gbNamespaces);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gbSearch);
            this.splitContainer1.Panel2.Controls.Add(this.gbMisc);
            this.splitContainer1.Panel2.Controls.Add(this.gbSets);
            this.splitContainer1.Size = new System.Drawing.Size(648, 282);
            this.splitContainer1.SplitterDistance = 237;
            this.splitContainer1.TabIndex = 0;
            // 
            // ListFilterForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(742, 284);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(750, 310);
            this.Name = "ListFilterForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "List Filter";
            this.Load += new System.EventHandler(this.specialFilter_Load);
            this.VisibleChanged += new System.EventHandler(this.SpecialFilter_VisibleChanged);
            this.gbNamespaces.ResumeLayout(false);
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            this.gbSets.ResumeLayout(false);
            this.gbMisc.ResumeLayout(false);
            this.flwOther.ResumeLayout(false);
            this.flwOther.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContains;
        private System.Windows.Forms.CheckBox chkIsRegex;
        private System.Windows.Forms.CheckBox chkNotContains;
        private System.Windows.Forms.TextBox txtDoesNotContain;
        private System.Windows.Forms.GroupBox gbNamespaces;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.GroupBox gbSets;
        private System.Windows.Forms.Button btnGetList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox gbMisc;
        private System.Windows.Forms.CheckBox chkRemoveDups;
        private WikiFunctions.Controls.Lists.ListBoxArticle lbRemove;
        private System.Windows.Forms.ComboBox cbOpType;
        private System.Windows.Forms.CheckBox chkSortAZ;
        private System.Windows.Forms.FlowLayoutPanel flwOther;
        private WikiFunctions.Controls.NamespacesControl pageNamespaces;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
