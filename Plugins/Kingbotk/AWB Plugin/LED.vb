Imports System.Drawing

Namespace AWB.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class LED
        Private br As Brush = Brushes.Red

        Private Sub LED_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
            'Dim br As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.Percent90, Color.White, Color.Red)
            e.Graphics.FillEllipse(br, 2, 2, Me.Size.Width - 2, Me.Size.Height - 2)
            'br.Dispose()
        End Sub
        Public Sub Green()
            br = Brushes.LimeGreen
            Me.Refresh()
        End Sub
        Public Sub Red()
            br = Brushes.Red
            Me.Refresh()
        End Sub
    End Class
End Namespace