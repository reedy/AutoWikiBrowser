Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class GenericWithWorkgroups
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
            Me.components = New System.ComponentModel.Container()
            Me.TextInsertContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.ProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.InsertTemplateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
            Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.StubClassCheckBox = New System.Windows.Forms.CheckBox()
            Me.RemoveImportanceCheckBox = New System.Windows.Forms.CheckBox()
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
            Me.ListView1 = New System.Windows.Forms.ListView()
            Me.colWG = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
            Me.TextInsertContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'TextInsertContextMenuStrip
            '
            Me.TextInsertContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ProjectToolStripMenuItem})
            Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
            Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(153, 48)
            '
            'ProjectToolStripMenuItem
            '
            Me.ProjectToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.InsertTemplateToolStripMenuItem, Me.ToolStripSeparator1})
            Me.ProjectToolStripMenuItem.Name = "ProjectToolStripMenuItem"
            Me.ProjectToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.ProjectToolStripMenuItem.Text = "WPMilHist"
            '
            'InsertTemplateToolStripMenuItem
            '
            Me.InsertTemplateToolStripMenuItem.Name = "InsertTemplateToolStripMenuItem"
            Me.InsertTemplateToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
            Me.InsertTemplateToolStripMenuItem.Text = "{{}}"
            '
            'ToolStripSeparator1
            '
            Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
            Me.ToolStripSeparator1.Size = New System.Drawing.Size(149, 6)
            '
            'StubClassCheckBox
            '
            Me.StubClassCheckBox.AutoSize = True
            Me.StubClassCheckBox.Location = New System.Drawing.Point(10, 306)
            Me.StubClassCheckBox.Name = "StubClassCheckBox"
            Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
            Me.StubClassCheckBox.TabIndex = 3
            Me.StubClassCheckBox.Text = "Stub-Class"
            Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
            Me.StubClassCheckBox.UseVisualStyleBackColor = True
            '
            'RemoveImportanceCheckBox
            '
            Me.RemoveImportanceCheckBox.AutoSize = True
            Me.RemoveImportanceCheckBox.Location = New System.Drawing.Point(92, 293)
            Me.RemoveImportanceCheckBox.Name = "RemoveImportanceCheckBox"
            Me.RemoveImportanceCheckBox.Size = New System.Drawing.Size(84, 43)
            Me.RemoveImportanceCheckBox.TabIndex = 9
            Me.RemoveImportanceCheckBox.Text = "Force" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "removal of" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "importance="
            Me.ToolTip1.SetToolTip(Me.RemoveImportanceCheckBox, "Remove importance= parameters forceably")
            Me.RemoveImportanceCheckBox.UseVisualStyleBackColor = True
            '
            'LinkLabel1
            '
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.Location = New System.Drawing.Point(182, 307)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(23, 13)
            Me.LinkLabel1.TabIndex = 8
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "{{}}"
            '
            'ListView1
            '
            Me.ListView1.CheckBoxes = True
            Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colWG})
            Me.ListView1.Location = New System.Drawing.Point(3, 3)
            Me.ListView1.Name = "ListView1"
            Me.ListView1.Size = New System.Drawing.Size(270, 284)
            Me.ListView1.TabIndex = 10
            Me.ListView1.UseCompatibleStateImageBehavior = False
            Me.ListView1.View = System.Windows.Forms.View.Details
            '
            'colWG
            '
            Me.colWG.Text = "Workgroup"
            Me.colWG.Width = 266
            '
            'WPMilitaryHistorySettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ListView1)
            Me.Controls.Add(Me.StubClassCheckBox)
            Me.Controls.Add(Me.RemoveImportanceCheckBox)
            Me.Controls.Add(Me.LinkLabel1)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "WPMilitaryHistorySettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.TextInsertContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Private WithEvents RemoveImportanceCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents ProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents InsertTemplateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
        Friend WithEvents ListView1 As System.Windows.Forms.ListView
        Friend WithEvents colWG As System.Windows.Forms.ColumnHeader

    End Class
End Namespace