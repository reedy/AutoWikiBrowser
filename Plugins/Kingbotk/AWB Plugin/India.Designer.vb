Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Plugins
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class IndiaSettings
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
            Me.MaharashtraCheckBox = New System.Windows.Forms.CheckBox
            Me.TipLabel = New System.Windows.Forms.Label
            Me.ParametersGroup = New System.Windows.Forms.GroupBox
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
            Me.TabControl1 = New System.Windows.Forms.TabControl
            Me.TabPage1 = New System.Windows.Forms.TabPage
            Me.PunjabCheckbox = New System.Windows.Forms.CheckBox
            Me.HimachalCheckBox = New System.Windows.Forms.CheckBox
            Me.WBengalCheckBox = New System.Windows.Forms.CheckBox
            Me.MapsCheckBox = New System.Windows.Forms.CheckBox
            Me.GeogCheckBox = New System.Windows.Forms.CheckBox
            Me.DistrictsCheckBox = New System.Windows.Forms.CheckBox
            Me.CitiesCheckBox = New System.Windows.Forms.CheckBox
            Me.StatesCheckBox = New System.Windows.Forms.CheckBox
            Me.TamilnaduCheckBox = New System.Windows.Forms.CheckBox
            Me.KeralaCheckBox = New System.Windows.Forms.CheckBox
            Me.KarnatakaCheckBox = New System.Windows.Forms.CheckBox
            Me.GoaCheckBox = New System.Windows.Forms.CheckBox
            Me.BengalCheckBox = New System.Windows.Forms.CheckBox
            Me.AndhraCheckBox = New System.Windows.Forms.CheckBox
            Me.TabPage2 = New System.Windows.Forms.TabPage
            Me.ProtectedCheckBox = New System.Windows.Forms.CheckBox
            Me.LiteratureCheckBox = New System.Windows.Forms.CheckBox
            Me.TamilCheckBox = New System.Windows.Forms.CheckBox
            Me.PoliticsCheckBox = New System.Windows.Forms.CheckBox
            Me.CinemaCheckBox = New System.Windows.Forms.CheckBox
            Me.HistoryCheckBox = New System.Windows.Forms.CheckBox
            Me.ParametersGroup.SuspendLayout()
            Me.TabControl1.SuspendLayout()
            Me.TabPage1.SuspendLayout()
            Me.TabPage2.SuspendLayout()
            Me.SuspendLayout()
            '
            'TextInsertContextMenuStrip
            '
            Me.TextInsertContextMenuStrip.Name = "TextInsertContextMenuStrip"
            Me.TextInsertContextMenuStrip.Size = New System.Drawing.Size(61, 4)
            '
            'StubClassCheckBox
            '
            Me.StubClassCheckBox.AutoSize = True
            Me.StubClassCheckBox.Location = New System.Drawing.Point(9, 19)
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
            Me.AutoStubCheckBox.Location = New System.Drawing.Point(91, 19)
            Me.AutoStubCheckBox.Name = "AutoStubCheckBox"
            Me.AutoStubCheckBox.Size = New System.Drawing.Size(73, 17)
            Me.AutoStubCheckBox.TabIndex = 4
            Me.AutoStubCheckBox.Text = "Auto-Stub"
            Me.ToolTip1.SetToolTip(Me.AutoStubCheckBox, "class=Stub|auto=yes")
            Me.AutoStubCheckBox.UseVisualStyleBackColor = True
            '
            'MaharashtraCheckBox
            '
            Me.MaharashtraCheckBox.AutoSize = True
            Me.MaharashtraCheckBox.Location = New System.Drawing.Point(104, 37)
            Me.MaharashtraCheckBox.Name = "MaharashtraCheckBox"
            Me.MaharashtraCheckBox.Size = New System.Drawing.Size(85, 17)
            Me.MaharashtraCheckBox.TabIndex = 28
            Me.MaharashtraCheckBox.Text = "Maharashtra"
            Me.ToolTip1.SetToolTip(Me.MaharashtraCheckBox, "Lake Macquarie")
            Me.MaharashtraCheckBox.UseVisualStyleBackColor = True
            '
            'TipLabel
            '
            Me.TipLabel.AutoSize = True
            Me.TipLabel.Location = New System.Drawing.Point(8, 306)
            Me.TipLabel.Name = "TipLabel"
            Me.TipLabel.Size = New System.Drawing.Size(255, 39)
            Me.TipLabel.TabIndex = 7
            Me.TipLabel.Text = "Tip: The plugin also adds parameter insertion options" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to the context menu of the" & _
                " edit box. Just right" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "click inside the edit box to access them."
            Me.TipLabel.Visible = False
            '
            'ParametersGroup
            '
            Me.ParametersGroup.Controls.Add(Me.AutoStubCheckBox)
            Me.ParametersGroup.Controls.Add(Me.StubClassCheckBox)
            Me.ParametersGroup.Location = New System.Drawing.Point(11, 256)
            Me.ParametersGroup.Name = "ParametersGroup"
            Me.ParametersGroup.Size = New System.Drawing.Size(172, 44)
            Me.ParametersGroup.TabIndex = 8
            Me.ParametersGroup.TabStop = False
            Me.ParametersGroup.Text = "Template Parameters"
            '
            'LinkLabel1
            '
            Me.LinkLabel1.AutoSize = True
            Me.LinkLabel1.Location = New System.Drawing.Point(189, 276)
            Me.LinkLabel1.Name = "LinkLabel1"
            Me.LinkLabel1.Size = New System.Drawing.Size(67, 13)
            Me.LinkLabel1.TabIndex = 9
            Me.LinkLabel1.TabStop = True
            Me.LinkLabel1.Text = "{{WP India}}"
            '
            'TabControl1
            '
            Me.TabControl1.Controls.Add(Me.TabPage1)
            Me.TabControl1.Controls.Add(Me.TabPage2)
            Me.TabControl1.Location = New System.Drawing.Point(11, 14)
            Me.TabControl1.Name = "TabControl1"
            Me.TabControl1.SelectedIndex = 0
            Me.TabControl1.Size = New System.Drawing.Size(252, 236)
            Me.TabControl1.TabIndex = 10
            '
            'TabPage1
            '
            Me.TabPage1.BackColor = System.Drawing.Color.Transparent
            Me.TabPage1.Controls.Add(Me.PunjabCheckbox)
            Me.TabPage1.Controls.Add(Me.HimachalCheckBox)
            Me.TabPage1.Controls.Add(Me.WBengalCheckBox)
            Me.TabPage1.Controls.Add(Me.MapsCheckBox)
            Me.TabPage1.Controls.Add(Me.GeogCheckBox)
            Me.TabPage1.Controls.Add(Me.DistrictsCheckBox)
            Me.TabPage1.Controls.Add(Me.CitiesCheckBox)
            Me.TabPage1.Controls.Add(Me.StatesCheckBox)
            Me.TabPage1.Controls.Add(Me.TamilnaduCheckBox)
            Me.TabPage1.Controls.Add(Me.MaharashtraCheckBox)
            Me.TabPage1.Controls.Add(Me.KeralaCheckBox)
            Me.TabPage1.Controls.Add(Me.KarnatakaCheckBox)
            Me.TabPage1.Controls.Add(Me.GoaCheckBox)
            Me.TabPage1.Controls.Add(Me.BengalCheckBox)
            Me.TabPage1.Controls.Add(Me.AndhraCheckBox)
            Me.TabPage1.Location = New System.Drawing.Point(4, 22)
            Me.TabPage1.Name = "TabPage1"
            Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage1.Size = New System.Drawing.Size(244, 210)
            Me.TabPage1.TabIndex = 0
            Me.TabPage1.Text = "India 1"
            '
            'PunjabCheckbox
            '
            Me.PunjabCheckbox.AutoSize = True
            Me.PunjabCheckbox.Location = New System.Drawing.Point(104, 174)
            Me.PunjabCheckbox.Name = "PunjabCheckbox"
            Me.PunjabCheckbox.Size = New System.Drawing.Size(59, 17)
            Me.PunjabCheckbox.TabIndex = 40
            Me.PunjabCheckbox.Text = "Punjab"
            Me.PunjabCheckbox.UseVisualStyleBackColor = True
            '
            'HimachalCheckBox
            '
            Me.HimachalCheckBox.AutoSize = True
            Me.HimachalCheckBox.Location = New System.Drawing.Point(104, 152)
            Me.HimachalCheckBox.Name = "HimachalCheckBox"
            Me.HimachalCheckBox.Size = New System.Drawing.Size(112, 17)
            Me.HimachalCheckBox.TabIndex = 39
            Me.HimachalCheckBox.Text = "Himachal Pradesh"
            Me.HimachalCheckBox.UseVisualStyleBackColor = True
            '
            'WBengalCheckBox
            '
            Me.WBengalCheckBox.AutoSize = True
            Me.WBengalCheckBox.Location = New System.Drawing.Point(104, 106)
            Me.WBengalCheckBox.Name = "WBengalCheckBox"
            Me.WBengalCheckBox.Size = New System.Drawing.Size(87, 17)
            Me.WBengalCheckBox.TabIndex = 38
            Me.WBengalCheckBox.Text = "West Bengal"
            Me.WBengalCheckBox.UseVisualStyleBackColor = True
            '
            'MapsCheckBox
            '
            Me.MapsCheckBox.AutoSize = True
            Me.MapsCheckBox.Location = New System.Drawing.Point(14, 39)
            Me.MapsCheckBox.Name = "MapsCheckBox"
            Me.MapsCheckBox.Size = New System.Drawing.Size(52, 17)
            Me.MapsCheckBox.TabIndex = 37
            Me.MapsCheckBox.Text = "Maps"
            Me.MapsCheckBox.UseVisualStyleBackColor = True
            '
            'GeogCheckBox
            '
            Me.GeogCheckBox.AutoSize = True
            Me.GeogCheckBox.Location = New System.Drawing.Point(14, 17)
            Me.GeogCheckBox.Name = "GeogCheckBox"
            Me.GeogCheckBox.Size = New System.Drawing.Size(78, 17)
            Me.GeogCheckBox.TabIndex = 36
            Me.GeogCheckBox.Text = "Geography"
            Me.GeogCheckBox.UseVisualStyleBackColor = True
            '
            'DistrictsCheckBox
            '
            Me.DistrictsCheckBox.AutoSize = True
            Me.DistrictsCheckBox.Location = New System.Drawing.Point(14, 62)
            Me.DistrictsCheckBox.Name = "DistrictsCheckBox"
            Me.DistrictsCheckBox.Size = New System.Drawing.Size(63, 17)
            Me.DistrictsCheckBox.TabIndex = 35
            Me.DistrictsCheckBox.Text = "Districts"
            Me.DistrictsCheckBox.UseVisualStyleBackColor = True
            '
            'CitiesCheckBox
            '
            Me.CitiesCheckBox.AutoSize = True
            Me.CitiesCheckBox.Location = New System.Drawing.Point(14, 82)
            Me.CitiesCheckBox.Name = "CitiesCheckBox"
            Me.CitiesCheckBox.Size = New System.Drawing.Size(51, 17)
            Me.CitiesCheckBox.TabIndex = 34
            Me.CitiesCheckBox.Text = "Cities"
            Me.CitiesCheckBox.UseVisualStyleBackColor = True
            '
            'StatesCheckBox
            '
            Me.StatesCheckBox.AutoSize = True
            Me.StatesCheckBox.Location = New System.Drawing.Point(14, 105)
            Me.StatesCheckBox.Name = "StatesCheckBox"
            Me.StatesCheckBox.Size = New System.Drawing.Size(56, 17)
            Me.StatesCheckBox.TabIndex = 30
            Me.StatesCheckBox.Text = "States"
            Me.StatesCheckBox.UseVisualStyleBackColor = True
            '
            'TamilnaduCheckBox
            '
            Me.TamilnaduCheckBox.AutoSize = True
            Me.TamilnaduCheckBox.Location = New System.Drawing.Point(104, 83)
            Me.TamilnaduCheckBox.Name = "TamilnaduCheckBox"
            Me.TamilnaduCheckBox.Size = New System.Drawing.Size(75, 17)
            Me.TamilnaduCheckBox.TabIndex = 29
            Me.TamilnaduCheckBox.Text = "Tamilnadu"
            Me.TamilnaduCheckBox.UseVisualStyleBackColor = True
            '
            'KeralaCheckBox
            '
            Me.KeralaCheckBox.AutoSize = True
            Me.KeralaCheckBox.Location = New System.Drawing.Point(104, 17)
            Me.KeralaCheckBox.Name = "KeralaCheckBox"
            Me.KeralaCheckBox.Size = New System.Drawing.Size(56, 17)
            Me.KeralaCheckBox.TabIndex = 27
            Me.KeralaCheckBox.Text = "Kerala"
            Me.KeralaCheckBox.UseVisualStyleBackColor = True
            '
            'KarnatakaCheckBox
            '
            Me.KarnatakaCheckBox.AutoSize = True
            Me.KarnatakaCheckBox.Location = New System.Drawing.Point(14, 174)
            Me.KarnatakaCheckBox.Name = "KarnatakaCheckBox"
            Me.KarnatakaCheckBox.Size = New System.Drawing.Size(75, 17)
            Me.KarnatakaCheckBox.TabIndex = 26
            Me.KarnatakaCheckBox.Text = "Karnataka"
            Me.KarnatakaCheckBox.UseVisualStyleBackColor = True
            '
            'GoaCheckBox
            '
            Me.GoaCheckBox.AutoSize = True
            Me.GoaCheckBox.Location = New System.Drawing.Point(14, 151)
            Me.GoaCheckBox.Name = "GoaCheckBox"
            Me.GoaCheckBox.Size = New System.Drawing.Size(46, 17)
            Me.GoaCheckBox.TabIndex = 25
            Me.GoaCheckBox.Text = "Goa"
            Me.GoaCheckBox.UseVisualStyleBackColor = True
            '
            'BengalCheckBox
            '
            Me.BengalCheckBox.AutoSize = True
            Me.BengalCheckBox.Location = New System.Drawing.Point(104, 129)
            Me.BengalCheckBox.Name = "BengalCheckBox"
            Me.BengalCheckBox.Size = New System.Drawing.Size(59, 17)
            Me.BengalCheckBox.TabIndex = 24
            Me.BengalCheckBox.Text = "Bengal"
            Me.BengalCheckBox.UseVisualStyleBackColor = True
            '
            'AndhraCheckBox
            '
            Me.AndhraCheckBox.AutoSize = True
            Me.AndhraCheckBox.Location = New System.Drawing.Point(104, 60)
            Me.AndhraCheckBox.Name = "AndhraCheckBox"
            Me.AndhraCheckBox.Size = New System.Drawing.Size(102, 17)
            Me.AndhraCheckBox.TabIndex = 23
            Me.AndhraCheckBox.Text = "Andhra Pradesh"
            Me.AndhraCheckBox.UseVisualStyleBackColor = True
            '
            'TabPage2
            '
            Me.TabPage2.BackColor = System.Drawing.Color.Transparent
            Me.TabPage2.Controls.Add(Me.ProtectedCheckBox)
            Me.TabPage2.Controls.Add(Me.LiteratureCheckBox)
            Me.TabPage2.Controls.Add(Me.TamilCheckBox)
            Me.TabPage2.Controls.Add(Me.PoliticsCheckBox)
            Me.TabPage2.Controls.Add(Me.CinemaCheckBox)
            Me.TabPage2.Controls.Add(Me.HistoryCheckBox)
            Me.TabPage2.Location = New System.Drawing.Point(4, 22)
            Me.TabPage2.Name = "TabPage2"
            Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
            Me.TabPage2.Size = New System.Drawing.Size(244, 210)
            Me.TabPage2.TabIndex = 1
            Me.TabPage2.Text = "India 2"
            '
            'ProtectedCheckBox
            '
            Me.ProtectedCheckBox.AutoSize = True
            Me.ProtectedCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.ProtectedCheckBox.Location = New System.Drawing.Point(6, 29)
            Me.ProtectedCheckBox.Name = "ProtectedCheckBox"
            Me.ProtectedCheckBox.Size = New System.Drawing.Size(139, 17)
            Me.ProtectedCheckBox.TabIndex = 43
            Me.ProtectedCheckBox.Text = "Protected areas of India"
            Me.ProtectedCheckBox.UseVisualStyleBackColor = True
            '
            'LiteratureCheckBox
            '
            Me.LiteratureCheckBox.AutoSize = True
            Me.LiteratureCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LiteratureCheckBox.Location = New System.Drawing.Point(73, 6)
            Me.LiteratureCheckBox.Name = "LiteratureCheckBox"
            Me.LiteratureCheckBox.Size = New System.Drawing.Size(70, 17)
            Me.LiteratureCheckBox.TabIndex = 42
            Me.LiteratureCheckBox.Text = "Literature"
            Me.LiteratureCheckBox.UseVisualStyleBackColor = True
            '
            'TamilCheckBox
            '
            Me.TamilCheckBox.AutoSize = True
            Me.TamilCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TamilCheckBox.Location = New System.Drawing.Point(71, 75)
            Me.TamilCheckBox.Name = "TamilCheckBox"
            Me.TamilCheckBox.Size = New System.Drawing.Size(51, 17)
            Me.TamilCheckBox.TabIndex = 41
            Me.TamilCheckBox.Text = "Tamil"
            Me.TamilCheckBox.UseVisualStyleBackColor = True
            '
            'PoliticsCheckBox
            '
            Me.PoliticsCheckBox.AutoSize = True
            Me.PoliticsCheckBox.Location = New System.Drawing.Point(6, 75)
            Me.PoliticsCheckBox.Name = "PoliticsCheckBox"
            Me.PoliticsCheckBox.Size = New System.Drawing.Size(59, 17)
            Me.PoliticsCheckBox.TabIndex = 40
            Me.PoliticsCheckBox.Text = "Politics"
            Me.PoliticsCheckBox.UseVisualStyleBackColor = True
            '
            'CinemaCheckBox
            '
            Me.CinemaCheckBox.AutoSize = True
            Me.CinemaCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CinemaCheckBox.Location = New System.Drawing.Point(6, 6)
            Me.CinemaCheckBox.Name = "CinemaCheckBox"
            Me.CinemaCheckBox.Size = New System.Drawing.Size(61, 17)
            Me.CinemaCheckBox.TabIndex = 39
            Me.CinemaCheckBox.Text = "Cinema"
            Me.CinemaCheckBox.UseVisualStyleBackColor = True
            '
            'HistoryCheckBox
            '
            Me.HistoryCheckBox.AutoSize = True
            Me.HistoryCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.HistoryCheckBox.Location = New System.Drawing.Point(5, 52)
            Me.HistoryCheckBox.Name = "HistoryCheckBox"
            Me.HistoryCheckBox.Size = New System.Drawing.Size(58, 17)
            Me.HistoryCheckBox.TabIndex = 38
            Me.HistoryCheckBox.Text = "History"
            Me.HistoryCheckBox.UseVisualStyleBackColor = True
            '
            'IndiaSettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.ParametersGroup)
            Me.Controls.Add(Me.TabControl1)
            Me.Controls.Add(Me.TipLabel)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "IndiaSettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.ParametersGroup.ResumeLayout(False)
            Me.ParametersGroup.PerformLayout()
            Me.TabControl1.ResumeLayout(False)
            Me.TabPage1.ResumeLayout(False)
            Me.TabPage1.PerformLayout()
            Me.TabPage2.ResumeLayout(False)
            Me.TabPage2.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents TipLabel As System.Windows.Forms.Label
        Friend WithEvents ParametersGroup As System.Windows.Forms.GroupBox
        Friend WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
        Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
        Friend WithEvents DistrictsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents CitiesCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents StatesCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents TamilnaduCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents MaharashtraCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents KeralaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents KarnatakaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents GoaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents BengalCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents AndhraCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
        Friend WithEvents TamilCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents PoliticsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents CinemaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents HistoryCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents MapsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents GeogCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents HimachalCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents WBengalCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents LiteratureCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents ProtectedCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents PunjabCheckbox As System.Windows.Forms.CheckBox

    End Class
End Namespace