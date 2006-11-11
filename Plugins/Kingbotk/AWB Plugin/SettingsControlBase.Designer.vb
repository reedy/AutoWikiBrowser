<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsControlBase
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
        Me.TipLabel = New System.Windows.Forms.Label
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
        Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'TipLabel
        '
        Me.TipLabel.AutoSize = True
        Me.TipLabel.Location = New System.Drawing.Point(18, 297)
        Me.TipLabel.Name = "TipLabel"
        Me.TipLabel.Size = New System.Drawing.Size(255, 39)
        Me.TipLabel.TabIndex = 7
        Me.TipLabel.Text = "Tip: The plugin also adds parameter insertion options" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to the context menu of the" & _
            " edit box. Just right" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "click inside the edit box to access them."
        Me.TipLabel.Visible = False
        '
        'StubClassCheckBox
        '
        Me.StubClassCheckBox.AutoSize = True
        Me.StubClassCheckBox.Location = New System.Drawing.Point(21, 42)
        Me.StubClassCheckBox.Name = "StubClassCheckBox"
        Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
        Me.StubClassCheckBox.TabIndex = 9
        Me.StubClassCheckBox.Text = "Stub-Class"
        Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
        Me.StubClassCheckBox.UseVisualStyleBackColor = True
        '
        'AutoStubCheckBox
        '
        Me.AutoStubCheckBox.AutoSize = True
        Me.AutoStubCheckBox.Enabled = False
        Me.AutoStubCheckBox.Location = New System.Drawing.Point(21, 19)
        Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
        Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
        Me.AutoStubCheckBox.TabIndex = 8
        Me.AutoStubCheckBox.Text = "Auto-Stub"
        Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
        Me.AutoStubCheckBox.UseVisualStyleBackColor = True
        '
        'SettingsControlBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.StubClassCheckBox)
        Me.Controls.Add(Me.AutoStubCheckBox)
        Me.Controls.Add(Me.TipLabel)
        Me.MaximumSize = New System.Drawing.Size(276, 349)
        Me.MinimumSize = New System.Drawing.Size(276, 349)
        Me.Name = "SettingsControlBase"
        Me.Size = New System.Drawing.Size(276, 349)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Protected Friend WithEvents TipLabel As System.Windows.Forms.Label
    Protected Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Protected Friend WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
    Protected Friend WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox

End Class
