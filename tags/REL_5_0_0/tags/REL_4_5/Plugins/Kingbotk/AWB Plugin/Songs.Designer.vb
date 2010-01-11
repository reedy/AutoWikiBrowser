Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WPSongsSettings
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
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
            Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
            Me.TextInsertContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.InsertTemplateCallMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.TextInsertContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'StubClassCheckBox
            '
            Me.StubClassCheckBox.AutoSize = True
            Me.StubClassCheckBox.Location = New System.Drawing.Point(24, 43)
            Me.StubClassCheckBox.Name = "StubClassCheckBox"
            Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
            Me.StubClassCheckBox.TabIndex = 5
            Me.StubClassCheckBox.Text = "Stub-Class"
            Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
            Me.StubClassCheckBox.UseVisualStyleBackColor = True
            '
            'AutoStubCheckBox
            '
            Me.AutoStubCheckBox.AutoSize = True
            Me.AutoStubCheckBox.Location = New System.Drawing.Point(24, 20)
            Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
            Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
            Me.AutoStubCheckBox.TabIndex = 4
            Me.AutoStubCheckBox.Text = "Auto-Stub"
            Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
            Me.AutoStubCheckBox.UseVisualStyleBackColor = True
            '
            'LinkLabel1
            '
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.Location = New System.Drawing.Point(130, 24)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(51, 13)
            Me.LinkLabel1.TabIndex = 8
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "{{songs}}"
            '
            'TextInsertContextMenuStrip
            '
            Me.TextInsertContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InsertTemplateCallMenuItem})
            Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
            Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(123, 26)
            '
            'InsertTemplateCallMenuItem
            '
            Me.InsertTemplateCallMenuItem.Name = "InsertTemplateCallMenuItem"
            Me.InsertTemplateCallMenuItem.Size = New System.Drawing.Size(122, 22)
            Me.InsertTemplateCallMenuItem.Text = "{{songs}}"
            '
            'WPSongsSettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.StubClassCheckBox)
            Me.Controls.Add(Me.AutoStubCheckBox)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "WPSongsSettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.TextInsertContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Private WithEvents InsertTemplateCallMenuItem As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace