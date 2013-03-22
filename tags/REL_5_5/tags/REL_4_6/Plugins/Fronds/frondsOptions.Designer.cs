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
            this.btnOptionsOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOptionsOK.Location = new System.Drawing.Point(277, 334);
            this.btnOptionsOK.Name = "btnOptionsOK";
            this.btnOptionsOK.Size = new System.Drawing.Size(75, 23);
            this.btnOptionsOK.TabIndex = 1;
            this.btnOptionsOK.Text = "Okay";
            this.btnOptionsOK.UseVisualStyleBackColor = true;
            this.btnOptionsOK.Click += new System.EventHandler(this.btnOptionsOK_Click);
            // 
            // btnOptionsCancel
            // 
            this.btnOptionsCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptionsCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOptionsCancel.Location = new System.Drawing.Point(359, 334);
            this.btnOptionsCancel.Name = "btnOptionsCancel";
            this.btnOptionsCancel.Size = new System.Drawing.Size(75, 23);
            this.btnOptionsCancel.TabIndex = 2;
            this.btnOptionsCancel.Text = "Cancel";
            this.btnOptionsCancel.UseVisualStyleBackColor = true;
            this.btnOptionsCancel.Click += new System.EventHandler(this.btnOptionsCancel_Click);
            // 
            // listOptionsFronds
            // 
            this.listOptionsFronds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listOptionsFronds.FormattingEnabled = true;
            this.listOptionsFronds.Location = new System.Drawing.Point(12, 89);
            this.listOptionsFronds.Name = "listOptionsFronds";
            this.listOptionsFronds.Size = new System.Drawing.Size(421, 229);
            this.listOptionsFronds.TabIndex = 3;
            // 
            // lblOptionsDesc
            // 
            this.lblOptionsDesc.AutoSize = true;
            this.lblOptionsDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOptionsDesc.Location = new System.Drawing.Point(9, 15);
            this.lblOptionsDesc.Name = "lblOptionsDesc";
            this.lblOptionsDesc.Size = new System.Drawing.Size(421, 60);
            this.lblOptionsDesc.TabIndex = 4;
            this.lblOptionsDesc.Text = resources.GetString("lblOptionsDesc.Text");
            // 
            // linkToolserver
            // 
            this.linkToolserver.AutoSize = true;
            this.linkToolserver.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkToolserver.Location = new System.Drawing.Point(290, 62);
            this.linkToolserver.Name = "linkToolserver";
            this.linkToolserver.Size = new System.Drawing.Size(144, 14);
            this.linkToolserver.TabIndex = 5;
            this.linkToolserver.TabStop = true;
            this.linkToolserver.Text = "toolserver.org/~jarry/fronds/";
            this.linkToolserver.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkToolserver_LinkClicked);
            // 
            // linkWikipedia
            // 
            this.linkWikipedia.AutoSize = true;
            this.linkWikipedia.Font = new System.Drawing.Font("Arial", 8.25F);
            this.linkWikipedia.Location = new System.Drawing.Point(58, 47);
            this.linkWikipedia.Name = "linkWikipedia";
            this.linkWikipedia.Size = new System.Drawing.Size(172, 14);
            this.linkWikipedia.TabIndex = 6;
            this.linkWikipedia.TabStop = true;
            this.linkWikipedia.Text = "en.wikipedia.org/wiki/WP:FRONDS";
            this.linkWikipedia.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkWikipedia_LinkClicked);
            // 
            // FrondsOptions
            // 
            this.AcceptButton = this.btnOptionsOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOptionsCancel;
            this.ClientSize = new System.Drawing.Size(445, 369);
            this.Controls.Add(this.linkWikipedia);
            this.Controls.Add(this.linkToolserver);
            this.Controls.Add(this.lblOptionsDesc);
            this.Controls.Add(this.listOptionsFronds);
            this.Controls.Add(this.btnOptionsCancel);
            this.Controls.Add(this.btnOptionsOK);
            this.Name = "FrondsOptions";
            this.ShowIcon = false;
            this.Text = "Fronds";
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