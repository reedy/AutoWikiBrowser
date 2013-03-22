Namespace AutoWikiBrowser.Plugins.Kingbotk.Components
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class TimerStats
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
            Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
            Me.TimerLabel = New System.Windows.Forms.Label
            Me.SpeedLabel = New System.Windows.Forms.Label
            Me.EditsLabel = New System.Windows.Forms.Label
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            Me.SuspendLayout()
            '
            'Timer1
            '
            Me.Timer1.Interval = 1000
            '
            'TimerLabel
            '
            Me.TimerLabel.AutoSize = True
            Me.TimerLabel.Location = New System.Drawing.Point(3, 9)
            Me.TimerLabel.Name = "TimerLabel"
            Me.TimerLabel.Size = New System.Drawing.Size(49, 13)
            Me.TimerLabel.TabIndex = 0
            Me.TimerLabel.Text = "00:00:00"
            Me.ToolTip1.SetToolTip(Me.TimerLabel, "Time running")
            '
            'SpeedLabel
            '
            Me.SpeedLabel.AutoSize = True
            Me.SpeedLabel.Location = New System.Drawing.Point(3, 30)
            Me.SpeedLabel.Name = "SpeedLabel"
            Me.SpeedLabel.Size = New System.Drawing.Size(45, 13)
            Me.SpeedLabel.TabIndex = 1
            Me.SpeedLabel.Text = "0 saved"
            Me.ToolTip1.SetToolTip(Me.SpeedLabel, "Seconds per save")
            '
            'EditsLabel
            '
            Me.EditsLabel.AutoSize = True
            Me.EditsLabel.Location = New System.Drawing.Point(3, 51)
            Me.EditsLabel.Name = "EditsLabel"
            Me.EditsLabel.Size = New System.Drawing.Size(13, 13)
            Me.EditsLabel.TabIndex = 2
            Me.EditsLabel.Text = "0"
            Me.ToolTip1.SetToolTip(Me.EditsLabel, "Number of pages saved")
            '
            'TimerStats
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.EditsLabel)
            Me.Controls.Add(Me.SpeedLabel)
            Me.Controls.Add(Me.TimerLabel)
            Me.MaximumSize = New System.Drawing.Size(63, 70)
            Me.Name = "TimerStats"
            Me.Size = New System.Drawing.Size(63, 70)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents Timer1 As System.Windows.Forms.Timer
        Private WithEvents TimerLabel As System.Windows.Forms.Label
        Private WithEvents SpeedLabel As System.Windows.Forms.Label
        Private WithEvents EditsLabel As System.Windows.Forms.Label
        Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip

    End Class
End Namespace