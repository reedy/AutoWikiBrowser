namespace AutoWikiBrowser
{
    partial class CustomModule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomModule));
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnDone = new System.Windows.Forms.Button();
            this.btnMake = new System.Windows.Forms.Button();
            this.cmboLang = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStart = new System.Windows.Forms.Label();
            this.lblEnd = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkModuleEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.Location = new System.Drawing.Point(12, 185);
            this.txtCode.Multiline = true;
            this.txtCode.Name = "txtCode";
            this.txtCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtCode.Size = new System.Drawing.Size(640, 154);
            this.txtCode.TabIndex = 0;
            this.txtCode.Text = resources.GetString("txtCode.Text");
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(490, 11);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 1;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // btnMake
            // 
            this.btnMake.Location = new System.Drawing.Point(397, 11);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(87, 23);
            this.btnMake.TabIndex = 2;
            this.btnMake.Text = "Make module";
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // cmboLang
            // 
            this.cmboLang.FormattingEnabled = true;
            this.cmboLang.Items.AddRange(new object[] {
            "C# 2.0",
            "Visual Basic.NET 2005"});
            this.cmboLang.Location = new System.Drawing.Point(254, 13);
            this.cmboLang.Name = "cmboLang";
            this.cmboLang.Size = new System.Drawing.Size(137, 21);
            this.cmboLang.TabIndex = 3;
            this.cmboLang.SelectedIndexChanged += new System.EventHandler(this.cmboLang_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(193, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Language";
            // 
            // lblStart
            // 
            this.lblStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStart.Location = new System.Drawing.Point(12, 52);
            this.lblStart.Name = "lblStart";
            this.lblStart.Size = new System.Drawing.Size(643, 130);
            this.lblStart.TabIndex = 5;
            this.lblStart.Text = resources.GetString("lblStart.Text");
            this.lblStart.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblEnd
            // 
            this.lblEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEnd.Location = new System.Drawing.Point(9, 342);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(643, 36);
            this.lblEnd.TabIndex = 6;
            this.lblEnd.Text = "    }\r\n}";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblStatus.Location = new System.Drawing.Point(94, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(93, 13);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "No module loaded";
            // 
            // chkModuleEnabled
            // 
            this.chkModuleEnabled.AutoSize = true;
            this.chkModuleEnabled.Location = new System.Drawing.Point(12, 15);
            this.chkModuleEnabled.Name = "chkModuleEnabled";
            this.chkModuleEnabled.Size = new System.Drawing.Size(65, 17);
            this.chkModuleEnabled.TabIndex = 8;
            this.chkModuleEnabled.Text = "Enabled";
            this.chkModuleEnabled.UseVisualStyleBackColor = true;
            // 
            // CustomModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 387);
            this.Controls.Add(this.chkModuleEnabled);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblEnd);
            this.Controls.Add(this.lblStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmboLang);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.txtCode);
            this.Name = "CustomModule";
            this.ShowIcon = false;
            this.Text = "Module";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CSParser_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.ComboBox cmboLang;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStart;
        private System.Windows.Forms.Label lblEnd;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkModuleEnabled;
    }
}