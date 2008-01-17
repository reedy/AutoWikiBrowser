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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Find = new System.Windows.Forms.TextBox();
            this.Replace = new System.Windows.Forms.TextBox();
            this.ReplaceBtn = new System.Windows.Forms.Button();
            this.Source = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.Multiline = new System.Windows.Forms.CheckBox();
            this.Ignorecase = new System.Windows.Forms.CheckBox();
            this.Captures = new System.Windows.Forms.TreeView();
            this.FindBtn = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.explicitcapture = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Singleline = new System.Windows.Forms.CheckBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find:";
            this.toolTip1.SetToolTip(this.label1, "The regular expression to find");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
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
            this.toolTip1.SetToolTip(this.Find, "The regular expression to find");
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
            this.toolTip1.SetToolTip(this.Replace, "The expression for doing replacements");
            this.Replace.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // ReplaceBtn
            // 
            this.ReplaceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceBtn.Location = new System.Drawing.Point(353, 57);
            this.ReplaceBtn.Name = "ReplaceBtn";
            this.ReplaceBtn.Size = new System.Drawing.Size(75, 23);
            this.ReplaceBtn.TabIndex = 4;
            this.ReplaceBtn.Text = "Replace";
            this.ReplaceBtn.UseVisualStyleBackColor = true;
            this.ReplaceBtn.Click += new System.EventHandler(this.Replace_Click);
            // 
            // Source
            // 
            this.Source.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Source.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Source.Location = new System.Drawing.Point(12, 80);
            this.Source.Multiline = true;
            this.Source.Name = "Source";
            this.Source.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Source.Size = new System.Drawing.Size(659, 160);
            this.Source.TabIndex = 5;
            this.toolTip1.SetToolTip(this.Source, "Enter or paste in the text to be searched here");
            this.Source.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 251);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Result:";
            this.toolTip1.SetToolTip(this.label3, "Displays the result");
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
            this.Multiline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Multiline.AutoSize = true;
            this.Multiline.Location = new System.Drawing.Point(496, 31);
            this.Multiline.Name = "Multiline";
            this.Multiline.Size = new System.Drawing.Size(68, 17);
            this.Multiline.TabIndex = 11;
            this.Multiline.Text = "MultiLine";
            this.toolTip1.SetToolTip(this.Multiline, "Multiline mode. Changes the meaning of ^ and $ so they match at the beginning and" +
                    " end, respectively, of any line, and not just the beginning and end of the entir" +
                    "e string");
            this.Multiline.UseVisualStyleBackColor = true;
            // 
            // Ignorecase
            // 
            this.Ignorecase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Ignorecase.AutoSize = true;
            this.Ignorecase.Location = new System.Drawing.Point(496, 57);
            this.Ignorecase.Name = "Ignorecase";
            this.Ignorecase.Size = new System.Drawing.Size(80, 17);
            this.Ignorecase.TabIndex = 12;
            this.Ignorecase.Text = "IgnoreCase";
            this.toolTip1.SetToolTip(this.Ignorecase, "Specifies case-insensitive matching");
            this.Ignorecase.UseVisualStyleBackColor = true;
            // 
            // Captures
            // 
            this.Captures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Captures.Location = new System.Drawing.Point(12, 274);
            this.Captures.Name = "Captures";
            this.Captures.Size = new System.Drawing.Size(659, 164);
            this.Captures.TabIndex = 13;
            this.toolTip1.SetToolTip(this.Captures, "Displays the result");
            // 
            // FindBtn
            // 
            this.FindBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindBtn.Location = new System.Drawing.Point(257, 57);
            this.FindBtn.Name = "FindBtn";
            this.FindBtn.Size = new System.Drawing.Size(75, 23);
            this.FindBtn.TabIndex = 14;
            this.FindBtn.Text = "Find";
            this.FindBtn.UseVisualStyleBackColor = true;
            this.FindBtn.Click += new System.EventHandler(this.FindBtn_Click);
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(0, 17);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Text to search :";
            this.toolTip1.SetToolTip(this.label4, "Enter or paste in the text to be searched here");
            // 
            // explicitcapture
            // 
            this.explicitcapture.AutoSize = true;
            this.explicitcapture.Location = new System.Drawing.Point(583, 57);
            this.explicitcapture.Name = "explicitcapture";
            this.explicitcapture.Size = new System.Drawing.Size(96, 17);
            this.explicitcapture.TabIndex = 16;
            this.explicitcapture.Text = "ExplicitCapture";
            this.explicitcapture.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(534, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "RegexOptions:";
            this.toolTip1.SetToolTip(this.label5, ".net RegexOptions:");
            // 
            // Singleline
            // 
            this.Singleline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Singleline.AutoSize = true;
            this.Singleline.Location = new System.Drawing.Point(583, 31);
            this.Singleline.Name = "Singleline";
            this.Singleline.Size = new System.Drawing.Size(75, 17);
            this.Singleline.TabIndex = 18;
            this.Singleline.Text = "SingleLine";
            this.toolTip1.SetToolTip(this.Singleline, "Specifies single-line mode. Changes the meaning of the dot (.) so it matches ever" +
                    "y character (instead of every character except \\n)");
            this.Singleline.UseVisualStyleBackColor = true;
            // 
            // RegexTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 479);
            this.Controls.Add(this.Singleline);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.explicitcapture);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FindBtn);
            this.Controls.Add(this.Captures);
            this.Controls.Add(this.Ignorecase);
            this.Controls.Add(this.Multiline);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Source);
            this.Controls.Add(this.ReplaceBtn);
            this.Controls.Add(this.Replace);
            this.Controls.Add(this.Find);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ResultText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(699, 513);
            this.Name = "RegexTester";
            this.ShowIcon = false;
            this.Text = "Test regexes";
            this.toolTip1.SetToolTip(this, "The expression for doing replacements");
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
        private System.Windows.Forms.Button ReplaceBtn;
        private System.Windows.Forms.TextBox Source;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.CheckBox Multiline;
        private System.Windows.Forms.CheckBox Ignorecase;
        private System.Windows.Forms.TreeView Captures;
        private System.Windows.Forms.Button FindBtn;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox explicitcapture;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox Singleline;
    }
}

