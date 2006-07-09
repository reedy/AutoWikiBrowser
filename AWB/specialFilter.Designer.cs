namespace AutoWikiBrowser
{
    partial class specialFilter
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
            this.btnDone = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCategory = new System.Windows.Forms.CheckBox();
            this.chkTemplate = new System.Windows.Forms.CheckBox();
            this.rdoFilterout = new System.Windows.Forms.RadioButton();
            this.rdoFilterIn = new System.Windows.Forms.RadioButton();
            this.chkWikipedia = new System.Windows.Forms.CheckBox();
            this.chkUser = new System.Windows.Forms.CheckBox();
            this.chkImage = new System.Windows.Forms.CheckBox();
            this.chkArticleTalk = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnDone
            // 
            this.btnDone.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDone.Location = new System.Drawing.Point(279, 153);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 0;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(279, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Location = new System.Drawing.Point(46, 59);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(68, 17);
            this.chkCategory.TabIndex = 2;
            this.chkCategory.Text = "Category";
            this.chkCategory.UseVisualStyleBackColor = true;
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Location = new System.Drawing.Point(46, 83);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(70, 17);
            this.chkTemplate.TabIndex = 3;
            this.chkTemplate.Text = "Template";
            this.chkTemplate.UseVisualStyleBackColor = true;
            // 
            // rdoFilterout
            // 
            this.rdoFilterout.AutoSize = true;
            this.rdoFilterout.Location = new System.Drawing.Point(87, 12);
            this.rdoFilterout.Name = "rdoFilterout";
            this.rdoFilterout.Size = new System.Drawing.Size(65, 17);
            this.rdoFilterout.TabIndex = 4;
            this.rdoFilterout.Text = "Filter out";
            this.rdoFilterout.UseVisualStyleBackColor = true;
            // 
            // rdoFilterIn
            // 
            this.rdoFilterIn.AutoSize = true;
            this.rdoFilterIn.Checked = true;
            this.rdoFilterIn.Location = new System.Drawing.Point(23, 12);
            this.rdoFilterIn.Name = "rdoFilterIn";
            this.rdoFilterIn.Size = new System.Drawing.Size(58, 17);
            this.rdoFilterIn.TabIndex = 5;
            this.rdoFilterIn.TabStop = true;
            this.rdoFilterIn.Text = "Filter in";
            this.rdoFilterIn.UseVisualStyleBackColor = true;
            // 
            // chkWikipedia
            // 
            this.chkWikipedia.AutoSize = true;
            this.chkWikipedia.Location = new System.Drawing.Point(46, 129);
            this.chkWikipedia.Name = "chkWikipedia";
            this.chkWikipedia.Size = new System.Drawing.Size(73, 17);
            this.chkWikipedia.TabIndex = 6;
            this.chkWikipedia.Text = "Wikipedia";
            this.chkWikipedia.UseVisualStyleBackColor = true;
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Location = new System.Drawing.Point(46, 150);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(51, 17);
            this.chkUser.TabIndex = 7;
            this.chkUser.Text = "User:";
            this.chkUser.UseVisualStyleBackColor = true;
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Location = new System.Drawing.Point(46, 106);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(55, 17);
            this.chkImage.TabIndex = 8;
            this.chkImage.Text = "Image";
            this.chkImage.UseVisualStyleBackColor = true;
            // 
            // chkArticleTalk
            // 
            this.chkArticleTalk.AutoSize = true;
            this.chkArticleTalk.Location = new System.Drawing.Point(46, 36);
            this.chkArticleTalk.Name = "chkArticleTalk";
            this.chkArticleTalk.Size = new System.Drawing.Size(75, 17);
            this.chkArticleTalk.TabIndex = 10;
            this.chkArticleTalk.Text = "Article talk";
            this.chkArticleTalk.UseVisualStyleBackColor = true;
            // 
            // txtContains
            // 
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(230, 33);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(124, 20);
            this.txtContains.TabIndex = 11;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(207, 12);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(152, 17);
            this.chkContains.TabIndex = 12;
            this.chkContains.Text = "Filter out titles that contain:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // specialFilter
            // 
            this.AcceptButton = this.btnDone;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(375, 223);
            this.ControlBox = false;
            this.Controls.Add(this.chkContains);
            this.Controls.Add(this.txtContains);
            this.Controls.Add(this.chkArticleTalk);
            this.Controls.Add(this.chkImage);
            this.Controls.Add(this.chkUser);
            this.Controls.Add(this.chkWikipedia);
            this.Controls.Add(this.rdoFilterIn);
            this.Controls.Add(this.rdoFilterout);
            this.Controls.Add(this.chkTemplate);
            this.Controls.Add(this.chkCategory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDone);
            this.Name = "specialFilter";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Special filter";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCategory;
        private System.Windows.Forms.CheckBox chkTemplate;
        private System.Windows.Forms.RadioButton rdoFilterout;
        private System.Windows.Forms.RadioButton rdoFilterIn;
        private System.Windows.Forms.CheckBox chkWikipedia;
        private System.Windows.Forms.CheckBox chkUser;
        private System.Windows.Forms.CheckBox chkImage;
        private System.Windows.Forms.CheckBox chkArticleTalk;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContains;
    }
}