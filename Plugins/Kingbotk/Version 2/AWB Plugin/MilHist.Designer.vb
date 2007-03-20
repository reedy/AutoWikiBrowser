Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MilHistSettings
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
            Me.TextInsertContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
            Me.WPMILHISTToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
            Me.NapoleonicCheckBox = New System.Windows.Forms.CheckBox
            Me.AncientNearEastCheckBox = New System.Windows.Forms.CheckBox
            Me.AmericanCivilWarCheckBox = New System.Windows.Forms.CheckBox
            Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
            Me.RemoveImportanceCheckBox = New System.Windows.Forms.CheckBox
            Me.TaskForcesGroupBox = New System.Windows.Forms.GroupBox
            Me.EarlyModernCheckBox = New System.Windows.Forms.CheckBox
            Me.WWIICheckBox = New System.Windows.Forms.CheckBox
            Me.WWICheckBox = New System.Windows.Forms.CheckBox
            Me.WeaponryCheckBox = New System.Windows.Forms.CheckBox
            Me.USCheckBox = New System.Windows.Forms.CheckBox
            Me.PolishCheckBox = New System.Windows.Forms.CheckBox
            Me.MiddleAgesCheckBox = New System.Windows.Forms.CheckBox
            Me.MemorialsCheckBox = New System.Windows.Forms.CheckBox
            Me.MaritimeCheckBox = New System.Windows.Forms.CheckBox
            Me.JapaneseCheckBox = New System.Windows.Forms.CheckBox
            Me.ItalianCheckBox = New System.Windows.Forms.CheckBox
            Me.IndianCheckBox = New System.Windows.Forms.CheckBox
            Me.GermanCheckBox = New System.Windows.Forms.CheckBox
            Me.FrenchCheckBox = New System.Windows.Forms.CheckBox
            Me.DutchCheckBox = New System.Windows.Forms.CheckBox
            Me.ClassicalCheckBox = New System.Windows.Forms.CheckBox
            Me.ChineseCheckBox = New System.Windows.Forms.CheckBox
            Me.CanadianCheckBox = New System.Windows.Forms.CheckBox
            Me.BritishCheckBox = New System.Windows.Forms.CheckBox
            Me.AviationCheckBox = New System.Windows.Forms.CheckBox
            Me.AustralianCheckBox = New System.Windows.Forms.CheckBox
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
            Me.TextInsertContextMenuStrip.SuspendLayout()
            Me.TaskForcesGroupBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'TextInsertContextMenuStrip
            '
            Me.TextInsertContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.WPMILHISTToolStripMenuItem})
            Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
            Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(151, 26)
            '
            'WPMILHISTToolStripMenuItem
            '
            Me.WPMILHISTToolStripMenuItem.Name = "WPMILHISTToolStripMenuItem"
            Me.WPMILHISTToolStripMenuItem.Size = New System.Drawing.Size(150, 22)
            Me.WPMILHISTToolStripMenuItem.Text = "{{WPMILHIST}}"
            '
            'StubClassCheckBox
            '
            Me.StubClassCheckBox.AutoSize = True
            Me.StubClassCheckBox.Location = New System.Drawing.Point(196, 22)
            Me.StubClassCheckBox.Name = "StubClassCheckBox"
            Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
            Me.StubClassCheckBox.TabIndex = 3
            Me.StubClassCheckBox.Text = "Stub-Class"
            Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
            Me.StubClassCheckBox.UseVisualStyleBackColor = True
            '
            'NapoleonicCheckBox
            '
            Me.NapoleonicCheckBox.AutoSize = True
            Me.NapoleonicCheckBox.Location = New System.Drawing.Point(100, 129)
            Me.NapoleonicCheckBox.Name = "NapoleonicCheckBox"
            Me.NapoleonicCheckBox.Size = New System.Drawing.Size(80, 17)
            Me.NapoleonicCheckBox.TabIndex = 17
            Me.NapoleonicCheckBox.Text = "Napoleonic"
            Me.ToolTip1.SetToolTip(Me.NapoleonicCheckBox, "Napoleonic Era")
            Me.NapoleonicCheckBox.UseVisualStyleBackColor = True
            '
            'AncientNearEastCheckBox
            '
            Me.AncientNearEastCheckBox.AutoSize = True
            Me.AncientNearEastCheckBox.Location = New System.Drawing.Point(3, 41)
            Me.AncientNearEastCheckBox.Name = "AncientNearEastCheckBox"
            Me.AncientNearEastCheckBox.Size = New System.Drawing.Size(80, 17)
            Me.AncientNearEastCheckBox.TabIndex = 1
            Me.AncientNearEastCheckBox.Text = "Ancient NE"
            Me.ToolTip1.SetToolTip(Me.AncientNearEastCheckBox, "Ancient Near-East")
            Me.AncientNearEastCheckBox.UseVisualStyleBackColor = True
            '
            'AmericanCivilWarCheckBox
            '
            Me.AmericanCivilWarCheckBox.AutoSize = True
            Me.AmericanCivilWarCheckBox.Location = New System.Drawing.Point(3, 19)
            Me.AmericanCivilWarCheckBox.Name = "AmericanCivilWarCheckBox"
            Me.AmericanCivilWarCheckBox.Size = New System.Drawing.Size(91, 17)
            Me.AmericanCivilWarCheckBox.TabIndex = 0
            Me.AmericanCivilWarCheckBox.Text = "American CW"
            Me.ToolTip1.SetToolTip(Me.AmericanCivilWarCheckBox, "American Civil War")
            Me.AmericanCivilWarCheckBox.UseVisualStyleBackColor = True
            '
            'AutoStubCheckBox
            '
            Me.AutoStubCheckBox.AutoSize = True
            Me.AutoStubCheckBox.Location = New System.Drawing.Point(196, 44)
            Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
            Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
            Me.AutoStubCheckBox.TabIndex = 8
            Me.AutoStubCheckBox.Text = "Auto-Stub"
            Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
            Me.AutoStubCheckBox.UseVisualStyleBackColor = True
            '
            'RemoveImportanceCheckBox
            '
            Me.RemoveImportanceCheckBox.AutoSize = True
            Me.RemoveImportanceCheckBox.Location = New System.Drawing.Point(196, 66)
            Me.RemoveImportanceCheckBox.Name = "RemoveImportanceCheckBox"
            Me.RemoveImportanceCheckBox.Size = New System.Drawing.Size(84, 43)
            Me.RemoveImportanceCheckBox.TabIndex = 9
            Me.RemoveImportanceCheckBox.Text = "Force" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "removal of" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "importance="
            Me.ToolTip1.SetToolTip(Me.RemoveImportanceCheckBox, "Remove importance= parameters forceably")
            Me.RemoveImportanceCheckBox.UseVisualStyleBackColor = True
            '
            'TaskForcesGroupBox
            '
            Me.TaskForcesGroupBox.Controls.Add(Me.EarlyModernCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.WWIICheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.WWICheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.WeaponryCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.USCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.PolishCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.NapoleonicCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.MiddleAgesCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.MemorialsCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.MaritimeCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.JapaneseCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.ItalianCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.IndianCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.GermanCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.FrenchCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.DutchCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.ClassicalCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.ChineseCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.CanadianCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.BritishCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.AviationCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.AustralianCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.AncientNearEastCheckBox)
            Me.TaskForcesGroupBox.Controls.Add(Me.AmericanCivilWarCheckBox)
            Me.TaskForcesGroupBox.Location = New System.Drawing.Point(3, 3)
            Me.TaskForcesGroupBox.Name = "TaskForcesGroupBox"
            Me.TaskForcesGroupBox.Size = New System.Drawing.Size(187, 289)
            Me.TaskForcesGroupBox.TabIndex = 1
            Me.TaskForcesGroupBox.TabStop = False
            Me.TaskForcesGroupBox.Text = "Task Forces"
            '
            'EarlyModernCheckBox
            '
            Me.EarlyModernCheckBox.AutoSize = True
            Me.EarlyModernCheckBox.Location = New System.Drawing.Point(100, 261)
            Me.EarlyModernCheckBox.Name = "EarlyModernCheckBox"
            Me.EarlyModernCheckBox.Size = New System.Drawing.Size(88, 17)
            Me.EarlyModernCheckBox.TabIndex = 10
            Me.EarlyModernCheckBox.Text = "Early-Modern"
            Me.EarlyModernCheckBox.UseVisualStyleBackColor = True
            '
            'WWIICheckBox
            '
            Me.WWIICheckBox.AutoSize = True
            Me.WWIICheckBox.Location = New System.Drawing.Point(100, 239)
            Me.WWIICheckBox.Name = "WWIICheckBox"
            Me.WWIICheckBox.Size = New System.Drawing.Size(54, 17)
            Me.WWIICheckBox.TabIndex = 22
            Me.WWIICheckBox.Text = "WWII"
            Me.WWIICheckBox.UseVisualStyleBackColor = True
            '
            'WWICheckBox
            '
            Me.WWICheckBox.AutoSize = True
            Me.WWICheckBox.Location = New System.Drawing.Point(100, 217)
            Me.WWICheckBox.Name = "WWICheckBox"
            Me.WWICheckBox.Size = New System.Drawing.Size(51, 17)
            Me.WWICheckBox.TabIndex = 21
            Me.WWICheckBox.Text = "WWI"
            Me.WWICheckBox.UseVisualStyleBackColor = True
            '
            'WeaponryCheckBox
            '
            Me.WeaponryCheckBox.AutoSize = True
            Me.WeaponryCheckBox.Location = New System.Drawing.Point(100, 195)
            Me.WeaponryCheckBox.Name = "WeaponryCheckBox"
            Me.WeaponryCheckBox.Size = New System.Drawing.Size(75, 17)
            Me.WeaponryCheckBox.TabIndex = 20
            Me.WeaponryCheckBox.Text = "Weaponry"
            Me.WeaponryCheckBox.UseVisualStyleBackColor = True
            '
            'USCheckBox
            '
            Me.USCheckBox.AutoSize = True
            Me.USCheckBox.Location = New System.Drawing.Point(100, 173)
            Me.USCheckBox.Name = "USCheckBox"
            Me.USCheckBox.Size = New System.Drawing.Size(41, 17)
            Me.USCheckBox.TabIndex = 19
            Me.USCheckBox.Text = "US"
            Me.USCheckBox.UseVisualStyleBackColor = True
            '
            'PolishCheckBox
            '
            Me.PolishCheckBox.AutoSize = True
            Me.PolishCheckBox.Location = New System.Drawing.Point(100, 151)
            Me.PolishCheckBox.Name = "PolishCheckBox"
            Me.PolishCheckBox.Size = New System.Drawing.Size(54, 17)
            Me.PolishCheckBox.TabIndex = 18
            Me.PolishCheckBox.Text = "Polish"
            Me.PolishCheckBox.UseVisualStyleBackColor = True
            '
            'MiddleAgesCheckBox
            '
            Me.MiddleAgesCheckBox.AutoSize = True
            Me.MiddleAgesCheckBox.Location = New System.Drawing.Point(100, 107)
            Me.MiddleAgesCheckBox.Name = "MiddleAgesCheckBox"
            Me.MiddleAgesCheckBox.Size = New System.Drawing.Size(84, 17)
            Me.MiddleAgesCheckBox.TabIndex = 16
            Me.MiddleAgesCheckBox.Text = "Middle Ages"
            Me.MiddleAgesCheckBox.UseVisualStyleBackColor = True
            '
            'MemorialsCheckBox
            '
            Me.MemorialsCheckBox.AutoSize = True
            Me.MemorialsCheckBox.Location = New System.Drawing.Point(100, 85)
            Me.MemorialsCheckBox.Name = "MemorialsCheckBox"
            Me.MemorialsCheckBox.Size = New System.Drawing.Size(73, 17)
            Me.MemorialsCheckBox.TabIndex = 15
            Me.MemorialsCheckBox.Text = "Memorials"
            Me.MemorialsCheckBox.UseVisualStyleBackColor = True
            '
            'MaritimeCheckBox
            '
            Me.MaritimeCheckBox.AutoSize = True
            Me.MaritimeCheckBox.Location = New System.Drawing.Point(100, 63)
            Me.MaritimeCheckBox.Name = "MaritimeCheckBox"
            Me.MaritimeCheckBox.Size = New System.Drawing.Size(65, 17)
            Me.MaritimeCheckBox.TabIndex = 14
            Me.MaritimeCheckBox.Text = "Maritime"
            Me.MaritimeCheckBox.UseVisualStyleBackColor = True
            '
            'JapaneseCheckBox
            '
            Me.JapaneseCheckBox.AutoSize = True
            Me.JapaneseCheckBox.Location = New System.Drawing.Point(100, 41)
            Me.JapaneseCheckBox.Name = "JapaneseCheckBox"
            Me.JapaneseCheckBox.Size = New System.Drawing.Size(72, 17)
            Me.JapaneseCheckBox.TabIndex = 13
            Me.JapaneseCheckBox.Text = "Japanese"
            Me.JapaneseCheckBox.UseVisualStyleBackColor = True
            '
            'ItalianCheckBox
            '
            Me.ItalianCheckBox.AutoSize = True
            Me.ItalianCheckBox.Location = New System.Drawing.Point(100, 19)
            Me.ItalianCheckBox.Name = "ItalianCheckBox"
            Me.ItalianCheckBox.Size = New System.Drawing.Size(54, 17)
            Me.ItalianCheckBox.TabIndex = 12
            Me.ItalianCheckBox.Text = "Italian"
            Me.ItalianCheckBox.UseVisualStyleBackColor = True
            '
            'IndianCheckBox
            '
            Me.IndianCheckBox.AutoSize = True
            Me.IndianCheckBox.Location = New System.Drawing.Point(3, 261)
            Me.IndianCheckBox.Name = "IndianCheckBox"
            Me.IndianCheckBox.Size = New System.Drawing.Size(55, 17)
            Me.IndianCheckBox.TabIndex = 11
            Me.IndianCheckBox.Text = "Indian"
            Me.IndianCheckBox.UseVisualStyleBackColor = True
            '
            'GermanCheckBox
            '
            Me.GermanCheckBox.AutoSize = True
            Me.GermanCheckBox.Location = New System.Drawing.Point(3, 239)
            Me.GermanCheckBox.Name = "GermanCheckBox"
            Me.GermanCheckBox.Size = New System.Drawing.Size(63, 17)
            Me.GermanCheckBox.TabIndex = 10
            Me.GermanCheckBox.Text = "German"
            Me.GermanCheckBox.UseVisualStyleBackColor = True
            '
            'FrenchCheckBox
            '
            Me.FrenchCheckBox.AutoSize = True
            Me.FrenchCheckBox.Location = New System.Drawing.Point(3, 217)
            Me.FrenchCheckBox.Name = "FrenchCheckBox"
            Me.FrenchCheckBox.Size = New System.Drawing.Size(59, 17)
            Me.FrenchCheckBox.TabIndex = 9
            Me.FrenchCheckBox.Text = "French"
            Me.FrenchCheckBox.UseVisualStyleBackColor = True
            '
            'DutchCheckBox
            '
            Me.DutchCheckBox.AutoSize = True
            Me.DutchCheckBox.Location = New System.Drawing.Point(3, 195)
            Me.DutchCheckBox.Name = "DutchCheckBox"
            Me.DutchCheckBox.Size = New System.Drawing.Size(55, 17)
            Me.DutchCheckBox.TabIndex = 8
            Me.DutchCheckBox.Text = "Dutch"
            Me.DutchCheckBox.UseVisualStyleBackColor = True
            '
            'ClassicalCheckBox
            '
            Me.ClassicalCheckBox.AutoSize = True
            Me.ClassicalCheckBox.Location = New System.Drawing.Point(3, 173)
            Me.ClassicalCheckBox.Name = "ClassicalCheckBox"
            Me.ClassicalCheckBox.Size = New System.Drawing.Size(67, 17)
            Me.ClassicalCheckBox.TabIndex = 7
            Me.ClassicalCheckBox.Text = "Classical"
            Me.ClassicalCheckBox.UseVisualStyleBackColor = True
            '
            'ChineseCheckBox
            '
            Me.ChineseCheckBox.AutoSize = True
            Me.ChineseCheckBox.Location = New System.Drawing.Point(3, 151)
            Me.ChineseCheckBox.Name = "ChineseCheckBox"
            Me.ChineseCheckBox.Size = New System.Drawing.Size(64, 17)
            Me.ChineseCheckBox.TabIndex = 6
            Me.ChineseCheckBox.Text = "Chinese"
            Me.ChineseCheckBox.UseVisualStyleBackColor = True
            '
            'CanadianCheckBox
            '
            Me.CanadianCheckBox.AutoSize = True
            Me.CanadianCheckBox.Location = New System.Drawing.Point(3, 129)
            Me.CanadianCheckBox.Name = "CanadianCheckBox"
            Me.CanadianCheckBox.Size = New System.Drawing.Size(71, 17)
            Me.CanadianCheckBox.TabIndex = 5
            Me.CanadianCheckBox.Text = "Canadian"
            Me.CanadianCheckBox.UseVisualStyleBackColor = True
            '
            'BritishCheckBox
            '
            Me.BritishCheckBox.AutoSize = True
            Me.BritishCheckBox.Location = New System.Drawing.Point(3, 107)
            Me.BritishCheckBox.Name = "BritishCheckBox"
            Me.BritishCheckBox.Size = New System.Drawing.Size(54, 17)
            Me.BritishCheckBox.TabIndex = 4
            Me.BritishCheckBox.Text = "British"
            Me.BritishCheckBox.UseVisualStyleBackColor = True
            '
            'AviationCheckBox
            '
            Me.AviationCheckBox.AutoSize = True
            Me.AviationCheckBox.Location = New System.Drawing.Point(3, 85)
            Me.AviationCheckBox.Name = "AviationCheckBox"
            Me.AviationCheckBox.Size = New System.Drawing.Size(64, 17)
            Me.AviationCheckBox.TabIndex = 3
            Me.AviationCheckBox.Text = "Aviation"
            Me.AviationCheckBox.UseVisualStyleBackColor = True
            '
            'AustralianCheckBox
            '
            Me.AustralianCheckBox.AutoSize = True
            Me.AustralianCheckBox.Location = New System.Drawing.Point(3, 63)
            Me.AustralianCheckBox.Name = "AustralianCheckBox"
            Me.AustralianCheckBox.Size = New System.Drawing.Size(72, 17)
            Me.AustralianCheckBox.TabIndex = 2
            Me.AustralianCheckBox.Text = "Australian"
            Me.AustralianCheckBox.UseVisualStyleBackColor = True
            '
            'LinkLabel1
            '
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.Location = New System.Drawing.Point(185, 325)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(84, 13)
            Me.LinkLabel1.TabIndex = 8
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "{{WPMILHIST}}"
            '
            'MilHistSettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.RemoveImportanceCheckBox)
            Me.Controls.Add(Me.AutoStubCheckBox)
            Me.Controls.Add(Me.StubClassCheckBox)
            Me.Controls.Add(Me.TaskForcesGroupBox)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "MilHistSettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.TextInsertContextMenuStrip.ResumeLayout(False)
            Me.TaskForcesGroupBox.ResumeLayout(False)
            Me.TaskForcesGroupBox.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents TaskForcesGroupBox As System.Windows.Forms.GroupBox
        Private WithEvents WWIICheckBox As System.Windows.Forms.CheckBox
        Private WithEvents WWICheckBox As System.Windows.Forms.CheckBox
        Private WithEvents WeaponryCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents USCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents PolishCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents NapoleonicCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MiddleAgesCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MemorialsCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MaritimeCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents JapaneseCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents ItalianCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents IndianCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents GermanCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents FrenchCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents DutchCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents ClassicalCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents ChineseCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents CanadianCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents BritishCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AviationCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AustralianCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AncientNearEastCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AmericanCivilWarCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Private WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents RemoveImportanceCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents WPMILHISTToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
        Private WithEvents EarlyModernCheckBox As System.Windows.Forms.CheckBox

    End Class
End Namespace