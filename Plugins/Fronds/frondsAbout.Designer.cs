namespace Fronds
{
    partial class FrondsAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrondsAbout));
            this.btnAboutOK = new System.Windows.Forms.Button();
            this.lblAbout = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAboutOK
            // 
            resources.ApplyResources(this.btnAboutOK, "btnAboutOK");
            this.btnAboutOK.Name = "btnAboutOK";
            this.btnAboutOK.UseVisualStyleBackColor = true;
            this.btnAboutOK.Click += new System.EventHandler(this.btnAboutOK_Click);
            // 
            // lblAbout
            // 
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.Name = "lblAbout";
            // 
            // FrondsAbout
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblAbout);
            this.Controls.Add(this.btnAboutOK);
            this.Name = "FrondsAbout";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.FrondsAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAboutOK;
        private System.Windows.Forms.Label lblAbout;
    }
}