Public Class ErrorForm

    Public Sub New(ByVal errorMessage As String)
        InitializeComponent()

        lblError.Text = errorMessage
    End Sub
End Class