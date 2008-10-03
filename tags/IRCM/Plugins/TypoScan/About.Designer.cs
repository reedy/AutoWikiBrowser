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
            this.components = new System.ComponentModel.Container();
            this.okButton = new System.Windows.Forms.Button();
            this.linkMboverload = new System.Windows.Forms.LinkLabel();
            this.linkTypoScanPage = new System.Windows.Forms.LinkLabel();
            this.linkStats = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSaved = new System.Windows.Forms.Label();
            this.lblSkipped = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblLoaded = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblUploaded = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.lblToUpload = new System.Windows.Forms.Label();
            this.lblTimeLeft = new System.Windows.Forms.Label();
            this.MaxSemLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.ReedyLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(189, 240);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 22);
            this.okButton.TabIndex = 32;
            this.okButton.Text = "&OK";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkMboverload
            // 
            this.linkMboverload.AutoSize = true;
            this.linkMboverload.Location = new System.Drawing.Point(132, 0);
            this.linkMboverload.Name = "linkMboverload";
            this.linkMboverload.Size = new System.Drawing.Size(62, 13);
            this.linkMboverload.TabIndex = 34;
            this.linkMboverload.TabStop = true;
            this.linkMboverload.Text = "mboverload";
            this.linkMboverload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMboverload_LinkClicked);
            // 
            // linkTypoScanPage
            // 
            this.linkTypoScanPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkTypoScanPage.Location = new System.Drawing.Point(12, 9);
            this.linkTypoScanPage.Name = "linkTypoScanPage";
            this.linkTypoScanPage.Size = new System.Drawing.Size(154, 27);
            this.linkTypoScanPage.TabIndex = 35;
            this.linkTypoScanPage.TabStop = true;
            this.linkTypoScanPage.Text = "TypoScan Plugin";
            this.linkTypoScanPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTypoScanPage_LinkClicked);
            // 
            // linkStats
            // 
            this.linkStats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkStats.AutoSize = true;
            this.linkStats.Location = new System.Drawing.Point(13, 245);
            this.linkStats.Name = "linkStats";
            this.linkStats.Size = new System.Drawing.Size(101, 13);
            this.linkStats.TabIndex = 36;
            this.linkStats.TabStop = true;
            this.linkStats.Text = "TypoScan Statistics";
            this.linkStats.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkStats_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 37;
            this.label1.Text = "Developers: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Founder: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Saved this Session: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Skipped this Session: ";
            // 
            // lblSaved
            // 
            this.lblSaved.AutoSize = true;
            this.lblSaved.Location = new System.Drawing.Point(132, 75);
            this.lblSaved.Name = "lblSaved";
            this.lblSaved.Size = new System.Drawing.Size(0, 13);
            this.lblSaved.TabIndex = 41;
            // 
            // lblSkipped
            // 
            this.lblSkipped.AutoSize = true;
            this.lblSkipped.Location = new System.Drawing.Point(132, 100);
            this.lblSkipped.Name = "lblSkipped";
            this.lblSkipped.Size = new System.Drawing.Size(0, 13);
            this.lblSkipped.TabIndex = 42;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Loaded Articles:";
            // 
            // lblLoaded
            // 
            this.lblLoaded.AutoSize = true;
            this.lblLoaded.Location = new System.Drawing.Point(132, 50);
            this.lblLoaded.Name = "lblLoaded";
            this.lblLoaded.Size = new System.Drawing.Size(0, 13);
            this.lblLoaded.TabIndex = 44;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "Uploaded this Session: ";
            // 
            // lblUploaded
            // 
            this.lblUploaded.AutoSize = true;
            this.lblUploaded.Location = new System.Drawing.Point(132, 125);
            this.lblUploaded.Name = "lblUploaded";
            this.lblUploaded.Size = new System.Drawing.Size(0, 13);
            this.lblUploaded.TabIndex = 46;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.5873F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.4127F));
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.linkMboverload, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblLoaded, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblUploaded, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblSaved, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblSkipped, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblToUpload, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblTimeLeft, 1, 7);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(16, 41);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66666F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 193);
            this.tableLayoutPanel1.TabIndex = 47;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.MaxSemLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.ReedyLabel, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(132, 28);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(113, 17);
            this.tableLayoutPanel2.TabIndex = 48;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "No. to be Uploaded: ";
            // 
            // lblToUpload
            // 
            this.lblToUpload.AutoSize = true;
            this.lblToUpload.Location = new System.Drawing.Point(132, 150);
            this.lblToUpload.Name = "lblToUpload";
            this.lblToUpload.Size = new System.Drawing.Size(0, 13);
            this.lblToUpload.TabIndex = 48;
            // 
            // lblTimeLeft
            // 
            this.lblTimeLeft.AutoSize = true;
            this.lblTimeLeft.Location = new System.Drawing.Point(132, 170);
            this.lblTimeLeft.Name = "lblTimeLeft";
            this.lblTimeLeft.Size = new System.Drawing.Size(0, 13);
            this.lblTimeLeft.TabIndex = 49;
            // 
            // MaxSemLabel
            // 
            this.MaxSemLabel.AutoSize = true;
            this.MaxSemLabel.Location = new System.Drawing.Point(59, 0);
            this.MaxSemLabel.Name = "MaxSemLabel";
            this.MaxSemLabel.Size = new System.Drawing.Size(48, 13);
            this.MaxSemLabel.TabIndex = 36;
            this.MaxSemLabel.TabStop = true;
            this.MaxSemLabel.Text = "MaxSem";
            this.MaxSemLabel.WhichDeveloper = WikiFunctions.Controls.Developers.MaxSem;
            // 
            // ReedyLabel
            // 
            this.ReedyLabel.AutoSize = true;
            this.ReedyLabel.Location = new System.Drawing.Point(3, 0);
            this.ReedyLabel.Name = "ReedyLabel";
            this.ReedyLabel.Size = new System.Drawing.Size(38, 13);
            this.ReedyLabel.TabIndex = 35;
            this.ReedyLabel.TabStop = true;
            this.ReedyLabel.Text = "Reedy";
            this.ReedyLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Reedy;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 13);
            this.label8.TabIndex = 50;
            this.label8.Text = "Checkout expires in: ";
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Interval = 1000;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // About
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 274);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.linkStats);
            this.Controls.Add(this.linkTypoScanPage);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel linkMboverload;
        private System.Windows.Forms.LinkLabel linkTypoScanPage;
        private System.Windows.Forms.LinkLabel linkStats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSaved;
        private System.Windows.Forms.Label lblSkipped;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblLoaded;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblUploaded;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblToUpload;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private WikiFunctions.Controls.DeveloperLinkLabel MaxSemLabel;
        private WikiFunctions.Controls.DeveloperLinkLabel ReedyLabel;
        private System.Windows.Forms.Label lblTimeLeft;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}