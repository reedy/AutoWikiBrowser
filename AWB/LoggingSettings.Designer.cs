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
        /// <param name="disposing">true if managed resources should be disposde; otherwise, false.</param>
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
            this.ApplyButton = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.UploadMaxLinesControl = new System.Windows.Forms.NumericUpDown();
            this.MaxLinesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MaxLinesResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetToMaximumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.UploadJobNameTextBox = new System.Windows.Forms.TextBox();
            this.JobNameContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.JobNameResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.UploadLocationTextBox = new System.Windows.Forms.TextBox();
            this.LocationContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LocationResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoggingCategoryTextBox = new System.Windows.Forms.TextBox();
            this.CategoryTextboxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemCategoryCut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCategoryCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCategoryPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCategoryClear = new System.Windows.Forms.ToolStripMenuItem();
            this.UploadWikiProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.UploadWatchlistCheckBox = new System.Windows.Forms.CheckBox();
            this.UploadOpenInBrowserCheckBox = new System.Windows.Forms.CheckBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Led1 = new WikiFunctions.Controls.LED();
            this.Label8 = new System.Windows.Forms.Label();
            this.UploadsCountLabel = new System.Windows.Forms.Label();
            this.WikiLinesLabel = new System.Windows.Forms.Label();
            this.XHTMLLinesLabel = new System.Windows.Forms.Label();
            this.WikiLinesSinceUploadLabel = new System.Windows.Forms.Label();
            this.XHTMLLinesSinceUploadLabel = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.DebugUploadingCheckBox = new System.Windows.Forms.CheckBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.FolderTextBox = new System.Windows.Forms.TextBox();
            this.FolderButton = new System.Windows.Forms.Button();
            this.UploadCheckBox = new System.Windows.Forms.CheckBox();
            this.WikiLogCheckBox = new System.Windows.Forms.CheckBox();
            this.XHTMLLogCheckBox = new System.Windows.Forms.CheckBox();
            this.VerboseCheckBox = new System.Windows.Forms.CheckBox();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ResetButton = new System.Windows.Forms.Button();
            this.GroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UploadMaxLinesControl)).BeginInit();
            this.MaxLinesContextMenuStrip.SuspendLayout();
            this.JobNameContextMenuStrip.SuspendLayout();
            this.LocationContextMenuStrip.SuspendLayout();
            this.CategoryTextboxContextMenuStrip.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // ApplyButton
            // 
            this.ApplyButton.Enabled = false;
            this.ApplyButton.Location = new System.Drawing.Point(8, 221);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(60, 23);
            this.ApplyButton.TabIndex = 1;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.label11);
            this.GroupBox3.Controls.Add(this.UploadMaxLinesControl);
            this.GroupBox3.Controls.Add(this.label10);
            this.GroupBox3.Controls.Add(this.label9);
            this.GroupBox3.Controls.Add(this.UploadJobNameTextBox);
            this.GroupBox3.Controls.Add(this.label1);
            this.GroupBox3.Controls.Add(this.UploadLocationTextBox);
            this.GroupBox3.Controls.Add(this.LoggingCategoryTextBox);
            this.GroupBox3.Controls.Add(this.UploadWikiProjectCheckBox);
            this.GroupBox3.Controls.Add(this.UploadWatchlistCheckBox);
            this.GroupBox3.Controls.Add(this.UploadOpenInBrowserCheckBox);
            this.GroupBox3.Location = new System.Drawing.Point(142, 3);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(127, 244);
            this.GroupBox3.TabIndex = 3;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Uploading";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(11, 210);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 26);
            this.label11.TabIndex = 9;
            this.label11.Text = "Maximum\r\nlines:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UploadMaxLinesControl
            // 
            this.UploadMaxLinesControl.ContextMenuStrip = this.MaxLinesContextMenuStrip;
            this.UploadMaxLinesControl.Enabled = false;
            this.UploadMaxLinesControl.Location = new System.Drawing.Point(64, 215);
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
            this.UploadMaxLinesControl.Size = new System.Drawing.Size(50, 20);
            this.UploadMaxLinesControl.TabIndex = 10;
            this.UploadMaxLinesControl.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
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
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 128);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "Category:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Current job name:";
            // 
            // UploadJobNameTextBox
            // 
            this.UploadJobNameTextBox.ContextMenuStrip = this.JobNameContextMenuStrip;
            this.UploadJobNameTextBox.Enabled = false;
            this.UploadJobNameTextBox.Location = new System.Drawing.Point(11, 183);
            this.UploadJobNameTextBox.Name = "UploadJobNameTextBox";
            this.UploadJobNameTextBox.Size = new System.Drawing.Size(103, 20);
            this.UploadJobNameTextBox.TabIndex = 8;
            this.UploadJobNameTextBox.Text = "$CATEGORY";
            // 
            // JobNameContextMenuStrip
            // 
            this.JobNameContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.JobNameResetToolStripMenuItem});
            this.JobNameContextMenuStrip.Name = "JobNameContextMenuStrip";
            this.JobNameContextMenuStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // JobNameResetToolStripMenuItem
            // 
            this.JobNameResetToolStripMenuItem.Name = "JobNameResetToolStripMenuItem";
            this.JobNameResetToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.JobNameResetToolStripMenuItem.Text = "Reset";
            this.JobNameResetToolStripMenuItem.Click += new System.EventHandler(this.JobNameReset);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Upload location:";
            // 
            // UploadLocationTextBox
            // 
            this.UploadLocationTextBox.ContextMenuStrip = this.LocationContextMenuStrip;
            this.UploadLocationTextBox.Enabled = false;
            this.UploadLocationTextBox.Location = new System.Drawing.Point(11, 104);
            this.UploadLocationTextBox.Name = "UploadLocationTextBox";
            this.UploadLocationTextBox.Size = new System.Drawing.Size(103, 20);
            this.UploadLocationTextBox.TabIndex = 4;
            this.UploadLocationTextBox.Text = "$USER/Logs";
            // 
            // LocationContextMenuStrip
            // 
            this.LocationContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LocationResetToolStripMenuItem});
            this.LocationContextMenuStrip.Name = "LocationContextMenuStrip";
            this.LocationContextMenuStrip.Size = new System.Drawing.Size(103, 26);
            // 
            // LocationResetToolStripMenuItem
            // 
            this.LocationResetToolStripMenuItem.Name = "LocationResetToolStripMenuItem";
            this.LocationResetToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.LocationResetToolStripMenuItem.Text = "Reset";
            this.LocationResetToolStripMenuItem.Click += new System.EventHandler(this.LocationReset);
            // 
            // LoggingCategoryTextBox
            // 
            this.LoggingCategoryTextBox.ContextMenuStrip = this.CategoryTextboxContextMenuStrip;
            this.LoggingCategoryTextBox.Location = new System.Drawing.Point(11, 144);
            this.LoggingCategoryTextBox.Name = "LoggingCategoryTextBox";
            this.LoggingCategoryTextBox.Size = new System.Drawing.Size(103, 20);
            this.LoggingCategoryTextBox.TabIndex = 6;
            // 
            // CategoryTextboxContextMenuStrip
            // 
            this.CategoryTextboxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCategoryCut,
            this.toolStripMenuItemCategoryCopy,
            this.toolStripMenuItemCategoryPaste,
            this.toolStripMenuItemCategoryClear});
            this.CategoryTextboxContextMenuStrip.Name = "ContextMenuStrip1";
            this.CategoryTextboxContextMenuStrip.Size = new System.Drawing.Size(145, 92);
            // 
            // toolStripMenuItemCategoryCut
            // 
            this.toolStripMenuItemCategoryCut.Name = "toolStripMenuItemCategoryCut";
            this.toolStripMenuItemCategoryCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.toolStripMenuItemCategoryCut.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemCategoryCut.Text = "Cut";
            this.toolStripMenuItemCategoryCut.Click += new System.EventHandler(this.toolStripMenuItemCategoryCut_Click);
            // 
            // toolStripMenuItemCategoryCopy
            // 
            this.toolStripMenuItemCategoryCopy.Name = "toolStripMenuItemCategoryCopy";
            this.toolStripMenuItemCategoryCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMenuItemCategoryCopy.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemCategoryCopy.Text = "Copy";
            this.toolStripMenuItemCategoryCopy.Click += new System.EventHandler(this.toolStripMenuItemCategoryCopy_Click);
            // 
            // toolStripMenuItemCategoryPaste
            // 
            this.toolStripMenuItemCategoryPaste.Name = "toolStripMenuItemCategoryPaste";
            this.toolStripMenuItemCategoryPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.toolStripMenuItemCategoryPaste.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemCategoryPaste.Text = "Paste";
            this.toolStripMenuItemCategoryPaste.Click += new System.EventHandler(this.toolStripMenuItemCategoryPaste_Click);
            // 
            // toolStripMenuItemCategoryClear
            // 
            this.toolStripMenuItemCategoryClear.Name = "toolStripMenuItemCategoryClear";
            this.toolStripMenuItemCategoryClear.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.toolStripMenuItemCategoryClear.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItemCategoryClear.Text = "Clear";
            this.toolStripMenuItemCategoryClear.Click += new System.EventHandler(this.toolStripMenuItemCategoryClear_Click);
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
            this.UploadWikiProjectCheckBox.TabIndex = 0;
            this.UploadWikiProjectCheckBox.Text = "Upload to WPs";
            this.UploadWikiProjectCheckBox.UseVisualStyleBackColor = true;
            this.UploadWikiProjectCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
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
            this.UploadWatchlistCheckBox.TabIndex = 1;
            this.UploadWatchlistCheckBox.Text = "Add to watchlist";
            this.UploadWatchlistCheckBox.UseVisualStyleBackColor = true;
            this.UploadWatchlistCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
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
            this.UploadOpenInBrowserCheckBox.TabIndex = 2;
            this.UploadOpenInBrowserCheckBox.Text = "Open in browser";
            this.UploadOpenInBrowserCheckBox.UseVisualStyleBackColor = true;
            this.UploadOpenInBrowserCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.Led1);
            this.GroupBox2.Controls.Add(this.Label8);
            this.GroupBox2.Controls.Add(this.UploadsCountLabel);
            this.GroupBox2.Controls.Add(this.WikiLinesLabel);
            this.GroupBox2.Controls.Add(this.XHTMLLinesLabel);
            this.GroupBox2.Controls.Add(this.WikiLinesSinceUploadLabel);
            this.GroupBox2.Controls.Add(this.XHTMLLinesSinceUploadLabel);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.Label3);
            this.GroupBox2.Controls.Add(this.Label4);
            this.GroupBox2.Controls.Add(this.Label6);
            this.GroupBox2.Controls.Add(this.Label7);
            this.GroupBox2.Location = new System.Drawing.Point(8, 247);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(261, 95);
            this.GroupBox2.TabIndex = 4;
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
            this.ToolTip1.SetToolTip(this.Led1, "Turns green when writing and blue when uploading");
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
            this.Label7.Location = new System.Drawing.Point(99, 16);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(83, 13);
            this.Label7.TabIndex = 27;
            this.Label7.Text = "Since Upload";
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.DebugUploadingCheckBox);
            this.GroupBox1.Controls.Add(this.GroupBox4);
            this.GroupBox1.Controls.Add(this.UploadCheckBox);
            this.GroupBox1.Controls.Add(this.WikiLogCheckBox);
            this.GroupBox1.Controls.Add(this.XHTMLLogCheckBox);
            this.GroupBox1.Controls.Add(this.VerboseCheckBox);
            this.GroupBox1.Location = new System.Drawing.Point(8, 3);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(131, 212);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Options";
            // 
            // DebugUploadingCheckBox
            // 
            this.DebugUploadingCheckBox.AutoSize = true;
            this.DebugUploadingCheckBox.Location = new System.Drawing.Point(70, 88);
            this.DebugUploadingCheckBox.Name = "DebugUploadingCheckBox";
            this.DebugUploadingCheckBox.Size = new System.Drawing.Size(58, 17);
            this.DebugUploadingCheckBox.TabIndex = 5;
            this.DebugUploadingCheckBox.Text = "Debug";
            this.DebugUploadingCheckBox.UseVisualStyleBackColor = true;
            this.DebugUploadingCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.FolderTextBox);
            this.GroupBox4.Controls.Add(this.FolderButton);
            this.GroupBox4.Location = new System.Drawing.Point(6, 134);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(119, 73);
            this.GroupBox4.TabIndex = 6;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Folder";
            // 
            // FolderTextBox
            // 
            this.FolderTextBox.Location = new System.Drawing.Point(6, 19);
            this.FolderTextBox.Name = "FolderTextBox";
            this.FolderTextBox.Size = new System.Drawing.Size(107, 20);
            this.FolderTextBox.TabIndex = 0;
            this.FolderTextBox.TextChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
            // 
            // FolderButton
            // 
            this.FolderButton.Location = new System.Drawing.Point(6, 45);
            this.FolderButton.Name = "FolderButton";
            this.FolderButton.Size = new System.Drawing.Size(75, 20);
            this.FolderButton.TabIndex = 1;
            this.FolderButton.Text = "Select";
            this.FolderButton.UseVisualStyleBackColor = true;
            this.FolderButton.Click += new System.EventHandler(this.FolderButton_Click);
            // 
            // UploadCheckBox
            // 
            this.UploadCheckBox.AutoSize = true;
            this.UploadCheckBox.Location = new System.Drawing.Point(6, 88);
            this.UploadCheckBox.Name = "UploadCheckBox";
            this.UploadCheckBox.Size = new System.Drawing.Size(60, 17);
            this.UploadCheckBox.TabIndex = 4;
            this.UploadCheckBox.Text = "Upload";
            this.UploadCheckBox.UseVisualStyleBackColor = true;
            this.UploadCheckBox.CheckedChanged += new System.EventHandler(this.UploadCheckBox_CheckedChanged);
            // 
            // WikiLogCheckBox
            // 
            this.WikiLogCheckBox.AutoSize = true;
            this.WikiLogCheckBox.Location = new System.Drawing.Point(6, 42);
            this.WikiLogCheckBox.Name = "WikiLogCheckBox";
            this.WikiLogCheckBox.Size = new System.Drawing.Size(104, 17);
            this.WikiLogCheckBox.TabIndex = 1;
            this.WikiLogCheckBox.Text = "Log to wiki code";
            this.WikiLogCheckBox.UseVisualStyleBackColor = true;
            this.WikiLogCheckBox.CheckedChanged += new System.EventHandler(this.WikiLogCheckBox_CheckedChanged);
            // 
            // XHTMLLogCheckBox
            // 
            this.XHTMLLogCheckBox.AutoSize = true;
            this.XHTMLLogCheckBox.Location = new System.Drawing.Point(6, 19);
            this.XHTMLLogCheckBox.Name = "XHTMLLogCheckBox";
            this.XHTMLLogCheckBox.Size = new System.Drawing.Size(96, 17);
            this.XHTMLLogCheckBox.TabIndex = 0;
            this.XHTMLLogCheckBox.Text = "Log to XHTML";
            this.XHTMLLogCheckBox.UseVisualStyleBackColor = true;
            this.XHTMLLogCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
            // 
            // VerboseCheckBox
            // 
            this.VerboseCheckBox.AutoSize = true;
            this.VerboseCheckBox.Location = new System.Drawing.Point(6, 65);
            this.VerboseCheckBox.Name = "VerboseCheckBox";
            this.VerboseCheckBox.Size = new System.Drawing.Size(102, 17);
            this.VerboseCheckBox.TabIndex = 3;
            this.VerboseCheckBox.Text = "Verbose logging";
            this.VerboseCheckBox.UseVisualStyleBackColor = true;
            this.VerboseCheckBox.CheckedChanged += new System.EventHandler(this.WeHaveUnappliedChanges);
            // 
            // FolderBrowserDialog1
            // 
            this.FolderBrowserDialog1.Description = "Please select the folder where log files should be stored";
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(70, 221);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(66, 23);
            this.ResetButton.TabIndex = 2;
            this.ResetButton.Text = "Turn on";
            this.ToolTip1.SetToolTip(this.ResetButton, "Turn on wiki-code logging with some sensible defaults");
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // LoggingSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.ApplyButton);
            this.Name = "LoggingSettings";
            this.Size = new System.Drawing.Size(276, 349);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UploadMaxLinesControl)).EndInit();
            this.MaxLinesContextMenuStrip.ResumeLayout(false);
            this.JobNameContextMenuStrip.ResumeLayout(false);
            this.LocationContextMenuStrip.ResumeLayout(false);
            this.CategoryTextboxContextMenuStrip.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.CheckBox UploadWikiProjectCheckBox;
        private System.Windows.Forms.CheckBox UploadWatchlistCheckBox;
        private System.Windows.Forms.CheckBox UploadOpenInBrowserCheckBox;
        private System.Windows.Forms.GroupBox GroupBox2;
        private WikiFunctions.Controls.LED Led1;
        private System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label UploadsCountLabel;
        internal System.Windows.Forms.Label WikiLinesLabel;
        internal System.Windows.Forms.Label XHTMLLinesLabel;
        internal System.Windows.Forms.Label WikiLinesSinceUploadLabel;
        internal System.Windows.Forms.Label XHTMLLinesSinceUploadLabel;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.Label Label3;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.Label Label6;
        private System.Windows.Forms.Label Label7;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.GroupBox GroupBox4;
        private System.Windows.Forms.TextBox FolderTextBox;
        private System.Windows.Forms.Button FolderButton;
        private System.Windows.Forms.CheckBox UploadCheckBox;
        internal System.Windows.Forms.CheckBox WikiLogCheckBox;
        internal System.Windows.Forms.CheckBox XHTMLLogCheckBox;
        private System.Windows.Forms.CheckBox VerboseCheckBox;
        private System.Windows.Forms.ContextMenuStrip JobNameContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem JobNameResetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip LocationContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem LocationResetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip MaxLinesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MaxLinesResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetToMaximumToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
        private System.Windows.Forms.ToolTip ToolTip1;
        private System.Windows.Forms.CheckBox DebugUploadingCheckBox;
        private System.Windows.Forms.Button ResetButton;
        internal System.Windows.Forms.TextBox LoggingCategoryTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox UploadLocationTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown UploadMaxLinesControl;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox UploadJobNameTextBox;
        internal System.Windows.Forms.ContextMenuStrip CategoryTextboxContextMenuStrip;
        internal System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCategoryCut;
        internal System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCategoryCopy;
        internal System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCategoryPaste;
        internal System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCategoryClear;
    }
}
