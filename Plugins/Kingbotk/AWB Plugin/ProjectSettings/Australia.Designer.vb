Namespace AutoWikiBrowser.Plugins.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class WPAustraliaSettings
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
        	Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        	Me.StubClassCheckBox = New System.Windows.Forms.CheckBox
        	Me.AutoStubCheckBox = New System.Windows.Forms.CheckBox
        	Me.LakeMacquarieCheckBox = New System.Windows.Forms.CheckBox
        	Me.TipLabel = New System.Windows.Forms.Label
        	Me.ParametersGroup = New System.Windows.Forms.GroupBox
        	Me.AdelaideCheckBox = New System.Windows.Forms.CheckBox
        	Me.BrisbaneCheckBox = New System.Windows.Forms.CheckBox
        	Me.CanberraCheckBox = New System.Windows.Forms.CheckBox
        	Me.GeelongCheckBox = New System.Windows.Forms.CheckBox
        	Me.HobartCheckBox = New System.Windows.Forms.CheckBox
        	Me.MelbourneCheckBox = New System.Windows.Forms.CheckBox
        	Me.PerthCheckBox = New System.Windows.Forms.CheckBox
        	Me.SydneyCheckBox = New System.Windows.Forms.CheckBox
        	Me.AFLCheckBox = New System.Windows.Forms.CheckBox
        	Me.ALeagueCheckBox = New System.Windows.Forms.CheckBox
        	Me.NRLCheckBox = New System.Windows.Forms.CheckBox
        	Me.V8CheckBox = New System.Windows.Forms.CheckBox
        	Me.CrimeCheckBox = New System.Windows.Forms.CheckBox
        	Me.LawCheckBox = New System.Windows.Forms.CheckBox
        	Me.MilitaryCheckBox = New System.Windows.Forms.CheckBox
        	Me.PlaceCheckBox = New System.Windows.Forms.CheckBox
        	Me.PoliticsCheckBox = New System.Windows.Forms.CheckBox
        	Me.SportCheckBox = New System.Windows.Forms.CheckBox
        	Me.TopicsGroupBox = New System.Windows.Forms.GroupBox
        	Me.MusicCheckBox = New System.Windows.Forms.CheckBox
        	Me.NBLCheckBox = New System.Windows.Forms.CheckBox
        	Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
        	Me.ParametersGroup.SuspendLayout
        	Me.TopicsGroupBox.SuspendLayout
        	Me.SuspendLayout
        	'
        	'TextInsertContextMenuStrip
        	'
        	Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
        	Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(61, 4)
        	'
        	'StubClassCheckBox
        	'
        	Me.StubClassCheckBox.AutoSize = true
        	Me.StubClassCheckBox.Location = New System.Drawing.Point(9, 19)
        	Me.StubClassCheckBox.Name = "StubClassCheckBox"
        	Me.StubClassCheckBox.Size = New System.Drawing.Size(76, 17)
        	Me.StubClassCheckBox.TabIndex = 3
        	Me.StubClassCheckBox.Text = "Stub-Class"
        	Me.ToolTip1.SetToolTip(Me.StubClassCheckBox, "class=Stub (not for use in bot mode; use Auto-Stub)")
        	Me.StubClassCheckBox.UseVisualStyleBackColor = true
        	'
        	'AutoStubCheckBox
        	'
        	Me.AutoStubCheckBox.AutoSize = true
        	Me.AutoStubCheckBox.Location = New System.Drawing.Point(91, 19)
        	Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
        	Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
        	Me.AutoStubCheckBox.TabIndex = 4
        	Me.AutoStubCheckBox.Text = "Auto-Stub"
        	Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
        	Me.AutoStubCheckBox.UseVisualStyleBackColor = true
        	'
        	'LakeMacquarieCheckBox
        	'
        	Me.LakeMacquarieCheckBox.AutoSize = true
        	Me.LakeMacquarieCheckBox.Location = New System.Drawing.Point(3, 129)
        	Me.LakeMacquarieCheckBox.Name = "LakeMacquarieCheckBox"
        	Me.LakeMacquarieCheckBox.Size = New System.Drawing.Size(85, 17)
        	Me.LakeMacquarieCheckBox.TabIndex = 5
        	Me.LakeMacquarieCheckBox.Text = "L Macquarie"
        	Me.ToolTip1.SetToolTip(Me.LakeMacquarieCheckBox, "Lake Macquarie")
        	Me.LakeMacquarieCheckBox.UseVisualStyleBackColor = true
        	'
        	'TipLabel
        	'
        	Me.TipLabel.AutoSize = true
        	Me.TipLabel.Location = New System.Drawing.Point(8, 306)
        	Me.TipLabel.Name = "TipLabel"
        	Me.TipLabel.Size = New System.Drawing.Size(255, 39)
        	Me.TipLabel.TabIndex = 7
        	Me.TipLabel.Text = "Tip: The plugin also adds parameter insertion options"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"to the context menu of the"& _ 
        	" edit box. Just right"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"click inside the edit box to access them."
        	Me.TipLabel.Visible = false
        	'
        	'ParametersGroup
        	'
        	Me.ParametersGroup.Controls.Add(Me.AutoStubCheckBox)
        	Me.ParametersGroup.Controls.Add(Me.StubClassCheckBox)
        	Me.ParametersGroup.Location = New System.Drawing.Point(11, 256)
        	Me.ParametersGroup.Name = "ParametersGroup"
        	Me.ParametersGroup.Size = New System.Drawing.Size(172, 44)
        	Me.ParametersGroup.TabIndex = 8
        	Me.ParametersGroup.TabStop = false
        	Me.ParametersGroup.Text = "Template Parameters"
        	'
        	'AdelaideCheckBox
        	'
        	Me.AdelaideCheckBox.AutoSize = true
        	Me.AdelaideCheckBox.Location = New System.Drawing.Point(3, 19)
        	Me.AdelaideCheckBox.Name = "AdelaideCheckBox"
        	Me.AdelaideCheckBox.Size = New System.Drawing.Size(67, 17)
        	Me.AdelaideCheckBox.TabIndex = 0
        	Me.AdelaideCheckBox.Text = "Adelaide"
        	Me.AdelaideCheckBox.UseVisualStyleBackColor = true
        	'
        	'BrisbaneCheckBox
        	'
        	Me.BrisbaneCheckBox.AutoSize = true
        	Me.BrisbaneCheckBox.Location = New System.Drawing.Point(3, 41)
        	Me.BrisbaneCheckBox.Name = "BrisbaneCheckBox"
        	Me.BrisbaneCheckBox.Size = New System.Drawing.Size(67, 17)
        	Me.BrisbaneCheckBox.TabIndex = 1
        	Me.BrisbaneCheckBox.Text = "Brisbane"
        	Me.BrisbaneCheckBox.UseVisualStyleBackColor = true
        	'
        	'CanberraCheckBox
        	'
        	Me.CanberraCheckBox.AutoSize = true
        	Me.CanberraCheckBox.Location = New System.Drawing.Point(3, 63)
        	Me.CanberraCheckBox.Name = "CanberraCheckBox"
        	Me.CanberraCheckBox.Size = New System.Drawing.Size(69, 17)
        	Me.CanberraCheckBox.TabIndex = 2
        	Me.CanberraCheckBox.Text = "Canberra"
        	Me.CanberraCheckBox.UseVisualStyleBackColor = true
        	'
        	'GeelongCheckBox
        	'
        	Me.GeelongCheckBox.AutoSize = true
        	Me.GeelongCheckBox.Location = New System.Drawing.Point(3, 85)
        	Me.GeelongCheckBox.Name = "GeelongCheckBox"
        	Me.GeelongCheckBox.Size = New System.Drawing.Size(66, 17)
        	Me.GeelongCheckBox.TabIndex = 3
        	Me.GeelongCheckBox.Text = "Geelong"
        	Me.GeelongCheckBox.UseVisualStyleBackColor = true
        	'
        	'HobartCheckBox
        	'
        	Me.HobartCheckBox.AutoSize = true
        	Me.HobartCheckBox.Location = New System.Drawing.Point(3, 107)
        	Me.HobartCheckBox.Name = "HobartCheckBox"
        	Me.HobartCheckBox.Size = New System.Drawing.Size(58, 17)
        	Me.HobartCheckBox.TabIndex = 4
        	Me.HobartCheckBox.Text = "Hobart"
        	Me.HobartCheckBox.UseVisualStyleBackColor = true
        	'
        	'MelbourneCheckBox
        	'
        	Me.MelbourneCheckBox.AutoSize = true
        	Me.MelbourneCheckBox.Location = New System.Drawing.Point(3, 151)
        	Me.MelbourneCheckBox.Name = "MelbourneCheckBox"
        	Me.MelbourneCheckBox.Size = New System.Drawing.Size(76, 17)
        	Me.MelbourneCheckBox.TabIndex = 6
        	Me.MelbourneCheckBox.Text = "Melbourne"
        	Me.MelbourneCheckBox.UseVisualStyleBackColor = true
        	'
        	'PerthCheckBox
        	'
        	Me.PerthCheckBox.AutoSize = true
        	Me.PerthCheckBox.Location = New System.Drawing.Point(3, 173)
        	Me.PerthCheckBox.Name = "PerthCheckBox"
        	Me.PerthCheckBox.Size = New System.Drawing.Size(51, 17)
        	Me.PerthCheckBox.TabIndex = 7
        	Me.PerthCheckBox.Text = "Perth"
        	Me.PerthCheckBox.UseVisualStyleBackColor = true
        	'
        	'SydneyCheckBox
        	'
        	Me.SydneyCheckBox.AutoSize = true
        	Me.SydneyCheckBox.Location = New System.Drawing.Point(3, 195)
        	Me.SydneyCheckBox.Name = "SydneyCheckBox"
        	Me.SydneyCheckBox.Size = New System.Drawing.Size(61, 17)
        	Me.SydneyCheckBox.TabIndex = 8
        	Me.SydneyCheckBox.Text = "Sydney"
        	Me.SydneyCheckBox.UseVisualStyleBackColor = true
        	'
        	'AFLCheckBox
        	'
        	Me.AFLCheckBox.AutoSize = true
        	Me.AFLCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.AFLCheckBox.Location = New System.Drawing.Point(165, 41)
        	Me.AFLCheckBox.Name = "AFLCheckBox"
        	Me.AFLCheckBox.Size = New System.Drawing.Size(45, 17)
        	Me.AFLCheckBox.TabIndex = 12
        	Me.AFLCheckBox.Text = "AFL"
        	Me.AFLCheckBox.UseVisualStyleBackColor = true
        	'
        	'ALeagueCheckBox
        	'
        	Me.ALeagueCheckBox.AutoSize = true
        	Me.ALeagueCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.ALeagueCheckBox.Location = New System.Drawing.Point(165, 63)
        	Me.ALeagueCheckBox.Name = "ALeagueCheckBox"
        	Me.ALeagueCheckBox.Size = New System.Drawing.Size(72, 17)
        	Me.ALeagueCheckBox.TabIndex = 13
        	Me.ALeagueCheckBox.Text = "A-League"
        	Me.ALeagueCheckBox.UseVisualStyleBackColor = true
        	'
        	'NRLCheckBox
        	'
        	Me.NRLCheckBox.AutoSize = true
        	Me.NRLCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.NRLCheckBox.Location = New System.Drawing.Point(165, 107)
        	Me.NRLCheckBox.Name = "NRLCheckBox"
        	Me.NRLCheckBox.Size = New System.Drawing.Size(48, 17)
        	Me.NRLCheckBox.TabIndex = 14
        	Me.NRLCheckBox.Text = "NRL"
        	Me.NRLCheckBox.UseVisualStyleBackColor = true
        	'
        	'V8CheckBox
        	'
        	Me.V8CheckBox.AutoSize = true
        	Me.V8CheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.V8CheckBox.Location = New System.Drawing.Point(165, 129)
        	Me.V8CheckBox.Name = "V8CheckBox"
        	Me.V8CheckBox.Size = New System.Drawing.Size(39, 17)
        	Me.V8CheckBox.TabIndex = 15
        	Me.V8CheckBox.Text = "V8"
        	Me.V8CheckBox.UseVisualStyleBackColor = true
        	'
        	'CrimeCheckBox
        	'
        	Me.CrimeCheckBox.AutoSize = true
        	Me.CrimeCheckBox.Location = New System.Drawing.Point(91, 19)
        	Me.CrimeCheckBox.Name = "CrimeCheckBox"
        	Me.CrimeCheckBox.Size = New System.Drawing.Size(52, 17)
        	Me.CrimeCheckBox.TabIndex = 16
        	Me.CrimeCheckBox.Text = "Crime"
        	Me.CrimeCheckBox.UseVisualStyleBackColor = true
        	'
        	'LawCheckBox
        	'
        	Me.LawCheckBox.AutoSize = true
        	Me.LawCheckBox.Location = New System.Drawing.Point(91, 41)
        	Me.LawCheckBox.Name = "LawCheckBox"
        	Me.LawCheckBox.Size = New System.Drawing.Size(46, 17)
        	Me.LawCheckBox.TabIndex = 17
        	Me.LawCheckBox.Text = "Law"
        	Me.LawCheckBox.UseVisualStyleBackColor = true
        	'
        	'MilitaryCheckBox
        	'
        	Me.MilitaryCheckBox.AutoSize = true
        	Me.MilitaryCheckBox.Location = New System.Drawing.Point(91, 63)
        	Me.MilitaryCheckBox.Name = "MilitaryCheckBox"
        	Me.MilitaryCheckBox.Size = New System.Drawing.Size(58, 17)
        	Me.MilitaryCheckBox.TabIndex = 18
        	Me.MilitaryCheckBox.Text = "Military"
        	Me.MilitaryCheckBox.UseVisualStyleBackColor = true
        	'
        	'PlaceCheckBox
        	'
        	Me.PlaceCheckBox.AutoSize = true
        	Me.PlaceCheckBox.Location = New System.Drawing.Point(91, 85)
        	Me.PlaceCheckBox.Name = "PlaceCheckBox"
        	Me.PlaceCheckBox.Size = New System.Drawing.Size(53, 17)
        	Me.PlaceCheckBox.TabIndex = 19
        	Me.PlaceCheckBox.Text = "Place"
        	Me.PlaceCheckBox.UseVisualStyleBackColor = true
        	'
        	'PoliticsCheckBox
        	'
        	Me.PoliticsCheckBox.AutoSize = true
        	Me.PoliticsCheckBox.Location = New System.Drawing.Point(91, 107)
        	Me.PoliticsCheckBox.Name = "PoliticsCheckBox"
        	Me.PoliticsCheckBox.Size = New System.Drawing.Size(59, 17)
        	Me.PoliticsCheckBox.TabIndex = 20
        	Me.PoliticsCheckBox.Text = "Politics"
        	Me.PoliticsCheckBox.UseVisualStyleBackColor = true
        	'
        	'SportCheckBox
        	'
        	Me.SportCheckBox.AutoSize = true
        	Me.SportCheckBox.Location = New System.Drawing.Point(165, 19)
        	Me.SportCheckBox.Name = "SportCheckBox"
        	Me.SportCheckBox.Size = New System.Drawing.Size(56, 17)
        	Me.SportCheckBox.TabIndex = 21
        	Me.SportCheckBox.Text = "Sports"
        	Me.SportCheckBox.UseVisualStyleBackColor = true
        	'
        	'TopicsGroupBox
        	'
        	Me.TopicsGroupBox.Controls.Add(Me.MusicCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.NBLCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.SportCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.PoliticsCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.PlaceCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.MilitaryCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.LawCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.CrimeCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.V8CheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.NRLCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.ALeagueCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.AFLCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.SydneyCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.PerthCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.MelbourneCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.LakeMacquarieCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.HobartCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.GeelongCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.CanberraCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.BrisbaneCheckBox)
        	Me.TopicsGroupBox.Controls.Add(Me.AdelaideCheckBox)
        	Me.TopicsGroupBox.Location = New System.Drawing.Point(11, 6)
        	Me.TopicsGroupBox.Name = "TopicsGroupBox"
        	Me.TopicsGroupBox.Size = New System.Drawing.Size(241, 244)
        	Me.TopicsGroupBox.TabIndex = 2
        	Me.TopicsGroupBox.TabStop = false
        	Me.TopicsGroupBox.Text = "Cities && Topics"
        	'
        	'MusicCheckBox
        	'
        	Me.MusicCheckBox.AutoSize = true
        	Me.MusicCheckBox.Location = New System.Drawing.Point(91, 130)
        	Me.MusicCheckBox.Name = "MusicCheckBox"
        	Me.MusicCheckBox.Size = New System.Drawing.Size(54, 17)
        	Me.MusicCheckBox.TabIndex = 23
        	Me.MusicCheckBox.Text = "Music"
        	Me.MusicCheckBox.UseVisualStyleBackColor = true
        	'
        	'NBLCheckBox
        	'
        	Me.NBLCheckBox.AutoSize = true
        	Me.NBLCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        	Me.NBLCheckBox.Location = New System.Drawing.Point(165, 85)
        	Me.NBLCheckBox.Name = "NBLCheckBox"
        	Me.NBLCheckBox.Size = New System.Drawing.Size(47, 17)
        	Me.NBLCheckBox.TabIndex = 22
        	Me.NBLCheckBox.Text = "NBL"
        	Me.NBLCheckBox.UseVisualStyleBackColor = true
        	'
        	'LinkLabel1
        	'
        	Me.LinkLabel1.AutoSize = true
        	Me.LinkLabel1.Location = New System.Drawing.Point(189, 276)
        	Me.LinkLabel1.Name = "LinkLabel1"
        	Me.LinkLabel1.Size = New System.Drawing.Size(120, 13)
        	Me.LinkLabel1.TabIndex = 9
        	Me.LinkLabel1.TabStop = true
        	Me.LinkLabel1.Text = "{{WikiProject Australia}}"
        	'
        	'WPAustraliaSettings
        	'
        	Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        	Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        	Me.Controls.Add(Me.LinkLabel1)
        	Me.Controls.Add(Me.ParametersGroup)
        	Me.Controls.Add(Me.TipLabel)
        	Me.Controls.Add(Me.TopicsGroupBox)
        	Me.MaximumSize = New System.Drawing.Size(276, 349)
        	Me.MinimumSize = New System.Drawing.Size(276, 349)
        	Me.Name = "WPAustraliaSettings"
        	Me.Size = New System.Drawing.Size(276, 349)
        	Me.ParametersGroup.ResumeLayout(false)
        	Me.ParametersGroup.PerformLayout
        	Me.TopicsGroupBox.ResumeLayout(false)
        	Me.TopicsGroupBox.PerformLayout
        	Me.ResumeLayout(false)
        	Me.PerformLayout
        End Sub
        Private WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Private WithEvents TipLabel As System.Windows.Forms.Label
        Private WithEvents ParametersGroup As System.Windows.Forms.GroupBox
        Private WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AdelaideCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents BrisbaneCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents CanberraCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents GeelongCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents HobartCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents LakeMacquarieCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MelbourneCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents PerthCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents SydneyCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents AFLCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents ALeagueCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents NRLCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents V8CheckBox As System.Windows.Forms.CheckBox
        Private WithEvents CrimeCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents LawCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MilitaryCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents PlaceCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents PoliticsCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents SportCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents TopicsGroupBox As System.Windows.Forms.GroupBox
        Private WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Private WithEvents NBLCheckBox As System.Windows.Forms.CheckBox
        Private WithEvents MusicCheckBox As System.Windows.Forms.CheckBox

    End Class
End Namespace