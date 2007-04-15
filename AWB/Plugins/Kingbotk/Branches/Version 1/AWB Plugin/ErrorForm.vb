Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend Class ErrorForm

        Public Sub New(ByVal errorMessage As String)
            InitializeComponent()

            lblError.Text = errorMessage
        End Sub
    End Class
End Namespace