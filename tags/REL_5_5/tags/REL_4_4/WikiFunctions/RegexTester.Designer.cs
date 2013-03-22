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
            this.lblFind = new System.Windows.Forms.Label();
            this.lblReplace = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.ReplaceBtn = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ResultText = new System.Windows.Forms.TextBox();
            this.Captures = new System.Windows.Forms.TreeView();
            this.FindBtn = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.chkSingleline = new System.Windows.Forms.CheckBox();
            this.chkExplicitCapture = new System.Windows.Forms.CheckBox();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.chkMultiline = new System.Windows.Forms.CheckBox();
            this.gbRegexOptions = new System.Windows.Forms.GroupBox();
            this.statusStrip1.SuspendLayout();
            this.gbRegexOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.Location = new System.Drawing.Point(12, 9);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(30, 13);
            this.lblFind.TabIndex = 0;
            this.lblFind.Text = "Fi&nd:";
            this.toolTip1.SetToolTip(this.lblFind, "The regular expression to find");
            // 
            // lblReplace
            // 
            this.lblReplace.AutoSize = true;
            this.lblReplace.Location = new System.Drawing.Point(12, 90);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(50, 13);
            this.lblReplace.TabIndex = 2;
            this.lblReplace.Text = "Replace:";
            // 
            // txtFind
            // 
            this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFind.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtFind.Location = new System.Drawing.Point(72, 6);
            this.txtFind.Multiline = true;
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(404, 78);
            this.txtFind.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtFind, "The regular expression to find");
            this.txtFind.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // txtReplace
            // 
            this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReplace.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtReplace.Location = new System.Drawing.Point(72, 90);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(404, 22);
            this.txtReplace.TabIndex = 3;
            this.toolTip1.SetToolTip(this.txtReplace, "The expression for doing replacements");
            this.txtReplace.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtReplace.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // ReplaceBtn
            // 
            this.ReplaceBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceBtn.Enabled = false;
            this.ReplaceBtn.Location = new System.Drawing.Point(577, 90);
            this.ReplaceBtn.Name = "ReplaceBtn";
            this.ReplaceBtn.Size = new System.Drawing.Size(75, 23);
            this.ReplaceBtn.TabIndex = 5;
            this.ReplaceBtn.Text = "&Replace";
            this.ReplaceBtn.UseVisualStyleBackColor = true;
            this.ReplaceBtn.Click += new System.EventHandler(this.Replace_Click);
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtInput.Location = new System.Drawing.Point(72, 120);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(596, 160);
            this.txtInput.TabIndex = 8;
            this.toolTip1.SetToolTip(this.txtInput, "Enter or paste in the text to be searched here");
            this.txtInput.TextChanged += new System.EventHandler(this.ConditionsChanged);
            this.txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPressHandler);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 9;
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
            this.Captures.TabIndex = 11;
            this.toolTip1.SetToolTip(this.Captures, "Displays the result");
            // 
            // FindBtn
            // 
            this.FindBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FindBtn.Enabled = false;
            this.FindBtn.Location = new System.Drawing.Point(496, 90);
            this.FindBtn.Name = "FindBtn";
            this.FindBtn.Size = new System.Drawing.Size(75, 23);
            this.FindBtn.TabIndex = 4;
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
            this.progressBar,
            this.Status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 465);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(691, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 26);
            this.label4.TabIndex = 7;
            this.label4.Text = "Text to\r\nsearch :";
            this.toolTip1.SetToolTip(this.label4, "Enter or paste in the text to be searched here");
            // 
            // chkSingleline
            // 
            this.chkSingleline.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkSingleline.AutoSize = true;
            this.chkSingleline.Location = new System.Drawing.Point(90, 20);
            this.chkSingleline.Name = "chkSingleline";
            this.chkSingleline.Size = new System.Drawing.Size(75, 17);
            this.chkSingleline.TabIndex = 2;
            this.chkSingleline.Text = "&SingleLine";
            this.toolTip1.SetToolTip(this.chkSingleline, "Specifies single-line mode. Changes the meaning of the dot (.) so it matches ever" +
                    "y character (instead of every character except \\n)");
            this.chkSingleline.UseVisualStyleBackColor = true;
            // 
            // chkExplicitCapture
            // 
            this.chkExplicitCapture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkExplicitCapture.AutoSize = true;
            this.chkExplicitCapture.Location = new System.Drawing.Point(90, 43);
            this.chkExplicitCapture.Name = "chkExplicitCapture";
            this.chkExplicitCapture.Size = new System.Drawing.Size(96, 17);
            this.chkExplicitCapture.TabIndex = 3;
            this.chkExplicitCapture.Text = "Explicit&Capture";
            this.toolTip1.SetToolTip(this.chkExplicitCapture, resources.GetString("chkExplicitCapture.ToolTip"));
            this.chkExplicitCapture.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreCase
            // 
            this.chkIgnoreCase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.Location = new System.Drawing.Point(6, 43);
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(80, 17);
            this.chkIgnoreCase.TabIndex = 1;
            this.chkIgnoreCase.Text = "&IgnoreCase";
            this.toolTip1.SetToolTip(this.chkIgnoreCase, "Specifies case-insensitive matching");
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // chkMultiline
            // 
            this.chkMultiline.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkMultiline.AutoSize = true;
            this.chkMultiline.Location = new System.Drawing.Point(6, 20);
            this.chkMultiline.Name = "chkMultiline";
            this.chkMultiline.Size = new System.Drawing.Size(68, 17);
            this.chkMultiline.TabIndex = 0;
            this.chkMultiline.Text = "&MultiLine";
            this.toolTip1.SetToolTip(this.chkMultiline, "Multiline mode. Changes the meaning of ^ and $ so they match at the beginning and" +
                    " end, respectively, of any line, and not just the beginning and end of the entir" +
                    "e string");
            this.chkMultiline.UseVisualStyleBackColor = true;
            // 
            // gbRegexOptions
            // 
            this.gbRegexOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRegexOptions.Controls.Add(this.chkSingleline);
            this.gbRegexOptions.Controls.Add(this.chkExplicitCapture);
            this.gbRegexOptions.Controls.Add(this.chkIgnoreCase);
            this.gbRegexOptions.Controls.Add(this.chkMultiline);
            this.gbRegexOptions.Location = new System.Drawing.Point(482, 6);
            this.gbRegexOptions.Name = "gbRegexOptions";
            this.gbRegexOptions.Size = new System.Drawing.Size(189, 69);
            this.gbRegexOptions.TabIndex = 6;
            this.gbRegexOptions.TabStop = false;
            this.gbRegexOptions.Text = "RegexOptions";
            // 
            // RegexTester
            // 
            this.AcceptButton = this.FindBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 487);
            this.Controls.Add(this.gbRegexOptions);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FindBtn);
            this.Controls.Add(this.Captures);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.ReplaceBtn);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.lblReplace);
            this.Controls.Add(this.lblFind);
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbRegexOptions.ResumeLayout(false);
            this.gbRegexOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Button ReplaceBtn;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ResultText;
        private System.Windows.Forms.TreeView Captures;
        private System.Windows.Forms.Button FindBtn;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbRegexOptions;
        private System.Windows.Forms.CheckBox chkSingleline;
        private System.Windows.Forms.CheckBox chkExplicitCapture;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
        private System.Windows.Forms.CheckBox chkMultiline;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
    }
}

