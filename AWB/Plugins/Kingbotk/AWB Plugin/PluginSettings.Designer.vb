<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PluginSettings
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
        Me.btnStop = New System.Windows.Forms.Button
        Me.btnStart = New System.Windows.Forms.Button
        Me.btnPreview = New System.Windows.Forms.Button
        Me.btnSave = New System.Windows.Forms.Button
        Me.btnDiff = New System.Windows.Forms.Button
        Me.btnIgnore = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.lblTagged = New System.Windows.Forms.Label
        Me.lblSkipped = New System.Windows.Forms.Label
        Me.lblNoChange = New System.Windows.Forms.Label
        Me.lblBadTag = New System.Windows.Forms.Label
        Me.lblNamespace = New System.Windows.Forms.Label
        Me.lblNew = New System.Windows.Forms.Label
        Me.groupBox3 = New System.Windows.Forms.GroupBox
        Me.lblWords = New System.Windows.Forms.Label
        Me.lblInterLinks = New System.Windows.Forms.Label
        Me.lblCats = New System.Windows.Forms.Label
        Me.lblImages = New System.Windows.Forms.Label
        Me.lblLinks = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.groupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnStop
        '
        Me.btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnStop.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStop.Location = New System.Drawing.Point(6, 51)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(102, 23)
        Me.btnStop.TabIndex = 34
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnStart
        '
        Me.btnStart.Enabled = False
        Me.btnStart.Location = New System.Drawing.Point(6, 22)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(102, 23)
        Me.btnStart.TabIndex = 29
        Me.btnStart.Tag = "Start the process"
        Me.btnStart.Text = "Start the process (Shortcut ctrl + s)"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnPreview
        '
        Me.btnPreview.Enabled = False
        Me.btnPreview.Location = New System.Drawing.Point(6, 87)
        Me.btnPreview.Name = "btnPreview"
        Me.btnPreview.Size = New System.Drawing.Size(102, 23)
        Me.btnPreview.TabIndex = 32
        Me.btnPreview.Text = "Preview"
        Me.btnPreview.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Enabled = False
        Me.btnSave.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSave.Location = New System.Drawing.Point(6, 186)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(102, 32)
        Me.btnSave.TabIndex = 30
        Me.btnSave.Tag = "Apply all the changes"
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnDiff
        '
        Me.btnDiff.Enabled = False
        Me.btnDiff.Location = New System.Drawing.Point(6, 114)
        Me.btnDiff.Name = "btnDiff"
        Me.btnDiff.Size = New System.Drawing.Size(102, 23)
        Me.btnDiff.TabIndex = 33
        Me.btnDiff.Text = "Show changes"
        Me.btnDiff.UseVisualStyleBackColor = True
        '
        'btnIgnore
        '
        Me.btnIgnore.Enabled = False
        Me.btnIgnore.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnIgnore.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnIgnore.Location = New System.Drawing.Point(6, 151)
        Me.btnIgnore.Name = "btnIgnore"
        Me.btnIgnore.Size = New System.Drawing.Size(102, 32)
        Me.btnIgnore.TabIndex = 31
        Me.btnIgnore.Text = "Ignore"
        Me.btnIgnore.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnIgnore)
        Me.GroupBox1.Controls.Add(Me.btnStop)
        Me.GroupBox1.Controls.Add(Me.btnDiff)
        Me.GroupBox1.Controls.Add(Me.btnStart)
        Me.GroupBox1.Controls.Add(Me.btnSave)
        Me.GroupBox1.Controls.Add(Me.btnPreview)
        Me.GroupBox1.Location = New System.Drawing.Point(141, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(122, 235)
        Me.GroupBox1.TabIndex = 35
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "AWB"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.groupBox3)
        Me.GroupBox2.Controls.Add(Me.lblTagged)
        Me.GroupBox2.Controls.Add(Me.lblSkipped)
        Me.GroupBox2.Controls.Add(Me.lblNoChange)
        Me.GroupBox2.Controls.Add(Me.lblBadTag)
        Me.GroupBox2.Controls.Add(Me.lblNamespace)
        Me.GroupBox2.Controls.Add(Me.lblNew)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(135, 235)
        Me.GroupBox2.TabIndex = 36
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Statistics"
        Me.ToolTip1.SetToolTip(Me.GroupBox2, "Lies, damned lies and statistics")
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Tagged"
        Me.ToolTip1.SetToolTip(Me.Label1, "Number of articles tagged")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(3, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Skipped:"
        Me.ToolTip1.SetToolTip(Me.Label2, "Number of articles skipped")
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 54)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(61, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "No Change"
        Me.ToolTip1.SetToolTip(Me.Label3, "Number of articles skipped because no change was made")
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(3, 73)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Bad Tag"
        Me.ToolTip1.SetToolTip(Me.Label4, "Number of articles skipped because they had an unparseable template")
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 92)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Namespace"
        Me.ToolTip1.SetToolTip(Me.Label5, "Number of articles skipped because they were in an incorrect namespace (e.g. we w" & _
                "on't tag articles with talk page templates)")
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(3, 111)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(32, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "New"
        Me.ToolTip1.SetToolTip(Me.Label6, "Number of redlink pages turned blue")
        '
        'lblTagged
        '
        Me.lblTagged.AutoSize = True
        Me.lblTagged.Location = New System.Drawing.Point(73, 16)
        Me.lblTagged.Name = "lblTagged"
        Me.lblTagged.Size = New System.Drawing.Size(0, 13)
        Me.lblTagged.TabIndex = 37
        Me.ToolTip1.SetToolTip(Me.lblTagged, "Number of articles tagged")
        '
        'lblSkipped
        '
        Me.lblSkipped.AutoSize = True
        Me.lblSkipped.Location = New System.Drawing.Point(73, 35)
        Me.lblSkipped.Name = "lblSkipped"
        Me.lblSkipped.Size = New System.Drawing.Size(0, 13)
        Me.lblSkipped.TabIndex = 38
        Me.ToolTip1.SetToolTip(Me.lblSkipped, "Number of articles skipped")
        '
        'lblNoChange
        '
        Me.lblNoChange.AutoSize = True
        Me.lblNoChange.Location = New System.Drawing.Point(73, 54)
        Me.lblNoChange.Name = "lblNoChange"
        Me.lblNoChange.Size = New System.Drawing.Size(0, 13)
        Me.lblNoChange.TabIndex = 39
        Me.ToolTip1.SetToolTip(Me.lblNoChange, "Number of articles skipped because no change was made")
        '
        'lblBadTag
        '
        Me.lblBadTag.AutoSize = True
        Me.lblBadTag.Location = New System.Drawing.Point(73, 73)
        Me.lblBadTag.Name = "lblBadTag"
        Me.lblBadTag.Size = New System.Drawing.Size(0, 13)
        Me.lblBadTag.TabIndex = 40
        Me.ToolTip1.SetToolTip(Me.lblBadTag, "Number of articles skipped because they had an unparseable template")
        '
        'lblNamespace
        '
        Me.lblNamespace.AutoSize = True
        Me.lblNamespace.Location = New System.Drawing.Point(73, 92)
        Me.lblNamespace.Name = "lblNamespace"
        Me.lblNamespace.Size = New System.Drawing.Size(0, 13)
        Me.lblNamespace.TabIndex = 41
        Me.ToolTip1.SetToolTip(Me.lblNamespace, "Number of articles skipped because they were in an incorrect namespace (e.g. we w" & _
                "on't tag articles with talk page templates)")
        '
        'lblNew
        '
        Me.lblNew.AutoSize = True
        Me.lblNew.Location = New System.Drawing.Point(73, 111)
        Me.lblNew.Name = "lblNew"
        Me.lblNew.Size = New System.Drawing.Size(0, 13)
        Me.lblNew.TabIndex = 42
        Me.ToolTip1.SetToolTip(Me.lblNew, "Number of redlink pages turned blue")
        '
        'groupBox3
        '
        Me.groupBox3.Controls.Add(Me.lblWords)
        Me.groupBox3.Controls.Add(Me.lblInterLinks)
        Me.groupBox3.Controls.Add(Me.lblCats)
        Me.groupBox3.Controls.Add(Me.lblImages)
        Me.groupBox3.Controls.Add(Me.lblLinks)
        Me.groupBox3.Location = New System.Drawing.Point(6, 128)
        Me.groupBox3.Name = "groupBox3"
        Me.groupBox3.Size = New System.Drawing.Size(123, 101)
        Me.groupBox3.TabIndex = 43
        Me.groupBox3.TabStop = False
        Me.groupBox3.Text = "Article statistics"
        '
        'lblWords
        '
        Me.lblWords.AutoSize = True
        Me.lblWords.Location = New System.Drawing.Point(6, 18)
        Me.lblWords.Name = "lblWords"
        Me.lblWords.Size = New System.Drawing.Size(41, 13)
        Me.lblWords.TabIndex = 17
        Me.lblWords.Text = "Words:"
        '
        'lblInterLinks
        '
        Me.lblInterLinks.AutoSize = True
        Me.lblInterLinks.Location = New System.Drawing.Point(5, 86)
        Me.lblInterLinks.Name = "lblInterLinks"
        Me.lblInterLinks.Size = New System.Drawing.Size(55, 13)
        Me.lblInterLinks.TabIndex = 22
        Me.lblInterLinks.Text = "Inter links:"
        '
        'lblCats
        '
        Me.lblCats.AutoSize = True
        Me.lblCats.Location = New System.Drawing.Point(5, 69)
        Me.lblCats.Name = "lblCats"
        Me.lblCats.Size = New System.Drawing.Size(60, 13)
        Me.lblCats.TabIndex = 18
        Me.lblCats.Text = "Categories:"
        '
        'lblImages
        '
        Me.lblImages.AutoSize = True
        Me.lblImages.Location = New System.Drawing.Point(6, 52)
        Me.lblImages.Name = "lblImages"
        Me.lblImages.Size = New System.Drawing.Size(44, 13)
        Me.lblImages.TabIndex = 19
        Me.lblImages.Text = "Images:"
        '
        'lblLinks
        '
        Me.lblLinks.AutoSize = True
        Me.lblLinks.Location = New System.Drawing.Point(6, 35)
        Me.lblLinks.Name = "lblLinks"
        Me.lblLinks.Size = New System.Drawing.Size(35, 13)
        Me.lblLinks.TabIndex = 20
        Me.lblLinks.Text = "Links:"
        '
        'PluginSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "PluginSettings"
        Me.Size = New System.Drawing.Size(276, 349)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.groupBox3.ResumeLayout(False)
        Me.groupBox3.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnPreview As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnDiff As System.Windows.Forms.Button
    Friend WithEvents btnIgnore As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblSkipped As System.Windows.Forms.Label
    Friend WithEvents lblNoChange As System.Windows.Forms.Label
    Friend WithEvents lblBadTag As System.Windows.Forms.Label
    Friend WithEvents lblNamespace As System.Windows.Forms.Label
    Friend WithEvents lblNew As System.Windows.Forms.Label
    Friend WithEvents lblTagged As System.Windows.Forms.Label
    Private WithEvents groupBox3 As System.Windows.Forms.GroupBox
    Private WithEvents lblWords As System.Windows.Forms.Label
    Private WithEvents lblInterLinks As System.Windows.Forms.Label
    Private WithEvents lblCats As System.Windows.Forms.Label
    Private WithEvents lblImages As System.Windows.Forms.Label
    Private WithEvents lblLinks As System.Windows.Forms.Label

End Class
