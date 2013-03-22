namespace WikiFunctions.Controls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegexTester));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.ReplaceBtn = new System.Windows.Forms.Button();
            this.Source = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.Captures = new System.Windows.Forms.TreeView();
            this.FindBtn = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.chkSingleline = new System.Windows.Forms.CheckBox();
            this.chkExplicitCapture = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Replace:";
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtFind.Location = new System.Drawing.Point(75, 6);
            this.txtFind.Multiline = true;
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(401, 78);
            this.txtFind.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtFind, "The regular expression to find");
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            this.txtFind.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReplace.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtReplace.Location = new System.Drawing.Point(75, 90);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(401, 22);
            this.txtReplace.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtReplace, "The expression for doing replacements");
            this.txtReplace.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            this.txtReplace.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // ReplaceBtn
            // 
            this.ReplaceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceBtn.Enabled = false;
            this.ReplaceBtn.Location = new System.Drawing.Point(577, 90);
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
            this.Source.Location = new System.Drawing.Point(72, 120);
            this.Source.Multiline = true;
            this.Source.Name = "Source";
            this.Source.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Source.Size = new System.Drawing.Size(596, 160);
            this.Source.TabIndex = 5;
            this.toolTip1.SetToolTip(this.Source, "Enter or paste in the text to be searched here");
            this.Source.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            this.Source.TextChanged += new System.EventHandler(this.ConditionsChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 290);
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
            this.ResultText.Location = new System.Drawing.Point(73, 286);
            this.ResultText.Multiline = true;
            this.ResultText.Name = "ResultText";
            this.ResultText.ReadOnly = true;
            this.ResultText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ResultText.Size = new System.Drawing.Size(595, 164);
            this.ResultText.TabIndex = 10;
            // 
            // Captures
            // 
            this.Captures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Captures.Location = new System.Drawing.Point(72, 286);
            this.Captures.Name = "Captures";
            this.Captures.Size = new System.Drawing.Size(596, 164);
            this.Captures.TabIndex = 13;
            this.toolTip1.SetToolTip(this.Captures, "Displays the result");
            // 
            // FindBtn
            // 
            this.FindBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindBtn.Enabled = false;
            this.FindBtn.Location = new System.Drawing.Point(496, 90);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 465);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(691, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 26);
            this.label4.TabIndex = 15;
            this.label4.Text = "Text to\r\nsearch :";
            this.toolTip1.SetToolTip(this.label4, "Enter or paste in the text to be searched here");
            // 
            // Singleline
            // 
            this.chkSingleline.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkSingleline.AutoSize = true;
            this.chkSingleline.Location = new System.Drawing.Point(90, 20);
            this.chkSingleline.Name = "Singleline";
            this.chkSingleline.Size = new System.Drawing.Size(75, 17);
            this.chkSingleline.TabIndex = 22;
            this.chkSingleline.Text = "SingleLine";
            this.toolTip1.SetToolTip(this.chkSingleline, "Specifies single-line mode. Changes the meaning of the dot (.) so it matches ever" +
                    "y character (instead of every character except \\n)");
            this.chkSingleline.UseVisualStyleBackColor = true;
            // 
            // explicitcapture
            // 
            this.chkExplicitCapture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkExplicitCapture.AutoSize = true;
            this.chkExplicitCapture.Location = new System.Drawing.Point(90, 43);
            this.chkExplicitCapture.Name = "explicitcapture";
            this.chkExplicitCapture.Size = new System.Drawing.Size(96, 17);
            this.chkExplicitCapture.TabIndex = 21;
            this.chkExplicitCapture.Text = "ExplicitCapture";
            this.toolTip1.SetToolTip(this.chkExplicitCapture, resources.GetString("explicitcapture.ToolTip"));
            this.chkExplicitCapture.UseVisualStyleBackColor = true;
            // 
            // Ignorecase
            // 
            this.chkIgnoreCase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.Location = new System.Drawing.Point(6, 43);
            this.chkIgnoreCase.Name = "Ignorecase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(80, 17);
            this.chkIgnoreCase.TabIndex = 20;
            this.chkIgnoreCase.Text = "IgnoreCase";
            this.toolTip1.SetToolTip(this.chkIgnoreCase, "Specifies case-insensitive matching");
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // Multiline
            // 
            this.chkMultiline.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkMultiline.AutoSize = true;
            this.chkMultiline.Location = new System.Drawing.Point(6, 20);
            this.chkMultiline.Name = "Multiline";
            this.chkMultiline.Size = new System.Drawing.Size(68, 17);
            this.chkMultiline.TabIndex = 19;
            this.chkMultiline.Text = "MultiLine";
            this.toolTip1.SetToolTip(this.chkMultiline, "Multiline mode. Changes the meaning of ^ and $ so they match at the beginning and" +
                    " end, respectively, of any line, and not just the beginning and end of the entir" +
                    "e string");
            this.chkMultiline.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkSingleline);
            this.groupBox1.Controls.Add(this.chkExplicitCapture);
            this.groupBox1.Controls.Add(this.chkIgnoreCase);
            this.groupBox1.Controls.Add(this.chkMultiline);
            this.groupBox1.Location = new System.Drawing.Point(482, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(189, 69);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "RegexOptions";
            // 
            // RegexTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 487);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FindBtn);
            this.Controls.Add(this.Captures);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Source);
            this.Controls.Add(this.ReplaceBtn);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ResultText);
            this.HelpButton = true;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(699, 513);
            this.Name = "RegexTester";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AWB Regex Tester";
            this.toolTip1.SetToolTip(this, "The expression for doing replacements");
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.RegexTester_HelpButtonClicked);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RegexTester_KeyPress);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegexTester_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.RegexTester_HelpRequested);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Button ReplaceBtn;
        private System.Windows.Forms.TextBox Source;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.TreeView Captures;
        private System.Windows.Forms.Button FindBtn;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkSingleline;
        private System.Windows.Forms.CheckBox chkExplicitCapture;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.CheckBox chkMultiline;
    }
}

