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
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkCategory = new System.Windows.Forms.CheckBox();
            this.chkTemplate = new System.Windows.Forms.CheckBox();
            this.chkWikipedia = new System.Windows.Forms.CheckBox();
            this.chkUser = new System.Windows.Forms.CheckBox();
            this.chkImage = new System.Windows.Forms.CheckBox();
            this.chkArticleTalk = new System.Windows.Forms.CheckBox();
            this.txtContains = new System.Windows.Forms.TextBox();
            this.chkContains = new System.Windows.Forms.CheckBox();
            this.chkIsRegex = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkNotContains = new System.Windows.Forms.CheckBox();
            this.txtDoesNotContain = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(115, 164);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(196, 164);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCategory
            // 
            this.chkCategory.AutoSize = true;
            this.chkCategory.Location = new System.Drawing.Point(204, 49);
            this.chkCategory.Name = "chkCategory";
            this.chkCategory.Size = new System.Drawing.Size(71, 17);
            this.chkCategory.TabIndex = 2;
            this.chkCategory.Text = "Category:";
            this.chkCategory.UseVisualStyleBackColor = true;
            this.chkCategory.Visible = false;
            // 
            // chkTemplate
            // 
            this.chkTemplate.AutoSize = true;
            this.chkTemplate.Location = new System.Drawing.Point(204, 73);
            this.chkTemplate.Name = "chkTemplate";
            this.chkTemplate.Size = new System.Drawing.Size(73, 17);
            this.chkTemplate.TabIndex = 3;
            this.chkTemplate.Text = "Template:";
            this.chkTemplate.UseVisualStyleBackColor = true;
            this.chkTemplate.Visible = false;
            // 
            // chkWikipedia
            // 
            this.chkWikipedia.AutoSize = true;
            this.chkWikipedia.Location = new System.Drawing.Point(204, 119);
            this.chkWikipedia.Name = "chkWikipedia";
            this.chkWikipedia.Size = new System.Drawing.Size(76, 17);
            this.chkWikipedia.TabIndex = 6;
            this.chkWikipedia.Text = "Wikipedia:";
            this.chkWikipedia.UseVisualStyleBackColor = true;
            this.chkWikipedia.Visible = false;
            // 
            // chkUser
            // 
            this.chkUser.AutoSize = true;
            this.chkUser.Location = new System.Drawing.Point(204, 140);
            this.chkUser.Name = "chkUser";
            this.chkUser.Size = new System.Drawing.Size(51, 17);
            this.chkUser.TabIndex = 7;
            this.chkUser.Text = "User:";
            this.chkUser.UseVisualStyleBackColor = true;
            this.chkUser.Visible = false;
            // 
            // chkImage
            // 
            this.chkImage.AutoSize = true;
            this.chkImage.Location = new System.Drawing.Point(204, 96);
            this.chkImage.Name = "chkImage";
            this.chkImage.Size = new System.Drawing.Size(58, 17);
            this.chkImage.TabIndex = 8;
            this.chkImage.Text = "Image:";
            this.chkImage.UseVisualStyleBackColor = true;
            this.chkImage.Visible = false;
            // 
            // chkArticleTalk
            // 
            this.chkArticleTalk.AutoSize = true;
            this.chkArticleTalk.Location = new System.Drawing.Point(204, 26);
            this.chkArticleTalk.Name = "chkArticleTalk";
            this.chkArticleTalk.Size = new System.Drawing.Size(79, 17);
            this.chkArticleTalk.TabIndex = 10;
            this.chkArticleTalk.Text = "Talk pages";
            this.chkArticleTalk.UseVisualStyleBackColor = true;
            this.chkArticleTalk.Visible = false;
            // 
            // txtContains
            // 
            this.txtContains.Enabled = false;
            this.txtContains.Location = new System.Drawing.Point(12, 51);
            this.txtContains.Name = "txtContains";
            this.txtContains.Size = new System.Drawing.Size(178, 20);
            this.txtContains.TabIndex = 11;
            // 
            // chkContains
            // 
            this.chkContains.AutoSize = true;
            this.chkContains.Location = new System.Drawing.Point(12, 30);
            this.chkContains.Name = "chkContains";
            this.chkContains.Size = new System.Drawing.Size(152, 17);
            this.chkContains.TabIndex = 12;
            this.chkContains.Text = "Filter out titles that contain:";
            this.chkContains.UseVisualStyleBackColor = true;
            this.chkContains.CheckedChanged += new System.EventHandler(this.chkContains_CheckedChanged);
            // 
            // chkIsRegex
            // 
            this.chkIsRegex.AutoSize = true;
            this.chkIsRegex.Enabled = false;
            this.chkIsRegex.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIsRegex.Location = new System.Drawing.Point(12, 7);
            this.chkIsRegex.Name = "chkIsRegex";
            this.chkIsRegex.Size = new System.Drawing.Size(120, 17);
            this.chkIsRegex.TabIndex = 13;
            this.chkIsRegex.Text = "Regular expressions";
            this.chkIsRegex.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(204, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Filter out:";
            this.label1.Visible = false;
            // 
            // chkNotContains
            // 
            this.chkNotContains.AutoSize = true;
            this.chkNotContains.Location = new System.Drawing.Point(12, 95);
            this.chkNotContains.Name = "chkNotContains";
            this.chkNotContains.Size = new System.Drawing.Size(178, 17);
            this.chkNotContains.TabIndex = 15;
            this.chkNotContains.Text = "Filter out titles that don\'t contain:";
            this.chkNotContains.UseVisualStyleBackColor = true;
            this.chkNotContains.CheckedChanged += new System.EventHandler(this.chkNotContains_CheckedChanged);
            // 
            // txtDoesNotContain
            // 
            this.txtDoesNotContain.Enabled = false;
            this.txtDoesNotContain.Location = new System.Drawing.Point(12, 118);
            this.txtDoesNotContain.Name = "txtDoesNotContain";
            this.txtDoesNotContain.Size = new System.Drawing.Size(178, 20);
            this.txtDoesNotContain.TabIndex = 16;
            // 
            // specialFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 199);
            this.Controls.Add(this.txtDoesNotContain);
            this.Controls.Add(this.chkNotContains);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkIsRegex);
            this.Controls.Add(this.chkContains);
            this.Controls.Add(this.txtContains);
            this.Controls.Add(this.chkArticleTalk);
            this.Controls.Add(this.chkImage);
            this.Controls.Add(this.chkUser);
            this.Controls.Add(this.chkWikipedia);
            this.Controls.Add(this.chkTemplate);
            this.Controls.Add(this.chkCategory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCategory;
        private System.Windows.Forms.CheckBox chkTemplate;
        private System.Windows.Forms.CheckBox chkWikipedia;
        private System.Windows.Forms.CheckBox chkUser;
        private System.Windows.Forms.CheckBox chkImage;
        private System.Windows.Forms.CheckBox chkArticleTalk;
        private System.Windows.Forms.TextBox txtContains;
        private System.Windows.Forms.CheckBox chkContains;
        private System.Windows.Forms.CheckBox chkIsRegex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkNotContains;
        private System.Windows.Forms.TextBox txtDoesNotContain;
    }
}