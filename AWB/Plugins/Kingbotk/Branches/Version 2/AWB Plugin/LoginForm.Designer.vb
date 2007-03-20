Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class LoginForm
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub
        Friend WithEvents LogoPictureBox As System.Windows.Forms.PictureBox
        Friend WithEvents UsernameLabel As System.Windows.Forms.Label
        Friend WithEvents PasswordLabel As System.Windows.Forms.Label
        Friend WithEvents UsernameTextBox As System.Windows.Forms.TextBox
        Friend WithEvents PasswordTextBox As System.Windows.Forms.TextBox
        Friend WithEvents OK As System.Windows.Forms.Button

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(LoginForm))
            Me.LogoPictureBox = New System.Windows.Forms.PictureBox
            Me.UsernameLabel = New System.Windows.Forms.Label
            Me.PasswordLabel = New System.Windows.Forms.Label
            Me.UsernameTextBox = New System.Windows.Forms.TextBox
            Me.PasswordTextBox = New System.Windows.Forms.TextBox
            Me.OK = New System.Windows.Forms.Button
            Me.CheckBox1 = New System.Windows.Forms.CheckBox
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'LogoPictureBox
            '
            Me.LogoPictureBox.Image = CType(resources.GetObject("LogoPictureBox.Image"), System.Drawing.Image)
            Me.LogoPictureBox.Location = New System.Drawing.Point(0, 0)
            Me.LogoPictureBox.Name = "LogoPictureBox"
            Me.LogoPictureBox.Size = New System.Drawing.Size(165, 193)
            Me.LogoPictureBox.TabIndex = 0
            Me.LogoPictureBox.TabStop = False
            '
            'UsernameLabel
            '
            Me.UsernameLabel.Location = New System.Drawing.Point(172, 24)
            Me.UsernameLabel.Name = "UsernameLabel"
            Me.UsernameLabel.Size = New System.Drawing.Size(220, 23)
            Me.UsernameLabel.TabIndex = 0
            Me.UsernameLabel.Text = "&User name"
            Me.UsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'PasswordLabel
            '
            Me.PasswordLabel.Location = New System.Drawing.Point(172, 81)
            Me.PasswordLabel.Name = "PasswordLabel"
            Me.PasswordLabel.Size = New System.Drawing.Size(220, 23)
            Me.PasswordLabel.TabIndex = 2
            Me.PasswordLabel.Text = "&Password"
            Me.PasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'UsernameTextBox
            '
            Me.UsernameTextBox.Location = New System.Drawing.Point(174, 44)
            Me.UsernameTextBox.Name = "UsernameTextBox"
            Me.UsernameTextBox.Size = New System.Drawing.Size(220, 20)
            Me.UsernameTextBox.TabIndex = 1
            '
            'PasswordTextBox
            '
            Me.PasswordTextBox.Location = New System.Drawing.Point(174, 101)
            Me.PasswordTextBox.Name = "PasswordTextBox"
            Me.PasswordTextBox.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
            Me.PasswordTextBox.Size = New System.Drawing.Size(220, 20)
            Me.PasswordTextBox.TabIndex = 3
            '
            'OK
            '
            Me.OK.Location = New System.Drawing.Point(300, 157)
            Me.OK.Name = "OK"
            Me.OK.Size = New System.Drawing.Size(94, 23)
            Me.OK.TabIndex = 4
            Me.OK.Text = "&OK"
            '
            'CheckBox1
            '
            Me.CheckBox1.AutoSize = True
            Me.CheckBox1.Checked = True
            Me.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked
            Me.CheckBox1.Location = New System.Drawing.Point(240, 127)
            Me.CheckBox1.Name = "CheckBox1"
            Me.CheckBox1.Size = New System.Drawing.Size(154, 17)
            Me.CheckBox1.TabIndex = 6
            Me.CheckBox1.Text = "Save in registry (encrypted)"
            Me.ToolTip1.SetToolTip(Me.CheckBox1, "Save the username and password in the system registry. The information will NOT b" & _
                    "e saved in any" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "XML file or used for any purpose other than logging you in.")
            Me.CheckBox1.UseVisualStyleBackColor = True
            '
            'LoginForm
            '
            Me.AcceptButton = Me.OK
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(401, 192)
            Me.ControlBox = False
            Me.Controls.Add(Me.CheckBox1)
            Me.Controls.Add(Me.OK)
            Me.Controls.Add(Me.PasswordTextBox)
            Me.Controls.Add(Me.UsernameTextBox)
            Me.Controls.Add(Me.PasswordLabel)
            Me.Controls.Add(Me.UsernameLabel)
            Me.Controls.Add(Me.LogoPictureBox)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "LoginForm"
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "LoginForm"
            CType(Me.LogoPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
        Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip

    End Class
End Namespace