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
            Me.AndhraCheckBox = New System.Windows.Forms.CheckBox
            Me.BengalCheckBox = New System.Windows.Forms.CheckBox
            Me.GoaCheckBox = New System.Windows.Forms.CheckBox
            Me.KarnatakaCheckBox = New System.Windows.Forms.CheckBox
            Me.KeralaCheckBox = New System.Windows.Forms.CheckBox
            Me.TamilnaduCheckBox = New System.Windows.Forms.CheckBox
            Me.StatesCheckBox = New System.Windows.Forms.CheckBox
            Me.HistoryCheckBox = New System.Windows.Forms.CheckBox
            Me.CinemaCheckBox = New System.Windows.Forms.CheckBox
            Me.MapsCheckBox = New System.Windows.Forms.CheckBox
            Me.CitiesCheckBox = New System.Windows.Forms.CheckBox
            Me.DistrictsCheckBox = New System.Windows.Forms.CheckBox
            Me.PoliticsCheckBox = New System.Windows.Forms.CheckBox
            Me.TopicsGroupBox = New System.Windows.Forms.GroupBox
            Me.LinkLabel1 = New System.Windows.Forms.LinkLabel
            Me.TamilCheckBox = New System.Windows.Forms.CheckBox
            Me.ParametersGroup.SuspendLayout()
            Me.TopicsGroupBox.SuspendLayout()
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
            Me.MaharashtraCheckBox.Location = New System.Drawing.Point(82, 151)
            Me.MaharashtraCheckBox.Name = "MaharashtraCheckBox"
            Me.MaharashtraCheckBox.Size = New System.Drawing.Size(85, 17)
            Me.MaharashtraCheckBox.TabIndex = 5
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
            'AndhraCheckBox
            '
            Me.AndhraCheckBox.AutoSize = True
            Me.AndhraCheckBox.Location = New System.Drawing.Point(82, 41)
            Me.AndhraCheckBox.Name = "AndhraCheckBox"
            Me.AndhraCheckBox.Size = New System.Drawing.Size(60, 17)
            Me.AndhraCheckBox.TabIndex = 0
            Me.AndhraCheckBox.Text = "Andhra"
            Me.AndhraCheckBox.UseVisualStyleBackColor = True
            '
            'BengalCheckBox
            '
            Me.BengalCheckBox.AutoSize = True
            Me.BengalCheckBox.Location = New System.Drawing.Point(82, 63)
            Me.BengalCheckBox.Name = "BengalCheckBox"
            Me.BengalCheckBox.Size = New System.Drawing.Size(59, 17)
            Me.BengalCheckBox.TabIndex = 1
            Me.BengalCheckBox.Text = "Bengal"
            Me.BengalCheckBox.UseVisualStyleBackColor = True
            '
            'GoaCheckBox
            '
            Me.GoaCheckBox.AutoSize = True
            Me.GoaCheckBox.Location = New System.Drawing.Point(82, 85)
            Me.GoaCheckBox.Name = "GoaCheckBox"
            Me.GoaCheckBox.Size = New System.Drawing.Size(46, 17)
            Me.GoaCheckBox.TabIndex = 2
            Me.GoaCheckBox.Text = "Goa"
            Me.GoaCheckBox.UseVisualStyleBackColor = True
            '
            'KarnatakaCheckBox
            '
            Me.KarnatakaCheckBox.AutoSize = True
            Me.KarnatakaCheckBox.Location = New System.Drawing.Point(82, 107)
            Me.KarnatakaCheckBox.Name = "KarnatakaCheckBox"
            Me.KarnatakaCheckBox.Size = New System.Drawing.Size(75, 17)
            Me.KarnatakaCheckBox.TabIndex = 3
            Me.KarnatakaCheckBox.Text = "Karnataka"
            Me.KarnatakaCheckBox.UseVisualStyleBackColor = True
            '
            'KeralaCheckBox
            '
            Me.KeralaCheckBox.AutoSize = True
            Me.KeralaCheckBox.Location = New System.Drawing.Point(82, 129)
            Me.KeralaCheckBox.Name = "KeralaCheckBox"
            Me.KeralaCheckBox.Size = New System.Drawing.Size(56, 17)
            Me.KeralaCheckBox.TabIndex = 4
            Me.KeralaCheckBox.Text = "Kerala"
            Me.KeralaCheckBox.UseVisualStyleBackColor = True
            '
            'TamilnaduCheckBox
            '
            Me.TamilnaduCheckBox.AutoSize = True
            Me.TamilnaduCheckBox.Location = New System.Drawing.Point(82, 173)
            Me.TamilnaduCheckBox.Name = "TamilnaduCheckBox"
            Me.TamilnaduCheckBox.Size = New System.Drawing.Size(75, 17)
            Me.TamilnaduCheckBox.TabIndex = 6
            Me.TamilnaduCheckBox.Text = "Tamilnadu"
            Me.TamilnaduCheckBox.UseVisualStyleBackColor = True
            '
            'StatesCheckBox
            '
            Me.StatesCheckBox.AutoSize = True
            Me.StatesCheckBox.Location = New System.Drawing.Point(82, 19)
            Me.StatesCheckBox.Name = "StatesCheckBox"
            Me.StatesCheckBox.Size = New System.Drawing.Size(56, 17)
            Me.StatesCheckBox.TabIndex = 7
            Me.StatesCheckBox.Text = "States"
            Me.StatesCheckBox.UseVisualStyleBackColor = True
            '
            'HistoryCheckBox
            '
            Me.HistoryCheckBox.AutoSize = True
            Me.HistoryCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.HistoryCheckBox.Location = New System.Drawing.Point(165, 41)
            Me.HistoryCheckBox.Name = "HistoryCheckBox"
            Me.HistoryCheckBox.Size = New System.Drawing.Size(58, 17)
            Me.HistoryCheckBox.TabIndex = 12
            Me.HistoryCheckBox.Text = "History"
            Me.HistoryCheckBox.UseVisualStyleBackColor = True
            '
            'CinemaCheckBox
            '
            Me.CinemaCheckBox.AutoSize = True
            Me.CinemaCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.CinemaCheckBox.Location = New System.Drawing.Point(165, 63)
            Me.CinemaCheckBox.Name = "CinemaCheckBox"
            Me.CinemaCheckBox.Size = New System.Drawing.Size(61, 17)
            Me.CinemaCheckBox.TabIndex = 13
            Me.CinemaCheckBox.Text = "Cinema"
            Me.CinemaCheckBox.UseVisualStyleBackColor = True
            '
            'MapsCheckBox
            '
            Me.MapsCheckBox.AutoSize = True
            Me.MapsCheckBox.Location = New System.Drawing.Point(9, 19)
            Me.MapsCheckBox.Name = "MapsCheckBox"
            Me.MapsCheckBox.Size = New System.Drawing.Size(52, 17)
            Me.MapsCheckBox.TabIndex = 16
            Me.MapsCheckBox.Text = "Maps"
            Me.MapsCheckBox.UseVisualStyleBackColor = True
            '
            'CitiesCheckBox
            '
            Me.CitiesCheckBox.AutoSize = True
            Me.CitiesCheckBox.Location = New System.Drawing.Point(9, 41)
            Me.CitiesCheckBox.Name = "CitiesCheckBox"
            Me.CitiesCheckBox.Size = New System.Drawing.Size(51, 17)
            Me.CitiesCheckBox.TabIndex = 17
            Me.CitiesCheckBox.Text = "Cities"
            Me.CitiesCheckBox.UseVisualStyleBackColor = True
            '
            'DistrictsCheckBox
            '
            Me.DistrictsCheckBox.AutoSize = True
            Me.DistrictsCheckBox.Location = New System.Drawing.Point(9, 63)
            Me.DistrictsCheckBox.Name = "DistrictsCheckBox"
            Me.DistrictsCheckBox.Size = New System.Drawing.Size(63, 17)
            Me.DistrictsCheckBox.TabIndex = 18
            Me.DistrictsCheckBox.Text = "Districts"
            Me.DistrictsCheckBox.UseVisualStyleBackColor = True
            '
            'PoliticsCheckBox
            '
            Me.PoliticsCheckBox.AutoSize = True
            Me.PoliticsCheckBox.Location = New System.Drawing.Point(165, 19)
            Me.PoliticsCheckBox.Name = "PoliticsCheckBox"
            Me.PoliticsCheckBox.Size = New System.Drawing.Size(59, 17)
            Me.PoliticsCheckBox.TabIndex = 21
            Me.PoliticsCheckBox.Text = "Politics"
            Me.PoliticsCheckBox.UseVisualStyleBackColor = True
            '
            'TopicsGroupBox
            '
            Me.TopicsGroupBox.Controls.Add(Me.TamilCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.PoliticsCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.DistrictsCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.CitiesCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.MapsCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.CinemaCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.HistoryCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.StatesCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.TamilnaduCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.MaharashtraCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.KeralaCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.KarnatakaCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.GoaCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.BengalCheckBox)
            Me.TopicsGroupBox.Controls.Add(Me.AndhraCheckBox)
            Me.TopicsGroupBox.Location = New System.Drawing.Point(11, 6)
            Me.TopicsGroupBox.Name = "TopicsGroupBox"
            Me.TopicsGroupBox.Size = New System.Drawing.Size(241, 244)
            Me.TopicsGroupBox.TabIndex = 2
            Me.TopicsGroupBox.TabStop = False
            Me.TopicsGroupBox.Text = "Cities && Topics"
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
            'TamilCheckBox
            '
            Me.TamilCheckBox.AutoSize = True
            Me.TamilCheckBox.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TamilCheckBox.Location = New System.Drawing.Point(165, 85)
            Me.TamilCheckBox.Name = "TamilCheckBox"
            Me.TamilCheckBox.Size = New System.Drawing.Size(51, 17)
            Me.TamilCheckBox.TabIndex = 22
            Me.TamilCheckBox.Text = "Tamil"
            Me.TamilCheckBox.UseVisualStyleBackColor = True
            '
            'IndiaSettings
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.LinkLabel1)
            Me.Controls.Add(Me.ParametersGroup)
            Me.Controls.Add(Me.TipLabel)
            Me.Controls.Add(Me.TopicsGroupBox)
            Me.MaximumSize = New System.Drawing.Size(276, 349)
            Me.MinimumSize = New System.Drawing.Size(276, 349)
            Me.Name = "IndiaSettings"
            Me.Size = New System.Drawing.Size(276, 349)
            Me.ParametersGroup.ResumeLayout(False)
            Me.ParametersGroup.PerformLayout()
            Me.TopicsGroupBox.ResumeLayout(False)
            Me.TopicsGroupBox.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents TextInsertContextMenuStrip As System.Windows.Forms.ContextMenuStrip
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
        Friend WithEvents TipLabel As System.Windows.Forms.Label
        Friend WithEvents ParametersGroup As System.Windows.Forms.GroupBox
        Friend WithEvents StubClassCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents AutoStubCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents AndhraCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents BengalCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents GoaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents KarnatakaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents KeralaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents MaharashtraCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents TamilnaduCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents StatesCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents HistoryCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents CinemaCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents MapsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents CitiesCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents DistrictsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents PoliticsCheckBox As System.Windows.Forms.CheckBox
        Friend WithEvents TopicsGroupBox As System.Windows.Forms.GroupBox
        Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
        Friend WithEvents TamilCheckBox As System.Windows.Forms.CheckBox

    End Class
End Namespace