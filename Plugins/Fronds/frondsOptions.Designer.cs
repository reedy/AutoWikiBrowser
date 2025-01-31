namespace Fronds
{
    partial class FrondsOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrondsOptions));
            this.btnOptionsOK = new System.Windows.Forms.Button();
            this.btnOptionsCancel = new System.Windows.Forms.Button();
            this.listOptionsFronds = new System.Windows.Forms.CheckedListBox();
            this.lblOptionsDesc = new System.Windows.Forms.Label();
            this.linkToolserver = new System.Windows.Forms.LinkLabel();
            this.linkWikipedia = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnOptionsOK
            // 
            resources.ApplyResources(this.btnOptionsOK, "btnOptionsOK");
            this.btnOptionsOK.Name = "btnOptionsOK";
            this.btnOptionsOK.UseVisualStyleBackColor = true;
            this.btnOptionsOK.Click += new System.EventHandler(this.btnOptionsOK_Click);
            // 
            // btnOptionsCancel
            // 
            resources.ApplyResources(this.btnOptionsCancel, "btnOptionsCancel");
            this.btnOptionsCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOptionsCancel.Name = "btnOptionsCancel";
            this.btnOptionsCancel.UseVisualStyleBackColor = true;
            this.btnOptionsCancel.Click += new System.EventHandler(this.btnOptionsCancel_Click);
            // 
            // listOptionsFronds
            // 
            resources.ApplyResources(this.listOptionsFronds, "listOptionsFronds");
            this.listOptionsFronds.FormattingEnabled = true;
            this.listOptionsFronds.Name = "listOptionsFronds";
            // 
            // lblOptionsDesc
            // 
            resources.ApplyResources(this.lblOptionsDesc, "lblOptionsDesc");
            this.lblOptionsDesc.Name = "lblOptionsDesc";
            // 
            // linkToolserver
            // 
            resources.ApplyResources(this.linkToolserver, "linkToolserver");
            this.linkToolserver.Name = "linkToolserver";
            this.linkToolserver.TabStop = true;
            this.linkToolserver.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkToolserver_LinkClicked);
            // 
            // linkWikipedia
            // 
            resources.ApplyResources(this.linkWikipedia, "linkWikipedia");
            this.linkWikipedia.Name = "linkWikipedia";
            this.linkWikipedia.TabStop = true;
            this.linkWikipedia.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWikipedia_LinkClicked);
            // 
            // FrondsOptions
            // 
            this.AcceptButton = this.btnOptionsOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOptionsCancel;
            this.Controls.Add(this.linkWikipedia);
            this.Controls.Add(this.linkToolserver);
            this.Controls.Add(this.lblOptionsDesc);
            this.Controls.Add(this.listOptionsFronds);
            this.Controls.Add(this.btnOptionsCancel);
            this.Controls.Add(this.btnOptionsOK);
            this.Name = "FrondsOptions";
            this.ShowIcon = false;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOptionsOK;
        private System.Windows.Forms.Button btnOptionsCancel;
        private System.Windows.Forms.CheckedListBox listOptionsFronds;
        private System.Windows.Forms.Label lblOptionsDesc;
        private System.Windows.Forms.LinkLabel linkToolserver;
        private System.Windows.Forms.LinkLabel linkWikipedia;
    }
}