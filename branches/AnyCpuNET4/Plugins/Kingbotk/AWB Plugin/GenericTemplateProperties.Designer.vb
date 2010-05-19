Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GenericTemplatePropertiesForm
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
            Me.OK_Button = New System.Windows.Forms.Button
            Me.NameLabel = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.MainRegexTextBox = New System.Windows.Forms.TextBox
            Me.AmIReadyLabel = New System.Windows.Forms.Label
            Me.HasAltNamesLabel = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.PreferredTemplateNameRegexTextBox = New System.Windows.Forms.TextBox
            Me.Label3 = New System.Windows.Forms.Label
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.SecondChanceRegexTextBox = New System.Windows.Forms.TextBox
            Me.SkipRegexTextBox = New System.Windows.Forms.TextBox
            Me.SkipLabel = New System.Windows.Forms.Label
            Me.ImportanceLabel = New System.Windows.Forms.Label
            Me.AutoStubLabel = New System.Windows.Forms.Label
            Me.CatsLabel = New System.Windows.Forms.Label
            Me.TemplatesLabel = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'OK_Button
            '
            Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
            Me.OK_Button.Location = New System.Drawing.Point(359, 284)
            Me.OK_Button.Name = "OK_Button"
            Me.OK_Button.Size = New System.Drawing.Size(67, 23)
            Me.OK_Button.TabIndex = 0
            Me.OK_Button.Text = "OK"
            '
            'NameLabel
            '
            Me.NameLabel.AutoSize = True
            Me.NameLabel.Location = New System.Drawing.Point(12, 21)
            Me.NameLabel.Name = "NameLabel"
            Me.NameLabel.Size = New System.Drawing.Size(134, 13)
            Me.NameLabel.TabIndex = 1
            Me.NameLabel.Text = "Preferred Template Name: "
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(12, 67)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(121, 13)
            Me.Label1.TabIndex = 2
            Me.Label1.Text = "Main regular expression:"
            '
            'MainRegexTextBox
            '
            Me.MainRegexTextBox.Location = New System.Drawing.Point(145, 67)
            Me.MainRegexTextBox.Multiline = True
            Me.MainRegexTextBox.Name = "MainRegexTextBox"
            Me.MainRegexTextBox.ReadOnly = True
            Me.MainRegexTextBox.Size = New System.Drawing.Size(281, 36)
            Me.MainRegexTextBox.TabIndex = 3
            Me.ToolTip1.SetToolTip(Me.MainRegexTextBox, "The main regular expression used to search for existing template instances")
            '
            'AmIReadyLabel
            '
            Me.AmIReadyLabel.AutoSize = True
            Me.AmIReadyLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.AmIReadyLabel.Location = New System.Drawing.Point(12, 294)
            Me.AmIReadyLabel.Name = "AmIReadyLabel"
            Me.AmIReadyLabel.Size = New System.Drawing.Size(95, 13)
            Me.AmIReadyLabel.TabIndex = 4
            Me.AmIReadyLabel.Text = "AmIReadyLabel"
            '
            'HasAltNamesLabel
            '
            Me.HasAltNamesLabel.AutoSize = True
            Me.HasAltNamesLabel.Location = New System.Drawing.Point(12, 43)
            Me.HasAltNamesLabel.Name = "HasAltNamesLabel"
            Me.HasAltNamesLabel.Size = New System.Drawing.Size(204, 13)
            Me.HasAltNamesLabel.TabIndex = 5
            Me.HasAltNamesLabel.Text = "Template has alternate names (redirects): "
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(12, 112)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(221, 13)
            Me.Label2.TabIndex = 6
            Me.Label2.Text = "Template name-checking regular expression: "
            '
            'PreferredTemplateNameRegexTextBox
            '
            Me.PreferredTemplateNameRegexTextBox.Location = New System.Drawing.Point(239, 112)
            Me.PreferredTemplateNameRegexTextBox.Name = "PreferredTemplateNameRegexTextBox"
            Me.PreferredTemplateNameRegexTextBox.ReadOnly = True
            Me.PreferredTemplateNameRegexTextBox.Size = New System.Drawing.Size(187, 20)
            Me.PreferredTemplateNameRegexTextBox.TabIndex = 7
            Me.ToolTip1.SetToolTip(Me.PreferredTemplateNameRegexTextBox, "The regex used to check that the template instance bears the preferred name")
            '
            'Label3
            '
            Me.Label3.AutoSize = True
            Me.Label3.Location = New System.Drawing.Point(12, 146)
            Me.Label3.Name = "Label3"
            Me.Label3.Size = New System.Drawing.Size(174, 13)
            Me.Label3.TabIndex = 8
            Me.Label3.Text = "Second-chance regular expression:"
            '
            'SecondChanceRegexTextBox
            '
            Me.SecondChanceRegexTextBox.Location = New System.Drawing.Point(192, 146)
            Me.SecondChanceRegexTextBox.Multiline = True
            Me.SecondChanceRegexTextBox.Name = "SecondChanceRegexTextBox"
            Me.SecondChanceRegexTextBox.ReadOnly = True
            Me.SecondChanceRegexTextBox.Size = New System.Drawing.Size(234, 36)
            Me.SecondChanceRegexTextBox.TabIndex = 9
            Me.ToolTip1.SetToolTip(Me.SecondChanceRegexTextBox, "Looser regex used to search for bad tags")
            '
            'SkipRegexTextBox
            '
            Me.SkipRegexTextBox.Location = New System.Drawing.Point(192, 191)
            Me.SkipRegexTextBox.Multiline = True
            Me.SkipRegexTextBox.Name = "SkipRegexTextBox"
            Me.SkipRegexTextBox.ReadOnly = True
            Me.SkipRegexTextBox.Size = New System.Drawing.Size(234, 36)
            Me.SkipRegexTextBox.TabIndex = 11
            Me.ToolTip1.SetToolTip(Me.SkipRegexTextBox, "We skip if the talk page contains a match for this regex")
            '
            'SkipLabel
            '
            Me.SkipLabel.AutoSize = True
            Me.SkipLabel.Location = New System.Drawing.Point(12, 191)
            Me.SkipLabel.Name = "SkipLabel"
            Me.SkipLabel.Size = New System.Drawing.Size(88, 13)
            Me.SkipLabel.TabIndex = 10
            Me.SkipLabel.Text = "Skip if contains:  "
            '
            'ImportanceLabel
            '
            Me.ImportanceLabel.AutoSize = True
            Me.ImportanceLabel.Location = New System.Drawing.Point(12, 237)
            Me.ImportanceLabel.Name = "ImportanceLabel"
            Me.ImportanceLabel.Size = New System.Drawing.Size(116, 13)
            Me.ImportanceLabel.TabIndex = 12
            Me.ImportanceLabel.Text = "Importance parameter: "
            '
            'AutoStubLabel
            '
            Me.AutoStubLabel.AutoSize = True
            Me.AutoStubLabel.Location = New System.Drawing.Point(236, 237)
            Me.AutoStubLabel.Name = "AutoStubLabel"
            Me.AutoStubLabel.Size = New System.Drawing.Size(110, 13)
            Me.AutoStubLabel.TabIndex = 13
            Me.AutoStubLabel.Text = "Auto-Stub parameter: "
            '
            'CatsLabel
            '
            Me.CatsLabel.AutoSize = True
            Me.CatsLabel.Location = New System.Drawing.Point(12, 261)
            Me.CatsLabel.Name = "CatsLabel"
            Me.CatsLabel.Size = New System.Drawing.Size(135, 13)
            Me.CatsLabel.TabIndex = 14
            Me.CatsLabel.Text = "Category parameter: class="
            '
            'TemplatesLabel
            '
            Me.TemplatesLabel.AutoSize = True
            Me.TemplatesLabel.Location = New System.Drawing.Point(236, 261)
            Me.TemplatesLabel.Name = "TemplatesLabel"
            Me.TemplatesLabel.Size = New System.Drawing.Size(142, 13)
            Me.TemplatesLabel.TabIndex = 15
            Me.TemplatesLabel.Text = "Templates parameter: class="
            '
            'GenericTemplatePropertiesForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(441, 322)
            Me.Controls.Add(Me.TemplatesLabel)
            Me.Controls.Add(Me.CatsLabel)
            Me.Controls.Add(Me.AutoStubLabel)
            Me.Controls.Add(Me.ImportanceLabel)
            Me.Controls.Add(Me.SkipRegexTextBox)
            Me.Controls.Add(Me.SkipLabel)
            Me.Controls.Add(Me.SecondChanceRegexTextBox)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.PreferredTemplateNameRegexTextBox)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.HasAltNamesLabel)
            Me.Controls.Add(Me.AmIReadyLabel)
            Me.Controls.Add(Me.MainRegexTextBox)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.NameLabel)
            Me.Controls.Add(Me.OK_Button)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "GenericTemplatePropertiesForm"
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Generic Template Properties"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents OK_Button As System.Windows.Forms.Button
        Friend WithEvents NameLabel As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents MainRegexTextBox As System.Windows.Forms.TextBox
        Friend WithEvents AmIReadyLabel As System.Windows.Forms.Label
        Friend WithEvents HasAltNamesLabel As System.Windows.Forms.Label
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents PreferredTemplateNameRegexTextBox As System.Windows.Forms.TextBox
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents SecondChanceRegexTextBox As System.Windows.Forms.TextBox
        Friend WithEvents SkipLabel As System.Windows.Forms.Label
        Friend WithEvents SkipRegexTextBox As System.Windows.Forms.TextBox
        Friend WithEvents ImportanceLabel As System.Windows.Forms.Label
        Friend WithEvents AutoStubLabel As System.Windows.Forms.Label
        Friend WithEvents CatsLabel As System.Windows.Forms.Label
        Friend WithEvents TemplatesLabel As System.Windows.Forms.Label

    End Class
End Namespace