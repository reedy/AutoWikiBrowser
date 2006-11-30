Imports System.Drawing

Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend Enum Colour
        Red
        Green
        Blue
    End Enum
    ''' <summary>
    ''' A simple "LED" user control
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class LED
        Private br As Brush = Brushes.Red, col As Colour = Colour.Red

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)
            e.Graphics.FillEllipse(br, 2, 2, Me.Size.Width - 2, Me.Size.Height - 2)
        End Sub
        Public Property Colour() As Colour
            Get
                Return col
            End Get
            Set(ByVal value As Colour)
                Select Case col
                    Case Colour.Blue
                        br = Brushes.Blue
                        col = Colour.Blue
                    Case Kingbotk.Components.Colour.Green
                        br = Brushes.LimeGreen
                        col = Colour.Green
                    Case Kingbotk.Components.Colour.Red
                        br = Brushes.Red
                        col = Colour.Red
                End Select

                MyBase.Refresh()
            End Set
        End Property
    End Class
End Namespace