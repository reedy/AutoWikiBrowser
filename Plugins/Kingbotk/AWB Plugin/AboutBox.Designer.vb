Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
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
            Me.LabelCompanyName = New System.Windows.Forms.Label
            Me.TextBoxDescription = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'OKButton
            '
            Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.OKButton.Location = New System.Drawing.Point(346, 34)
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
            Me.LabelProductName.Size = New System.Drawing.Size(87, 13)
            Me.LabelProductName.TabIndex = 4
            Me.LabelProductName.Text = "Product Name"
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
            Me.LabelCopyright.Size = New System.Drawing.Size(60, 13)
            Me.LabelCopyright.TabIndex = 2
            Me.LabelCopyright.Text = "Copyright"
            Me.LabelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'LabelCompanyName
            '
            Me.LabelCompanyName.AutoSize = True
            Me.LabelCompanyName.BackColor = System.Drawing.Color.Transparent
            Me.LabelCompanyName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.LabelCompanyName.Location = New System.Drawing.Point(10, 119)
            Me.LabelCompanyName.Margin = New System.Windows.Forms.Padding(7, 0, 3, 0)
            Me.LabelCompanyName.MaximumSize = New System.Drawing.Size(0, 17)
            Me.LabelCompanyName.Name = "LabelCompanyName"
            Me.LabelCompanyName.Size = New System.Drawing.Size(94, 13)
            Me.LabelCompanyName.TabIndex = 3
            Me.LabelCompanyName.Text = "Company Name"
            Me.LabelCompanyName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'TextBoxDescription
            '
            Me.TextBoxDescription.BackColor = System.Drawing.Color.Transparent
            Me.TextBoxDescription.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.TextBoxDescription.Location = New System.Drawing.Point(10, 151)
            Me.TextBoxDescription.Name = "TextBoxDescription"
            Me.TextBoxDescription.Size = New System.Drawing.Size(423, 112)
            Me.TextBoxDescription.TabIndex = 6
            Me.TextBoxDescription.Text = "Label1"
            '
            'AboutBox
            '
            Me.AcceptButton = Me.OKButton
            Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.BackgroundImage = Global.My.Resources.Resources.king_worship1
            Me.ClientSize = New System.Drawing.Size(448, 262)
            Me.Controls.Add(Me.TextBoxDescription)
            Me.Controls.Add(Me.LabelProductName)
            Me.Controls.Add(Me.LabelVersion)
            Me.Controls.Add(Me.LabelCopyright)
            Me.Controls.Add(Me.LabelCompanyName)
            Me.Controls.Add(Me.OKButton)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "AboutBox"
            Me.Padding = New System.Windows.Forms.Padding(10, 9, 10, 9)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
            Me.Text = "AboutBox"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents OKButton As System.Windows.Forms.Button
        Friend WithEvents LabelProductName As System.Windows.Forms.Label
        Friend WithEvents LabelVersion As System.Windows.Forms.Label
        Friend WithEvents LabelCopyright As System.Windows.Forms.Label
        Friend WithEvents LabelCompanyName As System.Windows.Forms.Label
        Friend WithEvents TextBoxDescription As System.Windows.Forms.Label

    End Class
End Namespace