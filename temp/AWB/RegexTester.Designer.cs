namespace AutoWikiBrowser
{
    partial class RegexTester
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Find = new System.Windows.Forms.TextBox();
            this.Replace = new System.Windows.Forms.TextBox();
            this.Go = new System.Windows.Forms.Button();
            this.Source = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultList = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.Multiline = new System.Windows.Forms.CheckBox();
            this.Casesensitive = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Replace:";
            // 
            // Find
            // 
            this.Find.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Find.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Find.Location = new System.Drawing.Point(75, 6);
            this.Find.Name = "Find";
            this.Find.Size = new System.Drawing.Size(415, 22);
            this.Find.TabIndex = 2;
            this.Find.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // Replace
            // 
            this.Replace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Replace.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Replace.Location = new System.Drawing.Point(75, 32);
            this.Replace.Name = "Replace";
            this.Replace.Size = new System.Drawing.Size(415, 22);
            this.Replace.TabIndex = 3;
            this.Replace.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // Go
            // 
            this.Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Go.Location = new System.Drawing.Point(596, 19);
            this.Go.Name = "Go";
            this.Go.Size = new System.Drawing.Size(75, 23);
            this.Go.TabIndex = 4;
            this.Go.Text = "Go!";
            this.Go.UseVisualStyleBackColor = true;
            this.Go.Click += new System.EventHandler(this.Go_Click);
            // 
            // Source
            // 
            this.Source.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Source.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Source.Location = new System.Drawing.Point(12, 69);
            this.Source.Multiline = true;
            this.Source.Name = "Source";
            this.Source.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Source.Size = new System.Drawing.Size(659, 171);
            this.Source.TabIndex = 5;
            this.Source.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Result:";
            // 
            // ResultList
            // 
            this.ResultList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultList.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultList.FormattingEnabled = true;
            this.ResultList.ItemHeight = 16;
            this.ResultList.Location = new System.Drawing.Point(12, 274);
            this.ResultList.Name = "ResultList";
            this.ResultList.Size = new System.Drawing.Size(659, 164);
            this.ResultList.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 457);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(683, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(0, 17);
            // 
            // ResultText
            // 
            this.ResultText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultText.BackColor = System.Drawing.SystemColors.Window;
            this.ResultText.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ResultText.Location = new System.Drawing.Point(12, 274);
            this.ResultText.Multiline = true;
            this.ResultText.Name = "ResultText";
            this.ResultText.ReadOnly = true;
            this.ResultText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultText.Size = new System.Drawing.Size(659, 164);
            this.ResultText.TabIndex = 10;
            // 
            // Multiline
            // 
            this.Multiline.AutoSize = true;
            this.Multiline.Checked = true;
            this.Multiline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Multiline.Location = new System.Drawing.Point(496, 12);
            this.Multiline.Name = "Multiline";
            this.Multiline.Size = new System.Drawing.Size(64, 17);
            this.Multiline.TabIndex = 11;
            this.Multiline.Text = "Multiline";
            this.Multiline.UseVisualStyleBackColor = true;
            // 
            // Casesensitive
            // 
            this.Casesensitive.AutoSize = true;
            this.Casesensitive.Checked = true;
            this.Casesensitive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Casesensitive.Location = new System.Drawing.Point(496, 35);
            this.Casesensitive.Name = "Casesensitive";
            this.Casesensitive.Size = new System.Drawing.Size(95, 17);
            this.Casesensitive.TabIndex = 12;
            this.Casesensitive.Text = "Case sensitive";
            this.Casesensitive.UseVisualStyleBackColor = true;
            // 
            // RegexTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 479);
            this.Controls.Add(this.Casesensitive);
            this.Controls.Add(this.Multiline);
            this.Controls.Add(this.ResultText);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ResultList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Source);
            this.Controls.Add(this.Go);
            this.Controls.Add(this.Replace);
            this.Controls.Add(this.Find);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "RegexTester";
            this.ShowIcon = false;
            this.Text = "Test regexes";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegexTester_KeyPress);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Find;
        private System.Windows.Forms.TextBox Replace;
        private System.Windows.Forms.Button Go;
        private System.Windows.Forms.TextBox Source;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox ResultList;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.CheckBox Multiline;
        private System.Windows.Forms.CheckBox Casesensitive;
    }
}

