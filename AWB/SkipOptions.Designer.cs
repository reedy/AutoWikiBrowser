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
            this.components = new System.ComponentModel.Container();
            this.btnClose = new System.Windows.Forms.Button();
            this.CheckAll = new System.Windows.Forms.CheckBox();
            this.CheckNone = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new WikiFunctions.Controls.AWBToolTip(this.components);
            this.skipListBox = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(154, 322);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // CheckAll
            // 
            this.CheckAll.AutoSize = true;
            this.CheckAll.Location = new System.Drawing.Point(56, 8);
            this.CheckAll.Name = "CheckAll";
            this.CheckAll.Size = new System.Drawing.Size(36, 17);
            this.CheckAll.TabIndex = 11;
            this.CheckAll.Text = "all";
            this.toolTip1.SetToolTip(this.CheckAll, "Check all");
            this.CheckAll.UseVisualStyleBackColor = true;
            this.CheckAll.CheckedChanged += new System.EventHandler(this.CheckAll_CheckedChanged);
            // 
            // CheckNone
            // 
            this.CheckNone.AutoSize = true;
            this.CheckNone.Location = new System.Drawing.Point(98, 8);
            this.CheckNone.Name = "CheckNone";
            this.CheckNone.Size = new System.Drawing.Size(50, 17);
            this.CheckNone.TabIndex = 12;
            this.CheckNone.Text = "none";
            this.toolTip1.SetToolTip(this.CheckNone, "Uncheck all");
            this.CheckNone.UseVisualStyleBackColor = true;
            this.CheckNone.CheckedChanged += new System.EventHandler(this.CheckNone_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Check";
            // 
            // skipListBox
            // 
            this.skipListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skipListBox.FormattingEnabled = true;
            this.skipListBox.Location = new System.Drawing.Point(13, 26);
            this.skipListBox.Name = "skipListBox";
            this.skipListBox.ScrollAlwaysVisible = true;
            this.skipListBox.Size = new System.Drawing.Size(216, 274);
            this.skipListBox.TabIndex = 14;
            // 
            // SkipOptions
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(241, 357);
            this.Controls.Add(this.skipListBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CheckNone);
            this.Controls.Add(this.CheckAll);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SkipOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Skip if no...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkipOptions_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox CheckAll;
        private System.Windows.Forms.CheckBox CheckNone;
        private System.Windows.Forms.Label label1;
        private WikiFunctions.Controls.AWBToolTip toolTip1;
        private System.Windows.Forms.CheckedListBox skipListBox;
    }
}