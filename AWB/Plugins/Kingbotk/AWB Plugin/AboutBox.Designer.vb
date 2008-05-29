Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class AboutBox
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
            Me.OKButton = New System.Windows.Forms.Button
            Me.LabelProductName = New System.Windows.Forms.Label
            Me.LabelVersion = New System.Windows.Forms.Label
            Me.LabelCopyright = New System.Windows.Forms.Label
            Me.TextBoxDescription = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.linkKingboy = New System.Windows.Forms.LinkLabel
            Me.Label2 = New System.Windows.Forms.Label
            Me.linkReedy = New System.Windows.Forms.LinkLabel
            Me.LicencingButton = New System.Windows.Forms.Button
            Me.SuspendLayout()
            '
            'OKButton
            '
            Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.OKButton.Location = New System.Drawing.Point(348, 16)
            Me.OKButton.Name = "OKButton"
            Me.OKButton.Size = New System.Drawing.Size(87, 23)
            Me.OKButton.TabIndex = 0
            Me.OKButton.Text = "&OK"
            '
            'LabelProductName
            '
            Me.LabelProductName.AutoSize = True
            Me.LabelProductName.BackColor = System.Drawing.Color.Transparent
            Me.LabelProductName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LabelProductName.Location = New System.Drawing.Point(10, 21)
            Me.LabelProductName.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.LabelProductName.MaximumSize = New System.Drawing.Size(0, 17)
            Me.LabelProductName.Name = "LabelProductName"
            Me.LabelProductName.Size = New System.Drawing.Size(221, 13)
            Me.LabelProductName.TabIndex = 4
            Me.LabelProductName.Text = "About the Kingbotk Templating Plugin"
            Me.LabelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'LabelVersion
            '
            Me.LabelVersion.AutoSize = True
            Me.LabelVersion.BackColor = System.Drawing.Color.Transparent
            Me.LabelVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LabelVersion.Location = New System.Drawing.Point(10, 54)
            Me.LabelVersion.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.LabelVersion.MaximumSize = New System.Drawing.Size(0, 17)
            Me.LabelVersion.Name = "LabelVersion"
            Me.LabelVersion.Size = New System.Drawing.Size(49, 13)
            Me.LabelVersion.TabIndex = 5
            Me.LabelVersion.Text = "Version"
            Me.LabelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'LabelCopyright
            '
            Me.LabelCopyright.AutoSize = True
            Me.LabelCopyright.BackColor = System.Drawing.Color.Transparent
            Me.LabelCopyright.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LabelCopyright.Location = New System.Drawing.Point(10, 88)
            Me.LabelCopyright.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.LabelCopyright.MaximumSize = New System.Drawing.Size(0, 17)
            Me.LabelCopyright.Name = "LabelCopyright"
            Me.LabelCopyright.Size = New System.Drawing.Size(189, 13)
            Me.LabelCopyright.TabIndex = 2
            Me.LabelCopyright.Text = "Copyright © SDK Software 2008"
            Me.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'TextBoxDescription
            '
            Me.TextBoxDescription.BackColor = System.Drawing.Color.Transparent
            Me.TextBoxDescription.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TextBoxDescription.Location = New System.Drawing.Point(10, 151)
            Me.TextBoxDescription.Name = "TextBoxDescription"
            Me.TextBoxDescription.Size = New System.Drawing.Size(366, 112)
            Me.TextBoxDescription.TabIndex = 6
            Me.TextBoxDescription.Text = "Disclaimer"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.BackColor = System.Drawing.Color.Transparent
            Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label1.Location = New System.Drawing.Point(357, 88)
            Me.Label1.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.Label1.MaximumSize = New System.Drawing.Size(0, 17)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(65, 13)
            Me.Label1.TabIndex = 7
            Me.Label1.Text = "Written by"
            Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'linkKingboy
            '
            Me.linkKingboy.AutoSize = True
            Me.linkKingboy.BackColor = System.Drawing.Color.Transparent
            Me.linkKingboy.Location = New System.Drawing.Point(343, 120)
            Me.linkKingboy.Name = "linkKingboy"
            Me.linkKingboy.Size = New System.Drawing.Size(40, 13)
            Me.linkKingboy.TabIndex = 47
            Me.linkKingboy.TabStop = True
            Me.linkKingboy.Text = "Steve"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.BackColor = System.Drawing.Color.Transparent
            Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Label2.Location = New System.Drawing.Point(349, 151)
            Me.Label2.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.Label2.MaximumSize = New System.Drawing.Size(0, 17)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(86, 13)
            Me.Label2.TabIndex = 48
            Me.Label2.Text = "with help from"
            Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'linkReedy
            '
            Me.linkReedy.AutoSize = True
            Me.linkReedy.BackColor = System.Drawing.Color.Transparent
            Me.linkReedy.Location = New System.Drawing.Point(382, 177)
            Me.linkReedy.Name = "linkReedy"
            Me.linkReedy.Size = New System.Drawing.Size(31, 13)
            Me.linkReedy.TabIndex = 49
            Me.linkReedy.TabStop = True
            Me.linkReedy.Text = "Sam"
            '
            'LicencingButton
            '
            Me.LicencingButton.Location = New System.Drawing.Point(348, 45)
            Me.LicencingButton.Name = "LicencingButton"
            Me.LicencingButton.Size = New System.Drawing.Size(87, 23)
            Me.LicencingButton.TabIndex = 50
            Me.LicencingButton.Text = "Licensing"
            Me.LicencingButton.UseVisualStyleBackColor = True
            '
            'AboutBox
            '
            Me.AcceptButton = Me.OKButton
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackgroundImage = Global.My.Resources.Resources.king_worship1
            Me.ClientSize = New System.Drawing.Size(448, 262)
            Me.Controls.Add(Me.LicencingButton)
            Me.Controls.Add(Me.linkReedy)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.linkKingboy)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.TextBoxDescription)
            Me.Controls.Add(Me.LabelProductName)
            Me.Controls.Add(Me.LabelVersion)
            Me.Controls.Add(Me.LabelCopyright)
            Me.Controls.Add(Me.OKButton)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AboutBox"
            Me.Padding = New System.Windows.Forms.Padding(10, 9, 10, 9)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "About the Kingbotk Templating Plugin"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents linkKingboy As System.Windows.Forms.LinkLabel
        Private WithEvents linkReedy As System.Windows.Forms.LinkLabel
        Private WithEvents OKButton As System.Windows.Forms.Button
        Private WithEvents LabelProductName As System.Windows.Forms.Label
        Private WithEvents LabelVersion As System.Windows.Forms.Label
        Private WithEvents LabelCopyright As System.Windows.Forms.Label
        Private WithEvents TextBoxDescription As System.Windows.Forms.Label
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents LicencingButton As System.Windows.Forms.Button

    End Class
End Namespace