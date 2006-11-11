<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WPBioSettings
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
        Me.ParametersGroup = New System.Windows.Forms.GroupBox
        Me.ActivePoliticianCheckBox = New System.Windows.Forms.CheckBox
        Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
        Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
        Me.LivingCheckBox = New System.Windows.Forms.CheckBox
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.WorkgroupsGroupBox = New System.Windows.Forms.GroupBox
        Me.PoliticianCheckBox = New System.Windows.Forms.CheckBox
        Me.RoyaltyCheckBox = New System.Windows.Forms.CheckBox
        Me.MilitaryCheckBox = New System.Windows.Forms.CheckBox
        Me.ArtsEntsCheckBox = New System.Windows.Forms.CheckBox
        Me.CategoryTalkCheckBox = New System.Windows.Forms.CheckBox
        Me.ForcePriorityParmCheckBox = New System.Windows.Forms.CheckBox
        Me.CategoryTextBox = New System.Windows.Forms.TextBox
        Me.OptionsGroupBox = New System.Windows.Forms.GroupBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.TipLabel = New System.Windows.Forms.Label
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
        Me.ParametersGroup.SuspendLayout()
        Me.WorkgroupsGroupBox.SuspendLayout()
        Me.OptionsGroupBox.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ParametersGroup
        '
        Me.ParametersGroup.Controls.Add(Me.ActivePoliticianCheckBox)
        Me.ParametersGroup.Controls.Add(Me.StubClassCheckBox)
        Me.ParametersGroup.Controls.Add(Me.AutoStubCheckBox)
        Me.ParametersGroup.Controls.Add(Me.LivingCheckBox)
        Me.ParametersGroup.Location = New System.Drawing.Point(6, 6)
        Me.ParametersGroup.Name = "ParametersGroup"
        Me.ParametersGroup.Size = New System.Drawing.Size(123, 118)
        Me.ParametersGroup.TabIndex = 1
        Me.ParametersGroup.TabStop = False
        Me.ParametersGroup.Text = "Template Parameters"
        '
        'ActivePoliticianCheckBox
        '
        Me.ActivePoliticianCheckBox.AutoSize = True
        Me.ActivePoliticianCheckBox.Location = New System.Drawing.Point(6, 88)
        Me.ActivePoliticianCheckBox.Name = "ActivePoliticianCheckBox"
        Me.ActivePoliticianCheckBox.Size = New System.Drawing.Size(101, 17)
        Me.ActivePoliticianCheckBox.TabIndex = 4
        Me.ActivePoliticianCheckBox.Text = "Active Politician"
        Me.ToolTip1.SetToolTip(Me.ActivePoliticianCheckBox, "activepol=yes")
        Me.ActivePoliticianCheckBox.UseVisualStyleBackColor = True
        '
        'StubClassCheckBox
        '
        Me.StubClassCheckBox.AutoSize = True
        Me.StubClassCheckBox.Location = New System.Drawing.Point(6, 65)
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
        Me.AutoStubCheckBox.Location = New System.Drawing.Point(6, 42)
        Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
        Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
        Me.AutoStubCheckBox.TabIndex = 2
        Me.AutoStubCheckBox.Text = "Auto-Stub"
        Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
        Me.AutoStubCheckBox.UseVisualStyleBackColor = True
        '
        'LivingCheckBox
        '
        Me.LivingCheckBox.AutoSize = True
        Me.LivingCheckBox.Location = New System.Drawing.Point(6, 19)
        Me.LivingCheckBox.Name = "LivingCheckBox"
        Me.LivingCheckBox.Size = New System.Drawing.Size(89, 17)
        Me.LivingCheckBox.TabIndex = 1
        Me.LivingCheckBox.Text = "Living person"
        Me.ToolTip1.SetToolTip(Me.LivingCheckBox, "living=yes")
        Me.LivingCheckBox.UseVisualStyleBackColor = True
        '
        'WorkgroupsGroupBox
        '
        Me.WorkgroupsGroupBox.Controls.Add(Me.PoliticianCheckBox)
        Me.WorkgroupsGroupBox.Controls.Add(Me.RoyaltyCheckBox)
        Me.WorkgroupsGroupBox.Controls.Add(Me.MilitaryCheckBox)
        Me.WorkgroupsGroupBox.Controls.Add(Me.ArtsEntsCheckBox)
        Me.WorkgroupsGroupBox.Location = New System.Drawing.Point(135, 6)
        Me.WorkgroupsGroupBox.Name = "WorkgroupsGroupBox"
        Me.WorkgroupsGroupBox.Size = New System.Drawing.Size(121, 118)
        Me.WorkgroupsGroupBox.TabIndex = 2
        Me.WorkgroupsGroupBox.TabStop = False
        Me.WorkgroupsGroupBox.Text = "Workgroups"
        Me.ToolTip1.SetToolTip(Me.WorkgroupsGroupBox, "a&e-work-group=yes")
        '
        'PoliticianCheckBox
        '
        Me.PoliticianCheckBox.AutoSize = True
        Me.PoliticianCheckBox.Location = New System.Drawing.Point(6, 88)
        Me.PoliticianCheckBox.Name = "PoliticianCheckBox"
        Me.PoliticianCheckBox.Size = New System.Drawing.Size(68, 17)
        Me.PoliticianCheckBox.TabIndex = 5
        Me.PoliticianCheckBox.Text = "Politician"
        Me.ToolTip1.SetToolTip(Me.PoliticianCheckBox, "politician-work-group=yes")
        Me.PoliticianCheckBox.UseVisualStyleBackColor = True
        '
        'RoyaltyCheckBox
        '
        Me.RoyaltyCheckBox.AutoSize = True
        Me.RoyaltyCheckBox.Location = New System.Drawing.Point(6, 65)
        Me.RoyaltyCheckBox.Name = "RoyaltyCheckBox"
        Me.RoyaltyCheckBox.Size = New System.Drawing.Size(61, 17)
        Me.RoyaltyCheckBox.TabIndex = 4
        Me.RoyaltyCheckBox.Text = "Royalty"
        Me.ToolTip1.SetToolTip(Me.RoyaltyCheckBox, "royalty-work-group=yes")
        Me.RoyaltyCheckBox.UseVisualStyleBackColor = True
        '
        'MilitaryCheckBox
        '
        Me.MilitaryCheckBox.AutoSize = True
        Me.MilitaryCheckBox.Location = New System.Drawing.Point(6, 42)
        Me.MilitaryCheckBox.Name = "MilitaryCheckBox"
        Me.MilitaryCheckBox.Size = New System.Drawing.Size(58, 17)
        Me.MilitaryCheckBox.TabIndex = 3
        Me.MilitaryCheckBox.Text = "Military"
        Me.ToolTip1.SetToolTip(Me.MilitaryCheckBox, "military-work-group=yes")
        Me.MilitaryCheckBox.UseVisualStyleBackColor = True
        '
        'ArtsEntsCheckBox
        '
        Me.ArtsEntsCheckBox.AutoSize = True
        Me.ArtsEntsCheckBox.Location = New System.Drawing.Point(6, 19)
        Me.ArtsEntsCheckBox.Name = "ArtsEntsCheckBox"
        Me.ArtsEntsCheckBox.Size = New System.Drawing.Size(77, 17)
        Me.ArtsEntsCheckBox.TabIndex = 3
        Me.ArtsEntsCheckBox.Text = "Arts && Ents"
        Me.ToolTip1.SetToolTip(Me.ArtsEntsCheckBox, "a&e-work-group=yes")
        Me.ArtsEntsCheckBox.UseVisualStyleBackColor = True
        '
        'CategoryTalkCheckBox
        '
        Me.CategoryTalkCheckBox.AutoSize = True
        Me.CategoryTalkCheckBox.Checked = True
        Me.CategoryTalkCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CategoryTalkCheckBox.Location = New System.Drawing.Point(6, 19)
        Me.CategoryTalkCheckBox.Name = "CategoryTalkCheckBox"
        Me.CategoryTalkCheckBox.Size = New System.Drawing.Size(141, 17)
        Me.CategoryTalkCheckBox.TabIndex = 4
        Me.CategoryTalkCheckBox.Text = "Tag category talk pages"
        Me.ToolTip1.SetToolTip(Me.CategoryTalkCheckBox, "{{WPBiography|class=Cat}}")
        Me.CategoryTalkCheckBox.UseVisualStyleBackColor = True
        '
        'ForcePriorityParmCheckBox
        '
        Me.ForcePriorityParmCheckBox.AutoSize = True
        Me.ForcePriorityParmCheckBox.Location = New System.Drawing.Point(6, 42)
        Me.ForcePriorityParmCheckBox.Name = "ForcePriorityParmCheckBox"
        Me.ForcePriorityParmCheckBox.Size = New System.Drawing.Size(222, 17)
        Me.ForcePriorityParmCheckBox.TabIndex = 5
        Me.ForcePriorityParmCheckBox.Text = "Force migration of importance= to priority="
        Me.ToolTip1.SetToolTip(Me.ForcePriorityParmCheckBox, "Changes importance= to priority=, even if the field is empty and there are no oth" & _
                "er changes")
        Me.ForcePriorityParmCheckBox.UseVisualStyleBackColor = True
        '
        'CategoryTextBox
        '
        Me.CategoryTextBox.Location = New System.Drawing.Point(6, 19)
        Me.CategoryTextBox.Name = "CategoryTextBox"
        Me.CategoryTextBox.Size = New System.Drawing.Size(172, 20)
        Me.CategoryTextBox.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.CategoryTextBox, "Enter the name of the category you are working with here, and it will be added to" & _
                " the edit summary")
        '
        'OptionsGroupBox
        '
        Me.OptionsGroupBox.Controls.Add(Me.ForcePriorityParmCheckBox)
        Me.OptionsGroupBox.Controls.Add(Me.CategoryTalkCheckBox)
        Me.OptionsGroupBox.Location = New System.Drawing.Point(6, 130)
        Me.OptionsGroupBox.Name = "OptionsGroupBox"
        Me.OptionsGroupBox.Size = New System.Drawing.Size(250, 65)
        Me.OptionsGroupBox.TabIndex = 3
        Me.OptionsGroupBox.TabStop = False
        Me.OptionsGroupBox.Text = "Options"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.CategoryTextBox)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 201)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(250, 50)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Category (optional, for edit summary)"
        '
        'TipLabel
        '
        Me.TipLabel.AutoSize = True
        Me.TipLabel.Location = New System.Drawing.Point(9, 265)
        Me.TipLabel.Name = "TipLabel"
        Me.TipLabel.Size = New System.Drawing.Size(255, 39)
        Me.TipLabel.TabIndex = 6
        Me.TipLabel.Text = "Tip: The plugin also adds parameter insertion options" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to the context menu of the" & _
            " edit box. Just right" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "click inside the edit box to access them."
        Me.TipLabel.Visible = False
        '
        'LinkLabel1
        '
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(176, 314)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(88, 13)
        Me.LinkLabel1.TabIndex = 7
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "{{WPBiography}}"
        '
        'WPBioSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.TipLabel)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.OptionsGroupBox)
        Me.Controls.Add(Me.WorkgroupsGroupBox)
        Me.Controls.Add(Me.ParametersGroup)
        Me.Name = "WPBioSettings"
        Me.Size = New System.Drawing.Size(276, 349)
        Me.ParametersGroup.ResumeLayout(False)
        Me.ParametersGroup.PerformLayout()
        Me.WorkgroupsGroupBox.ResumeLayout(False)
        Me.WorkgroupsGroupBox.PerformLayout()
        Me.OptionsGroupBox.ResumeLayout(False)
        Me.OptionsGroupBox.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ParametersGroup As System.Windows.Forms.GroupBox
    Friend WithEvents LivingCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ActivePoliticianCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents WorkgroupsGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents ArtsEntsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents PoliticianCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents RoyaltyCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents MilitaryCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents OptionsGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents ForcePriorityParmCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CategoryTalkCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CategoryTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TipLabel As System.Windows.Forms.Label
    Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel

End Class
