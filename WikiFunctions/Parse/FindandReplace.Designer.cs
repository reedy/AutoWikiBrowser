/*
    Autowikibrowser
    Copyright (C) 2007 Martin Richards

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

namespace WikiFunctions.Parse
{
    partial class FindandReplace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindandReplace));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.find = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.casesensitive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.regex = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.multi = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.single = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Minor = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.BeforeOrAfter = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FindAndReplaceContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.testRegexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRetfRuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.moveToTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.allCaseSensitiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllMultlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllRegularExpressionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllSinglelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllBeforeOrAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.uncheckAllCaseSensitiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMultilineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllRegularExpressionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllSinglelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllBeforeOrAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDone = new System.Windows.Forms.Button();
            this.lnkWpRE = new System.Windows.Forms.LinkLabel();
            this.btnClear = new System.Windows.Forms.Button();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.chkIgnoreLinks = new System.Windows.Forms.CheckBox();
            this.chkAddToSummary = new System.Windows.Forms.CheckBox();
            this.chkIgnoreMore = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMsdn = new System.Windows.Forms.LinkLabel();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.FindAndReplaceContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.find,
            this.replace,
            this.casesensitive,
            this.regex,
            this.multi,
            this.single,
            this.Minor,
            this.BeforeOrAfter,
            this.enabled,
            this.Comment});
            this.dataGridView1.ContextMenuStrip = this.FindAndReplaceContextMenu;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // find
            // 
            this.find.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.find.FillWeight = 92.50688F;
            resources.ApplyResources(this.find, "find");
            this.find.Name = "find";
            this.find.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // replace
            // 
            this.replace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.NullValue = null;
            this.replace.DefaultCellStyle = dataGridViewCellStyle1;
            this.replace.FillWeight = 87.66718F;
            resources.ApplyResources(this.replace, "replace");
            this.replace.Name = "replace";
            // 
            // casesensitive
            // 
            this.casesensitive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.casesensitive.FalseValue = false;
            this.casesensitive.FillWeight = 71.79286F;
            resources.ApplyResources(this.casesensitive, "casesensitive");
            this.casesensitive.Name = "casesensitive";
            this.casesensitive.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.casesensitive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.casesensitive.TrueValue = true;
            // 
            // regex
            // 
            this.regex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.regex.FalseValue = false;
            this.regex.FillWeight = 131.5755F;
            resources.ApplyResources(this.regex, "regex");
            this.regex.Name = "regex";
            this.regex.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.regex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.regex.TrueValue = true;
            // 
            // multi
            // 
            this.multi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.multi.FalseValue = false;
            this.multi.FillWeight = 150.1532F;
            resources.ApplyResources(this.multi, "multi");
            this.multi.Name = "multi";
            this.multi.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.multi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.multi.TrueValue = true;
            // 
            // single
            // 
            this.single.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.single.FalseValue = false;
            this.single.FillWeight = 173.4641F;
            resources.ApplyResources(this.single, "single");
            this.single.Name = "single";
            this.single.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.single.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.single.TrueValue = true;
            // 
            // Minor
            // 
            this.Minor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Minor.FalseValue = false;
            resources.ApplyResources(this.Minor, "Minor");
            this.Minor.Name = "Minor";
            this.Minor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Minor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Minor.TrueValue = true;
            // 
            // BeforeOrAfter
            // 
            this.BeforeOrAfter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.BeforeOrAfter.FalseValue = false;
            resources.ApplyResources(this.BeforeOrAfter, "BeforeOrAfter");
            this.BeforeOrAfter.Name = "BeforeOrAfter";
            this.BeforeOrAfter.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BeforeOrAfter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.BeforeOrAfter.TrueValue = true;
            // 
            // enabled
            // 
            this.enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.enabled.FalseValue = false;
            resources.ApplyResources(this.enabled, "enabled");
            this.enabled.Name = "enabled";
            this.enabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.enabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.enabled.TrueValue = true;
            // 
            // Comment
            // 
            this.Comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.Comment, "Comment");
            this.Comment.Name = "Comment";
            // 
            // FindAndReplaceContextMenu
            // 
            this.FindAndReplaceContextMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.FindAndReplaceContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.toolStripSeparator1,
            this.testRegexToolStripMenuItem,
            this.createRetfRuleToolStripMenuItem,
            this.toolStripSeparator4,
            this.moveToTopToolStripMenuItem,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.moveToBottomToolStripMenuItem,
            this.toolStripSeparator3,
            this.allCaseSensitiveToolStripMenuItem,
            this.checkAllMinorToolStripMenuItem,
            this.checkAllMultlineToolStripMenuItem,
            this.checkAllRegularExpressionsToolStripMenuItem,
            this.checkAllSinglelineToolStripMenuItem,
            this.checkAllBeforeOrAfterToolStripMenuItem,
            this.enableAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.uncheckAllCaseSensitiveToolStripMenuItem,
            this.uncheckAllMinorToolStripMenuItem,
            this.uncheckAllMultilineToolStripMenuItem,
            this.uncheckAllRegularExpressionsToolStripMenuItem,
            this.uncheckAllSinglelineToolStripMenuItem,
            this.uncheckAllBeforeOrAfterToolStripMenuItem,
            this.disableAllToolStripMenuItem});
            this.FindAndReplaceContextMenu.Name = "FindAndReplaceContextMenu";
            resources.ApplyResources(this.FindAndReplaceContextMenu, "FindAndReplaceContextMenu");
            this.FindAndReplaceContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FindAndReplaceContextMenu_Opening);
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            resources.ApplyResources(this.addRowToolStripMenuItem, "addRowToolStripMenuItem");
            this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // testRegexToolStripMenuItem
            // 
            this.testRegexToolStripMenuItem.Name = "testRegexToolStripMenuItem";
            resources.ApplyResources(this.testRegexToolStripMenuItem, "testRegexToolStripMenuItem");
            this.testRegexToolStripMenuItem.Click += new System.EventHandler(this.testRegexToolStripMenuItem_Click);
            // 
            // createRetfRuleToolStripMenuItem
            // 
            this.createRetfRuleToolStripMenuItem.Name = "createRetfRuleToolStripMenuItem";
            resources.ApplyResources(this.createRetfRuleToolStripMenuItem, "createRetfRuleToolStripMenuItem");
            this.createRetfRuleToolStripMenuItem.Click += new System.EventHandler(this.createRetfRuleToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            resources.ApplyResources(this.moveToTopToolStripMenuItem, "moveToTopToolStripMenuItem");
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            resources.ApplyResources(this.moveUpToolStripMenuItem, "moveUpToolStripMenuItem");
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            resources.ApplyResources(this.moveDownToolStripMenuItem, "moveDownToolStripMenuItem");
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            resources.ApplyResources(this.moveToBottomToolStripMenuItem, "moveToBottomToolStripMenuItem");
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // allCaseSensitiveToolStripMenuItem
            // 
            this.allCaseSensitiveToolStripMenuItem.Name = "allCaseSensitiveToolStripMenuItem";
            resources.ApplyResources(this.allCaseSensitiveToolStripMenuItem, "allCaseSensitiveToolStripMenuItem");
            this.allCaseSensitiveToolStripMenuItem.Click += new System.EventHandler(this.allCaseSensitiveToolStripMenuItem_Click);
            // 
            // checkAllMinorToolStripMenuItem
            // 
            this.checkAllMinorToolStripMenuItem.Name = "checkAllMinorToolStripMenuItem";
            resources.ApplyResources(this.checkAllMinorToolStripMenuItem, "checkAllMinorToolStripMenuItem");
            this.checkAllMinorToolStripMenuItem.Click += new System.EventHandler(this.checkAllMinorToolStripMenuItem_Click);
            // 
            // checkAllMultlineToolStripMenuItem
            // 
            this.checkAllMultlineToolStripMenuItem.Name = "checkAllMultlineToolStripMenuItem";
            resources.ApplyResources(this.checkAllMultlineToolStripMenuItem, "checkAllMultlineToolStripMenuItem");
            this.checkAllMultlineToolStripMenuItem.Click += new System.EventHandler(this.checkAllMultlineToolStripMenuItem_Click);
            // 
            // checkAllRegularExpressionsToolStripMenuItem
            // 
            this.checkAllRegularExpressionsToolStripMenuItem.Name = "checkAllRegularExpressionsToolStripMenuItem";
            resources.ApplyResources(this.checkAllRegularExpressionsToolStripMenuItem, "checkAllRegularExpressionsToolStripMenuItem");
            this.checkAllRegularExpressionsToolStripMenuItem.Click += new System.EventHandler(this.checkAllRegularExpressionsToolStripMenuItem_Click);
            // 
            // checkAllSinglelineToolStripMenuItem
            // 
            this.checkAllSinglelineToolStripMenuItem.Name = "checkAllSinglelineToolStripMenuItem";
            resources.ApplyResources(this.checkAllSinglelineToolStripMenuItem, "checkAllSinglelineToolStripMenuItem");
            this.checkAllSinglelineToolStripMenuItem.Click += new System.EventHandler(this.checkAllSinglelineToolStripMenuItem_Click);
            // 
            // checkAllBeforeOrAfterToolStripMenuItem
            // 
            this.checkAllBeforeOrAfterToolStripMenuItem.Name = "checkAllBeforeOrAfterToolStripMenuItem";
            resources.ApplyResources(this.checkAllBeforeOrAfterToolStripMenuItem, "checkAllBeforeOrAfterToolStripMenuItem");
            this.checkAllBeforeOrAfterToolStripMenuItem.Click += new System.EventHandler(this.checkAllBeforeOrAfterToolStripMenuItem_Click);
            // 
            // enableAllToolStripMenuItem
            // 
            this.enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            resources.ApplyResources(this.enableAllToolStripMenuItem, "enableAllToolStripMenuItem");
            this.enableAllToolStripMenuItem.Click += new System.EventHandler(this.enableAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // uncheckAllCaseSensitiveToolStripMenuItem
            // 
            this.uncheckAllCaseSensitiveToolStripMenuItem.Name = "uncheckAllCaseSensitiveToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllCaseSensitiveToolStripMenuItem, "uncheckAllCaseSensitiveToolStripMenuItem");
            this.uncheckAllCaseSensitiveToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllCaseSensitiveToolStripMenuItem_Click);
            // 
            // uncheckAllMinorToolStripMenuItem
            // 
            this.uncheckAllMinorToolStripMenuItem.Name = "uncheckAllMinorToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllMinorToolStripMenuItem, "uncheckAllMinorToolStripMenuItem");
            this.uncheckAllMinorToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllMinorToolStripMenuItem_Click);
            // 
            // uncheckAllMultilineToolStripMenuItem
            // 
            this.uncheckAllMultilineToolStripMenuItem.Name = "uncheckAllMultilineToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllMultilineToolStripMenuItem, "uncheckAllMultilineToolStripMenuItem");
            this.uncheckAllMultilineToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllMultilineToolStripMenuItem_Click);
            // 
            // uncheckAllRegularExpressionsToolStripMenuItem
            // 
            this.uncheckAllRegularExpressionsToolStripMenuItem.Name = "uncheckAllRegularExpressionsToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllRegularExpressionsToolStripMenuItem, "uncheckAllRegularExpressionsToolStripMenuItem");
            this.uncheckAllRegularExpressionsToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllRegularExpressionsToolStripMenuItem_Click);
            // 
            // uncheckAllSinglelineToolStripMenuItem
            // 
            this.uncheckAllSinglelineToolStripMenuItem.Name = "uncheckAllSinglelineToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllSinglelineToolStripMenuItem, "uncheckAllSinglelineToolStripMenuItem");
            this.uncheckAllSinglelineToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllSinglelineToolStripMenuItem_Click);
            // 
            // uncheckAllBeforeOrAfterToolStripMenuItem
            // 
            this.uncheckAllBeforeOrAfterToolStripMenuItem.Name = "uncheckAllBeforeOrAfterToolStripMenuItem";
            resources.ApplyResources(this.uncheckAllBeforeOrAfterToolStripMenuItem, "uncheckAllBeforeOrAfterToolStripMenuItem");
            this.uncheckAllBeforeOrAfterToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllBeforeOrAfterToolStripMenuItem_Click);
            // 
            // disableAllToolStripMenuItem
            // 
            this.disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            resources.ApplyResources(this.disableAllToolStripMenuItem, "disableAllToolStripMenuItem");
            this.disableAllToolStripMenuItem.Click += new System.EventHandler(this.disableAllToolStripMenuItem_Click);
            // 
            // btnDone
            // 
            resources.ApplyResources(this.btnDone, "btnDone");
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDone.Name = "btnDone";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // lnkWpRE
            // 
            resources.ApplyResources(this.lnkWpRE, "lnkWpRE");
            this.lnkWpRE.Name = "lnkWpRE";
            this.lnkWpRE.TabStop = true;
            this.lnkWpRE.UseCompatibleTextRendering = true;
            this.lnkWpRE.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkIgnoreLinks
            // 
            resources.ApplyResources(this.chkIgnoreLinks, "chkIgnoreLinks");
            this.chkIgnoreLinks.Name = "chkIgnoreLinks";
            this.toolTip1.SetToolTip(this.chkIgnoreLinks, resources.GetString("chkIgnoreLinks.ToolTip"));
            this.chkIgnoreLinks.UseVisualStyleBackColor = true;
            // 
            // chkAddToSummary
            // 
            resources.ApplyResources(this.chkAddToSummary, "chkAddToSummary");
            this.chkAddToSummary.Checked = true;
            this.chkAddToSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddToSummary.Name = "chkAddToSummary";
            this.toolTip1.SetToolTip(this.chkAddToSummary, resources.GetString("chkAddToSummary.ToolTip"));
            this.chkAddToSummary.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreMore
            // 
            resources.ApplyResources(this.chkIgnoreMore, "chkIgnoreMore");
            this.chkIgnoreMore.Name = "chkIgnoreMore";
            this.toolTip1.SetToolTip(this.chkIgnoreMore, resources.GetString("chkIgnoreMore.ToolTip"));
            this.chkIgnoreMore.UseVisualStyleBackColor = true;
            this.chkIgnoreMore.CheckedChanged += new System.EventHandler(this.chkIgnoreMore_CheckedChanged);
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblMsdn
            // 
            resources.ApplyResources(this.lblMsdn, "lblMsdn");
            this.lblMsdn.Name = "lblMsdn";
            this.lblMsdn.TabStop = true;
            this.lblMsdn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMsdn_LinkClicked);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FindandReplace
            // 
            this.AcceptButton = this.btnDone;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblMsdn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.chkIgnoreMore);
            this.Controls.Add(this.chkIgnoreLinks);
            this.Controls.Add(this.chkAddToSummary);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lnkWpRE);
            this.Controls.Add(this.btnDone);
            this.Name = "FindandReplace";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindandReplace_FormClosing);
            this.Shown += new System.EventHandler(this.FindandReplace_Shown);
            this.VisibleChanged += new System.EventHandler(this.FindandReplace_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.FindAndReplaceContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.LinkLabel lnkWpRE;
        private System.Windows.Forms.Button btnClear;
        private WikiFunctions.Controls.AWBToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkIgnoreLinks;
        private System.Windows.Forms.CheckBox chkAddToSummary;
        private System.Windows.Forms.ContextMenuStrip FindAndReplaceContextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem allCaseSensitiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllCaseSensitiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllRegularExpressionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllRegularExpressionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllMultlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllMultilineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllSinglelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllSinglelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllBeforeOrAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllBeforeOrAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableAllToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.CheckBox chkIgnoreMore;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem addRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testRegexToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem moveToTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveToBottomToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lblMsdn;
        private System.Windows.Forms.ToolStripMenuItem createRetfRuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllMinorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllMinorToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn find;
        private System.Windows.Forms.DataGridViewTextBoxColumn replace;
        private System.Windows.Forms.DataGridViewCheckBoxColumn casesensitive;
        private System.Windows.Forms.DataGridViewCheckBoxColumn regex;
        private System.Windows.Forms.DataGridViewCheckBoxColumn multi;
        private System.Windows.Forms.DataGridViewCheckBoxColumn single;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Minor;
        private System.Windows.Forms.DataGridViewCheckBoxColumn BeforeOrAfter;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.Button btnCancel;
    }
}
