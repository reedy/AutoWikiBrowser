/*
    Autowikibrowser
    Copyright (C) 2006 Martin Richards

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

namespace WikiFunctions
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
            this.find = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.casesensitive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuCase = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkAllCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regex = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuRegex = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkAllRegexToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllRegexToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.multi = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuMulti = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkAllMultiToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllMultiToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.single = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contextMenuSingle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkAllSingleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllSingleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.enabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnDone = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.btnClear = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkIgnoreLinks = new System.Windows.Forms.CheckBox();
            this.chkAddToSummary = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuCase.SuspendLayout();
            this.contextMenuRegex.SuspendLayout();
            this.contextMenuMulti.SuspendLayout();
            this.contextMenuSingle.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
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
            this.enabled});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridView1.Location = new System.Drawing.Point(12, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(673, 260);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // find
            // 
            this.find.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.find.FillWeight = 92.50688F;
            this.find.HeaderText = "Find";
            this.find.MinimumWidth = 45;
            this.find.Name = "find";
            this.find.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // replace
            // 
            this.replace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.NullValue = null;
            this.replace.DefaultCellStyle = dataGridViewCellStyle1;
            this.replace.FillWeight = 87.66718F;
            this.replace.HeaderText = "Replace with";
            this.replace.MinimumWidth = 45;
            this.replace.Name = "replace";
            // 
            // casesensitive
            // 
            this.casesensitive.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.casesensitive.ContextMenuStrip = this.contextMenuCase;
            this.casesensitive.FalseValue = "0";
            this.casesensitive.FillWeight = 71.79286F;
            this.casesensitive.HeaderText = "CaseSensitive";
            this.casesensitive.Name = "casesensitive";
            this.casesensitive.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.casesensitive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.casesensitive.TrueValue = "1";
            this.casesensitive.Width = 75;
            // 
            // contextMenuCase
            // 
            this.contextMenuCase.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkAllCaseToolStripMenuItem,
            this.uncheckAllCaseToolStripMenuItem});
            this.contextMenuCase.Name = "contextMenuCase";
            this.contextMenuCase.Size = new System.Drawing.Size(128, 48);
            // 
            // checkAllCaseToolStripMenuItem
            // 
            this.checkAllCaseToolStripMenuItem.Name = "checkAllCaseToolStripMenuItem";
            this.checkAllCaseToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.checkAllCaseToolStripMenuItem.Text = "Check all";
            this.checkAllCaseToolStripMenuItem.Click += new System.EventHandler(this.checkAllCaseToolStripMenuItem_Click);
            // 
            // uncheckAllCaseToolStripMenuItem
            // 
            this.uncheckAllCaseToolStripMenuItem.Name = "uncheckAllCaseToolStripMenuItem";
            this.uncheckAllCaseToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.uncheckAllCaseToolStripMenuItem.Text = "Uncheck all";
            this.uncheckAllCaseToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllCaseToolStripMenuItem_Click);
            // 
            // regex
            // 
            this.regex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.regex.ContextMenuStrip = this.contextMenuRegex;
            this.regex.FalseValue = "0";
            this.regex.FillWeight = 131.5755F;
            this.regex.HeaderText = "Regex";
            this.regex.Name = "regex";
            this.regex.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.regex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.regex.TrueValue = "1";
            this.regex.Width = 40;
            // 
            // contextMenuRegex
            // 
            this.contextMenuRegex.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkAllRegexToolStripMenuItem1,
            this.uncheckAllRegexToolStripMenuItem1});
            this.contextMenuRegex.Name = "contextMenuRegex";
            this.contextMenuRegex.Size = new System.Drawing.Size(128, 48);
            // 
            // checkAllRegexToolStripMenuItem1
            // 
            this.checkAllRegexToolStripMenuItem1.Name = "checkAllRegexToolStripMenuItem1";
            this.checkAllRegexToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.checkAllRegexToolStripMenuItem1.Text = "Check all";
            this.checkAllRegexToolStripMenuItem1.Click += new System.EventHandler(this.checkAllRegexToolStripMenuItem1_Click);
            // 
            // uncheckAllRegexToolStripMenuItem1
            // 
            this.uncheckAllRegexToolStripMenuItem1.Name = "uncheckAllRegexToolStripMenuItem1";
            this.uncheckAllRegexToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.uncheckAllRegexToolStripMenuItem1.Text = "Uncheck all";
            this.uncheckAllRegexToolStripMenuItem1.Click += new System.EventHandler(this.uncheckAllRegexToolStripMenuItem1_Click);
            // 
            // multi
            // 
            this.multi.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.multi.ContextMenuStrip = this.contextMenuMulti;
            this.multi.FalseValue = "0";
            this.multi.FillWeight = 150.1532F;
            this.multi.HeaderText = "MultiLine";
            this.multi.Name = "multi";
            this.multi.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.multi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.multi.TrueValue = "1";
            this.multi.Width = 50;
            // 
            // contextMenuMulti
            // 
            this.contextMenuMulti.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkAllMultiToolStripMenuItem1,
            this.uncheckAllMultiToolStripMenuItem1});
            this.contextMenuMulti.Name = "contextMenuMulti";
            this.contextMenuMulti.Size = new System.Drawing.Size(128, 48);
            // 
            // checkAllMultiToolStripMenuItem1
            // 
            this.checkAllMultiToolStripMenuItem1.Name = "checkAllMultiToolStripMenuItem1";
            this.checkAllMultiToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.checkAllMultiToolStripMenuItem1.Text = "Check all";
            this.checkAllMultiToolStripMenuItem1.Click += new System.EventHandler(this.checkAllMultiToolStripMenuItem1_Click);
            // 
            // uncheckAllMultiToolStripMenuItem1
            // 
            this.uncheckAllMultiToolStripMenuItem1.Name = "uncheckAllMultiToolStripMenuItem1";
            this.uncheckAllMultiToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.uncheckAllMultiToolStripMenuItem1.Text = "Uncheck all";
            this.uncheckAllMultiToolStripMenuItem1.Click += new System.EventHandler(this.uncheckAllMultiToolStripMenuItem1_Click);
            // 
            // single
            // 
            this.single.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.single.ContextMenuStrip = this.contextMenuSingle;
            this.single.FalseValue = "0";
            this.single.FillWeight = 173.4641F;
            this.single.HeaderText = "SingleLine";
            this.single.Name = "single";
            this.single.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.single.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.single.TrueValue = "1";
            this.single.Width = 58;
            // 
            // contextMenuSingle
            // 
            this.contextMenuSingle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkAllSingleToolStripMenuItem1,
            this.uncheckAllSingleToolStripMenuItem1});
            this.contextMenuSingle.Name = "contextMenuSingle";
            this.contextMenuSingle.Size = new System.Drawing.Size(128, 48);
            // 
            // checkAllSingleToolStripMenuItem1
            // 
            this.checkAllSingleToolStripMenuItem1.Name = "checkAllSingleToolStripMenuItem1";
            this.checkAllSingleToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.checkAllSingleToolStripMenuItem1.Text = "Check all";
            this.checkAllSingleToolStripMenuItem1.Click += new System.EventHandler(this.checkAllSingleToolStripMenuItem1_Click);
            // 
            // uncheckAllSingleToolStripMenuItem1
            // 
            this.uncheckAllSingleToolStripMenuItem1.Name = "uncheckAllSingleToolStripMenuItem1";
            this.uncheckAllSingleToolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.uncheckAllSingleToolStripMenuItem1.Text = "Uncheck all";
            this.uncheckAllSingleToolStripMenuItem1.Click += new System.EventHandler(this.uncheckAllSingleToolStripMenuItem1_Click);
            // 
            // enabled
            // 
            this.enabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.enabled.FalseValue = "0";
            this.enabled.HeaderText = "Enabled";
            this.enabled.Name = "enabled";
            this.enabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.enabled.TrueValue = "1";
            this.enabled.Width = 50;
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(691, 267);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(60, 23);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(555, 6);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(119, 13);
            this.linkLabel2.TabIndex = 9;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "See regular expressions";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(691, 238);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(60, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkIgnoreLinks
            // 
            this.chkIgnoreLinks.AutoSize = true;
            this.chkIgnoreLinks.Location = new System.Drawing.Point(12, 5);
            this.chkIgnoreLinks.Name = "chkIgnoreLinks";
            this.chkIgnoreLinks.Size = new System.Drawing.Size(321, 17);
            this.chkIgnoreLinks.TabIndex = 14;
            this.chkIgnoreLinks.Text = "Ignore external/interwiki links, images, nowiki, math and <!-- -->";
            this.toolTip1.SetToolTip(this.chkIgnoreLinks, "Find and replacements will not be made in external/interwiki links, images, <nowi" +
                    "ki>. <math> and <!-- comments -->");
            this.chkIgnoreLinks.UseVisualStyleBackColor = true;
            // 
            // chkAddToSummary
            // 
            this.chkAddToSummary.AutoSize = true;
            this.chkAddToSummary.Checked = true;
            this.chkAddToSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddToSummary.Location = new System.Drawing.Point(339, 5);
            this.chkAddToSummary.Name = "chkAddToSummary";
            this.chkAddToSummary.Size = new System.Drawing.Size(187, 17);
            this.chkAddToSummary.TabIndex = 16;
            this.chkAddToSummary.Text = "Add replacements to edit summary";
            this.toolTip1.SetToolTip(this.chkAddToSummary, "Appends information about replacements made to the edit summary");
            this.chkAddToSummary.UseVisualStyleBackColor = true;
            // 
            // FindandReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 302);
            this.Controls.Add(this.chkAddToSummary);
            this.Controls.Add(this.chkIgnoreLinks);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(263, 129);
            this.Name = "FindandReplace";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Find & Replace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindandReplace_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuCase.ResumeLayout(false);
            this.contextMenuRegex.ResumeLayout(false);
            this.contextMenuMulti.ResumeLayout(false);
            this.contextMenuSingle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkIgnoreLinks;
        private System.Windows.Forms.CheckBox chkAddToSummary;
        private System.Windows.Forms.ContextMenuStrip contextMenuCase;
        private System.Windows.Forms.ToolStripMenuItem checkAllCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllCaseToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuRegex;
        private System.Windows.Forms.ContextMenuStrip contextMenuMulti;
        private System.Windows.Forms.ContextMenuStrip contextMenuSingle;
        private System.Windows.Forms.ToolStripMenuItem checkAllMultiToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllMultiToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem checkAllRegexToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllRegexToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem checkAllSingleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllSingleToolStripMenuItem1;
        private System.Windows.Forms.DataGridViewTextBoxColumn find;
        private System.Windows.Forms.DataGridViewTextBoxColumn replace;
        private System.Windows.Forms.DataGridViewCheckBoxColumn casesensitive;
        private System.Windows.Forms.DataGridViewCheckBoxColumn regex;
        private System.Windows.Forms.DataGridViewCheckBoxColumn multi;
        private System.Windows.Forms.DataGridViewCheckBoxColumn single;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabled;
    }
}
