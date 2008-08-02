namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    partial class About
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
            this.okButton = new System.Windows.Forms.Button();
            this.linkReedy = new System.Windows.Forms.LinkLabel();
            this.linkMboverload = new System.Windows.Forms.LinkLabel();
            this.linkTypoScanPage = new System.Windows.Forms.LinkLabel();
            this.linkStats = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(193, 118);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 22);
            this.okButton.TabIndex = 32;
            this.okButton.Text = "&OK";
            // 
            // linkReedy
            // 
            this.linkReedy.AutoSize = true;
            this.linkReedy.Location = new System.Drawing.Point(13, 52);
            this.linkReedy.Name = "linkReedy";
            this.linkReedy.Size = new System.Drawing.Size(63, 13);
            this.linkReedy.TabIndex = 33;
            this.linkReedy.TabStop = true;
            this.linkReedy.Text = "User:Reedy";
            this.linkReedy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReedy_LinkClicked);
            // 
            // linkMboverload
            // 
            this.linkMboverload.AutoSize = true;
            this.linkMboverload.Location = new System.Drawing.Point(13, 78);
            this.linkMboverload.Name = "linkMboverload";
            this.linkMboverload.Size = new System.Drawing.Size(87, 13);
            this.linkMboverload.TabIndex = 34;
            this.linkMboverload.TabStop = true;
            this.linkMboverload.Text = "User:mboverload";
            this.linkMboverload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMboverload_LinkClicked);
            // 
            // linkTypoScanPage
            // 
            this.linkTypoScanPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkTypoScanPage.Location = new System.Drawing.Point(12, 9);
            this.linkTypoScanPage.Name = "linkTypoScanPage";
            this.linkTypoScanPage.Size = new System.Drawing.Size(102, 27);
            this.linkTypoScanPage.TabIndex = 35;
            this.linkTypoScanPage.TabStop = true;
            this.linkTypoScanPage.Text = "TypoScan";
            this.linkTypoScanPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTypoScanPage_LinkClicked);
            // 
            // linkStats
            // 
            this.linkStats.AutoSize = true;
            this.linkStats.Location = new System.Drawing.Point(160, 52);
            this.linkStats.Name = "linkStats";
            this.linkStats.Size = new System.Drawing.Size(49, 13);
            this.linkStats.TabIndex = 36;
            this.linkStats.TabStop = true;
            this.linkStats.Text = "Statistics";
            this.linkStats.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkStats_LinkClicked);
            // 
            // About
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 152);
            this.Controls.Add(this.linkStats);
            this.Controls.Add(this.linkTypoScanPage);
            this.Controls.Add(this.linkMboverload);
            this.Controls.Add(this.linkReedy);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkReedy;
        private System.Windows.Forms.LinkLabel linkMboverload;
        private System.Windows.Forms.LinkLabel linkTypoScanPage;
        private System.Windows.Forms.LinkLabel linkStats;
    }
}