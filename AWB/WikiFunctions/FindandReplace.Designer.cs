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
            this.chkAreRegexes = new System.Windows.Forms.CheckBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDone = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.btnClear = new System.Windows.Forms.Button();
            this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.chkSingleline = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkIgnoreLinks = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // chkAreRegexes
            // 
            this.chkAreRegexes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAreRegexes.AutoSize = true;
            this.chkAreRegexes.Location = new System.Drawing.Point(407, 12);
            this.chkAreRegexes.Name = "chkAreRegexes";
            this.chkAreRegexes.Size = new System.Drawing.Size(42, 17);
            this.chkAreRegexes.TabIndex = 0;
            this.chkAreRegexes.Text = "Are";
            this.chkAreRegexes.UseVisualStyleBackColor = true;
            this.chkAreRegexes.CheckedChanged += new System.EventHandler(this.chkAreRegexes_CheckedChanged);
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
            this.Column1,
            this.Column2});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(12, 10);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(389, 284);
            this.dataGridView1.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Find";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Replace with";
            this.Column2.Name = "Column2";
            // 
            // btnDone
            // 
            this.btnDone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDone.Location = new System.Drawing.Point(480, 271);
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
            this.linkLabel2.Location = new System.Drawing.Point(442, 13);
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
            this.btnClear.Location = new System.Drawing.Point(511, 242);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(60, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // chkCaseSensitive
            // 
            this.chkCaseSensitive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCaseSensitive.AutoSize = true;
            this.chkCaseSensitive.Location = new System.Drawing.Point(407, 33);
            this.chkCaseSensitive.Name = "chkCaseSensitive";
            this.chkCaseSensitive.Size = new System.Drawing.Size(94, 17);
            this.chkCaseSensitive.TabIndex = 11;
            this.chkCaseSensitive.Text = "Case sensitive";
            this.chkCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // chkMultiline
            // 
            this.chkMultiline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMultiline.AutoSize = true;
            this.chkMultiline.Enabled = false;
            this.chkMultiline.Location = new System.Drawing.Point(407, 54);
            this.chkMultiline.Name = "chkMultiline";
            this.chkMultiline.Size = new System.Drawing.Size(64, 17);
            this.chkMultiline.TabIndex = 12;
            this.chkMultiline.Text = "Multiline";
            this.toolTip1.SetToolTip(this.chkMultiline, "Changes meaning of \"^\" and \"$\" so they match beginning and end of every line, rat" +
                    "her than just  of the entire string.");
            this.chkMultiline.UseVisualStyleBackColor = true;
            // 
            // chkSingleline
            // 
            this.chkSingleline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSingleline.AutoSize = true;
            this.chkSingleline.Enabled = false;
            this.chkSingleline.Location = new System.Drawing.Point(407, 75);
            this.chkSingleline.Name = "chkSingleline";
            this.chkSingleline.Size = new System.Drawing.Size(71, 17);
            this.chkSingleline.TabIndex = 13;
            this.chkSingleline.Text = "Singleline";
            this.toolTip1.SetToolTip(this.chkSingleline, "Changes meaning of \".\" so it matches all characters, rather than all but newline." +
                    "");
            this.chkSingleline.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreLinks
            // 
            this.chkIgnoreLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIgnoreLinks.AutoSize = true;
            this.chkIgnoreLinks.Location = new System.Drawing.Point(407, 98);
            this.chkIgnoreLinks.Name = "chkIgnoreLinks";
            this.chkIgnoreLinks.Size = new System.Drawing.Size(177, 17);
            this.chkIgnoreLinks.TabIndex = 14;
            this.chkIgnoreLinks.Text = "Ignore external links and images";
            this.chkIgnoreLinks.UseVisualStyleBackColor = true;
            // 
            // FindandReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 306);
            this.Controls.Add(this.chkIgnoreLinks);
            this.Controls.Add(this.chkSingleline);
            this.Controls.Add(this.chkMultiline);
            this.Controls.Add(this.chkCaseSensitive);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.chkAreRegexes);
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

        private System.Windows.Forms.CheckBox chkAreRegexes;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox chkCaseSensitive;
        private System.Windows.Forms.CheckBox chkMultiline;
        private System.Windows.Forms.CheckBox chkSingleline;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkIgnoreLinks;
    }
}