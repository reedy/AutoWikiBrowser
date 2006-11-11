<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Logging
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.LogBadTagsCheckBox = New System.Windows.Forms.CheckBox
        Me.XHTMLLogCheckBox = New System.Windows.Forms.CheckBox
        Me.WikiLogCheckBox = New System.Windows.Forms.CheckBox
        Me.VerboseCheckBox = New System.Windows.Forms.CheckBox
        Me.FolderButton = New System.Windows.Forms.Button
        Me.FolderTextBox = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(53, 166)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'LogBadTagsCheckBox
        '
        Me.LogBadTagsCheckBox.AutoSize = True
        Me.LogBadTagsCheckBox.Checked = True
        Me.LogBadTagsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.LogBadTagsCheckBox.Location = New System.Drawing.Point(12, 12)
        Me.LogBadTagsCheckBox.Name = "LogBadTagsCheckBox"
        Me.LogBadTagsCheckBox.Size = New System.Drawing.Size(133, 17)
        Me.LogBadTagsCheckBox.TabIndex = 2
        Me.LogBadTagsCheckBox.Text = "Log problematic pages"
        Me.ToolTip1.SetToolTip(Me.LogBadTagsCheckBox, "If the plugin can't parse a template on a page, it can log it for manual inspecti" & _
                "on or sending to the plugin author. Recommendation is to leave this checked.")
        Me.LogBadTagsCheckBox.UseVisualStyleBackColor = True
        '
        'XHTMLLogCheckBox
        '
        Me.XHTMLLogCheckBox.AutoSize = True
        Me.XHTMLLogCheckBox.Location = New System.Drawing.Point(12, 35)
        Me.XHTMLLogCheckBox.Name = "XHTMLLogCheckBox"
        Me.XHTMLLogCheckBox.Size = New System.Drawing.Size(96, 17)
        Me.XHTMLLogCheckBox.TabIndex = 3
        Me.XHTMLLogCheckBox.Text = "Log to XHTML"
        Me.ToolTip1.SetToolTip(Me.XHTMLLogCheckBox, "Log the articles processed and what we did with them in XHTML format")
        Me.XHTMLLogCheckBox.UseVisualStyleBackColor = True
        '
        'WikiLogCheckBox
        '
        Me.WikiLogCheckBox.AutoSize = True
        Me.WikiLogCheckBox.Checked = True
        Me.WikiLogCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.WikiLogCheckBox.Location = New System.Drawing.Point(12, 58)
        Me.WikiLogCheckBox.Name = "WikiLogCheckBox"
        Me.WikiLogCheckBox.Size = New System.Drawing.Size(104, 17)
        Me.WikiLogCheckBox.TabIndex = 4
        Me.WikiLogCheckBox.Text = "Log to wiki code"
        Me.ToolTip1.SetToolTip(Me.WikiLogCheckBox, "Log the articles processed and what we did with them in Mediawiki-markup format (" & _
                "recommended)")
        Me.WikiLogCheckBox.UseVisualStyleBackColor = True
        '
        'VerboseCheckBox
        '
        Me.VerboseCheckBox.AutoSize = True
        Me.VerboseCheckBox.Checked = True
        Me.VerboseCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.VerboseCheckBox.Location = New System.Drawing.Point(12, 81)
        Me.VerboseCheckBox.Name = "VerboseCheckBox"
        Me.VerboseCheckBox.Size = New System.Drawing.Size(102, 17)
        Me.VerboseCheckBox.TabIndex = 5
        Me.VerboseCheckBox.Text = "Verbose logging"
        Me.ToolTip1.SetToolTip(Me.VerboseCheckBox, "Log in detail (recommended whilst plugin is still in development)")
        Me.VerboseCheckBox.UseVisualStyleBackColor = True
        '
        'FolderButton
        '
        Me.FolderButton.Location = New System.Drawing.Point(122, 124)
        Me.FolderButton.Name = "FolderButton"
        Me.FolderButton.Size = New System.Drawing.Size(75, 20)
        Me.FolderButton.TabIndex = 6
        Me.FolderButton.Text = "Select"
        Me.FolderButton.UseVisualStyleBackColor = True
        '
        'FolderTextBox
        '
        Me.FolderTextBox.Location = New System.Drawing.Point(16, 124)
        Me.FolderTextBox.Name = "FolderTextBox"
        Me.FolderTextBox.Size = New System.Drawing.Size(100, 20)
        Me.FolderTextBox.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(13, 108)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Folder"
        '
        'Logging
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(211, 207)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.FolderTextBox)
        Me.Controls.Add(Me.FolderButton)
        Me.Controls.Add(Me.VerboseCheckBox)
        Me.Controls.Add(Me.WikiLogCheckBox)
        Me.Controls.Add(Me.XHTMLLogCheckBox)
        Me.Controls.Add(Me.LogBadTagsCheckBox)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Logging"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Logging"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents LogBadTagsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents XHTMLLogCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents WikiLogCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents VerboseCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents FolderButton As System.Windows.Forms.Button
    Friend WithEvents FolderTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label

End Class
