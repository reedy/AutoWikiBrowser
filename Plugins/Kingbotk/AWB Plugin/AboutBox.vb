Namespace AutoWikiBrowser.Plugins.SDKSoftware.Kingbotk.Components
    Friend NotInheritable Class AboutBox
        Private Sub AboutBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.LabelCopyright.Text = "Copyright © SDK Software 2006"
            Me.LabelCompanyName.Text = "Written by Kingboyk"
            Me.TextBoxDescription.Text = _
               "An AWB plugin for adding and updating WikiProject templates on Wikipedia talk pages. " & _
               Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & "AWB Version: " & _
               Application.ProductVersion.ToString & Microsoft.VisualBasic.vbCrLf & Microsoft.VisualBasic.vbCrLf & _
               "Made in England. Store in a dry place and consume within 7 days of opening. COMES WITH NO WARRANTY - " _
               & "check your edits and use sensibly!"
            Me.LabelProductName.Text = "Kingbotk Templating Plugin"
            Me.Text = "About the Kingbotk Templating Plugin"
        End Sub

        Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
            Me.Close()
        End Sub

        Public Sub New(ByVal Version As String)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.LabelVersion.Text = Version
        End Sub
    End Class
End Namespace