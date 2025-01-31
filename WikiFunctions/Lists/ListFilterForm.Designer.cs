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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListFilterForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtContains = new System.Windows.Forms.RichTextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.chkNotContains = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.RichTextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtContains
            // 
            resources.ApplyResources(this.txtContains, "txtContains");
            this.txtContains.DetectUrls = false;
            this.txtContains.Name = "txtContains";
            // 
            // chkContains
            // 
            resources.ApplyResources(this.chkContains, "chkContains");
            this.chkContains.Name = "chkContains";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            resources.ApplyResources(this.chkIsRegex, "chkIsRegex");
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // chkNotContains
            // 
            resources.ApplyResources(this.chkNotContains, "chkNotContains");
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            resources.ApplyResources(this.txtDoesNotContain, "txtDoesNotContain");
            this.txtDoesNotContain.DetectUrls = false;
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            // 
            // gbNamespaces
            // 
            resources.ApplyResources(this.gbNamespaces, "gbNamespaces");
            this.gbNamespaces.Controls.Add(this.pageNamespaces);
            this.gbNamespaces.Name = "gbNamespaces";
            this.gbNamespaces.TabStop = false;
            // 
            // pageNamespaces
            // 
            resources.ApplyResources(this.pageNamespaces, "pageNamespaces");
            this.pageNamespaces.Name = "pageNamespaces";
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.chkIsRegex);
            this.gbSearch.Controls.Add(this.txtDoesNotContain);
            this.gbSearch.Controls.Add(this.chkNotContains);
            this.gbSearch.Controls.Add(this.txtContains);
            this.gbSearch.Controls.Add(this.chkContains);
            resources.ApplyResources(this.gbSearch, "gbSearch");
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.TabStop = false;
            // 
            // gbSets
            // 
            resources.ApplyResources(this.gbSets, "gbSets");
            this.gbSets.Controls.Add(this.btnClear);
            this.gbSets.Controls.Add(this.btnGetList);
            this.gbSets.Controls.Add(this.lbRemove);
            this.gbSets.Controls.Add(this.cbOpType);
            this.gbSets.Name = "gbSets";
            this.gbSets.TabStop = false;
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnGetList
            // 
            resources.ApplyResources(this.btnGetList, "btnGetList");
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_Click);
            // 
            // lbRemove
            // 
            resources.ApplyResources(this.lbRemove, "lbRemove");
            this.lbRemove.FormattingEnabled = true;
            this.lbRemove.Name = "lbRemove";
            // 
            // cbOpType
            // 
            resources.ApplyResources(this.cbOpType, "cbOpType");
            this.cbOpType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOpType.FormattingEnabled = true;
            this.cbOpType.Items.AddRange(new object[] {
            resources.GetString("cbOpType.Items"),
            resources.GetString("cbOpType.Items1")});
            this.cbOpType.Name = "cbOpType";
            // 
            // gbMisc
            // 
            this.gbMisc.Controls.Add(this.flwOther);
            resources.ApplyResources(this.gbMisc, "gbMisc");
            this.gbMisc.Name = "gbMisc";
            this.gbMisc.TabStop = false;
            // 
            // flwOther
            // 
            this.flwOther.Controls.Add(this.chkSortAZ);
            this.flwOther.Controls.Add(this.chkRemoveDups);
            resources.ApplyResources(this.flwOther, "flwOther");
            this.flwOther.Name = "flwOther";
            // 
            // chkSortAZ
            // 
            resources.ApplyResources(this.chkSortAZ, "chkSortAZ");
            this.chkSortAZ.Checked = true;
            this.chkSortAZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortAZ.Name = "chkSortAZ";
            this.chkSortAZ.UseVisualStyleBackColor = true;
            // 
            // chkRemoveDups
            // 
            resources.ApplyResources(this.chkRemoveDups, "chkRemoveDups");
            this.chkRemoveDups.Checked = true;
            this.chkRemoveDups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemoveDups.Name = "chkRemoveDups";
            this.chkRemoveDups.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
            // 
            // ListFilterForm
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ListFilterForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RichTextBox txtContains;
        private System.Windows.Forms.CheckBox chkContains;
        private System.Windows.Forms.CheckBox chkIsRegex;
        private System.Windows.Forms.CheckBox chkNotContains;
        private System.Windows.Forms.RichTextBox txtDoesNotContain;
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
