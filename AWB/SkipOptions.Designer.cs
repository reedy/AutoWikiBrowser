namespace AutoWikiBrowser
{
    partial class SkipOptions
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
            this.btnClose = new System.Windows.Forms.Button();
            this.rdoNone = new System.Windows.Forms.RadioButton();
            this.rdoNoUnicode = new System.Windows.Forms.RadioButton();
            this.rdoNoTag = new System.Windows.Forms.RadioButton();
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(369, 200);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // rdoNone
            // 
            this.rdoNone.AutoSize = true;
            this.rdoNone.Checked = true;
            this.rdoNone.Location = new System.Drawing.Point(6, 19);
            this.rdoNone.Name = "rdoNone";
            this.rdoNone.Size = new System.Drawing.Size(51, 17);
            this.rdoNone.TabIndex = 3;
            this.rdoNone.TabStop = true;
            this.rdoNone.Tag = "0";
            this.rdoNone.Text = "None";
            this.rdoNone.UseVisualStyleBackColor = true;
            // 
            // rdoNoUnicode
            // 
            this.rdoNoUnicode.AutoSize = true;
            this.rdoNoUnicode.Location = new System.Drawing.Point(6, 43);
            this.rdoNoUnicode.Name = "rdoNoUnicode";
            this.rdoNoUnicode.Size = new System.Drawing.Size(140, 17);
            this.rdoNoUnicode.TabIndex = 4;
            this.rdoNoUnicode.Tag = "1";
            this.rdoNoUnicode.Text = "Skip if no unicodification";
            this.rdoNoUnicode.UseVisualStyleBackColor = true;
            // 
            // rdoNoTag
            // 
            this.rdoNoTag.AutoSize = true;
            this.rdoNoTag.Location = new System.Drawing.Point(6, 67);
            this.rdoNoTag.Name = "rdoNoTag";
            this.rdoNoTag.Size = new System.Drawing.Size(131, 17);
            this.rdoNoTag.TabIndex = 5;
            this.rdoNoTag.Tag = "2";
            this.rdoNoTag.Text = "Skip if no tag changes";
            this.rdoNoTag.UseVisualStyleBackColor = true;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.rdoNone);
            this.gbOptions.Controls.Add(this.rdoNoTag);
            this.gbOptions.Controls.Add(this.rdoNoUnicode);
            this.gbOptions.Location = new System.Drawing.Point(12, 12);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(200, 100);
            this.gbOptions.TabIndex = 6;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // SkipOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 235);
            this.Controls.Add(this.gbOptions);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "";
            this.Text = "SkipOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.RadioButton rdoNone;
        private System.Windows.Forms.RadioButton rdoNoUnicode;
        private System.Windows.Forms.RadioButton rdoNoTag;
        private System.Windows.Forms.GroupBox gbOptions;
    }
}