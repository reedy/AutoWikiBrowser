namespace AutoWikiBrowser
{
    partial class LoggingSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CloseAllButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.UploadMaxLinesControl = new System.Windows.Forms.NumericUpDown();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.UploadJobNameTextBox = new System.Windows.Forms.TextBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.UploadLocationTextBox = new System.Windows.Forms.TextBox();
            this.UploadWikiProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.UploadWatchlistCheckBox = new System.Windows.Forms.CheckBox();
            this.UploadOpenInBrowserCheckBox = new System.Windows.Forms.CheckBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Led1 = new WikiFunctions.Controls.LED();
            this.Label8 = new System.Windows.Forms.Label();
            this.UploadsCountLabel = new System.Windows.Forms.Label();
            this.WikiLinesLabel = new System.Windows.Forms.Label();
            this.XHTMLLinesLabel = new System.Windows.Forms.Label();
            this.BadTagsLinesLabel = new System.Windows.Forms.Label();
            this.WikiLinesSinceUploadLabel = new System.Windows.Forms.Label();
            this.XHTMLLinesSinceUploadLabel = new System.Windows.Forms.Label();
            this.BadTagsLinesSinceUploadLabel = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.FolderTextBox = new System.Windows.Forms.TextBox();
            this.FolderButton = new System.Windows.Forms.Button();
            this.UploadCheckBox = new System.Windows.Forms.CheckBox();
            this.WikiLogCheckBox = new System.Windows.Forms.CheckBox();
            this.LogBadTagsCheckBox = new System.Windows.Forms.CheckBox();
            this.XHTMLLogCheckBox = new System.Windows.Forms.CheckBox();
            this.VerboseCheckBox = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.JobNameContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.JobNameResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LocationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LocationResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MaxLinesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MaxLinesResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetToMaximumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GroupBox3.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UploadMaxLinesControl)).BeginInit();
            this.GroupBox6.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.JobNameContextMenuStrip.SuspendLayout();
            this.LocationContextMenuStrip.SuspendLayout();
            this.MaxLinesContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseAllButton
            // 
            this.CloseAllButton.Location = new System.Drawing.Point(80, 222);
            this.CloseAllButton.Name = "CloseAllButton";
            this.CloseAllButton.Size = new System.Drawing.Size(56, 23);
            this.CloseAllButton.TabIndex = 27;
            this.CloseAllButton.Text = "Close";
            this.CloseAllButton.UseVisualStyleBackColor = true;
            this.CloseAllButton.Click += new System.EventHandler(this.CloseAllButton_Click);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Enabled = false;
            this.ApplyButton.Location = new System.Drawing.Point(14, 222);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(60, 23);
            this.ApplyButton.TabIndex = 26;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.GroupBox7);
            this.GroupBox3.Controls.Add(this.GroupBox6);
            this.GroupBox3.Controls.Add(this.GroupBox5);
            this.GroupBox3.Controls.Add(this.UploadWikiProjectCheckBox);
            this.GroupBox3.Controls.Add(this.UploadWatchlistCheckBox);
            this.GroupBox3.Controls.Add(this.UploadOpenInBrowserCheckBox);
            this.GroupBox3.Location = new System.Drawing.Point(142, 7);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(127, 244);
            this.GroupBox3.TabIndex = 25;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Uploading";
            // 
            // GroupBox7
            // 
            this.GroupBox7.Controls.Add(this.UploadMaxLinesControl);
            this.GroupBox7.Location = new System.Drawing.Point(6, 195);
            this.GroupBox7.Name = "GroupBox7";
            this.GroupBox7.Size = new System.Drawing.Size(114, 43);
            this.GroupBox7.TabIndex = 23;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Maximum Lines";
            // 
            // UploadMaxLinesControl
            // 
            this.UploadMaxLinesControl.Enabled = false;
            this.UploadMaxLinesControl.Location = new System.Drawing.Point(8, 17);
            this.UploadMaxLinesControl.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.UploadMaxLinesControl.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.UploadMaxLinesControl.Name = "UploadMaxLinesControl";
            this.UploadMaxLinesControl.Size = new System.Drawing.Size(103, 20);
            this.UploadMaxLinesControl.TabIndex = 0;
            this.UploadMaxLinesControl.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.UploadMaxLinesControl.ValueChanged += new System.EventHandler(this.UploadMaxLines_ValueChanged);
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.UploadJobNameTextBox);
            this.GroupBox6.Location = new System.Drawing.Point(6, 143);
            this.GroupBox6.Name = "GroupBox6";
            this.GroupBox6.Size = new System.Drawing.Size(115, 45);
            this.GroupBox6.TabIndex = 0;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Current Job Name";
            // 
            // UploadJobNameTextBox
            // 
            this.UploadJobNameTextBox.Enabled = false;
            this.UploadJobNameTextBox.Location = new System.Drawing.Point(8, 19);
            this.UploadJobNameTextBox.Name = "UploadJobNameTextBox";
            this.UploadJobNameTextBox.Size = new System.Drawing.Size(103, 20);
            this.UploadJobNameTextBox.TabIndex = 0;
            this.UploadJobNameTextBox.Text = "$CATEGORY";
            this.UploadJobNameTextBox.TextChanged += new System.EventHandler(this.UploadJobNameTextBox_TextChanged);
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.UploadLocationTextBox);
            this.GroupBox5.Location = new System.Drawing.Point(6, 88);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(115, 49);
            this.GroupBox5.TabIndex = 22;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Upload Location";
            // 
            // UploadLocationTextBox
            // 
            this.UploadLocationTextBox.Enabled = false;
            this.UploadLocationTextBox.Location = new System.Drawing.Point(8, 19);
            this.UploadLocationTextBox.Name = "UploadLocationTextBox";
            this.UploadLocationTextBox.Size = new System.Drawing.Size(103, 20);
            this.UploadLocationTextBox.TabIndex = 21;
            this.UploadLocationTextBox.Text = "$USER/Logs";
            this.UploadLocationTextBox.TextChanged += new System.EventHandler(this.UploadLocationTextBox_TextChanged);
            // 
            // UploadWikiProjectCheckBox
            // 
            this.UploadWikiProjectCheckBox.AutoSize = true;
            this.UploadWikiProjectCheckBox.Checked = true;
            this.UploadWikiProjectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UploadWikiProjectCheckBox.Enabled = false;
            this.UploadWikiProjectCheckBox.Location = new System.Drawing.Point(11, 19);
            this.UploadWikiProjectCheckBox.Name = "UploadWikiProjectCheckBox";
            this.UploadWikiProjectCheckBox.Size = new System.Drawing.Size(98, 17);
            this.UploadWikiProjectCheckBox.TabIndex = 19;
            this.UploadWikiProjectCheckBox.Text = "Upload to WPs";
            this.UploadWikiProjectCheckBox.UseVisualStyleBackColor = true;
            this.UploadWikiProjectCheckBox.CheckedChanged += new System.EventHandler(this.UploadWikiProjectCheckBox_CheckedChanged);
            // 
            // UploadWatchlistCheckBox
            // 
            this.UploadWatchlistCheckBox.AutoSize = true;
            this.UploadWatchlistCheckBox.Checked = true;
            this.UploadWatchlistCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UploadWatchlistCheckBox.Enabled = false;
            this.UploadWatchlistCheckBox.Location = new System.Drawing.Point(11, 42);
            this.UploadWatchlistCheckBox.Name = "UploadWatchlistCheckBox";
            this.UploadWatchlistCheckBox.Size = new System.Drawing.Size(101, 17);
            this.UploadWatchlistCheckBox.TabIndex = 20;
            this.UploadWatchlistCheckBox.Text = "Add to watchlist";
            this.UploadWatchlistCheckBox.UseVisualStyleBackColor = true;
            this.UploadWatchlistCheckBox.CheckedChanged += new System.EventHandler(this.UploadWatchlistCheckBox_CheckedChanged);
            // 
            // UploadOpenInBrowserCheckBox
            // 
            this.UploadOpenInBrowserCheckBox.AutoSize = true;
            this.UploadOpenInBrowserCheckBox.Checked = true;
            this.UploadOpenInBrowserCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UploadOpenInBrowserCheckBox.Enabled = false;
            this.UploadOpenInBrowserCheckBox.Location = new System.Drawing.Point(11, 65);
            this.UploadOpenInBrowserCheckBox.Name = "UploadOpenInBrowserCheckBox";
            this.UploadOpenInBrowserCheckBox.Size = new System.Drawing.Size(103, 17);
            this.UploadOpenInBrowserCheckBox.TabIndex = 21;
            this.UploadOpenInBrowserCheckBox.Text = "Open in browser";
            this.UploadOpenInBrowserCheckBox.UseVisualStyleBackColor = true;
            this.UploadOpenInBrowserCheckBox.CheckedChanged += new System.EventHandler(this.UploadOpenInBrowserCheckBox_CheckedChanged);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.Led1);
            this.GroupBox2.Controls.Add(this.Label8);
            this.GroupBox2.Controls.Add(this.UploadsCountLabel);
            this.GroupBox2.Controls.Add(this.WikiLinesLabel);
            this.GroupBox2.Controls.Add(this.XHTMLLinesLabel);
            this.GroupBox2.Controls.Add(this.BadTagsLinesLabel);
            this.GroupBox2.Controls.Add(this.WikiLinesSinceUploadLabel);
            this.GroupBox2.Controls.Add(this.XHTMLLinesSinceUploadLabel);
            this.GroupBox2.Controls.Add(this.BadTagsLinesSinceUploadLabel);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.Label3);
            this.GroupBox2.Controls.Add(this.Label4);
            this.GroupBox2.Controls.Add(this.Label5);
            this.GroupBox2.Controls.Add(this.Label6);
            this.GroupBox2.Controls.Add(this.Label7);
            this.GroupBox2.Location = new System.Drawing.Point(8, 247);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(261, 95);
            this.GroupBox2.TabIndex = 24;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Status";
            // 
            // Led1
            // 
            this.Led1.Colour = WikiFunctions.Controls.Colour.Red;
            this.Led1.Location = new System.Drawing.Point(231, 69);
            this.Led1.Name = "Led1";
            this.Led1.Size = new System.Drawing.Size(20, 20);
            this.Led1.TabIndex = 31;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(198, 16);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(53, 13);
            this.Label8.TabIndex = 22;
            this.Label8.Text = "Uploads";
            // 
            // UploadsCountLabel
            // 
            this.UploadsCountLabel.AutoSize = true;
            this.UploadsCountLabel.Location = new System.Drawing.Point(198, 36);
            this.UploadsCountLabel.Name = "UploadsCountLabel";
            this.UploadsCountLabel.Size = new System.Drawing.Size(27, 13);
            this.UploadsCountLabel.TabIndex = 23;
            this.UploadsCountLabel.Text = "N/A";
            // 
            // WikiLinesLabel
            // 
            this.WikiLinesLabel.AutoSize = true;
            this.WikiLinesLabel.Location = new System.Drawing.Point(56, 36);
            this.WikiLinesLabel.Name = "WikiLinesLabel";
            this.WikiLinesLabel.Size = new System.Drawing.Size(27, 13);
            this.WikiLinesLabel.TabIndex = 25;
            this.WikiLinesLabel.Text = "N/A";
            // 
            // XHTMLLinesLabel
            // 
            this.XHTMLLinesLabel.AutoSize = true;
            this.XHTMLLinesLabel.Location = new System.Drawing.Point(56, 56);
            this.XHTMLLinesLabel.Name = "XHTMLLinesLabel";
            this.XHTMLLinesLabel.Size = new System.Drawing.Size(27, 13);
            this.XHTMLLinesLabel.TabIndex = 26;
            this.XHTMLLinesLabel.Text = "N/A";
            // 
            // BadTagsLinesLabel
            // 
            this.BadTagsLinesLabel.AutoSize = true;
            this.BadTagsLinesLabel.Location = new System.Drawing.Point(56, 77);
            this.BadTagsLinesLabel.Name = "BadTagsLinesLabel";
            this.BadTagsLinesLabel.Size = new System.Drawing.Size(27, 13);
            this.BadTagsLinesLabel.TabIndex = 27;
            this.BadTagsLinesLabel.Text = "N/A";
            // 
            // WikiLinesSinceUploadLabel
            // 
            this.WikiLinesSinceUploadLabel.AutoSize = true;
            this.WikiLinesSinceUploadLabel.Location = new System.Drawing.Point(123, 36);
            this.WikiLinesSinceUploadLabel.Name = "WikiLinesSinceUploadLabel";
            this.WikiLinesSinceUploadLabel.Size = new System.Drawing.Size(27, 13);
            this.WikiLinesSinceUploadLabel.TabIndex = 28;
            this.WikiLinesSinceUploadLabel.Text = "N/A";
            // 
            // XHTMLLinesSinceUploadLabel
            // 
            this.XHTMLLinesSinceUploadLabel.AutoSize = true;
            this.XHTMLLinesSinceUploadLabel.Location = new System.Drawing.Point(123, 56);
            this.XHTMLLinesSinceUploadLabel.Name = "XHTMLLinesSinceUploadLabel";
            this.XHTMLLinesSinceUploadLabel.Size = new System.Drawing.Size(27, 13);
            this.XHTMLLinesSinceUploadLabel.TabIndex = 29;
            this.XHTMLLinesSinceUploadLabel.Text = "N/A";
            // 
            // BadTagsLinesSinceUploadLabel
            // 
            this.BadTagsLinesSinceUploadLabel.AutoSize = true;
            this.BadTagsLinesSinceUploadLabel.Location = new System.Drawing.Point(123, 76);
            this.BadTagsLinesSinceUploadLabel.Name = "BadTagsLinesSinceUploadLabel";
            this.BadTagsLinesSinceUploadLabel.Size = new System.Drawing.Size(27, 13);
            this.BadTagsLinesSinceUploadLabel.TabIndex = 30;
            this.BadTagsLinesSinceUploadLabel.Text = "N/A";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(9, 16);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(28, 13);
            this.Label2.TabIndex = 22;
            this.Label2.Text = "Log";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(9, 36);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(32, 13);
            this.Label3.TabIndex = 23;
            this.Label3.Text = "Wiki";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(9, 56);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(41, 13);
            this.Label4.TabIndex = 24;
            this.Label4.Text = "HMTL";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(9, 76);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(29, 13);
            this.Label5.TabIndex = 25;
            this.Label5.Text = "Bad";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(56, 16);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(37, 13);
            this.Label6.TabIndex = 26;
            this.Label6.Text = "Lines";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(103, 16);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(65, 13);
            this.Label7.TabIndex = 27;
            this.Label7.Text = "Since U/L";
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.GroupBox4);
            this.GroupBox1.Controls.Add(this.UploadCheckBox);
            this.GroupBox1.Controls.Add(this.WikiLogCheckBox);
            this.GroupBox1.Controls.Add(this.LogBadTagsCheckBox);
            this.GroupBox1.Controls.Add(this.XHTMLLogCheckBox);
            this.GroupBox1.Controls.Add(this.VerboseCheckBox);
            this.GroupBox1.Location = new System.Drawing.Point(8, 7);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(131, 212);
            this.GroupBox1.TabIndex = 23;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Options";
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.FolderTextBox);
            this.GroupBox4.Controls.Add(this.FolderButton);
            this.GroupBox4.Location = new System.Drawing.Point(6, 134);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(114, 73);
            this.GroupBox4.TabIndex = 20;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Folder";
            // 
            // FolderTextBox
            // 
            this.FolderTextBox.Location = new System.Drawing.Point(6, 19);
            this.FolderTextBox.Name = "FolderTextBox";
            this.FolderTextBox.Size = new System.Drawing.Size(100, 20);
            this.FolderTextBox.TabIndex = 15;
            this.FolderTextBox.TextChanged += new System.EventHandler(this.FolderTextBox_TextChanged);
            // 
            // FolderButton
            // 
            this.FolderButton.Location = new System.Drawing.Point(6, 45);
            this.FolderButton.Name = "FolderButton";
            this.FolderButton.Size = new System.Drawing.Size(75, 20);
            this.FolderButton.TabIndex = 14;
            this.FolderButton.Text = "Select";
            this.FolderButton.UseVisualStyleBackColor = true;
            this.FolderButton.Click += new System.EventHandler(this.FolderButton_Click);
            // 
            // UploadCheckBox
            // 
            this.UploadCheckBox.AutoSize = true;
            this.UploadCheckBox.Location = new System.Drawing.Point(6, 111);
            this.UploadCheckBox.Name = "UploadCheckBox";
            this.UploadCheckBox.Size = new System.Drawing.Size(60, 17);
            this.UploadCheckBox.TabIndex = 18;
            this.UploadCheckBox.Text = "Upload";
            this.UploadCheckBox.UseVisualStyleBackColor = true;
            this.UploadCheckBox.CheckedChanged += new System.EventHandler(this.UploadCheckBox_CheckedChanged);
            // 
            // WikiLogCheckBox
            // 
            this.WikiLogCheckBox.AutoSize = true;
            this.WikiLogCheckBox.Checked = true;
            this.WikiLogCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.WikiLogCheckBox.Location = new System.Drawing.Point(6, 65);
            this.WikiLogCheckBox.Name = "WikiLogCheckBox";
            this.WikiLogCheckBox.Size = new System.Drawing.Size(104, 17);
            this.WikiLogCheckBox.TabIndex = 12;
            this.WikiLogCheckBox.Text = "Log to wiki code";
            this.WikiLogCheckBox.UseVisualStyleBackColor = true;
            this.WikiLogCheckBox.CheckedChanged += new System.EventHandler(this.WikiLogCheckBox_CheckedChanged);
            // 
            // LogBadTagsCheckBox
            // 
            this.LogBadTagsCheckBox.AutoSize = true;
            this.LogBadTagsCheckBox.Checked = true;
            this.LogBadTagsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LogBadTagsCheckBox.Location = new System.Drawing.Point(6, 19);
            this.LogBadTagsCheckBox.Name = "LogBadTagsCheckBox";
            this.LogBadTagsCheckBox.Size = new System.Drawing.Size(116, 17);
            this.LogBadTagsCheckBox.TabIndex = 10;
            this.LogBadTagsCheckBox.Text = "Log problem pages";
            this.LogBadTagsCheckBox.UseVisualStyleBackColor = true;
            this.LogBadTagsCheckBox.CheckedChanged += new System.EventHandler(this.LogBadTagsCheckBox_CheckedChanged);
            // 
            // XHTMLLogCheckBox
            // 
            this.XHTMLLogCheckBox.AutoSize = true;
            this.XHTMLLogCheckBox.Location = new System.Drawing.Point(6, 42);
            this.XHTMLLogCheckBox.Name = "XHTMLLogCheckBox";
            this.XHTMLLogCheckBox.Size = new System.Drawing.Size(96, 17);
            this.XHTMLLogCheckBox.TabIndex = 11;
            this.XHTMLLogCheckBox.Text = "Log to XHTML";
            this.XHTMLLogCheckBox.UseVisualStyleBackColor = true;
            this.XHTMLLogCheckBox.CheckedChanged += new System.EventHandler(this.XHTMLLogCheckBox_CheckedChanged);
            // 
            // VerboseCheckBox
            // 
            this.VerboseCheckBox.AutoSize = true;
            this.VerboseCheckBox.Location = new System.Drawing.Point(6, 88);
            this.VerboseCheckBox.Name = "VerboseCheckBox";
            this.VerboseCheckBox.Size = new System.Drawing.Size(102, 17);
            this.VerboseCheckBox.TabIndex = 13;
            this.VerboseCheckBox.Text = "Verbose logging";
            this.VerboseCheckBox.UseVisualStyleBackColor = true;
            this.VerboseCheckBox.CheckedChanged += new System.EventHandler(this.VerboseCheckBox_CheckedChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(20, 111);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(36, 13);
            this.Label1.TabIndex = 22;
            this.Label1.Text = "Folder";
            // 
            // JobNameContextMenuStrip
            // 
            this.JobNameContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JobNameResetToolStripMenuItem});
            this.JobNameContextMenuStrip.Name = "JobNameContextMenuStrip";
            this.JobNameContextMenuStrip.Size = new System.Drawing.Size(114, 26);
            // 
            // JobNameResetToolStripMenuItem
            // 
            this.JobNameResetToolStripMenuItem.Name = "JobNameResetToolStripMenuItem";
            this.JobNameResetToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.JobNameResetToolStripMenuItem.Text = "Reset";
            this.JobNameResetToolStripMenuItem.Click += new System.EventHandler(this.JobNameReset);
            // 
            // LocationContextMenuStrip
            // 
            this.LocationContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LocationResetToolStripMenuItem});
            this.LocationContextMenuStrip.Name = "LocationContextMenuStrip";
            this.LocationContextMenuStrip.Size = new System.Drawing.Size(114, 26);
            // 
            // LocationResetToolStripMenuItem
            // 
            this.LocationResetToolStripMenuItem.Name = "LocationResetToolStripMenuItem";
            this.LocationResetToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.LocationResetToolStripMenuItem.Text = "Reset";
            this.LocationResetToolStripMenuItem.Click += new System.EventHandler(this.LocationReset);
            // 
            // MaxLinesContextMenuStrip
            // 
            this.MaxLinesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MaxLinesResetToolStripMenuItem,
            this.SetToMaximumToolStripMenuItem});
            this.MaxLinesContextMenuStrip.Name = "MaxLinesContextMenuStrip";
            this.MaxLinesContextMenuStrip.Size = new System.Drawing.Size(162, 48);
            // 
            // MaxLinesResetToolStripMenuItem
            // 
            this.MaxLinesResetToolStripMenuItem.Name = "MaxLinesResetToolStripMenuItem";
            this.MaxLinesResetToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.MaxLinesResetToolStripMenuItem.Text = "Reset";
            this.MaxLinesResetToolStripMenuItem.Click += new System.EventHandler(this.MaxLinesReset);
            // 
            // SetToMaximumToolStripMenuItem
            // 
            this.SetToMaximumToolStripMenuItem.Name = "SetToMaximumToolStripMenuItem";
            this.SetToMaximumToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.SetToMaximumToolStripMenuItem.Text = "Set to maximum";
            this.SetToMaximumToolStripMenuItem.Click += new System.EventHandler(this.SetLinesToMaximum);
            // 
            // PluginLogging
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CloseAllButton);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Label1);
            this.Name = "PluginLogging";
            this.Size = new System.Drawing.Size(276, 349);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UploadMaxLinesControl)).EndInit();
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox6.PerformLayout();
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.JobNameContextMenuStrip.ResumeLayout(false);
            this.LocationContextMenuStrip.ResumeLayout(false);
            this.MaxLinesContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseAllButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.GroupBox GroupBox3;
        private System.Windows.Forms.GroupBox GroupBox7;
        private System.Windows.Forms.NumericUpDown UploadMaxLinesControl;
        private System.Windows.Forms.GroupBox GroupBox6;
        private System.Windows.Forms.TextBox UploadJobNameTextBox;
        private System.Windows.Forms.GroupBox GroupBox5;
        private System.Windows.Forms.TextBox UploadLocationTextBox;
        internal System.Windows.Forms.CheckBox UploadWikiProjectCheckBox;
        private System.Windows.Forms.CheckBox UploadWatchlistCheckBox;
        private System.Windows.Forms.CheckBox UploadOpenInBrowserCheckBox;
        private System.Windows.Forms.GroupBox GroupBox2;
        private WikiFunctions.Controls.LED Led1;
        private System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label UploadsCountLabel;
        internal System.Windows.Forms.Label WikiLinesLabel;
        internal System.Windows.Forms.Label XHTMLLinesLabel;
        internal System.Windows.Forms.Label BadTagsLinesLabel;
        internal System.Windows.Forms.Label WikiLinesSinceUploadLabel;
        internal System.Windows.Forms.Label XHTMLLinesSinceUploadLabel;
        internal System.Windows.Forms.Label BadTagsLinesSinceUploadLabel;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.Label Label7;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.GroupBox GroupBox4;
        private System.Windows.Forms.TextBox FolderTextBox;
        private System.Windows.Forms.Button FolderButton;
        private System.Windows.Forms.CheckBox UploadCheckBox;
        internal System.Windows.Forms.CheckBox WikiLogCheckBox;
        internal System.Windows.Forms.CheckBox LogBadTagsCheckBox;
        internal System.Windows.Forms.CheckBox XHTMLLogCheckBox;
        private System.Windows.Forms.CheckBox VerboseCheckBox;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ContextMenuStrip JobNameContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem JobNameResetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip LocationContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem LocationResetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip MaxLinesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MaxLinesResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetToMaximumToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
        private System.Windows.Forms.ToolTip ToolTip1;
    }
}
