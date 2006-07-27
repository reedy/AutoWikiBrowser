//$Header: /cvsroot/autowikibrowser/WikiFunctions/FindandReplace.Designer.cs,v 1.4 2006/07/04 19:27:35 wikibluemoose Exp $
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnDone = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.btnClear = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkIgnoreLinks = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkAddToSummary = new System.Windows.Forms.CheckBox();
            this.find = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.casesensitive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.regex = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.multi = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.single = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
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
            this.single});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 10);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(561, 280);
            this.dataGridView1.TabIndex = 1;
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(660, 267);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(91, 23);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(614, 11);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(97, 13);
            this.linkLabel2.TabIndex = 9;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "regular expressions";
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
            this.chkIgnoreLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIgnoreLinks.AutoSize = true;
            this.chkIgnoreLinks.Location = new System.Drawing.Point(579, 96);
            this.chkIgnoreLinks.Name = "chkIgnoreLinks";
            this.chkIgnoreLinks.Size = new System.Drawing.Size(166, 17);
            this.chkIgnoreLinks.TabIndex = 14;
            this.chkIgnoreLinks.Text = "Ignore external/interwiki links,";
            this.toolTip1.SetToolTip(this.chkIgnoreLinks, "Find and replacements will not be made in external/interwiki links, images, <nowi" +
                    "ki>. <math> and <!-- comments -->");
            this.chkIgnoreLinks.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(586, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "images, nowiki, math and <!-- -->";
            this.toolTip1.SetToolTip(this.label1, "Find and replacements will not be made in external/interwiki links, images, <nowi" +
                    "ki>. <math> and <!-- comments -->");
            // 
            // chkAddToSummary
            // 
            this.chkAddToSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAddToSummary.AutoSize = true;
            this.chkAddToSummary.Checked = true;
            this.chkAddToSummary.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAddToSummary.Location = new System.Drawing.Point(579, 143);
            this.chkAddToSummary.Name = "chkAddToSummary";
            this.chkAddToSummary.Size = new System.Drawing.Size(187, 17);
            this.chkAddToSummary.TabIndex = 16;
            this.chkAddToSummary.Text = "Add replacements to edit summary";
            this.toolTip1.SetToolTip(this.chkAddToSummary, "Appends information about replacements made to the edit summary");
            this.chkAddToSummary.UseVisualStyleBackColor = true;
            // 
            // find
            // 
            dataGridViewCellStyle2.NullValue = null;
            this.find.DefaultCellStyle = dataGridViewCellStyle2;
            this.find.FillWeight = 157.7052F;
            this.find.HeaderText = "Find";
            this.find.MinimumWidth = 45;
            this.find.Name = "find";
            this.find.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // replace
            // 
            this.replace.FillWeight = 149.4545F;
            this.replace.HeaderText = "Replace with";
            this.replace.MinimumWidth = 45;
            this.replace.Name = "replace";
            // 
            // casesensitive
            // 
            this.casesensitive.FillWeight = 93.08352F;
            this.casesensitive.HeaderText = "CaseSensitive";
            this.casesensitive.MinimumWidth = 30;
            this.casesensitive.Name = "casesensitive";
            this.casesensitive.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.casesensitive.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // regex
            // 
            this.regex.FillWeight = 76.14212F;
            this.regex.HeaderText = "Regex";
            this.regex.MinimumWidth = 15;
            this.regex.Name = "regex";
            this.regex.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.regex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // multi
            // 
            this.multi.FillWeight = 61.80732F;
            this.multi.HeaderText = "MultiLine";
            this.multi.Name = "multi";
            this.multi.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.multi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // single
            // 
            this.single.FillWeight = 61.80732F;
            this.single.HeaderText = "SingleLine";
            this.single.MinimumWidth = 30;
            this.single.Name = "single";
            this.single.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.single.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // FindandReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 302);
            this.Controls.Add(this.chkAddToSummary);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkAddToSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn find;
        private System.Windows.Forms.DataGridViewTextBoxColumn replace;
        private System.Windows.Forms.DataGridViewCheckBoxColumn casesensitive;
        private System.Windows.Forms.DataGridViewCheckBoxColumn regex;
        private System.Windows.Forms.DataGridViewCheckBoxColumn multi;
        private System.Windows.Forms.DataGridViewCheckBoxColumn single;
    }
}
