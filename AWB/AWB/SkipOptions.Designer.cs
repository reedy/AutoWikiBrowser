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
            this.rdoNoBadLink = new System.Windows.Forms.RadioButton();
            this.rdoNoBulletedLink = new System.Windows.Forms.RadioButton();
            this.rdoNoBoldTitle = new System.Windows.Forms.RadioButton();
            this.rdoNoHeaderError = new System.Windows.Forms.RadioButton();
            this.gbOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(292, 135);
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
            this.rdoNoTag.Size = new System.Drawing.Size(132, 17);
            this.rdoNoTag.TabIndex = 5;
            this.rdoNoTag.Tag = "2";
            this.rdoNoTag.Text = "Skip if no tag changed";
            this.rdoNoTag.UseVisualStyleBackColor = true;
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.rdoNoBadLink);
            this.gbOptions.Controls.Add(this.rdoNoBulletedLink);
            this.gbOptions.Controls.Add(this.rdoNoBoldTitle);
            this.gbOptions.Controls.Add(this.rdoNoHeaderError);
            this.gbOptions.Controls.Add(this.rdoNone);
            this.gbOptions.Controls.Add(this.rdoNoTag);
            this.gbOptions.Controls.Add(this.rdoNoUnicode);
            this.gbOptions.Location = new System.Drawing.Point(12, 12);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(355, 117);
            this.gbOptions.TabIndex = 6;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "Options";
            // 
            // rdoNoBadLink
            // 
            this.rdoNoBadLink.AutoSize = true;
            this.rdoNoBadLink.Location = new System.Drawing.Point(171, 91);
            this.rdoNoBadLink.Name = "rdoNoBadLink";
            this.rdoNoBadLink.Size = new System.Drawing.Size(139, 17);
            this.rdoNoBadLink.TabIndex = 10;
            this.rdoNoBadLink.TabStop = true;
            this.rdoNoBadLink.Tag = "6";
            this.rdoNoBadLink.Text = "Skip if no bad links fixed";
            this.rdoNoBadLink.UseVisualStyleBackColor = true;
            // 
            // rdoNoBulletedLink
            // 
            this.rdoNoBulletedLink.AutoSize = true;
            this.rdoNoBulletedLink.Location = new System.Drawing.Point(171, 67);
            this.rdoNoBulletedLink.Name = "rdoNoBulletedLink";
            this.rdoNoBulletedLink.Size = new System.Drawing.Size(168, 17);
            this.rdoNoBulletedLink.TabIndex = 9;
            this.rdoNoBulletedLink.TabStop = true;
            this.rdoNoBulletedLink.Tag = "5";
            this.rdoNoBulletedLink.Text = "Skip if no external link bulleted";
            this.rdoNoBulletedLink.UseVisualStyleBackColor = true;
            // 
            // rdoNoBoldTitle
            // 
            this.rdoNoBoldTitle.AutoSize = true;
            this.rdoNoBoldTitle.Location = new System.Drawing.Point(171, 43);
            this.rdoNoBoldTitle.Name = "rdoNoBoldTitle";
            this.rdoNoBoldTitle.Size = new System.Drawing.Size(135, 17);
            this.rdoNoBoldTitle.TabIndex = 7;
            this.rdoNoBoldTitle.TabStop = true;
            this.rdoNoBoldTitle.Tag = "4";
            this.rdoNoBoldTitle.Text = "Skip if no title boldened";
            this.rdoNoBoldTitle.UseVisualStyleBackColor = true;
            // 
            // rdoNoHeaderError
            // 
            this.rdoNoHeaderError.AutoSize = true;
            this.rdoNoHeaderError.Location = new System.Drawing.Point(171, 20);
            this.rdoNoHeaderError.Name = "rdoNoHeaderError";
            this.rdoNoHeaderError.Size = new System.Drawing.Size(154, 17);
            this.rdoNoHeaderError.TabIndex = 6;
            this.rdoNoHeaderError.TabStop = true;
            this.rdoNoHeaderError.Tag = "3";
            this.rdoNoHeaderError.Text = "Skip if no header error fixed";
            this.rdoNoHeaderError.UseVisualStyleBackColor = true;
            // 
            // SkipOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 167);
            this.Controls.Add(this.gbOptions);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "";
            this.Text = "Skip options";
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
        private System.Windows.Forms.RadioButton rdoNoHeaderError;
        private System.Windows.Forms.RadioButton rdoNoBoldTitle;
        private System.Windows.Forms.RadioButton rdoNoBulletedLink;
        private System.Windows.Forms.RadioButton rdoNoBadLink;
    }
}