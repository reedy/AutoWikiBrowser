Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WPNovelSettings
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
            Me.WorkgroupsGroupBox = New System.Windows.Forms.GroupBox
            Me.SFCheckBox = New System.Windows.Forms.CheckBox
            Me.ShortStoryCheckBox = New System.Windows.Forms.CheckBox
            Me.CrimeCheckBox = New System.Windows.Forms.CheckBox
            Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
            Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
            Me.ParametersGroup = New System.Windows.Forms.GroupBox
            Me.TextInsertContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.WPNovelsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.NovelsWikiProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.NovelinfoboxneededToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.NovelinfoboxincompToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ClassListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.CoverNeededToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.CoverNeeded1stEditionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.WorkgroupsGroupBox.SuspendLayout()
            Me.ParametersGroup.SuspendLayout()
            Me.TextInsertContextMenuStrip.SuspendLayout()
            Me.SuspendLayout()
            '
            'WorkgroupsGroupBox
            '
            Me.WorkgroupsGroupBox.Controls.Add(Me.SFCheckBox)
            Me.WorkgroupsGroupBox.Controls.Add(Me.ShortStoryCheckBox)
            Me.WorkgroupsGroupBox.Controls.Add(Me.CrimeCheckBox)
            Me.WorkgroupsGroupBox.Location = New System.Drawing.Point(141, 6)
            Me.WorkgroupsGroupBox.Name = "WorkgroupsGroupBox"
            Me.WorkgroupsGroupBox.Size = New System.Drawing.Size(123, 82)
            Me.WorkgroupsGroupBox.TabIndex = 2
            Me.WorkgroupsGroupBox.TabStop = False
            Me.WorkgroupsGroupBox.Text = "Workgroups"
            Me.ToolTip1.SetToolTip(Me.WorkgroupsGroupBox, "a&e-work-group=yes")
            '
            'SFCheckBox
            '
            Me.SFCheckBox.AutoSize = True
            Me.SFCheckBox.Location = New System.Drawing.Point(6, 63)
            Me.SFCheckBox.Name = "SFCheckBox"
            Me.SFCheckBox.Size = New System.Drawing.Size(39, 17)
            Me.SFCheckBox.TabIndex = 8
            Me.SFCheckBox.Text = "SF"
            Me.ToolTip1.SetToolTip(Me.SFCheckBox, "musician-work-group=yes")
            Me.SFCheckBox.UseVisualStyleBackColor = True
            '
            'ShortStoryCheckBox
            '
            Me.ShortStoryCheckBox.AutoSize = True
            Me.ShortStoryCheckBox.Location = New System.Drawing.Point(6, 41)
            Me.ShortStoryCheckBox.Name = "ShortStoryCheckBox"
            Me.ShortStoryCheckBox.Size = New System.Drawing.Size(78, 17)
            Me.ShortStoryCheckBox.TabIndex = 3
            Me.ShortStoryCheckBox.Text = "Short Story"
            Me.ToolTip1.SetToolTip(Me.ShortStoryCheckBox, "military-work-group=yes")
            Me.ShortStoryCheckBox.UseVisualStyleBackColor = True
            '
            'CrimeCheckBox
            '
            Me.CrimeCheckBox.AutoSize = True
            Me.CrimeCheckBox.Location = New System.Drawing.Point(6, 19)
            Me.CrimeCheckBox.Name = "CrimeCheckBox"
            Me.CrimeCheckBox.Size = New System.Drawing.Size(52, 17)
            Me.CrimeCheckBox.TabIndex = 3
            Me.CrimeCheckBox.Text = "Crime"
            Me.ToolTip1.SetToolTip(Me.CrimeCheckBox, "a&e-work-group=yes")
            Me.CrimeCheckBox.UseVisualStyleBackColor = True
            '
            'StubClassCheckBox
            '
            Me.StubClassCheckBox.AutoSize = True
            Me.StubClassCheckBox.Location = New System.Drawing.Point(6, 42)
            Me.StubClassCheckBox.Name = "StubClassCheckBox"
            Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
            Me.StubClassCheckBox.TabIndex = 3
            Me.StubClassCheckBox.Text = "Stub-Class"
            Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
            Me.StubClassCheckBox.UseVisualStyleBackColor = True
            '
            'AutoStubCheckBox
            '
            Me.AutoStubCheckBox.AutoSize = True
            Me.AutoStubCheckBox.Location = New System.Drawing.Point(6, 19)
            Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
            Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
            Me.AutoStubCheckBox.TabIndex = 2
            Me.AutoStubCheckBox.Text = "Auto-Stub"
            Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
            Me.AutoStubCheckBox.UseVisualStyleBackColor = True
            '
            'LinkLabel1
            '
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.Location = New System.Drawing.Point(176, 314)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(74, 13)
            Me.LinkLabel1.TabIndex = 7
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "{{NovelsWikiProject}}"
            '
            'ParametersGroup
            '
            Me.ParametersGroup.Controls.Add(Me.StubClassCheckBox)
            Me.ParametersGroup.Controls.Add(Me.AutoStubCheckBox)
            Me.ParametersGroup.Location = New System.Drawing.Point(12, 6)
            Me.ParametersGroup.Name = "ParametersGroup"
            Me.ParametersGroup.Size = New System.Drawing.Size(123, 74)
            Me.ParametersGroup.TabIndex = 8
            Me.ParametersGroup.TabStop = False
            Me.ParametersGroup.Text = "Template Parameters"
            '
            'TextInsertContextMenuStrip
            '
            Me.TextInsertContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.WPNovelsToolStripMenuItem})
            Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
            Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(123, 26)
            '
            'WPNovelsToolStripMenuItem
            '
            Me.WPNovelsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NovelsWikiProjectToolStripMenuItem, Me.NovelinfoboxneededToolStripMenuItem, Me.NovelinfoboxincompToolStripMenuItem, Me.ClassListToolStripMenuItem, Me.CoverNeededToolStripMenuItem, Me.CoverNeeded1stEditionToolStripMenuItem})
            Me.WPNovelsToolStripMenuItem.Name = "WPNovelsToolStripMenuItem"
            Me.WPNovelsToolStripMenuItem.Size = New System.Drawing.Size(122, 22)
            Me.WPNovelsToolStripMenuItem.Text = "WPNovels"
            '
            'NovelsWikiProjectToolStripMenuItem
            '
            Me.NovelsWikiProjectToolStripMenuItem.Name = "NovelsWikiProjectToolStripMenuItem"
            Me.NovelsWikiProjectToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.NovelsWikiProjectToolStripMenuItem.Text = "{{NovelsWikiProject}}"
            '
            'NovelinfoboxneededToolStripMenuItem
            '
            Me.NovelinfoboxneededToolStripMenuItem.Name = "NovelinfoboxneededToolStripMenuItem"
            Me.NovelinfoboxneededToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.NovelinfoboxneededToolStripMenuItem.Text = "Infobox needed"
            '
            'NovelinfoboxincompToolStripMenuItem
            '
            Me.NovelinfoboxincompToolStripMenuItem.Name = "NovelinfoboxincompToolStripMenuItem"
            Me.NovelinfoboxincompToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.NovelinfoboxincompToolStripMenuItem.Text = "Infobox incomplete"
            '
            'ClassListToolStripMenuItem
            '
            Me.ClassListToolStripMenuItem.Name = "ClassListToolStripMenuItem"
            Me.ClassListToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.ClassListToolStripMenuItem.Text = "class=List"
            '
            'CoverNeededToolStripMenuItem
            '
            Me.CoverNeededToolStripMenuItem.Name = "CoverNeededToolStripMenuItem"
            Me.CoverNeededToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.CoverNeededToolStripMenuItem.Text = "Cover needed"
            '
            'CoverNeeded1stEditionToolStripMenuItem
            '
            Me.CoverNeeded1stEditionToolStripMenuItem.Name = "CoverNeeded1stEditionToolStripMenuItem"
            Me.CoverNeeded1stEditionToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
            Me.CoverNeeded1stEditionToolStripMenuItem.Text = "Cover needed (1st edition)"
            '
            'WPNovelSettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.ParametersGroup)
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.WorkgroupsGroupBox)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "WPNovelSettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.WorkgroupsGroupBox.ResumeLayout(False)
            Me.WorkgroupsGroupBox.PerformLayout()
            Me.ParametersGroup.ResumeLayout(False)
            Me.ParametersGroup.PerformLayout()
            Me.TextInsertContextMenuStrip.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents WorkgroupsGroupBox As System.Windows.Forms.GroupBox
        Friend WithEvents CrimeCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents ShortStoryCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Friend WithEvents SFCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents ParametersGroup As System.Windows.Forms.GroupBox
        Friend WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents WPNovelsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents NovelsWikiProjectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents NovelinfoboxneededToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents NovelinfoboxincompToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents ClassListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CoverNeededToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Friend WithEvents CoverNeeded1stEditionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

    End Class
End Namespace