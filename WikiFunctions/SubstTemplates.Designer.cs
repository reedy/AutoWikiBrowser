namespace WikiFunctions
{
    partial class SubstTemplates
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTemplates = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.chkIgnoreUnformatted = new System.Windows.Forms.CheckBox();
            this.chkUseExpandTemplates = new System.Windows.Forms.CheckBox();
            this.chkIncludeComment = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Templates to substitute:";
            // 
            // textBoxTemplates
            // 
            this.textBoxTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTemplates.Location = new System.Drawing.Point(12, 25);
            this.textBoxTemplates.Multiline = true;
            this.textBoxTemplates.Name = "textBoxTemplates";
            this.textBoxTemplates.Size = new System.Drawing.Size(386, 161);
            this.textBoxTemplates.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(323, 238);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(242, 238);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.Location = new System.Drawing.Point(12, 238);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Location = new System.Drawing.Point(93, 238);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // chkIgnoreUnformatted
            // 
            this.chkIgnoreUnformatted.AutoSize = true;
            this.chkIgnoreUnformatted.Location = new System.Drawing.Point(15, 192);
            this.chkIgnoreUnformatted.Name = "chkIgnoreUnformatted";
            this.chkIgnoreUnformatted.Size = new System.Drawing.Size(337, 17);
            this.chkIgnoreUnformatted.TabIndex = 6;
            this.chkIgnoreUnformatted.Text = "Ignore external/interwiki links, images, nowiki, math and <!-- -->";
            this.chkIgnoreUnformatted.UseVisualStyleBackColor = true;
            // 
            // chkUseExpandTemplates
            // 
            this.chkUseExpandTemplates.AutoSize = true;
            this.chkUseExpandTemplates.Checked = true;
            this.chkUseExpandTemplates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseExpandTemplates.Location = new System.Drawing.Point(15, 215);
            this.chkUseExpandTemplates.Name = "chkUseExpandTemplates";
            this.chkUseExpandTemplates.Size = new System.Drawing.Size(117, 17);
            this.chkUseExpandTemplates.TabIndex = 7;
            this.chkUseExpandTemplates.Text = "Expand recursively";
            this.chkUseExpandTemplates.UseVisualStyleBackColor = true;
            this.chkUseExpandTemplates.CheckedChanged += new System.EventHandler(this.chkUseExpandTemplates_CheckedChanged);
            // 
            // chkIncludeComment
            // 
            this.chkIncludeComment.AutoSize = true;
            this.chkIncludeComment.Location = new System.Drawing.Point(192, 215);
            this.chkIncludeComment.Name = "chkIncludeComment";
            this.chkIncludeComment.Size = new System.Drawing.Size(204, 17);
            this.chkIncludeComment.TabIndex = 8;
            this.chkIncludeComment.Text = "Include comment with template name";
            this.chkIncludeComment.UseVisualStyleBackColor = true;
            // 
            // SubstTemplates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 270);
            this.Controls.Add(this.chkIncludeComment);
            this.Controls.Add(this.chkUseExpandTemplates);
            this.Controls.Add(this.chkIgnoreUnformatted);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxTemplates);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(418, 276);
            this.Name = "SubstTemplates";
            this.Text = "Substitute templates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTemplates;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkIgnoreUnformatted;
        private System.Windows.Forms.CheckBox chkUseExpandTemplates;
        private System.Windows.Forms.CheckBox chkIncludeComment;
    }
}