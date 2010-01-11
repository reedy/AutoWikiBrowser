namespace WikiFunctions.Controls.Lists
{
    partial class ProtectionLevel
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
            this.lbLevel = new System.Windows.Forms.ListBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lbType = new System.Windows.Forms.ListBox();
            this.lblType = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbLevel
            // 
            this.lbLevel.FormattingEnabled = true;
            this.lbLevel.Items.AddRange(new object[] {
            "Autoconfirmed",
            "Sysop",
            "Autoconfirmed & Sysop"});
            this.lbLevel.Location = new System.Drawing.Point(166, 25);
            this.lbLevel.Name = "lbLevel";
            this.lbLevel.Size = new System.Drawing.Size(145, 43);
            this.lbLevel.TabIndex = 2;
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(163, 9);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(33, 13);
            this.lblLevel.TabIndex = 20;
            this.lblLevel.Text = "Level";
            // 
            // lbType
            // 
            this.lbType.FormattingEnabled = true;
            this.lbType.Items.AddRange(new object[] {
            "Edit protection",
            "Move protection",
            "Edit & Move protection"});
            this.lbType.Location = new System.Drawing.Point(15, 25);
            this.lbType.Name = "lbType";
            this.lbType.Size = new System.Drawing.Size(145, 43);
            this.lbType.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 9);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 18;
            this.lblType.Text = "Type";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(236, 74);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ProtectionLevel
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 103);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lbLevel);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lbType);
            this.Controls.Add(this.lblType);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ProtectionLevel";
            this.Text = "Protection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbLevel;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.ListBox lbType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Button btnOk;

    }
}