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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
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
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.MaxSemLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.ReedyLabel = new WikiFunctions.Controls.DeveloperLinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.lblToUpload = new System.Windows.Forms.Label();
            this.lblTimeLeft = new System.Windows.Forms.Label();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // linkMboverload
            // 
            resources.ApplyResources(this.linkMboverload, "linkMboverload");
            this.linkMboverload.Name = "linkMboverload";
            this.linkMboverload.TabStop = true;
            this.linkMboverload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMboverload_LinkClicked);
            // 
            // linkTypoScanPage
            // 
            resources.ApplyResources(this.linkTypoScanPage, "linkTypoScanPage");
            this.linkTypoScanPage.Name = "linkTypoScanPage";
            this.linkTypoScanPage.TabStop = true;
            this.linkTypoScanPage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTypoScanPage_LinkClicked);
            // 
            // linkStats
            // 
            resources.ApplyResources(this.linkStats, "linkStats");
            this.linkStats.Name = "linkStats";
            this.linkStats.TabStop = true;
            this.linkStats.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkStats_LinkClicked);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblSaved
            // 
            resources.ApplyResources(this.lblSaved, "lblSaved");
            this.lblSaved.Name = "lblSaved";
            // 
            // lblSkipped
            // 
            resources.ApplyResources(this.lblSkipped, "lblSkipped");
            this.lblSkipped.Name = "lblSkipped";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblLoaded
            // 
            resources.ApplyResources(this.lblLoaded, "lblLoaded");
            this.lblLoaded.Name = "lblLoaded";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // lblUploaded
            // 
            resources.ApplyResources(this.lblUploaded, "lblUploaded");
            this.lblUploaded.Name = "lblUploaded";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
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
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.MaxSemLabel, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.ReedyLabel, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // MaxSemLabel
            // 
            resources.ApplyResources(this.MaxSemLabel, "MaxSemLabel");
            this.MaxSemLabel.Name = "MaxSemLabel";
            this.MaxSemLabel.TabStop = true;
            this.MaxSemLabel.WhichDeveloper = WikiFunctions.Controls.Developers.MaxSem;
            // 
            // ReedyLabel
            // 
            resources.ApplyResources(this.ReedyLabel, "ReedyLabel");
            this.ReedyLabel.Name = "ReedyLabel";
            this.ReedyLabel.TabStop = true;
            this.ReedyLabel.WhichDeveloper = WikiFunctions.Controls.Developers.Reedy;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // lblToUpload
            // 
            resources.ApplyResources(this.lblToUpload, "lblToUpload");
            this.lblToUpload.Name = "lblToUpload";
            // 
            // lblTimeLeft
            // 
            resources.ApplyResources(this.lblTimeLeft, "lblTimeLeft");
            this.lblTimeLeft.Name = "lblTimeLeft";
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
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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