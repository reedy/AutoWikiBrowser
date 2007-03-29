Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class PluginLogging
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
            Me.ToolTip2 = New System.Windows.Forms.ToolTip(Me.components)
            Me.VerboseCheckBox = New System.Windows.Forms.CheckBox
            Me.XHTMLLogCheckBox = New System.Windows.Forms.CheckBox
            Me.WikiLogCheckBox = New System.Windows.Forms.CheckBox
            Me.LogBadTagsCheckBox = New System.Windows.Forms.CheckBox
            Me.UploadCheckBox = New System.Windows.Forms.CheckBox
            Me.UploadWikiProjectCheckBox = New System.Windows.Forms.CheckBox
            Me.UploadOpenInBrowserCheckBox = New System.Windows.Forms.CheckBox
            Me.CloseAllButton = New System.Windows.Forms.Button
            Me.Label1 = New System.Windows.Forms.Label
            Me.FolderTextBox = New System.Windows.Forms.TextBox
            Me.FolderButton = New System.Windows.Forms.Button
            Me.GroupBox1 = New System.Windows.Forms.GroupBox
            Me.GroupBox4 = New System.Windows.Forms.GroupBox
            Me.GroupBox2 = New System.Windows.Forms.GroupBox
            Me.Label8 = New System.Windows.Forms.Label
            Me.UploadsCountLabel = New System.Windows.Forms.Label
            Me.WikiLinesLabel = New System.Windows.Forms.Label
            Me.XHTMLLinesLabel = New System.Windows.Forms.Label
            Me.BadTagsLinesLabel = New System.Windows.Forms.Label
            Me.WikiLinesSinceUploadLabel = New System.Windows.Forms.Label
            Me.XHTMLLinesSinceUploadLabel = New System.Windows.Forms.Label
            Me.BadTagsLinesSinceUploadLabel = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label3 = New System.Windows.Forms.Label
            Me.Label4 = New System.Windows.Forms.Label
            Me.Label5 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.Label7 = New System.Windows.Forms.Label
            Me.GroupBox3 = New System.Windows.Forms.GroupBox
            Me.GroupBox7 = New System.Windows.Forms.GroupBox
            Me.UploadMaxLinesControl = New System.Windows.Forms.NumericUpDown
            Me.MaxLinesContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.MaxLinesResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.SetToMaximumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.GroupBox6 = New System.Windows.Forms.GroupBox
            Me.UploadJobNameTextBox = New System.Windows.Forms.TextBox
            Me.JobNameContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.JobNameResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.GroupBox5 = New System.Windows.Forms.GroupBox
            Me.UploadLocationTextBox = New System.Windows.Forms.TextBox
            Me.LocationContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.LocationResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.UploadWatchlistCheckBox = New System.Windows.Forms.CheckBox
            Me.ApplyButton = New System.Windows.Forms.Button
            Me.Led1 = New AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components.LED
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox4.SuspendLayout()
            Me.GroupBox2.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.GroupBox7.SuspendLayout()
            CType(Me.UploadMaxLinesControl, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.MaxLinesContextMenuStrip.SuspendLayout()
            Me.GroupBox6.SuspendLayout()
            Me.JobNameContextMenuStrip.SuspendLayout()
            Me.GroupBox5.SuspendLayout()
            Me.LocationContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'VerboseCheckBox
            '
            Me.VerboseCheckBox.AutoSize = True
            Me.VerboseCheckBox.Location = New System.Drawing.Point(6, 88)
            Me.VerboseCheckBox.Name = "VerboseCheckBox"
            Me.VerboseCheckBox.Size = New System.Drawing.Size(102, 17)
            Me.VerboseCheckBox.TabIndex = 13
            Me.VerboseCheckBox.Text = "Verbose logging"
            Me.ToolTip2.SetToolTip(Me.VerboseCheckBox, "Log in detail (recommended whilst plugin is still in development)")
            Me.VerboseCheckBox.UseVisualStyleBackColor = True
            '
            'XHTMLLogCheckBox
            '
            Me.XHTMLLogCheckBox.AutoSize = True
            Me.XHTMLLogCheckBox.Location = New System.Drawing.Point(6, 42)
            Me.XHTMLLogCheckBox.Name = "XHTMLLogCheckBox"
            Me.XHTMLLogCheckBox.Size = New System.Drawing.Size(96, 17)
            Me.XHTMLLogCheckBox.TabIndex = 11
            Me.XHTMLLogCheckBox.Text = "Log to XHTML"
            Me.ToolTip2.SetToolTip(Me.XHTMLLogCheckBox, "Log the articles processed and what we did with them in XHTML format")
            Me.XHTMLLogCheckBox.UseVisualStyleBackColor = True
            '
            'WikiLogCheckBox
            '
            Me.WikiLogCheckBox.AutoSize = True
            Me.WikiLogCheckBox.Checked = True
            Me.WikiLogCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me.WikiLogCheckBox.Location = New System.Drawing.Point(6, 65)
            Me.WikiLogCheckBox.Name = "WikiLogCheckBox"
            Me.WikiLogCheckBox.Size = New System.Drawing.Size(104, 17)
            Me.WikiLogCheckBox.TabIndex = 12
            Me.WikiLogCheckBox.Text = "Log to wiki code"
            Me.ToolTip2.SetToolTip(Me.WikiLogCheckBox, "Log the articles processed and what we did with them in Mediawiki-markup format (" & _
                    "recommended)")
            Me.WikiLogCheckBox.UseVisualStyleBackColor = True
            '
            'LogBadTagsCheckBox
            '
            Me.LogBadTagsCheckBox.AutoSize = True
            Me.LogBadTagsCheckBox.Checked = True
            Me.LogBadTagsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me.LogBadTagsCheckBox.Location = New System.Drawing.Point(6, 19)
            Me.LogBadTagsCheckBox.Name = "LogBadTagsCheckBox"
            Me.LogBadTagsCheckBox.Size = New System.Drawing.Size(116, 17)
            Me.LogBadTagsCheckBox.TabIndex = 10
            Me.LogBadTagsCheckBox.Text = "Log problem pages"
            Me.ToolTip2.SetToolTip(Me.LogBadTagsCheckBox, "If the plugin can't parse a template on a page, it can log it for manual inspecti" & _
                    "on or sending to the plugin author. Recommendation is to leave this checked.")
            Me.LogBadTagsCheckBox.UseVisualStyleBackColor = True
            '
            'UploadCheckBox
            '
            Me.UploadCheckBox.AutoSize = True
            Me.UploadCheckBox.Location = New System.Drawing.Point(6, 111)
            Me.UploadCheckBox.Name = "UploadCheckBox"
            Me.UploadCheckBox.Size = New System.Drawing.Size(60, 17)
            Me.UploadCheckBox.TabIndex = 18
            Me.UploadCheckBox.Text = "Upload"
            Me.ToolTip2.SetToolTip(Me.UploadCheckBox, "Upload logs to Wikipedia?")
            Me.UploadCheckBox.UseVisualStyleBackColor = True
            '
            'UploadWikiProjectCheckBox
            '
            Me.UploadWikiProjectCheckBox.AutoSize = True
            Me.UploadWikiProjectCheckBox.Checked = True
            Me.UploadWikiProjectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me.UploadWikiProjectCheckBox.Enabled = False
            Me.UploadWikiProjectCheckBox.Location = New System.Drawing.Point(11, 19)
            Me.UploadWikiProjectCheckBox.Name = "UploadWikiProjectCheckBox"
            Me.UploadWikiProjectCheckBox.Size = New System.Drawing.Size(98, 17)
            Me.UploadWikiProjectCheckBox.TabIndex = 19
            Me.UploadWikiProjectCheckBox.Text = "Upload to WPs"
            Me.ToolTip2.SetToolTip(Me.UploadWikiProjectCheckBox, "Upload logs to the automation department of your WikiProject, if the template plu" & _
                    "gin supports it")
            Me.UploadWikiProjectCheckBox.UseVisualStyleBackColor = True
            '
            'UploadOpenInBrowserCheckBox
            '
            Me.UploadOpenInBrowserCheckBox.AutoSize = True
            Me.UploadOpenInBrowserCheckBox.Checked = True
            Me.UploadOpenInBrowserCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me.UploadOpenInBrowserCheckBox.Enabled = False
            Me.UploadOpenInBrowserCheckBox.Location = New System.Drawing.Point(11, 65)
            Me.UploadOpenInBrowserCheckBox.Name = "UploadOpenInBrowserCheckBox"
            Me.UploadOpenInBrowserCheckBox.Size = New System.Drawing.Size(103, 17)
            Me.UploadOpenInBrowserCheckBox.TabIndex = 21
            Me.UploadOpenInBrowserCheckBox.Text = "Open in browser"
            Me.ToolTip2.SetToolTip(Me.UploadOpenInBrowserCheckBox, "Open uploaded pages in your default web browser." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "A good way to add the logs to y" & _
                    "our own watchlist if you're in bot mode!")
            Me.UploadOpenInBrowserCheckBox.UseVisualStyleBackColor = True
            '
            'CloseAllButton
            '
            Me.CloseAllButton.Location = New System.Drawing.Point(76, 219)
            Me.CloseAllButton.Name = "CloseAllButton"
            Me.CloseAllButton.Size = New System.Drawing.Size(56, 23)
            Me.CloseAllButton.TabIndex = 21
            Me.CloseAllButton.Text = "Close"
            Me.ToolTip2.SetToolTip(Me.CloseAllButton, "Close all logs")
            Me.CloseAllButton.UseVisualStyleBackColor = True
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(16, 108)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(36, 13)
            Me.Label1.TabIndex = 16
            Me.Label1.Text = "Folder"
            '
            'FolderTextBox
            '
            Me.FolderTextBox.Location = New System.Drawing.Point(6, 19)
            Me.FolderTextBox.Name = "FolderTextBox"
            Me.FolderTextBox.Size = New System.Drawing.Size(100, 20)
            Me.FolderTextBox.TabIndex = 15
            '
            'FolderButton
            '
            Me.FolderButton.Location = New System.Drawing.Point(6, 45)
            Me.FolderButton.Name = "FolderButton"
            Me.FolderButton.Size = New System.Drawing.Size(75, 20)
            Me.FolderButton.TabIndex = 14
            Me.FolderButton.Text = "Select"
            Me.FolderButton.UseVisualStyleBackColor = True
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.GroupBox4)
            Me.GroupBox1.Controls.Add(Me.UploadCheckBox)
            Me.GroupBox1.Controls.Add(Me.WikiLogCheckBox)
            Me.GroupBox1.Controls.Add(Me.LogBadTagsCheckBox)
            Me.GroupBox1.Controls.Add(Me.XHTMLLogCheckBox)
            Me.GroupBox1.Controls.Add(Me.VerboseCheckBox)
            Me.GroupBox1.Location = New System.Drawing.Point(4, 4)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(131, 212)
            Me.GroupBox1.TabIndex = 17
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Options"
            '
            'GroupBox4
            '
            Me.GroupBox4.Controls.Add(Me.FolderTextBox)
            Me.GroupBox4.Controls.Add(Me.FolderButton)
            Me.GroupBox4.Location = New System.Drawing.Point(6, 134)
            Me.GroupBox4.Name = "GroupBox4"
            Me.GroupBox4.Size = New System.Drawing.Size(114, 73)
            Me.GroupBox4.TabIndex = 20
            Me.GroupBox4.TabStop = False
            Me.GroupBox4.Text = "Folder"
            '
            'GroupBox2
            '
            Me.GroupBox2.Controls.Add(Me.Led1)
            Me.GroupBox2.Controls.Add(Me.Label8)
            Me.GroupBox2.Controls.Add(Me.UploadsCountLabel)
            Me.GroupBox2.Controls.Add(Me.WikiLinesLabel)
            Me.GroupBox2.Controls.Add(Me.XHTMLLinesLabel)
            Me.GroupBox2.Controls.Add(Me.BadTagsLinesLabel)
            Me.GroupBox2.Controls.Add(Me.WikiLinesSinceUploadLabel)
            Me.GroupBox2.Controls.Add(Me.XHTMLLinesSinceUploadLabel)
            Me.GroupBox2.Controls.Add(Me.BadTagsLinesSinceUploadLabel)
            Me.GroupBox2.Controls.Add(Me.Label2)
            Me.GroupBox2.Controls.Add(Me.Label3)
            Me.GroupBox2.Controls.Add(Me.Label4)
            Me.GroupBox2.Controls.Add(Me.Label5)
            Me.GroupBox2.Controls.Add(Me.Label6)
            Me.GroupBox2.Controls.Add(Me.Label7)
            Me.GroupBox2.Location = New System.Drawing.Point(4, 244)
            Me.GroupBox2.Name = "GroupBox2"
            Me.GroupBox2.Size = New System.Drawing.Size(261, 95)
            Me.GroupBox2.TabIndex = 18
            Me.GroupBox2.TabStop = False
            Me.GroupBox2.Text = "Status"
            '
            'Label8
            '
            Me.Label8.AutoSize = True
            Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label8.Location = New System.Drawing.Point(198, 16)
            Me.Label8.Name = "Label8"
            Me.Label8.Size = New System.Drawing.Size(53, 13)
            Me.Label8.TabIndex = 22
            Me.Label8.Text = "Uploads"
            '
            'UploadsCountLabel
            '
            Me.UploadsCountLabel.AutoSize = True
            Me.UploadsCountLabel.Location = New System.Drawing.Point(198, 36)
            Me.UploadsCountLabel.Name = "UploadsCountLabel"
            Me.UploadsCountLabel.Size = New System.Drawing.Size(27, 13)
            Me.UploadsCountLabel.TabIndex = 23
            Me.UploadsCountLabel.Text = "N/A"
            '
            'WikiLinesLabel
            '
            Me.WikiLinesLabel.AutoSize = True
            Me.WikiLinesLabel.Location = New System.Drawing.Point(56, 36)
            Me.WikiLinesLabel.Name = "WikiLinesLabel"
            Me.WikiLinesLabel.Size = New System.Drawing.Size(27, 13)
            Me.WikiLinesLabel.TabIndex = 25
            Me.WikiLinesLabel.Text = "N/A"
            '
            'XHTMLLinesLabel
            '
            Me.XHTMLLinesLabel.AutoSize = True
            Me.XHTMLLinesLabel.Location = New System.Drawing.Point(56, 56)
            Me.XHTMLLinesLabel.Name = "XHTMLLinesLabel"
            Me.XHTMLLinesLabel.Size = New System.Drawing.Size(27, 13)
            Me.XHTMLLinesLabel.TabIndex = 26
            Me.XHTMLLinesLabel.Text = "N/A"
            '
            'BadTagsLinesLabel
            '
            Me.BadTagsLinesLabel.AutoSize = True
            Me.BadTagsLinesLabel.Location = New System.Drawing.Point(56, 77)
            Me.BadTagsLinesLabel.Name = "BadTagsLinesLabel"
            Me.BadTagsLinesLabel.Size = New System.Drawing.Size(27, 13)
            Me.BadTagsLinesLabel.TabIndex = 27
            Me.BadTagsLinesLabel.Text = "N/A"
            '
            'WikiLinesSinceUploadLabel
            '
            Me.WikiLinesSinceUploadLabel.AutoSize = True
            Me.WikiLinesSinceUploadLabel.Location = New System.Drawing.Point(123, 36)
            Me.WikiLinesSinceUploadLabel.Name = "WikiLinesSinceUploadLabel"
            Me.WikiLinesSinceUploadLabel.Size = New System.Drawing.Size(27, 13)
            Me.WikiLinesSinceUploadLabel.TabIndex = 28
            Me.WikiLinesSinceUploadLabel.Text = "N/A"
            '
            'XHTMLLinesSinceUploadLabel
            '
            Me.XHTMLLinesSinceUploadLabel.AutoSize = True
            Me.XHTMLLinesSinceUploadLabel.Location = New System.Drawing.Point(123, 56)
            Me.XHTMLLinesSinceUploadLabel.Name = "XHTMLLinesSinceUploadLabel"
            Me.XHTMLLinesSinceUploadLabel.Size = New System.Drawing.Size(27, 13)
            Me.XHTMLLinesSinceUploadLabel.TabIndex = 29
            Me.XHTMLLinesSinceUploadLabel.Text = "N/A"
            '
            'BadTagsLinesSinceUploadLabel
            '
            Me.BadTagsLinesSinceUploadLabel.AutoSize = True
            Me.BadTagsLinesSinceUploadLabel.Location = New System.Drawing.Point(123, 76)
            Me.BadTagsLinesSinceUploadLabel.Name = "BadTagsLinesSinceUploadLabel"
            Me.BadTagsLinesSinceUploadLabel.Size = New System.Drawing.Size(27, 13)
            Me.BadTagsLinesSinceUploadLabel.TabIndex = 30
            Me.BadTagsLinesSinceUploadLabel.Text = "N/A"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label2.Location = New System.Drawing.Point(9, 16)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(28, 13)
            Me.Label2.TabIndex = 22
            Me.Label2.Text = "Log"
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label3.Location = New System.Drawing.Point(9, 36)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(32, 13)
            Me.Label3.TabIndex = 23
            Me.Label3.Text = "Wiki"
            '
            'Label4
            '
            Me.Label4.AutoSize = True
            Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label4.Location = New System.Drawing.Point(9, 56)
            Me.Label4.Name = "Label4"
            Me.Label4.Size = New System.Drawing.Size(40, 13)
            Me.Label4.TabIndex = 24
            Me.Label4.Text = "XMTL"
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label5.Location = New System.Drawing.Point(9, 76)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(29, 13)
            Me.Label5.TabIndex = 25
            Me.Label5.Text = "Bad"
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label6.Location = New System.Drawing.Point(56, 16)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(37, 13)
            Me.Label6.TabIndex = 26
            Me.Label6.Text = "Lines"
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label7.Location = New System.Drawing.Point(103, 16)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(65, 13)
            Me.Label7.TabIndex = 27
            Me.Label7.Text = "Since U/L"
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.GroupBox7)
            Me.GroupBox3.Controls.Add(Me.GroupBox6)
            Me.GroupBox3.Controls.Add(Me.GroupBox5)
            Me.GroupBox3.Controls.Add(Me.UploadWikiProjectCheckBox)
            Me.GroupBox3.Controls.Add(Me.UploadWatchlistCheckBox)
            Me.GroupBox3.Controls.Add(Me.UploadOpenInBrowserCheckBox)
            Me.GroupBox3.Location = New System.Drawing.Point(138, 4)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(127, 244)
            Me.GroupBox3.TabIndex = 19
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "Uploading"
            '
            'GroupBox7
            '
            Me.GroupBox7.Controls.Add(Me.UploadMaxLinesControl)
            Me.GroupBox7.Location = New System.Drawing.Point(6, 195)
            Me.GroupBox7.Name = "GroupBox7"
            Me.GroupBox7.Size = New System.Drawing.Size(114, 43)
            Me.GroupBox7.TabIndex = 23
            Me.GroupBox7.TabStop = False
            Me.GroupBox7.Text = "Maximum Lines"
            '
            'UploadMaxLinesControl
            '
            Me.UploadMaxLinesControl.ContextMenuStrip = Me.MaxLinesContextMenuStrip
            Me.UploadMaxLinesControl.Enabled = False
            Me.UploadMaxLinesControl.Location = New System.Drawing.Point(8, 17)
            Me.UploadMaxLinesControl.Maximum = New Decimal(New Integer() {3000, 0, 0, 0})
            Me.UploadMaxLinesControl.Minimum = New Decimal(New Integer() {500, 0, 0, 0})
            Me.UploadMaxLinesControl.Name = "UploadMaxLinesControl"
            Me.UploadMaxLinesControl.Size = New System.Drawing.Size(103, 20)
            Me.UploadMaxLinesControl.TabIndex = 0
            Me.UploadMaxLinesControl.Value = New Decimal(New Integer() {1000, 0, 0, 0})
            '
            'MaxLinesContextMenuStrip
            '
            Me.MaxLinesContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MaxLinesResetToolStripMenuItem, Me.SetToMaximumToolStripMenuItem})
            Me.MaxLinesContextMenuStrip.Name = "MaxLinesContextMenuStrip"
            Me.MaxLinesContextMenuStrip.Size = New System.Drawing.Size(151, 48)
            '
            'MaxLinesResetToolStripMenuItem
            '
            Me.MaxLinesResetToolStripMenuItem.Name = "MaxLinesResetToolStripMenuItem"
            Me.MaxLinesResetToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
            Me.MaxLinesResetToolStripMenuItem.Text = "Reset"
            '
            'SetToMaximumToolStripMenuItem
            '
            Me.SetToMaximumToolStripMenuItem.Name = "SetToMaximumToolStripMenuItem"
            Me.SetToMaximumToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
            Me.SetToMaximumToolStripMenuItem.Text = "Set to maximum"
            '
            'GroupBox6
            '
            Me.GroupBox6.Controls.Add(Me.UploadJobNameTextBox)
            Me.GroupBox6.Location = New System.Drawing.Point(6, 143)
            Me.GroupBox6.Name = "GroupBox6"
            Me.GroupBox6.Size = New System.Drawing.Size(115, 45)
            Me.GroupBox6.TabIndex = 0
            Me.GroupBox6.TabStop = False
            Me.GroupBox6.Text = "Current Job Name"
            '
            'UploadJobNameTextBox
            '
            Me.UploadJobNameTextBox.ContextMenuStrip = Me.JobNameContextMenuStrip
            Me.UploadJobNameTextBox.Enabled = False
            Me.UploadJobNameTextBox.Location = New System.Drawing.Point(8, 19)
            Me.UploadJobNameTextBox.Name = "UploadJobNameTextBox"
            Me.UploadJobNameTextBox.Size = New System.Drawing.Size(103, 20)
            Me.UploadJobNameTextBox.TabIndex = 0
            Me.UploadJobNameTextBox.Text = "$CATEGORY"
            '
            'JobNameContextMenuStrip
            '
            Me.JobNameContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.JobNameResetToolStripMenuItem})
            Me.JobNameContextMenuStrip.Name = "JobNameContextMenuStrip"
            Me.JobNameContextMenuStrip.Size = New System.Drawing.Size(103, 26)
            '
            'JobNameResetToolStripMenuItem
            '
            Me.JobNameResetToolStripMenuItem.Name = "JobNameResetToolStripMenuItem"
            Me.JobNameResetToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
            Me.JobNameResetToolStripMenuItem.Text = "Reset"
            '
            'GroupBox5
            '
            Me.GroupBox5.Controls.Add(Me.UploadLocationTextBox)
            Me.GroupBox5.Location = New System.Drawing.Point(6, 88)
            Me.GroupBox5.Name = "GroupBox5"
            Me.GroupBox5.Size = New System.Drawing.Size(115, 49)
            Me.GroupBox5.TabIndex = 22
            Me.GroupBox5.TabStop = False
            Me.GroupBox5.Text = "Upload Location"
            '
            'UploadLocationTextBox
            '
            Me.UploadLocationTextBox.ContextMenuStrip = Me.LocationContextMenuStrip
            Me.UploadLocationTextBox.Enabled = False
            Me.UploadLocationTextBox.Location = New System.Drawing.Point(8, 19)
            Me.UploadLocationTextBox.Name = "UploadLocationTextBox"
            Me.UploadLocationTextBox.Size = New System.Drawing.Size(103, 20)
            Me.UploadLocationTextBox.TabIndex = 21
            Me.UploadLocationTextBox.Text = "$USER/Logs"
            '
            'LocationContextMenuStrip
            '
            Me.LocationContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LocationResetToolStripMenuItem})
            Me.LocationContextMenuStrip.Name = "LocationContextMenuStrip"
            Me.LocationContextMenuStrip.Size = New System.Drawing.Size(103, 26)
            '
            'LocationResetToolStripMenuItem
            '
            Me.LocationResetToolStripMenuItem.Name = "LocationResetToolStripMenuItem"
            Me.LocationResetToolStripMenuItem.Size = New System.Drawing.Size(102, 22)
            Me.LocationResetToolStripMenuItem.Text = "Reset"
            '
            'UploadWatchlistCheckBox
            '
            Me.UploadWatchlistCheckBox.AutoSize = True
            Me.UploadWatchlistCheckBox.Checked = True
            Me.UploadWatchlistCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
            Me.UploadWatchlistCheckBox.Enabled = False
            Me.UploadWatchlistCheckBox.Location = New System.Drawing.Point(11, 42)
            Me.UploadWatchlistCheckBox.Name = "UploadWatchlistCheckBox"
            Me.UploadWatchlistCheckBox.Size = New System.Drawing.Size(101, 17)
            Me.UploadWatchlistCheckBox.TabIndex = 20
            Me.UploadWatchlistCheckBox.Text = "Add to watchlist"
            Me.UploadWatchlistCheckBox.UseVisualStyleBackColor = True
            '
            'ApplyButton
            '
            Me.ApplyButton.Enabled = False
            Me.ApplyButton.Location = New System.Drawing.Point(10, 219)
            Me.ApplyButton.Name = "ApplyButton"
            Me.ApplyButton.Size = New System.Drawing.Size(60, 23)
            Me.ApplyButton.TabIndex = 20
            Me.ApplyButton.Text = "Apply"
            Me.ApplyButton.UseVisualStyleBackColor = True
            '
            'Led1
            '
            Me.Led1.Colour = AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components.Colour.Red
            Me.Led1.Location = New System.Drawing.Point(231, 69)
            Me.Led1.Name = "Led1"
            Me.Led1.Size = New System.Drawing.Size(20, 20)
            Me.Led1.TabIndex = 44
            Me.ToolTip2.SetToolTip(Me.Led1, "Green when logging, blue when uploading")
            '
            'PluginLogging
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.CloseAllButton)
            Me.Controls.Add(Me.ApplyButton)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox2)
            Me.Controls.Add(Me.GroupBox1)
            Me.Controls.Add(Me.Label1)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "PluginLogging"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox4.ResumeLayout(False)
            Me.GroupBox4.PerformLayout()
            Me.GroupBox2.ResumeLayout(False)
            Me.GroupBox2.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            Me.GroupBox7.ResumeLayout(False)
            CType(Me.UploadMaxLinesControl, System.ComponentModel.ISupportInitialize).EndInit()
            Me.MaxLinesContextMenuStrip.ResumeLayout(False)
            Me.GroupBox6.ResumeLayout(False)
            Me.GroupBox6.PerformLayout()
            Me.JobNameContextMenuStrip.ResumeLayout(False)
            Me.GroupBox5.ResumeLayout(False)
            Me.GroupBox5.PerformLayout()
            Me.LocationContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents XHTMLLinesSinceUploadLabel As System.Windows.Forms.Label
        Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
        Friend WithEvents ToolTip2 As System.Windows.Forms.ToolTip
        Friend WithEvents VerboseCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents XHTMLLogCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents WikiLogCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents LogBadTagsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents FolderTextBox As System.Windows.Forms.TextBox
        Friend WithEvents FolderButton As System.Windows.Forms.Button
        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents UploadCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
        Friend WithEvents UploadWikiProjectCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents UploadWatchlistCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents UploadOpenInBrowserCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
        Friend WithEvents ApplyButton As System.Windows.Forms.Button
        Friend WithEvents UploadLocationTextBox As System.Windows.Forms.TextBox
        Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
        Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
        Friend WithEvents UploadJobNameTextBox As System.Windows.Forms.TextBox
        Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
        Friend WithEvents UploadMaxLinesControl As System.Windows.Forms.NumericUpDown
        Friend WithEvents CloseAllButton As System.Windows.Forms.Button
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label8 As System.Windows.Forms.Label
        Friend WithEvents UploadsCountLabel As System.Windows.Forms.Label
        Friend WithEvents WikiLinesLabel As System.Windows.Forms.Label
        Friend WithEvents XHTMLLinesLabel As System.Windows.Forms.Label
        Friend WithEvents BadTagsLinesLabel As System.Windows.Forms.Label
        Friend WithEvents WikiLinesSinceUploadLabel As System.Windows.Forms.Label
        Friend WithEvents BadTagsLinesSinceUploadLabel As System.Windows.Forms.Label
        Friend WithEvents Led1 As AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components.LED
        Friend WithEvents LocationContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents JobNameContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents MaxLinesContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents MaxLinesResetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents JobNameResetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents LocationResetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents SetToMaximumToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace