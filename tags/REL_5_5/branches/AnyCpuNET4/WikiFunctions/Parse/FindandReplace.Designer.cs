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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
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
            this.enableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.uncheckAllCaseSensitiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMinorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMultilineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllRegularExpressionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllSinglelineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDone = new System.Windows.Forms.Button();
            this.lnkWpRE = new System.Windows.Forms.LinkLabel();
            this.btnClear = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkIgnoreLinks = new System.Windows.Forms.CheckBox();
            this.chkAddToSummary = new System.Windows.Forms.CheckBox();
            this.chkIgnoreMore = new System.Windows.Forms.CheckBox();
            this.chkAfterOtherFixes = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblMsdn = new System.Windows.Forms.LinkLabel();
            this.find = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.casesensitive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.regex = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.multi = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.single = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Minor = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.FindAndReplaceContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
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
            this.enabled,
            this.Comment});
            this.dataGridView1.ContextMenuStrip = this.FindAndReplaceContextMenu;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 80);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(717, 290);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // FindAndReplaceContextMenu
            // 
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
            this.enableAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.uncheckAllCaseSensitiveToolStripMenuItem,
            this.uncheckAllMinorToolStripMenuItem,
            this.uncheckAllMultilineToolStripMenuItem,
            this.uncheckAllRegularExpressionsToolStripMenuItem,
            this.uncheckAllSinglelineToolStripMenuItem,
            this.disableAllToolStripMenuItem});
            this.FindAndReplaceContextMenu.Name = "FindAndReplaceContextMenu";
            this.FindAndReplaceContextMenu.Size = new System.Drawing.Size(239, 468);
            this.FindAndReplaceContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.FindAndReplaceContextMenu_Opening);
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            this.addRowToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.addRowToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.addRowToolStripMenuItem.Text = "Insert row";
            this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete row";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(235, 6);
            // 
            // testRegexToolStripMenuItem
            // 
            this.testRegexToolStripMenuItem.Name = "testRegexToolStripMenuItem";
            this.testRegexToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.testRegexToolStripMenuItem.Text = "Test regex...";
            this.testRegexToolStripMenuItem.Click += new System.EventHandler(this.testRegexToolStripMenuItem_Click);
            // 
            // createRetfRuleToolStripMenuItem
            // 
            this.createRetfRuleToolStripMenuItem.Name = "createRetfRuleToolStripMenuItem";
            this.createRetfRuleToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.createRetfRuleToolStripMenuItem.Text = "Copy as a RETF rule";
            this.createRetfRuleToolStripMenuItem.Click += new System.EventHandler(this.createRetfRuleToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(235, 6);
            // 
            // moveToTopToolStripMenuItem
            // 
            this.moveToTopToolStripMenuItem.Name = "moveToTopToolStripMenuItem";
            this.moveToTopToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.moveToTopToolStripMenuItem.Text = "Move to top";
            this.moveToTopToolStripMenuItem.Click += new System.EventHandler(this.moveToTopToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // moveToBottomToolStripMenuItem
            // 
            this.moveToBottomToolStripMenuItem.Name = "moveToBottomToolStripMenuItem";
            this.moveToBottomToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.moveToBottomToolStripMenuItem.Text = "Move to bottom";
            this.moveToBottomToolStripMenuItem.Click += new System.EventHandler(this.moveToBottomToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(235, 6);
            // 
            // allCaseSensitiveToolStripMenuItem
            // 
            this.allCaseSensitiveToolStripMenuItem.Name = "allCaseSensitiveToolStripMenuItem";
            this.allCaseSensitiveToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.allCaseSensitiveToolStripMenuItem.Text = "Check all case sensitive";
            this.allCaseSensitiveToolStripMenuItem.Click += new System.EventHandler(this.allCaseSensitiveToolStripMenuItem_Click);
            // 
            // checkAllMinorToolStripMenuItem
            // 
            this.checkAllMinorToolStripMenuItem.Name = "checkAllMinorToolStripMenuItem";
            this.checkAllMinorToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.checkAllMinorToolStripMenuItem.Text = "Check all minor";
            this.checkAllMinorToolStripMenuItem.Click += new System.EventHandler(this.checkAllMinorToolStripMenuItem_Click);
            // 
            // checkAllMultlineToolStripMenuItem
            // 
            this.checkAllMultlineToolStripMenuItem.Name = "checkAllMultlineToolStripMenuItem";
            this.checkAllMultlineToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.checkAllMultlineToolStripMenuItem.Text = "Check all multline";
            this.checkAllMultlineToolStripMenuItem.Click += new System.EventHandler(this.checkAllMultlineToolStripMenuItem_Click);
            // 
            // checkAllRegularExpressionsToolStripMenuItem
            // 
            this.checkAllRegularExpressionsToolStripMenuItem.Name = "checkAllRegularExpressionsToolStripMenuItem";
            this.checkAllRegularExpressionsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.checkAllRegularExpressionsToolStripMenuItem.Text = "Check all regular expressions";
            this.checkAllRegularExpressionsToolStripMenuItem.Click += new System.EventHandler(this.checkAllRegularExpressionsToolStripMenuItem_Click);
            // 
            // checkAllSinglelineToolStripMenuItem
            // 
            this.checkAllSinglelineToolStripMenuItem.Name = "checkAllSinglelineToolStripMenuItem";
            this.checkAllSinglelineToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.checkAllSinglelineToolStripMenuItem.Text = "Check all singleline";
            this.checkAllSinglelineToolStripMenuItem.Click += new System.EventHandler(this.checkAllSinglelineToolStripMenuItem_Click);
            // 
            // enableAllToolStripMenuItem
            // 
            this.enableAllToolStripMenuItem.Name = "enableAllToolStripMenuItem";
            this.enableAllToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.enableAllToolStripMenuItem.Text = "Enable all";
            this.enableAllToolStripMenuItem.Click += new System.EventHandler(this.enableAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(235, 6);
            // 
            // uncheckAllCaseSensitiveToolStripMenuItem
            // 
            this.uncheckAllCaseSensitiveToolStripMenuItem.Name = "uncheckAllCaseSensitiveToolStripMenuItem";
            this.uncheckAllCaseSensitiveToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.uncheckAllCaseSensitiveToolStripMenuItem.Text = "Uncheck all case sensitive";
            this.uncheckAllCaseSensitiveToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllCaseSensitiveToolStripMenuItem_Click);
            // 
            // uncheckAllMinorToolStripMenuItem
            // 
            this.uncheckAllMinorToolStripMenuItem.Name = "uncheckAllMinorToolStripMenuItem";
            this.uncheckAllMinorToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.uncheckAllMinorToolStripMenuItem.Text = "Uncheck all minor";
            this.uncheckAllMinorToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllMinorToolStripMenuItem_Click);
            // 
            // uncheckAllMultilineToolStripMenuItem
            // 
            this.uncheckAllMultilineToolStripMenuItem.Name = "uncheckAllMultilineToolStripMenuItem";
            this.uncheckAllMultilineToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.uncheckAllMultilineToolStripMenuItem.Text = "Uncheck all multiline";
            this.uncheckAllMultilineToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllMultilineToolStripMenuItem_Click);
            // 
            // uncheckAllRegularExpressionsToolStripMenuItem
            // 
            this.uncheckAllRegularExpressionsToolStripMenuItem.Name = "uncheckAllRegularExpressionsToolStripMenuItem";
            this.uncheckAllRegularExpressionsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.uncheckAllRegularExpressionsToolStripMenuItem.Text = "Uncheck all regular expressions";
            this.uncheckAllRegularExpressionsToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllRegularExpressionsToolStripMenuItem_Click);
            // 
            // uncheckAllSinglelineToolStripMenuItem
            // 
            this.uncheckAllSinglelineToolStripMenuItem.Name = "uncheckAllSinglelineToolStripMenuItem";
            this.uncheckAllSinglelineToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.uncheckAllSinglelineToolStripMenuItem.Text = "Uncheck all singleline";
            this.uncheckAllSinglelineToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllSinglelineToolStripMenuItem_Click);
            // 
            // disableAllToolStripMenuItem
            // 
            this.disableAllToolStripMenuItem.Name = "disableAllToolStripMenuItem";
            this.disableAllToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.disableAllToolStripMenuItem.Text = "Disable all";
            this.disableAllToolStripMenuItem.Click += new System.EventHandler(this.disableAllToolStripMenuItem_Click);
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDone.Location = new System.Drawing.Point(654, 37);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "&Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // lnkWpRE
            // 
            this.lnkWpRE.AutoSize = true;
            this.lnkWpRE.LinkArea = new System.Windows.Forms.LinkArea(4, 32);
            this.lnkWpRE.Location = new System.Drawing.Point(12, 59);
            this.lnkWpRE.Name = "lnkWpRE";
            this.lnkWpRE.Size = new System.Drawing.Size(194, 17);
            this.lnkWpRE.TabIndex = 9;
            this.lnkWpRE.TabStop = true;
            this.lnkWpRE.Text = "See regular expressions on Wikipedia";
            this.lnkWpRE.UseCompatibleTextRendering = true;
            this.lnkWpRE.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(654, 8);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "&Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkIgnoreLinks
            // 
            this.chkIgnoreLinks.AutoSize = true;
            this.chkIgnoreLinks.Location = new System.Drawing.Point(12, 12);
            this.chkIgnoreLinks.Name = "chkIgnoreLinks";
            this.chkIgnoreLinks.Size = new System.Drawing.Size(321, 17);
            this.chkIgnoreLinks.TabIndex = 14;
            this.chkIgnoreLinks.Text = "Ignore &external/interwiki links, images, nowiki, math and <!-- -->";
            this.toolTip1.SetToolTip(this.chkIgnoreLinks, "Find and replacements will not be made in external/interwiki links, images, <nowi" +
                    "ki>. <math> and <!-- comments -->");
            this.chkIgnoreLinks.UseVisualStyleBackColor = true;
            this.chkIgnoreLinks.CheckedChanged += new System.EventHandler(this.chkIgnoreLinks_CheckedChanged);
            // 
            // chkAddToSummary
            // 
            this.chkAddToSummary.AutoSize = true;
            this.chkAddToSummary.Checked = true;
            this.chkAddToSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddToSummary.Location = new System.Drawing.Point(363, 10);
            this.chkAddToSummary.MinimumSize = new System.Drawing.Size(190, 20);
            this.chkAddToSummary.Name = "chkAddToSummary";
            this.chkAddToSummary.Size = new System.Drawing.Size(190, 20);
            this.chkAddToSummary.TabIndex = 16;
            this.chkAddToSummary.Text = "Add &replacements to edit summary";
            this.toolTip1.SetToolTip(this.chkAddToSummary, "Appends information about replacements made to the edit summary");
            this.chkAddToSummary.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreMore
            // 
            this.chkIgnoreMore.AutoSize = true;
            this.chkIgnoreMore.Location = new System.Drawing.Point(12, 35);
            this.chkIgnoreMore.Name = "chkIgnoreMore";
            this.chkIgnoreMore.Size = new System.Drawing.Size(254, 17);
            this.chkIgnoreMore.TabIndex = 18;
            this.chkIgnoreMore.Text = "Ignore &templates, refs, link targets, and headings";
            this.toolTip1.SetToolTip(this.chkIgnoreMore, "Find and replacements will not be made in templates, <source>, <cite>, quotes and heading" +
                    "s");
            this.chkIgnoreMore.UseVisualStyleBackColor = true;
            this.chkIgnoreMore.CheckedChanged += new System.EventHandler(this.chkIgnoreMore_CheckedChanged);
            // 
            // chkAfterOtherFixes
            // 
            this.chkAfterOtherFixes.AutoSize = true;
            this.chkAfterOtherFixes.Location = new System.Drawing.Point(363, 35);
            this.chkAfterOtherFixes.Name = "chkAfterOtherFixes";
            this.chkAfterOtherFixes.Size = new System.Drawing.Size(222, 17);
            this.chkAfterOtherFixes.TabIndex = 17;
            this.chkAfterOtherFixes.Text = "&Apply after general fixes, otherwise before";
            this.chkAfterOtherFixes.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(363, 56);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(179, 20);
            this.txtSearch.TabIndex = 19;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(548, 54);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(34, 23);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "&Go";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(313, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "&Search:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(203, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "/ in";
            // 
            // lblMsdn
            // 
            this.lblMsdn.AutoSize = true;
            this.lblMsdn.Location = new System.Drawing.Point(223, 59);
            this.lblMsdn.Name = "lblMsdn";
            this.lblMsdn.Size = new System.Drawing.Size(39, 13);
            this.lblMsdn.TabIndex = 23;
            this.lblMsdn.TabStop = true;
            this.lblMsdn.Text = "MSDN";
            this.lblMsdn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblMsdn_LinkClicked);
            // 
            // find
            // 
            this.find.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.find.FillWeight = 92.50688F;
            this.find.Frozen = true;
            this.find.HeaderText = "Find";
            this.find.MinimumWidth = 45;
            this.find.Name = "find";
            this.find.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.find.Width = 98;
            // 
            // replace
            // 
            this.replace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle1.NullValue = null;
            this.replace.DefaultCellStyle = dataGridViewCellStyle1;
            this.replace.FillWeight = 87.66718F;
            this.replace.Frozen = true;
            this.replace.HeaderText = "Replace with";
            this.replace.MinimumWidth = 45;
            this.replace.Name = "replace";
            this.replace.Width = 92;
            // 
            // casesensitive
            // 
            this.casesensitive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.casesensitive.FalseValue = false;
            this.casesensitive.FillWeight = 71.79286F;
            this.casesensitive.Frozen = true;
            this.casesensitive.HeaderText = "CaseSensitive";
            this.casesensitive.Name = "casesensitive";
            this.casesensitive.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.casesensitive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.casesensitive.TrueValue = true;
            this.casesensitive.Width = 75;
            // 
            // regex
            // 
            this.regex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.regex.FalseValue = false;
            this.regex.FillWeight = 131.5755F;
            this.regex.Frozen = true;
            this.regex.HeaderText = "Regex";
            this.regex.Name = "regex";
            this.regex.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.regex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.regex.TrueValue = true;
            this.regex.Width = 40;
            // 
            // multi
            // 
            this.multi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.multi.FalseValue = false;
            this.multi.FillWeight = 150.1532F;
            this.multi.Frozen = true;
            this.multi.HeaderText = "MultiLine";
            this.multi.Name = "multi";
            this.multi.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.multi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.multi.TrueValue = true;
            this.multi.Width = 50;
            // 
            // single
            // 
            this.single.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.single.FalseValue = false;
            this.single.FillWeight = 173.4641F;
            this.single.Frozen = true;
            this.single.HeaderText = "SingleLine";
            this.single.Name = "single";
            this.single.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.single.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.single.TrueValue = true;
            this.single.Width = 58;
            // 
            // Minor
            // 
            this.Minor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Minor.FalseValue = false;
            this.Minor.Frozen = true;
            this.Minor.HeaderText = "Minor";
            this.Minor.Name = "Minor";
            this.Minor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Minor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Minor.TrueValue = true;
            this.Minor.Width = 50;
            // 
            // enabled
            // 
            this.enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.enabled.FalseValue = false;
            this.enabled.Frozen = true;
            this.enabled.HeaderText = "Enabled";
            this.enabled.Name = "enabled";
            this.enabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.enabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.enabled.TrueValue = true;
            this.enabled.Width = 50;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            // 
            // FindandReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDone;
            this.ClientSize = new System.Drawing.Size(741, 382);
            this.Controls.Add(this.lblMsdn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.chkIgnoreMore);
            this.Controls.Add(this.chkIgnoreLinks);
            this.Controls.Add(this.chkAfterOtherFixes);
            this.Controls.Add(this.chkAddToSummary);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lnkWpRE);
            this.Controls.Add(this.btnDone);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(685, 130);
            this.Name = "FindandReplace";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find & Replace";
            this.Shown += new System.EventHandler(this.FindandReplace_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindandReplace_FormClosing);
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
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkIgnoreLinks;
        public System.Windows.Forms.CheckBox chkAddToSummary;
        private System.Windows.Forms.ContextMenuStrip FindAndReplaceContextMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem allCaseSensitiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllCaseSensitiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllRegularExpressionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllRegularExpressionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllMultlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllMultilineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllSinglelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllSinglelineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.CheckBox chkAfterOtherFixes;
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
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
    }
}
